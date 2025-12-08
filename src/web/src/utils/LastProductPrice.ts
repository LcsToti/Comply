import { BidStatus } from "@/types/auction";
import { ListingStatus } from "@/types/listing";
import type { Product } from "@/types/product";

export const getFinalSalePrice = (product: Product): number | null => {
    const listing = product.listing;

    // Produto não foi vendido
    if (
        listing?.status !== ListingStatus.Sold &&
        listing?.status !== ListingStatus.SoldByAuction
    ) {
        return null;
    }

    // Venda por leilão
    if (listing.status === ListingStatus.SoldByAuction) {
        const auction = listing.auction;

        if (!auction?.bids || auction.bids.length === 0) {
            return null;
        }

        // Procura o lance vencedor final
        const winnerBid = auction.bids.find(
            (bid) => bid?.status === BidStatus.Winner
        );

        return winnerBid?.value ?? null;
    }

    // Venda direta (compra imediata)
    if (listing.status === ListingStatus.Sold) {
        return listing.buyPrice ?? null;
    }

    return null;
};