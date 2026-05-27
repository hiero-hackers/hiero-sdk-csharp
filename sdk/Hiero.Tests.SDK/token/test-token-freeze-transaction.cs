// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Transactions;
using Hiero.SDK.Token;
using Hiero.SDK.Cryptography;

using VerifyXunit;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Token
{
    /// <include file="test-token-freeze-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Token.TokenFreezeTransactionTest"]' />
    public class TokenFreezeTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        [Fact] public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }

        private TokenFreezeTransaction SpawnTestTransaction()
        {
            return new TokenFreezeTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				AccountId = AccountId.FromString("0.0.222"),
				TokenId = TokenId.FromString("6.5.4"),
				MaxTransactionFee = new Hbar(1),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-token-freeze-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenFreezeTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new TokenFreezeTransaction();
            var tx2 = Transaction.FromBytes<TokenFreezeTransaction>(tx.ToBytes());
            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-token-freeze-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenFreezeTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<TokenFreezeTransaction>(tx.ToBytes());
            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-token-freeze-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenFreezeTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
				TokenFreeze = new Proto.Services.TokenFreezeAccountTransactionBody()
			};
            var tx = Transaction.FromScheduledTransaction<TokenFreezeTransaction>(transactionBody);
            
            Assert.IsType<TokenFreezeTransaction>(tx);
        }
    }
}
