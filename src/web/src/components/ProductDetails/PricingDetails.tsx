import { useCountdown } from "@/hooks/useCountdownHook";
import { formatCurrency } from "@/utils/formatters/formatCurrency";
import {
  Clock,
  ShoppingBag,
  ShieldCheck,
  PackageX,
  AlertCircle,
} from "lucide-react";
import { Button } from "../ui/button";
import { useNavigate } from "react-router";
import { useState } from "react";
import PlaceNewBidModal from "./PlaceNewBidModal";
import { toast } from "sonner";
import { usePlaceBidMutation } from "@/hooks/listings/useAuctionsMutations";
import hammerGreenImg from "@/assets/logo/hammer-green.png"

interface PriceCardProps {
  currentBid: number | undefined;
  buyNowPrice: number | undefined;
  watchers: number | undefined;
  bidCount: number | undefined;
  auctionEndDate: string | undefined;
  startBidValue: number | undefined;
  auctionStartDate: string | undefined;
  existsAuction: boolean | undefined;
  auctionId: string | undefined;
  disabled?: boolean;
  isOwner?: boolean;
}

function PriceCard({
  currentBid,
  buyNowPrice,
  watchers,
  bidCount,
  auctionEndDate,
  startBidValue,
  auctionStartDate,
  existsAuction,
  auctionId,
  disabled = false,
  isOwner = false,
}: PriceCardProps) {
  const [openBidModal, setOpenBidModal] = useState(false);
  const { mutateAsync: placeBid } = usePlaceBidMutation();
  const timeRemaining = useCountdown(auctionEndDate);
  const timeToStart = useCountdown(auctionStartDate);

  const now = new Date();
  const startDate = auctionStartDate ? new Date(auctionStartDate) : null;
  const auctionHasStarted = startDate ? startDate <= now : true;
  const navigate = useNavigate();

  async function handleConfirmBid(amount: number, paymentMethodId: string) {
    if (!auctionId) return;

    await placeBid({
      auctionId,
      params: {
        Value: amount,
        PaymentMethodId: paymentMethodId,
      },
    });
  }

  const handleBuyNow = () => {
    if (isOwner) {
      toast.error("Você não pode comprar seu próprio produto.");
      return;
    }

    if (disabled) {
      toast.error("Este produto não está mais disponível.");
      return;
    }

    navigate("checkout");
  };

  const renderStatusBadge = () => {
    if (isOwner) {
      return (
        <div className="flex items-center gap-2 p-3 bg-blue-50 border border-blue-200 rounded-lg mb-4">
          <ShieldCheck className="h-4 w-4 text-blue-600" />
          <span className="text-xs font-medium text-blue-800">Seu produto</span>
        </div>
      );
    }

    if (disabled) {
      return (
        <div className="flex items-center gap-2 p-3 bg-gray-50 border border-gray-200 rounded-lg mb-4">
          <PackageX className="h-4 w-4 text-gray-600" />
          <span className="text-xs font-medium text-gray-800">
            Produto indisponível
          </span>
        </div>
      );
    }

    return null;
  };

  return (
    <div className="border border-gray-200 rounded-lg p-6 bg-white shadow-sm flex flex-col gap-4 sticky top-24">
      {/* Badge de status */}
      {renderStatusBadge()}

      {!existsAuction ? (
        <>
          <div className="text-center flex flex-col items-center gap-2 py-6">
            <Clock className="text-gray-400" size={22} />
            <p className="text-gray-600 font-medium">Sem leilão agendado</p>
          </div>

          <div className="flex items-center gap-3 my-2">
            <hr className="flex-grow border-t border-gray-200" />
            <span className="text-xs text-gray-400 font-medium">OU</span>
            <hr className="flex-grow border-t border-gray-200" />
          </div>
        </>
      ) : auctionHasStarted ? (
        <>
          <div className="flex justify-between items-start">
            <div>
              <span className="text-sm text-gray-500">
                {currentBid
                  ? "Lance atual:"
                  : startBidValue
                    ? "Lance a partir de:"
                    : ""}
              </span>
              <p className="text-4xl font-bold text-gray-800 leading-tight">
                {currentBid
                  ? formatCurrency(currentBid)
                  : startBidValue
                    ? formatCurrency(startBidValue)
                    : "Sem Lances"}
              </p>
              <span className="flex items-center gap-1.5 text-gray-600">
                <Clock size={16} /> {timeRemaining ?? "--"}
              </span>
            </div>

            <div className="text-right text-sm text-gray-500 flex flex-col gap-1 pt-1">
              <span className="flex items-center justify-end gap-1.5">
                <img
                  src={hammerGreenImg}
                  className="w-4 h-4"
                />
                {bidCount ?? 0} lances
              </span>
            </div>
          </div>

          {/* Botão de lance com validação */}
          {isOwner ? (
            <Button
              size="lg"
              className="w-full bg-gray-300 text-gray-600 cursor-not-allowed"
              disabled
            >
              <AlertCircle size={20} className="mr-2" />
              Você é o proprietário
            </Button>
          ) : disabled ? (
            <Button
              size="lg"
              className="w-full bg-gray-300 text-gray-600 cursor-not-allowed"
              disabled
            >
              <PackageX size={20} className="mr-2" />
              Produto indisponível
            </Button>
          ) : (
            <PlaceNewBidModal
              open={openBidModal}
              onOpenChange={setOpenBidModal}
              minBid={
                currentBid
                  ? Math.ceil(currentBid * 1.05)
                  : startBidValue
                    ? Math.ceil(startBidValue * 1.05)
                    : 0
              }
              maxBid={buyNowPrice ? buyNowPrice : 0}
              onConfirm={handleConfirmBid}
            />
          )}

          <div className="text-right text-xs text-gray-500 -mt-2">
            {watchers ?? 0} pessoas estão de olho neste produto
          </div>

          <div className="flex items-center gap-3 my-2">
            <hr className="flex-grow border-t border-gray-200" />
            <span className="text-xs text-gray-400 font-medium">OU</span>
            <hr className="flex-grow border-t border-gray-200" />
          </div>
        </>
      ) : (
        <>
          <div className="text-center flex flex-col items-center gap-1">
            <Clock className="text-emerald-700" size={20} />
            <p className="text-gray-700 text-sm">Leilão começa em:</p>
            <p className="text-4xl font-bold text-yellow-700">{timeToStart}</p>
          </div>

          <div className="flex items-center gap-3 my-2">
            <hr className="flex-grow border-t border-gray-200" />
            <span className="text-xs text-gray-400 font-medium">OU</span>
            <hr className="flex-grow border-t border-gray-200" />
          </div>
        </>
      )}

      {/* --- COMPRE AGORA --- */}
      <div>
        <span className="text-sm text-gray-500">Compre agora:</span>
        <p className="text-3xl font-bold text-gray-800 leading-tight">
          {buyNowPrice ? formatCurrency(buyNowPrice) : "Sem Valor"}
        </p>
      </div>

      <Button
        size="lg"
        variant="outline"
        className={`w-full ${
          isOwner || disabled
            ? "border-gray-300 text-gray-400 cursor-not-allowed"
            : "border-emerald-600 text-emerald-600 hover:bg-emerald-50 cursor-pointer"
        }`}
        onClick={handleBuyNow}
        disabled={isOwner || disabled}
      >
        {isOwner ? (
          <>
            <ShieldCheck size={20} className="mr-2" />
            Seu produto
          </>
        ) : disabled ? (
          <>
            <PackageX size={20} className="mr-2" />
            Indisponível
          </>
        ) : (
          <>
            <ShoppingBag size={20} className="mr-2" />
            Compre agora
          </>
        )}
      </Button>
    </div>
  );
}

export default PriceCard;
