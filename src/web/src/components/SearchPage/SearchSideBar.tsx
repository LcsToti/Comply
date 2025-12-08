import { SearchIcon, X, Filter } from "lucide-react";
import { Input } from "../ui/input";
import { Button } from "../ui/button";
import { Checkbox } from "../ui/checkbox";
import { Label } from "../ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "../ui/select";
import { Separator } from "../ui/separator";
import { Badge } from "../ui/badge";
import type { GetFilteredProductsParams } from "@/api/products/products";
import { useState } from "react";
import {
  CategoriesLabels,
  ProductCondition,
  ProductConditionLabels,
  type Categories,
  type ProductSortBy,
} from "@/types/product";

const CLEAR_VALUE = "___CLEAR___";

const sortButtons: { label: string; value: ProductSortBy }[] = [
  { label: "Mais recentes", value: "Newest" },
  { label: "Mais antigos", value: "Oldest" },
  { label: "Mais populares", value: "Popularity" },
  { label: "Menor preço", value: "PriceAsc" },
  { label: "Maior preço", value: "PriceDesc" },
];

const auctionSortButtons: { label: string; value: ProductSortBy }[] = [
  { label: "Finalizando", value: "AuctionEndingSoon" },
  { label: "Começando em breve", value: "AuctionStartingSoon" },
  { label: "Mais lances", value: "MostBids" },
  { label: "Menos lances", value: "LessBids" },
];

interface SearchSidebarProps {
  filterParams: GetFilteredProductsParams;
  onFilterChange: <K extends keyof GetFilteredProductsParams>(
    key: K,
    value: GetFilteredProductsParams[K]
  ) => void;
  onMultipleFiltersChange: (
    filters: Partial<GetFilteredProductsParams>
  ) => void;
}

