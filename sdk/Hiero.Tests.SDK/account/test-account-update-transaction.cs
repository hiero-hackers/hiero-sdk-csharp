// SPDX-License-Identifier: Apache-2.0

using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Transactions;

using System;

using VerifyXunit;
using Hiero.SDK;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Account
{
    /// <include file="test-account-update-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Account.AccountUpdateTransactionTest"]' />
    public class AccountUpdateTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        public virtual AccountUpdateTransaction SpawnTestTransaction()
        {
            return new AccountUpdateTransaction
            {
				Key = unusedPrivateKey,
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				AccountId = AccountId.FromString("0.0.2002"),
				ProxyAccountId = AccountId.FromString("0.0.1001"),
				AutoRenewPeriod = NodaTime.Duration.FromHours(10),
				ExpirationTime = NodaTime.Instant.FromUnixTimeMilliseconds(1554158543),
				ReceiverSigRequired = false,
				MaxAutomaticTokenAssociations = 100,
				AccountMemo = "Some memo",
				MaxTransactionFee = Hbar.FromTinybars(100000),
				StakedAccountId = AccountId.FromString("0.0.3"),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }

        public virtual AccountUpdateTransaction SpawnTestTransaction2()
        {
            return new AccountUpdateTransaction
            {
				Key = unusedPrivateKey,
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				AccountId = AccountId.FromString("0.0.2002"),
				ProxyAccountId = AccountId.FromString("0.0.1001"),
				AutoRenewPeriod = NodaTime.Duration.FromHours(10),
				ExpirationTime = NodaTime.Instant.FromUnixTimeMilliseconds(1554158543),
				ReceiverSigRequired = false,
				MaxAutomaticTokenAssociations = 100,
				AccountMemo = "Some memo",
				MaxTransactionFee = Hbar.FromTinybars(100000),
				StakedNodeId = 4,
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }

        [Fact] public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }
        [Fact]
        /// <include file="test-account-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Account.AccountUpdateTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<AccountUpdateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-account-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Account.AccountUpdateTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new AccountUpdateTransaction();
            var tx2 = Transaction.FromBytes<AccountUpdateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }

        public virtual void ShouldSerialize2()
        {
            Verifier.Verify(SpawnTestTransaction2().ToString());
        }
        [Fact]
        /// <include file="test-account-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Account.AccountUpdateTransactionTest.ShouldBytes2"]' />
        public virtual void ShouldBytes2()
        {
            var tx = SpawnTestTransaction2();
            var tx2 = Transaction.FromBytes<AccountUpdateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-account-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Account.AccountUpdateTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
				CryptoUpdateAccount = new Proto.Services.CryptoUpdateTransactionBody()
			};
            var tx = Transaction.FromScheduledTransaction<AccountUpdateTransaction>(transactionBody);

            Assert.IsType<AccountUpdateTransaction>(tx);
        }
    }
}
