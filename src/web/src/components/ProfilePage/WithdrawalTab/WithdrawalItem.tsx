import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import {
  Clock,
  CheckCircle,
  AlertTriangle,
  XCircle,
  Package,
  Truck,
  Loader2,
  ArrowUpRight,
} from "lucide-react";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import { SaleStatus, DeliveryStatus, type Sale } from "@/types/sale";
import { toast } from "sonner";
import { usePaymentQuery } from "@/hooks/payments/usePaymentsQueries";
import { useProductQuery } from "@/hooks/products/useProductsQueries";
import { useWithdrawPaymentMutation } from "@/hooks/payments/usePaymentsMutations";

interface WithdrawalItemProps {
  sale: Sale;
}

const WithdrawalItem = ({ sale }: WithdrawalItemProps) => {
  const { data: payment, isLoading: isPaymentLoading } = usePaymentQuery(
    sale.paymentId
  );
  const { data: product, isLoading: isProductLoading } = useProductQuery(
    sale.productId
  );

  const withdrawPayment = useWithdrawPaymentMutation();

  const isLoading = isPaymentLoading || isProductLoading;

  const handleWithdrawPayment = () => {
    if (!payment) return;

    withdrawPayment.mutate(payment.paymentId, {
      onSuccess: () => {
        toast.success("Saque solicitado com sucesso!", {
          description: `O valor de R$ ${payment.amount.net.toFixed(2)} será processado em até 2 dias úteis.`,
          duration: 5000,
        });
      },
      onError: (error: any) => {
        console.error("Erro ao sacar pagamento:", error);

        let errorMessage = "Erro ao solicitar saque. Tente novamente.";
        let errorDescription = "";

        if (error?.response?.data) {
          const errorData = error.response.data;
          console.log(errorData);

          // Trata o erro específico de "no cash-out payments available"
          if (errorData.status === 422 && errorData.detail) {
            if (
              errorData.detail.includes(
                "There are no cash-out payments available"
              ) ||
              errorData.detail.includes("no cash-out payments")
            ) {
              errorMessage = "Pagamento não disponível para saque";
              errorDescription =
                "Este pagamento não está aprovado para saque. Verifique se a entrega foi confirmada e se o período de disputa expirou.";
            } else if (errorData.detail.includes("Invalid operation")) {
              errorMessage = "Operação inválida";
              errorDescription =
                errorData.detail ||
                "Não é possível realizar este saque no momento.";
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
      },
    });
  };

  const getSaleStatusBadge = (status: number) => {
    const statusMap: Record<
      number,
      { label: string; variant: any; icon: any }
    > = {
      [SaleStatus.AwaitingDelivery]: {
        label: "Aguardando Entrega",
        variant: "secondary",
        icon: Clock,
      },
      [SaleStatus.Done]: {
        label: "Concluída",
        variant: "default",
        icon: CheckCircle,
      },
      [SaleStatus.Dispute]: {
        label: "Em Disputa",
        variant: "destructive",
        icon: AlertTriangle,
      },
      [SaleStatus.Cancelled]: {
        label: "Cancelada",
        variant: "outline",
        icon: XCircle,
      },
    };

    const statusInfo = statusMap[status];
    const Icon = statusInfo.icon;

    return (
      <Badge
        variant={statusInfo.variant}
        className="flex items-center gap-1 w-fit"
      >
        <Icon className="h-3 w-3" />
        {statusInfo.label}
      </Badge>
    );
  };
  const getDeliveryStatusBadge = (status: number) => {
    const statusMap: Record<
      number,
      { label: string; variant: any; icon: any }
    > = {
      [DeliveryStatus.Pending]: {
        label: "Pendente",
        variant: "secondary",
        icon: Package,
      },
      [DeliveryStatus.Shipped]: {
        label: "Enviado",
        variant: "default",
        icon: Truck,
      },
      [DeliveryStatus.Delivered]: {
        label: "Entregue",
        variant: "default",
        icon: CheckCircle,
      },
    };

    const statusInfo = statusMap[status];
    const Icon = statusInfo.icon;

    return (
      <Badge
        variant={statusInfo.variant}
        className="flex items-center gap-1 w-fit"
      >
        <Icon className="h-3 w-3" />
        {statusInfo.label}
      </Badge>
    );
  };
  const getWithdrawalStatusBadge = (status: string) => {
    const statusMap: Record<
      string,
      { label: string; variant: any; icon: any; color: string }
    > = {
      WaitingApproval: {
        label: "Aguardando Aprovação",
        variant: "secondary",
        icon: Clock,
        color: "text-amber-600",
      },
      ApprovedToWithdraw: {
        label: "Aprovado para Saque",
        variant: "default",
        icon: CheckCircle,
        color: "text-emerald-600",
      },
      Withdrawing: {
        label: "Processando Saque",
        variant: "default",
        icon: Loader2,
        color: "text-blue-600",
      },
      AlreadyWithdrawn: {
        label: "Já Sacado",
        variant: "outline",
        icon: CheckCircle,
        color: "text-gray-600",
      },
      Failed: {
        label: "Falhou",
        variant: "destructive",
        icon: XCircle,
        color: "text-red-600",
      },
    };

    const statusInfo = statusMap[status] || statusMap.WaitingApproval;
    const Icon = statusInfo.icon;

    return (
      <div className="flex items-center gap-2">
        <Icon className={`h-4 w-4 ${statusInfo.color}`} />
        <span className={`text-sm font-medium ${statusInfo.color}`}>
          {statusInfo.label}
        </span>
      </div>
    );
  };

  if (isLoading) {
    return (
      <Card>
        <CardContent className="p-4">
          <div className="flex items-start gap-4">
            <Skeleton className="h-16 w-16 rounded-lg" />
            <div className="flex-1 space-y-2">
              <Skeleton className="h-4 w-3/4" />
              <Skeleton className="h-4 w-1/2" />
              <Skeleton className="h-4 w-2/3" />
            </div>
          </div>
        </CardContent>
      </Card>
    );
  }

  const canWithdraw =
    payment &&
    (payment.withdrawalStatus === "ApprovedToWithdraw" ||
      payment?.withdrawalStatus === "Failed") &&
    sale.status === SaleStatus.Done;

  return (
    <Card className="shadow-none">
      <CardContent className="p-3">
        <div className="flex items-start gap-4">
          {/* Imagem do Produto */}
          {product?.images && product.images.length > 0 && (
            <img
              src={product.images[0]}
              alt={product.title}
              className="h-16 w-16 rounded-lg object-cover border"
            />
          )}

          {/* Informações */}
          <div className="flex-1 min-w-0">
            <div className="flex items-start justify-between gap-2 mb-2">
              <div className="flex-1">
                <h3 className="font-medium text-gray-900 line-clamp-1">
                  {product?.title || `Venda #${sale.listingId.slice(0, 8)}`}
                </h3>
                <p className="text-xs text-gray-500">
                  {format(new Date(sale.createdAt), "PPp", { locale: ptBR })}
                </p>
              </div>
              {getSaleStatusBadge(sale.status)}
            </div>

            <div className="flex items-center gap-2 mb-3">
              {getDeliveryStatusBadge(sale.deliveryStatus)}
              {sale.dispute && (
                <Badge
                  variant="destructive"
                  className="flex items-center gap-1"
                >
                  <AlertTriangle className="h-3 w-3" />
                  Disputa
                </Badge>
              )}
            </div>

            {/* Informações Financeiras */}
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm mb-3">
              <div>
                <p className="text-gray-500 text-xs">Valor da Venda</p>
                <p className="font-semibold">
                  R$ {sale.productValue.toFixed(2)}
                </p>
              </div>

              {payment && (
                <>
                  <div>
                    <p className="text-gray-500 text-xs">Taxa</p>
                    <p className="font-semibold text-red-600">
                      -R$ {payment.amount.fee.toFixed(2)}
                    </p>
                  </div>
                  <div>
                    <p className="text-gray-500 text-xs">Você Recebe</p>
                    <p className="font-semibold text-emerald-700">
                      R$ {payment.amount.net.toFixed(2)}
                    </p>
                  </div>
                  <div>
                    <p className="text-gray-500 text-xs mb-1">
                      Status do Saque
                    </p>
                    {getWithdrawalStatusBadge(payment.withdrawalStatus)}
                  </div>
                </>
              )}
            </div>

            {/* Botão de Saque Individual */}
            {canWithdraw && (
              <Button
                size="sm"
                variant="outline"
                className="border-emerald-700 text-emerald-700 hover:bg-emerald-50"
                onClick={handleWithdrawPayment}
                disabled={withdrawPayment.isPending}
              >
                {withdrawPayment.isPending ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Processando...
                  </>
                ) : (
                  <>
                    <ArrowUpRight className="mr-2 h-4 w-4" />
                    Sacar Este Pagamento
                  </>
                )}
              </Button>
            )}

            {payment?.timestamps.withdrawnAt && (
              <p className="text-xs text-emerald-700 font-medium mt-1">
                ✓ Sacado em{" "}
                {format(new Date(payment.timestamps.withdrawnAt), "PPp", {
                  locale: ptBR,
                })}
              </p>
            )}
          </div>
        </div>
      </CardContent>
    </Card>
  );
};

export default WithdrawalItem;
