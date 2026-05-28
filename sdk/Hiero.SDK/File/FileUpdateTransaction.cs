// SPDX-License-Identifier: Apache-2.0
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Hiero.SDK.Core;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Cryptography;

using System;
using System.Collections.Generic;

namespace Hiero.SDK.File
{
    /// <include file="FileUpdateTransaction.cs.xml" path='docs/member[@name="T:FileUpdateTransaction"]' />
    public sealed class FileUpdateTransaction : Transaction<FileUpdateTransaction>
    {
		/// <include file="FileUpdateTransaction.cs.xml" path='docs/member[@name="M:FileUpdateTransaction.#ctor"]' />
		public FileUpdateTransaction() { }
		/// <include file="FileUpdateTransaction.cs.xml" path='docs/member[@name="M:FileUpdateTransaction.#ctor(Proto.Services.TransactionBody)"]' />
		internal FileUpdateTransaction(Proto.Services.TransactionBody txBody) : base(txBody)
		{
			InitFromTransactionBody();
		}
		/// <include file="FileUpdateTransaction.cs.xml" path='docs/member[@name="M:FileUpdateTransaction.#ctor(DictionaryLinked{TransactionId,DictionaryLinked{AccountId,Proto.Services.Transaction}})"]' />
		internal FileUpdateTransaction(DictionaryLinked<TransactionId, DictionaryLinked<AccountId, Proto.Services.Transaction>> txs) : base(txs)
        {
            InitFromTransactionBody();
        }

		/// <include file="FileUpdateTransaction.cs.xml" path='docs/member[@name="M:FileUpdateTransaction.RequireNotFrozen"]' />
		public FileId? FileId
		{
			get => field;
			set
			{
				RequireNotFrozen();
				field = value;
			}
		}
		/// <include file="FileUpdateTransaction.cs.xml" path='docs/member[@name="M:FileUpdateTransaction.RequireNotFrozen_2"]' />
		public string? FileMemo
		{
			get => field;
			set
			{
				RequireNotFrozen();
				field = value;
			}
		}
		/// <include file="FileUpdateTransaction.cs.xml" path='docs/member[@name="M:FileUpdateTransaction.RequireNotFrozen_3"]' />
		public KeyList? Keys
		{
			private get;
			set
			{
				RequireNotFrozen();
				field = value;
			}
		}
		public IReadOnlyList<Key>? Keys_Read { get => Keys?.AsReadOnly(); }

		/// <include file="FileUpdateTransaction.cs.xml" path='docs/member[@name="M:FileUpdateTransaction.RequireNotFrozen_4"]' />
		public ByteString? Contents
		{
			get => field;
			set
			{
				RequireNotFrozen();
				field = ByteString.CopyFrom(value?.ToByteArray());
			}
		}
		/// <include file="FileUpdateTransaction.cs.xml" path='docs/member[@name="M:FileUpdateTransaction.RequireNotFrozen_7"]' />
		public NodaTime.Instant? ExpirationTime
		{
			get => field;
			set
			{
				RequireNotFrozen();
				field = value;
                if (field is not null && ExpirationTimeDuration is not null)
                    ExpirationTimeDuration = null;
            }
		}
		public NodaTime.Duration? ExpirationTimeDuration
		{
			get => field;
			set
			{
				RequireNotFrozen();
				field = value;
                if (field is not null && ExpirationTime is not null)
                    ExpirationTime = null;
            }
		}

		/// <include file="FileUpdateTransaction.cs.xml" path='docs/member[@name="M:FileUpdateTransaction.ToProtobuf"]' />
		public Proto.Services.FileUpdateTransactionBody ToProtobuf()
        {
            var builder = new Proto.Services.FileUpdateTransactionBody { };

            if (Contents != null)
				builder.Contents = Contents;

            if (FileId != null)
				builder.FileId = FileId.ToProtobuf();

			if (Keys != null)
				builder.Keys = Keys.ToProtobuf();

            if (ExpirationTime != null)
                builder.ExpirationTime = ExpirationTime.Value.ToProtoTimestamp();
            else if (ExpirationTimeDuration != null)
                builder.ExpirationTime = ExpirationTimeDuration.Value.ToProtoTimestamp();

            if (FileMemo != null)
				builder.Memo = FileMemo;

			return builder;
        }

        public override void ValidateChecksums(Client client)
        {
			FileId?.ValidateChecksum(client);
		}
		public override void OnFreeze(Proto.Services.TransactionBody bodyBuilder)
        {
            bodyBuilder.FileUpdate = ToProtobuf();
        }
        public override void OnScheduled(Proto.Services.SchedulableTransactionBody scheduled)
        {
            scheduled.FileUpdate = ToProtobuf();
        }
		public override MethodDescriptor GetMethodDescriptor()
		{
			string methodname = nameof(Proto.Services.FileService.FileServiceClient.updateFile);

			return Proto.Services.FileService.Descriptor.FindMethodByName(methodname);
		}

		public override void OnExecute(Client client)
        {
            throw new NotImplementedException();
        }

		/// <include file="FileUpdateTransaction.cs.xml" path='docs/member[@name="M:FileUpdateTransaction.InitFromTransactionBody"]' />
		private void InitFromTransactionBody()
		{
			var body = SourceTransactionBody.FileUpdate;

			if (body.FileId is not null)
				FileId = FileId.FromProtobuf(body.FileId);

			if (body.Keys is not null)
				Keys = KeyList.FromProtobuf(body.Keys, null);

			if (body.ExpirationTime is not null)
				ExpirationTime = body.ExpirationTime.ToNodaTimeInstant();

			if (body.Memo is not null)
				FileMemo = body.Memo;

			Contents = body.Contents;
		}
	}
}
