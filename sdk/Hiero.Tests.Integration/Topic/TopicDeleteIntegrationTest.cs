// SPDX-License-Identifier: Apache-2.0

using Hiero.SDK.Exceptions;
using Hiero.SDK.Consensus;
using Hiero.SDK;

namespace Hiero.Tests.Integration.Topic
{
    /// <include file="TopicDeleteIntegrationTest.cs.xml" path='docs/member[@name="T:Hiero.Tests.Integration.TopicDeleteIntegrationTest"]' />
    public class TopicDeleteIntegrationTest
    {
        [Fact]
        /// <include file="TopicDeleteIntegrationTest.cs.xml" path='docs/member[@name="M:Hiero.Tests.Integration.TopicDeleteIntegrationTest.CanDeleteTopic"]' />
        public virtual void CanDeleteTopic()
        {
            using (var testEnv = new IntegrationTestEnv(1))
            {
                var response = new TopicCreateTransaction
                {
                    AdminKey = testEnv.OperatorKey,
                    TopicMemo = "[e2e::TopicCreateTransaction]"
                
                }.Execute(testEnv.Client);

                var topicId = response.GetReceipt(testEnv.Client).TopicId;

                new TopicDeleteTransaction
                {
                    TopicId = topicId
                
                }.Execute(testEnv.Client).GetReceipt(testEnv.Client);
            }
        }
        [Fact]
        /// <include file="TopicDeleteIntegrationTest.cs.xml" path='docs/member[@name="M:Hiero.Tests.Integration.TopicDeleteIntegrationTest.CannotDeleteImmutableTopic"]' />
        public virtual void CannotDeleteImmutableTopic()
        {
            using (var testEnv = new IntegrationTestEnv(1))
            {
                var response = new TopicCreateTransaction().Execute(testEnv.Client);
                var topicId = response.GetReceipt(testEnv.Client).TopicId;
                ReceiptStatusException exception = Assert.Throws<ReceiptStatusException>(() =>
                {
                    new TopicDeleteTransaction
                    {
						TopicId = topicId
					
                    }.Execute(testEnv.Client).GetReceipt(testEnv.Client);

                }); Assert.Contains(ResponseStatus.Unauthorized.ToString(), exception.Message);
            }
        }
    }
}
