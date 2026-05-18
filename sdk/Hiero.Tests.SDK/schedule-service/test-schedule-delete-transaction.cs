// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Schedule;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Transactions;

using VerifyXunit;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Schedule
{
    /// <include file="test-schedule-delete-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Schedule.ScheduleDeleteTransactionTest"]' />
    public class ScheduleDeleteTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly DateTimeOffset validStart = DateTimeOffset.FromUnixTimeMilliseconds(1554158542);

        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }

        private ScheduleDeleteTransaction SpawnTestTransaction()
        {
            return new ScheduleDeleteTransaction
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
        /// <include file="test-schedule-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Schedule.ScheduleDeleteTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<ScheduleDeleteTransaction>(tx.ToBytes());
            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-schedule-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Schedule.ScheduleDeleteTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new ScheduleDeleteTransaction();
            var tx2 = Transaction.FromBytes<ScheduleDeleteTransaction>(tx.ToBytes());
            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-schedule-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Schedule.ScheduleDeleteTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
				ScheduleDelete = new Proto.Services.ScheduleDeleteTransactionBody()
			};
            var tx = Transaction.FromScheduledTransaction<ScheduleDeleteTransaction>(transactionBody);
            
            Assert.IsType<ScheduleDeleteTransaction>(tx);
        }
    }
}
