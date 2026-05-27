// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Contract;

using System.Text.RegularExpressions;

using VerifyXunit;

namespace Hiero.Tests.SDK.Contract
{
    public class ContractByteCodeQueryTest
    {
        [Fact] public virtual void ShouldSerialize()
        {
            var builder = new Proto.Services.Query();
            new ContractByteCodeQuery()
            {
				ContractId = ContractId.FromString("0.0.5005")

			}.OnMakeRequest(builder, new Proto.Services.QueryHeader());

            Verifier.Verify(Regex.Replace(builder.ToString(), "@[A-Za-z0-9]+", ""));
        }
    }
}