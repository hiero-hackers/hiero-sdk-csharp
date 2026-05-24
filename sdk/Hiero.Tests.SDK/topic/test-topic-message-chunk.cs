// SPDX-License-Identifier: Apache-2.0
using System;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Consensus;

using Google.Protobuf;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Topic
{
    /// <include file="test-topic-message-chunk.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Topic.TopicMessageChunkTest"]' />
    public class TopicMessageChunkTest
    {
        private static readonly NodaTime.Instant testTimestamp = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);
        private static readonly byte[] testContents = new byte[]
        {
            0x01,
            0x02,
            0x03
        };
        private static readonly byte[] testRunningHash = new byte[]
        {
            0x04,
            0x05,
            0x06
        };
        private static readonly ulong testSequenceNumber = 7;
        private static readonly TransactionId testTransactionId = new TransactionId(new AccountId(0, 0, 1), testTimestamp);

        [Fact]
        /// <include file="test-topic-message-chunk.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicMessageChunkTest.ConstructWithArgs"]' />
        public virtual void ConstructWithArgs()
        {
            var consensusTopicResponse = new Proto.Mirror.ConsensusTopicResponse
            {
                ConsensusTimestamp = testTimestamp.ToProtoTimestamp(),
                Message = ByteString.CopyFrom(testContents),
                RunningHash = ByteString.CopyFrom(testRunningHash),
                SequenceNumber = testSequenceNumber,
                ChunkInfo = new Proto.Services.ConsensusMessageChunkInfo { InitialTransactionId = testTransactionId.ToProtobuf() },
            };

            TopicMessageChunk topicMessageChunk = new (consensusTopicResponse);

            Assert.Equal(topicMessageChunk.ConsensusTimestamp, testTimestamp);
            Assert.Equal(topicMessageChunk.ContentSize, testContents.Length);
            Assert.Equal(topicMessageChunk.RunningHash, testRunningHash);
            Assert.Equal(topicMessageChunk.SequenceNumber, testSequenceNumber);
        }
    }
}
