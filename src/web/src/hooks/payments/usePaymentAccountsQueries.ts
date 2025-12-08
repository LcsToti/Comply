import { paymentAccountsApi } from '@/api/payments/paymentAccounts';
import type { PaymentAccountStatus, PaymentMethod } from '@/types/paymentAccount';
import { useQuery } from '@tanstack/react-query';

export const paymentAccountKeys = {
    all: ['payment-accounts'] as const,
    paymentMethods: () => [...paymentAccountKeys.all, 'payment-methods'] as const,
    balance: () => [...paymentAccountKeys.all, 'balance'] as const,
    status: () => [...paymentAccountKeys.all, 'status'] as const,
    onboardingLink: () => [...paymentAccountKeys.all, 'onboarding-link'] as const,
    dashboardLink: () => [...paymentAccountKeys.all, 'dashboard-link'] as const,
};

export const usePaymentMethodsQuery = () => {
    return useQuery<PaymentMethod[]>({
        queryKey: paymentAccountKeys.paymentMethods(),
        queryFn: () => paymentAccountsApi.getPaymentMethods(),
        staleTime: 24 * 60 * 60 * 1000, // dados raramente mudam
    });
};

export const useCashBalanceQuery = () => {
    return useQuery<number>({
        queryKey: paymentAccountKeys.balance(),
        queryFn: () => paymentAccountsApi.getCashBalance(),
        staleTime: 5 * 60 * 1000,       // cache curto, pode mudar frequentemente
        refetchInterval: 5 * 60 * 1000, // atualiza automaticamente a cada 5min
        placeholderData: 0
    });
};

export const useSellerAccountStatusQuery = (enabled: boolean = true) => {
    return useQuery<PaymentAccountStatus>({
        queryKey: paymentAccountKeys.status(),
        queryFn: () => paymentAccountsApi.getSellerAccountStatus(),
        enabled,
        staleTime: 30 * 1000,           // status pode mudar, cache curto
    });
};

export const useOnboardingLinkQuery = (enabled: boolean = false) => {
    return useQuery<string>({
        queryKey: paymentAccountKeys.onboardingLink(),
        queryFn: () => paymentAccountsApi.getOnboardingLink(),
        enabled,
        staleTime: 0,                    // sempre buscar link atualizado
        refetchOnMount: 'always',        // garante que o link novo é obtido
    });
};

export const useDashboardLinkQuery = (enabled: boolean = false) => {
    return useQuery<string>({
        queryKey: paymentAccountKeys.dashboardLink(),
        queryFn: () => paymentAccountsApi.getDashboardLink(),
        enabled,
        staleTime: 0,                    // sempre buscar link atualizado
        refetchOnMount: 'always',
    });
};

