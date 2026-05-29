// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Transactions;

using System;

using VerifyXunit;
using Hiero.SDK;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Transactions
{
    public class PrngTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        private PrngTransaction SpawnTestTransaction()
        {
            return new PrngTransaction()
            {
                NodeAccountIds = new(AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
                TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
                MaxTransactionFee = Hbar.FromTinybars(100000)
            }
            .Freeze()
            .Sign(unusedPrivateKey);
        }

        private PrngTransaction SpawnTestTransaction2()
        {
            return new PrngTransaction()
            {
                Range = 100,
                NodeAccountIds = new(AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
                TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
                MaxTransactionFee = Hbar.FromTinybars(100000)
            }
            .Freeze()
            .Sign(unusedPrivateKey);
        }

        [Fact] 
        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }

        [Fact]
        public virtual void ShouldSerialize2()
        {
            Verifier.Verify(SpawnTestTransaction2().ToString());
        }
    }
}