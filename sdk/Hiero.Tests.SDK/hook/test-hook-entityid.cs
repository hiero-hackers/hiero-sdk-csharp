// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Contract;
using Hiero.SDK.Hook;

namespace Hiero.Tests.SDK.Hook
{
    /// <include file="test-hook-entityid.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Hook.HookEntityIdTest"]' />
    public class HookEntityIdTest
    {
        [Fact]
        /// <include file="test-hook-entityid.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Hook.HookEntityIdTest.AccountVariantToFromProto"]' />
        public virtual void AccountVariantToFromProto()
        {
            var acct = new AccountId(0, 0, 1234);
            var id = new HookEntityId(acct);
            Assert.True(id.IsAccount);
            Assert.False(id.IsContract);
            Assert.Equal(id.AccountId, acct);
            Assert.Null(id.ContractId);
            var proto = id.ToProtobuf();
            var parsed = HookEntityId.FromProtobuf(proto);
            Assert.Equal(parsed, id);
            Assert.Equal(parsed.GetHashCode(), id.GetHashCode());
        }
        [Fact]
        /// <include file="test-hook-entityid.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Hook.HookEntityIdTest.ContractVariantToFromProto"]' />
        public virtual void ContractVariantToFromProto()
        {
            var contract = new ContractId(0, 0, 5678);
            var id = new HookEntityId(contract);
            Assert.False(id.IsAccount);
            Assert.True(id.IsContract);
            Assert.Null(id.AccountId);
            Assert.Equal(id.ContractId, contract);
            var proto = id.ToProtobuf();
            var parsed = HookEntityId.FromProtobuf(proto);
            Assert.Equal(parsed, id);
        }
    }
}
