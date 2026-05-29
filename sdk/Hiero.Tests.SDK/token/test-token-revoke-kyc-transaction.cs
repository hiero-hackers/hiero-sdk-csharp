// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Token;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Transactions;

using VerifyXunit;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Token
{
    /// <include file="test-token-revoke-kyc-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Token.TokenRevokeKycTransactionTest"]' />
    public class TokenRevokeKycTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private static readonly TokenId testTokenId = TokenId.FromString("4.2.0");
        private static readonly AccountId testAccountId = AccountId.FromString("6.9.0");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        [Fact] 
        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }

        private TokenRevokeKycTransaction SpawnTestTransaction()
        {
            return new TokenRevokeKycTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				AccountId = testAccountId,
				TokenId = testTokenId,
				MaxTransactionFee = new Hbar(1),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-token-revoke-kyc-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenRevokeKycTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new TokenRevokeKycTransaction();
            var tx2 = Transaction.FromBytes<TokenRevokeKycTransaction>(tx.ToBytes());
            
            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-token-revoke-kyc-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenRevokeKycTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<TokenRevokeKycTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-token-revoke-kyc-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenRevokeKycTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
				TokenRevokeKyc = new Proto.Services.TokenRevokeKycTransactionBody()
			};
			var tx = Transaction.FromScheduledTransaction(transactionBody);
            Assert.IsType<TokenRevokeKycTransaction>(tx);
        }
        [Fact]
        /// <include file="test-token-revoke-kyc-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenRevokeKycTransactionTest.ConstructTokenRevokeKycTransactionFromTransactionBodyProtobuf"]' />
        public virtual void ConstructTokenRevokeKycTransactionFromTransactionBodyProtobuf()
        {
            var transactionBody = new Proto.Services.TokenRevokeKycTransactionBody
            {
				Account = testAccountId.ToProtobuf(),
				Token = testTokenId.ToProtobuf()
			};
            var tx = new Proto.Services.TransactionBody { TokenRevokeKyc = transactionBody };
            var tokenRevokeKycTransaction = new TokenRevokeKycTransaction(tx);

            Assert.Equal(tokenRevokeKycTransaction.TokenId, testTokenId);
        }
        [Fact]
        /// <include file="test-token-revoke-kyc-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenRevokeKycTransactionTest.GetSetAccountId"]' />
        public virtual void GetSetAccountId()
        {
            var tokenRevokeKycTransaction = new TokenRevokeKycTransaction { AccountId = testAccountId };
            Assert.Equal(tokenRevokeKycTransaction.AccountId, testAccountId);
        }
        [Fact]
        /// <include file="test-token-revoke-kyc-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenRevokeKycTransactionTest.GetSetAccountIdFrozen"]' />
        public virtual void GetSetAccountIdFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.AccountId = testAccountId);
        }
        [Fact]
        /// <include file="test-token-revoke-kyc-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenRevokeKycTransactionTest.GetSetTokenId"]' />
        public virtual void GetSetTokenId()
        {
            var tokenRevokeKycTransaction = new TokenRevokeKycTransaction { TokenId = testTokenId };
            Assert.Equal(tokenRevokeKycTransaction.TokenId, testTokenId);
        }
        [Fact]
        /// <include file="test-token-revoke-kyc-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenRevokeKycTransactionTest.GetSetTokenIdFrozen"]' />
        public virtual void GetSetTokenIdFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.TokenId = testTokenId);
        }
    }
}
