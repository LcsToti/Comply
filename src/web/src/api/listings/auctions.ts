import api from "../axios";

export interface GetAuctionsParams {
    MaxStartBid?: number;
    StartsBefore?: string;
    EndsBefore?: string;
    Page?: number;
    PageSize?: number;
}
export interface CreateAuctionParams {
    ListingId: string;
    StartBidValue: number;
    WinBidValue: number;
    StartDate: Date;
    EndDate: Date;
}
export interface PlaceNewBidParams {
    Value: number;
    PaymentMethodId: string;
}
export interface NewAuctionSettingsParams {
    StartBidValue: number;
    WinBidValue: number;
    StartDate: string;
    EndDate: string;
}
export class AuctionsApi {
    getActiveCount(): number | Promise<number> {
        throw new Error('Method not implemented.');
    }
    private readonly baseUrl = "auctions";

    async getAll(params?: GetAuctionsParams) {
        const { data } = await api.get(this.baseUrl, { params });
        return data;
    }

    async getById(auctionId: string) {
        const { data } = await api.get(`${this.baseUrl}/${auctionId}`);
        return data;
    }

    async create(params: CreateAuctionParams) {
        const { data } = await api.post(this.baseUrl, params);
        return data;
    }

    async getBids(auctionId: string) {
        const { data } = await api.get(`${this.baseUrl}/${auctionId}/bids`);
        return data;
    }

    async placeBid(auctionId: string, params: PlaceNewBidParams) {
        await api.post(`${this.baseUrl}/${auctionId}/bids`, params);
        return true;
    }

    async cancel(auctionId: string) {
        const { data } = await api.patch(`${this.baseUrl}/${auctionId}/cancel`);
        return data;
    }

    async updateSettings(auctionId: string, params: NewAuctionSettingsParams) {
        const { data } = await api.put(`${this.baseUrl}/${auctionId}/settings`, params);
        return data;
    }
}

export const auctionsApi = new AuctionsApi();