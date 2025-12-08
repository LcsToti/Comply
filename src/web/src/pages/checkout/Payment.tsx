import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Bomb,
  CheckCircle2,
  CircleCheck,
  CreditCard,
  Repeat,
  ShoppingBag,
  Wrench,
  Plus,
  AlertCircle,
  Loader2,
  Gavel,
} from "lucide-react";
import Header from "@/components/Header";
import {
  AlertDialog,
  AlertDialogTrigger,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogFooter,
  AlertDialogTitle,
  AlertDialogDescription,
  AlertDialogCancel,
  AlertDialogAction,
} from "@/components/ui/alert-dialog";
import Footer from "@/components/Footer";
import VisaIcon from "@/assets/creditcards/visa.svg";
import MastercardIcon from "@/assets/creditcards/mastercard.svg";
import PixIcon from "@/assets/creditcards/pix.svg";
import AmexIcon from "@/assets/creditcards/amex.svg";
import DiscoverIcon from "@/assets/creditcards/discover.svg";
import PaypalIcon from "@/assets/creditcards/paypal.svg";
import GenericCard from "@/assets/creditcards/generic.svg";
import { Link, useNavigate, useParams } from "react-router";
import PaymentCompletion from "@/components/PaymentCompletion";
import {
  DialogHeader,
  Dialog,
  DialogTrigger,
  DialogContent,
  DialogTitle,
  DialogDescription,
} from "@/components/ui/dialog";
import CreatePaymentMethod from "@/components/CreatePaymentMethod";
import StripeLogo from "@/assets/stripe.svg";
import { formatCurrency } from "@/utils/formatters/formatCurrency";
import {
  Breadcrumb,
  BreadcrumbList,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { toast } from "sonner";
import { AuctionChecker } from "@/utils/checkers/AuctionStatsChecker";
import { useProductQuery } from "@/hooks/products/useProductsQueries";
import { useBuyNowMutation } from "@/hooks/listings/useListingsMutations";
import { usePlaceBidMutation } from "@/hooks/listings/useAuctionsMutations";
import { usePaymentMethodsQuery } from "@/hooks/payments/usePaymentAccountsQueries";
import type { PaymentMethod } from "@/types/paymentAccount";

export default function Payment() {
  const [selectedMethod, setSelectedMethod] = useState<string | null>(null);
  const [purchaseCompleted, setPurchaseCompleted] = useState(false);
  const [openPaymentDialog, setOpenPaymentDialog] = useState(false);

  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();

  const { data: productData, isLoading: loadingProduct } = useProductQuery(id!);
  const { mutate: buyNow, isPending: purchasingProduct } = useBuyNowMutation();
  const { mutate: placeBid, isPending: placingBid } = usePlaceBidMutation();
  const {
    data: paymentMethodsData,
    isLoading: loadingPaymentMethods,
    refetch: refetchPaymentMethods,
  } = usePaymentMethodsQuery();

  const brandIcons: Record<string, string> = {
    visa: VisaIcon,
    americanexpress: AmexIcon,
    discover: DiscoverIcon,
    mastercard: MastercardIcon,
    pix: PixIcon,
    paypal: PaypalIcon,
    default: GenericCard,
  };

  const sendToProductsBought = () => navigate("/dashboard/MyWatchList");

  const isAuctionActive = AuctionChecker.isActive(productData!);

  const isWinBidPurchase =
    isAuctionActive && productData?.listing?.auction?.settings?.winBidValue;

  const purchaseValue = isWinBidPurchase
    ? productData?.listing?.auction?.settings?.winBidValue
    : productData?.listing?.buyPrice;

  const getIcon = (method: PaymentMethod) => {
    if (method.type === "pix") return brandIcons.pix;
    if (method.type === "paypal") return brandIcons.paypal;
    if (method.type && brandIcons[method.brand.toLowerCase()])
      return brandIcons[method.brand.toLowerCase()];
    return brandIcons.default;
  };

  const handlePurchase = () => {
    if (!id || !selectedMethod) {
      toast.error("Selecione um método de pagamento");
      return;
    }

    if (!productData) {
      toast.error("Produto não encontrado");
      return;
    }

    // Determina qual ação executar
    if (isWinBidPurchase) {
      // Compra através de lance de vitória (win bid)
      if (!productData.listing?.auction?.id) {
        toast.error("Leilão não encontrado");
        return;
      }

      if (!productData.listing.auction.settings.winBidValue) {
        toast.error("Valor de compra imediata não definido");
        return;
      }

      placeBid(
        {
          auctionId: productData.listing.auction.id,
          params: {
            Value: productData.listing.auction.settings.winBidValue,
            PaymentMethodId: selectedMethod,
          },
        },
        {
          onSuccess: () => {
            toast.success("Compra realizada com sucesso!");
            setTimeout(() => setPurchaseCompleted(true), 800);
          },
          onError: (error: any) => {
            console.error("Erro no lance de vitória:", error);
            handleError(error);
          },
        }
      );
    } else {
      // Compra direta
      if (!productData.listing?.id) {
        toast.error("Produto não encontrado");
        return;
      }

      buyNow(
        {
          listingId: productData.listing.id,
          paymentMethodId: selectedMethod,
        },
        {
          onSuccess: () => {
            toast.success("Compra realizada com sucesso!");
            setTimeout(() => setPurchaseCompleted(true), 800);
          },
          onError: (error: any) => {
            console.error("Erro na compra:", error);
            handleError(error);
          },
        }
      );
    }
  };

  const handleError = (error: any) => {
    let errorMessage = "Falha ao realizar compra. Tente novamente.";

    if (error?.response?.data) {
      const errorData = error.response.data;

      if (typeof errorData === "string") {
        errorMessage = errorData;
      } else if (errorData.message) {
        errorMessage = errorData.message;
      } else if (errorData.error) {
        errorMessage = errorData.error;
      } else if (errorData.errors && Array.isArray(errorData.errors)) {
        errorMessage = errorData.errors
          .map((err: any) => err.message || err)
          .join(", ");
      } else if (errorData.title && errorData.detail) {
        errorMessage = `${errorData.title}: ${errorData.detail}`;
      }
    } else if (error?.message) {
      errorMessage = error.message;
    }

    toast.error(errorMessage, {
      duration: 5000,
    });
  };

  const handlePaymentMethodCreated = () => {
    refetchPaymentMethods();
    setOpenPaymentDialog(false);
    toast.success("Método de pagamento adicionado!");
  };

  const renderConditionBadge = () => {
    switch (productData?.condition) {
      case 0:
        return (
          <Badge
            variant="outline"
            className="text-xs self-start text-emerald-800"
          >
            <CircleCheck size={20} />
            Novo
          </Badge>
        );
      case 1:
        return (
          <Badge variant="outline" className="text-xs self-start">
            <Repeat size={20} />
            Usado
          </Badge>
        );
      case 2:
        return (
          <Badge variant="outline" className="text-xs self-start text-red-700">
            <Bomb size={20} />
            Não funciona
          </Badge>
        );
      case 3:
        return (
          <Badge variant="outline" className="text-xs self-start">
            <Wrench size={20} />
            Recondicionado
          </Badge>
        );
      default:
        return null;
    }
  };

  const isProcessing = purchasingProduct || placingBid;

  if (loadingProduct) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="flex flex-col items-center gap-3">
          <Loader2 className="h-8 w-8 animate-spin text-emerald-700" />
          <p className="text-gray-600">Carregando produto...</p>
        </div>
      </div>
    );
  }

  if (!productData) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="flex flex-col items-center gap-3">
          <AlertCircle className="h-8 w-8 text-red-600" />
          <p className="text-red-600">Produto não encontrado</p>
          <Button onClick={() => navigate("/")}>Voltar ao início</Button>
        </div>
      </div>
    );
  }

  if (purchaseCompleted) {
    return <PaymentCompletion onComplete={sendToProductsBought} />;
  }

  return (
    <div className="min-h-screen justify-between flex flex-col bg-gray-50">
      <Header isCheckoutPage />

      <main className="flex-1 my-5 mx-4 md:my-10 md:mx-10 lg:my-12 lg:mx-20 xl:mx-40 flex flex-col items-start gap-10">
        <Breadcrumb>
          <BreadcrumbList>
            <BreadcrumbItem>
              <BreadcrumbLink asChild>
                <Link to="/">Home</Link>
              </BreadcrumbLink>
            </BreadcrumbItem>
            <BreadcrumbSeparator />
            <BreadcrumbItem>
              <BreadcrumbLink asChild>
                <Link to="/search">Search</Link>
              </BreadcrumbLink>
            </BreadcrumbItem>
            <BreadcrumbSeparator />
            <BreadcrumbItem>
              <BreadcrumbLink asChild>
                <Link to={`/product/${id}`}>{productData.title}</Link>
              </BreadcrumbLink>
            </BreadcrumbItem>
          </BreadcrumbList>
        </Breadcrumb>

        {/* Badge informativo sobre tipo de compra */}
        {isWinBidPurchase && (
          <div className="w-full p-4 bg-amber-50 border border-amber-200 rounded-lg flex items-center gap-3">
            <Gavel className="h-5 w-5 text-amber-700" />
            <div className="flex-1">
              <p className="text-sm font-semibold text-amber-900">
                Compra Imediata via Leilão
              </p>
              <p className="text-xs text-amber-700">
                Ao confirmar, você ganhará o leilão automaticamente com o valor
                de compra imediata
              </p>
            </div>
          </div>
        )}

        <div className="flex flex-col lg:flex-row justify-center items-start gap-5 w-full">
          <div className="flex flex-col flex-1 w-full max-w-200 border border-gray-200 rounded-lg p-6 bg-white shadow-sm">
            <div className="flex justify-between items-center mb-6">
              <div className="flex items-center gap-2">
                <CreditCard className="text-gray-600" size={22} />
                <h2 className="text-lg font-semibold text-gray-800">
                  Formas de Pagamento
                </h2>
              </div>
              <div className="flex items-center gap-2">
                <span className="text-sm text-gray-500">Powered by</span>
                <img src={StripeLogo} alt="Stripe" className="h-5" />
              </div>
            </div>

            <div className="flex flex-col gap-2 mb-6">
              {loadingPaymentMethods ? (
                <div className="flex items-center justify-center p-8">
                  <Loader2 className="h-6 w-6 animate-spin text-emerald-700" />
                  <span className="ml-2 text-gray-600">Carregando...</span>
                </div>
              ) : !paymentMethodsData || paymentMethodsData.length === 0 ? (
                <div className="flex flex-col items-center gap-4 p-8 bg-gray-50 border-2 border-dashed border-gray-300 rounded-lg">
                  <CreditCard className="h-12 w-12 text-gray-400" />
                  <div className="text-center">
                    <p className="text-gray-700 font-medium mb-1">
                      Nenhum método de pagamento
                    </p>
                    <p className="text-gray-500 text-sm">
                      Adicione um cartão ou PIX para continuar
                    </p>
                  </div>
                </div>
              ) : (
                paymentMethodsData.map((method: PaymentMethod) => (
                  <Card
                    key={method.id}
                    onClick={() => setSelectedMethod(method.id)}
                    className={`cursor-pointer transition-all border-2 shadow-none ${
                      selectedMethod === method.id
                        ? "border-emerald-600 bg-emerald-50"
                        : "border-gray-200 hover:border-emerald-500"
                    }`}
                  >
                    <CardContent className="flex items-center justify-between px-5 py-4 font-medium text-gray-700">
                      <div className="flex items-center gap-3">
                        <img
                          src={getIcon(method)}
                          alt={method.brand || method.type}
                          className="w-7 h-7 object-contain"
                        />
                        <span>
                          {method.type === "pix"
                            ? "PIX"
                            : `${method.brand?.toUpperCase()} •••• ${method.last4}`}
                        </span>
                      </div>
                      {selectedMethod === method.id && (
                        <CheckCircle2 className="text-emerald-600" size={22} />
                      )}
                    </CardContent>
                  </Card>
                ))
              )}
            </div>

            <Dialog
              open={openPaymentDialog}
              onOpenChange={setOpenPaymentDialog}
            >
              <DialogTrigger asChild>
                <Button
                  variant="outline"
                  className="border-emerald-600 text-emerald-600 hover:bg-emerald-50 cursor-pointer"
                >
                  <Plus size={18} className="mr-2" />
                  Adicionar nova forma de pagamento
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle className="mb-2">
                    Criar forma de pagamento
                  </DialogTitle>
                  <DialogDescription asChild>
                    <div>
                      <CreatePaymentMethod
                        onSuccess={handlePaymentMethodCreated}
                      />
                    </div>
                  </DialogDescription>
                </DialogHeader>
              </DialogContent>
            </Dialog>
          </div>

          {/* Resumo do Pedido */}
          <div className="border border-gray-200 rounded-lg p-6 bg-white shadow-sm flex flex-col gap-4 lg:w-[340px] w-full sticky top-24">
            <div className="flex flex-col items-center">
              <img
                src={productData.images[0]}
                alt={productData.title}
                className="rounded-lg mb-4 w-70 h-70 object-contain"
              />
              {renderConditionBadge()}
              <h3 className="text-md font-semibold text-gray-800 mt-2 text-center">
                {productData.title}
              </h3>
            </div>

            <div className="text-gray-600 text-sm">
              <div className="flex justify-between py-1">
                <span>
                  {isWinBidPurchase
                    ? "Valor de compra imediata"
                    : "Preço do produto"}
                </span>
                <span>
                  {purchaseValue ? formatCurrency(purchaseValue) : "R$ 0,00"}
                </span>
              </div>
              {isWinBidPurchase && (
                <p className="text-xs text-amber-700 mt-1">
                  Lance de vitória automático
                </p>
              )}
              <hr className="my-2" />
              <div className="flex justify-between items-center text-gray-800 text-base font-semibold">
                <span>Total</span>
                <span>
                  {purchaseValue ? formatCurrency(purchaseValue) : "R$ 0,00"}
                </span>
              </div>
            </div>

            <AlertDialog>
              <AlertDialogTrigger asChild>
                <Button
                  className="w-full bg-emerald-600 hover:bg-emerald-700 text-white font-semibold text-lg py-6 rounded-lg transition cursor-pointer"
                  disabled={!selectedMethod || isProcessing}
                >
                  {isProcessing ? (
                    <>
                      <Loader2 className="mr-2 h-5 w-5 animate-spin" />
                      Processando...
                    </>
                  ) : isWinBidPurchase ? (
                    <>
                      <Gavel size={20} className="mr-2" />
                      Comprar e Vencer
                    </>
                  ) : (
                    <>
                      <ShoppingBag size={20} className="mr-2" />
                      Comprar Agora
                    </>
                  )}
                </Button>
              </AlertDialogTrigger>
              <AlertDialogContent>
                <AlertDialogHeader>
                  <AlertDialogTitle>
                    {isWinBidPurchase
                      ? "Confirmar Compra Imediata"
                      : "Confirmar Compra"}
                  </AlertDialogTitle>
                  <AlertDialogDescription>
                    {isWinBidPurchase ? (
                      <>
                        Ao confirmar, você dará um lance de{" "}
                        <strong>
                          {purchaseValue
                            ? formatCurrency(purchaseValue)
                            : "R$ 0,00"}
                        </strong>{" "}
                        e <strong>vencerá automaticamente</strong> o leilão de{" "}
                        <strong>{productData.title}</strong>.
                      </>
                    ) : (
                      <>
                        Deseja finalizar a compra de{" "}
                        <strong>{productData.title}</strong> por{" "}
                        <strong>
                          {purchaseValue
                            ? formatCurrency(purchaseValue)
                            : "R$ 0,00"}
                        </strong>
                        ?
                      </>
                    )}
                  </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                  <AlertDialogCancel className="cursor-pointer">
                    Cancelar
                  </AlertDialogCancel>
                  <AlertDialogAction
                    className="cursor-pointer bg-emerald-600 hover:bg-emerald-700"
                    onClick={handlePurchase}
                  >
                    {isWinBidPurchase
                      ? "Confirmar e Vencer"
                      : "Confirmar Compra"}
                  </AlertDialogAction>
                </AlertDialogFooter>
              </AlertDialogContent>
            </AlertDialog>
          </div>
        </div>
      </main>

      <Footer />
    </div>
  );
}
