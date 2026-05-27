// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK.Cryptography;
using Hiero.SDK.Transactions;
using Hiero.SDK.Token;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Nfts;

using Google.Protobuf;

using VerifyXunit;
using Hiero.SDK;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Transactions
{
    /// <include file="test-transactions-cryptotransfer.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Transactions.CryptoTransferTransactionTest"]' />
    public class CryptoTransferTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = KeyTestDataFactory.ED25519_TEST_KEY;
        private readonly NodaTime.Instant validStart = TransactionTestFactory.DEFAULT_VALID_START;

        [Fact] public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }
        [Fact]
        /// <include file="test-transactions-cryptotransfer.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Transactions.CryptoTransferTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new TransferTransaction();
            var tx2 = Transaction.FromBytes<TransferTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }

        private TransferTransaction SpawnTestTransaction()
        {
            return new TransferTransaction
            {
				NodeAccountIds = TransactionTestFactory.CreateDefaultNodeAccountIds(),
				TransactionId = TransactionTestFactory.CreateDefaultTransactionId(),
				MaxTransactionFee = Hbar.FromTinybars(TestData.HBAR_100000),
			}
                .AddHbarTransfer(AccountId.FromString(TestData.TRANSFER_ACCOUNT_2), Hbar.FromTinybars(TestData.HBAR_400))
                .AddHbarTransfer(AccountId.FromString(TestData.SECONDARY_ENTITY_ID), Hbar.FromTinybars(TestData.HBAR_800).Negated())
                .AddHbarTransfer(AccountId.FromString(TestData.TRANSFER_ACCOUNT_1), Hbar.FromTinybars(TestData.HBAR_400))
                .AddTokenTransfer(TokenId.FromString(TestData.TOKEN_ID_1), AccountId.FromString(TestData.TRANSFER_ACCOUNT_2), 400)
                .AddTokenTransferWithDecimals(TokenId.FromString(TestData.TOKEN_ID_1), AccountId.FromString(TestData.SECONDARY_ENTITY_ID), -800, 3)
                .AddTokenTransferWithDecimals(TokenId.FromString(TestData.TOKEN_ID_1), AccountId.FromString(TestData.TRANSFER_ACCOUNT_1), 400, 3)
                .AddTokenTransfer(TokenId.FromString(TestData.TOKEN_ID_2), AccountId.FromString(TestData.TRANSFER_ACCOUNT_2), 1)
                .AddTokenTransfer(TokenId.FromString(TestData.TOKEN_ID_2), AccountId.FromString(TestData.SECONDARY_ENTITY_ID), -1)
                .AddNftTransfer(TokenId.FromString(TestData.TOKEN_ID_3).Nft(2), AccountId.FromString(TestData.TRANSFER_ACCOUNT_2), AccountId.FromString(TestData.TRANSFER_ACCOUNT_1))
                .AddNftTransfer(TokenId.FromString(TestData.TOKEN_ID_3).Nft(1), AccountId.FromString(TestData.TRANSFER_ACCOUNT_2), AccountId.FromString(TestData.TRANSFER_ACCOUNT_1))
                .AddNftTransfer(TokenId.FromString(TestData.TOKEN_ID_3).Nft(3), AccountId.FromString(TestData.TRANSFER_ACCOUNT_2), AccountId.FromString(TestData.SECONDARY_ENTITY_ID))
                .AddNftTransfer(TokenId.FromString(TestData.TOKEN_ID_3).Nft(4), AccountId.FromString(TestData.TRANSFER_ACCOUNT_1), AccountId.FromString(TestData.SECONDARY_ENTITY_ID))
                .AddNftTransfer(TokenId.FromString(TestData.TOKEN_ID_4).Nft(4), AccountId.FromString(TestData.TRANSFER_ACCOUNT_1), AccountId.FromString(TestData.SECONDARY_ENTITY_ID))
                .SetHbarTransferApproval(AccountId.FromString(TestData.TRANSFER_ACCOUNT_1), true)
                .SetTokenTransferApproval(TokenId.FromString(TestData.TOKEN_ID_2), AccountId.FromString(TestData.SECONDARY_ENTITY_ID), true)
                .SetNftTransferApproval(new NftId(TokenId.FromString(TestData.TOKEN_ID_2), 4), true)
            .Freeze()
            .Sign(unusedPrivateKey);
        }

        private TransferTransaction SpawnModifiedTestTransaction()
        {
			return new TransferTransaction
			{
				NodeAccountIds = TransactionTestFactory.CreateDefaultNodeAccountIds(),
				TransactionId = TransactionTestFactory.CreateDefaultTransactionId(),
				MaxTransactionFee = Hbar.FromTinybars(TestData.HBAR_100000),
			}
                .AddHbarTransfer(AccountId.FromString(TestData.TRANSFER_ACCOUNT_2), Hbar.FromTinybars(TestData.HBAR_400))
                .AddHbarTransfer(AccountId.FromString(TestData.SECONDARY_ENTITY_ID), Hbar.FromTinybars(TestData.HBAR_800).Negated())
                .AddHbarTransfer(AccountId.FromString(TestData.TRANSFER_ACCOUNT_1), Hbar.FromTinybars(TestData.HBAR_400))
                .AddTokenTransfer(TokenId.FromString(TestData.TOKEN_ID_1), AccountId.FromString(TestData.TRANSFER_ACCOUNT_2), 400)
                .AddTokenTransferWithDecimals(TokenId.FromString(TestData.TOKEN_ID_1), AccountId.FromString(TestData.SECONDARY_ENTITY_ID), -800, 3)
                .AddTokenTransferWithDecimals(TokenId.FromString(TestData.TOKEN_ID_1), AccountId.FromString(TestData.TRANSFER_ACCOUNT_1), 400, 3)
                .AddTokenTransfer(TokenId.FromString(TestData.TOKEN_ID_2), AccountId.FromString(TestData.TRANSFER_ACCOUNT_2), 1)
                .AddTokenTransfer(TokenId.FromString(TestData.TOKEN_ID_2), AccountId.FromString(TestData.SECONDARY_ENTITY_ID), -1)
                .AddNftTransfer(TokenId.FromString(TestData.TOKEN_ID_3).Nft(2), AccountId.FromString(TestData.TRANSFER_ACCOUNT_2), AccountId.FromString(TestData.TRANSFER_ACCOUNT_1))
                .AddNftTransfer(TokenId.FromString(TestData.TOKEN_ID_3).Nft(1), AccountId.FromString(TestData.TRANSFER_ACCOUNT_2), AccountId.FromString(TestData.TRANSFER_ACCOUNT_1))
                .AddNftTransfer(TokenId.FromString(TestData.TOKEN_ID_3).Nft(3), AccountId.FromString(TestData.TRANSFER_ACCOUNT_2), AccountId.FromString(TestData.SECONDARY_ENTITY_ID))
                .AddNftTransfer(TokenId.FromString(TestData.TOKEN_ID_3).Nft(4), AccountId.FromString(TestData.TRANSFER_ACCOUNT_1), AccountId.FromString(TestData.SECONDARY_ENTITY_ID))
                .AddNftTransfer(TokenId.FromString(TestData.TOKEN_ID_4).Nft(4), AccountId.FromString(TestData.TRANSFER_ACCOUNT_1), AccountId.FromString(TestData.SECONDARY_ENTITY_ID))
                .SetHbarTransferApproval(AccountId.FromString(TestData.TRANSFER_ACCOUNT_1), true)
                .SetNftTransferApproval(new NftId(TokenId.FromString(TestData.TOKEN_ID_2), 4), true)
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-transactions-cryptotransfer.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Transactions.CryptoTransferTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<TransferTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }

        public virtual void DecimalsMustBeConsistent()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new TransferTransaction()
                    .AddTokenTransferWithDecimals(TokenId.FromString(TestData.TOKEN_ID_1), AccountId.FromString("0.0.8"), 100, 2)
                    .AddTokenTransferWithDecimals(TokenId.FromString(TestData.TOKEN_ID_1), AccountId.FromString("0.0.7"), -100, 3);
            });
        }
        [Fact]
        /// <include file="test-transactions-cryptotransfer.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Transactions.CryptoTransferTransactionTest.CanGetDecimals"]' />
        public virtual void CanGetDecimals()
        {
            var tx = new TransferTransaction();
            Assert.False(tx.GetTokenIdDecimals().ContainsKey(TokenId.FromString(TestData.TOKEN_ID_1)));
            tx.AddTokenTransfer(TokenId.FromString(TestData.TOKEN_ID_1), AccountId.FromString("0.0.8"), 100);
            Assert.False(tx.GetTokenIdDecimals().ContainsKey(TokenId.FromString(TestData.TOKEN_ID_1)));
            tx.AddTokenTransferWithDecimals(TokenId.FromString(TestData.TOKEN_ID_1), AccountId.FromString("0.0.7"), -100, 5);
            Assert.Equal(tx.GetTokenIdDecimals()[TokenId.FromString(TestData.TOKEN_ID_1)], (uint)5);
        }
        [Fact]
        /// <include file="test-transactions-cryptotransfer.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Transactions.CryptoTransferTransactionTest.TransactionBodiesMustMatch"]' />
        public virtual void TransactionBodiesMustMatch()
        {
            Proto.Services.Transaction tx1 = Proto.SDK.TransactionList.Parser.ParseFrom(SpawnTestTransaction().ToBytes()).TransactionList_[0];
            Proto.Services.Transaction tx2 = Proto.SDK.TransactionList.Parser.ParseFrom(SpawnModifiedTestTransaction().ToBytes()).TransactionList_[1];
            var brokenTxList = new Proto.SDK.TransactionList();
            brokenTxList.TransactionList_.Add(tx1);
            brokenTxList.TransactionList_.Add(tx2);
            var brokenTxBytes = brokenTxList.ToByteArray();

            Assert.Throws<ArgumentException>(() =>
            {
                ITransaction.FromBytes(brokenTxBytes);
            });
        }
        [Fact]
        /// <include file="test-transactions-cryptotransfer.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Transactions.CryptoTransferTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody()
            {
                CryptoTransfer = new Proto.Services.CryptoTransferTransactionBody()
			};
            var tx = Transaction.FromScheduledTransaction(transactionBody);
            Assert.IsType<TransferTransaction>(tx);
        }
    }
}
