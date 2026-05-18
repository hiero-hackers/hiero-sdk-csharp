// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Token;

using VerifyXunit;

namespace Hiero.Tests.SDK.Token
{
    /// <include file="test-token-type-supply.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Token.TokenSupplyTypeTest"]' />
    public class TokenSupplyTypeTest
    {
        private readonly TokenSupplyType tokenSupplyTypeInfinite = TokenSupplyType.Infinite;
        private readonly TokenSupplyType tokenSupplyTypeFinite = TokenSupplyType.Finite;

        public virtual void FromProtobuf()
        {
            Verifier.Verify(tokenSupplyTypeInfinite.ToString(), tokenSupplyTypeFinite.ToString());
		}

        public virtual void ToProtobuf()
        {
            //Verifier.Verify((Proto.TokenSupplyType)tokenSupplyTypeInfinite, (Proto.TokenSupplyType)tokenSupplyTypeFinite);
        }
        [Fact]
        /// <include file="test-token-type-supply.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Token.TokenSupplyTypeTest.TokenSupplyTestToString"]' />
        public virtual void TokenSupplyTestToString()
        {
            Assert.Equal(TokenSupplyType.Infinite.ToString(), "INFINITE");
            Assert.Equal(TokenSupplyType.Finite.ToString(), "FINITE");
        }
    }
}
