// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Ethereum;

using Org.BouncyCastle.Utilities.Encoders;

namespace Hiero.Tests.SDK.Ethereum
{
    /// <include file="test-ethereum-transaction-data-legacy.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Ethereum.EthereumTransactionDataLegacyTest"]' />
    public class EthereumTransactionDataLegacyTest
    {
        // https://github.com/hashgraph/hedera-services/blob/1e01d9c6b8923639b41359c55413640b589c4ec7/hapi-utils/src/test/java/com/hedera/services/ethereum/EthTxDataTest.java#L49
        static readonly string RAW_TX_TYPE_0 = "f864012f83018000947e3a9eaf9bcc39e2ffa38eb30bf7a93feacbc18180827653820277a0f9fbff985d374be4a55f296915002eec11ac96f1ce2df183adf992baa9390b2fa00c1e867cc960d9c74ec2e6a662b7908ec4c8cc9f3091e886bcefbeb2290fb792";
        static readonly string RAW_TX_TYPE_0_TRIMMED_LAST_BYTES = "f864012f83018000947e3a9eaf9bcc39e2ffa38eb30bf7a93feacbc18180827653820277a0f9fbff985d374be4a55f296915002eec11ac96f1ce2df183adf992baa9390b2fa00c1e867cc960d9c74ec2e6a662b7908ec4c8cc9f3091e886bcefbeb2290000";
        static readonly string RAW_TX_TYPE_2 = "02f87082012a022f2f83018000947e3a9eaf9bcc39e2ffa38eb30bf7a93feacbc181880de0b6b3a764000083123456c001a0df48f2efd10421811de2bfb125ab75b2d3c44139c4642837fb1fccce911fd479a01aaf7ae92bee896651dfc9d99ae422a296bf5d9f1ca49b2d96d82b79eb112d66";

        [Fact]
        /// <include file="test-ethereum-transaction-data-legacy.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Ethereum.EthereumTransactionDataLegacyTest.LegacyToFromBytes"]' />
        public virtual void LegacyToFromBytes()
        {
            var data = (EthereumTransactionDataLegacy)EthereumTransactionData.FromBytes(Hex.Decode(RAW_TX_TYPE_0));
            Assert.Equal(RAW_TX_TYPE_0, Hex.ToHexString(data.ToBytes()));

            // Chain ID is not part of the legacy ethereum transaction, so why are you calculating and checking it?
            // assertEquals("012a", Hex.toHexString(data.chainId()));
            Assert.Equal("01", Hex.ToHexString(data.Nonce));
            Assert.Equal("2f", Hex.ToHexString(data.GasPrice));
            Assert.Equal("018000", Hex.ToHexString(data.GasLimit));
            Assert.Equal("7e3a9eaf9bcc39e2ffa38eb30bf7a93feacbc181", Hex.ToHexString(data.To));
            Assert.Equal("", Hex.ToHexString(data.Value));
            Assert.Equal("7653", Hex.ToHexString(data.CallData));
            Assert.Equal("0277", Hex.ToHexString(data.V));
            Assert.Equal("f9fbff985d374be4a55f296915002eec11ac96f1ce2df183adf992baa9390b2f", Hex.ToHexString(data.R));
            Assert.Equal("0c1e867cc960d9c74ec2e6a662b7908ec4c8cc9f3091e886bcefbeb2290fb792", Hex.ToHexString(data.S)); 
            // We don't currently support a way to get the ethereum has, but we probably should
            // assertEquals("9ffbd69c44cf643ed8d1e756b505e545e3b5dd3a6b5ef9da1d8eca6679706594",
            //    Hex.toHexString(data.getEthereumHash()));
        }
        [Fact]
        /// <include file="test-ethereum-transaction-data-legacy.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Ethereum.EthereumTransactionDataLegacyTest.Eip1559ToFromBytes"]' />
        public virtual void Eip1559ToFromBytes()
        {
            var data = (EthereumTransactionDataEip1559)EthereumTransactionData.FromBytes(Hex.Decode(RAW_TX_TYPE_2));
            Assert.Equal(RAW_TX_TYPE_2, Hex.ToHexString(data.ToBytes()));
            Assert.Equal("012a", Hex.ToHexString(data.ChainId));
            Assert.Equal("02", Hex.ToHexString(data.Nonce));
            Assert.Equal("2f", Hex.ToHexString(data.MaxPriorityGas));
            Assert.Equal("2f", Hex.ToHexString(data.MaxGas));
            Assert.Equal("018000", Hex.ToHexString(data.GasLimit));
            Assert.Equal("7e3a9eaf9bcc39e2ffa38eb30bf7a93feacbc181", Hex.ToHexString(data.To));
            Assert.Equal("0de0b6b3a7640000", Hex.ToHexString(data.Value));
            Assert.Equal("123456", Hex.ToHexString(data.CallData));
            Assert.Equal("", Hex.ToHexString(data.AccessList));
            Assert.Equal("01", Hex.ToHexString(data.RecoveryId));
            Assert.Equal("df48f2efd10421811de2bfb125ab75b2d3c44139c4642837fb1fccce911fd479", Hex.ToHexString(data.R));
            Assert.Equal("1aaf7ae92bee896651dfc9d99ae422a296bf5d9f1ca49b2d96d82b79eb112d66", Hex.ToHexString(data.S));
        }
    }
}