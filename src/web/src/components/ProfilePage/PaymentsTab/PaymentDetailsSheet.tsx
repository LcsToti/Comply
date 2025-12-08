import { useState } from "react";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Skeleton } from "@/components/ui/skeleton";
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
  Package,
  Truck,
  CheckCircle,
  XCircle,
  AlertTriangle,
  Copy,
  Check,
  Clock,
  Ban,
  RefreshCw,
  Loader2,
} from "lucide-react";
import { useAuth } from "@/contexts/AuthContext";
import { DeliveryStatus, SaleStatus } from "@/types/sale";
import { useSaleQuery } from "@/hooks/sales/useSalesQueries";
import { usePaymentQuery } from "@/hooks/payments/usePaymentsQueries";
import { useProductQuery } from "@/hooks/products/useProductsQueries";
import { useDeliveryCodeQuery } from "@/hooks/sales/useDeliveriesQueries";
import { useCancelSaleMutation } from "@/hooks/sales/useSalesMutations";
import { useOpenDisputeMutation } from "@/hooks/sales/useDisputesMutations";
import {
  useMarkAsDeliveredMutation,
  useMarkAsShippedMutation,
} from "@/hooks/sales/useDeliveriesMutations";

interface PaymentDetailsSheetProps {
  saleId: string | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}
