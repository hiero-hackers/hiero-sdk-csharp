// SPDX-License-Identifier: Apache-2.0
using System;

using Hiero.SDK;
using Hiero.SDK.Systems;
using Hiero.SDK.Transactions;
using Hiero.SDK.Cryptography;
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.File;
using Hiero.SDK.Contract;

using VerifyXunit;
using Hiero.SDK.Core;

namespace Hiero.Tests.SDK.System
{
    /// <include file="test-system-undelete-transaction.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.System.SystemUndeleteTransactionTest"]' />
    public class SystemUndeleteTransactionTest
    {
        private static readonly PrivateKey unusedPrivateKey = PrivateKey.FromString("302e020100300506032b657004220420db484b828e64b2d8f12ce3c0a0e93a0b8cce7af1bb8f39c97732394482538e10");
        private readonly NodaTime.Instant validStart = NodaTime.Instant.FromUnixTimeMilliseconds(1554158542);

        [Fact]
        public virtual void ShouldSerializeFile()
        {
            Verifier.Verify(SpawnTestTransactionFile().ToString());
        }

        [Fact]
        public virtual void ShouldBytesNoSetters()
        {
            var tx = new SystemUndeleteTransaction();
            var tx2 = Transaction.FromBytes<SystemUndeleteTransaction>(tx.ToBytes());
            Assert.Equal(tx.ToString(), tx2.ToString());
        }

        private SystemUndeleteTransaction SpawnTestTransactionFile()
        {
            return new SystemUndeleteTransaction()
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				FileId = FileId.FromString("0.0.444"),
				MaxTransactionFee = new Hbar(1)
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }

        [Fact]
        public virtual void ShouldSerializeContract()
        {
            Verifier.Verify(SpawnTestTransactionContract().ToString());
        }

        private SystemUndeleteTransaction SpawnTestTransactionContract()
        {
            return new SystemUndeleteTransaction()
            {
				NodeAccountIds = new (AccountId.FromString("0.0.5005"), AccountId.FromString("0.0.5006")),
				TransactionId = TransactionId.WithValidStart(AccountId.FromString("0.0.5006"), validStart),
				ContractId = ContractId.FromString("0.0.444"),
				MaxTransactionFee = new Hbar(1),
			}
            .Freeze()
            .Sign(unusedPrivateKey);
        }
        [Fact]
        /// <include file="test-system-undelete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemUndeleteTransactionTest.ShouldBytesContract"]' />
        public virtual void ShouldBytesContract()
        {
            var tx = SpawnTestTransactionContract();
            var tx2 = Transaction.FromBytes<SystemUndeleteTransaction>(tx.ToBytes());
            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-system-undelete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemUndeleteTransactionTest.ShouldBytesFile"]' />
        public virtual void ShouldBytesFile()
        {
            var tx = SpawnTestTransactionFile();
            var tx2 = Transaction.FromBytes<SystemUndeleteTransaction>(tx.ToBytes());
            Assert.Equal(tx.ToString(), tx2.ToString());
        }
        [Fact]
        /// <include file="test-system-undelete-transaction.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.System.SystemUndeleteTransactionTest.FromScheduledTransaction"]' />
        public virtual void FromScheduledTransaction()
        {
            var transactionBody = new Proto.Services.SchedulableTransactionBody
            {
                SystemUndelete = new Proto.Services.SystemUndeleteTransactionBody()
            };
            
            var tx = Transaction.FromScheduledTransaction(transactionBody);

            Assert.IsType<SystemUndeleteTransaction>(tx);
        }
    }
}
