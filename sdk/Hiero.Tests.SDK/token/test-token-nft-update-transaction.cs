// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;

using Hiero.SDK;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Transactions;
using Hiero.SDK.Nfts;
using Hiero.SDK.Token;
using Hiero.SDK.Cryptography;

using Google.Protobuf;

using VerifyXunit;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Nfts
{
    /// <include file="test-token-nft-update-transaction.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Nfts.TokenUpdateNftsTransactionTest"]' />
    public class TokenUpdateNftsTransactionTest
    {
        private static readonly PrivateKey testMetadataKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private static readonly TokenId testTokenId = TokenId.FromString("4.2.0");
        private static readonly List<long> testSerialNumbers = [8, 9, 10];
        private static readonly byte[] testMetadata = [1, 2, 3, 4, 5];
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }

        private TokenUpdateNftsTransaction SpawnTestTransaction()
        {
            return new TokenUpdateNftsTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				TokenId = testTokenId,
				Metadata = testMetadata,
				Serials = [..testSerialNumbers],
				MaxTransactionFee = new Hbar(1),
            }
            .Freeze()
            .Sign(testMetadataKey);
        }
        [Fact]
        /// <include file="test-token-nft-update-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Nfts.TokenUpdateNftsTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new TokenUpdateNftsTransaction();
            var tx2 = Transaction.FromBytes<TokenUpdateNftsTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-token-nft-update-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Nfts.TokenUpdateNftsTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<TokenUpdateNftsTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-token-nft-update-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Nfts.TokenUpdateNftsTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
				TokenUpdateNfts = new Proto.Services.TokenUpdateNftsTransactionBody()
			};
            var tx = Transaction.FromScheduledTransaction<TokenUpdateNftsTransaction>(transactionBody);
            Assert.IsType<TokenUpdateNftsTransaction>(tx);
        }
        [Fact]
        /// <include file="test-token-nft-update-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Nfts.TokenUpdateNftsTransactionTest.ConstructTokenUpdateTransactionFromTransactionBodyProtobuf"]' />
        public virtual void ConstructTokenUpdateTransactionFromTransactionBodyProtobuf()
        {
            var transactionBody = new Proto.Services.TokenUpdateNftsTransactionBody
            {
                Token = testTokenId.ToProtobuf(),
                Metadata = ByteString.CopyFrom(testMetadata),
            };

            transactionBody.SerialNumbers.AddRange(testSerialNumbers);
                
            var tx = new Proto.Services.TransactionBody
            {
				TokenUpdateNfts = transactionBody
			};
            var tokenUpdateNftsTransaction = new TokenUpdateNftsTransaction(tx);
            Assert.Equal(tokenUpdateNftsTransaction.TokenId, testTokenId);
            Assert.Equal(tokenUpdateNftsTransaction.Metadata, testMetadata);
            Assert.Equal(tokenUpdateNftsTransaction.Serials, testSerialNumbers);
        }
        [Fact]
        /// <include file="test-token-nft-update-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Nfts.TokenUpdateNftsTransactionTest.GetSetTokenId"]' />
        public virtual void GetSetTokenId()
        {
            var tokenUpdateNftsTransaction = new TokenUpdateNftsTransaction
            {
				TokenId = testTokenId
			};
            
            Assert.Equal(tokenUpdateNftsTransaction.TokenId, testTokenId);
        }
        [Fact]
        /// <include file="test-token-nft-update-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Nfts.TokenUpdateNftsTransactionTest.GetSetTokenIdFrozen"]' />
        public virtual void GetSetTokenIdFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.TokenId = testTokenId);
        }
        [Fact]
        /// <include file="test-token-nft-update-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Nfts.TokenUpdateNftsTransactionTest.GetSetMetadata"]' />
        public virtual void GetSetMetadata()
        {
            var tx = SpawnTestTransaction();
            Assert.Equal(tx.Metadata, testMetadata);
        }
        [Fact]
        /// <include file="test-token-nft-update-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Nfts.TokenUpdateNftsTransactionTest.GetSetMetadataFrozen"]' />
        public virtual void GetSetMetadataFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.Metadata = testMetadata);
        }
        [Fact]
        /// <include file="test-token-nft-update-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Nfts.TokenUpdateNftsTransactionTest.GetSetSerialNumbers"]' />
        public virtual void GetSetSerialNumbers()
        {
            var tx = SpawnTestTransaction();
            Assert.Equal(tx.Serials, testSerialNumbers);
        }
        [Fact]
        /// <include file="test-token-nft-update-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Nfts.TokenUpdateNftsTransactionTest.GetSetSerialNumbersFrozen"]' />
        public virtual void GetSetSerialNumbersFrozen()
        {
            var tx = SpawnTestTransaction();
            Assert.Throws<InvalidOperationException>(() => tx.Serials.AddRange(testSerialNumbers));
        }
    }
}
