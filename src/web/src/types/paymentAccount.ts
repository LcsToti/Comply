export type PaymentMethod = {
    id: string,
    type: string,
    last4: string,
    brand: string
}

export const PaymentAccountStatus = {
    None: 0,
    Incomplete: 1,
    PendingReview: 2,
    Issues: 3,
    Active: 4,
} as const;

export type PaymentAccountStatus =
    (typeof PaymentAccountStatus)[keyof typeof PaymentAccountStatus];

export const PaymentAccountStatusLabels: Record<PaymentAccountStatus, string> = {
    [PaymentAccountStatus.None]: "Sem conta de vendedor criada",
    [PaymentAccountStatus.Incomplete]: "Incompleto",
    [PaymentAccountStatus.PendingReview]: "Aguardando revisão",
    [PaymentAccountStatus.Issues]: "Problemas detectados",
    [PaymentAccountStatus.Active]: "Ativo",
};