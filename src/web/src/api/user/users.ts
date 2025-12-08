import api from "../axios";

export interface UpdateProfileRequest {
    Name?: string;
    PhoneNumber?: string;
}

export class UsersApi {
    private readonly baseUrl = "users";

    async getMyProfile() {
        const { data } = await api.get(`${this.baseUrl}/me`);
        return data;
    }

    async updateMyProfile(params: UpdateProfileRequest) {
        await api.patch(`${this.baseUrl}/me`, params);
    }

    async getById(userId: string) {
        const { data } = await api.get(`${this.baseUrl}/${userId}`);
        return data;
    }

    async getByEmail(email: string) {
        const { data } = await api.get(`${this.baseUrl}/by-email/${email}`);
        return data;
    }

    async getCount() {
        const { data } = await api.get(`${this.baseUrl}/count`);
        return data;
    }
}

export const usersApi = new UsersApi();