import { useQuery } from '@tanstack/react-query';
import type { User } from '@/types/user';
import { usersApi } from '@/api/user/users';

export const userKeys = {
    all: ['users'] as const,
    details: () => [...userKeys.all, 'detail'] as const,
    detail: (id: string) => [...userKeys.details(), id] as const,
    byEmail: (email: string) => [...userKeys.all, 'by-email', email] as const,
    profile: () => ['myProfile'] as const,
    roles: () => [...userKeys.all, 'role'] as const,
    role: (id: string) => [...userKeys.roles(), id] as const,
    counts: () => [...userKeys.all, 'counts'] as const,
    count: () => [...userKeys.counts(), 'total'] as const,
};

export const useMyProfileQuery = (enabled: boolean = true) => {
    return useQuery<User>({
        queryKey: userKeys.profile(),
        queryFn: () => usersApi.getMyProfile(),
        enabled,
        staleTime: 30 * 60 * 1000,
        refetchOnWindowFocus: true,
    });
};

export const useUserQuery = (userId: string, enabled: boolean = true) => {
    return useQuery<User>({
        queryKey: userKeys.detail(userId),
        queryFn: () => usersApi.getById(userId),
        enabled: !!userId && enabled,
        staleTime: 30 * 60 * 1000,
    });
};

export const useUserByEmailQuery = (email: string, enabled: boolean = true) => {
    return useQuery<User>({
        queryKey: userKeys.byEmail(email),
        queryFn: () => usersApi.getByEmail(email),
        enabled: !!email && enabled,
        staleTime: 30 * 60 * 1000,
    });
};

export const useUsersCountQuery = () => {
    return useQuery<number>({
        queryKey: userKeys.count(),
        queryFn: () => usersApi.getCount(),
        staleTime: 60 * 60 * 1000,
    });
};
