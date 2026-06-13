// SPDX-License-Identifier: Apache-2.0
using Google.Protobuf.WellKnownTypes;

using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Exceptions;
using Hiero.SDK.File;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Logging;
using Hiero.SDK.Networking;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;

namespace Hiero.SDK
{
	/// <include file="Client.cs.xml" path='docs/member[@name="T:Client"]' />
	public sealed partial class Client : IDisposable
    {
		private readonly bool ShouldShutdownExecutor;
		internal readonly ExecutorService Executor;
        private long _GrpcDeadline_Ticks = (long)DEFAULT_GRPC_DEADLINE.TotalTicks;
        private readonly HashSet<SubscriptionHandle> Subscriptions = [];

        private Task? NetworkUpdateFuture;
        private CancellationTokenSource? NetworkUpdateFutureCancellationTokenSource;
        
        /// <include file="Client.cs.xml" path='docs/member[@name="M:Client.#ctor(ExecutorService,Network,MirrorNetwork,System.NodaTime.Duration,System.Boolean,System.NodaTime.Duration,System.Int64,System.Int64)"]' />
        internal Client(ExecutorService executor, Network network, MirrorNetwork mirrorNetwork, NodaTime.Duration? networkUpdateInitialDelay, bool shouldShutdownExecutor, NodaTime.Duration? networkUpdatePeriod, long shard, long realm)
        {
            Executor = executor;
            Network_ = network;
            MirrorNetwork_ = mirrorNetwork;
            ShouldShutdownExecutor = shouldShutdownExecutor;
            NetworkUpdatePeriod = networkUpdatePeriod;
            Shard = shard;
            Realm = realm;

            ScheduleNetworkUpdate(networkUpdateInitialDelay);
        }

		public Logger Logger_ { get; set; } = new (LogLevel.Silent);
		public Network Network_ { get; internal set; }
		public MirrorNetwork MirrorNetwork_ { get; internal set; }

		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_2"]' />
		public NodeAddressBook NetworkFromAddressBook
		{
			set
			{
				lock (this)
				{
					Network_.SetNetwork(Network.AddressBookToNetwork(value.NodeAddresses));
					Network_.SetAddressBook(value);
				}
			}
		}

		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_3"]' />
		public bool AutoValidateChecksums
		{
			get { lock (this) return field; }
			set
			{
				lock (this)
				{
					field = value;
				}
			}
		}
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_4"]' />
		public bool DefaultRegenerateTransactionId
		{
			get { lock (this) return field; }
			set
			{
				lock (this)
				{
					field = value;
				}
			}
		} = true;
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.GetRestBaseUrl"]' />
		public string MirrorRestBaseUrl
		{
			get
			{
				try
				{
					return MirrorNetwork_.GetRestBaseUrl();
				}
				catch (ThreadInterruptedException e)
				{
					Thread.CurrentThread.Interrupt();
					throw new InvalidOperationException("Interrupted while retrieving mirror base URL", e);
				}
			}
		}
		/// <include file="Client.cs.xml" path='docs/member[@name="T:Client_2"]' />
		public bool MirrorTransportSecurity
		{
			// No-op setter preserved for API compatibility
			set { /* intentionally ignored */ }
		}
		/// <include file="Client.cs.xml" path='docs/member[@name="T:Client_3"]' />
		public bool MirrorTransportSecurityEnabled
		{
			get => MirrorNetwork_.TransportSecurity;
		}
		/// <include file="Client.cs.xml" path='docs/member[@name="T:Client_4"]' />
		public bool TransportSecurity
		{
			get => Network_.TransportSecurity;
			set => Network_.TransportSecurity = value;
		}
		/// <include file="Client.cs.xml" path='docs/member[@name="T:Client_5"]' />
		public bool VerifyCertificates
		{
			get => Network_.VerifyCertificates;
			set => Network_.VerifyCertificates = value;
		}

