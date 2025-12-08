import { productQnaApi, type AnswerRequest, type QuestionRequest } from '@/api/products/productsQnA';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { productKeys } from './useProductsQueries';
import { toast } from 'sonner';

export const useAddQuestionMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ productId, params }: { productId: string; params: QuestionRequest }) =>
            productQnaApi.addQuestion(productId, params),
        onSuccess: (_, { productId }) => {
            queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });

            toast.success('Pergunta enviada!', {
                description: 'O vendedor será notificado'
            });
        },
        onError: (error: any) => {
            console.error('Erro ao adicionar pergunta:', error);

            toast.error('Erro ao enviar pergunta', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};

export const useUpdateQuestionMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({
            productId,
            questionId,
            params
        }: {
            productId: string;
            questionId: string;
            params: QuestionRequest
        }) => productQnaApi.updateQuestion(productId, questionId, params),
        onSuccess: (_, { productId }) => {
            queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });

            toast.success('Pergunta atualizada!');
        },
        onError: (error: any) => {
            console.error('Erro ao atualizar pergunta:', error);

            toast.error('Erro ao atualizar pergunta', {
                description: 'Tente novamente',
            });
        },
    });
};

export const useRemoveQuestionMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ productId, questionId }: { productId: string; questionId: string }) =>
            productQnaApi.removeQuestion(productId, questionId),
        onSuccess: (_, { productId }) => {
            queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });

            toast.success('Pergunta removida');
        },
        onError: (error: any) => {
            console.error('Erro ao remover pergunta:', error);

            toast.error('Erro ao remover pergunta', {
                description: 'Tente novamente',
            });
        },
    });
};

export const useAnswerQuestionMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({
            productId,
            questionId,
            params
        }: {
            productId: string;
            questionId: string;
            params: AnswerRequest
        }) => productQnaApi.answerQuestion(productId, questionId, params),
        onSuccess: (_, { productId }) => {
            queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });

            toast.success('Resposta enviada!', {
                description: 'O comprador será notificado'
            });
        },
        onError: (error: any) => {
            console.error('Erro ao responder pergunta:', error);

            toast.error('Erro ao enviar resposta', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};

export const useUpdateAnswerMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({
            productId,
            questionId,
            params
        }: {
            productId: string;
            questionId: string;
            params: AnswerRequest
        }) => productQnaApi.updateAnswer(productId, questionId, params),
        onSuccess: (_, { productId }) => {
            queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });

            toast.success('Resposta atualizada!');
        },
        onError: (error: any) => {
            console.error('Erro ao atualizar resposta:', error);

            toast.error('Erro ao atualizar resposta', {
                description: 'Tente novamente',
            });
        },
    });
};

export const useRemoveAnswerMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ productId, questionId }: { productId: string; questionId: string }) =>
            productQnaApi.removeAnswer(productId, questionId),
        onSuccess: (_, { productId }) => {
            queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });

            toast.success('Resposta removida');
        },
        onError: (error: any) => {
            console.error('Erro ao remover resposta:', error);

            toast.error('Erro ao remover resposta', {
                description: 'Tente novamente',
            });
        },
    });
};
