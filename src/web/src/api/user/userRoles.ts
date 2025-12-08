import api from "../axios";

export type Role = "User" | "Moderator" | "Admin";
export interface ChangeRoleRequest {
    NewRole: Role;
}

export class UsersRolesApi {
    private readonly baseUrl = "users";

    async getUserRole(userId: string): Promise<Role> {
        const { data } = await api.get(`${this.baseUrl}/${userId}/role`);
        return data;
    }

    async changeUserRole(userId: string, request: ChangeRoleRequest): Promise<void> {
        await api.put(`${this.baseUrl}/${userId}/role`, request);
    }
}

export const usersRolesApi = new UsersRolesApi();