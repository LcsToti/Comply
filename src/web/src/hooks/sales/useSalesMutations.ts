import { salesApi } from '@/api/sales/sales';
import { useMutation, useQueryClient } from '@tanstack/react-query';
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

export const useCancelSaleMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (saleId: string) => salesApi.cancel(saleId),
        onSuccess: (_, saleId) => {
            queryClient.invalidateQueries({ queryKey: saleKeys.detail(saleId) });
            queryClient.invalidateQueries({ queryKey: saleKeys.lists() });
            queryClient.invalidateQueries({ queryKey: saleKeys.allSales() });

            toast.success('Venda cancelada com sucesso!', {
                description: 'O valor será estornado para o comprador'
            });
        },
        onError: (error: any) => {
            console.error('Erro ao cancelar venda:', error);

            const errorMessage = parseApiError(error);

            if (errorMessage.toLowerCase().includes('já foi entregue')) {
                toast.error('Não é possível cancelar', {
                    description: 'Vendas já entregues não podem ser canceladas',
                    duration: 5000,
                });
            } else if (errorMessage.toLowerCase().includes('prazo')) {
                toast.error('Prazo expirado', {
                    description: 'O prazo para cancelamento já passou',
                    duration: 5000,
                });
            } else {
                toast.error('Erro ao cancelar venda', {
                    description: errorMessage,
                    duration: 5000,
                });
            }
        },
    });
};
