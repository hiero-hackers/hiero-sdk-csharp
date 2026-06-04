// SPDX-License-Identifier: Apache-2.0
using Google.Protobuf;
using Google.Protobuf.Reflection;

using Hiero.SDK.Core;
using Hiero.SDK.Cryptocurrency;

using System;
using System.Collections.Generic;

namespace Hiero.SDK.Transactions
{
    /// <include file="BatchTransaction.cs.xml" path='docs/member[@name="T:BatchTransaction"]' />
    public sealed class BatchTransaction : Transaction<BatchTransaction>
    {
        /// <include file="BatchTransaction.cs.xml" path='docs/member[@name="M:BatchTransaction.typeof(FreezeTransaction)"]' />
        private static readonly HashSet<Type> BLACKLISTED_TRANSACTIONS = [typeof(FreezeTransaction), typeof(BatchTransaction)];

		/// <include file="BatchTransaction.cs.xml" path='docs/member[@name="M:BatchTransaction.#ctor"]' />
		public BatchTransaction() { }
		/// <include file="BatchTransaction.cs.xml" path='docs/member[@name="M:BatchTransaction.#ctor(Proto.Services.TransactionBody)"]' />
		internal BatchTransaction(Proto.Services.TransactionBody txBody) : base(txBody)
		{
			InitFromTransactionBody();
		}
		/// <include file="BatchTransaction.cs.xml" path='docs/member[@name="M:BatchTransaction.#ctor(DictionaryLinked{TransactionId,DictionaryLinked{AccountId,Proto.Services.Transaction}})"]' />
		internal BatchTransaction(DictionaryLinked<TransactionId, DictionaryLinked<AccountId, Proto.Services.Transaction>> txs) : base(txs)
        {
            InitFromTransactionBody();
        }

		/// <include file="BatchTransaction.cs.xml" path='docs/member[@name="M:BatchTransaction.InitFromTransactionBody"]' />
		public ListGuarded<Transaction> InnerTransactions
        {
            init => field = GenerateListGuarded_Transaction(value);
            internal get => field ??= GenerateListGuarded_Transaction(field);
        }

        private ListGuarded<Transaction> GenerateListGuarded_Transaction(ListGuarded<Transaction>? list = null, Action<ListGuarded<Transaction>>? init = null)
        {
            list = GenerateListGuarded(list, init);
			list.OnValidateItem = ValidateInnerTransaction;
			list.OnRequireNotFrozen = RequireNotFrozen;

            return list;
        }

		/// <include file="BatchTransaction.cs.xml" path='docs/member[@name="M:BatchTransaction.InitFromTransactionBody_2"]' />
		private void InitFromTransactionBody()
		{
            InnerTransactions.OnValidateItem = null;

            foreach (var atomicTransactionBytes in SourceTransactionBody.AtomicBatch.Transactions)
			{
				Transaction transaction = Transaction.FromBytes(new Proto.Services.Transaction
				{
					SignedTransactionBytes = atomicTransactionBytes

				}.ToByteArray());

                InnerTransactions.Add(transaction);
			}

            InnerTransactions.OnValidateItem = ValidateInnerTransaction;
        }
		/// <include file="BatchTransaction.cs.xml" path='docs/member[@name="M:BatchTransaction.ValidateInnerTransaction(Transaction)"]' />
		private void ValidateInnerTransaction(Transaction transaction) 
		{
			if (BLACKLISTED_TRANSACTIONS.Contains(transaction.GetType()))
				throw new InvalidOperationException("Transaction type " + transaction.GetType().Name + " is not allowed in a batch transaction");

			if (!transaction.IsFrozen())
				throw new InvalidOperationException("Inner transaction should be frozen");

			if (transaction.BatchKey == null)
				throw new InvalidOperationException("Batch key needs to be set");
		}

		/// <include file="BatchTransaction.cs.xml" path='docs/member[@name="M:BatchTransaction.ToProtobuf"]' />
		public Proto.Services.AtomicBatchTransactionBody ToProtobuf()
        {
            var builder = new Proto.Services.AtomicBatchTransactionBody();

            foreach (var transaction in InnerTransactions)
            {
                builder.Transactions.Add(transaction.MakeRequest().SignedTransactionBytes);
            }

            return builder;
        }

		public override void ValidateChecksums(Client client)
		{
			foreach (Transaction transaction in InnerTransactions)
				transaction.ValidateChecksums(client);
		}
		public override void OnFreeze(Proto.Services.TransactionBody bodyBuilder)
        {
            bodyBuilder.AtomicBatch = ToProtobuf();
        }
        public override void OnScheduled(Proto.Services.SchedulableTransactionBody scheduled)
        {
            throw new NotSupportedException("Cannot schedule Atomic Batch");
        }
		public override MethodDescriptor GetMethodDescriptor()
		{
			string methodname = nameof(Proto.Services.UtilService.UtilServiceClient.atomicBatch);

			return Proto.Services.UtilService.Descriptor.FindMethodByName(methodname);
		}	
    }
}
