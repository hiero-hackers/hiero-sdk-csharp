// SPDX-License-Identifier: Apache-2.0
using Google.Protobuf;

using Hiero.SDK.Cryptography;
using Hiero.SDK.File;
using Hiero.SDK.Transactions;
using Hiero.SDK.Cryptocurrency;

using System;

using VerifyXunit;
using Hiero.SDK;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.File
{
    /// <include file="test-file-update-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.File.FileUpdateTransactionTest"]' />
    public class FileUpdateTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly DateTimeOffset validStart = DateTimeOffset.FromUnixTimeMilliseconds(1554158542);

        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }

        private FileUpdateTransaction SpawnTestTransaction()
        {
            return new FileUpdateTransaction()
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				FileId = FileId.FromString("0.0.6006"),
				ExpirationTime = DateTimeOffset.FromUnixTimeMilliseconds(1554158728),
				Contents = ByteString.CopyFrom([1, 2, 3, 4, 5]),
				MaxTransactionFee = Hbar.FromTinybars(100000),
				Keys = KeyList.Of(null, unusedPrivateKey),
				FileMemo = "Hello memo",
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-file-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.File.FileUpdateTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<FileUpdateTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-file-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.File.FileUpdateTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new FileUpdateTransaction();
            var tx2 = Transaction.FromBytes<FileUpdateTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-file-update-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.File.FileUpdateTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
                FileUpdate = new Proto.Services.FileUpdateTransactionBody { }
            };
            var tx = Transaction.FromScheduledTransaction<FileUpdateTransaction>(transactionBody);

            Assert.IsType<FileUpdateTransaction>(tx);
        }
    }
}
