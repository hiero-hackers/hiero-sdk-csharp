// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Cryptocurrency;

using System.Collections.Generic;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Transactions
{
    /// <include file="test-transactions-duplicate.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Transactions.DuplicateTransactionTest"]' />
    public class DuplicateTransactionTest
    {
        [Fact]
        /// <include file="test-transactions-duplicate.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Transactions.DuplicateTransactionTest.GenerateTransactionIds"]' />
        public virtual void GenerateTransactionIds()
        {
            TransactionId[] ids = new TransactionId[1000000];
            AccountId accountId = AccountId.FromString("0.0.1000");

            for (int i = 0; i < ids.Length; ++i)
				ids[i] = TransactionId.Generate(accountId);

			HashSet<TransactionId> set = new (ids.Length);

            for (int i = 0; i < ids.Length; ++i)
				Assert.True(set.Add(ids[i]), $"ids[{i}] is not unique");
		}
	}
}
