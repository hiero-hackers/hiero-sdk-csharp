// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK;
using Hiero.SDK.Schedule;

using System.Text.RegularExpressions;

using VerifyXunit;

namespace Hiero.Tests.SDK.Schedule
{
    public class ScheduleInfoQueryTest
    {
        [Fact] public virtual void ShouldSerialize()
        {
            var builder = new Proto.Services.Query();
            new ScheduleInfoQuery
            {
				ScheduleId = ScheduleId.FromString("0.0.5005"),
				MaxQueryPayment = Hbar.FromTinybars(100000)

			}.OnMakeRequest(builder, new Proto.Services.QueryHeader());

            Verifier.Verify(Regex.Replace(builder.ToString(), "@[A-Za-z0-9]+", ""));
        }
    }
}