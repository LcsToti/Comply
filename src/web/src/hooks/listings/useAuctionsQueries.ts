import { useQuery } from '@tanstack/react-query';
import type { Auction, AuctionPagedList, Bid } from '@/types/auction';
import { auctionsApi, type GetAuctionsParams } from '@/api/listings/auctions';

export const auctionKeys = {
    all: ['auctions'] as const,
    lists: () => [...auctionKeys.all, 'list'] as const,
    list: (params?: GetAuctionsParams) => [...auctionKeys.lists(), params] as const,
    details: () => [...auctionKeys.all, 'detail'] as const,
    detail: (id: string) => [...auctionKeys.details(), id] as const,
    bids: (id: string) => [...auctionKeys.all, id, 'bids'] as const,
    count: () => [...auctionKeys.all, 'count'] as const,
};

export const useAuctionsQuery = (params?: GetAuctionsParams) => {
    return useQuery<AuctionPagedList<Auction>>({
        queryKey: auctionKeys.list(params),
        queryFn: () => auctionsApi.getAll(params),
        staleTime: 30 * 1000,
    });
};

export const useAuctionQuery = (auctionId: string) => {
    return useQuery<Auction>({
        queryKey: auctionKeys.detail(auctionId),
        queryFn: () => auctionsApi.getById(auctionId),
        enabled: !!auctionId,
        staleTime: 30 * 1000,
    });
};

export const useAuctionBidsQuery = (auctionId: string) => {
    return useQuery<Bid[]>({
        queryKey: auctionKeys.bids(auctionId),
        queryFn: () => auctionsApi.getBids(auctionId),
        enabled: !!auctionId,
        staleTime: 5 * 1000,
        refetchInterval: 10000,
    });
};

export const useActiveAuctionsCountQuery = () => {
    return useQuery<number>({
        queryKey: auctionKeys.count(),
        queryFn: () => auctionsApi.getActiveCount(),
        staleTime: 60 * 1000,
    });
};
