import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { paymentKeys } from './usePaymentsQueries';
import { paymentsApi } from '@/api/payments/payments';
import { paymentAccountKeys } from './usePaymentAccountsQueries';
import { notificationKeys } from '../notifications/useNotificationsQueries';

export const useWithdrawAllPaymentsMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: () => paymentsApi.withdrawAll(),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: paymentKeys.all });
            queryClient.invalidateQueries({ queryKey: paymentAccountKeys.balance() });
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });

            toast.success('Saque realizado com sucesso!', {
                description: 'O valor será transferido para sua conta bancária',
            });
        },
        onError: (error: any) => {
            console.error('Erro ao retirar todos os pagamentos:', error);

            const message = error.response?.data?.message;

            if (message?.includes('saldo insuficiente')) {
                toast.error('Saldo insuficiente', {
                    description: 'Você não possui saldo disponível para saque',
                });
            } else if (message?.includes('conta bancária')) {
                toast.error('Conta bancária não cadastrada', {
                    description: 'Configure sua conta bancária no Stripe primeiro',
                });
            } else {
                toast.error('Erro ao processar saque', {
                    description: 'Tente novamente em alguns instantes',
                });
            }
        },
    });
};

export const useWithdrawPaymentMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (paymentId: string) => paymentsApi.withdraw(paymentId),
        onSuccess: (_, paymentId) => {
            queryClient.invalidateQueries({ queryKey: paymentKeys.detail(paymentId) });
            queryClient.invalidateQueries({ queryKey: paymentKeys.all });
            queryClient.invalidateQueries({ queryKey: paymentAccountKeys.balance() });
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });

            toast.success('Pagamento sacado com sucesso!');
        },
        onError: (error: any) => {
            console.error('Erro ao retirar o pagamento:', error);

            toast.error('Erro ao processar saque', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};
