// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK.Consensus;
using Hiero.SDK.Cryptocurrency;

using Google.Protobuf;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Topic
{
    /// <include file="test-topic-message.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Topic.TopicMessageTest"]' />
    public class TopicMessageTest
    {
        private static readonly DateTimeOffset testTimestamp = DateTimeOffset.FromUnixTimeMilliseconds(1554158542);
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
        private static readonly TransactionId testTransactionId = new (new AccountId(0, 0, 1), testTimestamp);

        [Fact]
        /// <include file="test-topic-message.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicMessageTest.ConstructWithArgs"]' />
        public virtual void ConstructWithArgs()
        {
            var consensusTopicResponse = new Proto.Mirror.ConsensusTopicResponse
            {
                Message = ByteString.CopyFrom(testContents),
                RunningHash = ByteString.CopyFrom(testRunningHash),
                SequenceNumber = testSequenceNumber,
                ChunkInfo = new Proto.Services.ConsensusMessageChunkInfo { InitialTransactionId = testTransactionId.ToProtobuf() },
                ConsensusTimestamp = new Proto.Services.Timestamp
                {
                    Seconds = testTimestamp.ToUnixTimeSeconds(),
                    Nanos = testTimestamp.Nanosecond
                }
            };

            TopicMessageChunk topicMessageChunk = new (new Proto.Mirror.ConsensusTopicResponse
            {
                Message = ByteString.CopyFrom(testContents),
                RunningHash = ByteString.CopyFrom(testRunningHash),
                SequenceNumber = testSequenceNumber,
                ConsensusTimestamp = new Proto.Services.Timestamp
                {
                    Seconds = testTimestamp.ToUnixTimeSeconds(),
                    Nanos = testTimestamp.Nanosecond
                }
            });
            TopicMessageChunk[] topicMessageChunkArr = new[]
            {
                topicMessageChunk,
                topicMessageChunk,
                topicMessageChunk
            };
            TopicMessage topicMessage = new (testTimestamp, testContents, testRunningHash, testSequenceNumber, topicMessageChunkArr, testTransactionId);
            
            Assert.Equal(topicMessage.ConsensusTimestamp, testTimestamp);
            Assert.Equal(topicMessage.Contents, testContents);
            Assert.Equal(topicMessage.RunningHash, testRunningHash);
            Assert.Equal(topicMessage.SequenceNumber, testSequenceNumber);
            Assert.Equal(topicMessage.TransactionId, testTransactionId);
        }
        [Fact]
        /// <include file="test-topic-message.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicMessageTest.OfSingle"]' />
        public virtual void OfSingle()
        {
            var consensusTopicResponse = new Proto.Mirror.ConsensusTopicResponse
            {
                Message = ByteString.CopyFrom(testContents),
                RunningHash = ByteString.CopyFrom(testRunningHash),
                SequenceNumber = testSequenceNumber,
                ChunkInfo = new Proto.Services.ConsensusMessageChunkInfo { InitialTransactionId = testTransactionId.ToProtobuf() },
                ConsensusTimestamp = new Proto.Services.Timestamp
                {
                    Seconds = testTimestamp.ToUnixTimeSeconds(),
                    Nanos = testTimestamp.Nanosecond
                }
            };
            
            TopicMessage topicMessage = TopicMessage.OfSingle(consensusTopicResponse);
            
            Assert.Equal(testTimestamp, topicMessage.ConsensusTimestamp);
            Assert.Equal(testContents, topicMessage.Contents);
            Assert.Equal(testRunningHash, topicMessage.RunningHash);
            Assert.Equal(testSequenceNumber, topicMessage.SequenceNumber);
            Assert.Single(topicMessage.Chunks);
            Assert.Equal(testTransactionId, topicMessage.TransactionId);
        }
        [Fact]
        /// <include file="test-topic-message.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicMessageTest.OfMany"]' />
        public virtual void OfMany()
        {
            var consensusTopicResponse1 = new Proto.Mirror.ConsensusTopicResponse
            {
                Message = ByteString.CopyFrom(testContents),
                RunningHash = ByteString.CopyFrom(testRunningHash),
                SequenceNumber = testSequenceNumber,
                ChunkInfo = new Proto.Services.ConsensusMessageChunkInfo 
                {
                    InitialTransactionId = testTransactionId.ToProtobuf(),
                    Number = 1,
                    Total = 2,
                },
                ConsensusTimestamp = new Proto.Services.Timestamp
                {
                    Seconds = testTimestamp.ToUnixTimeSeconds(),
                    Nanos = testTimestamp.Nanosecond
                }
            };
            var consensusTopicResponse2 = new Proto.Mirror.ConsensusTopicResponse
            {
                Message = ByteString.CopyFrom(testContents),
                RunningHash = ByteString.CopyFrom(testRunningHash),
                SequenceNumber = testSequenceNumber + 1,
                ChunkInfo = new Proto.Services.ConsensusMessageChunkInfo 
                {
                    InitialTransactionId = testTransactionId.ToProtobuf(),
                    Number = 2,
                    Total = 2,
                },
                ConsensusTimestamp = new Proto.Services.Timestamp
                {
                    Seconds = testTimestamp.ToUnixTimeSeconds() + 1,
                    Nanos = testTimestamp.Nanosecond
                }
            };
            
            TopicMessage topicMessage = TopicMessage.OfMany([consensusTopicResponse1, consensusTopicResponse2]);
            byte[] totalContents = new byte[testContents.Length * 2];
            
            Array.Copy(testContents, 0, totalContents, 0, testContents.Length);
            Array.Copy(testContents, 0, totalContents, testContents.Length, testContents.Length);
            Assert.Equal(testTimestamp.AddSeconds(1), topicMessage.ConsensusTimestamp);
            Assert.Equal(totalContents, topicMessage.Contents);
            Assert.Equal(testRunningHash, topicMessage.RunningHash);
            Assert.Equal(testSequenceNumber + 1, topicMessage.SequenceNumber);
            Assert.Equal(testTransactionId, topicMessage.TransactionId);
        }
    }
}
