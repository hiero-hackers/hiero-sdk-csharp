// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using System.Linq;

using Org.BouncyCastle.Utilities.Encoders;

using Hiero.SDK.Cryptography;
using Hiero.SDK.Fee;
using Hiero.SDK.Token;
using Hiero.SDK.Consensus;
using Hiero.SDK.Networking;
using Hiero.SDK.Cryptocurrency;

using Google.Protobuf;

using VerifyXunit;

namespace Hiero.Tests.SDK.Topic
{
    public class TopicInfoTest
    {
        private static readonly PrivateKey privateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private static readonly PrivateKey feeScheduleKey = PrivateKey.FromString("302e020100300506032b657004220420aabbccddeeff00112233445566778899aabbccddeeff00112233445566778899");
        private static readonly List<PrivateKey> feeExemptKeys = 
        [
            PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10"), 
            PrivateKey.FromString("302e020100300506032b657004220420aabbccddeeff00112233445566778899aabbccddeeff00112233445566778899")
        ];
        private static readonly byte[] hash = [2];
        private static readonly List<CustomFixedFee> customFees = 
        [
            new CustomFixedFee 
            {
                Amount = 100,
                DenominatingTokenId = new TokenId(0, 0, 0)
            } 
        ];
        private static readonly Proto.Services.ConsensusGetTopicInfoResponse info = new Proto.Services.ConsensusGetTopicInfoResponse
        {
            TopicInfo = new Proto.Services.ConsensusTopicInfo
            {
                Memo = "1",
                RunningHash = ByteString.CopyFrom(hash),
                SequenceNumber = 3,
                ExpirationTime = NodaTime.Instant.FromUnixTimeMilliseconds(4).ToProtoTimestamp(),
                AutoRenewPeriod = NodaTime.Duration.FromDays(5).ToProtoDuration(),
                AdminKey = privateKey.GetPublicKey().ToProtobufKey(),
                SubmitKey = privateKey.GetPublicKey().ToProtobufKey(),
                FeeScheduleKey = feeScheduleKey.GetPublicKey().ToProtobufKey(),
                AutoRenewAccount = new AccountId(0, 0, 4).ToProtobuf(),
                LedgerId = LedgerId.TESTNET.ToByteString(),
                FeeExemptKeyList = { feeExemptKeys.Select(_ => _.GetPublicKey().ToProtobufKey()) },
                CustomFees = { customFees.Select(_ => _.ToTopicFeeProtobuf()) },
            }
        };
            
        public virtual void FromProtobuf()
        {
            Verifier.Verify(TopicInfo.FromProtobuf(info).ToString());
        }

        public virtual void ToProtobuf()
        {
            Verifier.Verify(TopicInfo.FromProtobuf(info).ToProtobuf().ToString());
        }

        public virtual void FromBytes()
        {
            Verifier.Verify(TopicInfo.FromBytes(info.ToByteArray()).ToString());
        }

        public virtual void ToBytes()
        {
            Verifier.Verify(Hex.ToHexString(TopicInfo.FromProtobuf(info).ToBytes()));
        }
    }
}