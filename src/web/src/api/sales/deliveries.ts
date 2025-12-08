import api from "../axios";

export interface DeliveryCodeRequest {
    Code: string;
}

export class DeliveriesApi {
    private readonly baseUrl = "deliveries";

    async getDeliveryCode(saleId: string) {
        const { data } = await api.get(`${this.baseUrl}/${saleId}/delivery-code`);
        return data;
    }

    async markAsDelivered(saleId: string, params: DeliveryCodeRequest) {
        const { data } = await api.post(`${this.baseUrl}/${saleId}/delivered`, params);
        return data;
    }

    async markAsShipped(saleId: string) {
        const { data } = await api.post(`${this.baseUrl}/${saleId}/shipped`);
        return data;
    }
}

export const deliveriesApi = new DeliveriesApi();