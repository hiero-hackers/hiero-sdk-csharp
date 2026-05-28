// SPDX-License-Identifier: Apache-2.0
using Google.Protobuf;

using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Consensus;
using Hiero.SDK.Transactions;

using System;

using VerifyXunit;
using Hiero.SDK;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Transactions
{
    /// <include file="test-transactions-messagesubmit.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Transactions.MessageSubmitTransactionTest"]' />
    public class MessageSubmitTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);
        [Fact]
        /// <include file="test-transactions-messagesubmit.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Transactions.MessageSubmitTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<TopicMessageSubmitTransaction>(tx.ToBytes());
            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-transactions-messagesubmit.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Transactions.MessageSubmitTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new TopicMessageSubmitTransaction();
            var tx2 = Transaction.FromBytes<TopicMessageSubmitTransaction>(tx.ToBytes());
            Assert.Equal(tx.ToString(), tx2.ToString());
        }

        [Fact] public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }

        private TopicMessageSubmitTransaction SpawnTestTransaction()
        {
            return new TopicMessageSubmitTransaction()
			{
                NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				TopicId = TopicId.FromString("0.0.5007"),
				Message = ByteString.CopyFromUtf8("hello"),
				MaxTransactionFee = Hbar.FromTinybars(100000),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-transactions-messagesubmit.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Transactions.MessageSubmitTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
                ConsensusSubmitMessage = new Proto.Services.ConsensusSubmitMessageTransactionBody()
			};
            var tx = Transaction.FromScheduledTransaction<TopicMessageSubmitTransaction>(transactionBody);

            Assert.IsType<TopicMessageSubmitTransaction>(tx);
        }
    }
}
