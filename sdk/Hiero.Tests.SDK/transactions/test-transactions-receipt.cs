// SPDX-License-Identifier: Apache-2.0
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

using Hiero.SDK;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Contract;
using Hiero.SDK.File;
using Hiero.SDK.Schedule;
using Hiero.SDK.Token;
using Hiero.SDK.Consensus;

using System;

using VerifyXunit;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Transactions
{
    /// <include file="test-transactions-receipt.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Transactions.TransactionReceiptTest"]' />
    public class TransactionReceiptTest
    {
        private static readonly NodaTime.Instant time = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        public static TransactionReceipt SpawnReceiptExample()
        {
            return new TransactionReceipt(
                null,
				ResponseStatus.ScheduleAlreadyDeleted, 
                new ExchangeRate(3, 4, time), 
                new ExchangeRate(3, 4, time), 
                AccountId.FromString("1.2.3"), 
                FileId.FromString("4.5.6"), 
                ContractId.FromString("3.2.1"), 
                TopicId.FromString("9.8.7"), 
                TokenId.FromString("6.5.4"), 
                3, 
                ByteString.CopyFromUtf8("how now brown cow"), 30, 
                ScheduleId.FromString("1.1.1"), 
                TransactionId.WithValidStart(AccountId.FromString("3.3.3"), time), 
                [1, 2, 3], 
                1, 
                [],
                []);
        }
        [Fact]
        /// <include file="test-transactions-receipt.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Transactions.TransactionReceiptTest.ShouldSerialize"]' />
        public virtual void ShouldSerialize()
        {
            var originalTransactionReceipt = SpawnReceiptExample();
            byte[] transactionReceiptBytes = originalTransactionReceipt.ToBytes();
            var copyTransactionReceipt = TransactionReceipt.FromBytes(transactionReceiptBytes);
            Assert.Equal(copyTransactionReceipt.ToString(), originalTransactionReceipt.ToString());
            
            Verifier.Verify(originalTransactionReceipt.ToString());
        }
    }
}
