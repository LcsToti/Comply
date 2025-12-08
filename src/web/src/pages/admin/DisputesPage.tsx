import { useState } from "react";
import {
  AlertCircle,
  Clock,
  CheckCircle2,
  ShoppingBag,
  Calendar,
  ArrowLeft,
  Send,
  AlertTriangle,
  Package,
  CreditCard,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { ScrollArea } from "@/components/ui/scroll-area";
import { cn } from "@/lib/utils";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { formatCurrency } from "@/utils/formatters/formatCurrency";
import { Separator } from "@/components/ui/separator";
import { DisputeResolutionStatus } from "@/types/dispute";
import { saleStatusLabels, type Sale } from "@/types/sale";
import { useAllSalesQuery } from "@/hooks/sales/useSalesQueries";
import { useUserQuery } from "@/hooks/user/useUsersQueries";
import { usePaymentQuery } from "@/hooks/payments/usePaymentsQueries";
import { useProductQuery } from "@/hooks/products/useProductsQueries";
import {
  useAssignAdminToDisputeMutation,
  useCloseDisputeMutation,
} from "@/hooks/sales/useDisputesMutations";

export default function DisputesPage() {
  const [selectedSaleInDispute, setSelectedSaleInDispute] =
    useState<Sale | null>(null);
  const [showDetailOnMobile, setShowDetailOnMobile] = useState(false);
  const [resolveDialogOpen, setResolveDialogOpen] = useState(false);
  const [resolutionText, setResolutionText] = useState("");
  const [resolutionStatus, setResolutionStatus] =
    useState<DisputeResolutionStatus>(0);

  const {
    data: salesData,
    isLoading: isLoadingSales,
    isError: isErrorSales,
  } = useAllSalesQuery();

  const salesInDispute =
    salesData?.filter((sales: Sale) => sales.dispute != null) ?? [];

  const { data: buyerData } = useUserQuery(
    selectedSaleInDispute?.buyerId ?? "",
    !!selectedSaleInDispute?.buyerId
  );
  const { data: sellerData } = useUserQuery(
    selectedSaleInDispute?.sellerId ?? "",
    !!selectedSaleInDispute?.sellerId
  );
  const { data: adminData } = useUserQuery(
    selectedSaleInDispute?.dispute?.adminId ?? "",
    !!selectedSaleInDispute?.dispute?.adminId
  );
  const { data: paymentData } = usePaymentQuery(
    selectedSaleInDispute?.paymentId ?? "",
    !!selectedSaleInDispute?.paymentId
  );
  const { data: productData } = useProductQuery(
    selectedSaleInDispute?.productId ?? ""
  );
  const { mutate: assignAdminToDispute } = useAssignAdminToDisputeMutation();
  const { mutate: closeDispute } = useCloseDisputeMutation();

  const statusConfig = {
    0: {
      color: "bg-yellow-100 text-yellow-800",
      icon: AlertCircle,
      label: "Pendente",
    },
    1: {
      color: "bg-blue-100 text-blue-800",
      icon: Clock,
      label: "Em Análise",
    },
    2: {
      color: "bg-gray-100 text-gray-800",
      icon: CheckCircle2,
      label: "Fechada",
    },
  };
  const resolutionStatusConfig = {
    0: { color: "bg-green-100 text-green-800", label: "Resolvida" },
    1: { color: "bg-red-100 text-red-800", label: "Não Resolvida" },
    2: { color: "bg-blue-100 text-blue-800", label: "Reembolsada" },
    3: {
      color: "bg-purple-100 text-purple-800",
      label: "Saque Aprovado",
    },
    4: {
      color: "bg-indigo-100 text-indigo-800",
      label: "Resolvida pelo Admin",
    },
    5: { color: "bg-gray-100 text-gray-800", label: "Expirada" },
  };

  const handleDisputeClick = (saleInDispute: Sale) => {
    setSelectedSaleInDispute(saleInDispute);
    setShowDetailOnMobile(true);
  };
  const handleBackToList = () => {
    setShowDetailOnMobile(false);
    setSelectedSaleInDispute(null);
  };
  const handleAssignToMe = () => {
    if (!selectedSaleInDispute?.dispute) return;
    const saleId = selectedSaleInDispute.id;
    assignAdminToDispute(saleId);
  };
  const handleOpenResolveDialog = () => {
    setResolveDialogOpen(true);
  };
  const handleResolveDispute = () => {
    if (!selectedSaleInDispute?.dispute || !resolutionText.trim()) return;

    const saleId = selectedSaleInDispute.id;

    closeDispute({
      saleId,
      resolution: resolutionText,
      resolutionStatus,
    });

    setResolveDialogOpen(false);
    setResolutionText("");
  };

  const getTimeRemaining = (expiresAt: string | null) => {
    if (!expiresAt) return null;
    const now = new Date();
    const expiry = new Date(expiresAt);
    const diff = expiry.getTime() - now.getTime();
    const days = Math.floor(diff / (1000 * 60 * 60 * 24));

    if (days < 0) return "Expirada";
    if (days === 0) return "Expira hoje";
    if (days === 1) return "Expira amanhã";
    return `${days} dias restantes`;
  };

  if (isLoadingSales) {
    return (
      <div className="flex-1 flex items-center justify-center h-screen">
        <div className="text-center">
          <Clock className="w-12 h-12 mx-auto mb-4 animate-spin text-emerald-600" />
          <p className="text-sm text-gray-600">Carregando disputas...</p>
        </div>
      </div>
    );
  }

  if (isErrorSales || !salesData) {
    return (
      <div className="flex-1 flex items-center justify-center h-screen">
        <div className="text-center text-red-600">
          <AlertTriangle className="w-12 h-12 mx-auto mb-4" />
          <p className="text-sm">Erro ao carregar disputas. Tente novamente.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="flex flex-1 h-screen overflow-auto">
      {/* Back Button Mobile */}
      {showDetailOnMobile && (
        <Button
          variant="outline"
          size="icon"
          className="lg:hidden fixed top-20 left-4 z-50 shadow-lg bg-white"
          onClick={handleBackToList}
        >
          <ArrowLeft className="w-5 h-5" />
        </Button>
      )}

      {/* Disputes List */}
      <div
        className={cn(
          "w-full lg:w-96 border-r border-gray-200 bg-white flex flex-col flex-shrink-0",
          showDetailOnMobile && "hidden lg:flex"
        )}
      >
        {/* Disputes List */}
        <ScrollArea className="flex-1 overflow-auto">
          <div className="p-3">
            {salesInDispute.length === 0 ? (
              <div className="text-center py-8 text-gray-500">
                <AlertTriangle className="w-10 h-10 lg:w-12 lg:h-12 mx-auto mb-2 opacity-50" />
                <p className="text-sm lg:text-base">
                  Nenhuma disputa encontrada
                </p>
              </div>
            ) : (
              salesInDispute.map((saleInDispute: Sale) => {
                const StatusIcon =
                  statusConfig[saleInDispute.dispute.status].icon;
                const isSelected =
                  selectedSaleInDispute?.id === saleInDispute.id;
                const timeRemaining = getTimeRemaining(
                  saleInDispute.dispute.expiresAt
                );

                return (
                  <Card
                    key={saleInDispute.id}
                    className={cn(
                      "mb-2 cursor-pointer transition-all shadow-none hover:border-emerald-400",
                      isSelected && "border-emerald-600"
                    )}
                    onClick={() => handleDisputeClick(saleInDispute)}
                  >
                    <CardContent className="px-3 py-0">
                      <div className="flex items-start justify-between mb-2">
                        <div className="flex-1 min-w-0">
                          <div className="flex items-center gap-2 text-sm font-semibold text-gray-900 mb-1">
                            <ShoppingBag className="w-4 h-4 flex-shrink-0" />
                            <span>{saleInDispute.id}</span>
                          </div>{" "}
                          <div className="flex items-center gap-2 mb-1 flex-wrap">
                            <Badge
                              className={cn(
                                statusConfig[saleInDispute.dispute!.status]
                                  .color,
                                "text-xs"
                              )}
                            >
                              <StatusIcon className="w-3 h-3 mr-1" />
                              {
                                statusConfig[saleInDispute.dispute!.status]
                                  .label
                              }
                            </Badge>
                          </div>
                          <p className="text-xs text-gray-600 line-clamp-1 break-words">
                            {saleInDispute.dispute!.reason}
                          </p>
                        </div>
                      </div>

                      {saleInDispute.dispute!.status !== 2 && timeRemaining && (
                        <div className="mt-2 flex items-center gap-1 text-xs text-orange-600">
                          <Clock className="w-3 h-3" />
                          <span>{timeRemaining}</span>
                        </div>
                      )}

                      {saleInDispute.dispute!.resolutionStatus && (
                        <Badge
                          className={cn(
                            resolutionStatusConfig[
                              saleInDispute.dispute!.resolutionStatus
                            ].color,
                            "text-xs mt-2"
                          )}
                        >
                          {
                            resolutionStatusConfig[
                              saleInDispute.dispute!.resolutionStatus
                            ].label
                          }
                        </Badge>
                      )}
                    </CardContent>
                  </Card>
                );
              })
            )}
          </div>
        </ScrollArea>
      </div>

      {/* Dispute Details */}
      <div
        className={cn(
          "flex-1 flex flex-col min-w-0",
          !showDetailOnMobile && "hidden lg:flex"
        )}
      >
        {selectedSaleInDispute && selectedSaleInDispute.dispute ? (
          <>
            {/* Header */}
            <div className="bg-white border-b border-gray-200 p-3 sm:p-4 lg:p-6 flex-shrink-0">
              <div className="flex flex-col gap-4 lg:gap-6">
                {/* Primeira linha: Título e Info Principal */}
                <div className="flex flex-col lg:flex-row lg:items-start lg:justify-between gap-4">
                  <div className="flex-1 min-w-0">
                    {/* Título da Venda */}
                    <div className="flex items-center gap-2 mb-3">
                      <ShoppingBag className="w-4 h-4 sm:w-5 sm:h-5 text-gray-600 flex-shrink-0" />
                      <h2 className="text-base sm:text-lg lg:text-xl font-semibold text-gray-900">
                        Venda {selectedSaleInDispute.id}
                      </h2>
                      <div className="flex items-center gap-2 mb-2 flex-wrap">
                        <Badge
                          className={cn(
                            statusConfig[selectedSaleInDispute.dispute.status]
                              .color,
                            "text-xs whitespace-nowrap"
                          )}
                        >
                          {
                            statusConfig[selectedSaleInDispute.dispute.status]
                              .label
                          }
                        </Badge>
                        {selectedSaleInDispute.dispute.resolutionStatus && (
                          <Badge
                            className={cn(
                              resolutionStatusConfig[
                                selectedSaleInDispute.dispute.resolutionStatus
                              ].color,
                              "text-xs whitespace-nowrap"
                            )}
                          >
                            {
                              resolutionStatusConfig[
                                selectedSaleInDispute.dispute.resolutionStatus
                              ].label
                            }
                          </Badge>
                        )}
                      </div>
                    </div>

                    {/* Data e Tempo Restante */}
                    <div className="flex flex-col sm:flex-row sm:items-center gap-2 sm:gap-4 text-sm text-gray-600">
                      <div className="flex items-center gap-2">
                        <Calendar className="w-4 h-4 flex-shrink-0" />
                        <span className="text-xs lg:text-sm">
                          {new Date(
                            selectedSaleInDispute.dispute.createdAt
                          ).toLocaleString("pt-BR")}
                        </span>
                      </div>
                      {selectedSaleInDispute.dispute.status !== 2 &&
                        selectedSaleInDispute.dispute.expiresAt && (
                          <>
                            <span className="hidden sm:inline text-gray-400">
                              •
                            </span>
                            <div className="flex items-center gap-2 text-orange-600">
                              <Clock className="w-4 h-4 flex-shrink-0" />
                              <span className="text-xs lg:text-sm font-medium">
                                {getTimeRemaining(
                                  selectedSaleInDispute.dispute.expiresAt
                                )}
                              </span>
                            </div>
                          </>
                        )}
                    </div>
                  </div>
                </div>

                {/* Participantes - Responsivo */}
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3 sm:gap-4">
                  {/* Comprador */}
                  <div className="flex items-center gap-3 p-3 rounded-lg bg-blue-50/50 border border-blue-100">
                    <Avatar className="w-10 h-10 flex-shrink-0">
                      <AvatarFallback className="bg-blue-100 text-blue-700 text-xs">
                        {buyerData?.name
                          ?.split(" ")
                          .map((n) => n[0])
                          .join("") ?? "C"}
                      </AvatarFallback>
                    </Avatar>
                    <div className="flex items-center gap-2 flex-wrap">
                      <span className="font-semibold text-sm lg:text-base truncate">
                        {buyerData?.name ?? "Comprador"}
                      </span>
                      <Badge
                        variant="outline"
                        className="text-xs whitespace-nowrap"
                      >
                        Comprador
                      </Badge>
                    </div>
                  </div>

                  {/* Vendedor */}
                  <div className="flex items-center gap-3 p-3 rounded-lg bg-purple-50/50 border border-purple-100">
                    <Avatar className="w-10 h-10 flex-shrink-0">
                      <AvatarFallback className="bg-purple-100 text-purple-700 text-xs">
                        {sellerData?.name
                          ?.split(" ")
                          .map((n) => n[0])
                          .join("") ?? "V"}
                      </AvatarFallback>
                    </Avatar>
                    <div className="flex items-center gap-2 mb-1 flex-wrap">
                      <span className="font-semibold text-sm lg:text-base truncate">
                        {sellerData?.name ?? "Vendedor"}
                      </span>
                      <Badge
                        variant="outline"
                        className="text-xs whitespace-nowrap"
                      >
                        Vendedor
                      </Badge>
                    </div>
                  </div>

                  {/* Admin Responsável */}
                  {adminData && (
                    <div className="flex items-start gap-3 p-3 rounded-lg bg-emerald-50/50 border border-emerald-100 sm:col-span-2 lg:col-span-1">
                      <Avatar className="w-10 h-10 flex-shrink-0">
                        <AvatarFallback className="bg-emerald-600 text-white text-xs">
                          {adminData.name
                            ?.split(" ")
                            .map((n) => n[0])
                            .join("") ?? "A"}
                        </AvatarFallback>
                      </Avatar>
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2 mb-1 flex-wrap">
                          <span className="font-semibold text-sm lg:text-base truncate">
                            {adminData.name}
                          </span>
                          <Badge
                            variant="secondary"
                            className="text-xs whitespace-nowrap"
                          >
                            Admin Responsável
                          </Badge>
                        </div>
                        <p className="text-xs text-gray-500">
                          Assumiu em{" "}
                          {selectedSaleInDispute.dispute.updatedAt &&
                            new Date(
                              selectedSaleInDispute.dispute.updatedAt
                            ).toLocaleString("pt-BR")}
                        </p>
                      </div>
                    </div>
                  )}
                </div>

                {/* Motivo e Resolução */}
                <div className="flex flex-col gap-3">
                  {/* Motivo da Disputa */}
                  <div className="bg-gray-50 p-3 sm:p-4 rounded-lg border border-gray-200">
                    <p className="text-xs sm:text-sm font-medium text-gray-900 mb-2">
                      Motivo da disputa:
                    </p>
                    <p className="text-xs sm:text-sm text-gray-700 whitespace-pre-wrap break-words">
                      {selectedSaleInDispute.dispute.reason}
                    </p>
                  </div>

                  {/* Resolução */}
                  {selectedSaleInDispute.dispute.resolution && (
                    <div className="bg-emerald-50 p-3 sm:p-4 rounded-lg border border-emerald-200">
                      <p className="text-xs sm:text-sm font-medium text-emerald-900 mb-2">
                        Resolução:
                      </p>
                      <p className="text-xs sm:text-sm text-emerald-800 whitespace-pre-wrap break-words">
                        {selectedSaleInDispute.dispute.resolution}
                      </p>
                    </div>
                  )}
                </div>
              </div>
            </div>

            {/* Details */}
            <ScrollArea className="flex-1 overflow-auto p-3 lg:p-5">
              <div className="mx-auto flex flex-col gap-2">
                {/** Product Details */}
                <div className="p-2 flex flex-row gap-2">
                  <img
                    src={productData?.images[0]}
                    alt="Imagem do Produto"
                    className="w-15 h-15 rounded-2xl object-cover"
                  />
                  <div>
                    <h1 className="text-sm font-semibold">
                      {productData?.title}
                    </h1>
                    <div className="text-sm flex flex-row gap-2">
                      <Badge variant={"outline"}>
                        {productData?.listing.status}
                      </Badge>
                    </div>
                  </div>
                </div>

                {/** Payment Details */}
                <Card className="shadow-none">
                  <CardHeader>
                    <CardTitle className="text-base flex items-center gap-2">
                      <CreditCard className="w-5 h-5" />
                      Detalhes do Pagamento
                    </CardTitle>
                  </CardHeader>
                  <CardContent className="flex flex-col gap-2">
                    {/* Status Badges Section */}
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
                      <div className="flex flex-col gap-2 p-3 rounded-lg bg-gray-50">
                        <span className="text-xs font-medium text-gray-600">
                          Status do Pagamento
                        </span>
                        <Badge className="bg-green-100 text-green-800 w-fit">
                          <CheckCircle2 className="w-3 h-3 mr-1" />
                          {paymentData?.paymentStatus}
                        </Badge>
                      </div>
                      <div className="flex flex-col gap-2 p-3 rounded-lg bg-gray-50">
                        <span className="text-xs font-medium text-gray-600">
                          Status da Venda
                        </span>
                        <Badge className="bg-blue-100 text-blue-800 w-fit">
                          <Clock className="w-3 h-3 mr-1" />
                          {saleStatusLabels[selectedSaleInDispute.status]}
                        </Badge>
                      </div>

                      <div className="flex flex-col gap-2 p-3 rounded-lg bg-gray-50">
                        <span className="text-xs font-medium text-gray-600">
                          Status da Entrega
                        </span>
                        <Badge className="bg-orange-100 text-orange-800 w-fit">
                          <Package className="w-3 h-3 mr-1" />
                          {
                            saleStatusLabels[
                              selectedSaleInDispute.deliveryStatus
                            ]
                          }
                        </Badge>
                      </div>
                    </div>
                    <Separator />

                    {/* Financial Breakdown */}
                    <div className="space-y-4">
                      <h4 className="text-sm font-semibold text-gray-700">
                        Resumo Financeiro
                      </h4>

                      <div className="space-y-3">
                        {/* Valor Pago pelo Comprador */}
                        <div className="flex justify-between items-center p-3 rounded-lg bg-blue-50">
                          <div className="flex items-center gap-2">
                            <Avatar className="w-10 h-10">
                              <AvatarFallback className="bg-blue-100 text-blue-700 text-xs">
                                {buyerData?.name
                                  ?.split(" ")
                                  .map((n) => n[0])
                                  .join("") ?? "C"}
                              </AvatarFallback>
                            </Avatar>
                            <div>
                              <p className="text-xs text-gray-600">
                                Valor Pago
                              </p>
                              <p className="text-xs text-gray-500">
                                {buyerData?.name ?? "Comprador"}
                              </p>
                            </div>
                          </div>
                          <span className="text-lg font-bold text-blue-700">
                            {paymentData
                              ? formatCurrency(paymentData?.amount.total)
                              : ""}
                          </span>
                        </div>

                        {/* Taxas e Comissões */}
                        <div className="pl-4 space-y-2 border-l-2 border-gray-200">
                          <div className="flex justify-between text-sm">
                            <span className="text-gray-600">
                              Taxa da plataforma (8%)
                            </span>
                            <span className="text-gray-700 font-medium">
                              -{" "}
                              {paymentData
                                ? formatCurrency(paymentData?.amount.fee)
                                : ""}
                            </span>
                          </div>
                        </div>

                        {/* Valor a Receber pelo Vendedor */}
                        <div className="flex justify-between items-center p-3 rounded-lg bg-emerald-50 border border-emerald-100">
                          <div className="flex items-center gap-2">
                            <Avatar className="w-10 h-10">
                              <AvatarFallback className="bg-purple-100 text-purple-700 text-xs">
                                {sellerData?.name
                                  ?.split(" ")
                                  .map((n) => n[0])
                                  .join("") ?? "V"}
                              </AvatarFallback>
                            </Avatar>
                            <div>
                              <p className="text-xs text-gray-600">
                                Valor a Receber
                              </p>
                              <p className="text-xs text-gray-500">
                                {sellerData?.name ?? "Vendedor"}
                              </p>
                            </div>
                          </div>
                          <span className="text-lg font-bold text-emerald-700">
                            {paymentData
                              ? formatCurrency(paymentData?.amount.net)
                              : ""}
                          </span>
                        </div>
                      </div>
                    </div>

                    {/* Withdrawal Status */}
                    <div className="flex items-center justify-between p-4 rounded-lg bg-gray-50">
                      <div className="flex items-center gap-3">
                        <div>
                          <p className="text-sm font-semibold text-gray-900">
                            Status do Saque
                          </p>
                          <p className="text-xs text-gray-600">
                            Disponível após entrega confirmada
                          </p>
                        </div>
                      </div>
                      <Badge className="bg-yellow-100 text-yellow-800 border-yellow-200">
                        <Clock className="w-3 h-3 mr-1" />
                        {paymentData?.withdrawalStatus}
                      </Badge>
                    </div>

                    {/* Payment Info Footer */}
                    <div className="pt-3 border-t border-gray-200">
                      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-2 text-xs text-gray-500">
                        <div className="flex items-center gap-1">
                          <Calendar className="w-3 h-3" />
                          <span>
                            Pagamento realizado em{" "}
                            {new Date(
                              selectedSaleInDispute.createdAt
                            ).toLocaleString("pt-BR")}
                          </span>
                        </div>
                        <div className="flex items-center gap-1">
                          <span className="font-mono bg-gray-100 px-2 py-1 rounded">
                            ID: {paymentData?.paymentId}
                          </span>
                        </div>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              </div>
            </ScrollArea>

            {/** Resolve buttons */}
            <div className="bg-white border-t flex flex-row justify-end border-gray-200 p-3 flex-shrink-0">
              {selectedSaleInDispute.dispute.status === 0 && (
                <Button
                  onClick={handleAssignToMe}
                  className="bg-emerald-600 hover:bg-emerald-700 w-80 cursor-pointer text-sm"
                >
                  Assumir caso
                </Button>
              )}
              {selectedSaleInDispute.dispute.status === 1 && (
                <Button
                  onClick={handleOpenResolveDialog}
                  className="bg-blue-600 hover:bg-blue-700 w-80 cursor-pointer text-sm"
                >
                  Resolver Disputa
                </Button>
              )}
            </div>
          </>
        ) : (
          <div className="flex-1 flex items-center justify-center text-gray-500 p-4">
            <div className="text-center">
              <AlertTriangle className="w-12 h-12 lg:w-16 lg:h-16 mx-auto mb-4 opacity-50" />
              <p className="text-sm lg:text-lg">
                Selecione uma disputa para ver os detalhes
              </p>
            </div>
          </div>
        )}
      </div>

      {/* Resolve Dialog */}
      <Dialog open={resolveDialogOpen} onOpenChange={setResolveDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Resolver Disputa</DialogTitle>
            <DialogDescription>
              Forneça uma resolução detalhada para esta disputa. Esta ação é
              irreversível.
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <Label htmlFor="resolutionStatus">Status da Resolução</Label>
              <Select
                value={String(resolutionStatus)}
                onValueChange={(value) =>
                  setResolutionStatus(Number(value) as DisputeResolutionStatus)
                }
              >
                <SelectTrigger id="resolutionStatus">
                  <SelectValue />
                </SelectTrigger>

                <SelectContent>
                  <SelectItem value={String(DisputeResolutionStatus.Solved)}>
                    Resolvida
                  </SelectItem>
                  <SelectItem value={String(DisputeResolutionStatus.Unsolved)}>
                    Não Resolvida
                  </SelectItem>
                  <SelectItem value={String(DisputeResolutionStatus.Refunded)}>
                    Reembolsada
                  </SelectItem>
                  <SelectItem
                    value={String(DisputeResolutionStatus.ApprovedWithdrawal)}
                  >
                    Saque Aprovado
                  </SelectItem>
                  <SelectItem
                    value={String(DisputeResolutionStatus.ResolvedByAdmin)}
                  >
                    Resolvida pelo Admin
                  </SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2 max-w-md">
              <Label htmlFor="resolution">Descrição da Resolução</Label>
              <Textarea
                id="resolution"
                placeholder="Descreva detalhadamente como a disputa foi resolvida..."
                className="min-h-[150px] resize-y"
                value={resolutionText}
                onChange={(e) => setResolutionText(e.target.value)}
              />
            </div>
          </div>

          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setResolveDialogOpen(false)}
            >
              Cancelar
            </Button>
            <Button
              onClick={handleResolveDispute}
              disabled={!resolutionText.trim()}
              className="bg-blue-600 hover:bg-blue-700"
            >
              <Send className="w-4 h-4 mr-2" />
              Resolver Disputa
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
