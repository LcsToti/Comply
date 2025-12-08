import type { Categories, ProductCondition, ProductSortBy } from "@/types/product";
import type { ListingStatus } from "@/types/listing";
import api from "../axios";

export interface CreateProductParams {
    Title: string;
    Description: string;
    Locale: string;
    Characteristics: Record<string, string>;
    Condition: string;
    Category: string;
    DeliveryPreference: string;
    ImageUrls: File[];
}
export interface UpdateProductParams {
    Title?: string;
    Description?: string;
    Locale?: string;
    Characteristics?: Record<string, string>;
    Condition?: string;
    Category?: string;
    ImageUrls?: File[];
}
export interface AddImagesParams {
    Images: File[];
}
export interface ImageUrlsParams {
    ImageUrls: string[];
}
export interface GetFilteredProductsParams {
    PageNumber?: number;
    PageSize?: number;
    SearchTerm?: string;
    SellerId?: string;
    Category?: Categories;
    Condition?: ProductCondition;
    ListingStatus?: ListingStatus;
    MinPrice?: number;
    MaxPrice?: number;
    IsAuctionActive?: boolean;
    NoBidsOnly?: boolean;
    SortBy?: ProductSortBy;
}
export interface PaginationParams {
    pageNumber: number;
    pageSize: number;
}

export class ProductsApi {
    private readonly baseUrl = "products";

    async getFiltered(params?: GetFilteredProductsParams) {
        const { data } = await api.get(this.baseUrl, { params });
        return data;
    }

    async getById(productId: string) {
        const { data } = await api.get(`${this.baseUrl}/${productId}`);
        return data;
    }

    async create(params: CreateProductParams, isTest = false) {
        const formData = new FormData();

        formData.append("Title", params.Title);
        formData.append("Description", params.Description);
        formData.append("Locale", params.Locale);
        formData.append("Condition", params.Condition);
        formData.append("Category", params.Category);
        formData.append("DeliveryPreference", params.DeliveryPreference);

        Object.entries(params.Characteristics).forEach(([key, value]) => {
            formData.append(`Characteristics[${key}]`, value);
        });

        params.ImageUrls.forEach((file) => formData.append("ImageUrls", file));

        const { data } = await api.post(this.baseUrl, formData, {
            params: { isTest },
        });

        return data;
    }

    async update(productId: string, params: UpdateProductParams) {
        const formData = new FormData();

        if (params.Title) formData.append("Title", params.Title);
        if (params.Description) formData.append("Description", params.Description);
        if (params.Locale) formData.append("Locale", params.Locale);
        if (params.Condition) formData.append("Condition", params.Condition);
        if (params.Category) formData.append("Category", params.Category);

        if (params.Characteristics) {
            Object.entries(params.Characteristics).forEach(([key, value]) => {
                formData.append(`Characteristics[${key}]`, value);
            });
        }

        if (params.ImageUrls) {
            params.ImageUrls.forEach((file) => formData.append("ImageUrls", file));
        }

        const { data } = await api.patch(`${this.baseUrl}/${productId}`, formData, {
            headers: { "Content-Type": "multipart/form-data" },
        });

        return data;
    }

    async addImages(productId: string, params: AddImagesParams) {
        const formData = new FormData();
        params.Images.forEach((file) => formData.append("ImageUrls", file));

        const { data } = await api.post(`${this.baseUrl}/${productId}/images`, formData);
        return data;
    }

    async reorderImages(productId: string, params: ImageUrlsParams) {
        const { data } = await api.put(`${this.baseUrl}/${productId}/images`, params);
        return data;
    }

    async removeImage(productId: string, params: ImageUrlsParams) {
        const { data } = await api.delete(`${this.baseUrl}/${productId}/images`, {
            data: params,
        });
        return data;
    }

    async addFeature(productId: string, durationInDays: number) {
        const { data } = await api.post(`${this.baseUrl}/${productId}/feature`, durationInDays);
        return data;
    }

    async getCount() {
        const { data } = await api.get(`${this.baseUrl}/count`);
        return data;
    }

    async getActiveAuctionsCount() {
        const { data } = await api.get(`${this.baseUrl}/ActiveAuctions/count`);
        return data;
    }

    async getMyBidded(params: PaginationParams) {
        const { data } = await api.get(`${this.baseUrl}/me/bidded`, { params });
        return data;
    }

    async getMyBought(params: PaginationParams) {
        const { data } = await api.get(`${this.baseUrl}/me/bought`, { params });
        return data;
    }

    async getMyListed(params: PaginationParams) {
        const { data } = await api.get(`${this.baseUrl}/me/listed`, { params });
        return data;
    }

    async getMyOutbidded(params: PaginationParams) {
        const { data } = await api.get(`${this.baseUrl}/me/outbid`, { params });
        return data;
    }

    async getMyWinning(params: PaginationParams) {
        const { data } = await api.get(`${this.baseUrl}/me/winning`, { params });
        return data;
    }
}

export const productsApi = new ProductsApi();