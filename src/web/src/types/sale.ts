import type { Dispute } from "./dispute";

export type Sale = {
    id: string;
    productId: string;
    listingId: string;
    paymentId: string;
    buyerId: string;
    sellerId: string;
    productValue: number;
    createdAt: string;
    updatedAt?: string | null;
    status: SaleStatus;
    deliveryStatus: DeliveryStatus;
    dispute: Dispute;
}

export const SaleStatus = {
    AwaitingDelivery: 0,
    Done: 1,
    Dispute: 2,
    Cancelled: 3,
} as const;

export const DeliveryStatus = {
    Pending: 0,
    Shipped: 1,
    Delivered: 2
} as const;

export type SaleStatus = (typeof SaleStatus)[keyof typeof SaleStatus];
export type DeliveryStatus = (typeof DeliveryStatus)[keyof typeof DeliveryStatus];

export const saleStatusLabels: Record<SaleStatus, string> = {
    [SaleStatus.AwaitingDelivery]: "Aguardando Entrega",
    [SaleStatus.Done]: "Concluído",
    [SaleStatus.Dispute]: "Em Disputa",
    [SaleStatus.Cancelled]: "Cancelada",
};

export const saleDeliveredStatusLabels: Record<DeliveryStatus, string> = {
    [DeliveryStatus.Pending]: "Aguardando envio",
    [DeliveryStatus.Shipped]: "Enviado",
    [DeliveryStatus.Delivered]: "Entregue",
};