// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.File;

using System.Text.RegularExpressions;

using VerifyXunit;

namespace Hiero.Tests.SDK.File
{
    public class FileInfoQueryTest
    {
        [Fact] 
        public virtual void ShouldSerialize()
        {
            var builder = new Proto.Services.Query();
            new FileInfoQuery
            {
				FileId = FileId.FromString("0.0.5005")

			}.OnMakeRequest(builder, new Proto.Services.QueryHeader());

            Verifier.Verify(Regex.Replace(builder.ToString(), "@[A-Za-z0-9]+", ""));
        }
    }
}