// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Cryptography;

using System;

namespace Hiero.SDK
{
    public sealed partial class Client 
    {
		public class Operator(AccountId accountId, PublicKey publicKey, Func<byte[], byte[]> transactionSigner)
		{
			public AccountId AccountId { get; internal set; } = accountId;
			public PublicKey PublicKey { get; internal set; } = publicKey;
			public Func<byte[], byte[]> TransactionSigner { get; internal set; } = transactionSigner;
		}
	}
}