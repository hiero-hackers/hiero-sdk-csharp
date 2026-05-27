// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Core;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Transactions;

using System;
using System.Text.RegularExpressions;

using VerifyXunit;

namespace Hiero.Tests.SDK.Transactions
{
    public class TransactionReceiptQueryTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        [Fact] public virtual void ShouldSerialize()
        {
            var builder = new Proto.Services.Query();
            new TransactionReceiptQuery
            {
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5005"), validStart)

			}.OnMakeRequest(builder, new Proto.Services.QueryHeader());

            Verifier.Verify(Regex.Replace(builder.ToString(), "@[A-Za-z0-9]+", ""));
        }
    }
}