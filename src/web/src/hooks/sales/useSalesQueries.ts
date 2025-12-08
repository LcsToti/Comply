import { salesApi, type GetMySalesParams } from '@/api/sales/sales';
import type { Sale } from '@/types/sale';
import { useQuery } from '@tanstack/react-query';

export const saleKeys = {
    all: ['sales'] as const,
    lists: () => [...saleKeys.all, 'list'] as const,
    allSales: () => [...saleKeys.all, 'admin'] as const,
    mySales: (params?: GetMySalesParams) => [...saleKeys.all, 'user', params] as const,
    details: () => [...saleKeys.all, 'detail'] as const,
    detail: (id: string) => [...saleKeys.details(), id] as const,
};

export const useAllSalesQuery = () => {
    return useQuery<Sale[]>({
        queryKey: saleKeys.allSales(),
        queryFn: () => salesApi.getAll(),
        staleTime: 2 * 60 * 1000,
        refetchOnMount: 'always',
    });
};

export const useMySalesQuery = (
    params: GetMySalesParams = { Page: 1, PageSize: 100 },
    enabled: boolean = true
) => {
    return useQuery<Sale[]>({
        queryKey: saleKeys.mySales(params),
        queryFn: () => salesApi.getMySales(params),
        enabled,
        staleTime: 5 * 60 * 1000,
    });
};

export const useSaleQuery = (saleId: string, enabled: boolean = true) => {
    return useQuery<Sale>({
        queryKey: saleKeys.detail(saleId),
        queryFn: () => salesApi.getById(saleId),
        enabled: !!saleId && enabled,
        staleTime: 2 * 60 * 1000,
    });
};
