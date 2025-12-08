import Header from "@/components/Header";
import {
  Breadcrumb,
  BreadcrumbList,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import { Button } from "@/components/ui/button";
import {
  Share2,
  MapPinned,
  CircleCheck,
  Send,
  Bomb,
  Wrench,
  Repeat,
  AlertCircle,
  ShieldCheck,
  PackageX,
  CircleHelp,
} from "lucide-react";
import { useState } from "react";
import { Link, useNavigate, useParams } from "react-router";
import type { Product } from "@/types/product";
import { Label } from "@/components/ui/label";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Input } from "@/components/ui/input";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import ImageGallery from "@/components/ProductDetails/ImageGallery";
import PriceCard from "@/components/ProductDetails/PricingDetails";
import { Badge } from "@/components/ui/badge";
import ShareProduct from "@/components/Actions/ShareProduct";
import ToggleWatchList from "@/components/Actions/ToggleWatchList";
import { toast } from "sonner";
import { useAuth } from "@/contexts/AuthContext";
import Footer from "@/components/Footer";
import SessionLoading from "@/components/Loading/SessionLoading";
import { ListingStatus } from "@/types/listing";
import { AuctionChecker } from "@/utils/checkers/AuctionStatsChecker";
import { ListingChecker } from "@/utils/checkers/ListingStatsChecker";
import { useProductQuery } from "@/hooks/products/useProductsQueries";
import { useAddQuestionMutation } from "@/hooks/products/useProductsQnaMutations";

