import type { PaymentAccountStatus } from "@/types/paymentAccount";
import type { User } from "@/types/user";
import { createContext, useContext } from "react";

interface AuthContextType {
    token: string | null;
    isLoggedIn: boolean;
    accountStatus: PaymentAccountStatus | undefined;
    login: (token: string) => void;
    logout: () => void;
    user: User | undefined;
    isLoadingUser: boolean;
    isLoadingAccountStatus: boolean;
}

export const AuthContext = createContext<AuthContextType | undefined>(
    undefined,
);

export function useAuth() {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error("useAuth must be used within an AuthProvider");
    }
    return context;
}