// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK;
using Hiero.SDK.Cryptography;
using Hiero.SDK.File;
using Hiero.SDK.Contract;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Transactions;
using Hiero.SDK.Systems;

using VerifyXunit;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.System
{
    /// <include file="test-system-delete-transaction.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.System.SystemDeleteTransactionTest"]' />
    public class SystemDeleteTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private static readonly FileId testFileId = FileId.FromString("4.2.0");
        private static readonly ContractId testContractId = ContractId.FromString("0.6.9");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        public virtual void ShouldSerializeFile()
        {
            Verifier.Verify(SpawnTestTransactionFile().ToString());
        }

        private SystemDeleteTransaction SpawnTestTransactionFile()
        {
            return new SystemDeleteTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				ContractId = ContractId.FromString("0.0.444"),
				ExpirationTime = validStart,
				MaxTransactionFee = new Hbar(1),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }

        public virtual void ShouldSerializeContract()
        {
            Verifier.Verify(SpawnTestTransactionContract().ToString());
        }
        [Fact]
        /// <include file="test-system-delete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemDeleteTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new SystemDeleteTransaction();
            var tx2 = Transaction.FromBytes<SystemDeleteTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }

        private SystemDeleteTransaction SpawnTestTransactionContract()
        {
            return new SystemDeleteTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				ContractId = ContractId.FromString("0.0.444"),
				ExpirationTime = validStart,
				MaxTransactionFee = new Hbar(1),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-system-delete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemDeleteTransactionTest.ShouldBytesContract"]' />
        public virtual void ShouldBytesContract()
        {
            var tx = SpawnTestTransactionContract();
            var tx2 = Transaction.FromBytes<SystemDeleteTransaction>(tx.ToBytes());
            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-system-delete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemDeleteTransactionTest.ShouldBytesFile"]' />
        public virtual void ShouldBytesFile()
        {
            var tx = SpawnTestTransactionFile();
            var tx2 = Transaction.FromBytes<SystemDeleteTransaction>(tx.ToBytes());
            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-system-delete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemDeleteTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
				SystemDelete = new Proto.Services.SystemDeleteTransactionBody()
			};
            var tx = Transaction.FromScheduledTransaction<SystemDeleteTransaction>(transactionBody);

            Assert.IsType<SystemDeleteTransaction>(tx);
        }
        [Fact]
        /// <include file="test-system-delete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemDeleteTransactionTest.ConstructSystemDeleteTransactionFromTransactionBodyProtobuf"]' />
        public virtual void ConstructSystemDeleteTransactionFromTransactionBodyProtobuf()
        {
            var transactionBodyWithFileId = new Proto.Services.SystemDeleteTransactionBody
            {
                FileId = testFileId.ToProtobuf(),
                ExpirationTime = new Proto.Services.TimestampSeconds { Seconds = validStart.ToUnixTimeSeconds() }
            };
            var transactionBodyWithContractId = new Proto.Services.SystemDeleteTransactionBody
            {
                ContractId = testContractId.ToProtobuf(),
                ExpirationTime = new Proto.Services.TimestampSeconds { Seconds = validStart.ToUnixTimeSeconds() }
            };
            var txWithFileId = new Proto.Services.TransactionBody
            {
                SystemDelete = transactionBodyWithFileId
            };
            var systemDeleteTransactionWithFileId = new SystemDeleteTransaction(txWithFileId);
            var txWithContractId = new Proto.Services.TransactionBody
            {
                SystemDelete = transactionBodyWithContractId
            };
            var systemDeleteTransactionWithContractId = new SystemDeleteTransaction(txWithContractId);

            Assert.NotNull(systemDeleteTransactionWithFileId.FileId);
            Assert.Equal(systemDeleteTransactionWithFileId.FileId, testFileId);
            Assert.Null(systemDeleteTransactionWithFileId.ContractId);
            Assert.Equal(systemDeleteTransactionWithFileId.ExpirationTime?.ToUnixTimeSeconds(), validStart.ToUnixTimeSeconds());
            Assert.Null(systemDeleteTransactionWithContractId.FileId);
            Assert.NotNull(systemDeleteTransactionWithContractId.ContractId);
            Assert.Equal(systemDeleteTransactionWithContractId.ContractId, testContractId);
            Assert.Equal(systemDeleteTransactionWithContractId.ExpirationTime?.ToUnixTimeSeconds(), validStart.ToUnixTimeSeconds());
        }
        [Fact]
        /// <include file="test-system-delete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemDeleteTransactionTest.GetSetFileId"]' />
        public virtual void GetSetFileId()
        {
            var systemDeleteTransaction = new SystemDeleteTransaction
            {
				FileId = testFileId
			};
            Assert.NotNull(systemDeleteTransaction.FileId);
            Assert.Equal(systemDeleteTransaction.FileId, testFileId);
        }
        [Fact]
        /// <include file="test-system-delete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemDeleteTransactionTest.GetSetFileIdFrozen"]' />
        public virtual void GetSetFileIdFrozen()
        {
            var tx = SpawnTestTransactionFile();
            Assert.Throws<InvalidOperationException>(() => tx.FileId = testFileId);
        }
        [Fact]
        /// <include file="test-system-delete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemDeleteTransactionTest.GetSetContractId"]' />
        public virtual void GetSetContractId()
        {
            var systemDeleteTransaction = new SystemDeleteTransaction
            {
				ContractId = testContractId
			};
            Assert.NotNull(systemDeleteTransaction.ContractId);
            Assert.Equal(systemDeleteTransaction.ContractId, testContractId);
        }

        public virtual void GetSetContractIdFrozen()
        {
            var tx = SpawnTestTransactionContract();
            Assert.Throws<InvalidOperationException>(() => tx.ContractId = testContractId);
        }
        [Fact]
        /// <include file="test-system-delete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemDeleteTransactionTest.GetSetExpirationTime"]' />
        public virtual void GetSetExpirationTime()
        {
            var systemDeleteTransaction = new SystemDeleteTransaction
            {
				ExpirationTime = validStart
			};
            Assert.NotNull(systemDeleteTransaction.ExpirationTime);
            Assert.Equal(systemDeleteTransaction.ExpirationTime?.ToUnixTimeSeconds(), validStart.ToUnixTimeSeconds());
        }
        [Fact]
        /// <include file="test-system-delete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemDeleteTransactionTest.GetSetExpirationTimeFrozen"]' />
        public virtual void GetSetExpirationTimeFrozen()
        {
            var tx = SpawnTestTransactionFile();
            Assert.Throws<InvalidOperationException>(() => tx.ExpirationTime = validStart);
        }
        [Fact]
        /// <include file="test-system-delete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemDeleteTransactionTest.ResetFileId"]' />
        public virtual void ResetFileId()
        {
            var systemDeleteTransaction = new SystemDeleteTransaction();
            systemDeleteTransaction.FileId = testFileId;
            systemDeleteTransaction.ContractId = testContractId;
            Assert.Null(systemDeleteTransaction.FileId);
            Assert.NotNull(systemDeleteTransaction.ContractId);
        }
        [Fact]
        /// <include file="test-system-delete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemDeleteTransactionTest.ResetContractId"]' />
        public virtual void ResetContractId()
        {
            var systemDeleteTransaction = new SystemDeleteTransaction();
            systemDeleteTransaction.ContractId = testContractId;
            systemDeleteTransaction.FileId = testFileId;
            Assert.Null(systemDeleteTransaction.ContractId);
            Assert.NotNull(systemDeleteTransaction.FileId);
        }
    }
}
