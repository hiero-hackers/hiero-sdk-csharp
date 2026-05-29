// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Contract;

using Org.BouncyCastle.Utilities.Encoders;

using System;

namespace Hiero.Tests.SDK.Contract
{
    /// <include file="test-contract-function-selector.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Contract.ContractFunctionSelectorTest"]' />
    public class ContractFunctionSelectorTest
    {
        [Fact]
        /// <include file="test-contract-function-selector.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractFunctionSelectorTest.Selector"]' />
        public virtual void Selector()
        {
            var signature = new ContractFunctionSelector("testFunction")
                .AddAddress()
                .AddAddressArray()
                .AddBool()
                .AddBytes()
                .AddBytes32()
                .AddBytes32Array()
                .AddBytesArray()
                .AddFunction()
                .AddInt8()
                .AddInt8Array()
                .AddInt32()
                .AddInt32Array()
                .AddInt64()
                .AddInt64Array()
                .AddInt256()
                .AddInt256Array()
                .AddUint8()
                .AddUint8Array()
                .AddUint32()
                .AddUint32Array()
                .AddUint64()
                .AddUint64Array()
                .AddUint256()
                .AddUint256Array()
                .AddString()
                .AddStringArray()
                .Finish();

            Assert.Equal("4438e4ce", Hex.ToHexString(signature));
        }
        [Fact]
        /// <include file="test-contract-function-selector.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractFunctionSelectorTest.SelectorError"]' />
        public virtual void SelectorError()
        {
            var signature = new ContractFunctionSelector("testFunction")
                .AddAddress();
            
            signature.Finish();
            Assert.Throws<InvalidOperationException>(() => signature.AddStringArray());
			signature.Finish();
        }
    }
}
