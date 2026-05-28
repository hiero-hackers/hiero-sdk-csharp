// SPDX-License-Identifier: Apache-2.0
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Hiero.SDK.Core;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Cryptography;
using Hiero.SDK.File;
using Hiero.SDK.Hook;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hiero.SDK.Contract
{
    /// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="T:ContractUpdateTransaction"]' />
    public sealed class ContractUpdateTransaction : Transaction<ContractUpdateTransaction>
    {
		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.#ctor"]' />
		public ContractUpdateTransaction() { }
		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.#ctor(Proto.Services.TransactionBody)"]' />
		internal ContractUpdateTransaction(Proto.Services.TransactionBody txBody) : base(txBody)
		{
			InitFromTransactionBody();
		}
		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.#ctor(DictionaryLinked{TransactionId,DictionaryLinked{AccountId,Proto.Services.Transaction}})"]' />
		internal ContractUpdateTransaction(DictionaryLinked<TransactionId, DictionaryLinked<AccountId, Proto.Services.Transaction>> txs) : base(txs)
        {
            InitFromTransactionBody();
        }

		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.RequireNotFrozen"]' />
		public ContractId? ContractId
		{
			get;
			set
			{
				RequireNotFrozen();
				field = value;
			}
		}
		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.RequireNotFrozen_2"]' />
		public NodaTime.Instant? ExpirationTime
		{
			get;
			set
			{
				RequireNotFrozen();
				field = value;
                if (ExpirationTimeDuration is not null)
                    ExpirationTimeDuration = null;
            }
		}
		public NodaTime.Duration? ExpirationTimeDuration
		{
			get;
			set
			{
				RequireNotFrozen();
				field = value;
                if (ExpirationTime is not null)
                    ExpirationTime = null;
            }
		}
		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="T:ContractUpdateTransaction_2"]' />
		public Key? AdminKey
		{
			get;
			set;
		}
		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.RequireNotFrozen_3"]' />
		public AccountId? ProxyAccountId
		{
			get;
			set
			{
				RequireNotFrozen();
				field = value;
			}
		}
		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.RequireNotFrozen_4"]' />
		public int? MaxAutomaticTokenAssociations
		{
			get;
			set
			{
				RequireNotFrozen();
				field = value;
			}
		}
		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.RequireNotFrozen_5"]' />
		public NodaTime.Duration? AutoRenewPeriod
		{
			get;
			set
			{
				RequireNotFrozen();
				field = value;
			}
		}
		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.RequireNotFrozen_6"]' />
		public FileId? BytecodeFileId
		{
			get;
			set
			{
				RequireNotFrozen();
				field = value;
			}
		}
		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.RequireNotFrozen_7"]' />
		public string? ContractMemo
		{
			get;
			set
			{
				RequireNotFrozen();
				field = value;
			}
		}
		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.RequireNotFrozen_8"]' />
		public AccountId? StakedAccountId
		{
			get;
			set
			{
				RequireNotFrozen();
				field = value;
                if (StakedNodeId is not null) StakedNodeId = null;
			}
		}
		public long? StakedNodeId
		{
			get;
			set
			{
				RequireNotFrozen();
				field = value;
                if (StakedAccountId is not null) StakedAccountId = null;
			}
		}
		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.RequireNotFrozen_9"]' />
		public bool? DeclineStakingReward
		{
			get;
			set
			{
				RequireNotFrozen();
				field = value;
			}
		}
		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.RequireNotFrozen_10"]' />
		public AccountId? AutoRenewAccountId
		{
			get;
			set
			{
				RequireNotFrozen();
				field = value;
			}
		}
		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="T:ContractUpdateTransaction_3"]' />
		public ListGuarded<HookCreationDetails> HookCreationDetails
        {
            init => field = GenerateListGuarded(value);
            internal get => field ??= GenerateListGuarded(field);
        }

        /// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.InitFromTransactionBody"]' />
        public ListGuarded<long> HookIdsToDelete
        {
            init => field = GenerateListGuarded(value);
            internal get => field ??= GenerateListGuarded(field);
        }

        /// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.InitFromTransactionBody_2"]' />
        void InitFromTransactionBody()
        {
            var body = SourceTransactionBody.ContractUpdateInstance;

            ContractId = body.ContractId is null? null : ContractId.FromProtobuf(body.ContractId);
            ProxyAccountId = body.ProxyAccountId is null ? null : AccountId.FromProtobuf(body.ProxyAccountId);
            StakedAccountId = body.StakedAccountId is null ? null : AccountId.FromProtobuf(body.StakedAccountId);
            AutoRenewAccountId = body.AutoRenewAccountId is null ? null : AccountId.FromProtobuf(body.AutoRenewAccountId);

            if (body.AdminKey is not null)
				AdminKey = Key.FromProtobufKey(body.AdminKey);

            if (body.HasStakedNodeId) 
				StakedNodeId = body.StakedNodeId;

            AutoRenewPeriod = body.AutoRenewPeriod?.ToNodaDuration();
            ExpirationTime = body.ExpirationTime?.ToNodaTimeInstant();

            ContractMemo = body.MemoWrapper;
            DeclineStakingReward = body.DeclineReward;
            MaxAutomaticTokenAssociations = body.MaxAutomaticTokenAssociations;

            HookIdsToDelete.Set(body.HookIdsToDelete);
            HookCreationDetails.Set(body.HookCreationDetails.Select(_ => Hook.HookCreationDetails.FromProtobuf(_)));
        }

		/// <include file="ContractUpdateTransaction.cs.xml" path='docs/member[@name="M:ContractUpdateTransaction.ToProtobuf"]' />
		public Proto.Services.ContractUpdateTransactionBody ToProtobuf()
        {
            var builder = new Proto.Services.ContractUpdateTransactionBody { };

            if (ContractId != null)
				builder.ContractId = ContractId.ToProtobuf();

            if (ProxyAccountId != null)
				builder.ProxyAccountId = ProxyAccountId.ToProtobuf();

            if (ExpirationTime != null)
				builder.ExpirationTime = ExpirationTime.Value.ToProtoTimestamp();

            if (ExpirationTimeDuration != null)
				builder.ExpirationTime = ExpirationTimeDuration.Value.ToProtoTimestamp();

            if (AdminKey != null)
				builder.AdminKey = AdminKey.ToProtobufKey();

            if (MaxAutomaticTokenAssociations != null)
				builder.MaxAutomaticTokenAssociations = MaxAutomaticTokenAssociations;

			if (AutoRenewPeriod != null)
				builder.AutoRenewPeriod = AutoRenewPeriod.Value.ToProtoDuration();

            if (ContractMemo != null)
				builder.MemoWrapper = ContractMemo;

            if (StakedAccountId != null)
				builder.StakedAccountId = StakedAccountId.ToProtobuf();

            if (StakedNodeId != null)
				builder.StakedNodeId = StakedNodeId.Value;

            if (DeclineStakingReward != null)
				builder.DeclineReward = DeclineStakingReward.Value;

			if (HookIdsToDelete.Count != 0)
				builder.HookIdsToDelete.AddRange(HookIdsToDelete);

			if (AutoRenewAccountId != null)
			{
				if (AutoRenewAccountId.ToString().Equals("0.0.0"))
					builder.AutoRenewAccountId = new Proto.Services.AccountID { };
				else
					builder.AutoRenewAccountId = AutoRenewAccountId.ToProtobuf();
			}

			builder.HookCreationDetails.AddRange(HookCreationDetails.Select(_ => _.ToProtobuf()));

			return builder;
        }

        public override void ValidateChecksums(Client client)
        {
            ContractId?.ValidateChecksum(client);
            ProxyAccountId?.ValidateChecksum(client);
            StakedAccountId?.ValidateChecksum(client);
            AutoRenewAccountId?.ValidateChecksum(client);
        }
		public override void OnFreeze(Proto.Services.TransactionBody bodyBuilder)
        {
            bodyBuilder.ContractUpdateInstance = ToProtobuf();
        }
        public override void OnScheduled(Proto.Services.SchedulableTransactionBody scheduled)
        {
            scheduled.ContractUpdateInstance = ToProtobuf();
        }

		public override MethodDescriptor GetMethodDescriptor()
		{
			string methodname = nameof(Proto.Services.SmartContractService.SmartContractServiceClient.updateContract);

			return Proto.Services.SmartContractService.Descriptor.FindMethodByName(methodname);
		}
    }
}
