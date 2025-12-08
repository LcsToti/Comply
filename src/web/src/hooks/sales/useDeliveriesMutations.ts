import { deliveriesApi } from '@/api/sales/deliveries';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { deliveryKeys } from './useDeliveriesQueries';
import { toast } from 'sonner';
import { saleKeys } from './useSalesQueries';

function parseApiError(error: any): string {
    let errorMessage = 'Erro ao processar. Tente novamente.';

    if (error?.response?.data) {
        const errorData = error.response.data;

        if (typeof errorData === 'string') {
            errorMessage = errorData;
        } else if (errorData.message) {
            errorMessage = errorData.message;
        } else if (errorData.error) {
            errorMessage = errorData.error;
        } else if (errorData.title && errorData.detail) {
            errorMessage = `${errorData.title}: ${errorData.detail}`;
        }
    }

    return errorMessage;
}

export const useMarkAsShippedMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (saleId: string) => deliveriesApi.markAsShipped(saleId),
        onSuccess: (_, saleId) => {
            queryClient.invalidateQueries({ queryKey: saleKeys.all });
            queryClient.invalidateQueries({ queryKey: deliveryKeys.code(saleId) });

            toast.success('Produto marcado como enviado!', {
                description: 'O comprador foi notificado'
            });
        },
        onError: (error: any) => {
            console.error('Erro ao marcar como enviado:', error);

            const errorMessage = parseApiError(error);

            toast.error('Erro ao marcar como enviado', {
                description: errorMessage,
                duration: 5000,
            });
        },
    });
};

export const useMarkAsDeliveredMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ saleId, code }: { saleId: string; code: string }) =>
            deliveriesApi.markAsDelivered(saleId, { Code: code }),
        onSuccess: (_, { saleId }) => {
            queryClient.invalidateQueries({ queryKey: saleKeys.all });
            queryClient.invalidateQueries({ queryKey: deliveryKeys.code(saleId) });

            toast.success('Entrega confirmada com sucesso!', {
                description: 'O pagamento foi liberado para o vendedor',
            });
        },
        onError: (error: any) => {
            console.error('Erro ao marcar como entregue:', error);

            const errorMessage = parseApiError(error);

            if (errorMessage.toLowerCase().includes('código') || errorMessage.toLowerCase().includes('inválido')) {
                toast.error('Código incorreto', {
                    description: 'Verifique o código de entrega e tente novamente',
                    duration: 5000,
                });
            } else {
                toast.error('Erro ao confirmar entrega', {
                    description: errorMessage,
                    duration: 5000,
                });
            }
        },
    });
};
