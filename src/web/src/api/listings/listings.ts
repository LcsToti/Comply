import api from "../axios";

export interface GetListingsParams {
    MinBuyPrice?: number | null;
    MaxBuyPrice?: number | null;
    Status?: string | null;
    SellerId?: string | null;
    Page?: number;
    PageSize?: number;
}
export interface CreateListingParams {
    ProductId: string;
    BuyPrice: number;
}

export class ListingsApi {
    private readonly baseUrl = "listings";

    async getAll(params?: GetListingsParams) {
        const { data } = await api.get(this.baseUrl, { params });
        return data;
    }

    async getById(listingId: string) {
        const { data } = await api.get(`${this.baseUrl}/${listingId}`);
        return data;
    }

    async create(params: CreateListingParams) {
        const { data } = await api.post(this.baseUrl, params);
        return data;
    }

    async toggleAvailability(listingId: string) {
        const { data } = await api.patch(`${this.baseUrl}/${listingId}/availability`);
        return data;
    }

    async buyNow(listingId: string, paymentMethodId: string) {
        const { data } = await api.patch(`${this.baseUrl}/${listingId}/buynow`, {
            paymentMethod: paymentMethodId,
        });
        return data;
    }

    async updatePrice(listingId: string, newBuyPrice: number) {
        const { data } = await api.patch(`${this.baseUrl}/${listingId}/price`, {
            newBuyPrice,
        });
        return data;
    }
}

export const listingsApi = new ListingsApi();
