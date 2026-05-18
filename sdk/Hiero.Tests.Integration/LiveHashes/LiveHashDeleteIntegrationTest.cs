// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Cryptography;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK;
using Hiero.SDK.Exceptions;
using Hiero.SDK.LiveHashes;

using Org.BouncyCastle.Utilities.Encoders;

namespace Hiero.Tests.Integration.LiveHashes
{
    /// <include file="LiveHashDeleteIntegrationTest.cs.xml" path='docs/member[@name="T:Hiero.Tests.Integration.LiveHashDeleteIntegrationTest"]' />
    public class LiveHashDeleteIntegrationTest
    {
        private static readonly byte[] HASH = Hex.Decode("100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002");

        [Fact]
        /// <include file="LiveHashDeleteIntegrationTest.cs.xml" path='docs/member[@name="M:Hiero.Tests.Integration.LiveHashDeleteIntegrationTest.CannotDeleteLiveHashBecauseItsNotSupported"]' />
        public virtual void CannotDeleteLiveHashBecauseItsNotSupported()
        {
            using (var testEnv = new IntegrationTestEnv(1))
            {
                var key = PrivateKey.GenerateED25519();
                var response = new AccountCreateTransaction
                {
					Key = key,
					InitialBalance = new Hbar(1),
				
                }.Execute(testEnv.Client);
                var accountId = response.GetReceipt(testEnv.Client).AccountId;

                PrecheckStatusException exception = Assert.Throws<PrecheckStatusException>(() =>
                {
                    new LiveHashDeleteTransaction
                    {
						AccountId = accountId,
                        Hash = HASH
					
                    }.Execute(testEnv.Client).GetReceipt(testEnv.Client);

                }); 
                
                Assert.Contains(ResponseStatus.NotSupported.ToString(), exception.Message);
            }
        }
    }
}
