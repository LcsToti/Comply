import api, { handleLogout, setApiToken } from "@/api/axios";
import SessionLoading from "@/components/Loading/SessionLoading";
import { AuthContext } from "@/contexts/AuthContext";
import { useSellerAccountStatusQuery } from "@/hooks/payments/usePaymentAccountsQueries";
import { useMyProfileQuery } from "@/hooks/user/useUsersQueries";
import { useQueryClient } from "@tanstack/react-query";
import { useEffect, useState, type ReactNode } from "react";

export default function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(null);
  const [isRefreshing, setIsRefreshing] = useState(true);
  const queryClient = useQueryClient();

  const login = (newToken: string) => {
    setToken(newToken);
    setApiToken(newToken);
  };

  const logout = () => {
    handleLogout("Deslogado!");
    queryClient.removeQueries({ queryKey: ["myProfile"] });
    setToken(null);
  };

  useEffect(() => {
    const handleLogout = () => {
      if (token) {
        logout();
      }
    };
    window.addEventListener("logoutRequest", handleLogout);
    return () => {
      window.removeEventListener("logoutRequest", handleLogout);
    };
  }, [token]);

  useEffect(() => {
    const attemptSilentRefresh = async () => {
      try {
        const { data } = await api.post("auth/refresh");
        const newToken = data.token;
        if (newToken) {
          login(newToken);
        }
      } catch {
        console.log("Nenhum refresh token válido encontrado.");
      } finally {
        setIsRefreshing(false);
      }
    };

    attemptSilentRefresh();
  }, []);

  const isLoggedIn = !!token;

  const { data: user, isLoading: isLoadingUser } = useMyProfileQuery(
    isLoggedIn && !isRefreshing
  );

  const { data: accountStatus, isLoading: isLoadingAccountStatus } =
    useSellerAccountStatusQuery(isLoggedIn && !isRefreshing);

  if (isRefreshing) {
    return <SessionLoading />;
  }
  return (
    <AuthContext.Provider
      value={{
        token,
        isLoggedIn,
        accountStatus,
        login,
        logout,
        user,
        isLoadingUser: isLoadingUser || isRefreshing,
        isLoadingAccountStatus: isLoadingAccountStatus || isRefreshing,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}
