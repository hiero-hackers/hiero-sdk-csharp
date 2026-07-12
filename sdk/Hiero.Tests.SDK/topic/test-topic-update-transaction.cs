// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;

using Hiero.SDK.Cryptography;
using Hiero.SDK.Consensus;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Transactions;
using Hiero.SDK.Token;
using Hiero.SDK.Fee;

using VerifyXunit;
using Hiero.SDK.Core;
using System.Linq;

namespace Hiero.Tests.SDK.Topic
{
    /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest"]' />
    public class TopicUpdateTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private static readonly PublicKey testAdminKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e11").GetPublicKey();
        private static readonly PublicKey testSubmitKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e12").GetPublicKey();
        private static readonly TopicId testTopicId = TopicId.FromString("0.0.5007");
        private static readonly string testTopicMemo = "test memo";
        private static readonly NodaTime.Duration testAutoRenewPeriod = NodaTime.Duration.FromHours(10);
        private static readonly NodaTime.Instant testExpirationTime = NodaTime.SystemClock.Instance.GetCurrentInstant();
        private static readonly AccountId testAutoRenewAccountId = AccountId.FromString("8.8.8");
        private static readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        [Fact]
        public virtual void ClearShouldSerialize()
        {
            Verifier.Verify(new TopicUpdateTransaction
            {
                NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
                TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
                TopicId = testTopicId,
                AdminKey = null,
                AutoRenewAccountId = null,
                SubmitKey = null,
                TopicMemo = null,
            
            }.Freeze().Sign(unusedPrivateKey).ToString());
        }

