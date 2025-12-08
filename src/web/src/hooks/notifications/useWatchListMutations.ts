import { watchlistApi, type AddToWatchlistParams, type RemoveFromWatchlistParams } from '@/api/notifications/watchLists';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { watchlistKeys } from './useWatchListQueries';
import { toast } from 'sonner';
import { productKeys } from '../products/useProductsQueries';

export const useAddToWatchlistMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (params: AddToWatchlistParams) => watchlistApi.add(params),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: watchlistKeys.all });
            queryClient.invalidateQueries({ queryKey: productKeys.lists() });

            toast.success('Adicionado aos favoritos!', {
                description: 'Você pode acessar na sua lista de favoritos',
            });
        },
        onError: (error: any) => {
            const message = error.response?.data?.message;

            if (message?.includes('já existe')) {
                toast.error('Já está nos favoritos', {
                    description: 'Este produto já está na sua lista',
                });
            } else {
                toast.error('Erro ao adicionar aos favoritos', {
                    description: 'Tente novamente',
                });
            }
        },
    });
};

export const useRemoveFromWatchlistMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (params: RemoveFromWatchlistParams) => watchlistApi.remove(params),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: watchlistKeys.all });
            queryClient.invalidateQueries({ queryKey: productKeys.lists() });

            toast.success('Removido dos favoritos');
        },
        onError: (error: any) => {
            toast.error('Erro ao remover dos favoritos', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};

export const useToggleWatchlistMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async ({
            productId,
            listingId,
            isCurrentlyInWatchlist
        }: {
            productId: string;
            listingId: string;
            isCurrentlyInWatchlist: boolean;
        }) => {
            if (isCurrentlyInWatchlist) {
                return watchlistApi.remove({ ProductId: productId });
            } else {
                return watchlistApi.add({ ProductId: productId, ListingId: listingId });
            }
        },
        onSuccess: (_, { isCurrentlyInWatchlist }) => {
            queryClient.invalidateQueries({ queryKey: watchlistKeys.all });
            queryClient.invalidateQueries({ queryKey: productKeys.lists() });

            toast.success(
                isCurrentlyInWatchlist ? 'Removido dos favoritos' : 'Adicionado aos favoritos!'
            );
        },
        onError: (error: any) => {
            toast.error('Erro ao atualizar favoritos', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};
