import api from "../axios";

export interface AddToWatchlistParams {
    ProductId: string;
    ListingId: string;
}
export interface RemoveFromWatchlistParams {
    ProductId: string;
}

export class WatchlistApi {
    private readonly baseUrl = "watchlist";

    async getMyWatchlist() {
        const { data } = await api.get(this.baseUrl);
        return data;
    }

    async add(params: AddToWatchlistParams) {
        const { data } = await api.post(this.baseUrl, params);
        return data;
    }

    async remove(params: RemoveFromWatchlistParams) {
        const { data } = await api.delete(this.baseUrl, { data: params });
        return data;
    }

    async isInWatchlist(productId: string): Promise<boolean> {
        const watchlist = await this.getMyWatchlist();
        return watchlist.some((item: any) => item.productId === productId);
    }
}

export const watchlistApi = new WatchlistApi();