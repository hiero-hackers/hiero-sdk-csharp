// SPDX-License-Identifier: Apache-2.0
using Google.Protobuf;
using Google.Protobuf.Reflection;

using Grpc.Core;

using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Exceptions;
using Hiero.SDK.Fee;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Schedule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Hiero.SDK.Transactions;

namespace Hiero.SDK.Core
{	
	/// <include file="Transaction.cs.xml" path='docs/member[@name="T:Transaction"]' />
	public abstract partial class Transaction<T> : Transaction where T : Transaction<T>
    {		
		/// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.#ctor_2"]' />
		protected Transaction() : base() { }
		/// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.#ctor(Proto.Services.TransactionBody)"]' />
		internal Transaction(Proto.Services.TransactionBody txBody) : base(txBody) { }
        /// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.#ctor(DictionaryLinked{TransactionId,DictionaryLinked{AccountId,Proto.Services.Transaction}})"]' />
        internal Transaction(DictionaryLinked<TransactionId, DictionaryLinked<AccountId, Proto.Services.Transaction>> txs) : base(txs) { }


        /// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.Batchify(Client,Key)"]' />
        public T Batchify(Client client, Key batchKey)
        {
            RequireNotFrozen();
            ArgumentNullException.ThrowIfNull(batchKey);
            this.BatchKey = batchKey;
            SignWithOperator(client);

            // noinspection unchecked
            return (T)this;
        }
        /// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.Sign(PrivateKey)"]' />
        public T Sign(PrivateKey privateKey)
        {
            return SignWith(privateKey.GetPublicKey(), privateKey.Sign);
        }

        public override TransactionId TransactionIdInternal
		{
			get => TransactionIds.Current;
		}

		protected override bool IsBatchedAndNotBatchTransaction()
		{
			return BatchKey != null && this is not BatchTransaction;
		}


        /// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.AddSignature(PublicKey,System.Byte[])"]' />
        public virtual T AddSignature(PublicKey publicKey, byte[] signature)
		{
			RequireOneNodeAccountId();

			if (!IsFrozen())
				Freeze();

			if (KeyAlreadySigned(publicKey))
			{
				// noinspection unchecked
				return (T)this;
			}

			TransactionIds.IsLocked = true;
			NodeAccountIds.IsLocked = true;

			for (int i = 0; i < OuterTransactions.Count; i++)
				OuterTransactions[i] = null;

			PublicKeys.Add(publicKey);
			Signers.Add(null);
			SigPairLists[0].SigPair.Add(publicKey.ToSignaturePairProtobuf(signature));

			// noinspection unchecked
			return (T)this;
		}
		/// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.AddSignature(PublicKey,System.Byte[],TransactionId,AccountId)"]' />
		public virtual T AddSignature(PublicKey publicKey, byte[] signature, TransactionId transactionID, AccountId nodeId)
		{
			if (InnerSignedTransactions.Count == 0)
			{
				// noinspection unchecked
				return (T)this;
			}

			TransactionIds.IsLocked = true;

			for (int index = 0; index < InnerSignedTransactions.Count; index++)
				if (ProcessedSignatureForTransaction(index, publicKey, signature, transactionID, nodeId))
					UpdateTransactionState(publicKey);


			// noinspection unchecked
			return (T)this;
		}		
		/// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.Freeze"]' />
		public virtual T Freeze()
		{
			return FreezeWith(null);
		}
		/// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.FreezeWith(Client)"]' />
		public virtual T FreezeWith(Client? client)
		{
			if (IsFrozen())
			{
				// noinspection unchecked
				return (T)this;
			}

			if (TransactionIds.Count == 0)
			{
				if (client != null)
				{
					var @operator = client.Operator_;

					if (@operator != null)
					{
						// Set a default transaction ID, generated from the operator account ID
						TransactionIds.Set(TransactionId.Generate(@operator.AccountId));
					}
					else
					{
						// no client means there must be an explicitly set node ID and transaction ID
						throw new InvalidOperationException("`client` must have an `operator` or `transactionId` must be set");
					}
				}
				else
				{
					throw new InvalidOperationException("Transaction ID must be set, or operator must be provided via freezeWith()");
				}
			}

			if (NodeAccountIds.Count == 0)
			{
				if (client == null)
					throw new InvalidOperationException("`client` must be provided or both `nodeId` and `transactionId` must be set");

				try
				{
					if (BatchKey == null)
						NodeAccountIds.Set(client.Network_.GetNodeAccountIdsForExecute());
					else 
						NodeAccountIds.Set(AccountId.FromString(Transaction.ATOMIC_BATCH_NODE_ACCOUNT_ID));
				}
				catch (ThreadInterruptedException e)
				{
					throw new Exception(string.Empty, e);
				}
			}

            FrozenBodyBuilder = SpawnBodyBuilder(client, builder =>
			{
				builder.TransactionId = TransactionIds[0].ToProtobuf();
            });

			OnFreeze(FrozenBodyBuilder);

			int requiredChunks = GetRequiredChunks();
			
			GenerateTransactionIds(TransactionIds[0], requiredChunks);
			WipeTransactionLists(requiredChunks);

			regenerateTransactionId = regenerateTransactionId != null ? regenerateTransactionId : client?.DefaultRegenerateTransactionId;

			// noinspection unchecked
			return (T)this;
		}

