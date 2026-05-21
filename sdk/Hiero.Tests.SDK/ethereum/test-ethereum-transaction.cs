// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Ethereum;
using Hiero.SDK.Transactions;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.File;
using Hiero.SDK.Cryptography;

using Org.BouncyCastle.Utilities.Encoders;

using System;

using VerifyXunit;
using Hiero.SDK;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Ethereum
{
    /// <include file="test-ethereum-transaction.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Ethereum.EthereumTransactionTest"]' />
    public class EthereumTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        public virtual EthereumTransaction SpawnTestTransaction()
        {
            return new EthereumTransaction()
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				EthereumData = Hex.Decode("deadbeef"),
				CallDataFileId = FileId.FromString("4.5.6"),
				MaxGasAllowanceHbar = Hbar.FromString("3"),
				MaxTransactionFee = new Hbar(1),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-ethereum-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Ethereum.EthereumTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new EthereumTransaction();
            var tx2 = Transaction.FromBytes<EthereumTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }

        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }
        [Fact]
        /// <include file="test-ethereum-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Ethereum.EthereumTransactionTest.ShouldBytesNft"]' />
        public virtual void ShouldBytesNft()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<EthereumTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx2.ToString());
        }
    }
}
