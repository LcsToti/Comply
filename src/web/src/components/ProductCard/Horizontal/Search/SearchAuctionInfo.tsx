import { Zap } from "lucide-react";
import type { CommonProductCardProps } from "../../ProductCard";
import { Link } from "react-router";
import { Button } from "@/components/ui/button";
import { formatCurrency } from "@/utils/formatters/formatCurrency";
import { BidStatus } from "@/types/auction";
import { ProductConditionLabels } from "@/types/product";
import complyImg from "@/assets/logo/comply-icon.png"

type SearchCardProps = Pick<
  CommonProductCardProps,
  "product" | "formattedDates" | "formattedPrices" | "mainImageUrl"
>;

const SearchAuctionInfo: React.FC<SearchCardProps> = ({
  product,
  formattedPrices,
}) => {
  const winningBid = product.listing.auction?.bids?.find(
    (b) => b.status === BidStatus.Winning
  )?.value;
  return (
    <div className="flex flex-col justify-between w-full">
      {/* Título e data */}
      <div className="flex flex-col mb-3">
        <h3 className="text-lg font-semibold text-gray-900 truncate leading-tight">
          {product.title}{" "}
          <span className="text-gray-500 text-sm">
            {ProductConditionLabels[product.condition]}
          </span>
        </h3>
      </div>

      {/* Valores */}
      <div className="flex flex-col gap-2">
        {/* Lance inicial */}
        <div className="flex items-center gap-2">
          <img
            src={complyImg}
            className="w-6 h-6 object-contain"
            alt="Ícone Leilão"
          />
          <span className="text-2xl font-bold text-emerald-700 leading-none">
            {winningBid ? formatCurrency(winningBid) : formattedPrices.startBid}
          </span>
        </div>

        {/* Preço “Compre Agora” */}
        <div className="flex items-center gap-1 text-sm text-gray-500">
          <span>ou compre agora por</span>
          <Zap className="text-gray-400 w-4 h-4" />
          <span className="font-medium text-gray-700">
            {formattedPrices.buyNow}
          </span>
        </div>
        <Link to={`/product/${product.id}`} className="mt-4 w-fit">
          <Button
            size={"sm"}
            className="w-xs border-emerald-700 cursor-pointer hover:text-emerald-800"
            variant={"outline"}
          >
            Ver detalhes
          </Button>
        </Link>
      </div>
    </div>
  );
};

export default SearchAuctionInfo;