		public override TransactionResponse MapResponse(Proto.Services.TransactionResponse transactionResponse, AccountId nodeId, Proto.Services.Transaction request)
		{
			var transactionId = TransactionIdInternal;
			var hash = Transaction.GenerateHash(request.SignedTransactionBytes.ToByteArray());

			// advance is needed for chunked transactions
			TransactionIds.Advance();

			return TransactionResponse.Init(nodeId, transactionId, hash, null, this);
		}		
		public override ResponseStatus MapResponseStatus(Proto.Services.TransactionResponse transactionResponse)
		{
			return (ResponseStatus)transactionResponse.NodeTransactionPrecheckCode;
		}

		public virtual Transaction<T> RegenerateTransactionId(Client client)
		{
			ArgumentNullException.ThrowIfNull(client.OperatorAccountId);
			TransactionIds.IsLocked = false;
			var newTransactionId = TransactionId.Generate(client.OperatorAccountId);
			TransactionIds[TransactionIds.Index] = newTransactionId;
			TransactionIds.IsLocked = true;
			return this;
		}		
		/// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.SignWithOperator(Client)"]' />
		public virtual T SignWithOperator(Client client)
		{
			if (client.Operator_ == null)
				throw new InvalidOperationException("`client` must have an `operator` to sign with the operator");

			if (!IsFrozen())
				FreezeWith(client);

			return SignWith(client.Operator_.PublicKey, client.Operator_.TransactionSigner);
		}		
		/// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.SignWith(PublicKey,System.Func{System.Byte[],System.Byte[]})"]' />
		public virtual T SignWith(PublicKey publicKey, Func<byte[], byte[]> transactionSigner)
		{
			if (!IsFrozen())
			{
				throw new InvalidOperationException("Signing requires transaction to be frozen");
			}

			if (KeyAlreadySigned(publicKey))
			{

				// noinspection unchecked
				return (T)this;
			}

			for (int i = 0; i < OuterTransactions.Count; i++)
			{
				OuterTransactions[i] = null;
			}

			PublicKeys.Add(publicKey);
			Signers.Add(transactionSigner);

			// noinspection unchecked
			return (T)this;
		}
        /// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.SetNodeAccountIds(System.Collections.Generic.IEnumerable{AccountId})"]' />
        public virtual T SetNodeAccountIds(IEnumerable<AccountId> nodeaccountids)
        {
            RequireNotFrozen();

            NodeAccountIds.Set(nodeaccountids);

            return (T)this;
        }

        public override Method<Proto.Services.Transaction, Proto.Services.TransactionResponse> GetMethod()
		{
			MethodDescriptor methoddescriptor = GetMethodDescriptor();

			IMessage input = (IMessage)Activator.CreateInstance(methoddescriptor.InputType.ClrType)!;
			IMessage output = (IMessage)Activator.CreateInstance(methoddescriptor.OutputType.ClrType)!;

			return new Method<Proto.Services.Transaction, Proto.Services.TransactionResponse>(
				type: MethodType.Unary,
				name: methoddescriptor.Name,
				serviceName: methoddescriptor.Service.FullName,
				requestMarshaller: Marshallers.Create(r => r.ToByteArray(), data => Proto.Services.Transaction.Parser.ParseFrom(data)),
				responseMarshaller: Marshallers.Create(r => r.ToByteArray(), data => Proto.Services.TransactionResponse.Parser.ParseFrom(data)));
		}

