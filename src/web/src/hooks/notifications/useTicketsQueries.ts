import { useQuery } from '@tanstack/react-query';
import type { Ticket } from '@/types/ticket';
import { ticketsApi } from '@/api/notifications/tickets';

export const ticketKeys = {
    all: ['tickets'] as const,
    allTickets: () => [...ticketKeys.all, 'admin'] as const,
    myTickets: () => [...ticketKeys.all, 'my'] as const,
    details: () => [...ticketKeys.all, 'detail'] as const,
    detail: (id: string) => [...ticketKeys.details(), id] as const,
};

export const useAllTicketsQuery = (isAdmin: boolean = false) => {
    return useQuery<Ticket[]>({
        queryKey: ticketKeys.allTickets(),
        queryFn: () => ticketsApi.getAll(),
        enabled: isAdmin,
        staleTime: 5 * 60 * 1000,
        refetchInterval: 3 * 60 * 1000,
    });
};

export const useMyTicketsQuery = () => {
    return useQuery<Ticket[]>({
        queryKey: ticketKeys.myTickets(),
        queryFn: () => ticketsApi.getMyTickets(),
        staleTime: 1 * 60 * 1000,
    });
};

export const useTicketQuery = (ticketId?: string) => {
    return useQuery<Ticket>({
        queryKey: ticketKeys.detail(ticketId!),
        queryFn: () => ticketsApi.getById(ticketId!),
        enabled: !!ticketId,
        staleTime: 5 * 1000,
        refetchInterval: 10 * 1000,
    });
};
