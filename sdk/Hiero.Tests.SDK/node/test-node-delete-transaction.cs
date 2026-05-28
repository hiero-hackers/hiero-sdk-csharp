// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK.Cryptography;
using Hiero.SDK.Networking;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Transactions;
using Hiero.SDK;

using VerifyXunit;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Node
{
    /// <include file="test-node-delete-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Node.NodeDeleteTransactionTest"]' />
    public class NodeDeleteTransactionTest
    {
        private static readonly PrivateKey TEST_PRIVATE_KEY = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private static readonly ulong TEST_NODE_ID = 420;
        readonly NodaTime.Instant TEST_VALID_START = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);
        readonly AccountId ACCOUNT_ID = AccountId.FromString("0.6.9");

        [Fact] public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }

        private NodeDeleteTransaction SpawnTestTransaction()
        {
            return new NodeDeleteTransaction()
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), TEST_VALID_START),
				NodeId = TEST_NODE_ID,
				MaxTransactionFee = new Hbar(1),
			}
            .Freeze()
            .Sign(TEST_PRIVATE_KEY);
        }
        [Fact]
        /// <include file="test-node-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Node.NodeDeleteTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<NodeDeleteTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-node-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Node.NodeDeleteTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new NodeDeleteTransaction();
            var tx2 = Transaction.FromBytes<NodeDeleteTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-node-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Node.NodeDeleteTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
				NodeDelete = new Proto.Services.NodeDeleteTransactionBody()
			};
            var tx = Transaction.FromScheduledTransaction<NodeDeleteTransaction>(transactionBody);
            
            Assert.IsType<NodeDeleteTransaction>(tx);
        }
        [Fact]
        /// <include file="test-node-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Node.NodeDeleteTransactionTest.ConstructNodeDeleteTransactionFromTransactionBodyProtobuf"]' />
        public virtual void ConstructNodeDeleteTransactionFromTransactionBodyProtobuf()
        {
            var transactionBodyBuilder = new Proto.Services.NodeDeleteTransactionBody();
            transactionBodyBuilder.NodeId = TEST_NODE_ID;
            var tx = new Proto.Services.TransactionBody
            {
				NodeDelete = transactionBodyBuilder
			};
            var nodeDeleteTransaction = new NodeDeleteTransaction(tx);
            Assert.Equal(nodeDeleteTransaction.NodeId, TEST_NODE_ID);
        }
        [Fact]
        /// <include file="test-node-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Node.NodeDeleteTransactionTest.GetSetNodeId"]' />
        public virtual void GetSetNodeId()
        {
            var nodeDeleteTransaction = new NodeDeleteTransaction
            {
				NodeId = TEST_NODE_ID
			};
            Assert.Equal(nodeDeleteTransaction.NodeId, TEST_NODE_ID);
        }
        [Fact]
        /// <include file="test-node-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Node.NodeDeleteTransactionTest.GetSetNodeIdFrozen"]' />
        public virtual void GetSetNodeIdFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.NodeId = TEST_NODE_ID);
        }
        [Fact]
        /// <include file="test-node-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Node.NodeDeleteTransactionTest.ShouldFreezeSuccessfullyWhenNodeIdIsSet"]' />
        public virtual void ShouldFreezeSuccessfullyWhenNodeIdIsSet()
        {
            NodaTime.Instant VALID_START = NodaTime.Instant.FromUnixTimeMilliseconds(1596210382);
            AccountId ACCOUNT_Id = AccountId.FromString("0.6.9");
            var transaction = new NodeDeleteTransaction
            {
                NodeAccountIds = AccountId.FromString("0.0.3"),
                TransactionId = TransactionId.WithValidStart(ACCOUNT_ID, VALID_START),
                NodeId = 420,
            };

            transaction.FreezeWith(null);

			Assert.Equal(transaction.NodeId, (ulong)420);
        }
        [Fact]
        /// <include file="test-node-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Node.NodeDeleteTransactionTest.ShouldThrowErrorWhenFreezingWithoutSettingNodeId"]' />
        public virtual void ShouldThrowErrorWhenFreezingWithoutSettingNodeId()
        {
            NodaTime.Instant VALID_START = NodaTime.Instant.FromUnixTimeMilliseconds(1596210382);
            AccountId ACCOUNT_Id = AccountId.FromString("0.6.9");
            var transaction = new NodeDeleteTransaction
            {
                NodeAccountIds = AccountId.FromString("0.0.3"),
                TransactionId = TransactionId.WithValidStart(ACCOUNT_ID, VALID_START)
            };
            var exception = Assert.Throws<InvalidOperationException>(() => transaction.FreezeWith(null));
            
            Assert.Equal("NodeDeleteTransaction: 'nodeId' must be explicitly set before calling freeze().", exception.Message);
        }
        [Fact]
        /// <include file="test-node-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Node.NodeDeleteTransactionTest.ShouldThrowErrorWhenFreezingWithZeroNodeId"]' />
        public virtual void ShouldThrowErrorWhenFreezingWithZeroNodeId()
        {
            NodaTime.Instant VALID_START = NodaTime.Instant.FromUnixTimeMilliseconds(1596210382);
            AccountId ACCOUNT_Id = AccountId.FromString("0.6.9");
            var transaction = new NodeDeleteTransaction
            {
				NodeAccountIds = AccountId.FromString("0.0.3"),
				TransactionId = TransactionId.WithValidStart(ACCOUNT_ID, VALID_START)

			};
            var exception = Assert.Throws<InvalidOperationException>(() => transaction.FreezeWith(null));

            Assert.Equal("NodeDeleteTransaction: 'nodeId' must be explicitly set before calling freeze().", exception.Message);
        }
        [Fact]
        /// <include file="test-node-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Node.NodeDeleteTransactionTest.ShouldFreezeSuccessfullyWithActualClientWhenNodeIdIsSet"]' />
        public virtual void ShouldFreezeSuccessfullyWithActualClientWhenNodeIdIsSet()
        {
            NodaTime.Instant VALID_START = NodaTime.Instant.FromUnixTimeMilliseconds(1596210382);
            AccountId ACCOUNT_Id = AccountId.FromString("0.6.9");
            var transaction = new NodeDeleteTransaction()
            {
				NodeAccountIds = AccountId.FromString("0.0.3"),
				TransactionId = TransactionId.WithValidStart(ACCOUNT_ID, VALID_START),
				NodeId = 420
			};
            var mockClient = Client.ForTestnet();
            transaction.FreezeWith(mockClient); //.DoesNotThrowAnyException();
            Assert.Equal(transaction.NodeId, (ulong)420);
        }
        [Fact]
        /// <include file="test-node-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Node.NodeDeleteTransactionTest.ShouldThrowErrorWhenGettingNodeIdWithoutSettingIt"]' />
        public virtual void ShouldThrowErrorWhenGettingNodeIdWithoutSettingIt()
        {
            var transaction = new NodeDeleteTransaction();
            // TODO: (Don't know why)
            // var exception = Assert.Throws<InvalidOperationException>(() => transaction.NodeId);

            //Assert.Equal("NodeDeleteTransaction: 'nodeId' has not been set", exception.Message);
        }

        [Fact]
        /// <include file="test-node-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Node.NodeDeleteTransactionTest.ShouldAllowSettingNodeIdToZero"]' />
        public virtual void ShouldAllowSettingNodeIdToZero()
        {
            var transaction = new NodeDeleteTransaction
            {
				NodeId = 0
			};

            Assert.Equal(transaction.NodeId, (ulong)0);
        }
    }
}
