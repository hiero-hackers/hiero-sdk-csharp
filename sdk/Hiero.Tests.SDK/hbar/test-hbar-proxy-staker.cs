// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Cryptocurrency;

using VerifyXunit;

namespace Hiero.Tests.SDK.HBar
{
    public class ProxyStakerTest
    {
        private static readonly Proto.Services.ProxyStaker proxyStaker = new Proto.Services.ProxyStaker 
        { 
            AccountID = new AccountId(0, 0, 100).ToProtobuf(),
            Amount = 10
        };

        [Fact]
        public virtual void FromProtobuf()
        {
            Verifier.Verify(ProxyStaker.FromProtobuf(proxyStaker).ToString());
        }
    }
}