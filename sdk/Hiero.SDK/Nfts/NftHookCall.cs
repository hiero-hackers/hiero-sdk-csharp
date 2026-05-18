// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Hook;

namespace Hiero.SDK.Nfts
{
    /// <include file="NftHookCall.cs.xml" path='docs/member[@name="T:NftHookCall"]' />
    public class NftHookCall(long hookId, EvmHookCall evmHookCall, NftHookType type) : HookCall(hookId, evmHookCall)
    {
        public virtual NftHookType Type { get; } = type;
    }
}