function SearchSidebar({
  filterParams,
  onFilterChange,
  onMultipleFiltersChange,
}: SearchSidebarProps) {
  const [minPrice, setMinPrice] = useState(
    filterParams.MinPrice?.toString() || ""
  );
  const [maxPrice, setMaxPrice] = useState(
    filterParams.MaxPrice?.toString() || ""
  );
  const [minBidPrice, setMinBidPrice] = useState("");
  const [maxBidPrice, setMaxBidPrice] = useState("");

  const activeFiltersCount = [
    filterParams.Category,
    filterParams.Condition,
    filterParams.MinPrice,
    filterParams.MaxPrice,
    filterParams.IsAuctionActive,
    filterParams.NoBidsOnly,
  ].filter(Boolean).length;

  const handlePriceApply = () => {
    onMultipleFiltersChange({
      MinPrice: minPrice ? Number(minPrice) : undefined,
      MaxPrice: maxPrice ? Number(maxPrice) : undefined,
      PageNumber: 1,
    });
  };

  const handleBidPriceApply = () => {
    console.log("Filtro de lance:", { minBidPrice, maxBidPrice });
  };

  const handleSortByClick = (value: ProductSortBy) => {
    onFilterChange("SortBy", value);
    onFilterChange("PageNumber", 1);
  };

  const handleAuctionToggle = (checked: boolean) => {
    onFilterChange("IsAuctionActive", checked);
    onFilterChange("PageNumber", 1);
  };

  const handleNoBidsToggle = (checked: boolean) => {
    onFilterChange("NoBidsOnly", checked);
    onFilterChange("PageNumber", 1);
  };

  const handleClearFilters = () => {
    setMinPrice("");
    setMaxPrice("");
    setMinBidPrice("");
    setMaxBidPrice("");
    onMultipleFiltersChange({
      Category: undefined,
      Condition: undefined,
      MinPrice: undefined,
      MaxPrice: undefined,
      IsAuctionActive: undefined,
      NoBidsOnly: undefined,
      PageNumber: 1,
      ListingStatus: "Available",
    });
  };

  return (
    <aside className="w-full lg:w-1/4 xl:w-1/5 space-y-6 p-4 rounded-lg">
      {/* Cabeçalho com contador de filtros */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <Filter className="h-5 w-5 text-gray-700" />
          <h2 className="text-lg font-bold text-gray-800">Filtros</h2>
          {activeFiltersCount > 0 && (
            <Badge variant="default" className="bg-emerald-700">
              {activeFiltersCount}
            </Badge>
          )}
        </div>
        {activeFiltersCount > 0 && (
          <Button
            variant="ghost"
            size="sm"
            onClick={handleClearFilters}
            className="text-xs text-red-600 hover:text-red-700 hover:bg-red-50"
          >
            Limpar
          </Button>
        )}
      </div>

      <Separator />

      {/* Categoria */}
      <div>
        <h3 className="font-semibold mb-2 text-gray-800">Categoria</h3>
        <Select
          value={filterParams.Category || CLEAR_VALUE}
          onValueChange={(value) => {
            if (value === CLEAR_VALUE) {
              onFilterChange("Category", undefined);
            } else {
              onFilterChange("Category", value as Categories);
            }
            onFilterChange("PageNumber", 1);
          }}
        >
          <SelectTrigger className="bg-white">
            <SelectValue placeholder="Todas as categorias" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value={CLEAR_VALUE}>Todas as categorias</SelectItem>
            {Object.entries(CategoriesLabels).map(([value, label]) => (
              <SelectItem key={value} value={value}>
                {label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      {/* Condição */}
      <div>
        <h3 className="font-semibold mb-2 text-gray-800">Condição</h3>
        <Select
          value={filterParams.Condition || CLEAR_VALUE}
          onValueChange={(value) => {
            if (value === CLEAR_VALUE) {
              onFilterChange("Condition", undefined);
            } else {
              onFilterChange("Condition", value as ProductCondition);
            }
            onFilterChange("PageNumber", 1);
          }}
        >
          <SelectTrigger className="bg-white">
            <SelectValue placeholder="Todas as condições" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value={CLEAR_VALUE}>Todas as condições</SelectItem>
            {Object.entries(ProductConditionLabels).map(([value, label]) => (
              <SelectItem key={value} value={value}>
                {label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <Separator />

      {/* Preço de venda */}
      <div>
        <h3 className="font-semibold mb-2 text-gray-800">Preço de venda</h3>
        <div className="flex gap-2">
          <Input
            placeholder="Min."
            className="bg-white"
            type="number"
            value={minPrice}
            onChange={(e) => setMinPrice(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handlePriceApply()}
          />
          <Input
            placeholder="Max."
            className="bg-white"
            type="number"
            value={maxPrice}
            onChange={(e) => setMaxPrice(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handlePriceApply()}
          />
          <Button
            variant="outline"
            size="icon"
            className="bg-white flex-shrink-0 hover:bg-emerald-50"
            onClick={handlePriceApply}
          >
            <SearchIcon className="h-4 w-4" />
          </Button>
        </div>
        {(filterParams.MinPrice || filterParams.MaxPrice) && (
          <div className="flex items-center gap-2 mt-2">
            <Badge variant="secondary" className="text-xs">
              {filterParams.MinPrice && `Min: R$ ${filterParams.MinPrice}`}
              {filterParams.MinPrice && filterParams.MaxPrice && " - "}
              {filterParams.MaxPrice && `Max: R$ ${filterParams.MaxPrice}`}
            </Badge>
            <Button
              variant="ghost"
              size="sm"
              className="h-5 w-5 p-0"
              onClick={() => {
                setMinPrice("");
                setMaxPrice("");
                onMultipleFiltersChange({
                  MinPrice: undefined,
                  MaxPrice: undefined,
                  PageNumber: 1,
                });
              }}
            >
              <X className="h-3 w-3" />
            </Button>
          </div>
        )}
      </div>

      <Separator />

      {/* Ordenar produtos por */}
      <div>
        <h3 className="font-semibold mb-2 text-gray-800">
          Ordenar produtos por
        </h3>
        <div className="flex flex-wrap gap-2">
          {sortButtons.map((b) => (
            <Button
              key={b.value}
              variant={filterParams.SortBy === b.value ? "default" : "outline"}
              size="sm"
              className={`${
                filterParams.SortBy === b.value
                  ? "bg-emerald-700 hover:bg-emerald-800"
                  : "bg-white hover:bg-gray-50"
              }`}
              onClick={() => handleSortByClick(b.value)}
            >
              {b.label}
            </Button>
          ))}
        </div>
      </div>

      <Separator />

      {/* Filtros de Leilão */}
      <div className="space-y-4">
        <div className="flex items-center space-x-2">
          <Checkbox
            id="leiloes"
            checked={!!filterParams.IsAuctionActive}
            onCheckedChange={handleAuctionToggle}
          />
          <Label
            htmlFor="leiloes"
            className="font-semibold text-gray-800 cursor-pointer"
          >
            Apenas Leilões Ativos
          </Label>
        </div>

        <div className="flex items-center space-x-2">
          <Checkbox
            id="no-bids"
            checked={!!filterParams.NoBidsOnly}
            onCheckedChange={handleNoBidsToggle}
          />
          <Label
            htmlFor="no-bids"
            className="font-semibold text-gray-800 cursor-pointer"
          >
            Apenas Sem Lances
          </Label>
        </div>

        {/* Ordenar leilões por */}
        {filterParams.IsAuctionActive && (
          <>
            <h3 className="font-semibold mb-2 text-gray-800">
              Ordenar leilões por
            </h3>
            <div className="flex flex-wrap gap-2">
              {auctionSortButtons.map((btn) => (
                <Button
                  key={btn.value}
                  variant={
                    filterParams.SortBy === btn.value ? "default" : "outline"
                  }
                  size="sm"
                  className={`${
                    filterParams.SortBy === btn.value
                      ? "bg-emerald-700 hover:bg-emerald-800"
                      : "bg-white hover:bg-gray-50"
                  }`}
                  onClick={() => handleSortByClick(btn.value)}
                >
                  {btn.label}
                </Button>
              ))}
            </div>
          </>
        )}

        {/* Filtro de preço de lance (futuro) */}
        {filterParams.IsAuctionActive && (
          <div>
            <h3 className="font-semibold mb-2 text-gray-800">
              Valor atual do lance
            </h3>
            <div className="flex gap-2">
              <Input
                placeholder="Min."
                className="bg-white"
                type="number"
                value={minBidPrice}
                onChange={(e) => setMinBidPrice(e.target.value)}
                onKeyDown={(e) => e.key === "Enter" && handleBidPriceApply()}
              />
              <Input
                placeholder="Max."
                className="bg-white"
                type="number"
                value={maxBidPrice}
                onChange={(e) => setMaxBidPrice(e.target.value)}
                onKeyDown={(e) => e.key === "Enter" && handleBidPriceApply()}
              />
              <Button
                variant="outline"
                size="icon"
                className="bg-white flex-shrink-0 hover:bg-emerald-50"
                onClick={handleBidPriceApply}
              >
                <SearchIcon className="h-4 w-4" />
              </Button>
            </div>
            <p className="text-xs text-amber-600 mt-1">
              ⚠️ Funcionalidade em desenvolvimento
            </p>
          </div>
        )}
      </div>
    </aside>
  );
}

export default SearchSidebar;
