// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Networking;

namespace Hiero.Tests.Integration.Networking
{
    /// <include file="NetworkVersionInfoIntegrationTest.cs.xml" path='docs/member[@name="T:Hiero.Tests.Integration.NetworkVersionInfoIntegrationTest"]' />
    public class NetworkVersionInfoIntegrationTest
    {
        [Fact]
        /// <include file="NetworkVersionInfoIntegrationTest.cs.xml" path='docs/member[@name="M:Hiero.Tests.Integration.NetworkVersionInfoIntegrationTest.CannotQueryNetworkVersionInfo"]' />
        public virtual void CannotQueryNetworkVersionInfo()
        {
            using (var testEnv = new IntegrationTestEnv(1))
            {
                new NetworkVersionInfoQuery().Execute(testEnv.Client);
            }
        }
    }
}
