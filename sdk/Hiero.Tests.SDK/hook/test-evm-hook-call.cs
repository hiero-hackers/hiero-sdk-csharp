// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Hook;

namespace Hiero.Tests.SDK.Hook
{
    /// <include file="test-evm-hook-call.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Hook.EvmHookCallTest"]' />
    public class EvmHookCallTest
    {
        [Fact]
        /// <include file="test-evm-hook-call.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Hook.EvmHookCallTest.RoundTripProtoAndGettersAndEquality"]' />
        public virtual void RoundTripProtoAndGettersAndEquality()
        {
            byte[] data = [1,2,3];
            ulong gas = 25000;

            EvmHookCall call = new (data, gas);

            // getters
            Assert.Equal(call.GasLimit, gas);
            Assert.Equal(call.Data, [ 1, 2, 3 ]);

            // immutability of data
            var returned = call.Data;
            returned[0] = 9;

            Assert.Equal(call.Data, [ 1, 2, 3 ]);

            // proto round-trip
            var proto = call.ToProtobuf();
            EvmHookCall parsed = EvmHookCall.FromProtobuf(proto);

            Assert.Equal(parsed, call);
            Assert.Equal(parsed.GetHashCode(), call.GetHashCode());
        }
    }
}
