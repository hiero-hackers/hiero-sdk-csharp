// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Nfts;

using System.Collections.Generic;
using System.Linq;

namespace Hiero.SDK.Token
{
    public class TokenTransferList
    {
        public readonly TokenId TokenId;
        public readonly uint? ExpectDecimals;
        public IList<TokenTransfer> Transfers = [];
        public IList<TokenNftTransfer> NftTransfers = [];

        public TokenTransferList(TokenId tokenId, uint? expectDecimals, TokenTransfer? transfer, TokenNftTransfer? nftTransfer)
        {
            TokenId = tokenId;
            ExpectDecimals = expectDecimals;

            if (transfer != null)
                Transfers.Add(transfer);

            if (nftTransfer != null)
                NftTransfers.Add(nftTransfer);
        }

        public virtual Proto.Services.TokenTransferList ToProtobuf()
        {
			Proto.Services.TokenTransferList proto = new()
            {
                Token = TokenId.ToProtobuf(),
                Transfers = { Transfers.Select(_ => _.ToProtobuf()) },
                NftTransfers = { NftTransfers.Select(_ => _.ToProtobuf()) }
			};

            if (ExpectDecimals.HasValue)
                proto.ExpectedDecimals = ExpectDecimals;

            return proto;
        }
    }
}
