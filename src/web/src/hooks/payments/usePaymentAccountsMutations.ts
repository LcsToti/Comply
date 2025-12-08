// hooks/payment-accounts/usePaymentAccountMutations.ts
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { paymentAccountKeys } from './usePaymentAccountsQueries';
import { paymentAccountsApi } from '@/api/payments/paymentAccounts';

export const useCreatePaymentMethodMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: () => paymentAccountsApi.createPaymentMethod(),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: paymentAccountKeys.paymentMethods() });
        },
    });
};

export const useGetOnboardingLinkMutation = () => {
    return useMutation({
        mutationFn: () => paymentAccountsApi.getOnboardingLink(),
        onSuccess: (data) => {
            window.location.href = data.url;
        },
        onError: (error: any) => {
            toast.error('Erro ao gerar link de cadastro', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};

export const useGetDashboardLinkMutation = () => {
    return useMutation({
        mutationFn: () => paymentAccountsApi.getDashboardLink(),
        onSuccess: (data) => {
            window.open(data.url, '_blank', 'noopener,noreferrer');
        },
        onError: (error: any) => {
            toast.error('Erro ao abrir dashboard', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};
