// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Core;
using System;

namespace Hiero.SDK.Exceptions
{
	/// <include file="HederaReceiptStatusException.cs.xml" path='docs/member[@name="T:HederaReceiptStatusException"]' />
	[Obsolete("Obsolete")]
    public class HederaReceiptStatusException : ReceiptStatusException
    {
		internal HederaReceiptStatusException(TransactionId transactionId, TransactionReceipt receipt) : base(transactionId, receipt) { }
    }
}