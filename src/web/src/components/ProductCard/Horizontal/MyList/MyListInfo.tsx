// MyListInfo.tsx - REFATORADO COM UTILS
import { SquareArrowOutUpRight, Zap } from "lucide-react";
import { Link } from "react-router";
import type { CommonProductCardProps } from "../../ProductCard";
import { Badge } from "@/components/ui/badge";
import { formatCurrency } from "@/utils/formatters/formatCurrency";
import { Button } from "@/components/ui/button";
import { getCurrentBidPrice } from "@/utils/CurrentBidPrice";
import { getFinalSalePrice } from "@/utils/LastProductPrice";
import { ListingStatus } from "@/types/listing";
import { ListingChecker } from "@/utils/checkers/ListingStatsChecker";
import { AuctionChecker } from "@/utils/checkers/AuctionStatsChecker";
import { AuctionStatus, BidStatus } from "@/types/auction";
import complyImg from "@/assets/logo/comply-icon.png";

const MyListInfo: React.FC<
  Pick<
    CommonProductCardProps,
    | "product"
    | "formattedDates"
    | "auction"
    | "formattedPrices"
    | "saleType"
    | "userBidStatus"
    | "remainingTimeStart"
    | "remainingTimeEnd"
  >
> = ({
  product,
  formattedDates,
  auction,
  formattedPrices,
  saleType,
  userBidStatus,
  remainingTimeEnd,
  remainingTimeStart,
}) => {
  const { isWinning: isUserWinningBid, hasBid, userId } = userBidStatus;

  // Usa funções utilitárias para obter preços
  const currentBidPrice = getCurrentBidPrice(product);
  const finalSalePrice = getFinalSalePrice(product);

  // Verificações de status
  const isSold = ListingChecker.isSold(product);

  const auctionEnded =
    AuctionChecker.isFailed(product) ||
    AuctionChecker.isSuccess(product) ||
    AuctionChecker.isFinished(product);

  const auctionCancelled = AuctionChecker.isCancelled(product);

  // Verifica se usuário venceu o leilão
  const userWonAuction = auction?.bids?.some(
    (bid) => bid?.status === BidStatus.Winner && bid?.bidderId === userId
  );

  // Status badge logic
  const getStatusBadge = () => {
    if (isSold) {
      return (
        <Badge variant="default" className="bg-emerald-700">
          Vendido
        </Badge>
      );
    }

    if (auction?.status === AuctionStatus.Success && userWonAuction) {
      return (
        <Badge variant="default" className="bg-emerald-700">
          Você venceu
        </Badge>
      );
    }

    if (
      (auction?.status === AuctionStatus.Active ||
        auction?.status === AuctionStatus.Ending) &&
      isUserWinningBid
    ) {
      return (
        <Badge variant="default" className="bg-emerald-700">
          Vencendo
        </Badge>
      );
    }

    if (
      (auction?.status === AuctionStatus.Active ||
        auction?.status === AuctionStatus.Ending) &&
      hasBid &&
      !isUserWinningBid
    ) {
      return <Badge variant="destructive">Superado</Badge>;
    }

    if (auction?.status === AuctionStatus.Failed) {
      return (
        <Badge variant="outline" className="border-red-500 text-red-700">
          Falhou
        </Badge>
      );
    }

    if (auctionCancelled) {
      return (
        <Badge variant="outline" className="border-gray-500 text-gray-700">
          Cancelado
        </Badge>
      );
    }

    if (auction?.status === AuctionStatus.Ending) {
      return (
        <Badge variant="outline" className="border-red-500 text-red-700">
          Finalizando
        </Badge>
      );
    }

    if (auction?.status === AuctionStatus.Awaiting) {
      return (
        <Badge variant="outline" className="border-amber-500 text-amber-700">
          Aguardando
        </Badge>
      );
    }

    if (product.listing?.status === ListingStatus.Paused) {
      return (
        <Badge variant="outline" className="border-gray-500 text-gray-700">
          Pausado
        </Badge>
      );
    }

    return null;
  };

  // Determinar texto do preço
  const getPriceLabel = () => {
    if (isSold || auction?.status === AuctionStatus.Success) {
      return "Valor final";
    }
    if (auction?.status === AuctionStatus.Failed || auctionCancelled) {
      return "Último lance";
    }
    if (isUserWinningBid) {
      return "Seu lance";
    }
    if (hasBid) {
      return "Lance atual";
    }
    return "Lance inicial";
  };

  // Renderiza o preço apropriado
  const renderPrice = () => {
    // Produto vendido ou leilão encerrado
    if (isSold || auctionEnded || auctionCancelled) {
      const displayPrice = finalSalePrice ?? currentBidPrice;

      return (
        <>
          <span className="text-xs text-neutral-500">{getPriceLabel()}</span>
          <div className="flex items-baseline gap-2">
            <span className="font-bold text-neutral-900">
              {displayPrice
                ? formatCurrency(displayPrice)
                : formattedPrices.finalPrice || "Sem lances"}
            </span>
          </div>
        </>
      );
    }

    // Leilão ativo, finalizando ou aguardando
    if (
      saleType === "auction" &&
      (auction?.status === AuctionStatus.Active ||
        auction?.status === AuctionStatus.Ending ||
        auction?.status === AuctionStatus.Awaiting)
    ) {
      return (
        <>
          <span className="text-xs text-neutral-500">{getPriceLabel()}</span>
          <div className="flex items-center gap-2">
            <img
              src={complyImg}
              className="w-6"
              alt="Ícone"
            />
            <span className="flex flex-row text-sm font-bold text-emerald-700">
              {currentBidPrice ? formatCurrency(currentBidPrice) : "R$ 0,00"}
            </span>
          </div>
        </>
      );
    }

    // Compra direta
    if (saleType === "buyNow") {
      return (
        <>
          <span className="text-xs text-neutral-500">Compra imediata:</span>
          <div className="flex items-center gap-2">
            <Zap size={17} />
            <span className="text-sm font-semibold">
              {formattedPrices.buyNow ||
                formatCurrency(product.listing?.buyPrice || 0)}
            </span>
          </div>
        </>
      );
    }

    return null;
  };

  // Renderiza informações de data
  const renderDateInfo = () => {
    if (isSold) {
      return (
        <span>
          Comprado em {formattedDates.sold?.formattedDate} às{" "}
          {formattedDates.sold?.formattedTime}
        </span>
      );
    }

    if (auction?.status === AuctionStatus.Success) {
      return (
        <span>
          Leilão encerrado em {formattedDates.sold?.formattedDate || "—"} às{" "}
          {formattedDates.sold?.formattedTime || "—"}
        </span>
      );
    }

    if (auction?.status === AuctionStatus.Failed) {
      return <span>Leilão encerrado sem lances vencedores</span>;
    }

    if (auctionCancelled) {
      return <span>Leilão cancelado pelo vendedor</span>;
    }

    if (
      auction?.status === AuctionStatus.Active ||
      auction?.status === AuctionStatus.Ending
    ) {
      return (
        <span>
          Tempo restante: {remainingTimeEnd}
          {auction?.status === AuctionStatus.Ending && " (Encerrando!)"}
        </span>
      );
    }

    if (auction?.status === AuctionStatus.Awaiting) {
      return <span>Leilão começa em: {remainingTimeStart}</span>;
    }

    if (product.listing?.status === ListingStatus.Paused) {
      return <span>Anúncio pausado temporariamente</span>;
    }

    return null;
  };

  // Verifica se deve mostrar link de detalhes
  const shouldShowDetailsLink =
    product.listing?.status === ListingStatus.Available ||
    auction?.status === AuctionStatus.Awaiting ||
    auction?.status === AuctionStatus.Active ||
    auction?.status === AuctionStatus.Ending;

  return (
    <>
      {/* Título e Status */}
      <div className="flex flex-col gap-2">
        <div className="flex items-start gap-2 flex-wrap">
          <h3 className="text-sm font-semibold line-clamp-2 flex-1 min-w-0">
            {product.title}
          </h3>
          {getStatusBadge()}
        </div>
      </div>

      {/* Preço Principal */}
      <div className="flex flex-col gap-1">{renderPrice()}</div>

      {/* Informações de Data */}
      <div className="flex flex-col gap-1.5 text-xs text-neutral-600">
        {renderDateInfo()}
      </div>

      {/* Link para detalhes */}
      {shouldShowDetailsLink && (
        <Link
          to={`/product/${product.id}`}
          className="flex flex-row items-center gap-1 text-blue-500 text-xs font-semibold w-fit"
        >
          <Button
            variant={"outline"}
            size={"sm"}
            className="cursor-pointer border-blue-500 text-xs sm:text-sm w-full sm:w-auto"
          >
            <SquareArrowOutUpRight size={14} className="sm:size-4" />
            <span className="ml-1">Ver página</span>
          </Button>
        </Link>
      )}
    </>
  );
};

export default MyListInfo;
