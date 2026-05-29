// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK.Cryptography;
using Hiero.SDK.Transactions;
using Hiero.SDK.File;
using Hiero.SDK.Cryptocurrency;

using VerifyXunit;
using Hiero.SDK;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.File
{
    /// <include file="test-file-create-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.File.FileCreateTransactionTest"]' />
    public class FileCreateTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        [Fact] 
        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }
        [Fact]
        /// <include file="test-file-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.File.FileCreateTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new FileCreateTransaction();
            var tx2 = Transaction.FromBytes<FileCreateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }

        private FileCreateTransaction SpawnTestTransaction()
        {
            return new FileCreateTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				Contents = [1, 2, 3, 4],
				ExpirationTime = NodaTime.Instant.FromUnixTimeMilliseconds(1554158728),
				Keys = KeyList.Of(null, unusedPrivateKey),
				MaxTransactionFee = Hbar.FromTinybars(100000),
				FileMemo = "Hello memo",
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-file-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.File.FileCreateTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<FileCreateTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-file-create-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.File.FileCreateTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
				FileCreate = new Proto.Services.FileCreateTransactionBody()
			};
            var tx = Transaction.FromScheduledTransaction<FileCreateTransaction>(transactionBody);

            Assert.IsType<FileCreateTransaction>(tx);
        }
    }
}
