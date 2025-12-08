export type Ticket = {
    id: string;
    userId: string;
    title: string;
    description: string;
    createdAt: string;
    status: TicketStatus;
    updatedAt: string;
    assignedAdminId: string | null;
    comments: Comment[];
}

export type Comment = {
    authorId: string;
    content: string;
    createdAt: string;
}

export const TicketStatus = {
    Open: "Open",
    InProgress: "InProgress",
    Resolved: "Resolved",
    Closed: "Closed",
} as const;
export type TicketStatus = (typeof TicketStatus)[keyof typeof TicketStatus];

export const ticketStatusLabels: Record<TicketStatus, string> = {
    ["Open"]: "Aberto",
    ["InProgress"]: "Em Progresso",
    ["Resolved"]: "Resolvido",
    ["Closed"]: "Fechado",
};