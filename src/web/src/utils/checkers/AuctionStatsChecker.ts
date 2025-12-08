import { AuctionStatus } from "@/types/auction";
import type { Product } from "@/types/product";


export const AuctionChecker = {
    hasStatus: (product: Product, status: AuctionStatus): boolean => {
        return product.listing.auction?.status === status;
    },
    isActive: (product: Product): boolean => {
        return AuctionChecker.hasStatus(product, AuctionStatus.Active);
    },
    isAwaiting: (product: Product): boolean => {
        return AuctionChecker.hasStatus(product, AuctionStatus.Awaiting);
    },
    isEnding: (product: Product): boolean => {
        return AuctionChecker.hasStatus(product, AuctionStatus.Ending);
    },
    isSuccess: (product: Product): boolean => {
        return AuctionChecker.hasStatus(product, AuctionStatus.Success);
    },
    isFailed: (product: Product): boolean => {
        return AuctionChecker.hasStatus(product, AuctionStatus.Failed);
    },
    isCancelled: (product: Product): boolean => {
        return AuctionChecker.hasStatus(product, AuctionStatus.Cancelled);
    },
    isFinished: (product: Product): boolean => {
        return (
            AuctionChecker.isSuccess(product) ||
            AuctionChecker.isFailed(product) ||
            AuctionChecker.isCancelled(product)
        );
    },
    isInProgress: (product: Product): boolean => {
        return (
            AuctionChecker.isActive(product) || AuctionChecker.isEnding(product)
        );
    },
};