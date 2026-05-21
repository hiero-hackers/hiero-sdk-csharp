// SPDX-License-Identifier: Apache-2.0
using System;

using Google.Protobuf;

using Hiero.SDK.Cryptography;
using Hiero.SDK.Contract;
using Hiero.SDK.Transactions;
using Hiero.SDK.Cryptocurrency;

using VerifyXunit;
using Hiero.SDK;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.Contract
{
    /// <include file="test-contract-execute-transaction.ts.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Contract.ContractExecuteTransactionTest"]' />
    public class ContractExecuteTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        public virtual void ShouldSerialize()
        {
            Verifier.Verify(SpawnTestTransaction().ToString());
        }
        [Fact]
        /// <include file="test-contract-execute-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractExecuteTransactionTest.ShouldBytesNoSetters"]' />
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new ContractExecuteTransaction();
            var tx2 = Transaction.FromBytes<ContractExecuteTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }

        private ContractExecuteTransaction SpawnTestTransaction()
        {
            return new ContractExecuteTransaction
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				ContractId = ContractId.FromString("0.0.5007"),
				Gas = 10,
				PayableAmount = Hbar.FromTinybars(1000),
				FunctionParameters = ByteString.CopyFrom(new byte[] { 24, 43, 11 }),
				MaxTransactionFee = Hbar.FromTinybars(100000),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-contract-execute-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractExecuteTransactionTest.ShouldBytes"]' />
        public virtual void ShouldBytes()
        {
            var tx = SpawnTestTransaction();
            var tx2 = Transaction.FromBytes<ContractExecuteTransaction>(tx.ToBytes());

            Assert.Equal(tx2.ToString(), tx.ToString());
        }
        [Fact]
        /// <include file="test-contract-execute-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractExecuteTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
                ContractCall = new Proto.Services.ContractCallTransactionBody { }
            };
            var tx = Transaction.FromScheduledTransaction(transactionBody);
            Assert.IsType<ContractExecuteTransaction>(tx);
        }
        [Fact]
        /// <include file="test-contract-execute-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractExecuteTransactionTest.SetGasShouldRejectNegativeValues"]' />
        public virtual void SetGasShouldRejectNegativeValues()
        {
            var tx = new ContractExecuteTransaction();
            var ex = Assert.Throws<ArgumentException>(() => tx.Gas = -1);
            
            Assert.Equal(ex.Message, "Gas must be non-negative");
        }
        [Fact]
        /// <include file="test-contract-execute-transaction.ts.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Contract.ContractExecuteTransactionTest.SetGasShouldAcceptZeroAndPositiveValues"]' />
        public virtual void SetGasShouldAcceptZeroAndPositiveValues()
        {
            var tx = new ContractExecuteTransaction();
            tx.Gas = 0;
            Assert.Equal(tx.Gas, 0);
            tx.Gas = 123456;
            Assert.Equal(tx.Gas, 123456);
        }
    }
}
