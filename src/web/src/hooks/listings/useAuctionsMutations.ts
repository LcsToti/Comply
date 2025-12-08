import { auctionsApi, type CreateAuctionParams, type NewAuctionSettingsParams, type PlaceNewBidParams } from '@/api/listings/auctions';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { auctionKeys } from './useAuctionsQueries';
import { toast } from 'sonner';
import { notificationKeys } from '../notifications/useNotificationsQueries';
import { productKeys } from '../products/useProductsQueries';
import { saleKeys } from '../sales/useSalesQueries';

export const useCreateAuctionMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (params: CreateAuctionParams) => auctionsApi.create(params),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: auctionKeys.lists() });
            queryClient.invalidateQueries({ queryKey: productKeys.lists() });
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });

            toast.success('Leilão criado com sucesso!', {
                description: 'Seu produto está disponível para lances',
            });
        },
        onError: (error: any) => {
            toast.error('Erro ao criar leilão', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};

export const usePlaceBidMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ auctionId, params }: {
            auctionId: string;
            params: PlaceNewBidParams
        }) => auctionsApi.placeBid(auctionId, params),
        onSuccess: (_, { auctionId }) => {
            queryClient.invalidateQueries({ queryKey: auctionKeys.detail(auctionId) });
            queryClient.invalidateQueries({ queryKey: auctionKeys.bids(auctionId) });
            queryClient.invalidateQueries({ queryKey: productKeys.detail(auctionId) });
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });
            queryClient.invalidateQueries({ queryKey: saleKeys.all });

            toast.success('Lance realizado com sucesso!');
        },
        onError: (error: any) => {
            const message = error.response?.data?.message;

            if (message?.includes('valor mínimo')) {
                toast.error('Lance muito baixo', {
                    description: message,
                });
            } else if (message?.includes('encerrado')) {
                toast.error('Leilão encerrado', {
                    description: 'Este leilão já foi finalizado',
                });
            } else {
                toast.error('Erro ao dar lance', {
                    description: 'Tente novamente',
                });
            }
        },
    });
};

export const useCancelAuctionMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (auctionId: string) => auctionsApi.cancel(auctionId),
        onSuccess: (_, auctionId) => {
            queryClient.invalidateQueries({ queryKey: auctionKeys.detail(auctionId) });
            queryClient.invalidateQueries({ queryKey: auctionKeys.lists() });
            queryClient.invalidateQueries({ queryKey: productKeys.lists() });
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });

            toast.success('Leilão cancelado', {
                description: 'Os participantes foram notificados',
            });
        },
        onError: (error: any) => {
            toast.error('Erro ao cancelar leilão', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};

export const useUpdateAuctionSettingsMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ auctionId, params }: {
            auctionId: string;
            params: NewAuctionSettingsParams
        }) => auctionsApi.updateSettings(auctionId, params),
        onSuccess: (_, { auctionId }) => {
            queryClient.invalidateQueries({ queryKey: auctionKeys.detail(auctionId) });
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });

            toast.success('Configurações atualizadas!', {
                description: 'As alterações já estão visíveis',
            });
        },
        onError: (error: any) => {
            toast.error('Erro ao atualizar configurações', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};
