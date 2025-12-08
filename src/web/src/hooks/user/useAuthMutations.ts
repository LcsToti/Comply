import { authApi, type AuthenticationResponse, type LoginParams, type RegisterParams } from '@/api/user/auth';
import { useMutation } from '@tanstack/react-query';
import { toast } from 'sonner';

function parseApiError(error: any): string {
    let errorMessage = 'Erro ao processar. Tente novamente.';

    if (error?.response?.data) {
        const errorData = error.response.data;

        if (typeof errorData === 'string') {
            errorMessage = errorData;
        } else if (errorData.message) {
            errorMessage = errorData.message;
        } else if (errorData.error) {
            errorMessage = errorData.error;
        } else if (errorData.title && errorData.detail) {
            errorMessage = `${errorData.title}: ${errorData.detail}`;
        } else if (errorData.errors) {
            // Tratamento para erros de validação do ASP.NET
            const validationErrors = Object.values(errorData.errors).flat();
            errorMessage = validationErrors.join(', ');
        }
    }

    return errorMessage;
}

export const useRegisterMutation = () => {
    return useMutation<AuthenticationResponse, Error, RegisterParams>({
        mutationFn: (params) => authApi.register(params),
        onSuccess: () => {
            toast.success('Conta criada com sucesso!', {
                description: 'Bem-vindo! Você já pode começar a usar'
            });
        },
        onError: (error: any) => {
            console.error('Erro no registro:', error);

            const errorMessage = parseApiError(error);

            if (errorMessage.toLowerCase().includes('já existe') ||
                errorMessage.toLowerCase().includes('already exists')) {
                toast.error('Email já cadastrado', {
                    description: 'Este email já está em uso. Faça login ou use outro email',
                    duration: 5000,
                });
            } else if (errorMessage.toLowerCase().includes('senha') ||
                errorMessage.toLowerCase().includes('password')) {
                toast.error('Senha inválida', {
                    description: errorMessage,
                    duration: 5000,
                });
            } else {
                toast.error('Erro ao criar conta', {
                    description: errorMessage,
                    duration: 5000,
                });
            }
        },
    });
};

export const useLoginMutation = () => {
    return useMutation<AuthenticationResponse, Error, LoginParams>({
        mutationFn: (params) => authApi.login(params),
        onSuccess: () => {
            toast.success('Login realizado com sucesso!');
        },
        onError: (error: any) => {
            console.error('Erro no login:', error);

            const errorMessage = parseApiError(error);

            // Mensagens específicas para erros comuns
            if (errorMessage.toLowerCase().includes('credenciais') ||
                errorMessage.toLowerCase().includes('inválido') ||
                errorMessage.toLowerCase().includes('incorrect') ||
                errorMessage.toLowerCase().includes('invalid')) {
                toast.error('Credenciais incorretas', {
                    description: 'Email ou senha incorretos. Tente novamente',
                    duration: 5000,
                });
            } else if (errorMessage.toLowerCase().includes('não encontrado') ||
                errorMessage.toLowerCase().includes('not found')) {
                toast.error('Usuário não encontrado', {
                    description: 'Esta conta não existe. Faça o cadastro primeiro',
                    duration: 5000,
                });
            } else {
                toast.error('Erro ao fazer login', {
                    description: errorMessage,
                    duration: 5000,
                });
            }
        },
    });
};