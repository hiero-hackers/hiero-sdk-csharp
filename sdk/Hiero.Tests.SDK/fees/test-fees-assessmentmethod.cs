// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Fee;

namespace Hiero.Tests.SDK.Fees
{
    /// <include file="test-fees-assessmentmethod.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Fees.FeeAssessmentMethodTest"]' />
    public class FeeAssessmentMethodTest
    {
        [Fact]
        /// <include file="test-fees-assessmentmethod.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Fees.FeeAssessmentMethodTest.FeeAssessmentMethodToString"]' />
        public virtual void FeeAssessmentMethodToString()
        {
            Assert.Equal(true.ToFeeAssessmentMethod().ToString(), FeeAssessmentMethod.Exclusive.ToString());
            Assert.Equal(false.ToFeeAssessmentMethod().ToString(), FeeAssessmentMethod.Inclusive.ToString());
        }
    }
}
