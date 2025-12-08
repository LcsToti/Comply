// hooks/listings/useListingMutations.ts
import { listingsApi, type CreateListingParams } from '@/api/listings/listings';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { listingKeys } from './useListingsQueries';
import { toast } from 'sonner';
import { notificationKeys } from '../notifications/useNotificationsQueries';
import { productKeys } from '../products/useProductsQueries';
import { saleKeys } from '../sales/useSalesQueries';

export const useCreateListingMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (params: CreateListingParams) => listingsApi.create(params),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: listingKeys.lists() });
            queryClient.invalidateQueries({ queryKey: productKeys.lists() });
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });

            toast.success('Anúncio criado com sucesso!', {
                description: 'Seu produto está disponível para compra',
            });
        },
        onError: (error: any) => {
            toast.error('Erro ao criar anúncio', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};

export const useToggleListingAvailabilityMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (listingId: string) => listingsApi.toggleAvailability(listingId),
        onSuccess: (data, listingId) => {
            queryClient.invalidateQueries({ queryKey: listingKeys.detail(listingId) });
            queryClient.invalidateQueries({ queryKey: listingKeys.lists() });
            queryClient.invalidateQueries({ queryKey: productKeys.lists() });
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });

            const isAvailable = data.status === 'Available';
            toast.success(
                isAvailable ? 'Anúncio ativado!' : 'Anúncio pausado',
                {
                    description: isAvailable
                        ? 'Seu produto está visível para compradores'
                        : 'Seu produto foi ocultado temporariamente',
                }
            );
        },
        onError: (error: any) => {
            toast.error('Erro ao alterar disponibilidade', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};

export const useBuyNowMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ listingId, paymentMethodId }: {
            listingId: string;
            paymentMethodId: string
        }) => listingsApi.buyNow(listingId, paymentMethodId),
        onSuccess: (_, { listingId }) => {
            queryClient.invalidateQueries({ queryKey: listingKeys.detail(listingId) });
            queryClient.invalidateQueries({ queryKey: listingKeys.lists() });
            queryClient.invalidateQueries({ queryKey: productKeys.lists() });
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });
            queryClient.invalidateQueries({ queryKey: saleKeys.all });

            toast.success('Compra realizada com sucesso!', {
                description: 'Acompanhe o status na sua área de compras',
            });
        },
        onError: (error: any) => {
            const message = error.response?.data?.message;

            if (message?.includes('indisponível')) {
                toast.error('Produto indisponível', {
                    description: 'Este produto já foi vendido ou está pausado',
                });
            } else if (message?.includes('pagamento')) {
                toast.error('Erro no pagamento', {
                    description: 'Verifique seu método de pagamento e tente novamente',
                });
            } else {
                toast.error('Erro ao finalizar compra', {
                    description: 'Tente novamente ou entre em contato com o suporte',
                });
            }
        },
    });
};

export const useUpdateListingPriceMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ listingId, newBuyPrice }: {
            listingId: string;
            newBuyPrice: number
        }) => listingsApi.updatePrice(listingId, newBuyPrice),
        onSuccess: (_, { listingId }) => {
            queryClient.invalidateQueries({ queryKey: listingKeys.detail(listingId) });
            queryClient.invalidateQueries({ queryKey: listingKeys.lists() });
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });

            toast.success('Preço atualizado!', {
                description: 'O novo valor já está visível',
            });
        },
        onError: (error: any) => {
            const message = error.response?.data?.message;

            if (message?.includes('mínimo')) {
                toast.error('Preço inválido', {
                    description: 'O preço deve ser maior que o valor mínimo',
                });
            } else {
                toast.error('Erro ao atualizar preço', {
                    description: 'Tente novamente',
                });
            }
        },
    });
};
