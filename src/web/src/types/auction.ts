export type Bid = {
    id: string;
    bidderId: string;
    value: number;
    status: string;
    biddedAt: string;
    outbiddedAt?: string | null;
    wonAt?: string | null;
};

export type AuctionSettings = {
    startBidValue: number;
    winBidValue?: number | null;
    startDate: string;
    endDate: string;
};

export type Auction = {
    id: string;
    listingId: string;
    status: string;
    settings: AuctionSettings;
    bids: Bid[];
    editedAt?: string;
    startedAt?: string;
    endedAt?: string;
};

export type AuctionPagedList<T> = {
    items: T[];
    page: number;
    pageSize: number;
    totalCount: number;
};

export const BidStatus = {
    Outbid: "Outbid",
    Winning: "Winning",
    Winner: "Winner"
}

export const AuctionStatus = {
    Awaiting: "Awaiting",
    Active: "Active",
    Ending: "Ending",
    Success: "Success",
    Failed: "Failed",
    Cancelled: "Cancelled"
} as const;

export type BidStatus = typeof BidStatus[keyof typeof BidStatus]
export type AuctionStatus = typeof AuctionStatus[keyof typeof AuctionStatus];

export const AuctionStatusLabels: Record<AuctionStatus, string> = {
    [AuctionStatus.Awaiting]: "Aguardando início",
    [AuctionStatus.Active]: "Ativo",
    [AuctionStatus.Ending]: "Encerrando",
    [AuctionStatus.Success]: "Sucesso",
    [AuctionStatus.Failed]: "Falhou",
    [AuctionStatus.Cancelled]: "Cancelado"
};

export const BidStatusLabels: Record<BidStatus, string> = {
    [BidStatus.Outbid]: "Superado",
    [BidStatus.Winning]: "Liderando",
    [BidStatus.Winner]: "Vencedor"
};