// SPDX-License-Identifier: Apache-2.0
using Google.Protobuf;

using Hiero.SDK.Contract;

using Org.BouncyCastle.Utilities.Encoders;

using VerifyXunit;

namespace Hiero.Tests.SDK.Contract
{
    public class ContractLogInfoTest
    {
        private static readonly Proto.Services.ContractLoginfo info = new Proto.Services.ContractLoginfo
        {
			ContractId = new ContractId(0, 0, 10).ToProtobuf(),
			Bloom = ByteString.CopyFromUtf8("bloom"),
			//Topic = [ByteString.CopyFromUtf8("bloom")],
			Data = ByteString.CopyFromUtf8("data"),
		};

        [Fact]
        public virtual void FromProtobuf()
        {
            Verifier.Verify(ContractLogInfo.FromProtobuf(info).ToString());
        }

        [Fact]
        public virtual void ToProtobuf()
        {
            Verifier.Verify(ContractLogInfo.FromProtobuf(info).ToProtobuf().ToString());
        }

        [Fact]
        public virtual void FromBytes()
        {
            Verifier.Verify(ContractLogInfo.FromBytes(info.ToByteArray()).ToString());
        }

        [Fact]
        public virtual void ToBytes()
        {
            Verifier.Verify(Hex.ToHexString(ContractLogInfo.FromProtobuf(info).ToBytes()));
        }
    }
}