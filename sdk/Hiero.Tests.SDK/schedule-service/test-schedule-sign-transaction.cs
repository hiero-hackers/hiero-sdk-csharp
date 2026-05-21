// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK;
using Hiero.SDK.Core;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Schedule;
using Hiero.SDK.Transactions;

using VerifyXunit;

namespace Hiero.Tests.SDK.Schedule
{
    /// <include file="test-schedule-sign-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Schedule.ScheduleSignTransactionTest"]' />
    public class ScheduleSignTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }
        [Fact]
        /// <include file="test-schedule-sign-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Schedule.ScheduleSignTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new ScheduleSignTransaction();
            var tx2 = Transaction.FromBytes<ScheduleSignTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }

        private ScheduleSignTransaction SpawnTestTransaction()
        {
            return new ScheduleSignTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				ScheduleId = ScheduleId.FromString("0.0.444"),
				MaxTransactionFee = new Hbar(1),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-schedule-sign-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Schedule.ScheduleSignTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<ScheduleSignTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }
    }
}