export function PaymentDetailsSheet({
  saleId,
  open,
  onOpenChange,
}: PaymentDetailsSheetProps) {
  const [deliveryCode, setDeliveryCode] = useState("");
  const [disputeReason, setDisputeReason] = useState("");
  const [copiedCode, setCopiedCode] = useState(false);
  const [showCancelDialog, setShowCancelDialog] = useState(false);
  const [showDisputeDialog, setShowDisputeDialog] = useState(false);
  const { user } = useAuth();

  const { data: sale, isLoading: isSaleLoading } = useSaleQuery(
    saleId || "",
    !!saleId
  );
  const { data: payment, isLoading: isPaymentLoading } = usePaymentQuery(
    sale?.paymentId || "",
    !!sale?.paymentId
  );
  const { data: product, isLoading: isProductLoading } = useProductQuery(
    sale?.productId || ""
  );

  const isSeller = product?.sellerId === user?.id;
  const isBuyer = payment?.payerId === user?.id;

  const { data: codeData } = useDeliveryCodeQuery(
    saleId || "",
    !!saleId && isBuyer && sale?.deliveryStatus === DeliveryStatus.Shipped
  );

  const cancelSale = useCancelSaleMutation();
  const markAsShipped = useMarkAsShippedMutation();
  const markAsDelivered = useMarkAsDeliveredMutation();
  const openDispute = useOpenDisputeMutation();

  const isLoading = isSaleLoading || isPaymentLoading || isProductLoading;

  if (!sale || isLoading) {
    return (
      <Sheet open={open} onOpenChange={onOpenChange}>
        <SheetContent className="overflow-y-auto">
          <SheetHeader>
            <SheetTitle>Carregando detalhes...</SheetTitle>
          </SheetHeader>
          <div className="space-y-4 py-6">
            <Skeleton className="h-8 w-32" />
            <Separator />
            <Skeleton className="h-24 w-full" />
            <Separator />
            <Skeleton className="h-32 w-full" />
          </div>
        </SheetContent>
      </Sheet>
    );
  }

  const getSaleStatusBadge = (status: number) => {
    const statusMap: Record<
      number,
      { label: string; variant: any; icon: any }
    > = {
      [SaleStatus.AwaitingDelivery]: {
        label: "Aguardando Entrega",
        variant: "default",
        icon: Clock,
      },
      [SaleStatus.Done]: {
        label: "Entregue",
        variant: "default",
        icon: CheckCircle,
      },
      [SaleStatus.Dispute]: {
        label: "Em Disputa",
        variant: "warning",
        icon: AlertTriangle,
      },
      [SaleStatus.Cancelled]: {
        label: "Cancelada",
        variant: "destructive",
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
        label: "Aguardando Envio",
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
  const getPaymentStatusBadge = (status: string) => {
    const statusMap: Record<
      string,
      { label: string; variant: any; icon: any }
    > = {
      Pending: { label: "Pendente", variant: "secondary", icon: Clock },
      Succeeded: { label: "Sucesso", variant: "default", icon: CheckCircle },
      Failed: { label: "Falhou", variant: "destructive", icon: XCircle },
      Canceled: { label: "Cancelado", variant: "destructive", icon: Ban },
      Refunded: { label: "Reembolsado", variant: "secondary", icon: RefreshCw },
      PartiallyRefunded: {
        label: "Parcialmente Reembolsado",
        variant: "secondary",
        icon: RefreshCw,
      },
    };

    const statusInfo = statusMap[status] || statusMap.Pending;
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
  const getWithdrawalStatusLabel = (status: string) => {
    const statusMap: Record<string, string> = {
      WaitingApproval: "Aguardando Aprovação",
      ApprovedToWithdraw: "Aprovado para Saque",
      Withdrawing: "Processando Saque",
      AlreadyWithdrawn: "Já Sacado",
      Failed: "Falhou",
    };

    return statusMap[status] || status;
  };
  const copyToClipboard = (text: string) => {
    navigator.clipboard.writeText(text);
    setCopiedCode(true);
    setTimeout(() => setCopiedCode(false), 2000);
  };
  const handleCancelSale = () => {
    if (saleId) {
      cancelSale.mutate(saleId);
      setShowCancelDialog(false);
      onOpenChange(false);
    }
  };
  const handleMarkAsShipped = () => {
    if (saleId) {
      markAsShipped.mutate(saleId);
    }
  };
  const handleMarkAsDelivered = () => {
    if (saleId && deliveryCode) {
      markAsDelivered.mutate({ saleId, code: deliveryCode });
    }
  };
  const handleOpenDispute = () => {
    if (saleId && disputeReason) {
      openDispute.mutate({ saleId, reason: disputeReason });
      setShowDisputeDialog(false);
      setDisputeReason("");
    }
  };

  return (
    <>
      <Sheet open={open} onOpenChange={onOpenChange}>
        <SheetContent className="p-4 overflow-y-auto">
          <SheetHeader>
            <div className="flex items-center justify-between">
              <SheetTitle>
                Detalhes da {isSeller ? "Venda" : "Compra"}
              </SheetTitle>
              <Badge
                variant="outline"
                className={
                  isSeller
                    ? "bg-blue-50 text-blue-700 border-blue-200"
                    : "bg-emerald-50 text-emerald-700 border-emerald-200"
                }
              >
                {isSeller ? "Você Vendeu" : "Você Comprou"}
              </Badge>
            </div>
            <SheetDescription>
              {isSeller
                ? "Gerencie a entrega e acompanhe o pagamento"
                : "Acompanhe a entrega e confirme o recebimento"}
            </SheetDescription>
          </SheetHeader>

          <div className="space-y-6 py-6">
            {/* Status */}
            <div className="flex flex-row gap-5 flex-wrap">
              {/* Status da Venda */}
              <div className="space-y-2">
                <h3 className="text-sm font-semibold text-muted-foreground">
                  Status da Venda
                </h3>
                {getSaleStatusBadge(sale.status)}
              </div>

              <Separator
                orientation="vertical"
                className="h-full self-stretch w-[1px] bg-border hidden sm:block"
              />

              {/* Status de Entrega */}
              <div className="space-y-2">
                <h3 className="text-sm font-semibold text-muted-foreground">
                  Status de Entrega
                </h3>
                {getDeliveryStatusBadge(sale.deliveryStatus)}
              </div>
            </div>
            <Separator />
            {/* Informações do Produto */}
            {product && (
              <>
                <div className="space-y-3">
                  <h3 className="text-sm font-semibold text-muted-foreground">
                    Produto
                  </h3>
                  <div className="flex items-start gap-4">
                    {product.images && product.images.length > 0 && (
                      <img
                        src={product.images[0]}
                        alt={product.title}
                        className="w-20 h-20 object-cover rounded-lg border"
                      />
                    )}
                    <div className="flex-1">
                      <p className="font-medium">{product.title}</p>
                      <p className="text-sm text-muted-foreground line-clamp-2">
                        {product.description}
                      </p>
                      {product.listing && (
                        <p className="text-sm text-emerald-700 font-semibold mt-1">
                          R$ {product.listing.buyPrice.toFixed(2)}
                        </p>
                      )}
                    </div>
                  </div>
                </div>
                <Separator />
              </>
            )}

            {/* Informações Financeiras - Para Vendedor */}
            {isSeller && payment && (
              <>
                <div className="space-y-3">
                  <h3 className="text-sm font-semibold text-muted-foreground">
                    Informações de Pagamento
                  </h3>
                  <div className="bg-blue-50 dark:bg-blue-950 p-4 rounded-lg border border-blue-200 dark:border-blue-800">
                    <div className="grid grid-cols-2 gap-3 text-sm">
                      <div>
                        <p className="text-muted-foreground">Valor da Venda</p>
                        <p className="font-medium">
                          {payment.amount.currency}{" "}
                          {payment.amount.total.toFixed(2)}
                        </p>
                      </div>
                      <div>
                        <p className="text-muted-foreground">
                          Taxa da Plataforma
                        </p>
                        <p className="font-medium text-red-600">
                          - {payment.amount.currency}{" "}
                          {payment.amount.fee.toFixed(2)}
                        </p>
                      </div>
                      <div className="col-span-2">
                        <p className="text-muted-foreground">Você receberá</p>
                        <p className="font-semibold text-2xl text-blue-700">
                          {payment.amount.currency}{" "}
                          {payment.amount.net.toFixed(2)}
                        </p>
                        <p className="text-xs text-muted-foreground mt-1">
                          Status do saque:{" "}
                          {getWithdrawalStatusLabel(payment.withdrawalStatus)}
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
                <Separator />
              </>
            )}

            {/* Informações Financeiras - Para Comprador */}
            {isBuyer && payment && (
              <>
                <div className="space-y-3">
                  <h3 className="text-sm font-semibold text-muted-foreground">
                    Valores
                  </h3>
                  <div className="grid grid-cols-2 gap-3 text-sm">
                    <div>
                      <p className="text-muted-foreground">Valor Pago</p>
                      <p className="font-semibold text-lg text-emerald-700">
                        {payment.amount.currency}{" "}
                        {payment.amount.total.toFixed(2)}
                      </p>
                    </div>
                    <div>
                      <p className="text-muted-foreground">
                        Status do Pagamento
                      </p>
                      {getPaymentStatusBadge(payment.paymentStatus)}
                    </div>
                  </div>
                </div>
                <Separator />
              </>
            )}

            {/* Código de Entrega - Para COMPRADOR (VISUALIZAR) */}
            {isBuyer &&
              codeData &&
              sale.deliveryStatus === DeliveryStatus.Shipped && (
                <>
                  <div className="space-y-3">
                    <h3 className="text-sm font-semibold text-muted-foreground">
                      Seu Código de Entrega
                    </h3>
                    <div className="bg-emerald-50 dark:bg-emerald-950 p-4 rounded-lg border border-emerald-200 dark:border-emerald-800 space-y-3">
                      <div className="flex items-center gap-2">
                        <Input
                          value={codeData}
                          readOnly
                          className="font-mono text-center text-2xl font-bold tracking-wider"
                        />
                        <Button
                          variant="outline"
                          size="icon"
                          onClick={() => copyToClipboard(codeData)}
                        >
                          {copiedCode ? (
                            <Check className="h-4 w-4" />
                          ) : (
                            <Copy className="h-4 w-4" />
                          )}
                        </Button>
                      </div>
                      <div className="bg-emerald-100 dark:bg-emerald-900 p-3 rounded-md">
                        <p className="text-sm font-medium text-emerald-900 dark:text-emerald-100 mb-1">
                          🔐 Guarde este código
                        </p>
                        <p className="text-xs text-emerald-800 dark:text-emerald-200">
                          Este é o seu código de confirmação.{" "}
                          <strong>
                            Forneça este código ao vendedor somente após receber
                            o produto
                          </strong>
                          . O vendedor precisará deste código para confirmar a
                          entrega e liberar o pagamento.
                        </p>
                      </div>
                    </div>
                  </div>
                  <Separator />
                </>
              )}

            {/* Informações da Disputa */}
            {sale.dispute && (
              <>
                <div className="space-y-3">
                  <h3 className="text-sm font-semibold text-muted-foreground">
                    Disputa
                  </h3>
                  <div className="bg-yellow-50 dark:bg-yellow-950 p-4 rounded-lg space-y-2 border border-yellow-200 dark:border-yellow-800">
                    <p className="text-sm">
                      <span className="font-medium">Motivo:</span>{" "}
                      {sale.dispute.reason}
                    </p>
                    {sale.dispute.resolution && (
                      <p className="text-sm">
                        <span className="font-medium">Resolução:</span>{" "}
                        {sale.dispute.resolution}
                      </p>
                    )}
                    {sale.dispute.resolutionStatus && (
                      <Badge variant="outline" className="mt-2">
                        {sale.dispute.resolutionStatus}
                      </Badge>
                    )}
                    <p className="text-xs text-muted-foreground">
                      Aberta em{" "}
                      {format(new Date(sale.dispute.createdAt), "PPp", {
                        locale: ptBR,
                      })}
                    </p>
                  </div>
                </div>
                <Separator />
              </>
            )}

            {/* Botões de Ação */}
            <div className="space-y-3">
              {/* Botões para Vendedor */}
              {isSeller && (
                <>
                  {/* Permite marcar como enviado se: venda está aguardando entrega E entrega está pendente E pagamento foi bem-sucedido */}
                  {sale.status === SaleStatus.AwaitingDelivery &&
                    sale.deliveryStatus === DeliveryStatus.Pending &&
                    payment?.paymentStatus === "Succeeded" && (
                      <Button
                        className="w-full bg-blue-600 hover:bg-blue-700"
                        onClick={handleMarkAsShipped}
                        disabled={markAsShipped.isPending}
                      >
                        {markAsShipped.isPending ? (
                          <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                        ) : (
                          <Truck className="mr-2 h-4 w-4" />
                        )}
                        {markAsShipped.isPending
                          ? "Marcando..."
                          : "Marcar como Enviado"}
                      </Button>
                    )}

                  {/* VENDEDOR CONFIRMA ENTREGA - Insere o código que o comprador forneceu */}
                  {sale.deliveryStatus === DeliveryStatus.Shipped && (
                    <div className="space-y-3">
                      <div className="bg-blue-50 dark:bg-blue-950 p-4 rounded-lg border border-blue-200 dark:border-blue-800">
                        <h4 className="text-sm font-semibold text-blue-900 dark:text-blue-100 mb-2 flex items-center gap-2">
                          <Package className="h-4 w-4" />
                          Confirmar Entrega do Produto
                        </h4>
                        <p className="text-xs text-blue-800 dark:text-blue-200 mb-3">
                          Para confirmar a entrega e liberar o pagamento para
                          saque, solicite o código de confirmação ao comprador e
                          insira abaixo.
                        </p>

                        <Label
                          htmlFor="delivery-code"
                          className="text-sm font-medium"
                        >
                          Código de Confirmação do Comprador
                        </Label>
                        <div className="flex gap-2 mt-2">
                          <Input
                            id="delivery-code"
                            placeholder="Digite o código fornecido"
                            value={deliveryCode}
                            onChange={(e) =>
                              setDeliveryCode(e.target.value.toUpperCase())
                            }
                            className="font-mono text-center text-lg"
                            maxLength={6}
                          />
                          <Button
                            onClick={handleMarkAsDelivered}
                            disabled={
                              !deliveryCode || markAsDelivered.isPending
                            }
                            className="bg-blue-600 hover:bg-blue-700"
                          >
                            {markAsDelivered.isPending ? (
                              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                            ) : (
                              <CheckCircle className="mr-2 h-4 w-4" />
                            )}
                            {markAsDelivered.isPending
                              ? "Confirmando..."
                              : "Confirmar"}
                          </Button>
                        </div>

                        <div className="mt-3 p-2 bg-blue-100 dark:bg-blue-900 rounded text-xs text-blue-900 dark:text-blue-100">
                          💡 <strong>Dica:</strong> Após confirmar a entrega com
                          o código correto, o pagamento será liberado para
                          saque.
                        </div>
                      </div>
                    </div>
                  )}
                </>
              )}

              {/* Botões para Comprador */}
              {isBuyer && (
                <>
                  {/* Comprador só vê o código acima, não precisa de ação aqui */}
                  {sale.deliveryStatus === DeliveryStatus.Shipped && (
                    <div className="bg-amber-50 dark:bg-amber-950 p-3 rounded-lg border border-amber-200 dark:border-amber-800">
                      <p className="text-sm text-amber-900 dark:text-amber-100 flex items-center gap-2">
                        <AlertTriangle className="h-4 w-4" />
                        <span>
                          <strong>Aguardando confirmação:</strong> Forneça o
                          código acima ao vendedor após receber o produto.
                        </span>
                      </p>
                    </div>
                  )}

                  {/* Abrir disputa: apenas se não estiver cancelada e não houver disputa */}
                  {sale.status != SaleStatus.Done && (
                    <Button
                      variant="outline"
                      className="w-full"
                      onClick={() => setShowDisputeDialog(true)}
                    >
                      <AlertTriangle className="mr-2 h-4 w-4" />
                      Abrir Disputa
                    </Button>
                  )}
                </>
              )}
            </div>

            <Separator />

            {/* Datas */}
            <div className="space-y-3">
              <h3 className="text-sm font-semibold text-muted-foreground">
                Histórico
              </h3>
              <div className="text-sm space-y-1">
                <p>
                  <span className="text-muted-foreground">
                    Venda criada em:
                  </span>{" "}
                  {format(new Date(sale.createdAt), "PPp", { locale: ptBR })}
                </p>
                {payment && (
                  <>
                    {payment.timestamps.processedAt && (
                      <p>
                        <span className="text-muted-foreground">
                          Pagamento processado:
                        </span>{" "}
                        {format(
                          new Date(payment.timestamps.processedAt),
                          "PPp",
                          {
                            locale: ptBR,
                          }
                        )}
                      </p>
                    )}
                    {payment.timestamps.withdrawnAt && (
                      <p>
                        <span className="text-muted-foreground">
                          Saque realizado:
                        </span>{" "}
                        {format(
                          new Date(payment.timestamps.withdrawnAt),
                          "PPp",
                          {
                            locale: ptBR,
                          }
                        )}
                      </p>
                    )}
                  </>
                )}
              </div>
            </div>
          </div>
        </SheetContent>
      </Sheet>

      {/* Dialog para Cancelar Venda */}
      <AlertDialog open={showCancelDialog} onOpenChange={setShowCancelDialog}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Cancelar Venda</AlertDialogTitle>
            <AlertDialogDescription>
              Tem certeza que deseja cancelar esta venda? Esta ação não pode ser
              desfeita e o comprador será notificado.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Voltar</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleCancelSale}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              Sim, Cancelar
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Dialog para Abrir Disputa */}
      <AlertDialog open={showDisputeDialog} onOpenChange={setShowDisputeDialog}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Abrir Disputa</AlertDialogTitle>
            <AlertDialogDescription>
              Descreva o motivo da disputa. Um administrador irá analisar o caso
              em até 48 horas.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <div className="py-4">
            <Textarea
              placeholder="Descreva o problema detalhadamente..."
              value={disputeReason}
              onChange={(e) => setDisputeReason(e.target.value)}
              rows={5}
            />
          </div>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleOpenDispute}
              disabled={!disputeReason.trim() || openDispute.isPending}
            >
              {openDispute.isPending ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Abrindo...
                </>
              ) : (
                "Abrir Disputa"
              )}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
