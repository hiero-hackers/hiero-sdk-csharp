// SPDX-License-Identifier: Apache-2.0
using System;
using System.Text.RegularExpressions;

using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Schedule;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Networking;

using VerifyXunit;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Schedule
{
    /// <include file="test-schedule-info.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Schedule.ScheduleInfoTest"]' />
    public class ScheduleInfoTest
    {
        private static readonly PublicKey unusedPublicKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10").GetPublicKey();
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        public virtual ScheduleInfo SpawnScheduleInfoExample()
        {
            return new ScheduleInfo(
                ScheduleId.FromString("1.2.3"), 
                AccountId.FromString("4.5.6"), 
                AccountId.FromString("2.3.4"), 
                new Proto.Services.SchedulableTransactionBody 
                { 
                    CryptoDelete = new Proto.Services.CryptoDeleteTransactionBody 
                    { 
                        DeleteAccountID = AccountId.FromString("6.6.6").ToProtobuf()
                    }
                }, 
                [ unusedPublicKey ], 
                unusedPublicKey, 
                TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart), 
                "memo", 
                validStart, 
                validStart, 
                null, 
                LedgerId.TESTNET, 
                true);
        }
        public virtual ScheduleInfo SpawnScheduleInfoDeletedExample()
        {
            return new ScheduleInfo(
                ScheduleId.FromString("1.2.3"), 
                AccountId.FromString("4.5.6"), 
                AccountId.FromString("2.3.4"), 
                new Proto.Services.SchedulableTransactionBody
                {
                    CryptoDelete = new Proto.Services.CryptoDeleteTransactionBody
                    {
                        DeleteAccountID = AccountId.FromString("6.6.6").ToProtobuf()
                    }
                }, 
                [ unusedPublicKey ], 
                unusedPublicKey, 
                TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart), 
                "memo", 
                validStart, 
                null, 
                validStart, 
                LedgerId.TESTNET, 
                true);
        }

        [Fact]
        /// <include file="test-schedule-info.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Schedule.ScheduleInfoTest.ShouldSerialize"]' />
        public virtual void ShouldSerialize()
        {
            var originalScheduleInfo = SpawnScheduleInfoExample();
            byte[] scheduleInfoBytes = originalScheduleInfo.ToBytes();
            var copyScheduleInfo = ScheduleInfo.FromBytes(scheduleInfoBytes);
            
            Assert.Equal(Regex.Replace(copyScheduleInfo.ToString(), "@[A-Za-z0-9]+", ""), Regex.Replace(originalScheduleInfo.ToString(), "@[A-Za-z0-9]+", ""));
            
            Verifier.Verify(Regex.Replace(originalScheduleInfo.ToString(), "@[A-Za-z0-9]+", ""));
        }
        [Fact]
        /// <include file="test-schedule-info.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Schedule.ScheduleInfoTest.ShouldSerializeDeleted"]' />
        public virtual void ShouldSerializeDeleted()
        {
            var originalScheduleInfo = SpawnScheduleInfoDeletedExample();
            byte[] scheduleInfoBytes = originalScheduleInfo.ToBytes();
            var copyScheduleInfo = ScheduleInfo.FromBytes(scheduleInfoBytes);
            
            Assert.Equal(Regex.Replace(copyScheduleInfo.ToString(), "@[A-Za-z0-9]+", ""), Regex.Replace(originalScheduleInfo.ToString(), "@[A-Za-z0-9]+", ""));
            
            Verifier.Verify(Regex.Replace(originalScheduleInfo.ToString(), "@[A-Za-z0-9]+", ""));
        }
    }
}
