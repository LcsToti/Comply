
import { productsApi, type AddImagesParams, type CreateProductParams, type ImageUrlsParams, type UpdateProductParams } from '@/api/products/products';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { productKeys } from './useProductsQueries';
import { notificationKeys } from '../notifications/useNotificationsQueries';

export const useCreateProductMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ params, isTest }: { params: CreateProductParams; isTest?: boolean }) =>
            productsApi.create(params, isTest),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: productKeys.lists() });
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });

            toast.success('Produto criado com sucesso!', {
                description: 'Agora você pode criar um anúncio ou leilão'
            });
        },
        onError: (error: any) => {
            console.error('Erro ao criar produto:', error);

            toast.error('Erro ao criar produto', {
                description: error.response?.data?.message || 'Verifique os dados e tente novamente',
            });
        },
    });
};

export const useUpdateProductMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ productId, params }: { productId: string; params: UpdateProductParams }) =>
            productsApi.update(productId, params),
        onSuccess: (_, { productId }) => {
            queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });
            queryClient.invalidateQueries({ queryKey: productKeys.lists() });
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });

            toast.success('Produto atualizado!');
        },
        onError: (error: any) => {
            console.error('Erro ao atualizar produto:', error);

            toast.error('Erro ao atualizar produto', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};

export const useAddImagesMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ productId, params }: { productId: string; params: AddImagesParams }) =>
            productsApi.addImages(productId, params),
        onSuccess: (_, { productId }) => {
            queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });

            toast.success('Imagens adicionadas!');
        },
        onError: (error: any) => {
            console.error('Erro ao adicionar imagens:', error);

            toast.error('Erro ao adicionar imagens', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};

export const useReorderImagesMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ productId, params }: { productId: string; params: ImageUrlsParams }) =>
            productsApi.reorderImages(productId, params),
        onSuccess: (_, { productId }) => {
            queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });

            toast.success('Ordem das imagens atualizada!');
        },
        onError: (error: any) => {
            console.error('Erro ao reordenar imagens:', error);

            toast.error('Erro ao reordenar imagens', {
                description: 'Tente novamente',
            });
        },
    });
};

export const useRemoveImageMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ productId, params }: { productId: string; params: ImageUrlsParams }) =>
            productsApi.removeImage(productId, params),
        onSuccess: (_, { productId }) => {
            queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });

            toast.success('Imagem removida!');
        },
        onError: (error: any) => {
            console.error('Erro ao remover imagem:', error);

            toast.error('Erro ao remover imagem', {
                description: 'Tente novamente',
            });
        },
    });
};

export const useAddFeatureMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ productId, durationInDays }: { productId: string; durationInDays: number }) =>
            productsApi.addFeature(productId, durationInDays),
        onSuccess: (_, { productId }) => {
            queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });
            queryClient.invalidateQueries({ queryKey: productKeys.lists() });
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });

            toast.success('Produto destacado com sucesso!', {
                description: 'Seu produto terá maior visibilidade'
            });
        },
        onError: (error: any) => {
            console.error('Erro ao destacar produto:', error);

            toast.error('Erro ao destacar produto', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};
