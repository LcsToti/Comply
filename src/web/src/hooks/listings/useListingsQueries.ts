import { useQuery } from '@tanstack/react-query';
import type { Listing } from '@/types/listing';
import { listingsApi, type GetListingsParams } from '@/api/listings/listings';

export const listingKeys = {
    all: ['listings'] as const,
    lists: () => [...listingKeys.all, 'list'] as const,
    list: (params?: GetListingsParams) => [...listingKeys.lists(), params] as const,
    details: () => [...listingKeys.all, 'detail'] as const,
    detail: (id: string) => [...listingKeys.details(), id] as const,
};

export const useListingsQuery = (params?: GetListingsParams) => {
    return useQuery<Listing[]>({
        queryKey: listingKeys.list(params),
        queryFn: () => listingsApi.getAll(params),
        staleTime: 30 * 1000,
    });
};

export const useListingQuery = (listingId: string) => {
    return useQuery<Listing>({
        queryKey: listingKeys.detail(listingId),
        queryFn: () => listingsApi.getById(listingId),
        enabled: !!listingId,
        staleTime: 60 * 1000,
    });
};
