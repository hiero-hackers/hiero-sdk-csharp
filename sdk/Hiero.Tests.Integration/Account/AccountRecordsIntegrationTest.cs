// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Transactions;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK;

namespace Hiero.Tests.Integration.Account
{
    /// <include file="AccountRecordsIntegrationTest.cs.xml" path='docs/member[@name="T:Hiero.Tests.Integration.AccountRecordsIntegrationTest"]' />
    public class AccountRecordsIntegrationTest
    {
        [Fact]
        /// <include file="AccountRecordsIntegrationTest.cs.xml" path='docs/member[@name="M:Hiero.Tests.Integration.AccountRecordsIntegrationTest.CanQueryAccountRecords"]' />
        public virtual void CanQueryAccountRecords()
        {
            using (var testEnv = new IntegrationTestEnv(1))
            {
                var key = PrivateKey.GenerateED25519();
                var response = new AccountCreateTransaction
                {
					InitialBalance = new Hbar(1),
					Key = key,

				}.Execute(testEnv.Client);
                
                var accountId = response.GetReceipt(testEnv.Client).AccountId;
                new TransferTransaction()
                    .AddHbarTransfer(testEnv.OperatorId, new Hbar(1).Negated())
                    .AddHbarTransfer(accountId, new Hbar(1))
                    .Execute(testEnv.Client)
                    .GetReceipt(testEnv.Client);
                
                new TransferTransaction()
                    .AddHbarTransfer(testEnv.OperatorId, new Hbar(1))
                    .AddHbarTransfer(accountId, new Hbar(1).Negated())
                    .FreezeWith(testEnv.Client)
                    .Sign(key)
                    .Execute(testEnv.Client)
                    .GetReceipt(testEnv.Client);

                var records = new AccountRecordsQuery
                {
					QueryPayment = new Hbar(10),
					AccountId = testEnv.OperatorId,

				}.Execute(testEnv.Client);

                Assert.NotEmpty(records);
            }
        }
    }
}
