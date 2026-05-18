// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Cryptography;

namespace Hiero.Tests.Integration.Transactions
{
    /// <include file="TransactionResponseTest.cs.xml" path='docs/member[@name="T:Hiero.Tests.Integration.TransactionResponseTest"]' />
    public class TransactionResponseTest
    {
        [Fact]
        /// <include file="TransactionResponseTest.cs.xml" path='docs/member[@name="M:Hiero.Tests.Integration.TransactionResponseTest.TransactionHashInTransactionRecordIsEqualToTheTransactionResponseTransactionHash"]' />
        public virtual void TransactionHashInTransactionRecordIsEqualToTheTransactionResponseTransactionHash()
        {
            using (var testEnv = new IntegrationTestEnv(1))
            {
                var key = PrivateKey.GenerateED25519();
                var transaction = new AccountCreateTransaction
                {
					Key = key,
				
                }.Execute(testEnv.Client);
                var record = transaction.GetRecord(testEnv.Client);
                
                Assert.Equal(record.TransactionHash.ToByteArray(), transaction.TransactionHash);
                
                var accountId = record.Receipt.AccountId;

                Assert.NotNull(accountId);
            }
        }
    }
}
