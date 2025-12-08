import { notificationsApi } from '@/api/notifications/notifications';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { notificationKeys } from './useNotificationsQueries';
import { toast } from 'sonner';

export const useMarkNotificationAsReadMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (notificationId: string) =>
            notificationsApi.markAsRead(notificationId),
        onSuccess: (_) => {
            queryClient.invalidateQueries({ queryKey: notificationKeys.lists() });
            queryClient.invalidateQueries({ queryKey: notificationKeys.unreadCount() });
        },
        onError: (error: any) => {
            toast.error('Erro ao marcar como lida', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};

export const useMarkAllNotificationsAsReadMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: () => notificationsApi.markAllAsRead(),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: notificationKeys.all });

            toast.success('Todas as notificações foram marcadas como lidas');
        },
        onError: (error: any) => {
            toast.error('Erro ao marcar todas como lidas', {
                description: error.response?.data?.message || 'Tente novamente',
            });
        },
    });
};
