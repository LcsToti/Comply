import { watchlistApi } from '@/api/notifications/watchLists';
import { useQuery } from '@tanstack/react-query';

export const watchlistKeys = {
    all: ['watchlist'] as const,
    list: () => [...watchlistKeys.all, 'list'] as const,
};

export const useWatchlistQuery = () => {
    return useQuery<string[]>({
        queryKey: watchlistKeys.list(),
        queryFn: () => watchlistApi.getMyWatchlist(),
        staleTime: 2 * 60 * 1000,
    });
};

export const useIsInWatchlist = (productId?: string) => {
    const { data: watchlist } = useWatchlistQuery();

    if (!productId) {
        return { isInWatchlist: false, isLoading: false };
    }

    const isInWatchlist = watchlist?.includes(productId) ?? false;


    return {
        isInWatchlist,
        isLoading: !watchlist,
    };
};

export const useWatchlistCount = () => {
    const { data: watchlist } = useWatchlistQuery();

    return {
        count: watchlist?.length || 0,
        isEmpty: watchlist?.length === 0,
    };
};
