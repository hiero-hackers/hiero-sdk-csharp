// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Networking;
using Hiero.SDK.Nfts;
using Hiero.SDK.Token;

using Org.BouncyCastle.Utilities.Encoders;

using System;

using VerifyXunit;

namespace Hiero.Tests.SDK.Nfts
{
    /// <include file="test-token-nft-info.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Nfts.TokenNftInfoTest"]' />
    public class TokenNftInfoTest
    {
        static readonly NodaTime.Instant creationTime = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        private static TokenNftInfo SpawnTokenNftInfoExample(AccountId spenderAccountId)
        {
            return new TokenNftInfo(TokenId.FromString("1.2.3").Nft(4), AccountId.FromString("5.6.7"), creationTime, Hex.Decode("deadbeef"), LedgerId.MAINNET, spenderAccountId);
        }
        [Fact]
        /// <include file="test-token-nft-info.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Nfts.TokenNftInfoTest.ShouldSerialize"]' />
        public virtual void ShouldSerialize()
        {
            var originalTokenInfo = SpawnTokenNftInfoExample(AccountId.FromString("8.9.10"));
            byte[] tokenInfoBytes = originalTokenInfo.ToBytes();
            var copyTokenInfo = TokenNftInfo.FromBytes(tokenInfoBytes);

            Assert.Equal(copyTokenInfo.ToString(), originalTokenInfo.ToString());
            Verifier.Verify(originalTokenInfo.ToString());
        }
        [Fact]
        /// <include file="test-token-nft-info.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Nfts.TokenNftInfoTest.ShouldSerializeNullSpender"]' />
        public virtual void ShouldSerializeNullSpender()
        {
            var originalTokenInfo = SpawnTokenNftInfoExample(null);
            byte[] tokenInfoBytes = originalTokenInfo.ToBytes();
            var copyTokenInfo = TokenNftInfo.FromBytes(tokenInfoBytes);

            Assert.Equal(copyTokenInfo.ToString(), originalTokenInfo.ToString());
            Verifier.Verify(originalTokenInfo.ToString());
        }
    }
}
