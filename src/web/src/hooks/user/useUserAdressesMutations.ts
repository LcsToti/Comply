import { userAddressesApi, type DeliveryAddressParams } from '@/api/user/usersAdresses';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { addressKeys } from './useUserAdressesQueries';
import { toast } from 'sonner';

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
        } else if (errorData.errors) {
            // Erros de validação do ASP.NET
            const validationErrors = Object.values(errorData.errors).flat();
            errorMessage = validationErrors.join(', ');
        }
    }

    return errorMessage;
}

export const useAddAddressMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (address: DeliveryAddressParams) => userAddressesApi.add(address),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: addressKeys.all });

            toast.success('Endereço adicionado!', {
                description: 'Você pode usá-lo em suas compras'
            });
        },
        onError: (error: any) => {
            console.error('Erro ao adicionar endereço:', error);

            const errorMessage = parseApiError(error);

            toast.error('Erro ao adicionar endereço', {
                description: errorMessage,
                duration: 5000,
            });
        },
    });
};

export const useUpdateAddressMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ addressId, address }: { addressId: string; address: DeliveryAddressParams }) =>
            userAddressesApi.update(addressId, address),
        onSuccess: (_, { addressId }) => {
            queryClient.invalidateQueries({ queryKey: addressKeys.all });
            queryClient.invalidateQueries({ queryKey: addressKeys.detail(addressId) });

            toast.success('Endereço atualizado!');
        },
        onError: (error: any) => {
            console.error('Erro ao atualizar endereço:', error);

            const errorMessage = parseApiError(error);

            toast.error('Erro ao atualizar endereço', {
                description: errorMessage,
                duration: 5000,
            });
        },
    });
};

export const useDeleteAddressMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (addressId: string) => userAddressesApi.remove(addressId),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: addressKeys.all });

            toast.success('Endereço removido!');
        },
        onError: (error: any) => {
            console.error('Erro ao deletar endereço:', error);

            const errorMessage = parseApiError(error);

            toast.error('Erro ao remover endereço', {
                description: errorMessage,
                duration: 5000,
            });
        },
    });
};
