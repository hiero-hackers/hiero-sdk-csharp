// SPDX-License-Identifier: Apache-2.0
using System;

using Org.BouncyCastle.Utilities.Encoders;

using Hiero.SDK.Cryptography;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK;
using Hiero.SDK.Exceptions;
using Hiero.SDK.LiveHashes;

using Google.Protobuf.WellKnownTypes;

namespace Hiero.Tests.Integration.LiveHashes
{
    /// <include file="LiveHashAddIntegrationTest.cs.xml" path='docs/member[@name="T:Hiero.Tests.Integration.LiveHashAddIntegrationTest"]' />
    public class LiveHashAddIntegrationTest
    {
        private static readonly byte[] HASH = Hex.Decode("100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002");

        [Fact]
        /// <include file="LiveHashAddIntegrationTest.cs.xml" path='docs/member[@name="M:Hiero.Tests.Integration.LiveHashAddIntegrationTest.CannotCreateLiveHashBecauseItsNotSupported"]' />
        public virtual void CannotCreateLiveHashBecauseItsNotSupported()
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
                    new LiveHashAddTransaction
                    {
						AccountId = accountId,
						//Duration = TimeSpan.FromDays(30),
						Hash = HASH,
						Keys = [key]
					
                    }.Execute(testEnv.Client).GetReceipt(testEnv.Client);
                }); 
                
                Assert.Contains(ResponseStatus.NotSupported.ToString(), exception.Message);
            }
        }
    }
}
