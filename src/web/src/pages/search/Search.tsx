import type { GetFilteredProductsParams } from "@/api/products/products";
import Footer from "@/components/Footer";
import Header from "@/components/Header";
import NoItemsFound from "@/components/NoItemsFound";
import ProductCard from "@/components/ProductCard/ProductCard";
import SearchSidebar from "@/components/SearchPage/SearchSideBar";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useInfiniteProductsQuery } from "@/hooks/products/useProductsQueries";
import {
  CategoriesLabels,
  ProductConditionLabels,
  ProductSortByLabels,
  type Product,
  type Categories,
  type ProductCondition,
  type ProductSortBy,
} from "@/types/product";
import { ArrowLeft, Grip, List, Loader2, PackageX } from "lucide-react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router";

export default function Search() {
  const [viewMode, setViewMode] = useState<"list" | "grid">("list");
  const [filterParams, setFilterParams] = useState<GetFilteredProductsParams>({
    PageNumber: 1,
    PageSize: 10,
    SortBy: undefined,
    ListingStatus: "Available",
    SearchTerm: undefined,
    IsAuctionActive: undefined,
    Category: undefined,
  });

  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const newSearchTerm = params.get("q") || undefined;
    const newSortBy = (params.get("sort") as ProductSortBy) || "Newest";
    const newIsAuction = params.get("isAuction") === "true";
    const newCategory = params.get("category") as Categories | undefined;
    const newCondition =
      (params.get("condition") as ProductCondition) || undefined;
    const NoBidsOnly = params.get("nobidsonly") === "true";

    setFilterParams({
      PageNumber: 1,
      PageSize: 10,
      SortBy: newSortBy,
      ListingStatus: "Available",
      SearchTerm: newSearchTerm,
      IsAuctionActive: newIsAuction ? true : undefined,
      Category: newCategory,
      Condition: newCondition,
      NoBidsOnly: NoBidsOnly ? true : undefined,
    });
  }, [location.search]);

  const handleFilterChange = <K extends keyof GetFilteredProductsParams>(
    key: K,
    value: GetFilteredProductsParams[K]
  ) => {
    setFilterParams((prev) => ({
      ...prev,
      [key]: value,
      PageNumber: 1,
    }));
  };
  const handleMultipleFiltersChange = (
    filters: Partial<GetFilteredProductsParams>
  ) => {
    setFilterParams((prev) => ({
      ...prev,
      ...filters,
      PageNumber: 1,
    }));
  };

  const {
    data,
    isLoading,
    isError,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useInfiniteProductsQuery(filterParams);

  const products = data?.pages.flatMap((page) => page.items) ?? [];
  const totalCount = data?.pages[0]?.totalCount || 0;
  const displayCount = products.length;
  const productListClassName =
    viewMode === "list"
      ? "space-y-4"
      : "grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-4";
  const navigate = useNavigate();
  const searchTermDisplay = filterParams.SearchTerm
    ? `“${filterParams.SearchTerm}”`
    : null;

  const sortLabel = filterParams.SortBy
    ? ProductSortByLabels[filterParams.SortBy]
    : "Produtos";

  const categoryLabel = filterParams.Category
    ? CategoriesLabels[filterParams.Category]
    : null;

  const conditionLabel = filterParams.Condition
    ? ProductConditionLabels[filterParams.Condition]
    : null;
  const noBidsOnlyLabel = filterParams.NoBidsOnly ? "Sem lances" : null;

  return (
    <>
      <Header />
      <div className="min-h-screen">
        <div className="container mx-auto px-4 py-6">
          <div className="mb-4">
            <Button
              variant="link"
              className="p-0 text-gray-600 hover:no-underline cursor-pointer hover:bg-accent"
              onClick={() => navigate("/")}
            >
              <ArrowLeft className="h-4 w-4 mr-2" />
              Voltar para o Início
            </Button>
          </div>

          {/* 2. Layout Principal (Sidebar + Conteúdo) */}
          <div className="flex flex-col lg:flex-row gap-8">
            {/* --- SIDEBAR DE FILTROS --- */}
            <SearchSidebar
              filterParams={filterParams}
              onFilterChange={handleFilterChange}
              onMultipleFiltersChange={handleMultipleFiltersChange}
            />
            {/* --- ÁREA DE CONTEÚDO PRINCIPAL --- */}
            <main className="w-full lg:w-3/4 xl:w-4/5">
              {/* 2a. Cabeçalho dos Resultados (Título, Ordenação, Toggles) */}
              <div className="flex flex-col md:flex-row justify-between items-start md:items-center mb-4 gap-4">
                {/* Título e contagem */}
                <div>
                  <h1 className="text-2xl font-bold text-gray-900">
                    {searchTermDisplay && (
                      <>
                        {searchTermDisplay}
                        {" – "}
                      </>
                    )}
                    {sortLabel}
                    {categoryLabel && (
                      <>
                        {" – "}
                        {categoryLabel}
                      </>
                    )}
                    {conditionLabel && (
                      <>
                        {" – "}
                        {conditionLabel}
                      </>
                    )}
                    {noBidsOnlyLabel && (
                      <>
                        {" – "}
                        {noBidsOnlyLabel}
                      </>
                    )}
                  </h1>
                  <p className="text-sm text-gray-600">
                    Exibindo {displayCount} de {totalCount} resultados
                  </p>
                </div>

                {/* Controles de Ordenação e Visualização */}
                <div className="flex items-center gap-4 flex-shrink-0">
                  {/* Dropdown de Ordenação */}
                  <Select
                    value={filterParams.SortBy}
                    onValueChange={(value) =>
                      handleFilterChange("SortBy", value as ProductSortBy)
                    }
                  >
                    <SelectTrigger className="w-[200px] bg-white">
                      <span className="text-sm text-gray-600 mr-1">
                        Ordenar por:
                      </span>
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      {Object.entries(ProductSortByLabels).map(
                        ([value, label]) => (
                          <SelectItem key={value} value={value}>
                            {label}
                          </SelectItem>
                        )
                      )}
                      <SelectItem value="Oldest">Mais antigos</SelectItem>
                    </SelectContent>
                  </Select>

                  {/* Toggles de Visualização (Lista / Grade) */}
                  <div className="flex">
                    <Button
                      variant={viewMode === "list" ? "default" : "outline"}
                      size="icon"
                      onClick={() => setViewMode("list")}
                      className={
                        viewMode === "list"
                          ? ""
                          : "bg-white text-gray-700 rounded-r-none"
                      }
                    >
                      <List className="h-5 w-5" />
                    </Button>
                    <Button
                      variant={viewMode === "grid" ? "default" : "outline"}
                      size="icon"
                      onClick={() => setViewMode("grid")}
                      className={
                        viewMode === "grid"
                          ? ""
                          : "bg-white text-gray-700 rounded-l-none"
                      }
                    >
                      <Grip className="h-5 w-5" />
                    </Button>
                  </div>
                </div>
              </div>

              {isLoading ? (
                <div className="flex justify-center items-center h-64">
                  <Loader2 className="h-12 w-12 animate-spin text-emerald-700" />
                </div>
              ) : isError ? (
                <div className="text-center text-red-600">
                  Ocorreu um erro ao buscar os produtos.
                </div>
              ) : products.length === 0 ? (
                <NoItemsFound
                  Icon={PackageX}
                  description="Infelizmente ainda não temos produtos para essa busca."
                  title="Nenhum produto foi encontrado"
                />
              ) : (
                <div className={productListClassName}>
                  {products
                    .filter((product): product is Product => product?.listing !== undefined)
                    .map((product) => (
                      <ProductCard
                        key={product.id}
                        product={product}
                        saleType={product.listing?.isAuctionActive ? "auction" : "buyNow"}
                        cardType={viewMode === "list" ? "horizontal" : "vertical"}
                        pageType="search"
                      />
                    ))
                  }
                </div>
              )}

              {hasNextPage && (
                <div className="flex justify-center mt-8">
                  <Button
                    onClick={() => fetchNextPage()}
                    disabled={isFetchingNextPage}
                    variant="outline"
                    className="bg-white"
                  >
                    {isFetchingNextPage ? (
                      <>
                        <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                        Carregando...
                      </>
                    ) : (
                      "Carregar mais"
                    )}
                  </Button>
                </div>
              )}
            </main>
          </div>
        </div>
      </div>
      <Footer />
    </>
  );
}
