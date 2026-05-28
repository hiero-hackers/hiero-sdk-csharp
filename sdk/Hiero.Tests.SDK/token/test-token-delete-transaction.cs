// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Token;
using Hiero.SDK.Transactions;
using Hiero.SDK.Cryptocurrency;

using VerifyXunit;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Token
{
    /// <include file="test-token-delete-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Token.TokenDeleteTransactionTest"]' />
    public class TokenDeleteTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);
        [Fact] public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }
        [Fact]
        /// <include file="test-token-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDeleteTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new TokenDeleteTransaction();
            var tx2 = Transaction.FromBytes<TokenDeleteTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }

        private TokenDeleteTransaction SpawnTestTransaction()
        {
            return new TokenDeleteTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				TokenId = TokenId.FromString("1.2.3"),
				MaxTransactionFee = new Hbar(1),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-token-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDeleteTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<TokenDeleteTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-token-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDeleteTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
				TokenDeletion = new Proto.Services.TokenDeleteTransactionBody()
			};
            var tx = Transaction.FromScheduledTransaction(transactionBody);
            Assert.IsType<TokenDeleteTransaction>(tx);
        }
        [Fact]
        /// <include file="test-token-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDeleteTransactionTest.ConstructTokenDeleteTransaction"]' />
        public virtual void ConstructTokenDeleteTransaction()
        {
            var transaction = new TokenDeleteTransaction();
            
            Assert.Null(transaction.TokenId);
        }
        [Fact]
        /// <include file="test-token-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDeleteTransactionTest.ConstructTokenDeleteTransactionFromTransactionBodyProtobuf"]' />
        public virtual void ConstructTokenDeleteTransactionFromTransactionBodyProtobuf()
        {
            var tokenId = TokenId.FromString("1.2.3");
            var transactionBody = new Proto.Services.TokenDeleteTransactionBody
            {
                Token = tokenId.ToProtobuf()
            };
            var txBody = new Proto.Services.TransactionBody
            {
                TokenDeletion = transactionBody
            };
            var tokenDeleteTransaction = new TokenDeleteTransaction(txBody);
            
            Assert.Equal(tokenDeleteTransaction.TokenId, tokenId);
        }
        [Fact]
        /// <include file="test-token-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDeleteTransactionTest.GetSetTokenId"]' />
        public virtual void GetSetTokenId()
        {
            var tokenId = TokenId.FromString("1.2.3");
            var transaction = new TokenDeleteTransaction
            {
				TokenId = tokenId,
			};
            Assert.Equal(transaction.TokenId, tokenId);
        }
        [Fact]
        /// <include file="test-token-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDeleteTransactionTest.GetSetTokenIdFrozen"]' />
        public virtual void GetSetTokenIdFrozen()
        {
            var tokenId = TokenId.FromString("1.2.3");
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.TokenId = tokenId);
        }
    }
}
