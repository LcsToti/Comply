import api from "../axios";

export interface RegisterParams {
    Name: string;
    Email: string;
    Password: string;
}
export interface LoginParams {
    Email: string;
    Password: string;
}
export interface AuthenticationResponse {
    token: string;
}

export class AuthApi {
    private readonly baseUrl = "auth";

    async register(params: RegisterParams): Promise<AuthenticationResponse> {
        const { data } = await api.post(`${this.baseUrl}/register`, params);
        return { token: data.token };
    }

    async login(params: LoginParams): Promise<AuthenticationResponse> {
        const { data } = await api.post(`${this.baseUrl}/login`, params);
        return { token: data.token };
    }
    
    async logout(): Promise<void> {
        console.log("Realizando logout para " + "${this.baseUrl}/logout");
        await api.post(`${this.baseUrl}/logout`);
    }
}

export const authApi = new AuthApi();