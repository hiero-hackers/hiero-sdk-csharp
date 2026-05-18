// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Transactions;
using Hiero.SDK;
using Hiero.SDK.Core;

namespace Hiero.Tests.Integration.Account
{
    /// <include file="AccountAllowanceIntegrationTest.cs.xml" path='docs/member[@name="T:Hiero.Tests.Integration.AccountAllowanceIntegrationTest"]' />
    public class AccountAllowanceIntegrationTest
    {
        [Fact]
        /// <include file="AccountAllowanceIntegrationTest.cs.xml" path='docs/member[@name="M:Hiero.Tests.Integration.AccountAllowanceIntegrationTest.CanSpendHbarAllowance"]' />
        public virtual void CanSpendHbarAllowance()
        {
            using (var testEnv = new IntegrationTestEnv(1))
            {
                var aliceKey = PrivateKey.GenerateED25519();
                var aliceId = new AccountCreateTransaction
                {
                    InitialBalance = new Hbar(10),
					Key = aliceKey,
				}
                .Execute(testEnv.Client)
                .GetReceipt(testEnv.Client).AccountId;

                var bobKey = PrivateKey.GenerateED25519();
                var bobId = new AccountCreateTransaction
                {
                    InitialBalance = new Hbar(10),
					Key = bobKey,
				}
                .Execute(testEnv.Client)
                .GetReceipt(testEnv.Client).AccountId;
                
                new AccountAllowanceApproveTransaction()
                    .ApproveHbarAllowance(bobId, aliceId, new Hbar(10))
                    .FreezeWith(testEnv.Client)
                    .Sign(bobKey)
                    .Execute(testEnv.Client)
                    .GetReceipt(testEnv.Client);

                var transferRecord = new TransferTransaction
                {
					TransactionId = TransactionId.Generate(aliceId)
				}
                    .AddHbarTransfer(testEnv.OperatorId, new Hbar(5))
                    .AddApprovedHbarTransfer(bobId, new Hbar(5).Negated())
                    .FreezeWith(testEnv.Client)
                    .Sign(aliceKey)
                    .Execute(testEnv.Client)
                    .GetRecord(testEnv.Client);

                var transferFound = false;
                foreach (var transfer in transferRecord.Transfers)
                {
                    if (transfer.AccountId.Equals(testEnv.OperatorId) && transfer.AccountId.Equals(new Hbar(5)))
                    {
                        transferFound = true;
                        break;
                    }
                }

                Assert.True(transferFound);
                new AccountDeleteTransaction
                {
					AccountId = bobId,
					TransferAccountId = testEnv.OperatorId
				}
                .FreezeWith(testEnv.Client)
                .Sign(bobKey)
                .Execute(testEnv.Client)
                .GetReceipt(testEnv.Client);
            }
        }
    }
}
