export type Payment = {
    paymentId: string,
    withdrawalStatus: string,
    paymentStatus: string,
    payerId: string,
    amount: PaymentAmount,
    timestamps: PaymentTimestamps
}

export type PaymentAmount = {
    total: number,
    fee: number,
    net: number,
    currency: string
}

export type PaymentTimestamps = {
    createdAt: Date,
    processedAt: Date,
    updatedAt: Date,
    withdrawnAt: Date
}