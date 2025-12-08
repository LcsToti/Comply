import api from "../axios";

export interface GetNotificationsParams {
    Page?: number;
    PageSize?: number;
}

export class NotificationsApi {
    private readonly baseUrl = "notifications";

    async getAll(params: GetNotificationsParams = { Page: 1, PageSize: 20 }) {
        const { data } = await api.get(this.baseUrl, { params });
        return data;
    }

    async markAsRead(notificationId: string) {
        const { data } = await api.put(`${this.baseUrl}/${notificationId}/read`);
        return data;
    }

    async markAllAsRead() {
        const { data } = await api.put(`${this.baseUrl}/read-all`);
        return data;
    }
}

export const notificationsApi = new NotificationsApi();