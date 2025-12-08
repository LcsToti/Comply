export type Notification = {
    id: string;
    type: NotificationType;
    message: string;
    read: boolean;
    createdAt: string;
}

export const NotificationType = {
    Auction: "Auction",
    Ticket: "Ticket",
    Payment: "Payment",
    Admin: "Admin",
}
export type NotificationType = keyof typeof NotificationType;

export const NotificationTypesLabel: Record<NotificationType, string> = {
    Auction: "Leilão",
    Ticket: "Suporte",
    Payment: "Pagamento",
    Admin: "Administração",
};