		/// <include file="Client.cs.xml" path='docs/member[@name="P:Client.Realm"]' />
		public long Realm { get; private set; }
		/// <include file="Client.cs.xml" path='docs/member[@name="P:Client.Shard"]' />
		public long Shard { get; private set; }

		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_5"]' />
		public int MaxAttempts
		{
			get { lock (this) return field; }
			set
			{
				lock (this)
				{
					if (value <= 0)
					{
						throw new ArgumentException("MaxAttempts must be greater than zero");
					}

					field = value;
				}
			}
		} = DEFAULT_MAX_ATTEMPTS;
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_6"]' />
		public int MaxNodeAttempts
		{
			get
			{
				lock (this)
				{
					return Network_.MaxNodeAttempts;
				}
			}
			set
			{
				lock (this) Network_.MaxNodeAttempts = value;
			}
		}
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.FromTicks(Volatile.Read(ref)"]' />
		public NodaTime.Duration MinBackoff
		{
			get => NodaTime.Duration.FromTicks(Volatile.Read(ref _MinBackoff));
			set
			{
				if (value.TotalNanoseconds < 0)
				{
					throw new ArgumentException("MinBackoff must be a positive duration");
				}

				if (value.TotalTicks.CompareTo(_MaxBackoff) > 0)
				{
					throw new ArgumentException("MinBackoff must be less than or equal to MaxBackoff");
				}

				Volatile.Write(ref _MinBackoff, (long)value.TotalTicks);
			}

		} private long _MinBackoff = (long)DEFAULT_MIN_BACKOFF.TotalTicks;
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.FromTicks(Volatile.Read(ref)_2"]' />
		public NodaTime.Duration MaxBackoff
		{
			get => NodaTime.Duration.FromTicks(Volatile.Read(ref _MaxBackoff));
			set
			{
				if (value.TotalNanoseconds < 0)
				{
					throw new ArgumentException("MaxBackoff must be a positive duration");
				}

				if (value.TotalTicks.CompareTo(_MinBackoff) < 0)
				{
					throw new ArgumentException("MaxBackoff must be greater than or equal to MinBackoff");
				}

				Volatile.Write(ref _MaxBackoff, (long)value.TotalTicks);
			}

		} private long _MaxBackoff = (long)DEFAULT_MAX_BACKOFF.TotalTicks;
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_7"]' />
		public NodaTime.Duration NodeMinBackoff
		{
			get
			{
				lock (this)
				{
					return Network_.MinNodeBackoff;
				}
			}
			set
			{
				lock (this) Network_.MinNodeBackoff = value;
			}
		}
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_8"]' />
		public NodaTime.Duration NodeMaxBackoff
		{
			get
			{
				lock (this)
				{
					return Network_.MaxNodeBackoff;
				}
			}
			set
			{
				lock (this) Network_.MaxNodeBackoff = value;
			}
		}
		/// <include file="Client.cs.xml" path='docs/member[@name="T:Client_6"]' />
		public NodaTime.Duration MinNodeReadmitTime
		{
			get => Network_.MinNodeReadmitTime;
			set => Network_.MinNodeReadmitTime = value;
		}
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_9"]' />
		public NodaTime.Duration MaxNodeReadmitTime
		{
			get => Network_.MaxNodeReadmitTime;
			set => Network_.MaxNodeReadmitTime = value;
		}

		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_10"]' />
		public Operator Operator_
		{
			private set;
			get
			{
				lock (this)
				{
					return field;
				}
			}
		}
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_11"]' />
		public AccountId OperatorAccountId
		{
			get { lock (this) return Operator_.AccountId; }
			set
			{
				lock (this)
				{
					Operator_.AccountId = value;
				}
			}
		}
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_12"]' />
		public PublicKey OperatorPublicKey
		{
			get { lock (this) return Operator_.PublicKey; }
			set
			{
				lock (this)
				{
					Operator_.PublicKey = value;
				}
			}
		}

		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.OperatorSet(AccountId,PrivateKey)"]' />
		public Client OperatorSet(AccountId accountId, PrivateKey privateKey)
		{
			lock (this) OperatorSetWith(accountId, privateKey.GetPublicKey(), privateKey.Sign);

			return this;
		}
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.OperatorSetWith(AccountId,PublicKey,System.Func{System.Byte[],System.Byte[]})"]' />
		public Client OperatorSetWith(AccountId accountId, PublicKey publicKey, Func<byte[], byte[]> transactionSigner)
		{
			lock (this)
			{
				try
				{
					accountId.ValidateChecksum(this);
				}
				catch (BadEntityIdException exc)
				{
					throw new ArgumentException("Tried to set the client oper8r account ID to an account ID with an invalid checksum: " + exc.Message);
				}


				Operator_ = new Operator(accountId, publicKey, transactionSigner);
			}

			return this;
		}

		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_13"]' />
		public Hbar? DefaultMaxTransactionFee
		{
			get { lock (this) return field; }
			set
			{
				lock (this)
				{
					ArgumentNullException.ThrowIfNull(value);

					if (value.ToTinybars() < 0)
					{
						throw new ArgumentException("MaxTransactionFee must be non-negative");
					}

					field = value;
				}
			}
		}
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_14"]' />
		public Hbar DefaultMaxQueryPayment
		{
			get { lock (this) return field; }
			set
			{
				lock (this)
				{
					ArgumentNullException.ThrowIfNull(value);

					if (value.ToTinybars() < 0)
					{
						throw new ArgumentException("DefaultMaxQueryPayment must be non-negative");
					}

					field = value;
				}
			}

		} = DEFAULT_MAX_QUERY_PAYMENT;

		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_15"]' />
		public NodaTime.Duration RequestTimeout
		{
			get { lock (this) return field; }
			set { lock (this) field = value; }

		} = DEFAULT_REQUEST_TIMEOUT;
		/// <include file="Client.cs.xml" path='docs/member[@name="T:Client_7"]' />
		public NodaTime.Duration CloseTimeout
		{
			get => field;
			set
			{
				field = value;
				Network_.CloseTimeout = value;
				MirrorNetwork_.CloseTimeout = value;
			}

		} = DEFAULT_CLOSE_TIMEOUT;
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.Write(_GrpcDeadline_Ticks@,value.)"]' />
		public NodaTime.Duration GrpcDeadline
		{
			set => Volatile.Write(ref _GrpcDeadline_Ticks, (long)value.TotalTicks);
			get => NodaTime.Duration.FromTicks(Volatile.Read(ref _GrpcDeadline_Ticks));
		}
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.lock(this)_16"]' />
		public NodaTime.Duration? NetworkUpdatePeriod
		{
			get { lock (this) return field; }
			set
			{
				lock (this)
				{
					CancelScheduledNetworkUpdate();
					field = value;

					ScheduleNetworkUpdate(field);
				}
			}
		}

		internal void ScheduleNetworkUpdate(NodaTime.Duration? delay)
		{
			lock (this)
			{
				if (delay == null)
				{
					NetworkUpdateFuture = null;
					return;
				}
				
				NetworkUpdateFuture = new Delayer(Executor)
					.DelayAsync(delay.Value, () =>
					{
						// Checking NetworkUpdatePeriod != null must be synchronized, so I've put it in a synchronized method.
						RequireNetworkUpdatePeriodNotNull(async () =>
						{
							var fileId = FileId.GetAddressBookFileIdFor(Shard, Realm);

							await RequireNetworkUpdatePeriodNotNull(async () =>
							{
								try
								{
									NetworkFromAddressBook = await new AddressBookQuery { FileId = fileId, }.ExecuteAsync(this);
								}
								catch (Exception) { }
							});

							ScheduleNetworkUpdate(NetworkUpdatePeriod);
						});
					});
			}
		}
		internal void CancelAllSubscriptions()
		{
			foreach (var subscription in Subscriptions)
				subscription.Unsubscribe(); 
		}
		internal void CancelScheduledNetworkUpdate()
		{
			NetworkUpdateFutureCancellationTokenSource?.Cancel(true);
		}
		internal void TrackSubscription(SubscriptionHandle subscriptionHandle)
		{
			Subscriptions.Add(subscriptionHandle);
		}
		internal void UntrackSubscription(SubscriptionHandle subscriptionHandle)
		{
			Subscriptions.Remove(subscriptionHandle);
		}
		internal Task RequireNetworkUpdatePeriodNotNull(Func<Task> task)
		{
			lock (this)
			{
				return NetworkUpdatePeriod != null ? task() : Task.CompletedTask;
			}
		}
		internal Task<T?> RequireNetworkUpdatePeriodNotNull<T>(Func<Task<T?>> task)
		{
			lock (this)
			{
				return NetworkUpdatePeriod != null ? task() : Task.FromResult<T?>(default);
			}
		}

		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.UpdateNetworkFromAddressBook"]' />
		public Client UpdateNetworkFromAddressBook()
		{
			lock (this)
			{
				try
				{
					var fileId = FileId.GetAddressBookFileIdFor(Shard, Realm);
					Logger_.Debug("Fetching address book from file {0}", fileId);

					// Execute synchronously - no async complexity
					var addressBook = new AddressBookQuery { FileId = fileId, }.Execute(this); // ← Synchronous!
					Logger_.Debug("Received address book with {0} nodes", addressBook.NodeAddresses.Count);

					// Update the Network
					NetworkFromAddressBook = addressBook;

                    Logger_.Info("Address book update completed successfully");
				}
				catch (TimeoutException e)
				{
					Logger_.Warn("Failed to fetch address book: {0}", e.Message);
				}
				catch (Exception e)
				{
                    Logger_.Warn("Failed to update address book", e);
				}

				return this;
			}
		}

		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.Dispose"]' />
		public void Dispose()
        {
            lock (this)
            {
                Dispose(CloseTimeout);
            }
        }
		/// <include file="Client.cs.xml" path='docs/member[@name="M:Client.Dispose(System.NodaTime.Duration)"]' />
		public void Dispose(NodaTime.Duration timeout)
        {
            lock (this)
            {
				NetworkUpdatePeriod = null;
                CancelScheduledNetworkUpdate();
                CancelAllSubscriptions();
                Network_.BeginClose();
                MirrorNetwork_.BeginClose();

				var closeDeadline = timeout.ToInstant();

                var NetworkError = Network_.AwaitClose(closeDeadline, null).GetAwaiter().GetResult();
                var MirrorNetwork_Error = MirrorNetwork_.AwaitClose(closeDeadline, NetworkError).GetAwaiter().GetResult();

				if (ShouldShutdownExecutor && Executor != null)
				{
					try
					{
						Executor.Dispose();

						NodaTime.Duration waitTime = NodaTime.Duration.FromTicks(timeout.TotalTicks / 2);
					
						if (!Executor.WaitForTermination(waitTime))
						{
							Executor.ForceShutdown();

							if (!Executor.WaitForTermination(waitTime))
							{
								Logger_.Warn("Executor pool did not terminate in time");
							}
						}
					}
					catch (ThreadInterruptedException)
					{
						Executor.ForceShutdown();
						Thread.CurrentThread.Interrupt();
					}
				}

                if (MirrorNetwork_Error != null)
                {
                    if (MirrorNetwork_Error is TimeoutException)
						throw MirrorNetwork_Error;
					else
						throw new Exception(MirrorNetwork_Error.Message);
				}
            }
        }
    }
}