import { BidStatus } from "@/types/auction";
import type { Product } from "@/types/product";

interface UserBidStatus {
    isWinning: boolean;
    hasBid: boolean;
    highestBidValue: number | null;
    userId?: string;
    bidStatus?: BidStatus;
}

export const getUserBidStatus = (
    product: Product,
    userId?: string
): UserBidStatus => {
    const auction = product.listing?.auction;

    // Validação inicial
    if (!userId || !auction?.bids || auction.bids.length === 0) {
        return {
            isWinning: false,
            hasBid: false,
            highestBidValue: null
        };
    }

    // Encontra o lance vencedor atual
    const winningBid = auction.bids.find(
        (bid) => bid?.status === BidStatus.Winning
    );

    // Encontra o lance vencedor final (após leilão encerrado)
    const winnerBid = auction.bids.find(
        (bid) => bid?.status === BidStatus.Winner
    );

    // Filtra lances do usuário
    const userBids = auction.bids.filter(
        (bid) => bid?.bidderId === userId
    );

    if (userBids.length === 0) {
        return {
            isWinning: false,
            hasBid: false,
            highestBidValue: null
        };
    }

    // Encontra o maior lance do usuário
    const highestUserBid = userBids.reduce((highest, current) =>
        current.value > highest.value ? current : highest
    );

    // Determina se o usuário está vencendo
    const isWinning =
        winningBid?.bidderId === userId ||
        winnerBid?.bidderId === userId;

    // Determina o status do lance do usuário
    const userActiveBid = userBids.find(
        (bid) =>
            bid.status === BidStatus.Winning ||
            bid.status === BidStatus.Winner
    );

    return {
        isWinning,
        hasBid: true,
        highestBidValue: highestUserBid.value,
        userId,
        bidStatus: userActiveBid?.status || BidStatus.Outbid,
    };
};