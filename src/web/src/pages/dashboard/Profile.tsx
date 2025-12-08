import Footer from "@/components/Footer";
import Header from "@/components/Header";
import PaymentsTab from "@/components/ProfilePage/PaymentsTab/PaymentsTab";
import ProfileTab from "@/components/ProfilePage/ProfileTab";
import LogoutButton from "@/components/ProfilePage/LogoutButton";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/contexts/AuthContext";
import { cn } from "@/lib/utils";
import { BanknoteArrowUp, KeySquare, User, Wallet } from "lucide-react";
import WithdrawalsTab from "@/components/ProfilePage/WithdrawalTab/WithdrawalsTab";
import { useNavigate, useSearchParams } from "react-router";
import { useMyAddressesQuery } from "@/hooks/user/useUserAdressesQueries";

export default function Profile() {
  const [searchParams, setSearchParams] = useSearchParams();
  const { user } = useAuth();
  const navigate = useNavigate();

  const { data: addressData, isPending: isAddressDataPending } =
    useMyAddressesQuery();

  const activeTab = searchParams.get("tab") || "profile";

  const setActiveTab = (tab: string) => {
    setSearchParams({ tab });
  };

  const getButtonClass = (tabName: string) =>
    cn(
      "justify-start gap-3 px-4 py-6 text-sm sm:text-base transition-all",
      tabName === "logout"
        ? "font-medium text-red-600 hover:bg-red-100 hover:text-red-700 cursor-pointer"
        : activeTab === tabName
          ? "font-semibold text-emerald-700 bg-emerald-100 hover:bg-emerald-100 cursor-pointer"
          : "font-medium text-gray-600 hover:bg-gray-100 hover:text-gray-900 cursor-pointer"
    );

  return (
    <>
      <Header />
      <div className="flex min-h-screen w-full p-4 sm:p-8">
        <div className="mx-auto flex w-full max-w-6xl flex-col gap-6 sm:flex-row sm:gap-16">
          <aside className="flex w-full flex-wrap justify-center gap-2 sm:w-1/4 sm:max-w-[240px] sm:flex-col sm:justify-start">
            <Button
              variant="ghost"
              onClick={() => setActiveTab("profile")}
              className={getButtonClass("profile")}
            >
              <User className="h-5 w-5" />
              Meu Cadastro
            </Button>
            <Button
              variant="ghost"
              onClick={() => setActiveTab("payments")}
              className={getButtonClass("payments")}
            >
              <Wallet className="h-5 w-5" />
              Pedidos e Entrega
            </Button>
            <Button
              variant="ghost"
              onClick={() => setActiveTab("withdrawals")}
              className={getButtonClass("withdrawals")}
            >
              <BanknoteArrowUp className="h-5 w-5" />
              Retiradas
            </Button>
            {user?.role === 2 || user?.role === 3 ? (
              <Button
                variant="ghost"
                onClick={() => {
                  navigate("/admin");
                }}
                className={getButtonClass("restrict")}
              >
                <KeySquare className="h-5 w-5" />
                Acesso Restrito
              </Button>
            ) : (
              ""
            )}

            <LogoutButton />
          </aside>

          {/* Conteúdo principal */}
          <main className="flex-1">
            {activeTab === "profile" && (
              <ProfileTab
                user={user}
                addresses={addressData}
                addressDataIsPending={isAddressDataPending}
              />
            )}
            {activeTab === "payments" && <PaymentsTab />}
            {activeTab === "withdrawals" && <WithdrawalsTab />}
          </main>
        </div>
      </div>
      <Footer />
    </>
  );
}
