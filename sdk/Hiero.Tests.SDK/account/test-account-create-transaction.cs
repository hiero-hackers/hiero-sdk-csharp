// SPDX-License-Identifier: Apache-2.0
using VerifyXunit;

using Hiero.SDK;
using Hiero.SDK.Core;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Ethereum;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Transactions;

namespace Hiero.Tests.SDK.Account
{
    /// <include file="test-account-create-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Account.AccountCreateTransactionTest"]' />
    public class AccountCreateTransactionTest
    {
        private static readonly PrivateKey privateKeyED25519 = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        PrivateKey privateKeyECDSA = PrivateKey.FromStringECDSA("7f109a9e3b0d8ecfba9cc23a3614433ce0fa7ddcc80f2a8f10b222179a5a80d6");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);
        private readonly string ALIAS = "0x5c562e90feaf0eebd33ea75d21024f249d451417";

        public virtual AccountCreateTransaction SpawnTestTransaction()
        {
            return new AccountCreateTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				Key = privateKeyED25519,
				InitialBalance = Hbar.FromTinybars(450),
				ProxyAccountId = AccountId.FromString("0.0.1001"),
				AccountMemo = "some dumb memo",
				ReceiverSigRequired = true,
				AutoRenewPeriod = NodaTime.Duration.FromHours(10),
				StakedAccountId = AccountId.FromString("0.0.3"),
				Alias = EvmAddress.FromString(ALIAS),
				MaxAutomaticTokenAssociations = 100,
				MaxTransactionFee = Hbar.FromTinybars(100000),
            }
            .SetKeyWithAlias(privateKeyECDSA)
            .SetKeyWithAlias(privateKeyED25519, privateKeyECDSA)    
            .Freeze()
            .Sign(privateKeyED25519);
        }

        public virtual AccountCreateTransaction SpawnTestTransaction2()
        {
            return new AccountCreateTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				Key = privateKeyED25519,
				InitialBalance = Hbar.FromTinybars(450),
				ProxyAccountId = AccountId.FromString("0.0.1001"),
				AccountMemo = "some dumb memo",
				ReceiverSigRequired = true,
				AutoRenewPeriod = NodaTime.Duration.FromHours(10),
				StakedNodeId = 4,
				MaxAutomaticTokenAssociations = 100,
				MaxTransactionFee = Hbar.FromTinybars(100000),
			}
			.SetKeyWithAlias(privateKeyECDSA)
            .SetKeyWithAlias(privateKeyED25519, privateKeyECDSA)                
            .Freeze()
            .Sign(privateKeyED25519);
        }

        [Fact]
        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }
        [Fact]
        /// <include file="test-account-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Account.AccountCreateTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<AccountCreateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }

        [Fact]
        public virtual void ShouldSerialize2()
        {
            Verifier.Verify(SpawnTestTransaction2().ToString());
        }
        [Fact]
        /// <include file="test-account-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Account.AccountCreateTransactionTest.ShouldBytes2"]' />
        public virtual void ShouldBytes2()
        {
            var tx = SpawnTestTransaction2();
            var tx2 = Transaction.FromBytes<AccountCreateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-account-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Account.AccountCreateTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new AccountCreateTransaction();
            var tx2 = Transaction.FromBytes<AccountCreateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-account-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Account.AccountCreateTransactionTest.PropertiesTest"]' />
        public virtual void PropertiesTest()
        {
            var tx = SpawnTestTransaction();

            Assert.Equal(tx.Key, privateKeyED25519);
            Assert.Equal(tx.InitialBalance, Hbar.FromTinybars(450));
            Assert.True(tx.ReceiverSigRequired);
            Assert.Equal("0.0.1001", tx.ProxyAccountId?.ToString());
            Assert.Equal(10, tx.AutoRenewPeriod.Hours);
            Assert.Equal(100, tx.MaxAutomaticTokenAssociations);
            Assert.Equal("some dumb memo", tx.AccountMemo);
            Assert.Equal("0.0.3", tx.StakedAccountId?.ToString());
            Assert.Null(tx.StakedNodeId);
            Assert.False(tx.DeclineStakingReward);
            Assert.Equal(tx.Alias, EvmAddress.FromString(ALIAS));
        }
        [Fact]
        /// <include file="test-account-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Account.AccountCreateTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
				CryptoCreateAccount = new Proto.Services.CryptoCreateTransactionBody()
			};
                
            var tx = Transaction.FromScheduledTransaction<AccountCreateTransaction>(transactionBody);

            Assert.IsType<AccountCreateTransaction>(tx);
        }
    }
}
