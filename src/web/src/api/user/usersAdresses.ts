import api from "../axios";

export interface DeliveryAddressParams {
    Id?: string;
    Street: string;
    Number: string;
    City: string;
    State: string;
    ZipCode: string;
}

export class UserAddressesApi {
    private readonly baseUrl = "users/addresses/me";

    async getAll() {
        const { data } = await api.get(this.baseUrl);
        return data;
    }

    async getById(addressId: string) {
        const { data } = await api.get(`${this.baseUrl}/${addressId}`);
        return data;
    }

    async add(address: DeliveryAddressParams) {
        const { data } = await api.post(this.baseUrl, address);
        return data;
    }

    async update(addressId: string, address: DeliveryAddressParams) {
        await api.put(`${this.baseUrl}/${addressId}`, address);
    }

    async remove(addressId: string) {
        await api.delete(`${this.baseUrl}/${addressId}`);
    }
}

export const userAddressesApi = new UserAddressesApi();