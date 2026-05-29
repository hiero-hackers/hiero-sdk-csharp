// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Token;

using VerifyXunit;

namespace Hiero.Tests.SDK.Token
{
    public class TokenTypeTest
    {
        private readonly TokenType tokenTypeFungible = TokenType.FungibleCommon;
        private readonly TokenType tokenTypeNonFungible = TokenType.NonFungibleUnique;

        [Fact]
        public virtual void FromProtobuf()
        {
            Verifier.Verify(tokenTypeFungible.ToString(), tokenTypeNonFungible.ToString());

        }
        [Fact]
        public virtual void ToProtobuf()
        {
            //Verifier.Verify((Proto.Services.TokenType)tokenTypeFungible, (Proto.Services.TokenType)tokenTypeNonFungible);
        }
    }
}