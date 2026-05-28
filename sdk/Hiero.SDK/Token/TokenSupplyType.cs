// SPDX-License-Identifier: Apache-2.0
using System;

namespace Hiero.SDK.Token
{
    /// <include file="TokenSupplyType.cs.xml" path='docs/member[@name="T:TokenSupplyType"]' />
    public readonly struct TokenSupplyType(Proto.Services.TokenSupplyType protoTokenSupplyType)
    {
        public Proto.Services.TokenSupplyType ProtoTokenSupplyType { get; } = protoTokenSupplyType;

        /// <include file="TokenSupplyType.cs.xml" path='docs/member[@name="T:TokenSupplyType_2"]' />
        public static readonly TokenSupplyType Infinite = new(Proto.Services.TokenSupplyType.Infinite);
        /// <include file="TokenSupplyType.cs.xml" path='docs/member[@name="T:TokenSupplyType_3"]' />
        public static readonly TokenSupplyType Finite = new(Proto.Services.TokenSupplyType.Finite);

        public static implicit operator TokenSupplyType(Proto.Services.TokenSupplyType value) => new(value);
        public static implicit operator Proto.Services.TokenSupplyType(TokenSupplyType value) => value.ProtoTokenSupplyType;

        public override string ToString()
        {
            return ProtoTokenSupplyType switch
            {
                Proto.Services.TokenSupplyType.Finite => "FINITE",
                Proto.Services.TokenSupplyType.Infinite => "INFINITE",

                _ => throw new NotImplementedException(string.Format("'{0}' is not a registered type", ProtoTokenSupplyType))
            };
        }
    }
}
