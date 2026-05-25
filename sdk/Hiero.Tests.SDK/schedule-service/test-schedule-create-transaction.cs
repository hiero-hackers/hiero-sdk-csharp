// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK;
using Hiero.SDK.Core;
using Hiero.SDK.Schedule;
using Hiero.SDK.Transactions;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Cryptography;

using VerifyXunit;

using NodaTime;
using NodaTime.Extensions;

namespace Hiero.Tests.SDK.Schedule
{
    /// <include file="test-schedule-create-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Schedule.ScheduleCreateTransactionTest"]' />
    public class ScheduleCreateTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }

        private ScheduleCreateTransaction SpawnTestTransaction()
        {
            var transferTransaction = new TransferTransaction()
                .AddHbarTransfer(AccountId.FromString("0.0.555"), new Hbar(-10))
                .AddHbarTransfer(AccountId.FromString("0.0.333"), new Hbar(10));
            
            return transferTransaction.Schedule(_ =>
            {
                _.NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006"));
				_.TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart);
				_.AdminKey = unusedPrivateKey;
				_.PayerAccountId = AccountId.FromString("0.0.222");
				_.ScheduleMemo = "hi";
				_.MaxTransactionFee = new Hbar(1);
				_.ExpirationTime = validStart;     
            
            }).Freeze().Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-schedule-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Schedule.ScheduleCreateTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<ScheduleCreateTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-schedule-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Schedule.ScheduleCreateTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new ScheduleCreateTransaction();
            var tx2 = Transaction.FromBytes<ScheduleCreateTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-schedule-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Schedule.ScheduleCreateTransactionTest.ShouldSupportExpirationTimeDurationBytesRoundTrip"]' />
        public virtual void ShouldSupportExpirationTimeDurationBytesRoundTrip()
        {
            var tx = new TransferTransaction()
                .AddHbarTransfer(AccountId.FromString("0.0.555"), new Hbar(-10))
                .AddHbarTransfer(AccountId.FromString("0.0.333"), new Hbar(10))
                .Schedule(_ =>
                {
                    _.NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006"));
                    _.TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart);
                    _.AdminKey = unusedPrivateKey;
                    _.PayerAccountId = AccountId.FromString("0.0.222");
                    _.ScheduleMemo = "with-duration";
                    _.MaxTransactionFee = new Hbar(1);
                    _.ExpirationTime = DateTimeOffset.UnixEpoch.AddSeconds(1234).ToInstant();
                });

            // When expiration is set via Duration, NodaTime.Instant getter should be null
            // TODO Assert.Null(tx.ExpirationTime);
            
            var tx2 = Transaction.FromBytes<ScheduleCreateTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
            Assert.Equal(tx2.ExpirationTime, Instant.FromUnixTimeMilliseconds(1234));
        }
        [Fact]
        /// <include file="test-schedule-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Schedule.ScheduleCreateTransactionTest.SetExpirationTimeDurationOnFrozenTransactionShouldThrow"]' />
        public virtual void SetExpirationTimeDurationOnFrozenTransactionShouldThrow()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.ExpirationTime = NodaTime.Instant.FromUnixTimeSeconds(1));
        }
        [Fact]
        /// <include file="test-schedule-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Schedule.ScheduleCreateTransactionTest.GetSetExpirationTimeDateTime"]' />
        public virtual void GetSetExpirationTimeDateTime()
        {
            var instant = Instant.FromUnixTimeMilliseconds(1234567);
            var tx = new ScheduleCreateTransaction
            {
				ExpirationTime = instant
			};

            Assert.Equal(tx.ExpirationTime?.ToUnixTimeSeconds(), instant.ToUnixTimeSeconds());
            Assert.Equal(tx.ExpirationTime?.ToUnixTimeSecondsAndNanoseconds().nanoseconds, instant.ToUnixTimeSecondsAndNanoseconds().nanoseconds);
        }
    }
}
