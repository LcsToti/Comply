import { useQuery } from '@tanstack/react-query';
import type { UserAddress } from '@/types/user';
import { userAddressesApi } from '@/api/user/usersAdresses';

export const addressKeys = {
    all: ['myAddresses'] as const,
    lists: () => [...addressKeys.all, 'list'] as const,
    details: () => [...addressKeys.all, 'detail'] as const,
    detail: (id: string) => [...addressKeys.details(), id] as const,
};

export const useMyAddressesQuery = () => {
    return useQuery<UserAddress[]>({
        queryKey: addressKeys.all,
        queryFn: () => userAddressesApi.getAll(),
        staleTime: 30 * 60 * 1000,
    });
};

export const useAddressQuery = (addressId: string, enabled: boolean = true) => {
    return useQuery<UserAddress>({
        queryKey: addressKeys.detail(addressId),
        queryFn: () => userAddressesApi.getById(addressId),
        enabled: !!addressId && enabled,
        staleTime: 30 * 60 * 1000,
    });
};