        [Fact]
        public virtual void SetShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new TopicUpdateTransaction();
            var tx2 = Transaction.FromBytes(tx.ToBytes());
            Assert.Equal(tx.ToString(), tx2.ToString());
        }

        private TopicUpdateTransaction SpawnTestTransaction()
        {
            return new TopicUpdateTransaction
            {
                NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
                TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
                TopicId = testTopicId,
                AdminKey = testAdminKey,
                AutoRenewAccountId = testAutoRenewAccountId,
                AutoRenewPeriod = testAutoRenewPeriod,
                SubmitKey = testSubmitKey,
                TopicMemo = testTopicMemo,
                ExpirationTime = validStart

            }.Freeze().Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<TopicUpdateTransaction>(tx.ToBytes());
            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
                ConsensusUpdateTopic = new Proto.Services.ConsensusUpdateTopicTransactionBody()
            };
            var tx = Transaction.FromScheduledTransaction(transactionBody);
            Assert.IsType<TopicUpdateTransaction>(tx);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ConstructTopicUpdateTransactionFromTransactionBodyProtobuf"]' />
        public virtual void ConstructTopicUpdateTransactionFromTransactionBodyProtobuf()
        {
            var transactionBody = new Proto.Services.ConsensusUpdateTopicTransactionBody
            {
                TopicID = testTopicId.ToProtobuf(),
                Memo = testTopicMemo,
                ExpirationTime = new Proto.Services.Timestamp { Seconds = testExpirationTime.ToUnixTimeSeconds() },
                AdminKey = testAdminKey.ToProtobufKey(),
                SubmitKey = testSubmitKey.ToProtobufKey(),
                AutoRenewPeriod = new Proto.Services.Duration { Seconds = (long)testAutoRenewPeriod.TotalSeconds },
                AutoRenewAccount = testAutoRenewAccountId.ToProtobuf()
            };
            var tx = new Proto.Services.TransactionBody { ConsensusUpdateTopic = transactionBody };
            var topicUpdateTransaction = new TopicUpdateTransaction(tx);

            Assert.Equal(topicUpdateTransaction.TopicId, testTopicId);
            Assert.Equal(topicUpdateTransaction.TopicMemo, testTopicMemo);
            Assert.Equal(topicUpdateTransaction.ExpirationTime?.ToUnixTimeSeconds(), testExpirationTime.ToUnixTimeSeconds());
            Assert.Equal(topicUpdateTransaction.AdminKey, testAdminKey);
            Assert.Equal(topicUpdateTransaction.SubmitKey, testSubmitKey);
            Assert.Equal(topicUpdateTransaction.AutoRenewPeriod?.TotalSeconds, testAutoRenewPeriod.TotalSeconds);
            Assert.Equal(topicUpdateTransaction.AutoRenewAccountId, testAutoRenewAccountId);
        }

        [Fact]
        // ----------------
        // doesn't throw an exception as opposed to C++ sdk
        // ----------------
        // Above is from Java port. Throws in C#
        public virtual void ConstructTopicUpdateTransactionFromWrongTransactionBodyProtobuf()
        {
            var transactionBody = new Proto.Services.CryptoDeleteTransactionBody { };
            var tx = new Proto.Services.TransactionBody { CryptoDelete = transactionBody };

            Assert.ThrowsAny<Exception>(() => _ = new TopicUpdateTransaction(tx));
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.GetSetTopicId"]' />
        public virtual void GetSetTopicId()
        {
            var topicUpdateTransaction = new TopicUpdateTransaction { TopicId = testTopicId };
            Assert.Equal(topicUpdateTransaction.TopicId, testTopicId);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.GetSetTopicIdFrozen"]' />
        public virtual void GetSetTopicIdFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.TopicId = testTopicId);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.GetSetTopicMemo"]' />
        public virtual void GetSetTopicMemo()
        {
            var topicUpdateTransaction = new TopicUpdateTransaction { TopicMemo = testTopicMemo };
            Assert.Equal(topicUpdateTransaction.TopicMemo, testTopicMemo);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.GetSetTopicMemoFrozen"]' />
        public virtual void GetSetTopicMemoFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.TopicMemo = testTopicMemo);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ClearTopicMemo"]' />
        public virtual void ClearTopicMemo()
        {
            var topicUpdateTransaction = new TopicUpdateTransaction { TopicMemo = testTopicMemo };
            topicUpdateTransaction.TopicMemo = null;
            
            Assert.Null(topicUpdateTransaction.TopicMemo);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ClearTopicMemoFrozen"]' />
        public virtual void ClearTopicMemoFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.TopicMemo = null);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.GetSetExpirationTime"]' />
        public virtual void GetSetExpirationTime()
        {
            var topicUpdateTransaction = new TopicUpdateTransaction { ExpirationTime = testExpirationTime };
            Assert.Equal(topicUpdateTransaction.ExpirationTime, testExpirationTime);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.GetSetExpirationTimeFrozen"]' />
        public virtual void GetSetExpirationTimeFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.ExpirationTime = testExpirationTime);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.GetSetAdminKey"]' />
        public virtual void GetSetAdminKey()
        {
            var topicUpdateTransaction = new TopicUpdateTransaction { AdminKey = testAdminKey };
            Assert.Equal(topicUpdateTransaction.AdminKey, testAdminKey);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.GetSetAdminKeyFrozen"]' />
        public virtual void GetSetAdminKeyFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.AdminKey = testAdminKey);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ClearAdminKey"]' />
        public virtual void ClearAdminKey()
        {
            var topicUpdateTransaction = new TopicUpdateTransaction { AdminKey = testAdminKey };
            topicUpdateTransaction.AdminKey = null;
            Assert.Equal(topicUpdateTransaction.AdminKey, new KeyList());
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ClearAdminKeyFrozen"]' />
        public virtual void ClearAdminKeyFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.AdminKey = null);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.GetSetSubmitKey"]' />
        public virtual void GetSetSubmitKey()
        {
            var topicUpdateTransaction = new TopicUpdateTransaction { SubmitKey = testSubmitKey };
            Assert.Equal(topicUpdateTransaction.SubmitKey, testSubmitKey);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.GetSetSubmitKeyFrozen"]' />
        public virtual void GetSetSubmitKeyFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.SubmitKey = testSubmitKey);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ClearSubmitKey"]' />
        public virtual void ClearSubmitKey()
        {
            var topicUpdateTransaction = new TopicUpdateTransaction { SubmitKey = testSubmitKey };
            topicUpdateTransaction.SubmitKey = null;

            Assert.Equal(topicUpdateTransaction.SubmitKey, new KeyList());
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ClearSubmitKeyFrozen"]' />
        public virtual void ClearSubmitKeyFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.SubmitKey = null);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.GetSetAutoRenewPeriod"]' />
        public virtual void GetSetAutoRenewPeriod()
        {
            var topicUpdateTransaction = new TopicUpdateTransaction { AutoRenewPeriod = testAutoRenewPeriod };
            Assert.Equal(topicUpdateTransaction.AutoRenewPeriod, testAutoRenewPeriod);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.GetSetAutoRenewPeriodFrozen"]' />
        public virtual void GetSetAutoRenewPeriodFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.AutoRenewPeriod = testAutoRenewPeriod);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.GetSetAutoRenewAccountId"]' />
        public virtual void GetSetAutoRenewAccountId()
        {
            var topicUpdateTransaction = new TopicUpdateTransaction { AutoRenewAccountId = testAutoRenewAccountId };
            Assert.Equal(topicUpdateTransaction.AutoRenewAccountId, testAutoRenewAccountId);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.GetSetAutoRenewAccountIdFrozen"]' />
        public virtual void GetSetAutoRenewAccountIdFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.AutoRenewAccountId = testAutoRenewAccountId);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ClearAutoRenewAccountId"]' />
        public virtual void ClearAutoRenewAccountId()
        {
            var topicUpdateTransaction = new TopicUpdateTransaction { AutoRenewAccountId = testAutoRenewAccountId };
            topicUpdateTransaction.AutoRenewAccountId = null;
            Assert.Equal(topicUpdateTransaction.AutoRenewAccountId, new AccountId(0, 0, 0));
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ClearAutoRenewAccountIdFrozen"]' />
        public virtual void ClearAutoRenewAccountIdFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.AutoRenewAccountId = null);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ShouldSetFeeScheduleKey"]' />
        public virtual void ShouldSetFeeScheduleKey()
        {
            PrivateKey feeScheduleKey = PrivateKey.GenerateECDSA();
            TopicUpdateTransaction topicUpdateTransaction = new TopicUpdateTransaction
            {
                FeeScheduleKey = feeScheduleKey
            };
            Assert.Equal(topicUpdateTransaction.FeeScheduleKey.ToString(), feeScheduleKey.ToString());
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ShouldSetFeeExemptKeys"]' />
        public virtual void ShouldSetFeeExemptKeys()
        {
            List<PrivateKey> feeExemptKeys = [PrivateKey.GenerateECDSA(), PrivateKey.GenerateECDSA()];
            TopicUpdateTransaction topicUpdateTransaction = new()
            {
                FeeExemptKeys = new (feeExemptKeys.Select(_ => _ as Key))
            };
            Assert.Equal(topicUpdateTransaction.FeeExemptKeys, feeExemptKeys);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ShouldAddFeeExemptKeyToEmptyList"]' />
        public virtual void ShouldAddFeeExemptKeyToEmptyList()
        {
            TopicUpdateTransaction topicUpdateTransaction = new TopicUpdateTransaction();
            PrivateKey feeExemptKeyToBeAdded = PrivateKey.GenerateECDSA();
            topicUpdateTransaction.FeeExemptKeys.Add(feeExemptKeyToBeAdded);
            Assert.Equal(topicUpdateTransaction.FeeExemptKeys.Count, 1);
            Assert.Equal(topicUpdateTransaction.FeeExemptKeys, [feeExemptKeyToBeAdded]);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ShouldAddFeeExemptKeyToList"]' />
        public virtual void ShouldAddFeeExemptKeyToList()
        {
            PrivateKey feeExemptKey = PrivateKey.GenerateECDSA();
            TopicUpdateTransaction topicUpdateTransaction = new()
            {
                FeeExemptKeys = feeExemptKey
            };

            PrivateKey feeExemptKeyToBeAdded = PrivateKey.GenerateECDSA();
            topicUpdateTransaction.FeeExemptKeys.Add(feeExemptKeyToBeAdded);
            Assert.Equal(topicUpdateTransaction.FeeExemptKeys.Count, 2);
            Assert.Equal(topicUpdateTransaction.FeeExemptKeys, [feeExemptKey, feeExemptKeyToBeAdded]);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ShouldSetCustomFees"]' />
        public virtual void ShouldSetCustomFees()
        {
            List<CustomFixedFee> customFixedFees =
            [
                new CustomFixedFee
                {
                    Amount = 1,
                    DenominatingTokenId = new TokenId(0, 0, 0)
                },
                new CustomFixedFee
                {
                    Amount = 2,
                    DenominatingTokenId = new TokenId(0, 0, 1)
                },
                new CustomFixedFee
                {
                    Amount = 3,
                    DenominatingTokenId = new TokenId(0, 0, 2)
                }
            ];
            TopicUpdateTransaction topicUpdateTransaction = new()
            {
                CustomFees = [..customFixedFees]
            };
            Assert.Equal(3, topicUpdateTransaction.CustomFees.Count);
            Assert.Equal(topicUpdateTransaction.CustomFees, customFixedFees);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ShouldAddCustomFeeToList"]' />
        public virtual void ShouldAddCustomFeeToList()
        {
            List<CustomFixedFee> customFixedFees =
            [
                new CustomFixedFee
                {
                    Amount = 1,
                    DenominatingTokenId = new TokenId(0, 0, 0)
                },
                new CustomFixedFee
                {
                    Amount = 2,
                    DenominatingTokenId = new TokenId(0, 0, 1)
                },
                new CustomFixedFee
                {
                    Amount = 3,
                    DenominatingTokenId = new TokenId(0, 0, 2)
                }
            ];
            CustomFixedFee customFixedFeeToBeAdded = new ()
            {
                Amount = 4,
                DenominatingTokenId = new TokenId(0, 0, 3)
            };
            List<CustomFixedFee> expectedCustomFees = [.. customFixedFees];
            expectedCustomFees.Add(customFixedFeeToBeAdded);
            TopicUpdateTransaction topicUpdateTransaction = new()
            {
                CustomFees = [..customFixedFees]
            };
            topicUpdateTransaction.CustomFees.Add(customFixedFeeToBeAdded);

            Assert.Equal(4, topicUpdateTransaction.CustomFees.Count);
            Assert.True(topicUpdateTransaction.CustomFees.SequenceEqual(expectedCustomFees));
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ShouldAddCustomFeeToEmptyList"]' />
        public virtual void ShouldAddCustomFeeToEmptyList()
        {
            CustomFixedFee customFixedFeeToBeAdded = new CustomFixedFee
            {
                Amount = 4,
                DenominatingTokenId = new TokenId(0, 0, 3)
            };
            TopicUpdateTransaction topicUpdateTransaction = new ();
            topicUpdateTransaction.CustomFees.Add(customFixedFeeToBeAdded);
            Assert.Equal(topicUpdateTransaction.CustomFees.Count, 1);
            Assert.Equal(topicUpdateTransaction.CustomFees, [customFixedFeeToBeAdded]);
        }
        [Fact]
        /// <include file="test-topic-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Topic.TopicUpdateTransactionTest.ShouldClearCustomFees"]' />
        public virtual void ShouldClearCustomFees()
        {
            List<CustomFixedFee> customFixedFees = 
            [
                new CustomFixedFee
                {
                    Amount = 1,
                    DenominatingTokenId = new TokenId(0, 0, 0)
                }, 
                new CustomFixedFee
                {
                    Amount = 2,
                    DenominatingTokenId = new TokenId(0, 0, 1)
                },
                new CustomFixedFee
                {
                    Amount = 3,
                    DenominatingTokenId = new TokenId(0, 0, 2)
                }
            ];
            TopicUpdateTransaction topicUpdateTransaction = new()
            {
                CustomFees = [..customFixedFees]
            };
            topicUpdateTransaction.CustomFees.Clear();
            Assert.Empty(topicUpdateTransaction.CustomFees);
        }
    }
}
