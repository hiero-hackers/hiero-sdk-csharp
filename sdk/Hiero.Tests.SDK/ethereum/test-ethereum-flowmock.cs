// SPDX-License-Identifier: Apache-2.0
using Google.Protobuf;

using Hiero.SDK;
using Hiero.SDK.Ethereum;

using System;
using System.Collections.Generic;

namespace Hiero.Tests.SDK.Ethereum
{
    /// <include file="test-ethereum-flowmock.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.Ethereum.EthereumFlowMockTest"]' />
    public class EthereumFlowMockTest
    {
        readonly static ByteString ETHEREUM_DATA = ByteString.CopyFrom(
            Convert.FromHexString("f864012f83018000947e3a9eaf9bcc39e2ffa38eb30bf7a93feacbc18180827653820277a0f9fbff985d374be4a55f296915002eec11ac96f1ce2df183adf992baa9390b2fa00c1e867cc960d9c74ec2e6a662b7908ec4c8cc9f3091e886bcefbeb2290fb792")
        );
        readonly static ByteString LONG_CALL_DATA = ByteString.CopyFrom(
            Convert.FromHexString(new string('0', 5121 * 2))
        );
        [Fact]
        /// <include file="test-ethereum-flowmock.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.Ethereum.EthereumFlowMockTest.DontTruncateEthereumDataUnnecessarily"]' />
        public virtual void DontTruncateEthereumDataUnnecessarily()
        {
            List<object> responses1 =
            [
                (Func<object, object>)
                (_ =>
                {
                    var signedTransaction = Proto.Services.SignedTransaction.Parser.ParseFrom(((Proto.Services.Transaction)_).SignedTransactionBytes);
                    var transactionBody = Proto.Services.TransactionBody.Parser.ParseFrom(signedTransaction.BodyBytes);
                    
                    Assert.Equal(transactionBody.DataCase, Proto.Services.TransactionBody.DataOneofCase.EthereumTransaction);
                    Assert.True(transactionBody.EthereumTransaction is not null);
                    Assert.Equal(transactionBody.EthereumTransaction.EthereumData, ETHEREUM_DATA);
                
                    return new Proto.Services.TransactionResponse
                    {
                        NodeTransactionPrecheckCode = 0
                    };
                }),
                new Proto.Services.Response
                {
                    TransactionGetReceipt = new Proto.Services.TransactionGetReceiptResponse
                    {
                        Receipt = new Proto.Services.TransactionReceipt
                        {
                            Status = Proto.Services.ResponseCodeEnum.Success
                        }
                    }
                }
            ];

            using (var mocker = new Mocker([[.. responses1]]))
            {
                new EthereumFlow
                {
                    EthereumData = EthereumTransactionData.FromBytes(ETHEREUM_DATA.ToByteArray())

                }.Execute(mocker.client).GetReceipt(mocker.client);
            }
        }
        [Theory]
        [InlineData("")]
        public virtual void ExtractsCallData(string versionToTest)
        {
            if (string.IsNullOrEmpty(versionToTest))
                return;

            List<object> responses =
            [
                (Func<object, object>)
                (_ =>
                {
                    var signedTransaction = Proto.Services.SignedTransaction.Parser.ParseFrom(((Proto.Services.Transaction)_).SignedTransactionBytes);
                    var transactionBody = Proto.Services.TransactionBody.Parser.ParseFrom(signedTransaction.BodyBytes);

                    Assert.Equal(Proto.Services.TransactionBody.DataOneofCase.EthereumTransaction, transactionBody.DataCase);
                    Assert.True(transactionBody.EthereumTransaction is not null);
                    Assert.Equal(EthereumTransactionData.FromBytes(transactionBody.EthereumTransaction.EthereumData.ToByteArray()).CallData, LONG_CALL_DATA.ToByteArray());

                    return new Proto.Services.TransactionResponse
                    {
                        NodeTransactionPrecheckCode = 0
                    };
                }),
                new Proto.Services.Response
                {
                    TransactionGetReceipt = new Proto.Services.TransactionGetReceiptResponse
                    {
                        Receipt = new Proto.Services.TransactionReceipt
                        {
                            Status = Proto.Services.ResponseCodeEnum.Success
                        }
                    }
                }
            ];
            
            EthereumFlow ethereumFlow;

            using (var mocker = new Mocker([[.. responses]]))
            {
                var ethereumData = EthereumTransactionData.FromBytes(LONG_CALL_DATA.ToByteArray());

                if (versionToTest.Equals("sync"))
                {
                    ethereumFlow = new EthereumFlow
                    {
                        MaxGasAllowance = Hbar.FromTinybars(25),
                        EthereumData = EthereumTransactionData.FromBytes(ethereumData.ToBytes())
                    };

                    ethereumFlow.Execute(mocker.client).GetReceipt(mocker.client);
                }
                else
                {
                    ethereumFlow = new EthereumFlow
                    {
                        MaxGasAllowance = Hbar.FromTinybars(25), 
                        EthereumData = EthereumTransactionData.FromBytes(ethereumData.ToBytes())
                    };

                    _ = ethereumFlow
                        .ExecuteAsync(mocker.client)
                        .ContinueWith(async (response) => (await response).GetReceiptAsync(mocker.client))
                        .GetAwaiter()
                        .GetResult();
                }

                Assert.NotNull(ethereumFlow.EthereumData);
                Assert.Equal(25, ethereumFlow.MaxGasAllowance.ToTinybars());
            }
        }
    }
}
