import React from "react";
import { Zap } from "lucide-react";
import { Link } from "react-router";
import { formatCurrency } from "@/utils/formatters/formatCurrency";
import type { CommonProductCardProps } from "../ProductCard";
import { useCountdown } from "@/hooks/useCountdownHook";
import { Badge } from "@/components/ui/badge";
import complyImg from "@/assets/logo/comply-icon.png"

const ProductCardVertical: React.FC<CommonProductCardProps> = ({
  saleType,
  product,
  mainImageUrl,
  formattedPrices,
}) => {
  const timeRemaining = useCountdown(product.listing.auction?.settings.endDate);
  const startsOn = useCountdown(product.listing.auction?.settings.startDate);

  const isAuction = saleType === "auction";
  const hasScheduledAuction = product.listing.auction && !isAuction;

  return (
    <Link
      to={`/product/${product.id}`}
      className="group flex flex-col w-full rounded-xl overflow-hidden border border-neutral-200 bg-white hover:border-neutral-300 transition-all duration-300"
      onClick={() => window.scrollTo({ top: 0, behavior: "smooth" })}
    >
      {/* Container da imagem com overflow hidden para o efeito de zoom */}
      <div className="relative w-full aspect-square overflow-hidden bg-neutral-100">
        <img
          src={mainImageUrl}
          alt={product.title}
          className="w-full h-full object-cover transform group-hover:scale-110 transition-transform duration-500 ease-out"
        />

        {/* Badge de tipo de venda */}
        <div className="absolute top-3 left-3">
          {isAuction ? (
            <Badge className="bg-emerald-700 hover:bg-emerald-700 text-white">
              Leilão
            </Badge>
          ) : (
            <Badge className="bg-emerald-700 hover:bg-emerald-700 text-white">
              Compra direta
            </Badge>
          )}
        </div>
      </div>

      {/* Conteúdo do card */}
      <div className="flex flex-col flex-1 p-4 gap-3">
        {/* Título */}
        <h3 className="text-sm md:text-base font-semibold line-clamp-2 min-h-[2.5rem] md:min-h-[3rem] group-hover:text-emerald-700 transition-colors">
          {product.title}
        </h3>

        {/* Preços */}
        <div className="flex flex-col gap-2">
          {isAuction ? (
            <>
              {/* Lance inicial */}
              <div className="flex items-center gap-2">
                <img
                  src={complyImg}
                  className="w-4 h-4 object-contain"
                  alt="Leilão"
                />
                <span className="text-lg md:text-xl text-neutral-900">
                  {formattedPrices.startBid}
                </span>
              </div>

              {/* Compre agora (se disponível) */}
              {formattedPrices.buyNow && (
                <div className="flex items-center gap-1.5 text-xs md:text-sm text-neutral-600">
                  <span>ou</span>
                  <Zap className="fill-emerald-700" color="#047857" size={14} />
                  <span className="font-medium">{formattedPrices.buyNow}</span>
                </div>
              )}
            </>
          ) : (
            <>
              {/* Preço de compra direta */}
              <div className="flex items-center gap-2">
                <Zap className="fill-emerald-700" color="#047857" size={16} />
                <span className="text-lg md:text-xl text-neutral-900">
                  {formattedPrices.buyNow}
                </span>
              </div>

              {/* Parcelamento */}
              <p className="text-xs md:text-sm text-neutral-600">
                Em até <span className="font-medium">10x</span> de{" "}
                <span className="font-medium">
                  {formatCurrency(product.listing.buyPrice / 10)}
                </span>
              </p>
            </>
          )}
        </div>

        {/* Informação de tempo (footer do card) */}
        <div className="mt-auto pt-3 border-t border-neutral-100">
          {isAuction ? (
            <p className="text-xs md:text-sm text-center text-neutral-600">
              Termina em{" "}
              <span className="font-semibold text-amber-600">
                {timeRemaining}
              </span>
            </p>
          ) : hasScheduledAuction ? (
            <p className="text-xs md:text-sm text-center text-neutral-600">
              Leilão inicia em{" "}
              <span className="font-semibold text-emerald-700">{startsOn}</span>
            </p>
          ) : (
            <p className="text-xs md:text-sm text-center text-neutral-500">
              Sem leilão agendado
            </p>
          )}
        </div>
      </div>
    </Link>
  );
};

export default ProductCardVertical;
