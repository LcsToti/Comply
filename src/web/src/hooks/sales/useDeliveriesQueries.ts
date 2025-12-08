import { deliveriesApi } from '@/api/sales/deliveries';
import { useQuery } from '@tanstack/react-query';

export const deliveryKeys = {
    all: ['deliveries'] as const,
    codes: () => [...deliveryKeys.all, 'code'] as const,
    code: (saleId: string) => [...deliveryKeys.codes(), saleId] as const,
};

export const useDeliveryCodeQuery = (saleId: string, enabled: boolean = true) => {
    return useQuery<string>({
        queryKey: deliveryKeys.code(saleId),
        queryFn: () => deliveriesApi.getDeliveryCode(saleId),
        enabled: !!saleId && enabled,
        staleTime: 5 * 60 * 1000,
        retry: 1,
    });
};
