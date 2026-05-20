// SPDX-License-Identifier: Apache-2.0
using System.Runtime.Serialization;

namespace Hiero.SDK.Token
{
    /// <include file="TokenSupplyType.cs.xml" path='docs/member[@name="T:TokenSupplyType"]' />
    public enum TokenSupplyType
    {
        /// <include file="TokenSupplyType.cs.xml" path='docs/member[@name="T:TokenSupplyType_2"]' />
        [EnumMember(Value = "INFINITE")] Infinite = Proto.Services.TokenSupplyType.Infinite,

        /// <include file="TokenSupplyType.cs.xml" path='docs/member[@name="T:TokenSupplyType_3"]' />
        [EnumMember(Value = "FINITE")] Finite = Proto.Services.TokenSupplyType.Finite, 
    }

    public static class TokenSupplyTypeExtensions
    {
        public static string ToString(this TokenSupplyType tokensupplytype)
        {
            return tokensupplytype switch
            {
                TokenSupplyType.Finite => "FINITE",
                TokenSupplyType.Infinite => "INFINITE",

                _ => tokensupplytype.ToString(),
            };
        }
    }
}
