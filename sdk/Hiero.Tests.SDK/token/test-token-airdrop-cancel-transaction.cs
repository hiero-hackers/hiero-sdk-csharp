// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using System.Linq;

using Hiero.SDK;
using Hiero.SDK.Core;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Token;
using Hiero.SDK.Airdrops;
using Hiero.SDK.Nfts;
using Hiero.SDK.Transactions;
using Hiero.SDK.Cryptography;

using VerifyXunit;

namespace Hiero.Tests.SDK.Token
{
    /// <include file="test-token-airdrop-cancel-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Token.TokenCancelAirdropTransactionTest"]' />
    public class TokenCancelAirdropTransactionTest
    {
        private static readonly PrivateKey privateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);
        private TokenCancelAirdropTransaction transaction;

        private TokenCancelAirdropTransaction SpawnTestTransaction()
        {
            List<PendingAirdropId> pendingAirdropIds = [];
            pendingAirdropIds.Add(new PendingAirdropId(new AccountId(0, 0, 457), new AccountId(0, 0, 456), new TokenId(0, 0, 123)));
            pendingAirdropIds.Add(new PendingAirdropId(new AccountId(0, 0, 457), new AccountId(0, 0, 456), new NftId(new TokenId(0, 0, 1234), 123)));
            
            return new TokenCancelAirdropTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				MaxTransactionFee = Hbar.FromTinybars(100000),
				PendingAirdropIds = [..pendingAirdropIds],
			}
            .Freeze()
            .Sign(privateKey);
        }

        [Fact] public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }
        [Fact]
        /// <include file="test-token-airdrop-cancel-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenCancelAirdropTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new TokenCancelAirdropTransaction();
            var tx2 = Transaction.FromBytes<TokenCancelAirdropTransaction>(tx.ToBytes());
            
            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        public TokenCancelAirdropTransactionTest()
        {
            transaction = new TokenCancelAirdropTransaction();
        }
        [Fact]
        /// <include file="test-token-airdrop-cancel-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenCancelAirdropTransactionTest.TestConstructorSetsDefaultMaxTransactionFee"]' />
        public virtual void TestConstructorSetsDefaultMaxTransactionFee()
        {
            Assert.Equal(Hbar.From(1), transaction.DefaultMaxTransactionFee);
        }
        [Fact]
        /// <include file="test-token-airdrop-cancel-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenCancelAirdropTransactionTest.TestGetAndSetPendingAirdropIds"]' />
        public virtual void TestGetAndSetPendingAirdropIds()
        {
            List<PendingAirdropId> pendingAirdropIds = [];
            pendingAirdropIds.Add(new PendingAirdropId(new AccountId(0, 0, 457), new AccountId(0, 0, 456), new TokenId(0, 0, 123)));
            pendingAirdropIds.Add(new PendingAirdropId(new AccountId(0, 0, 457), new AccountId(0, 0, 456), new NftId(new TokenId(0, 0, 1234), 123)));
            transaction.PendingAirdropIds.Set(pendingAirdropIds);
            Assert.Equal(pendingAirdropIds, transaction.PendingAirdropIds);
        }
        [Fact]
        /// <include file="test-token-airdrop-cancel-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenCancelAirdropTransactionTest.TestClearPendingAirdropIds"]' />
        public virtual void TestClearPendingAirdropIds()
        {
            List<PendingAirdropId> pendingAirdropIds = [];
            PendingAirdropId pendingAirdropId = new (new AccountId(0, 0, 457), new AccountId(0, 0, 456), new TokenId(0, 0, 123));
            pendingAirdropIds.Add(pendingAirdropId);
            transaction.PendingAirdropIds.Set(pendingAirdropIds);
            transaction.PendingAirdropIds.Clear();
            
            Assert.Equal(transaction.PendingAirdropIds.Count, 0);
        }
        [Fact]
        /// <include file="test-token-airdrop-cancel-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenCancelAirdropTransactionTest.TestAddAllPendingAirdrops"]' />
        public virtual void TestAddAllPendingAirdrops()
        {
            PendingAirdropId pendingAirdropId1 = new (new AccountId(0, 0, 457), new AccountId(0, 0, 456), new TokenId(0, 0, 123));
            PendingAirdropId pendingAirdropId2 = new (new AccountId(0, 0, 458), new AccountId(0, 0, 459), new TokenId(0, 0, 123));
            
            transaction.PendingAirdropIds.Add(pendingAirdropId1);
            transaction.PendingAirdropIds.Add(pendingAirdropId2);
            
            Assert.Equal(2, transaction.PendingAirdropIds.Count);
            Assert.True(transaction.PendingAirdropIds.Contains(pendingAirdropId1));
            Assert.True(transaction.PendingAirdropIds.Contains(pendingAirdropId2));
        }
        [Fact]
        /// <include file="test-token-airdrop-cancel-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenCancelAirdropTransactionTest.TestBuildTransactionBody"]' />
        public virtual void TestBuildTransactionBody()
        {
            PendingAirdropId pendingAirdropId = new (new AccountId(0, 0, 457), new AccountId(0, 0, 456), new NftId(new TokenId(0, 0, 1234), 123));
            transaction.PendingAirdropIds.Add(pendingAirdropId);
            Proto.Services.TokenCancelAirdropTransactionBody builder = transaction.ToProtobuf();

            Assert.Equal(1, builder.PendingAirdrops.Count);
            Assert.Equal(pendingAirdropId.ToProtobuf(), builder.PendingAirdrops[0]);
        }
        [Fact]
        /// <include file="test-token-airdrop-cancel-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenCancelAirdropTransactionTest.TestGetMethodDescriptor"]' />
        public virtual void TestGetMethodDescriptor()
        {
            //Assert.Equal(TokenServiceGrpc.GetCancelAirdropMethod(), transaction.GetMethodDescriptor());
        }
        [Fact]
        /// <include file="test-token-airdrop-cancel-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenCancelAirdropTransactionTest.TestOnFreeze"]' />
        public virtual void TestOnFreeze()
        {
            var bodyBuilder = new Proto.Services.TransactionBody();
            
            transaction.OnFreeze(bodyBuilder);

            Assert.True(bodyBuilder.TokenCancelAirdrop is not null);
        }
        [Fact]
        /// <include file="test-token-airdrop-cancel-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenCancelAirdropTransactionTest.TestOnScheduled"]' />
        public virtual void TestOnScheduled()
        {
            Proto.Services.SchedulableTransactionBody scheduled = new ();
            
            transaction.OnScheduled(scheduled);
            
            Assert.True(scheduled.TokenCancelAirdrop is not null);
        }
    }
}
