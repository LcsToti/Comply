import type { Bid } from "@/types/auction";
import { BidStatus } from "@/types/auction";


export const BidChecker = {
    hasStatus: (bid: Bid, status: BidStatus): boolean => {
        return bid.status === status;
    },
    isOutbidded: (bid: Bid): boolean => {
        return BidChecker.hasStatus(bid, BidStatus.Outbid);
    },
    isWinning: (bid: Bid): boolean => {
        return BidChecker.hasStatus(bid, BidStatus.Winning);
    },
    isWinner: (bid: Bid): boolean => {
        return BidChecker.hasStatus(bid, BidStatus.Winner);
    }
}