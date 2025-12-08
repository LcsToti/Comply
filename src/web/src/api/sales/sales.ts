import api from "../axios";

export interface GetMySalesParams {
    Page?: number;
    PageSize?: number;
}

export class SalesApi {
    private readonly baseUrl = "sales";

    async getAll() {
        const { data } = await api.get(this.baseUrl);
        return data;
    }

    async getMySales(params?: GetMySalesParams) {
        const { data } = await api.get(`${this.baseUrl}/me`, { params });
        return data;
    }

    async getById(saleId: string) {
        const { data } = await api.get(`${this.baseUrl}/${saleId}`);
        return data;
    }

    async cancel(saleId: string) {
        const { data } = await api.post(`${this.baseUrl}/${saleId}/cancel`);
        return data;
    }
}

export const salesApi = new SalesApi();