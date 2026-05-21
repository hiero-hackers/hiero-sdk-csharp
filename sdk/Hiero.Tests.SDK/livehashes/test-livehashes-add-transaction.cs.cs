// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Core;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Cryptography;
using Hiero.SDK.LiveHashes;
using Hiero.SDK.Transactions;

using System;
using System.Text;

using VerifyXunit;

namespace Hiero.Tests.SDK.LiveHashes
{
    /// <include file="test-livehashes-add-transaction.cs.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.LiveHashes.LiveHashAddTransactionTest"]' />
    public class LiveHashAddTransactionTest
    {
        private static readonly PrivateKey privateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }

        private LiveHashAddTransaction SpawnTestTransaction()
        {
            return new LiveHashAddTransaction()
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				AccountId = AccountId.FromString("0.0.100"),
				Hash = Encoding.UTF8.GetBytes("hash"),
				Keys = KeyList.Of(null, privateKey),
			    TransactionValidDuration = NodaTime.Duration.FromDays(30),
			}
            .Freeze()
            .Sign(privateKey);
        }
        [Fact]
        /// <include file="test-livehashes-add-transaction.cs.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.LiveHashes.LiveHashAddTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new LiveHashAddTransaction();
            var tx2 = Transaction.FromBytes<LiveHashAddTransaction>(tx.ToBytes());
            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-livehashes-add-transaction.cs.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.LiveHashes.LiveHashAddTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<LiveHashAddTransaction>(tx.ToBytes());
            Assert.Equal(tx2.ToString(), tx.ToString());
        }
    }
}
