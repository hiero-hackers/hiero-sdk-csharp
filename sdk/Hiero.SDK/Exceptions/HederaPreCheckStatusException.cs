// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Core;
using System;

namespace Hiero.SDK.Exceptions
{
	/// <include file="HederaPreCheckStatusException.cs.xml" path='docs/member[@name="T:HederaPreCheckStatusException"]' />
	[Obsolete("Obsolete")]
    public sealed class HederaPreCheckStatusException : PrecheckStatusException
    {
		internal HederaPreCheckStatusException(ResponseStatus status, TransactionId transactionId) : base(status, transactionId) { }
    }
}