export default function ProductDetails() {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuth();
  const navigate = useNavigate();
  const { data: productData, isLoading, isError } = useProductQuery(id!);
  const [openShareModal, setOpenShareModal] = useState(false);
  const [questionText, setQuestionText] = useState("");
  const addQuestionMutation = useAddQuestionMutation();

  if (isLoading) {
    return <SessionLoading />;
  }

  if (isError || !productData) {
    return (
      <div className="flex flex-col items-center justify-center h-screen px-4">
        <PackageX className="h-16 w-16 md:h-20 md:w-20 text-red-500 mb-4" />
        <p className="text-red-500 text-sm md:text-base text-center">
          Produto não encontrado.
        </p>
        <Button onClick={() => navigate("/search")} className="mt-4">
          Voltar à busca
        </Button>
      </div>
    );
  }

  const product = productData as Product;
  const answeredQuestions = product.qna.questions.filter((q) => q.answer);

  const isOwner = user?.id === product.sellerId;
  const isSold = ListingChecker.isSold(product);
  const isPaused = ListingChecker.isPaused(product);
  const isAuctionCancelled = AuctionChecker.isCancelled(product);
  const isAuctionFailed = AuctionChecker.isFailed(product);

  const isProductUnavailable =
    isSold || isPaused || isAuctionCancelled || isAuctionFailed;
  const canInteract = !isOwner && !isProductUnavailable;

  const handleAddQuestion = () => {
    if (!user) {
      toast.error("Você precisa estar logado para fazer perguntas.");
      return;
    }

    if (isOwner) {
      toast.error("Você não pode fazer perguntas no seu próprio produto.");
      return;
    }

    if (!questionText.trim()) {
      toast.error("Digite uma pergunta válida.");
      return;
    }

    addQuestionMutation.mutate(
      {
        productId: product.id,
        params: { QuestionText: questionText },
      },
      {
        onSuccess: () => {
          toast.success("Sua pergunta foi enviada ao vendedor.", {
            description: "Notificaremos quando houver resposta.",
          });
          setQuestionText("");
        },
        onError: () => {
          toast.error("Não foi possível enviar sua pergunta. Tente novamente.");
        },
      }
    );
  };

  const characteristics = product?.characteristics
    ? Object.entries(product.characteristics).map(([key, value]) => ({
        key,
        value,
      }))
    : [];

  const renderProductStatusBadge = () => {
    if (isOwner) {
      return (
        <div className="flex items-start gap-3 md:p-4 bg-blue-50 border border-blue-200 rounded-lg mb-4 md:mb-6">
          <ShieldCheck className="h-5 w-5 md:h-6 md:w-6 text-blue-600 flex-shrink-0 mt-0.5" />
          <div className="flex flex-col min-w-0">
            <span className="text-sm md:text-base font-semibold text-blue-800">
              Você é o proprietário deste produto
            </span>
            <Link
              to={"/dashboard/MyListings"}
              className="text-xs md:text-sm text-blue-600"
            >
              Gerenciar em Meus Anúncios
            </Link>
          </div>
        </div>
      );
    }

    if (isSold) {
      return (
        <div className="flex items-start gap-3 p-3 md:p-4 bg-gray-50 border border-gray-200 rounded-lg mb-4 md:mb-6">
          <PackageX className="h-5 w-5 md:h-6 md:w-6 text-gray-600 flex-shrink-0 mt-0.5" />
          <div className="flex flex-col min-w-0">
            <span className="text-sm md:text-base font-semibold text-gray-800">
              Este produto foi vendido
            </span>
            <span className="text-xs md:text-sm text-gray-600">
              {product.listing.status === ListingStatus.SoldByAuction
                ? "Leilão encerrado com sucesso"
                : "Comprado por outro usuário"}
            </span>
          </div>
        </div>
      );
    }

    if (isPaused) {
      return (
        <div className="flex items-start gap-3 p-3 md:p-4 bg-amber-50 border border-amber-200 rounded-lg mb-4 md:mb-6">
          <AlertCircle className="h-5 w-5 md:h-6 md:w-6 text-amber-600 flex-shrink-0 mt-0.5" />
          <div className="flex flex-col min-w-0">
            <span className="text-sm md:text-base font-semibold text-amber-800">
              Este produto está pausado
            </span>
            <span className="text-xs md:text-sm text-amber-600">
              O vendedor pausou temporariamente este anúncio
            </span>
          </div>
        </div>
      );
    }

    if (isAuctionCancelled) {
      return (
        <div className="flex items-start gap-3 p-3 md:p-4 bg-red-50 border border-red-200 rounded-lg mb-4 md:mb-6">
          <AlertCircle className="h-5 w-5 md:h-6 md:w-6 text-red-600 flex-shrink-0 mt-0.5" />
          <div className="flex flex-col min-w-0">
            <span className="text-sm md:text-base font-semibold text-red-800">
              Leilão cancelado
            </span>
            <span className="text-xs md:text-sm text-red-600">
              Este leilão foi cancelado pelo vendedor
            </span>
          </div>
        </div>
      );
    }

    if (isAuctionFailed) {
      return (
        <div className="flex items-start gap-3 p-3 md:p-4 bg-red-50 border border-red-200 rounded-lg mb-4 md:mb-6">
          <AlertCircle className="h-5 w-5 md:h-6 md:w-6 text-red-600 flex-shrink-0 mt-0.5" />
          <div className="flex flex-col min-w-0">
            <span className="text-sm md:text-base font-semibold text-red-800">
              Leilão encerrado
            </span>
            <span className="text-xs md:text-sm text-red-600">
              Este leilão encerrou sem lances vencedores
            </span>
          </div>
        </div>
      );
    }

    return null;
  };

  return (
    <div className="min-h-screen flex flex-col">
      <Header />

      <main className="flex-1 w-full">
        <div className="container mx-auto px-4 sm:px-6 lg:px-8 py-4 md:py-6 lg:py-8 max-w-8xl">
          {/* Breadcrumb e ações */}
          <div className="flex flex-col sm:flex-row sm:justify-between sm:items-center gap-4 mb-6 md:mb-8">
            <Breadcrumb className="order-1">
              <BreadcrumbList>
                <BreadcrumbItem>
                  <BreadcrumbLink asChild>
                    <Link to="/" className="text-xs">
                      Home
                    </Link>
                  </BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbSeparator />
                <BreadcrumbItem>
                  <BreadcrumbLink asChild>
                    <Link to="/search" className="text-xs">
                      Buscar
                    </Link>
                  </BreadcrumbLink>
                </BreadcrumbItem>
              </BreadcrumbList>
            </Breadcrumb>

            <div className="flex items-center gap-2 order-2">
              {user && (
                <ToggleWatchList
                  listingId={product.listing.id}
                  productId={product.id}
                />
              )}
              <Button
                variant="outline"
                size="sm"
                onClick={() => setOpenShareModal((p) => !p)}
                className="flex-shrink-0 cursor-pointer"
              >
                <Share2 className="h-4 w-4 md:h-5 md:w-5 text-gray-500" />
                <span className="hidden sm:inline ml-2 text-sm">
                  Compartilhar
                </span>
              </Button>

              <Button
                variant="outline"
                size="sm"
                onClick={() => navigate("/faq")}
                className="flex-shrink-0 cursor-pointer"
              >
                <CircleHelp className="h-4 w-4 md:h-5 md:w-5 text-gray-500" />
                <span className="hidden lg:inline ml-2 text-sm">FAQ</span>
              </Button>

              <ShareProduct
                open={openShareModal}
                onOpenChange={setOpenShareModal}
              />
            </div>
          </div>

          {/* Badge de status do produto */}
          {renderProductStatusBadge()}

          {/* Grid principal */}
          <div className="grid grid-cols-1 xl:grid-cols-3 gap-6 lg:gap-8 xl:gap-10">
            {/* Coluna esquerda - Imagens e detalhes */}
            <div className="xl:col-span-2 space-y-6 md:space-y-8">
              <ImageGallery images={product.images ?? []} />

              {/* Card de preços - Mobile */}
              <div className="xl:hidden">
                <PriceCard
                  auctionId={product.listing.auction?.id}
                  existsAuction={!!product.listing.auction}
                  auctionStartDate={product.listing.auction?.settings.startDate}
                  startBidValue={
                    product.listing.auction?.settings.startBidValue
                  }
                  currentBid={
                    product.listing?.auction?.bids?.find(
                      (b) => b.status === "Winning"
                    )?.value
                  }
                  buyNowPrice={product.listing?.buyPrice}
                  watchers={product.watchListCount}
                  bidCount={product.listing?.auction?.bids?.length}
                  auctionEndDate={product.listing?.auction?.settings.endDate}
                  disabled={!canInteract}
                  isOwner={isOwner}
                />
              </div>

              {/* Badge de condição */}
              <div className="flex flex-wrap gap-2">
                {product.condition === 0 ? (
                  <Badge
                    variant="outline"
                    className="text-sm md:text-base text-emerald-800 px-3 py-1"
                  >
                    <CircleCheck className="h-4 w-4 md:h-5 md:w-5 mr-1" />
                    Novo
                  </Badge>
                ) : product.condition === 1 ? (
                  <Badge
                    variant="outline"
                    className="text-sm md:text-base px-3 py-1"
                  >
                    <Repeat className="h-4 w-4 md:h-5 md:w-5 mr-1" />
                    Usado
                  </Badge>
                ) : product.condition === 2 ? (
                  <Badge
                    variant="outline"
                    className="text-sm md:text-base text-red-700 px-3 py-1"
                  >
                    <Bomb className="h-4 w-4 md:h-5 md:w-5 mr-1" />
                    Não funciona
                  </Badge>
                ) : (
                  <Badge
                    variant="outline"
                    className="text-sm md:text-base px-3 py-1"
                  >
                    <Wrench className="h-4 w-4 md:h-5 md:w-5 mr-1" />
                    Recondicionado
                  </Badge>
                )}
              </div>

              {/* Título */}
              <h1 className="text-2xl md:text-3xl lg:text-4xl font-bold text-gray-800 leading-tight">
                {product.title}
              </h1>

              {/* Descrição */}
              <div className="prose prose-sm md:prose-base max-w-none">
                <p className="text-gray-600 leading-relaxed whitespace-pre-wrap">
                  {product.description}
                </p>
              </div>

              {/* Características */}
              {characteristics.length > 0 && (
                <div className="space-y-4">
                  <div className="flex flex-col sm:flex-row sm:justify-between sm:items-center gap-2">
                    <Label className="text-xl md:text-2xl font-semibold text-gray-700">
                      Características
                    </Label>
                    {characteristics.length > 6 && (
                      <Popover>
                        <PopoverTrigger asChild>
                          <Button
                            variant="link"
                            className="p-0 h-auto justify-start font-medium text-sm md:text-base"
                          >
                            Ver mais +{characteristics.length - 6}
                          </Button>
                        </PopoverTrigger>
                        <PopoverContent className="w-[90vw] sm:w-96 max-h-60 overflow-y-auto">
                          <div className="space-y-3">
                            <h4 className="font-bold text-sm md:text-base">
                              Mais características
                            </h4>
                            {characteristics.slice(6).map((char) => (
                              <div
                                key={char.key}
                                className="flex items-start gap-2 text-sm"
                              >
                                <CircleCheck className="h-4 w-4 flex-shrink-0 mt-0.5" />
                                <span className="font-light">{char.key}:</span>
                                <span className="font-medium break-words">
                                  {char.value || "N/A"}
                                </span>
                              </div>
                            ))}
                          </div>
                        </PopoverContent>
                      </Popover>
                    )}
                  </div>

                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 md:gap-4 bg-gray-50 rounded-lg p-3 md:p-4">
                    {characteristics.slice(0, 6).map((char) => (
                      <div
                        key={char.key}
                        className="flex items-start gap-2 text-sm md:text-base"
                      >
                        <CircleCheck className="h-4 w-4 md:h-5 md:w-5 flex-shrink-0 mt-0.5" />
                        <div className="min-w-0 flex-1">
                          <span className="font-light">{char.key}:</span>{" "}
                          <span className="font-medium break-words">
                            {char.value || "N/A"}
                          </span>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              )}

              {/* Localidade */}
              <div className="space-y-3">
                <h2 className="text-xl md:text-2xl font-semibold text-gray-700">
                  Localidade
                </h2>
                <p className="text-gray-600 flex items-center gap-2 text-sm md:text-base">
                  <MapPinned className="h-4 w-4 md:h-5 md:w-5" />
                  {product.locale}
                </p>
              </div>

              {/* Dúvidas */}
              <div className="space-y-4">
                <h2 className="text-xl md:text-2xl font-semibold text-gray-700">
                  Perguntas
                </h2>

                {isOwner ? (
                  <div className="flex items-start gap-2 p-3 md:p-4 bg-blue-50 border border-blue-200 rounded-lg">
                    <AlertCircle className="h-5 w-5 text-blue-600 flex-shrink-0 mt-0.5" />
                    <span className="text-sm md:text-base text-blue-800">
                      Você não pode fazer perguntas no seu próprio produto
                    </span>
                  </div>
                ) : isProductUnavailable ? (
                  <div className="flex items-start gap-2 p-3 md:p-4 bg-gray-50 border border-gray-200 rounded-lg">
                    <AlertCircle className="h-5 w-5 text-gray-600 flex-shrink-0 mt-0.5" />
                    <span className="text-sm md:text-base text-gray-800">
                      Este produto não está mais disponível para perguntas
                    </span>
                  </div>
                ) : (
                  <div className="flex flex-col sm:flex-row gap-2 md:gap-3">
                    <Input
                      className="flex-1 h-10 md:h-12 text-sm md:text-base"
                      value={questionText}
                      onChange={(e) => setQuestionText(e.target.value)}
                      placeholder={
                        product.qna.totalQuestions > 0
                          ? "Escreva aqui sua dúvida..."
                          : "Seja o primeiro a perguntar..."
                      }
                      disabled={!user}
                      maxLength={500}
                    />
                    <Button
                      className="h-10 md:h-12 bg-emerald-600 hover:bg-emerald-700 cursor-pointer px-4 md:px-6 text-sm md:text-base whitespace-nowrap"
                      onClick={handleAddQuestion}
                      disabled={
                        !user ||
                        addQuestionMutation.isPending ||
                        !questionText.trim()
                      }
                    >
                      <Send className="h-4 w-4 md:h-5 md:w-5 mr-2" />
                      {addQuestionMutation.isPending
                        ? "Enviando..."
                        : "Perguntar"}
                    </Button>
                  </div>
                )}

                {answeredQuestions.length > 0 && (
                  <Dialog>
                    <DialogTrigger asChild>
                      <Button
                        variant="outline"
                        className="border-emerald-600 text-emerald-800 hover:bg-emerald-50 text-sm md:text-base cursor-pointer w-full sm:w-auto"
                      >
                        Ver {answeredQuestions.length}{" "}
                        {answeredQuestions.length === 1
                          ? "pergunta"
                          : "perguntas"}
                      </Button>
                    </DialogTrigger>
                    <DialogContent className="max-w-2xl max-h-[80vh] overflow-y-auto">
                      <DialogHeader>
                        <DialogTitle className="text-lg md:text-xl">
                          Perguntas e Respostas
                        </DialogTitle>
                      </DialogHeader>
                      <div className="space-y-4 mt-4">
                        {answeredQuestions.map((q) => (
                          <div
                            key={q.questionId}
                            className="border-l-4 border-emerald-500 pl-4 py-2"
                          >
                            <p className="font-medium text-gray-900 text-sm md:text-base mb-2">
                              {q.questionText}
                            </p>
                            {q.answer && (
                              <div className="ml-2 space-y-1">
                                <p className="text-gray-700 text-sm md:text-base">
                                  {q.answer.answerText}
                                </p>
                                <p className="text-xs md:text-sm text-gray-500">
                                  Respondido em{" "}
                                  {new Date(
                                    q.answer.answeredAt
                                  ).toLocaleDateString("pt-BR")}
                                </p>
                              </div>
                            )}
                          </div>
                        ))}
                      </div>
                    </DialogContent>
                  </Dialog>
                )}
              </div>
            </div>

            {/* Coluna direita - Card de preços (Desktop) */}
            <div className="xl:col-span-1 hidden xl:block">
              <div className="sticky top-20">
                <PriceCard
                  auctionId={product.listing.auction?.id}
                  existsAuction={!!product.listing.auction}
                  auctionStartDate={product.listing.auction?.settings.startDate}
                  startBidValue={
                    product.listing.auction?.settings.startBidValue
                  }
                  currentBid={
                    product.listing?.auction?.bids?.find(
                      (b) => b.status === "Winning"
                    )?.value
                  }
                  buyNowPrice={product.listing?.buyPrice}
                  watchers={product.watchListCount}
                  bidCount={product.listing?.auction?.bids?.length}
                  auctionEndDate={product.listing?.auction?.settings.endDate}
                  disabled={!canInteract}
                  isOwner={isOwner}
                />
              </div>
            </div>
          </div>
        </div>
      </main>

      <Footer />
    </div>
  );
}
