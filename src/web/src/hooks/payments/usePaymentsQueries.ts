import { useQuery } from '@tanstack/react-query';
import type { Payment } from '@/types/payment';
import { paymentsApi } from '@/api/payments/payments';

export const paymentKeys = {
    all: ['payments'] as const,
    details: () => [...paymentKeys.all, 'detail'] as const,
    detail: (id: string) => [...paymentKeys.details(), id] as const,
    lastSuccessful: (amount: number) => [...paymentKeys.all, 'successful', amount] as const,
};

export const usePaymentQuery = (paymentId: string, enabled: boolean = true) => {
    return useQuery<Payment>({
        queryKey: paymentKeys.detail(paymentId),
        queryFn: () => paymentsApi.getById(paymentId),
        enabled: !!paymentId && enabled,
        staleTime: 60 * 1000,
    });
};

export const useLastSuccessfulPaymentsQuery = (amount: number = 10) => {
    return useQuery<number>({
        queryKey: paymentKeys.lastSuccessful(amount),
        queryFn: () => paymentsApi.getLastSuccessful(amount),
        staleTime: 60 * 60 * 1000,
    });
};
