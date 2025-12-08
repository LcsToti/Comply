import { useAuth } from "@/contexts/AuthContext";
import { Alert, AlertDescription } from "@/components/ui/alert";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import {
  AlertTriangle,
  Loader2,
  Banknote,
  TrendingUp,
  Clock,
  CheckCircle,
  Wallet,
  ArrowUpRight,
  ExternalLink,
  ShieldCheck,
} from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { SaleStatus } from "@/types/sale";
import { useState } from "react";
import { toast } from "sonner";
import WithdrawalItem from "./WithdrawalItem";
import { formatCurrency } from "@/utils/formatters/formatCurrency";
import { SellerAccountStatusBanner } from "@/components/SellerAccountStatusBanner";
import { PaymentAccountStatus } from "@/types/paymentAccount";
import NoItemsFound from "@/components/NoItemsFound";
import { useMySalesQuery } from "@/hooks/sales/useSalesQueries";
import {
  useCashBalanceQuery,
  useDashboardLinkQuery,
  useOnboardingLinkQuery,
} from "@/hooks/payments/usePaymentAccountsQueries";
import { useWithdrawAllPaymentsMutation } from "@/hooks/payments/usePaymentsMutations";

const WithdrawalsTab = () => {
  const {
    user,
    accountStatus,
    isLoadingAccountStatus,
    isLoggedIn,
    isLoadingUser,
  } = useAuth();
  const [showWithdrawDialog, setShowWithdrawDialog] = useState(false);

  const {
    data: sales,
    isLoading: isSalesLoading,
    isError: isSalesError,
    error: salesError,
  } = useMySalesQuery();
  const {
    data: cashBalance,
    isLoading: isBalanceLoading,
    isError: isBalanceError,
  } = useCashBalanceQuery();
  const { data: onboardingLink } = useOnboardingLinkQuery(
    isLoggedIn && !isLoadingUser
  );
  const { data: dashboardLink } = useDashboardLinkQuery(
    isLoggedIn && !isLoadingAccountStatus
  );

  const withdrawAll = useWithdrawAllPaymentsMutation();

  const mySales = sales?.filter((sale) => sale.sellerId === user?.id) || [];
  const isLoading = isSalesLoading || isBalanceLoading;

  const calculateStats = () => {
    let totalSales = 0;
    let pendingDelivery = 0;
    let completedSales = 0;

    mySales.forEach((sale) => {
      totalSales += sale.productValue;

      if (sale.status === SaleStatus.AwaitingDelivery) {
        pendingDelivery += sale.productValue;
      } else if (sale.status === SaleStatus.Done) {
        completedSales += sale.productValue;
      }
    });

    return { totalSales, pendingDelivery, completedSales };
  };
  const handleWithdrawAll = () => {
    withdrawAll.mutate(undefined, {
      onSuccess: () => {
        toast.success("Solicitação de saque realizada com sucesso!", {
          description: "Seu saque será processado em até 2 dias úteis.",
          duration: 5000,
        });
        setShowWithdrawDialog(false);
      },
      onError: (error: any) => {
        let errorMessage = "Erro ao solicitar saque. Tente novamente.";
        let errorDescription = "";

        if (error?.response?.data) {
          const errorData = error.response.data;

          if (errorData.status === 422 && errorData.detail) {
            if (
              errorData.detail.includes(
                "There are no cash-out payments available"
              ) ||
              errorData.detail.includes("no cash-out payments")
            ) {
              errorMessage = "Nenhum pagamento disponível para saque";
              errorDescription =
                "Você não possui pagamentos aprovados para saque no momento. Aguarde a confirmação de entrega das suas vendas.";
            } else {
              errorMessage = "Operação inválida";
              errorDescription = errorData.detail;
            }
          } else if (typeof errorData === "string") {
            errorMessage = errorData;
          } else if (errorData.message) {
            errorMessage = errorData.message;
          } else if (errorData.error) {
            errorMessage = errorData.error;
          } else if (errorData.title) {
            errorMessage = errorData.title;
            if (errorData.detail) {
              errorDescription = errorData.detail;
            }
          }
        }

        toast.error(errorMessage, {
          description: errorDescription || undefined,
          duration: 6000,
        });
        setShowWithdrawDialog(false);
      },
    });
  };

  const stats = calculateStats();
  return (
    <main className="flex flex-col gap-3">
      {accountStatus && (
        <SellerAccountStatusBanner
          status={accountStatus}
          onboardingLink={onboardingLink}
          className="mb-5"
        />
      )}

      <header className="relative overflow-hidden mb-5">
        <div className="relative flex flex-col lg:flex-row lg:items-center lg:justify-between">
          {/* Text Content */}
          <div className="space-y-2 flex-1">
            <div className="flex items-center gap-3">
              <h1 className="text-4xl font-bold">Saques</h1>
              <span className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-medium bg-blue-100 border border-blue-200">
                <div className="flex items-center gap-2 text-xs text-neutral-500">
                  <ShieldCheck className="w-4 h-4 text-blue-600" />
                  <span>
                    Pagamentos processados de forma segura pelo Stripe
                  </span>
                </div>
              </span>
            </div>
            <p className="text-base text-neutral-600 max-w-2xl">
              Gerencie e acompanhe seus saques de produtos vendidos
            </p>
          </div>

          {/* CTA Button */}
          {accountStatus === PaymentAccountStatus.Active && (
            <Button
              onClick={() =>
                dashboardLink ? window.open(dashboardLink, "_blank") : ""
              }
              size="lg"
              className="group cursor-pointer bg-blue-500 hover:bg-blue-600 text-white font-medium px-6 transition-all lg:min-w-fit"
            >
              <span className="flex items-center gap-2">
                Ver detalhes completos
                <ExternalLink className="w-4 h-4" />
              </span>
            </Button>
          )}
        </div>
      </header>

      {/* Card Principal - Saldo Disponível */}
      <Card className="shadow-none">
        <CardContent>
          <div className="flex items-center justify-between flex-wrap gap-4">
            <div className="flex-1 min-w-[200px]">
              <div className="flex items-center gap-2 mb-2">
                <Wallet className="h-5 w-5 text-emerald-700" />
                <h3 className="text-sm font-medium text-emerald-900 dark:text-emerald-100">
                  Saldo Disponível para Saque
                </h3>
              </div>
              {isBalanceLoading ? (
                <Loader2 className="h-8 w-8 animate-spin text-emerald-700" />
              ) : isBalanceError ? (
                <p className="text-red-600 text-sm">Erro ao carregar saldo</p>
              ) : (
                <>
                  <div className="text-4xl font-bold text-emerald-700 dark:text-emerald-400">
                    {formatCurrency(cashBalance || 0)}
                  </div>
                  {cashBalance === 0 ? (
                    <p className="text-xs text-amber-700 dark:text-amber-300 mt-1 flex items-center gap-1">
                      <AlertTriangle className="h-3 w-3" />
                      Nenhum pagamento disponível para saque
                    </p>
                  ) : (
                    <p className="text-xs text-emerald-700 dark:text-emerald-300 mt-1">
                      Vendas concluídas e aprovadas para saque
                    </p>
                  )}
                </>
              )}
            </div>
            <div className="flex flex-col gap-2">
              {cashBalance === 0 && !isBalanceLoading && (
                <p className="text-xs text-gray-500 text-center">
                  Complete as entregas para liberar pagamentos
                </p>
              )}
              <Button
                size="lg"
                className="bg-emerald-700 hover:bg-emerald-800 cursor-pointer"
                disabled={
                  !cashBalance ||
                  cashBalance <= 0 ||
                  withdrawAll.isPending ||
                  isBalanceLoading
                }
                onClick={() => setShowWithdrawDialog(true)}
              >
                {withdrawAll.isPending ? (
                  <>
                    <Loader2 className="mr-2 h-5 w-5 animate-spin" />
                    Processando...
                  </>
                ) : (
                  <>
                    <ArrowUpRight className="mr-2 h-5 w-5" />
                    Solicitar Saque
                  </>
                )}
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Cards de Estatísticas */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card className="shadow-none">
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle className="text-sm font-medium text-gray-600">
              Total em Vendas
            </CardTitle>
            <TrendingUp className="h-4 w-4 text-blue-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-blue-600">
              {formatCurrency(stats.totalSales)}
            </div>
            <p className="text-xs text-gray-500 mt-1">
              {mySales.length} {mySales.length === 1 ? "venda" : "vendas"}{" "}
              realizadas
            </p>
          </CardContent>
        </Card>
        <Card className="shadow-none">
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle className="text-sm font-medium text-gray-600">
              Vendas Concluídas
            </CardTitle>
            <CheckCircle className="h-4 w-4 text-emerald-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-emerald-600">
              {formatCurrency(stats.completedSales)}
            </div>
            <p className="text-xs text-gray-500 mt-1">
              {mySales.filter((s) => s.status === SaleStatus.Done).length}{" "}
              entregas confirmadas
            </p>
          </CardContent>
        </Card>
        <Card className="shadow-none">
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle className="text-sm font-medium text-gray-600">
              Aguardando Entrega
            </CardTitle>
            <Clock className="h-4 w-4 text-amber-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-amber-600">
              {formatCurrency(stats.pendingDelivery)}
            </div>
            <p className="text-xs text-gray-500 mt-1">
              {
                mySales.filter((s) => s.status === SaleStatus.AwaitingDelivery)
                  .length
              }{" "}
              vendas pendentes
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Lista de Vendas */}
      <section>
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-2xl font-semibold text-gray-900">
            Histórico de Vendas
          </h2>
          {mySales.length > 0 && (
            <span className="text-sm text-gray-500">
              {mySales.length} {mySales.length === 1 ? "venda" : "vendas"}
            </span>
          )}
        </div>

        {isLoading && (
          <div className="flex items-center justify-center p-12 bg-gray-50 rounded-lg">
            <Loader2 className="h-8 w-8 animate-spin text-blue-600" />
            <span className="ml-2 text-gray-600">Carregando vendas...</span>
          </div>
        )}

        {isSalesError && (
          <Alert variant="destructive" className="mb-4">
            <AlertTriangle className="h-4 w-4" />
            <AlertDescription>
              Erro ao carregar vendas:{" "}
              {salesError instanceof Error
                ? salesError.message
                : "Erro desconhecido"}
            </AlertDescription>
          </Alert>
        )}

        {!isLoading && !isSalesError && mySales.length === 0 && (
          <NoItemsFound
            Icon={Banknote}
            description="Suas vendas aparecerão aqui quando você vender produtos"
            title="Nenhuma venda encontrada"
          />
        )}

        {!isLoading && !isSalesError && mySales.length > 0 && (
          <div className="space-y-4">
            {mySales
              .sort(
                (a, b) =>
                  new Date(b.createdAt).getTime() -
                  new Date(a.createdAt).getTime()
              )
              .map((sale) => (
                <WithdrawalItem key={sale.id} sale={sale} />
              ))}
          </div>
        )}
      </section>

      {/* Dialog de Confirmação de Saque */}
      <AlertDialog
        open={showWithdrawDialog}
        onOpenChange={setShowWithdrawDialog}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle className="flex items-center gap-2">
              <Banknote className="h-5 w-5 text-emerald-700" />
              Confirmar Solicitação de Saque
            </AlertDialogTitle>
            <AlertDialogDescription asChild>
              <div className="space-y-4 pt-4">
                <div className="bg-emerald-50 dark:bg-emerald-950 p-4 rounded-lg border border-emerald-200 dark:border-emerald-800">
                  <p className="text-sm text-emerald-900 dark:text-emerald-100 font-medium mb-2">
                    Valor a ser sacado:
                  </p>
                  <p className="text-3xl font-bold text-emerald-700 dark:text-emerald-400">
                    {formatCurrency(cashBalance || 0)}
                  </p>
                </div>

                <div className="space-y-2 text-sm text-gray-600">
                  <p className="flex items-start gap-2">
                    <CheckCircle className="h-4 w-4 text-emerald-600 mt-0.5 flex-shrink-0" />
                    <span>
                      O valor será transferido para sua conta bancária
                      cadastrada
                    </span>
                  </p>
                  <p className="flex items-start gap-2">
                    <AlertTriangle className="h-4 w-4 text-amber-600 mt-0.5 flex-shrink-0" />
                    <span>
                      Certifique-se de que seus dados bancários estão
                      atualizados
                    </span>
                  </p>
                </div>

                <p className="text-xs text-gray-500 pt-2">
                  Ao confirmar, você está solicitando o saque de todos os
                  valores disponíveis em sua conta.
                </p>
              </div>
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={withdrawAll.isPending}>
              Cancelar
            </AlertDialogCancel>
            <AlertDialogAction
              onClick={handleWithdrawAll}
              disabled={withdrawAll.isPending}
              className="bg-emerald-700 hover:bg-emerald-800"
            >
              {withdrawAll.isPending ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Processando...
                </>
              ) : (
                <>
                  <ArrowUpRight className="mr-2 h-4 w-4" />
                  Confirmar Saque
                </>
              )}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </main>
  );
};

export default WithdrawalsTab;
