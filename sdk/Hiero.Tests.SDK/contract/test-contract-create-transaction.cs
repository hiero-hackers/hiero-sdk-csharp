// SPDX-License-Identifier: Apache-2.0
using System;

using Org.BouncyCastle.Utilities.Encoders;

using Google.Protobuf;

using Hiero.SDK.Cryptography;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Transactions;
using Hiero.SDK.Contract;
using Hiero.SDK.File;

using VerifyXunit;
using Hiero.SDK;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Contract
{
    /// <include file="test-contract-create-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Contract.ContractCreateTransactionTest"]' />
    public class ContractCreateTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        [Fact] public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }

        public virtual void ShouldSerialize2()
        {
            Verifier.Verify(SpawnTestTransaction2().ToString());
        }
        [Fact]
        /// <include file="test-contract-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractCreateTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new ContractCreateTransaction();
            var tx2 = Transaction.FromBytes<ContractCreateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }

        private ContractCreateTransaction SpawnTestTransaction()
        {
            return new ContractCreateTransaction
			{
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				BytecodeFileId = FileId.FromString("0.0.3003"),

				AdminKey = unusedPrivateKey,
				Gas = 0,
				InitialBalance = Hbar.FromTinybars(1000),
				StakedAccountId = AccountId.FromString("0.0.3"),
                MaxAutomaticTokenAssociations = 101,
				AutoRenewPeriod = NodaTime.Duration.FromHours(10),
				ConstructorParameters = ByteString.CopyFrom([10, 11, 12, 13, 25]),
				MaxTransactionFee = Hbar.FromTinybars(100000),
				AutoRenewAccountId = new AccountId(0, 0, 30),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }

        private ContractCreateTransaction SpawnTestTransaction2()
        {
            return new ContractCreateTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				Bytecode = Hex.Decode("deadbeef"),
				AdminKey = unusedPrivateKey,
				Gas = 0,
				InitialBalance = Hbar.FromTinybars(1000),
				StakedNodeId = 4,
				MaxAutomaticTokenAssociations = 101,
				AutoRenewPeriod = NodaTime.Duration.FromHours(10),
				ConstructorParameters = ByteString.CopyFrom([ 10, 11, 12, 13, 25 ]),
				MaxTransactionFee = Hbar.FromTinybars(100000),
				AutoRenewAccountId = new AccountId(0, 0, 30),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-contract-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractCreateTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<ContractCreateTransaction>(tx.ToBytes());
            
            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-contract-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractCreateTransactionTest.ShouldBytes2"]' />
        public virtual void ShouldBytes2()
        {
            var tx = SpawnTestTransaction2();
            var tx2 = Transaction.FromBytes<ContractCreateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-contract-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractCreateTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
                ContractCreateInstance = new Proto.Services.ContractCreateTransactionBody { }
            };
            var tx = Transaction.FromScheduledTransaction(transactionBody);
            Assert.IsType<ContractCreateTransaction>(tx);
        }
        [Fact]
        /// <include file="test-contract-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractCreateTransactionTest.SetGasShouldRejectNegativeValues"]' />
        public virtual void SetGasShouldRejectNegativeValues()
        {
            var tx = new ContractCreateTransaction();
            var ex = Assert.Throws<ArgumentException>(() => tx.Gas = -1);

            Assert.Contains("Gas must be non-negative", ex.Message);
        }
        [Fact]
        /// <include file="test-contract-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractCreateTransactionTest.SetGasShouldAcceptZeroAndPositiveValues"]' />
        public virtual void SetGasShouldAcceptZeroAndPositiveValues()
        {
            var tx = new ContractCreateTransaction
            {
                Gas = 0
            };
            Assert.Equal(0, tx.Gas);
            tx.Gas = 123456;
            Assert.Equal(123456, tx.Gas);
        }
    }
}
