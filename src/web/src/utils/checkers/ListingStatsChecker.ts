import { ListingStatus } from "@/types/listing";
import type { Product } from "@/types/product";


export const ListingChecker = {
    hasStatus: (product: Product, status: ListingStatus): boolean => {
        return product.listing.status === status;
    },
    isAvailable: (product: Product): boolean => {
        return ListingChecker.hasStatus(product, ListingStatus.Available);
    },
    isPaused: (product: Product): boolean => {
        return ListingChecker.hasStatus(product, ListingStatus.Paused);
    },
    isSoldByBuyNow: (product: Product): boolean => {
        return ListingChecker.hasStatus(product, ListingStatus.Sold);
    },
    isSoldByAuction: (product: Product): boolean => {
        return ListingChecker.hasStatus(product, ListingStatus.SoldByAuction);
    },
    isSold: (product: Product): boolean => {
        return ListingChecker.isSoldByAuction(product) || ListingChecker.isSoldByBuyNow(product);
    },
};