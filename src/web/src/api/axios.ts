import axios, { AxiosError, type AxiosRequestConfig } from "axios";
import { toast } from "sonner";
import { authApi } from "./user/auth";
import { logger } from '@/utils/logger'; // <- nosso helper de logs

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    withCredentials: true
});

let inMemoryToken: string | null = null;
let isRefreshing: Promise<string | null> | null = null; // Armazena a promessa do refresh
let failedQueue: Array<{ resolve: (value: unknown) => void; reject: (reason?: any) => void; config: AxiosRequestConfig }> = []; // Requisições que falharam e esperam o novo token

const processQueue = (error: AxiosError | null, token: string | null = null) => {
    failedQueue.forEach(p => {
        if (error) {
            p.reject(error);
        } else {
            p.config.headers = p.config.headers || {};
            p.config.headers.Authorization = `Bearer ${token}`;
            p.resolve(api(p.config));
        }
    });
    failedQueue = [];
};

export const setApiToken = (token: string | null) => {
    inMemoryToken = token;
    if (token) {
        api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        logger.log("[API] Token definido em memória:", token);
    } else {
        delete api.defaults.headers.common['Authorization'];
        logger.log("[API] Token removido da memória");
    }
};

export const getApiToken = (): string | null => inMemoryToken;

export const clearApiToken = (): void => {
    logger.log("[API] Deletando JWT InMemory...");
    inMemoryToken = null;
    delete api.defaults.headers.common['Authorization'];
};

api.interceptors.request.use(config => {
    if (inMemoryToken) {
        config.headers.Authorization = `Bearer ${inMemoryToken}`;
        logger.log(`[API] Adicionando token no header da requisição para ${config.url}`);
    }
    return config;
});

api.interceptors.response.use(
    response => response,
    async (error: AxiosError) => {
        const originalRequest = error.config as AxiosRequestConfig & { _retry?: boolean };

        // Se não for erro 401, rejeita imediatamente
        if (error.response?.status !== 401) {
            return Promise.reject(error);
        }

        // 1. Trata casos de falha crítica (401 na rota de refresh) OU ignora se for 401 sem token
        if (originalRequest.url?.includes("auth/refresh")) {
            logger.log(`[API] 401 no refresh.`);

            if (isRefreshing) {
                logger.log("[API] Limpando fila de requisições pendentes devido a falha no refresh.");
                processQueue(error);
            }
            
            if (!inMemoryToken) {
                logger.log("[API] Refresh falhou mas não havia token em memória. Usuário não estava logado. Ignorando logout.");   
                return Promise.reject(error);
            }
            
            logger.log("[API] Refresh falhou e havia token em memória. Sessão expirada, realizando logout.");
            handleLogout("Sessão expirada. Faça login novamente.");
            return Promise.reject(error);
        }

        // Se deu 401 E não é a rota de refresh, verifica se tem token em memória para tentar o refresh
        if (!inMemoryToken) {
            // Este é o caso que você quer cobrir: 401, mas sem token em memória.
            // Apenas rejeita o erro sem tentar refresh e sem disparar logout.
            logger.log(`[API] Erro 401 com inMemoryToken ausente. Não é necessário refresh/logout. Rejeitando.`);
            return Promise.reject(error);
        }

        // 2. Refresh já está em andamento (concorrência), adiciona à fila de espera
        if (isRefreshing) {
            logger.log(`[API] Refresh já em andamento, adicionando requisição para ${originalRequest.url} à fila.`);
            return new Promise((resolve, reject) => {
                failedQueue.push({ resolve, reject, config: originalRequest });
            });
        }

        // 3. Inicia um novo processo de refresh (primeira ocorrência)
        logger.log(`[API] Tentando refresh token por causa de 401 em ${originalRequest.url}`);
                
        // Armazena a promessa de refresh, bloqueando novas tentativas
        isRefreshing = new Promise<string | null>(async (resolve) => {
            try {
                const { data } = await api.post('auth/refresh');

                if (!data?.token) {
                    throw new Error('Token não recebido do servidor');
                }

                logger.log(`[API] Refresh token bem-sucedido, novo token recebido.`);

                setApiToken(data.token);

                // Processa a fila com sucesso e o novo token
                processQueue(null, data.token);
                resolve(data.token);

            } catch (refreshError) {
                logger.log(`[API] Falha no refresh token.`);
                // Processa a fila com erro
                processQueue(refreshError as AxiosError);
                handleLogout("Sessão expirada. Faça login novamente.");
                resolve(null); // Resolve a promessa do refresh como nula
            } finally {
                isRefreshing = null; // Libera o isRefreshing
            }
        });

        // 4. Aguarda a resolução do refresh para refazer a primeira requisição
        return isRefreshing.then(token => {
            if (token) {
                originalRequest.headers = originalRequest.headers || {};
                originalRequest.headers.Authorization = `Bearer ${token}`;
                logger.log(`[API] Refazendo requisição original para ${originalRequest.url} com novo token (após refresh)`);
                return api(originalRequest);
            }
            // Se o refresh falhou (token nulo), rejeita a requisição original
            return Promise.reject(error); 
        });
    }
);

export async function handleLogout(message?: string): Promise<void> {
    try {
        logger.log("[API] Chamando logout no backend...");
        await authApi.logout();
        logger.log("[API] Logout backend finalizado");
    } catch (err) {
        logger.warn('[API] Erro ao chamar logout no backend (ignorado):', err);
    }

    clearApiToken();

    window.dispatchEvent(new CustomEvent('logoutRequest', {
        detail: { message }
    }));

    if (message && typeof window !== 'undefined') {
        toast.error(message);
    }
}

export default api;