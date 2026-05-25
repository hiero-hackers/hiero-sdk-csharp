// SPDX-License-Identifier: Apache-2.0
// Using fully qualified names to avoid conflicts with generated classes
using Google.Protobuf;
using System;
using System.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hiero.SDK.Hook
{
    public class EvmHookMappingEntry
    {
        private EvmHookMappingEntry(byte[]? key, byte[]? preimage, byte[] value)
        {
            Key = key?.CopyArray();
            PreImage = preimage?.CopyArray();
            Value = value.CopyArray();

            HasExplicitKey = Key is not null;
            HasPreimageKey = PreImage is not null;
		}

		public static EvmHookMappingEntry OfKey(byte[] key, byte[] value)
		{
			return new EvmHookMappingEntry(key ?? throw new ArgumentNullException(nameof(key)), null, value ?? throw new ArgumentNullException(nameof(value)));
		}
		public static EvmHookMappingEntry WithPreimage(byte[] preimage, byte[] value)
		{
			return new EvmHookMappingEntry(null, preimage ?? throw new ArgumentNullException(nameof(preimage)), value ?? throw new ArgumentNullException(nameof(value)));
		}
		public static EvmHookMappingEntry FromProtobuf(Proto.Services.EvmHookMappingEntry proto)
		{
			return proto.EntryKeyCase switch
			{
				Proto.Services.EvmHookMappingEntry.EntryKeyOneofCase.Key => EvmHookMappingEntry.OfKey(proto.Key.ToByteArray(), proto.Value.ToByteArray()),
				Proto.Services.EvmHookMappingEntry.EntryKeyOneofCase.Preimage => EvmHookMappingEntry.WithPreimage(proto.Preimage.ToByteArray(), proto.Value.ToByteArray()),
				Proto.Services.EvmHookMappingEntry.EntryKeyOneofCase.None or _ => throw new ArgumentException("EvmHookMappingEntry must have either key or preimage set")
			};
		}

		public virtual bool HasExplicitKey { get; }
		public virtual bool HasPreimageKey { get; }
        public virtual byte[]? Key
		{
			get => field?.CopyArray();
		}
        public virtual byte[]? PreImage
		{
            get => field?.CopyArray();
        }
        public virtual byte[] Value
        {
            get => field.CopyArray();
        }

        public virtual Proto.Services.EvmHookMappingEntry ToProtobuf()
        {
            var builder = new Proto.Services.EvmHookMappingEntry();

            if (Key != null)
            {
                builder.Key = ByteString.CopyFrom(Key);
            }
            
            if (PreImage != null)
            {
                builder.Preimage = ByteString.CopyFrom(PreImage);
            }

            if (Value.Length > 0)
            {
                builder.Value = ByteString.CopyFrom(Value);
            }

            return builder;
        }

        public override bool Equals(object? o)
        {
            if (this == o)
                return true;

            if (o is not EvmHookMappingEntry that)
                return false;

            return Key.SequenceEqual(that.Key) && PreImage.SequenceEqual(that.PreImage) && Value.SequenceEqual(that.Value);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Key?.GetHashCodeEnumerable(), PreImage?.GetHashCodeEnumerable(), Value.GetHashCodeEnumerable());
        }
    }
}
