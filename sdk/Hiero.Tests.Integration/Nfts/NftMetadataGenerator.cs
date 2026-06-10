// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hiero.Tests.Integration.Nfts
{
    public class NftMetadataGenerator
    {
        private NftMetadataGenerator() { }

        public static List<byte[]> Generate(byte metadataCount)
        {
            List<byte[]> metadatas = new ();
            for (byte i = 0; i < metadataCount; i++)
            {
                byte[] md = [i];
                metadatas.Add(md);
            }

            return metadatas;
        }

        public static List<byte[]> Generate(byte[] metadata, int count)
        {
            return [.. Enumerable.Repeat(metadata.CopyArray(), count)];
        }

        public static List<byte[]> GenerateOneLarge()
        {
            return [new byte[101]];
        }
    }
}