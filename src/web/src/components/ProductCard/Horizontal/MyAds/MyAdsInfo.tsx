import { SquareArrowOutUpRight, Eye, Zap } from "lucide-react";
import { Link } from "react-router";
import type { CommonProductCardProps } from "../../ProductCard";
import { useCountdown } from "@/hooks/useCountdownHook";
import { formatCurrency } from "@/utils/formatters/formatCurrency";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import complyImg from "@/assets/logo/comply-icon.png";

const MyAdsInfo: React.FC<
  Pick<
    CommonProductCardProps,
    "product" | "formattedDates" | "auction" | "formattedPrices" | "saleType"
  >
> = ({ product, formattedDates, auction, formattedPrices, saleType }) => {
  const startsOn = useCountdown(product.listing?.auction?.settings.startDate);

  const status = product.listing?.status;
  const auctionStatus = auction?.status;
  const auctionBids = auction?.bids;
  const bidsCount = auctionBids?.length || 0;
  const winningBidEntity = auctionBids?.find((b) => b.status === "Winning");
  const winnerBidEntity = auctionBids?.find((b) => b.status === "Winner");
  const winningBid = winningBidEntity
    ? formatCurrency(winningBidEntity.value)
    : null;
  const winnerBid = winnerBidEntity
    ? formatCurrency(winnerBidEntity.value)
    : null;

  // Verifica se o leilão está ativo (Active ou Ending)
  const isAuctionActive =
    auctionStatus === "Active" || auctionStatus === "Ending";

  if (!product.listing) {
    return (
      <div className="flex flex-col gap-2 w-full">
        <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-2">
          <div className="flex-1">
            <h3 className="text-sm sm:text-md font-semibold break-words">
              {product.title}
            </h3>
          </div>
          {product.watchListCount > 0 && (
            <div className="flex items-center gap-1 text-xs sm:text-sm text-[#6B6B6B]">
              <Eye size={14} className="sm:size-4" />
              <span>{product.watchListCount}</span>
            </div>
          )}
        </div>

        <div className="flex flex-wrap gap-2">
          <Badge variant="destructive">Produto não listado</Badge>
        </div>
      </div>
    );
  }

  return (
    <div className="flex grow flex-col justify-between gap-2 sm:gap-3 w-full">
      {/* Título e Watchlist */}
      <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-2">
        <div className="flex-1 min-w-0">
          <h3 className="text-sm sm:text-md font-semibold truncate">
            {product.title}
          </h3>
        </div>
        {product.watchListCount > 0 && (
          <div className="flex items-center gap-1 text-xs sm:text-sm text-[#6B6B6B] shrink-0">
            <Eye size={14} className="sm:size-4" />
            <span>{product.watchListCount}</span>
          </div>
        )}
      </div>

      {/* Informações de Leilão */}
      {saleType === "auction" && (
        <div className="flex flex-col gap-2 sm:gap-3">
          {auctionStatus === "Awaiting" && (
            <p className="text-xs sm:text-sm text-[#6B6B6B]">
              Leilão começa em:{" "}
              <span className="font-semibold">{startsOn}</span>
            </p>
          )}

          {/* Mostra informações de lances quando Active ou Ending */}
          {isAuctionActive && (
            <div className="flex flex-col sm:flex-row sm:items-end gap-3 sm:gap-6">
              <div className="flex flex-row sm:flex-col items-center sm:items-center justify-between sm:justify-between">
                <span className="text-xs text-[#6B6B6B]">Lance Inicial:</span>
                <span className="text-xs sm:text-sm font-semibold">
                  {formatCurrency(auction?.settings.startBidValue || 0)}
                </span>
              </div>

              <div className="flex flex-row sm:flex-col items-center sm:items-center justify-between sm:justify-between">
                <span className="text-xs text-[#6B6B6B]">Lance Atual:</span>
                <div className="flex items-center gap-1">
                  <img
                    src={complyImg}
                    className="w-4 sm:w-5"
                    alt="Ícone"
                  />
                  <span className="text-sm sm:text-sm font-bold text-emerald-700">
                    {bidsCount > 0
                      ? winningBid
                      : formatCurrency(auction?.settings.startBidValue || 0)}
                  </span>
                </div>
              </div>

              <div className="flex flex-row sm:flex-col items-center sm:items-center justify-between sm:justify-between">
                <span className="text-xs text-[#6B6B6B]">Lances:</span>
                <span className="text-xs sm:text-sm font-semibold">
                  {bidsCount}
                </span>
              </div>
            </div>
          )}

          {/* Informações de leilões finalizados */}
          {(auctionStatus === "Success" || auctionStatus === "Failed") && (
            <div className="text-xs sm:text-sm text-[#6B6B6B]">
              {auctionStatus === "Success" && bidsCount > 0 && (
                <div className="space-y-1">
                  <p className="text-xs">Finalizado com o valor de:</p>
                  <div className="flex flex-row items-center gap-1">
                    <img
                      src={complyImg}
                      className="w-4 sm:w-5"
                      alt="Ícone"
                    />
                    <span className="font-bold text-sm sm:text-md text-emerald-700">
                      {winnerBid}
                    </span>
                  </div>
                </div>
              )}
              {auctionStatus === "Failed" && (
                <p className="text-xs">Leilão finalizado sem lances</p>
              )}
            </div>
          )}

          {auctionStatus === "Cancelled" && (
            <p className="text-xs sm:text-sm text-[#6B6B6B]">
              Leilão cancelado em: {formattedDates.updated.formattedDate}
            </p>
          )}
        </div>
      )}

      {/* Bloco de Preço Final (Vendidos) */}
      {status === "Sold" && (
        <div className="flex flex-col w-full gap-1">
          <h4 className="text-xs text-[#6B6B6B]">Valor final:</h4>
          <div className="flex flex-row gap-1 items-center">
            <Zap size={15} color="#047857" className="border-emerald-700" />
            <h1 className="text-sm sm:text-md font-bold text-emerald-700">
              {formattedPrices.finalPrice}
            </h1>
          </div>
        </div>
      )}

      {/* Data de Atualização/Venda */}
      {(status === "Sold" || status === "SoldByAuction") && (
        <span className="text-xs text-[#6B6B6B]">
          Vendido em: {formattedDates.sold.formattedDate} às{" "}
          {formattedDates.sold.formattedTime}
        </span>
      )}

      {/* Link Ver Detalhes */}
      {status === "Available" && (
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
    </div>
  );
};

export default MyAdsInfo;
