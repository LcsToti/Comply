import type { DisputeResolutionStatus } from "@/types/dispute";
import api from "../axios";

export interface OpenDisputeParams {
    Reason: string;
}
export interface CloseDisputeParams {
    Resolution: string;
    ResolutionStatus: DisputeResolutionStatus;
}
export interface GetAllDisputesParams {
    Page?: number;
    PageSize?: number;
}

export class DisputesApi {
    private readonly baseUrl = "disputes";

    async getMyDisputes(params?: GetAllDisputesParams) {
        const { data } = await api.get(this.baseUrl, { params });
        return data;
    }

    async open(saleId: string, params: OpenDisputeParams) {
        const { data } = await api.post(`${this.baseUrl}/${saleId}/open`, params);
        return data;
    }

    async close(saleId: string, params: CloseDisputeParams) {
        const { data } = await api.post(`${this.baseUrl}/${saleId}/close`, params);
        return data;
    }

    async assignAdmin(saleId: string) {
        const { data } = await api.post(`${this.baseUrl}/${saleId}/assign`);
        return data;
    }
}

export const disputesApi = new DisputesApi();