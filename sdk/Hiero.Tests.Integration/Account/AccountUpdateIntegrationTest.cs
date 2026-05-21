// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Exceptions;
using Hiero.SDK;
using Hiero.SDK.Cryptography;

using System;

namespace Hiero.Tests.Integration.Account
{
    /// <include file="AccountUpdateIntegrationTest.cs.xml" path='docs/member[@name="T:Hiero.Tests.Integration.AccountUpdateIntegrationTest"]' />
    public class AccountUpdateIntegrationTest
    {
        [Fact]
        /// <include file="AccountUpdateIntegrationTest.cs.xml" path='docs/member[@name="M:Hiero.Tests.Integration.AccountUpdateIntegrationTest.CanUpdateAccountWithNewKey"]' />
        public virtual void CanUpdateAccountWithNewKey()
        {
            using (var testEnv = new IntegrationTestEnv(1))
            {
                var key1 = PrivateKey.GenerateED25519();
                var key2 = PrivateKey.GenerateED25519();
                var response = new AccountCreateTransaction
                {
					Key = key1

				}.Execute(testEnv.Client);

                var accountId = response.GetReceipt(testEnv.Client).AccountId;
                var info = new AccountInfoQuery
                {
					AccountId = accountId

				}.Execute(testEnv.Client);

                Assert.Equal(info.AccountId, accountId);
                Assert.False(info.IsDeleted);
                Assert.Equal(info.Key.ToString(), key1.GetPublicKey().ToString());
                Assert.Equal(info.Balance, new Hbar(0));
                Assert.Equal(info.AutoRenewPeriod, NodaTime.Duration.FromDays(90));
                Assert.Null(info.ProxyAccountId);
                Assert.Equal(info.ProxyReceived, Hbar.ZERO);
                
                new AccountUpdateTransaction
                {
					AccountId = accountId,
					Key = key2.GetPublicKey(),
				
                }.FreezeWith(testEnv.Client).Sign(key1).Sign(key2).Execute(testEnv.Client).GetReceipt(testEnv.Client);

                info = new AccountInfoQuery
                {
					AccountId = accountId

				}.Execute(testEnv.Client);

                Assert.Equal(info.AccountId, accountId);
                Assert.False(info.IsDeleted);
                Assert.Equal(info.Key.ToString(), key2.GetPublicKey().ToString());
                Assert.Equal(info.Balance, new Hbar(0));
				Assert.Equal(info.AutoRenewPeriod, NodaTime.Duration.FromDays(90));
				Assert.Null(info.ProxyAccountId);
                Assert.Equal(info.ProxyReceived, Hbar.ZERO);
            }
        }
        [Fact]
        /// <include file="AccountUpdateIntegrationTest.cs.xml" path='docs/member[@name="M:Hiero.Tests.Integration.AccountUpdateIntegrationTest.CannotUpdateAccountWhenAccountIdIsNotSet"]' />
        public virtual void CannotUpdateAccountWhenAccountIdIsNotSet()
        {
            using (var testEnv = new IntegrationTestEnv(1))
            {
				PrecheckStatusException exception = Assert.Throws<PrecheckStatusException>(() =>
                {
                    new AccountUpdateTransaction()
                        .Execute(testEnv.Client)
                        .GetReceipt(testEnv.Client);
                });

                Assert.Contains(exception.Message, ResponseStatus.AccountIdDoesNotExist.ToString());
            }
        }
    }
}
