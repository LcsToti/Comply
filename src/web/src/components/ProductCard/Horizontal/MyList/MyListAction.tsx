// MyListAction.tsx - VERSÃO FINAL INTEGRADA
import { Button } from "@/components/ui/button";
import { PackageCheck } from "lucide-react";
import type { CommonProductCardProps } from "../../ProductCard";
import ShareProduct from "@/components/Actions/ShareProduct";
import ToggleWatchList from "@/components/Actions/ToggleWatchList";
import { useState } from "react";
import PlaceNewBidModal from "@/components/ProductDetails/PlaceNewBidModal";
import { toast } from "sonner";
import { getUserBidStatus } from "@/utils/UserBidStatus";
import { getCurrentBidPrice } from "@/utils/CurrentBidPrice";
import { ListingStatus } from "@/types/listing";
import { AuctionStatus } from "@/types/auction";
import { usePlaceBidMutation } from "@/hooks/listings/useAuctionsMutations";

const MyListAction: React.FC<
  Pick<
    CommonProductCardProps,
    "product" | "auction" | "saleType" | "navigate" | "user"
  >
> = ({ product, auction, saleType, user, navigate }) => {
  const [shareOpen, setShareOpen] = useState(false);
  const [bidModalOpen, setBidModalOpen] = useState(false);

  const { mutateAsync: placeBid } = usePlaceBidMutation();
  const userBidStatus = getUserBidStatus(product, user?.id);
  const { isWinning, hasBid } = userBidStatus;
  const currentBidPrice = getCurrentBidPrice(product);
  const isSold =
    product.listing?.status === ListingStatus.Sold ||
    product.listing?.status === ListingStatus.SoldByAuction;

  const isUserProduct = product.listing?.buyerId === user?.id;

  const auctionUnavailable =
    auction?.status === AuctionStatus.Failed ||
    auction?.status === AuctionStatus.Cancelled;

  const isPaused = product.listing?.status === ListingStatus.Paused;

  const isProcessing =
    auction?.isProcessingBid || product.listing?.isProcessingPurchase;

  const isAuctionActive =
    auction?.status === AuctionStatus.Active ||
    auction?.status === AuctionStatus.Ending;

  // Calcula valores mínimo e máximo para o lance
  const minBidValue = currentBidPrice
    ? currentBidPrice + 1
    : auction?.settings?.startBidValue || 0;

  const maxBidValue = auction?.settings?.winBidValue;

  async function handleConfirmBid(amount: number, paymentMethodId: string) {
    if (!auction?.id) return;

    const auctionId = auction.id;

    try {
      await placeBid({
        auctionId,
        params: {
          Value: amount,
          PaymentMethodId: paymentMethodId,
        },
      });
      toast.success("Lance realizado com sucesso!");
    } catch {
      toast.error("Erro ao realizar lance. Tente novamente.");
    }

    setBidModalOpen(false);
  }

  const handleBuyNow = () => {
    navigate(`/product/${product.id}/checkout`);
  };

  const renderNoAuctionActions = () => {
    if (auction || saleType === "auction" || isSold) return null;
    return (
      <div className="flex flex-col w-full gap-2">
        <Button
          size="lg"
          className="text-sm bg-emerald-600 cursor-pointer hover:bg-emerald-700"
          onClick={handleBuyNow}
          disabled={isPaused || isProcessing}
        >
          {isProcessing ? "Processando..." : "Comprar agora"}
        </Button>
      </div>
    );
  };

  const renderWinningBidActions = () => {
    if (!isAuctionActive || !hasBid || !isWinning) return null;

    return (
      <div className="flex flex-col w-full gap-2">
        <PlaceNewBidModal
          minBid={minBidValue}
          maxBid={maxBidValue}
          onConfirm={handleConfirmBid}
          onOpenChange={setBidModalOpen}
          open={bidModalOpen}
        />
      </div>
    );
  };

  const renderOutbidActions = () => {
    if (!isAuctionActive || !hasBid || isWinning) return null;

    return (
      <div className="flex flex-col w-full gap-2">
        <PlaceNewBidModal
          minBid={minBidValue}
          maxBid={maxBidValue}
          onConfirm={handleConfirmBid}
          onOpenChange={setBidModalOpen}
          open={bidModalOpen}
        />
      </div>
    );
  };

  // VARIAÇÃO 4: Produto comprado pelo usuário
  const renderPurchasedActions = () => {
    if (!isSold || !isUserProduct) return null;

    return (
      <div className="flex flex-row gap-2">
        <Button
          size="sm"
          variant="outline"
          className="text-sm gap-2 cursor-pointer"
          onClick={() => navigate(`/dashboard/profile?tab=payments`)}
        >
          <PackageCheck size={18} />
          Ver detalhes do pagamento
        </Button>
      </div>
    );
  };

  const renderDefaultBidForm = () => {
    if (hasBid || saleType !== "auction" || !isAuctionActive) return null;

    return (
      <div className="flex flex-col gap-2">
        <PlaceNewBidModal
          minBid={minBidValue}
          maxBid={maxBidValue}
          onConfirm={handleConfirmBid}
          onOpenChange={setBidModalOpen}
          open={bidModalOpen}
        />
      </div>
    );
  };

  // Renderiza status badge detalhado
  const renderStatusBadge = () => {
    if (isSold && isUserProduct) {
      return (
        <div className="flex flex-col items-end gap-0.5">
          <span className="text-sm font-semibold text-emerald-700">
            Você comprou
          </span>
          <span className="text-xs text-neutral-500">
            {saleType === "auction" ? "via Leilão" : "via Compra direta"}
          </span>
        </div>
      );
    }

    if (isSold && !isUserProduct) {
      return (
        <div className="flex flex-col items-end gap-0.5">
          <span className="text-sm font-semibold text-neutral-700">
            Vendido
          </span>
          <span className="text-xs text-neutral-500">
            {saleType === "auction"
              ? "Leilão encerrado"
              : "Comprado por outro usuário"}
          </span>
        </div>
      );
    }

    if (auctionUnavailable) {
      return (
        <div className="flex flex-col items-end gap-0.5">
          <span className="text-sm font-semibold text-red-600">
            {auction?.status === AuctionStatus.Failed
              ? "Leilão Encerrado"
              : "Leilão Cancelado"}
          </span>
          <span className="text-xs text-neutral-500">
            {auction?.status === AuctionStatus.Failed
              ? "Sem lances vencedores"
              : "Cancelado pelo vendedor"}
          </span>
        </div>
      );
    }

    return null;
  };

  // Renderiza ações do topo
  const renderTopActions = () => {
    return (
      <>
        <ToggleWatchList
          listingId={product.listing.id}
          productId={product.id}
        />

        {!isSold && !auctionUnavailable && (
          <ShareProduct open={shareOpen} onOpenChange={setShareOpen} />
        )}

        {renderStatusBadge()}
      </>
    );
  };

  return (
    <>
      <div className="flex flex-row justify-end items-start gap-2">
        {renderTopActions()}
      </div>

      <div className="flex flex-col gap-2">
        {renderNoAuctionActions()}
        {renderWinningBidActions()}
        {renderOutbidActions()}
        {renderDefaultBidForm()}
        {renderPurchasedActions()}
      </div>
    </>
  );
};

export default MyListAction;
