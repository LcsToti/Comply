import { useQuery } from '@tanstack/react-query';
import type { Dispute } from '@/types/dispute';
import { disputesApi, type GetAllDisputesParams } from '@/api/sales/disputes';

export const disputeKeys = {
    all: ['disputes'] as const,
    lists: () => [...disputeKeys.all, 'list'] as const,
    list: (params?: GetAllDisputesParams) => [...disputeKeys.lists(), params] as const,
    myDisputes: (params?: GetAllDisputesParams) => [...disputeKeys.all, 'user', params] as const,
};

export const useMyDisputesQuery = (params: GetAllDisputesParams = { Page: 1, PageSize: 20 }, enabled: boolean = true) => {
    return useQuery<Dispute[]>({
        queryKey: disputeKeys.myDisputes(params),
        queryFn: () => disputesApi.getMyDisputes(params),
        enabled,
        staleTime: 2 * 60 * 1000,
    });
};
