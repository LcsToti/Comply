import { formatCurrency } from "@/utils/formatters/formatCurrency";
import { Zap } from "lucide-react";
import type { CommonProductCardProps } from "../../ProductCard";
import { Link } from "react-router";
import { Button } from "@/components/ui/button";
import { ProductConditionLabels } from "@/types/product";

type SearchCardProps = Pick<
  CommonProductCardProps,
  "product" | "formattedDates" | "formattedPrices" | "mainImageUrl"
>;

const SearchBuyNowInfo: React.FC<SearchCardProps> = ({
  product,
  formattedDates,
  formattedPrices,
}) => (
  <div className="flex flex-col justify-between w-full">
    {/* Título e data */}
    <div className="flex flex-col mb-3">
      <h3 className="text-lg font-semibold text-gray-900 truncate leading-tight">
        {product.title}{" "}
        <span className="text-gray-500 text-sm">
          {ProductConditionLabels[product.condition]}
        </span>
      </h3>
      <span className="text-xs text-gray-500">
        Criado em {formattedDates.created.formattedDate} às{" "}
        {formattedDates.created.formattedTime}
      </span>
    </div>

    {/* Preço e parcelamento */}
    <div className="flex flex-col gap-2">
      <div className="flex items-center gap-2">
        <Zap className="text-emerald-700 w-5 h-5" />
        <span className="text-2xl font-bold text-emerald-700 leading-none">
          {formattedPrices.buyNow}
        </span>
      </div>

      {product.listing.buyPrice > 0 && (
        <span className="text-sm text-gray-500">
          Em até 10x sem juros de{" "}
          <span className="font-medium text-gray-700">
            {formatCurrency(product.listing.buyPrice / 10)}
          </span>
        </span>
      )}
    </div>

    <Link to={`/product/${product.id}`} className="mt-4 w-fit">
      <Button
        className="w-xs border-emerald-700 cursor-pointer hover:text-emerald-800"
        variant={"outline"}
      >
        Ver detalhes
      </Button>
    </Link>
  </div>
);

export default SearchBuyNowInfo;
