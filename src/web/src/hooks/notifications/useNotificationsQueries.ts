import { useQuery } from '@tanstack/react-query';
import type { Notification } from '@/types/notification';
import { notificationsApi, type GetNotificationsParams } from '@/api/notifications/notifications';

export const notificationKeys = {
    all: ['notifications'] as const,
    lists: () => [...notificationKeys.all, 'list'] as const,
    list: (params?: GetNotificationsParams) => [...notificationKeys.lists(), params] as const,
    unreadCount: () => [...notificationKeys.all, 'unread-count'] as const,
};

export const useNotificationsQuery = (params: GetNotificationsParams = { Page: 1, PageSize: 1000 }, enabled: boolean = false) => {
    return useQuery<Notification[]>({
        queryKey: notificationKeys.list(params),
        queryFn: () => notificationsApi.getAll(params),
        staleTime: 30 * 1000,
        refetchInterval: 60 * 1000,
        enabled: enabled,
    });
};

export const useUnreadNotificationsCountQuery = () => {
    const { data } = useNotificationsQuery({ Page: 1, PageSize: 1000 });

    const unreadCount = data?.filter((n: Notification) => !n.read).length || 0;

    return {
        unreadCount,
        hasUnread: unreadCount > 0,
    };
};
