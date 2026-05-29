// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using System.Linq;

using Hiero.SDK;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Token;
using Hiero.SDK.Transactions;

using VerifyXunit;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Token
{
    /// <include file="test-token-dissociate-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Token.TokenDissociateTransactionTest"]' />
    public class TokenDissociateTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private static readonly AccountId testAccountId = AccountId.FromString("6.9.0");
        private static readonly List<TokenId> testTokenIds = [TokenId.FromString("4.2.0"), TokenId.FromString("4.2.1"), TokenId.FromString("4.2.2") ];
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);
        
        [Fact] 
        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }
        [Fact]
        /// <include file="test-token-dissociate-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDissociateTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new TokenDissociateTransaction();
            var tx2 = Transaction.FromBytes<TokenDissociateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }

        private TokenDissociateTransaction SpawnTestTransaction()
        {
            return new TokenDissociateTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				AccountId = testAccountId,
				TokenIds = [.. testTokenIds],
				MaxTransactionFee = new Hbar(1),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-token-dissociate-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDissociateTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<TokenDissociateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-token-dissociate-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDissociateTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
				TokenDissociate = new Proto.Services.TokenDissociateTransactionBody()
			};
            var tx = Transaction.FromScheduledTransaction<TokenDissociateTransaction>(transactionBody);

            Assert.IsType<TokenDissociateTransaction>(tx);
        }
        [Fact]
        /// <include file="test-token-dissociate-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDissociateTransactionTest.ConstructTokenDissociateTransactionFromTransactionBodyProtobuf"]' />
        public virtual void ConstructTokenDissociateTransactionFromTransactionBodyProtobuf()
        {
            var tx = new TokenDissociateTransaction(new Proto.Services.TransactionBody
            {
                TokenDissociate = new Proto.Services.TokenDissociateTransactionBody
                {
                    Account = testAccountId.ToProtobuf(),
                    Tokens = { testTokenIds.Select(_ => _.ToProtobuf()) }
                }
            });

            Assert.Equal(testAccountId, tx.AccountId);
            Assert.Equal(testTokenIds.Count, tx.TokenIds.Count);
        }
        [Fact]
        /// <include file="test-token-dissociate-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDissociateTransactionTest.GetSetAccountId"]' />
        public virtual void GetSetAccountId()
        {
            var tokenDissociateTransaction = new TokenDissociateTransaction
            {
				AccountId = testAccountId
			};
            Assert.Equal(tokenDissociateTransaction.AccountId, testAccountId);
        }
        [Fact]
        /// <include file="test-token-dissociate-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDissociateTransactionTest.GetSetAccountIdFrozen"]' />
        public virtual void GetSetAccountIdFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.AccountId = testAccountId);
        }
        [Fact]
        /// <include file="test-token-dissociate-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDissociateTransactionTest.GetSetTokenIds"]' />
        public virtual void GetSetTokenIds()
        {
            var tokenDissociateTransaction = new TokenDissociateTransaction
            {
				TokenIds = [.. testTokenIds]
            };
            Assert.Equal(tokenDissociateTransaction.TokenIds, testTokenIds);
        }
        [Fact]
        /// <include file="test-token-dissociate-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenDissociateTransactionTest.GetSetTokenIdsFrozen"]' />
        public virtual void GetSetTokenIdsFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.TokenIds = [..testTokenIds]);
        }
    }
}
