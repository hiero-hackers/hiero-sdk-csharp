// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Cryptocurrency;

using System.Collections.Generic;
using System.Linq;

namespace Hiero.SDK.Fee
{
    /// <include file="CustomFeeLimit.cs.xml" path='docs/member[@name="T:CustomFeeLimit"]' />
    public class CustomFeeLimit
    {
        public virtual AccountId? PayerId { get; set; } 
        public virtual List<CustomFixedFee> CustomFees { get; set; } = [];

        public static CustomFeeLimit FromProtobuf(Proto.Services.CustomFeeLimit customFeeLimit)
        {
            return new CustomFeeLimit
            {
                PayerId = AccountId.FromProtobuf(customFeeLimit.AccountId),
                CustomFees = [.. customFeeLimit.Fees.Select(_ => CustomFixedFee.FromProtobuf(_))]
            };
        }

		public virtual Proto.Services.CustomFeeLimit ToProtobuf()
        {
			return new Proto.Services.CustomFeeLimit
            {
                AccountId = PayerId?.ToProtobuf(),
                Fees = { CustomFees.Select(_ => _.ToFixedFeeProtobuf()) }
			};
        }
    }
}
