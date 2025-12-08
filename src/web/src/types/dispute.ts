export type Dispute = {
    id: string;
    adminId?: string | null;
    status: DisputeStatus;
    resolutionStatus: DisputeResolutionStatus;
    reason: string;
    resolution?: string | null;
    createdAt: string;
    updatedAt: string;
    resolvedAt?: string | null;
    expiresAt: string;
}

export const DisputeStatus = {
    Pending: 0,
    InAnalysis: 1,
    Closed: 2,
} as const;
export type DisputeStatus = (typeof DisputeStatus)[keyof typeof DisputeStatus];

export const DisputeResolutionStatus = {
    Solved: 0,
    Unsolved: 1,
    Refunded: 2,
    ApprovedWithdrawal: 3,
    ResolvedByAdmin: 4,
    Expired: 5,
} as const;
export type DisputeResolutionStatus = (typeof DisputeResolutionStatus)[keyof typeof DisputeResolutionStatus];

export const disputeStatusLabels: Record<DisputeStatus, string> = {
    0: "Pendente",
    1: "Em Análise",
    2: "Encerrada",
};

export const disputeResolutionLabels: Record<DisputeResolutionStatus, string> = {
    0: "Resolvido",
    1: "Não Resolvido",
    2: "Reembolsado",
    3: "Saque Aprovado",
    4: "Resolvido por Admin",
    5: "Expirado",
};

export const disputeResolutionDescriptions: Record<
    DisputeResolutionStatus,
    string
> = {
    0: "A disputa foi resolvida com sucesso entre as partes",
    1: "A disputa não pôde ser resolvida",
    2: "O valor foi reembolsado ao comprador",
    3: "Saque aprovado para o vendedor",
    4: "Resolvido por intervenção de um administrador",
    5: "A disputa expirou sem resolução",
};
