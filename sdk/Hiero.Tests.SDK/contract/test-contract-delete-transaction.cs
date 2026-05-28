// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK.Cryptography;
using Hiero.SDK.Contract;
using Hiero.SDK.Transactions;
using Hiero.SDK.Cryptocurrency;

using VerifyXunit;
using Hiero.SDK;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Contract
{
    /// <include file="test-contract-delete-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Contract.ContractDeleteTransactionTest"]' />
    public class ContractDeleteTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        [Fact] public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }

        private ContractDeleteTransaction SpawnTestTransaction()
        {
            return new ContractDeleteTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				ContractId = ContractId.FromString("0.0.5007"),
				TransferAccountId = new AccountId(0, 0, 9),
				TransferContractId = ContractId.FromString("0.0.5008"),
				MaxTransactionFee = Hbar.FromTinybars(100000),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-contract-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractDeleteTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<ContractDeleteTransaction>(tx.ToBytes());
            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-contract-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractDeleteTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new ContractDeleteTransaction();
            var tx2 = Transaction.FromBytes<ContractDeleteTransaction>(tx.ToBytes());
            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-contract-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractDeleteTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
                ContractDeleteInstance = new Proto.Services.ContractDeleteTransactionBody { }
            };
            
            var tx = Transaction.FromScheduledTransaction(transactionBody);
            Assert.IsType<ContractDeleteTransaction>(tx);
        }
        [Fact]
        /// <include file="test-contract-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractDeleteTransactionTest.SetsPermanentRemovalInProtobufBody"]' />
        public virtual void SetsPermanentRemovalInProtobufBody()
        {
            var tx = new ContractDeleteTransaction
            {
				ContractId = ContractId.FromString("0.0.5007"),
				PermanentRemoval = true
			};
            var proto = tx.ToProtobuf();

            Assert.True(proto.PermanentRemoval);
        }
        [Fact]
        /// <include file="test-contract-delete-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractDeleteTransactionTest.ShouldSupportPermanentRemovalBytesRoundTrip"]' />
        public virtual void ShouldSupportPermanentRemovalBytesRoundTrip()
        {
            var tx = new ContractDeleteTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				ContractId = ContractId.FromString("0.0.5007"),
				TransferAccountId = new AccountId(0, 0, 9),
				PermanentRemoval = true,
				MaxTransactionFee = Hbar.FromTinybars(100000),

			}.Freeze();

            Assert.True(tx.PermanentRemoval);
            Assert.Equal(tx.ContractId, ContractId.FromString("0.0.5007"));
            Assert.Equal(tx.TransferAccountId, new AccountId(0, 0, 9));
            Assert.Null(tx.TransferContractId);
            Assert.Equal(tx.NodeAccountIds, [AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")]);
            Assert.Equal(tx.MaxTransactionFee, Hbar.FromTinybars(100000));

            var tx2 = Transaction.FromBytes<ContractDeleteTransaction>(tx.ToBytes());

            Assert.Equal(tx.ToString(), tx2.ToString());
            Assert.True(tx2.PermanentRemoval);
            Assert.Equal(tx2.ContractId, tx.ContractId);
            Assert.Equal(tx2.TransferAccountId, tx.TransferAccountId);
            Assert.Null(tx2.TransferContractId);
            Assert.Equal(tx2.NodeAccountIds, tx.NodeAccountIds);
            Assert.Equal(tx2.MaxTransactionFee, tx.MaxTransactionFee);
        }
    }
}
