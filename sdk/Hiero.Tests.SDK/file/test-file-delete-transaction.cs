// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK.Cryptography;
using Hiero.SDK.File;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Transactions;

using VerifyXunit;
using Hiero.SDK;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.File
{
    /// <include file="test-file-delete-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.File.FileDeleteTransactionTest"]' />
    public class FileDeleteTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        [Fact] public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }
        [Fact]
        /// <include file="test-file-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.File.FileDeleteTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new FileDeleteTransaction();
            var tx2 = Transaction.FromBytes<FileDeleteTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }

        private FileDeleteTransaction SpawnTestTransaction()
        {
            return new FileDeleteTransaction()
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				FileId = FileId.FromString("0.0.6006"),
				MaxTransactionFee = Hbar.FromTinybars(100000),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-file-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.File.FileDeleteTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<FileDeleteTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-file-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.File.FileDeleteTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
                FileDelete = new Proto.Services.FileDeleteTransactionBody()
            };

            var tx = Transaction.FromScheduledTransaction<FileDeleteTransaction>(transactionBody);
            
            Assert.IsType<FileDeleteTransaction>(tx);
        }
    }
}
