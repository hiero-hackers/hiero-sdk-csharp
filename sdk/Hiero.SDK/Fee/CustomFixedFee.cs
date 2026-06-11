// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Token;

using System;

namespace Hiero.SDK.Fee
{
    /// <include file="CustomFixedFee.cs.xml" path='docs/member[@name="T:CustomFixedFee"]' />
    public class CustomFixedFee : CustomFeeBase<CustomFixedFee>
    {
        /// <include file="CustomFixedFee.cs.xml" path='docs/member[@name="M:CustomFixedFee.#ctor_2"]' />
        public CustomFixedFee() { }
		/// <include file="CustomFixedFee.cs.xml" path='docs/member[@name="M:CustomFixedFee.FromProtobuf(Proto.Services.FixedFee)"]' />
		public static CustomFixedFee FromProtobuf(Proto.Services.FixedFee fixedFee)
        {
            CustomFixedFee fee = new() { Amount = fixedFee.Amount, };

            if (fixedFee.DenominatingTokenId is not null)
                fee.DenominatingTokenId = TokenId.FromProtobuf(fixedFee.DenominatingTokenId);
            
            return fee;
        }

        public long Amount { get; set; }
        public Hbar AmountHbar
        {
            get => Hbar.FromTinybars(Amount);
            set
            {
                DenominatingTokenId = null;
                Amount = value.ToTinybars();
            }
        }
        /// <include file="CustomFixedFee.cs.xml" path='docs/member[@name="M:CustomFixedFee.#ctor"]' />
        public TokenId? DenominatingTokenId { get; set; }

        /// <include file="CustomFixedFee.cs.xml" path='docs/member[@name="M:CustomFixedFee.SetDenominatingTokenToSameToken"]' />
        public virtual CustomFixedFee SetDenominatingTokenToSameToken()
        {
            DenominatingTokenId = new TokenId(0, 0, 0);
            return this;
        }
		/// <include file="CustomFixedFee.cs.xml" path='docs/member[@name="M:CustomFixedFee.ToFixedFeeProtobuf"]' />
		public virtual Proto.Services.FixedFee ToFixedFeeProtobuf()
        {
			return new Proto.Services.FixedFee
			{
				Amount = Amount,
				DenominatingTokenId = DenominatingTokenId?.ToProtobuf()
			};
        }

        public override Proto.Services.CustomFee ToProtobuf()
        {
            return FinishToProtobuf(new Proto.Services.CustomFee
            {
                FixedFee = ToFixedFeeProtobuf()
            });
        }
        /// <include file="CustomFixedFee.cs.xml" path='docs/member[@name="M:CustomFixedFee.ToFixedCustomFeeProtobuf"]' />
        public virtual Proto.Services.FixedCustomFee ToFixedCustomFeeProtobuf()
		{
			return new Proto.Services.FixedCustomFee
			{
                FixedFee = ToFixedFeeProtobuf()
			};
		}
        public virtual Proto.Services.FixedCustomFee ToTopicFeeProtobuf()
        {
            Proto.Services.FixedCustomFee proto = new()
            {
                FeeCollectorAccountId = FeeCollectorAccountId?.ToProtobuf(),
                FixedFee = new Proto.Services.FixedFee { },
            };

            if (DenominatingTokenId != null)
                proto.FixedFee.DenominatingTokenId = DenominatingTokenId.ToProtobuf();

            return proto;
        }

        public override bool Equals(object? obj)
        {
            if (this == obj)
                return true;

            if (obj is not CustomFixedFee that)
                return false;

            return
                Amount == that.Amount &&
                AmountHbar.Equals(that.AmountHbar) &&
                (DenominatingTokenId?.Equals(that.DenominatingTokenId) ?? that.DenominatingTokenId is null) &&
                base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Amount.GetHashCode(), AmountHbar.GetHashCode(), DenominatingTokenId?.GetHashCode(), base.GetHashCode());
        }
        public override void ValidateChecksums(Client client)
        {
            base.ValidateChecksums(client);

            DenominatingTokenId?.ValidateChecksum(client);
        }
        public override CustomFixedFee DeepCloneSubclass()
        {
            return new CustomFixedFee
            {
                Amount = Amount,
                DenominatingTokenId = DenominatingTokenId,

            }.FinishDeepClone(this);
        }
    }
}
