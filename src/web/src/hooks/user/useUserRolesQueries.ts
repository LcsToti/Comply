import { useQuery } from '@tanstack/react-query';
import { userKeys } from './useUsersQueries';
import { usersRolesApi, type Role } from '@/api/user/userRoles';

export const useUserRoleQuery = (userId: string, enabled: boolean = true) => {
    return useQuery<Role>({
        queryKey: userKeys.role(userId),
        queryFn: () => usersRolesApi.getUserRole(userId),
        enabled: !!userId && enabled,
        staleTime: 30 * 60 * 1000,
    });
};
