import { useQuery, useInfiniteQuery } from '@tanstack/react-query';
import type { Product, ProductPaginatedList } from '@/types/product';
import { productsApi, type GetFilteredProductsParams } from '@/api/products/products';

export const productKeys = {
    all: ['products'] as const,
    lists: () => [...productKeys.all, 'list'] as const,
    list: (params?: GetFilteredProductsParams) => [...productKeys.lists(), params] as const,
    infinite: (params?: GetFilteredProductsParams) => [...productKeys.all, 'infinite', params] as const,
    paginated: (params?: GetFilteredProductsParams) => [...productKeys.all, 'paginated', params] as const,
    userProducts: (params?: GetFilteredProductsParams) => [...productKeys.all, 'paginated', 'users', params] as const,
    details: () => [...productKeys.all, 'detail'] as const,
    detail: (id: string) => [...productKeys.details(), id] as const,
    myProducts: () => [...productKeys.all, 'my'] as const,
    myBidded: (page: number, size: number) => [...productKeys.myProducts(), 'bidded', page, size] as const,
    myBought: (page: number, size: number) => [...productKeys.myProducts(), 'bought', page, size] as const,
    myListed: (page: number, size: number) => [...productKeys.myProducts(), 'listed', page, size] as const,
    myOutbidded: (page: number, size: number) => [...productKeys.myProducts(), 'outbid', page, size] as const,
    myWinning: (page: number, size: number) => [...productKeys.myProducts(), 'winning', page, size] as const,
    byIds: (ids?: string[], page?: number, size?: number) => ['productsByIds', (ids ?? []).join(','), page ?? 1, size ?? 10] as const,
    counts: () => [...productKeys.all, 'counts'] as const,
    count: () => [...productKeys.counts(), 'total'] as const,
};

export const useProductsQuery = (params?: GetFilteredProductsParams) => {
    return useQuery<ProductPaginatedList<Product>>({
        queryKey: productKeys.list(params),
        queryFn: () => productsApi.getFiltered(params),
        staleTime: 10 * 1000,
        refetchInterval: 60 * 1000,
    });
};

export const useInfiniteProductsQuery = (filters?: GetFilteredProductsParams) => {
    return useInfiniteQuery({
        queryKey: productKeys.infinite(filters),
        queryFn: ({ pageParam = 1 }) => {
            return productsApi.getFiltered({ ...filters, PageNumber: pageParam });
        },
        getNextPageParam: (lastPage) => {
            if (lastPage.hasNextPage) {
                return (lastPage.pageNumber || 0) + 1;
            }
            return undefined;
        },
        initialPageParam: 1,
        staleTime: 10 * 1000,
        refetchInterval: 60 * 1000,
    });
};

export const usePaginatedProductsQuery = (filters?: GetFilteredProductsParams) => {
    return useQuery<ProductPaginatedList<Product>>({
        queryKey: productKeys.paginated(filters),
        queryFn: () => productsApi.getFiltered(filters),
        staleTime: 10 * 1000,
        refetchInterval: 60 * 1000,
    });
};

export const useUserProductsQuery = (filters?: GetFilteredProductsParams) => {
    return useQuery<ProductPaginatedList<Product>>({
        queryKey: productKeys.userProducts(filters),
        queryFn: () => productsApi.getFiltered(filters),
        enabled: !!filters?.SellerId,
        staleTime: 10 * 1000,
        refetchInterval: 60 * 1000,
    });
};

export const useProductQuery = (productId: string) => {
    return useQuery<Product>({
        queryKey: productKeys.detail(productId),
        queryFn: () => productsApi.getById(productId),
        enabled: !!productId,
        staleTime: 5 * 1000,
        refetchInterval: 30 * 1000,
    });
};

export const useProductsByIdsQuery = (
    ids?: string[],
    page: number = 1,
    pageSize: number = 10
) => {
    const queryFn = async () => {
        if (!ids || ids.length === 0) {
            return { items: [], totalPages: 0, totalCount: 0 };
        }

        const start = (page - 1) * pageSize;
        const end = start + pageSize;
        const pageIds = ids.slice(start, end);

        const products = await Promise.all(
            pageIds.map((id) => productsApi.getById(id))
        );

        const totalCount = ids.length;
        const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

        return { items: products, totalPages, totalCount };
    };

    return useQuery({
        queryKey: productKeys.byIds(ids || [], page, pageSize),
        queryFn,
        enabled: !!ids && ids.length > 0,
        staleTime: 60 * 1000,
    });
};

export const useMyBiddedProductsQuery = (pageNumber: number = 1, pageSize: number = 10) => {
    return useQuery<ProductPaginatedList<Product>>({
        queryKey: productKeys.myBidded(pageNumber, pageSize),
        queryFn: () => productsApi.getMyBidded({ pageNumber, pageSize }),
        staleTime: 30 * 1000,
    });
};

export const useMyBoughtProductsQuery = (pageNumber: number = 1, pageSize: number = 10) => {
    return useQuery<ProductPaginatedList<Product>>({
        queryKey: productKeys.myBought(pageNumber, pageSize),
        queryFn: () => productsApi.getMyBought({ pageNumber, pageSize }),
        staleTime: 30 * 1000,
    });
};

export const useMyListedProductsQuery = (pageNumber: number = 1, pageSize: number = 10) => {
    return useQuery<ProductPaginatedList<Product>>({
        queryKey: productKeys.myListed(pageNumber, pageSize),
        queryFn: () => productsApi.getMyListed({ pageNumber, pageSize }),
        staleTime: 30 * 1000,
    });
};

export const useMyOutbiddedProductsQuery = (pageNumber: number = 1, pageSize: number = 10) => {
    return useQuery<ProductPaginatedList<Product>>({
        queryKey: productKeys.myOutbidded(pageNumber, pageSize),
        queryFn: () => productsApi.getMyOutbidded({ pageNumber, pageSize }),
        staleTime: 30 * 1000,
    });
};

export const useMyWinningProductsQuery = (pageNumber: number = 1, pageSize: number = 10) => {
    return useQuery<ProductPaginatedList<Product>>({
        queryKey: productKeys.myWinning(pageNumber, pageSize),
        queryFn: () => productsApi.getMyWinning({ pageNumber, pageSize }),
        staleTime: 30 * 1000,
    });
};

export const useProductsCountQuery = () => {
    return useQuery<number>({
        queryKey: productKeys.count(),
        queryFn: () => productsApi.getCount(),
        staleTime: 60 * 60 * 1000,
    });
};

export const useActiveAuctionsCountQuery = () => {
    return useQuery<number>({
        queryKey: [...productKeys.counts(), 'active-auctions'],
        queryFn: () => productsApi.getActiveAuctionsCount(),
        staleTime: 60 * 60 * 1000,
    });
};
