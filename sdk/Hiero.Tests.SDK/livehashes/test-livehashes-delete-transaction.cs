// SPDX-License-Identifier: Apache-2.0
using System;
using System.Text;

using Hiero.SDK.Cryptography;
using Hiero.SDK.LiveHashes;
using Hiero.SDK.Transactions;
using Hiero.SDK.Cryptocurrency;

using VerifyXunit;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.LiveHashes
{
    /// <include file="test-livehashes-delete-transaction.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.LiveHashes.LiveHashDeleteTransactionTest"]' />
    public class LiveHashDeleteTransactionTest
    {
        private static readonly PrivateKey privateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }

        private LiveHashDeleteTransaction SpawnTestTransaction()
        {
            return new LiveHashDeleteTransaction()
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				AccountId = AccountId.FromString("0.0.100"),
				Hash = Encoding.UTF8.GetBytes("hash"),
			}
            .Freeze()
            .Sign(privateKey);
        }
        [Fact]
        /// <include file="test-livehashes-delete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.LiveHashes.LiveHashDeleteTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<LiveHashDeleteTransaction>(tx.ToBytes());
            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-livehashes-delete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.LiveHashes.LiveHashDeleteTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new LiveHashDeleteTransaction();
            var tx2 = Transaction.FromBytes<LiveHashDeleteTransaction>(tx.ToBytes());
            Assert.Equal(tx2.ToString(), tx.ToString());
        }
    }
}
