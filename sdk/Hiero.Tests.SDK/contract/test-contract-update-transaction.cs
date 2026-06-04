// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK;
using Hiero.SDK.Core;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Contract;
using Hiero.SDK.Transactions;
using Hiero.SDK.Cryptocurrency;

using VerifyXunit;
using NodaTime;

namespace Hiero.Tests.SDK.Contract
{
    /// <include file="test-contract-update-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Contract.ContractUpdateTransactionTest"]' />
    public class ContractUpdateTransactionTest
    {
        private static readonly PrivateKey privateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        [Fact] 
        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }
        [Fact]
        public virtual void ShouldSerialize2()
        {
            Verifier.Verify(SpawnTestTransaction2().ToString());
        }
        [Fact]
        /// <include file="test-contract-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractUpdateTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new ContractUpdateTransaction();
            var tx2 = Transaction.FromBytes(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }

        private ContractUpdateTransaction SpawnTestTransaction()
        {
            return new ContractUpdateTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				ContractId = ContractId.FromString("0.0.5007"),
				AdminKey = privateKey,
				MaxAutomaticTokenAssociations = 101,
				AutoRenewPeriod = NodaTime.Duration.FromDays(1),
				ContractMemo = "3",
				StakedAccountId = AccountId.FromString("0.0.3"),
				ExpirationTime = NodaTime.Instant.FromUnixTimeMilliseconds(4),
				ProxyAccountId = new AccountId(0, 0, 4),
				MaxTransactionFee = Hbar.FromTinybars(100000),
				AutoRenewAccountId = new AccountId(0, 0, 30),
			}
            .Freeze()
            .Sign(privateKey);
        }
        private ContractUpdateTransaction SpawnTestTransaction2()
        {
            return new ContractUpdateTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				ContractId = ContractId.FromString("0.0.5007"),
				AdminKey = privateKey,
				MaxAutomaticTokenAssociations = 101,
				AutoRenewPeriod = NodaTime.Duration.FromDays(1),
				ContractMemo = "3",
				StakedNodeId = 4,
				ExpirationTime = NodaTime.Instant.FromUnixTimeMilliseconds(4),
				ProxyAccountId = new AccountId(0, 0, 4),
				MaxTransactionFee = Hbar.FromTinybars(100000),
				AutoRenewAccountId = new AccountId(0, 0, 30),
			}
            .Freeze()
            .Sign(privateKey);
        }

        [Fact]
        /// <include file="test-contract-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractUpdateTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<ContractUpdateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-contract-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractUpdateTransactionTest.ShouldBytes2"]' />
        public virtual void ShouldBytes2()
        {
            var tx = SpawnTestTransaction2();
            var tx2 = Transaction.FromBytes<ContractUpdateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-contract-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractUpdateTransactionTest.ShouldSupportExpirationTimeDurationBytesRoundTrip"]' />
        public virtual void ShouldSupportExpirationTimeDurationBytesRoundTrip()
        {
            var tx = new ContractUpdateTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				ContractId = ContractId.FromString("0.0.5007"),
				AdminKey = privateKey,
				MaxAutomaticTokenAssociations = 101,
				AutoRenewPeriod = Duration.FromDays(1),
				ContractMemo = "with-duration",
				StakedAccountId = AccountId.FromString("0.0.3"),
				ExpirationTimeDuration = Duration.FromSeconds(1234),
				ProxyAccountId = new AccountId(0, 0, 4),
				MaxTransactionFee = Hbar.FromTinybars(100000),
				AutoRenewAccountId = new AccountId(0, 0, 30),
			};

            // When expiration is set via Duration, NodaTime.Instant getter should be null
            Assert.Null(tx.ExpirationTime);
            
            var tx2 = Transaction.FromBytes<ContractUpdateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
            Assert.Equal(tx2.ExpirationTime, Instant.FromUnixTimeMilliseconds(1234));
        }
        [Fact]
        /// <include file="test-contract-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractUpdateTransactionTest.SetExpirationTimeDurationOnFrozenTransactionShouldThrow"]' />
        public virtual void SetExpirationTimeDurationOnFrozenTransactionShouldThrow()
        {
            var tx = SpawnTestTransaction();

            Assert.Throws<InvalidOperationException>(() => tx.ExpirationTime = NodaTime.Instant.FromUnixTimeMilliseconds(1));
        }
        [Fact]
        /// <include file="test-contract-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractUpdateTransactionTest.GetSetExpirationTimeDateTime"]' />
        public virtual void GetSetExpirationTimeDateTime()
        {
            var instant = NodaTime.Instant.FromUnixTimeMilliseconds(1234567);
            var tx = new ContractUpdateTransaction
            {
				ExpirationTime = instant
			};

            Assert.Equal(tx.ExpirationTime, instant);
        }
        [Fact]
        /// <include file="test-contract-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractUpdateTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
                ContractUpdateInstance = new Proto.Services.ContractUpdateTransactionBody { }
            };
            var tx = Transaction.FromScheduledTransaction(transactionBody);

            Assert.IsType<ContractUpdateTransaction>(tx);
        }
    }
}
