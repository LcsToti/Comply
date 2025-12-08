import api from "../axios";

export interface GetSuccessfulPaymentsParams {
    amount: number;
}

export class PaymentsApi {
    private readonly baseUrl = "payments";

    async getById(paymentId: string) {
        const { data } = await api.get(`${this.baseUrl}/${paymentId}`);
        return data;
    }

    async getLastSuccessful(amount: number) {
        const { data } = await api.get(`${this.baseUrl}/LastSuccessful`, {
            params: { amount },
        });
        return data;
    }

    async withdrawAll() {
        const { data } = await api.post(`${this.baseUrl}/withdraw-all`);
        return data;
    }

    async withdraw(paymentId: string) {
        const { data } = await api.post(`${this.baseUrl}/withdraw/${paymentId}`);
        return data;
    }
}

export const paymentsApi = new PaymentsApi();