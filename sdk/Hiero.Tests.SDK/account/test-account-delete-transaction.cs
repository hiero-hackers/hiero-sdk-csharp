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
    /// <include file="test-account-delete-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Account.AccountDeleteTransactionTest"]' />
    public class AccountDeleteTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        [Fact] public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }

        private AccountDeleteTransaction SpawnTestTransaction()
        {
            return new AccountDeleteTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				AccountId = AccountId.FromString("0.0.5007"),
				TransferAccountId = AccountId.FromString("0.0.5008"),
				MaxTransactionFee = Hbar.FromTinybars(100000),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-account-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Account.AccountDeleteTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<AccountDeleteTransaction>(tx.ToBytes());
            
            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-account-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Account.AccountDeleteTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new AccountDeleteTransaction();
            var tx2 = Transaction.FromBytes<AccountDeleteTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-account-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Account.AccountDeleteTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody()
            {
                CryptoDelete = new Proto.Services.CryptoDeleteTransactionBody
                {
                    DeleteAccountId = AccountId.FromString("6.6.6").ToProtobuf()
                }
            };
            
            var tx = Transaction.FromScheduledTransaction<AccountDeleteTransaction>(transactionBody);

            Assert.IsType<AccountDeleteTransaction>(tx);
        }
    }
}
