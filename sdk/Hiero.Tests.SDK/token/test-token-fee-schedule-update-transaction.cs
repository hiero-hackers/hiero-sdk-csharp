// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;

using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Token;
using Hiero.SDK.Fee;
using Hiero.SDK.Transactions;

using VerifyXunit;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Token
{
    /// <include file="test-token-fee-schedule-update-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Token.TokenFeeScheduleUpdateTransactionTest"]' />
    public class TokenFeeScheduleUpdateTransactionTest
    {
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        private TokenFeeScheduleUpdateTransaction SpawnTestTransaction()
        {
            return new TokenFeeScheduleUpdateTransaction
            {
				TokenId = new TokenId(0, 0, 8798),
				CustomFees =
                [
                    new CustomFixedFee
                    {
                        FeeCollectorAccountId = new AccountId(0, 0, 4322),
                        DenominatingTokenId = new TokenId(0, 0, 483902),
                        Amount = 10,
                    },
                    new CustomFractionalFee
                    {
                        FeeCollectorAccountId = new AccountId(0, 0, 389042),
                        Numerator = 3,
                        Denominator = 7,
                        Min = 3,
                        Max = 100,
                        AssessmentMethod = FeeAssessmentMethod.Exclusive
                    }
                ],
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
			
            }.Freeze();
        }
        [Fact]
        /// <include file="test-token-fee-schedule-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenFeeScheduleUpdateTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new TokenFeeScheduleUpdateTransaction();
            var tx2 = Transaction.FromBytes<TokenFeeScheduleUpdateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-token-fee-schedule-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenFeeScheduleUpdateTransactionTest.ShouldSerialize"]' />
        public virtual void ShouldSerialize()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<TokenFeeScheduleUpdateTransaction>(tx.ToBytes());
            
            Assert.Equal(tx.ToString(), tx2.ToString());

            Verifier.Verify(tx.ToString());
        }
        [Fact]
        /// <include file="test-token-fee-schedule-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenFeeScheduleUpdateTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
				TokenFeeScheduleUpdate = new Proto.Services.TokenFeeScheduleUpdateTransactionBody()
			};
            var tx = Transaction.FromScheduledTransaction<TokenFeeScheduleUpdateTransaction>(transactionBody);
            Assert.IsType<TokenFeeScheduleUpdateTransaction>(tx);
        }
    }
}