		public override ExecutionState GetExecutionState(ResponseStatus status, Proto.Services.TransactionResponse response)
		{
			if (status == ResponseStatus.TransactionExpired)
			{
				if (regenerateTransactionId ?? false || TransactionIds.IsLocked)
					return ExecutionState.RequestError;
				else
				{
					var firstTransactionId = TransactionIds[0];
					var accountId = firstTransactionId.AccountId;

					GenerateTransactionIds(TransactionId.Generate(accountId), TransactionIds.Count);
					WipeTransactionLists(TransactionIds.Count);
					
					return ExecutionState.Retry;
				}
			}

			return base.GetExecutionState(status, response);
		}
		public override void OnExecute(Client client)
		{
			if (!IsFrozen())
				FreezeWith(client);

			var accountId = TransactionIds[0].AccountId;

			if (client.AutoValidateChecksums)
				try
				{
					accountId.ValidateChecksum(client);
					ValidateChecksums(client);
				}
				catch (BadEntityIdException exc)
				{
					throw new ArgumentException(exc.Message);
				}

			var operatorId = client.OperatorAccountId;

			if (operatorId != null && operatorId.Equals(accountId))
			{
				// on execute, sign each transaction with the operator, if present
				// and we are signing a transaction that used the default transaction ID
				SignWithOperator(client);
			}
		}
		public override Task OnExecuteAsync(Client client)
		{
			OnExecute(client);

			return Task.CompletedTask;
		}
        public override Proto.Services.Transaction MakeRequest()
        {
            var index = NodeAccountIds.Index + (TransactionIds.Index * NodeAccountIds.Count);

            BuildTransaction(index);
            return OuterTransactions[index];
        }
		public override string ToString()
		{
			// NOTE: regex is for removing the instance address from the default debug output
			Proto.Services.TransactionBody body = SpawnBodyBuilder(null);
			
			if (TransactionIds.Count != 0)
				body.TransactionId = TransactionIds[0].ToProtobuf();

			if (NodeAccountIds.Count != 0)
				body.NodeAccountId = NodeAccountIds[0].ToProtobuf();

			OnFreeze(body);

			return Regex.Replace(body.ToString(), "@[A-Za-z0-9]+", string.Empty);
		}
		
		/// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.AddSignatureIfNotExists(System.Int32,PublicKey,System.Byte[])"]' />
		private bool AddSignatureIfNotExists(int index, PublicKey publicKey, byte[] signature)
		{
			Proto.Services.SignatureMap sigMapBuilder = SigPairLists[index];

			// Check if the signature is already in the signature map
			if (IsSignatureAlreadyPresent(sigMapBuilder, publicKey))
				return false;

			// Add the signature to the signature map
			Proto.Services.SignaturePair newSigPair = publicKey.ToSignaturePairProtobuf(signature);
			sigMapBuilder.SigPair.Add(newSigPair);

			return true;
		}
		/// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.IsSignatureAlreadyPresent(Proto.Services.SignatureMap,PublicKey)"]' />
		private bool IsSignatureAlreadyPresent(Proto.Services.SignatureMap sigMapBuilder, PublicKey publicKey)
		{
			foreach (Proto.Services.SignaturePair sig in sigMapBuilder.SigPair)
                if (sig.PubKeyPrefix.ToByteArray().SequenceEqual(publicKey.ToBytesRaw()))
                    return true;

            return false;
		}
		/// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.MatchesTargetTransactionAndNode(Proto.Services.TransactionBody,TransactionId,AccountId)"]' />
		private bool MatchesTargetTransactionAndNode(Proto.Services.TransactionBody body, TransactionId targetTransactionID, AccountId targetNodeId)
        {
            TransactionId bodyTxId = TransactionId.FromProtobuf(body.TransactionId);
            AccountId bodyNodeId = AccountId.FromProtobuf(body.NodeAccountId);

            return bodyTxId.ToString().Equals(targetTransactionID.ToString()) && bodyNodeId.ToString().Equals(targetNodeId.ToString());
        }
		/// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.ProcessedSignatureForTransaction(System.Int32,PublicKey,System.Byte[],TransactionId,AccountId)"]' />
		private bool ProcessedSignatureForTransaction(int index, PublicKey publicKey, byte[] signature, TransactionId transactionID, AccountId nodeId)
		{
			Proto.Services.SignedTransaction temp = InnerSignedTransactions[index];
			Proto.Services.TransactionBody body = Transaction.ParseTransactionBody(temp);

			if (body == null)
				return false;

			if (!MatchesTargetTransactionAndNode(body, transactionID, nodeId))
				return false;

			return AddSignatureIfNotExists(index, publicKey, signature);
		}
		/// <include file="Transaction.cs.xml" path='docs/member[@name="M:Transaction.UpdateTransactionState(PublicKey)"]' />
		private void UpdateTransactionState(PublicKey publicKey)
        {
            PublicKeys.Add(publicKey);
            Signers.Add(null);
        }
    }
}
