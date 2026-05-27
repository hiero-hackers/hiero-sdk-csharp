// SPDX-License-Identifier: Apache-2.0
// Using fully qualified names to avoid conflicts with generated classes
using Google.Protobuf;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hiero.SDK.Hook
{
	public class EvmHookMappingEntries : EvmHookStorageUpdate
	{
		public EvmHookMappingEntries(byte[] mappingSlot, IEnumerable<EvmHookMappingEntry> entries)
		{
            ArgumentNullException.ThrowIfNull(mappingSlot, nameof(mappingSlot));
            ArgumentNullException.ThrowIfNull(entries, nameof(entries));

            MappingSlot = mappingSlot.CopyArray();
			Entries = [.. entries];
		}
		public static EvmHookMappingEntries FromProtobuf(Proto.Services.EvmHookMappingEntries proto)
		{
			return new EvmHookMappingEntries(
				proto.MappingSlot.ToByteArray(),
				proto.Entries.Select(_ => EvmHookMappingEntry.FromProtobuf(_)));
		}

		public virtual byte[] MappingSlot
		{
			get => field.CopyArray();
		}
		public virtual IList<EvmHookMappingEntry> Entries
		{
			get => [.. field];
		}

		public override Proto.Services.EvmHookStorageUpdate ToProtobuf()
		{
			var proto = new Proto.Services.EvmHookMappingEntries()
			{
				MappingSlot = ByteString.CopyFrom(MappingSlot)
			};
			
			proto.Entries.AddRange(Entries.Select(_ => _.ToProtobuf()));

			return new Proto.Services.EvmHookStorageUpdate
			{
				MappingEntries = proto,
			};
		}
		public override bool Equals(object? o)
		{
			if (this == o)
				return true;

			if (o is not EvmHookMappingEntries that)
				return false;

			return MappingSlot.SequenceEqual(that.MappingSlot) && Entries.SequenceEqual(that.Entries);
		}
		public override int GetHashCode()
		{
			return HashCode.Combine(MappingSlot.GetHashCodeEnumerable(), Entries.GetHashCodeEnumerable());
		}
        public override string ToString()
        {
			return string.Format("EvmHookMappingEntries {{ mappingSlot=\"[{0}]\", entries=\"{1}\" }}", string.Join("; ", MappingSlot), string.Join("; ", Entries));
        }
    }
}
