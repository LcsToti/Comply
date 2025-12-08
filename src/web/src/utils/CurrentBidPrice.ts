import { AuctionStatus, BidStatus } from "@/types/auction";
import type { Product } from "@/types/product";

export const getCurrentBidPrice = (product: Product): number | null => {
    const auction = product.listing?.auction;

    if (!auction?.bids || auction.bids.length === 0) {
        return auction?.settings?.startBidValue ?? null;
    }

    // Leilão ativo ou finalizando - procura lance "Winning"
    if (
        auction.status === AuctionStatus.Active ||
        auction.status === AuctionStatus.Ending
    ) {
        const winningBid = auction.bids.find(
            (bid) => bid?.status === BidStatus.Winning
        );
        return winningBid?.value ?? auction.settings?.startBidValue ?? null;
    }

    // Leilão encerrado com sucesso - procura lance "Winner"
    if (auction.status === AuctionStatus.Success) {
        const winnerBid = auction.bids.find(
            (bid) => bid?.status === BidStatus.Winner
        );
        return winnerBid?.value ?? null;
    }

    // Leilão falhou ou foi cancelado - retorna último lance (se houver)
    if (
        auction.status === AuctionStatus.Failed ||
        auction.status === AuctionStatus.Cancelled
    ) {
        const lastBid = auction.bids
            .filter(bid => bid?.value)
            .sort((a, b) => b.value - a.value)[0];
        return lastBid?.value ?? null;
    }

    return null;
};