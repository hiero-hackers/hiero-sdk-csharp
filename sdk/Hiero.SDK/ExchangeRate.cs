// SPDX-License-Identifier: Apache-2.0
using System;

namespace Hiero.SDK
{
    /// <include file="ExchangeRate.cs.xml" path='docs/member[@name="T:ExchangeRate"]' />
    public sealed class ExchangeRate(int hbars, int cents, NodaTime.Instant expirationTime)
    {
        /// <include file="ExchangeRate.cs.xml" path='docs/member[@name="M:ExchangeRate.FromProtobuf(Proto.Services.ExchangeRate)"]' />
        public static ExchangeRate FromProtobuf(Proto.Services.ExchangeRate pb)
        {
            return new ExchangeRate(pb.HbarEquiv, pb.CentEquiv, NodaTime.Instant.FromUnixTimeSeconds(pb.ExpirationTime.Seconds));
        }

        /// <include file="ExchangeRate.cs.xml" path='docs/member[@name="P:ExchangeRate.Hbars"]' />
        public int Hbars { get; } = hbars;
        /// <include file="ExchangeRate.cs.xml" path='docs/member[@name="P:ExchangeRate.Cents"]' />
        public int Cents { get; } = cents;
        /// <include file="ExchangeRate.cs.xml" path='docs/member[@name="P:ExchangeRate.ExpirationTime"]' />
        public NodaTime.Instant ExpirationTime { get; } = expirationTime;
        /// <include file="ExchangeRate.cs.xml" path='docs/member[@name="P:ExchangeRate.ExchangeRateInCents"]' />
        public double ExchangeRateInCents { get; } = cents / hbars;
    }
}
