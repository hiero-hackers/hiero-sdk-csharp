using Hiero.SDK;

using System;

namespace Hiero.Tests.SDK
{
    public class BaseTestFixture : IDisposable
    {
        protected Client MainnetClient { get; private set; }
        protected Client TestnetClient { get; private set; }
        protected Client PreviewnetClient { get; private set; }

        public BaseTestFixture()
        {
            MainnetClient = Client.ForMainnet();
            TestnetClient = Client.ForTestnet();
            PreviewnetClient = Client.ForPreviewnet();
        }

        public void Dispose()
        {
            MainnetClient?.Dispose();
            TestnetClient?.Dispose();
            PreviewnetClient?.Dispose();
        }
    }
}
