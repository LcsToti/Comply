import { Button } from "@/components/ui/button";
import { Star, Pen, BanknoteArrowUp } from "lucide-react";
import type { CommonProductCardProps } from "../../ProductCard";
import BidHistory from "@/components/Actions/BidHistory";
import CancelAuction from "@/components/Actions/CancelAuction";
import AddFeatured from "@/components/Actions/AddFeatured";
import { useState } from "react";
import ShareProduct from "@/components/Actions/ShareProduct";
import ToggleListingVisibility from "@/components/Actions/ToggleListingVisibility";
import { Link } from "react-router";
import { useProductBadges } from "@/hooks/useProductBadges";
import { ProductBadges } from "@/components/ProductBadges";
import { useCountdown } from "@/hooks/useCountdownHook";
import complyIcon from "@/assets/logo/comply-icon.png";
import complyIconWhite from "@/assets/logo/comply-icon-white.png";

const MyAdsAction: React.FC<
  Pick<
    CommonProductCardProps,
    "product" | "auction" | "saleType" | "remainingTimeEnd" | "navigate"
  >
> = ({ product, auction, saleType, remainingTimeEnd, navigate }) => {
  const [openShareModal, setOpenShareModal] = useState(false);

  const { badges, status, computed } = useProductBadges({
    product,
    auction,
    saleType,
  });
  const endsIn = useCountdown(auction?.settings.endDate);
  const auctionStatus = auction?.status;

  return (
    <div className="flex flex-col items-stretch sm:items-end justify-between flex-1 gap-3 sm:gap-4">
      <div className="flex items-stretch sm:items-end flex-col w-full gap-2 sm:gap-3">
        {/* Badges de Status */}
        <div className="flex justify-start sm:justify-end w-full">
          <ProductBadges
            primary={badges.primary}
            additional={badges.additional}
          />
        </div>

        <ShareProduct open={openShareModal} onOpenChange={setOpenShareModal} />

        {/* Informações de Status */}
        <div className="flex flex-col sm:flex-row gap-2 sm:gap-4 py-2 sm:py-4 items-start sm:items-center w-full justify-start sm:justify-end">
          {computed.isAvailable &&
            status.auction === "Active" &&
            saleType === "auction" && (
              <div className="flex flex-col items-start sm:items-end w-full sm:w-auto">
                <span className="text-xs sm:text-sm text-[#6B6B6B]">
                  Tempo restante: {remainingTimeEnd}
                </span>

                {product.featured && (
                  <div className="flex flex-row items-center gap-1 pt-1 sm:pt-2">
                    <span className="text-xs sm:text-sm text-[#6B6B6B] font-semibold">
                      Produto em destaque
                    </span>
                    <Star
                      size={16}
                      className="sm:size-[18px] text-emerald-700"
                    />
                  </div>
                )}
              </div>
            )}
          {/* Mostra tempo restante quando está Ending */}
          {auctionStatus === "Ending" && (
            <div className="flex items-center gap-2 text-xs sm:text-sm text-red-600 font-semibold">
              <span>Termina em: {endsIn}</span>
            </div>
          )}

          {computed.isPaused && (
            <div className="flex flex-row items-center gap-1 sm:gap-2">
              <span className="text-xs sm:text-sm text-[#6B6B6B]">
                Anúncio pausado
              </span>
              <div className="bg-[#FFC107] w-2.5 h-2.5 sm:w-3 sm:h-3 rounded-full"></div>
            </div>
          )}

          {status.auction === "Failed" && (
            <div className="flex flex-col items-start sm:items-end w-full sm:w-auto">
              <div className="flex flex-row items-center gap-1 sm:gap-2">
                <span className="text-xs sm:text-sm text-[#6B6B6B]">
                  Leilão falhou
                </span>
                <div className="bg-[#E53935] w-2.5 h-2.5 sm:w-3 sm:h-3 rounded-full"></div>
              </div>
              <span className="text-xs sm:text-sm text-[#6B6B6B]">
                Não recebeu lances
              </span>
            </div>
          )}
        </div>
      </div>

      {/* Botões de Ação Principal */}
      <div className="flex flex-col sm:flex-row lg:flex-row justify-stretch sm:justify-end w-full gap-1 sm:gap-2">
        {computed.isListed && computed.isPaused && (
          <div className="flex flex-col sm:flex-row gap-2 justify-start sm:justify-end w-full">
            <Button
              size={"sm"}
              variant="outline"
              onClick={() => navigate(`/product/${product.id}/edit`)}
              className="w-full sm:w-auto rounded-md hover:bg-neutral-50 active:scale-95 transition cursor-pointer text-xs sm:text-sm"
              title="Editar"
            >
              <Pen size={14} className="sm:size-[17px] text-neutral-700" />
              <span className="ml-1 sm:ml-2">Editar</span>
            </Button>
          </div>
        )}
        {computed.isListed && computed.isAvailable && (
          <div className="flex flex-col sm:flex-row gap-2 justify-start sm:justify-end w-full">
            {saleType === "buyNow" && (
              <Link
                to={`/product/${product.id}/auction`}
                className="w-full sm:w-auto"
              >
                <Button
                  size={"sm"}
                  variant="outline"
                  className="w-full sm:w-auto rounded-md hover:bg-neutral-50 active:scale-95 transition cursor-pointer text-xs sm:text-sm"
                >
                  <img
                    src={complyIcon}
                    alt="comply"
                    className="w-5 sm:w-7"
                  />
                  <span className="ml-1 sm:ml-2">Criar Leilão</span>
                </Button>
              </Link>
            )}
          </div>
        )}
        {!computed.isListed ? (
          <Button
            size={"sm"}
            className="w-full sm:w-auto bg-emerald-700 hover:bg-emerald-500 text-white cursor-pointer text-xs sm:text-sm"
          >
            Terminar anúncio
          </Button>
        ) : (
          <>
            {computed.isAvailable &&
              status.auction === "Active" &&
              auction &&
              product.listing?.auction?.bids && (
                <div className="w-full sm:w-auto">
                  <BidHistory auction={auction} />
                </div>
              )}

            {status.auction === "Active" &&
              auction &&
              (!product.listing?.auction?.bids ||
                product.listing?.auction?.bids.length === 0) && (
                <div className="w-full sm:w-auto">
                  <CancelAuction auctionId={auction.id} />
                </div>
              )}

            {computed.isPaused && (
              <div className="w-full sm:w-auto">
                <ToggleListingVisibility
                  action="resume"
                  listingId={product.listing?.id}
                />
              </div>
            )}

            {computed.isAvailable &&
              (saleType === "buyNow" ||
                (saleType === "auction" && status.auction !== "Active")) &&
              !product.listing.auction && (
                <div className="w-full sm:w-auto">
                  <ToggleListingVisibility
                    action="pause"
                    listingId={product.listing?.id}
                  />
                </div>
              )}

            {computed.isSold && (
              <Button
                size={"sm"}
                className="w-full sm:w-auto bg-emerald-700 hover:bg-[#00A884] cursor-pointer text-xs sm:text-sm"
                onClick={() => {
                  navigate("/dashboard/profile?tab=withdrawals");
                }}
              >
                <BanknoteArrowUp size={18} />
                <span className="ml-1 sm:ml-2">Ver detalhes</span>
              </Button>
            )}

            {status.auction === "Failed" && (
              <Button
                size={"sm"}
                className="w-full sm:w-auto bg-emerald-700 hover:bg-[#00A884] cursor-pointer text-xs sm:text-sm"
              >
                <img
                  src={complyIconWhite}
                  className="w-6 sm:w-8"
                  alt="Ícone Leilão"
                />
                <span className="ml-1 sm:ml-2">Retomar Leilão</span>
              </Button>
            )}

            {!product.featured && computed.isAvailable && (
              <div className="w-full sm:w-auto">
                <AddFeatured />
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
};

export default MyAdsAction;
