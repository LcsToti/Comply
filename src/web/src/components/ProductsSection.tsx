import ProductCard from "./ProductCard/ProductCard";
import { Link } from "react-router";
import {
  ChevronLeft,
  ChevronRight,
  ClockFading,
  Flame,
  PackageX,
  Search,
} from "lucide-react";
import { useState } from "react";
import { Skeleton } from "./ui/skeleton";
import NoItemsFound from "./NoItemsFound";
import type {
  Product,
  ProductPaginatedList,
  ProductSortBy,
} from "@/types/product";
import type { ListingStatus } from "@/types/listing";
import { usePaginatedProductsQuery } from "@/hooks/products/useProductsQueries";

export interface ProductSectionFilterParams {
  PageSize: number;
  SortBy: ProductSortBy;
  ListingStatus: ListingStatus;
  IsAuctionActive?: boolean;
}

interface ProductSectionProps {
  title: string;
  subtitle: string;
  icon: string;
  seeMoreHref: string;
  filterParams: ProductSectionFilterParams;
}

function ProductSection({
  title,
  subtitle,
  icon,
  seeMoreHref,
  filterParams,
}: ProductSectionProps) {
  const [pageNumber, setPageNumber] = useState(1);
  const [direction, setDirection] = useState<"next" | "prev">("next");

  const queryFilters = {
    ...filterParams,
    PageSize: 4,
    PageNumber: pageNumber,
  };
  const {
    data: paginatedData,
    isLoading,
    isFetching,
    isError,
  } = usePaginatedProductsQuery(queryFilters);
  const products = paginatedData as ProductPaginatedList<Product>;

  const hasNextPage = paginatedData?.hasNextPage ?? false;
  const hasPreviousPage = paginatedData?.hasPreviousPage ?? false;

  const goToNextPage = () => {
    if (hasNextPage) {
      setDirection("next");
      setPageNumber((prev) => prev + 1);
    }
  };

  const goToPrevPage = () => {
    if (hasPreviousPage) {
      setDirection("prev");
      setPageNumber((prev) => prev - 1);
    }
  };

  const isInitialLoading = isLoading;
  const fetchingTransform =
    direction === "next" ? "-translate-x-8" : "translate-x-8";
  const visibleTransform = "translate-x-0";
  const renderContent = () => {
    if (isInitialLoading) {
      return (
        <div className="text-center flex gap-2">
          <Skeleton className="h-[390px] w-[276px] rounded-2xl"></Skeleton>
          <Skeleton className="h-[390px] w-[276px] rounded-2xl"></Skeleton>
          <Skeleton className="h-[390px] w-[276px] rounded-2xl"></Skeleton>
          <Skeleton className="h-[390px] w-[276px] rounded-2xl"></Skeleton>
        </div>
      );
    }

    if (isError) {
      return (
        <div className="text-center text-red-500 p-10">
          Erro ao carregar produtos.
        </div>
      );
    }

    if (!products?.items || products.items.length === 0) {
      return (
        <NoItemsFound
          Icon={PackageX}
          title="Nenhum produto disponível"
          description="Não há produtos a venda atualmente, que tal aproveitar para tomar um café?"
        />
      );
    }

    return (
      <div className="relative">
        {hasPreviousPage && (
          <button
            onClick={goToPrevPage}
            disabled={isFetching}
            className="absolute -left-11 top-1/2 -translate-y-1/2 bg-white/80 shadow rounded-full p-2 z-10 cursor-pointer hover:bg-emerald-50 transition-all disabled:opacity-50"
          >
            <ChevronLeft className="text-gray-700" />
          </button>
        )}
        <div
          className={`flex flex-row gap-4 items-center min-h-[300px] 
            transition-all duration-300 ease-in-out ${
              isFetching
                ? `opacity-50 ${fetchingTransform}`
                : `opacity-100 ${visibleTransform}`
            }`}
        >
          {products.items.map((product: Product) => (
            <div key={product.id} className="w-69 flex-shrink-0">
              <ProductCard
                saleType={
                  product.listing!.isAuctionActive ? "auction" : "buyNow"
                }
                cardType="vertical"
                product={product}
                pageType="search"
              />
            </div>
          ))}
          {/* O loader de "carregar mais" foi removido */}
        </div>

        {/* seta direita */}
        {hasNextPage && (
          <button
            onClick={goToNextPage}
            disabled={isFetching} // Desabilita enquanto troca de página
            className="absolute -right-15 top-1/2 -translate-y-1/2 bg-white/80 shadow rounded-full p-2 z-10 cursor-pointer hover:bg-emerald-50 transition-all disabled:opacity-50"
          >
            <ChevronRight className="text-gray-700" />
          </button>
        )}
      </div>
    );
  };
  return (
    <section className="w-full max-w-6xl mx-auto px-2 py-8">
      <div className="flex justify-between items-center mb-4">
        <div className="flex flex-row justify-center items-center gap-3">
          {icon === "flame" ? (
            <Flame className="w-8 h-8 md:w-14 md:h-14" />
          ) : icon === "search" ? (
            <Search className="w-8 h-8 md:w-14 md:h-14" />
          ) : (
            <ClockFading className="w-8 h-8 md:w-14 md:h-14" />
          )}
          <div>
            <h2 className="md:text-2xl text-gray-700 font-bold">{title}</h2>
            <h3 className="text-xs md:text-sm font-light">{subtitle}</h3>
          </div>
        </div>
        <Link
          to={seeMoreHref}
          className="text-emerald-600 font-semibold text-xs md:text-base flex items-center gap-1 hover:underline"
          onClick={() => window.scrollTo({ top: 0, behavior: "smooth" })}
        >
          Veja mais
          <ChevronRight className="w-3 h-3 md:w-4 md:h-4" />
        </Link>
      </div>
      {renderContent()}
    </section>
  );
}

export default ProductSection;
