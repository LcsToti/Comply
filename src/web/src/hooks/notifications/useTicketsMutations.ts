import { useMutation, useQueryClient } from '@tanstack/react-query';
import type { TicketStatus } from '@/types/ticket';
import { toast } from 'sonner';
import { ticketsApi, type CreateTicketParams } from '@/api/notifications/tickets';
import { ticketKeys } from './useTicketsQueries';

export const useCreateTicketMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (params: CreateTicketParams) => ticketsApi.create(params),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ticketKeys.myTickets() });

            toast.success('Ticket criado com sucesso!', {
                description: 'Nossa equipe entrará em contato em breve'
            });
        },
        onError: (error: any) => {
            console.error('Erro ao criar ticket:', error);

            let errorMessage = 'Erro ao criar ticket. Tente novamente.';

            if (error?.response?.data) {
                const errorData = error.response.data;
                if (typeof errorData === 'string') {
                    errorMessage = errorData;
                } else if (errorData.message) {
                    errorMessage = errorData.message;
                } else if (errorData.error) {
                    errorMessage = errorData.error;
                }
            }

            toast.error('Erro ao criar ticket', {
                description: errorMessage,
            });
        },
    });
};

export const useAddCommentMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ ticketId, content }: { ticketId: string; content: string }) =>
            ticketsApi.addComment(ticketId, { Content: content }),
        onSuccess: (_, { ticketId }) => {
            queryClient.invalidateQueries({ queryKey: ticketKeys.detail(ticketId) });
            queryClient.invalidateQueries({ queryKey: ticketKeys.allTickets() });
            queryClient.invalidateQueries({ queryKey: ticketKeys.myTickets() });

            toast.success('Comentário adicionado!');
        },
        onError: (error: any) => {
            toast.error('Erro ao adicionar comentário', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};

export const useUpdateTicketStatusMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ ticketId, newStatus }: { ticketId: string; newStatus: TicketStatus }) =>
            ticketsApi.updateStatus(ticketId, newStatus),
        onSuccess: (_, { ticketId }) => {
            queryClient.invalidateQueries({ queryKey: ticketKeys.detail(ticketId) });
            queryClient.invalidateQueries({ queryKey: ticketKeys.allTickets() });
            queryClient.invalidateQueries({ queryKey: ticketKeys.myTickets() });

            toast.success('Status atualizado com sucesso!');
        },
        onError: (error: any) => {
            toast.error('Erro ao atualizar status', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};
