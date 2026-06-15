// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Exceptions;
using Hiero.SDK.Core;

using NodaTime;

namespace Hiero.Tests.SDK.Exceptions
{
    /// <include file="test-exception-receiptstatus.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Exceptions.ReceiptStatusExceptionTest"]' />
    public class ReceiptStatusExceptionTest
    {
        [Fact]
        /// <include file="test-exception-receiptstatus.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Exceptions.ReceiptStatusExceptionTest.ShouldHaveMessage"]' />
        public virtual void ShouldHaveMessage()
        {
            var validStart = Instant.FromUnixTimeSeconds(1554158542);
            var txId = new TransactionId(new AccountId(0, 0, 100), validStart);
            var txReceipt = TransactionReceipt.FromProtobuf(new Proto.Services.TransactionReceipt
            {
                Status = Proto.Services.ResponseCodeEnum.InsufficientTxFee,
                ExchangeRate = new Proto.Services.ExchangeRateSet 
                {
                    CurrentRate = new Proto.Services.ExchangeRate
                    {
                        HbarEquiv = 1,
                        CentEquiv = 1,
                        ExpirationTime = new Proto.Services.TimestampSeconds { Seconds = 100 },
                    },
                    NextRate = new Proto.Services.ExchangeRate
                    {
                        HbarEquiv = 1,
                        CentEquiv = 1,
                        ExpirationTime = new Proto.Services.TimestampSeconds { Seconds = 100 },
                    },
                },
            });
            var e = new ReceiptStatusException(txId, txReceipt);

            Assert.Equal("receipt for transaction 0.0.100@1554158542.000000000 raised status INSUFFICIENT_TX_FEE", e.Message);
        }
    }
}
