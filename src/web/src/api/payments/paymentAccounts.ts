import api from "../axios";

export class PaymentAccountsApi {
    private readonly baseUrl = "paymentaccounts";

    async getPaymentMethods() {
        const { data } = await api.get(`${this.baseUrl}/payment-methods`);
        return data;
    }

    async createPaymentMethod() {
        const { data } = await api.post(`${this.baseUrl}/payment-methods`);
        return data;
    }

    async getOnboardingLink() {
        const { data } = await api.get(`${this.baseUrl}/onboarding-link`);
        return data;
    }

    async getDashboardLink() {
        const { data } = await api.get(`${this.baseUrl}/dashboard-link`);
        return data;
    }

    async getCashBalance() {
        const { data } = await api.get(`${this.baseUrl}/cash-balance`);
        return data;
    }

    async getSellerAccountStatus() {
        const { data } = await api.get(`${this.baseUrl}/status`);
        return data;
    }
}

export const paymentAccountsApi = new PaymentAccountsApi();