export type Listing = {
    id: string,
    sellerId: string,
    productId: string,
    status: string,
    buyPrice: number,
    isAuctionActive: boolean,
    buyerId?: string,
    auctionId?: string,
    listedAt: Date,
    updatedAt: Date
}

export const ListingStatus = {
    Available: "Available",
    Paused: "Paused",
    Sold: "Sold",
    SoldByAuction: "SoldByAuction"
}
export type ListingStatus = typeof ListingStatus[keyof typeof ListingStatus]

export const ListingStatusLabels: Record<ListingStatus, string> = {
    [ListingStatus.Available]: "Disponível",
    [ListingStatus.Paused]: "Pausado",
    [ListingStatus.Sold]: "Vendido",
    [ListingStatus.SoldByAuction]: "Vendido em leilão"
}