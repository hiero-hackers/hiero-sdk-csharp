// SPDX-License-Identifier: Apache-2.0
using System.Text;
using Hiero.SDK;
using Hiero.SDK.Exceptions;
using Hiero.SDK.File;

namespace Hiero.Tests.Integration.File
{
    /// <include file="FileDeleteIntegrationTest.cs.xml" path='docs/member[@name="T:Hiero.Tests.Integration.FileDeleteIntegrationTest"]' />
    public class FileDeleteIntegrationTest
    {
        [Fact]
        /// <include file="FileDeleteIntegrationTest.cs.xml" path='docs/member[@name="M:Hiero.Tests.Integration.FileDeleteIntegrationTest.CanDeleteFile"]' />
        public virtual void CanDeleteFile()
        {
            using (var testEnv = new IntegrationTestEnv(1))
            {
                var response = new FileCreateTransaction
                {
					Keys = [testEnv.OperatorKey],
					Contents = Encoding.UTF8.GetBytes("[e2e::FileCreateTransaction]")
				
                }.Execute(testEnv.Client);

                var fileId = response.GetReceipt(testEnv.Client).FileId;
                var info = new FileInfoQuery
                {
					FileId = fileId,

				}.Execute(testEnv.Client);

                Assert.Equal(info.FileId, fileId);
                Assert.Equal(info.Size, 28);
                Assert.False(info.IsDeleted);
                Assert.NotNull(info.Keys);
                Assert.Null(info.Keys.Threshold);
                Assert.Equal(info.Keys, [testEnv.OperatorKey]);
                new FileDeleteTransaction
                {
					FileId = fileId,
				
                }.Execute(testEnv.Client).GetReceipt(testEnv.Client);
            }
        }
        [Fact]
        /// <include file="FileDeleteIntegrationTest.cs.xml" path='docs/member[@name="M:Hiero.Tests.Integration.FileDeleteIntegrationTest.CannotDeleteImmutableFile"]' />
        public virtual void CannotDeleteImmutableFile()
        {
            using (var testEnv = new IntegrationTestEnv(1))
            {
                var response = new FileCreateTransaction
                {
					Contents = Encoding.UTF8.GetBytes("[e2e::FileCreateTransaction]")
				
                }.Execute(testEnv.Client);

                var fileId = response.GetReceipt(testEnv.Client).FileId;
                var info = new FileInfoQuery
                {
					FileId = fileId,
				
                }.Execute(testEnv.Client);

                Assert.Equal(info.FileId, fileId);
                Assert.Equal(info.Size, 28);
                Assert.False(info.IsDeleted);
                Assert.Null(info.Keys);
                
                ReceiptStatusException exception = Assert.Throws<ReceiptStatusException>(() =>
                {
                    new FileDeleteTransaction
                    {
						FileId = fileId,
					
                    }.Execute(testEnv.Client).GetReceipt(testEnv.Client);

                });
                
                Assert.Contains(ResponseStatus.Unauthorized.ToString(), exception.Message);
            }
        }
    }
}
