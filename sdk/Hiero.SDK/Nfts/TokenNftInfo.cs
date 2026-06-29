// SPDX-License-Identifier: Apache-2.0
using Google.Protobuf;

using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Networking;

using System;

namespace Hiero.SDK.Nfts
{
	/// <include file="TokenNftInfo.cs.xml" path='docs/member[@name="T:TokenNftInfo"]' />
	public class TokenNftInfo
    {
        /// <include file="TokenNftInfo.cs.xml" path='docs/member[@name="M:TokenNftInfo.#ctor(NftId,AccountId,NodaTime.Instant,System.Byte[],LedgerId,AccountId)"]' />
        internal TokenNftInfo(NftId nftId, AccountId accountId, NodaTime.Instant creationTime, byte[] metadata, LedgerId ledgerId, AccountId? spenderId)
        {
            NftId = nftId;
            AccountId = accountId;
            CreationTime = creationTime;
            Metadata = metadata;
            LedgerId = ledgerId;
            SpenderId = spenderId;
        }

		/// <include file="TokenNftInfo.cs.xml" path='docs/member[@name="M:TokenNftInfo.FromBytes(System.Byte[])"]' />
		public static TokenNftInfo FromBytes(byte[] bytes)
		{
			return FromProtobuf(Proto.Services.TokenNftInfo.Parser.ParseFrom(bytes));
		}
		/// <include file="TokenNftInfo.cs.xml" path='docs/member[@name="M:TokenNftInfo.FromProtobuf(Proto.Services.TokenNftInfo)"]' />
		public static TokenNftInfo FromProtobuf(Proto.Services.TokenNftInfo info)
        {
            return new TokenNftInfo(
                NftId.FromProtobuf(info.NftID), 
                AccountId.FromProtobuf(info.AccountID), 
                info.CreationTime.ToNodaTimeInstant(), 
                info.Metadata.ToByteArray(), 
                LedgerId.FromByteString(info.LedgerId),
                info.SpenderId is null ? null : AccountId.FromProtobuf(info.SpenderId));
        }

        /// <include file="TokenNftInfo.cs.xml" path='docs/member[@name="F:TokenNftInfo.NftId"]' />
        public NftId NftId { get; }
        /// <include file="TokenNftInfo.cs.xml" path='docs/member[@name="F:TokenNftInfo.AccountId"]' />
        public AccountId AccountId { get; }
        /// <include file="TokenNftInfo.cs.xml" path='docs/member[@name="F:TokenNftInfo.CreationTime"]' />
        public NodaTime.Instant CreationTime { get; }
        /// <include file="TokenNftInfo.cs.xml" path='docs/member[@name="F:TokenNftInfo.Metadata"]' />
        public byte[] Metadata { get; }
        /// <include file="TokenNftInfo.cs.xml" path='docs/member[@name="F:TokenNftInfo.LedgerId"]' />
        public LedgerId LedgerId { get; }
        /// <include file="TokenNftInfo.cs.xml" path='docs/member[@name="F:TokenNftInfo.SpenderId"]' />
        public AccountId? SpenderId { get; }

        /// <include file="TokenNftInfo.cs.xml" path='docs/member[@name="M:TokenNftInfo.ToBytes"]' />
        public virtual byte[] ToBytes()
		{
			return ToProtobuf().ToByteArray();
		}
		/// <include file="TokenNftInfo.cs.xml" path='docs/member[@name="M:TokenNftInfo.ToProtobuf"]' />
		public virtual Proto.Services.TokenNftInfo ToProtobuf()
        {
            Proto.Services.TokenNftInfo proto = new()
            {
				NftID = NftId.ToProtobuf(),
				AccountID = AccountId.ToProtobuf(),
				CreationTime = CreationTime.ToProtoTimestamp(),
				Metadata = ByteString.CopyFrom(Metadata),
				LedgerId = LedgerId.ToByteString(),
			};
                
            if (SpenderId != null)
                proto.SpenderId = SpenderId.ToProtobuf();

			return proto;
        }
    }
}
