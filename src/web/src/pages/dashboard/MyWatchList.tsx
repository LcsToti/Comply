import Footer from "@/components/Footer";
import Header from "@/components/Header";
import NoItemsFound from "@/components/NoItemsFound";
import ProductCard from "@/components/ProductCard/ProductCard";
import { Button } from "@/components/ui/button";
import {
  Empty,
  EmptyHeader,
  EmptyMedia,
  EmptyTitle,
  EmptyDescription,
  EmptyContent,
} from "@/components/ui/empty";
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationPrevious,
  PaginationLink,
  PaginationEllipsis,
  PaginationNext,
} from "@/components/ui/pagination";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { Toggle } from "@/components/ui/toggle";
import { useAuth } from "@/contexts/AuthContext";
import { useWatchlistQuery } from "@/hooks/notifications/useWatchListQueries";
import {
  useMyBoughtProductsQuery,
  useMyOutbiddedProductsQuery,
  useMyWinningProductsQuery,
  useProductsByIdsQuery,
} from "@/hooks/products/useProductsQueries";
import type { ProductPaginatedList, Product } from "@/types/product";
import { ArchiveX, LogIn, PackageX } from "lucide-react";
import { useMemo, useState } from "react";
import { Link } from "react-router";

export default function MyWatchList() {
  const TAB_KEYS = ["deOlho", "superados", "vencendo", "comprados"] as const;
  type TabKey = (typeof TAB_KEYS)[number];

  const { isLoggedIn } = useAuth();
  const [activeTab, setActiveTab] = useState<TabKey>("deOlho");
  const [page, setPage] = useState(1);

  const buildPagination = (
    currentPage: number,
    totalPages: number,
    onPageChange: (page: number) => void
  ) => {
    const pages = [];
    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) pages.push(i);
    } else {
      pages.push(1);
      if (currentPage > 3) pages.push("...");
      if (currentPage > 2) pages.push(currentPage - 1);
      if (currentPage !== 1 && currentPage !== totalPages)
        pages.push(currentPage);
      if (currentPage < totalPages - 1) pages.push(currentPage + 1);
      if (currentPage < totalPages - 2) pages.push("...");
      pages.push(totalPages);
    }

    return (
      <PaginationContent>
        <PaginationItem>
          <PaginationPrevious
            className={
              currentPage === 1
                ? "opacity-50 cursor-not-allowed"
                : "cursor-pointer"
            }
            onClick={() => currentPage > 1 && onPageChange(currentPage - 1)}
          />
        </PaginationItem>
        {pages.map((page, index) => (
          <PaginationItem key={index}>
            {page === "..." ? (
              <PaginationEllipsis />
            ) : (
              <PaginationLink
                className="cursor-pointer"
                isActive={currentPage === page}
                onClick={() => onPageChange(page as number)}
              >
                {page}
              </PaginationLink>
            )}
          </PaginationItem>
        ))}
        <PaginationItem>
          <PaginationNext
            className={
              currentPage === totalPages
                ? "opacity-50 cursor-not-allowed"
                : "cursor-pointer"
            }
            onClick={() =>
              currentPage < totalPages && onPageChange(currentPage + 1)
            }
          />
        </PaginationItem>
      </PaginationContent>
    );
  };

  const pageSize = 10;

  const {
    data: watchlistIds = [],
    isLoading: isLoadingIds,
    isError: isErrorIds,
  } = useWatchlistQuery();

  const hasIds = watchlistIds && watchlistIds.length > 0;

  const {
    data: watchlistProductsData,
    isLoading: isLoadingWatchlistProducts,
    isError: isErrorWatchlistProducts,
    isFetching: isFetchingWatchlistProducts,
  } = useProductsByIdsQuery(
    activeTab === "deOlho" && hasIds ? watchlistIds : [],
    page,
    pageSize
  );

  const {
    data: outbiddedData,
    isLoading: isLoadingOutbidded,
    isError: isErrorOutbidded,
    isFetching: isFetchingOutbidded,
  } = useMyOutbiddedProductsQuery(page, pageSize);

  const {
    data: winningData,
    isLoading: isLoadingWinning,
    isError: isErrorWinning,
    isFetching: isFetchingWinning,
  } = useMyWinningProductsQuery(page, pageSize);

  const {
    data: boughtData,
    isLoading: isLoadingBought,
    isError: isErrorBought,
    isFetching: isFetchingBought,
  } = useMyBoughtProductsQuery(page, pageSize);

  const tabs = useMemo(
    () => [
      {
        key: "deOlho" as TabKey,
        label: "De Olho",
      },
      {
        key: "superados" as TabKey,
        label: "Superados",
      },
      {
        key: "vencendo" as TabKey,
        label: "Vencendo",
      },
      {
        key: "comprados" as TabKey,
        label: "Comprados",
      },
    ],
    []
  );

  const handleTabChange = (tabKey: TabKey) => {
    setActiveTab(tabKey);
    setPage(1);
  };

  let currentData: ProductPaginatedList<Product> | undefined;
  let isLoadingCurrent = false;
  let isErrorCurrent = false;
  let isFetchingCurrent = false;

  if (activeTab === "deOlho") {
    isLoadingCurrent = isLoadingIds || isLoadingWatchlistProducts;
    isErrorCurrent = isErrorIds || isErrorWatchlistProducts;
    isFetchingCurrent = isFetchingWatchlistProducts;
    currentData = watchlistProductsData as
      | ProductPaginatedList<Product>
      | undefined;
  }
  if (activeTab === "superados") {
    isLoadingCurrent = isLoadingOutbidded;
    isErrorCurrent = isErrorOutbidded;
    isFetchingCurrent = isFetchingOutbidded;
    const currentOutbidded = outbiddedData as
      | ProductPaginatedList<Product>
      | undefined;

    currentData = currentOutbidded
      ? {
          ...currentOutbidded,
          items: currentOutbidded.items.filter(
            (i) => i.listing.isAuctionActive
          ),
        }
      : undefined;
  }
  if (activeTab === "vencendo") {
    isLoadingCurrent = isLoadingWinning;
    isErrorCurrent = isErrorWinning;
    isFetchingCurrent = isFetchingWinning;
    currentData = winningData as ProductPaginatedList<Product> | undefined;
  }
  if (activeTab === "comprados") {
    isLoadingCurrent = isLoadingBought;
    isErrorCurrent = isErrorBought;
    isFetchingCurrent = isFetchingBought;
    currentData = boughtData as ProductPaginatedList<Product> | undefined;
  }

  const renderContent = () => {
    if (isLoadingCurrent) {
      return (
        <div className="flex flex-col justify-center items-center gap-4">
          <Skeleton className="w-full h-9" />
          <Skeleton className="w-full h-32 md:h-48" />
          <Skeleton className="w-full h-32 md:h-48" />
          <Skeleton className="w-full h-32 md:h-48" />
        </div>
      );
    }

    if (isErrorCurrent) {
      return (
        <div className="text-red-500 text-center p-4">
          Erro ao buscar seus produtos.
        </div>
      );
    }

    if (activeTab === "deOlho" && !hasIds) {
      return (
        <Empty className="from-muted/50 to-background from-30%">
          <EmptyHeader>
            <EmptyMedia variant="icon">
              <ArchiveX />
            </EmptyMedia>
            <EmptyTitle>Você não está de olho em nenhum produto.</EmptyTitle>
            <EmptyDescription>
              Encontre produtos e adicione-os à sua lista para acompanhar.
            </EmptyDescription>
          </EmptyHeader>
          <EmptyContent>
            <Link to={"/search"}>
              <Button size="lg" className="bg-emerald-700 hover:bg-emerald-800">
                Buscar produtos
              </Button>
            </Link>
          </EmptyContent>
        </Empty>
      );
    }

    if (!currentData) {
      return (
        <div className="text-center text-neutral-500 p-4">
          Carregando produtos...
        </div>
      );
    }

    const { items, totalPages } = currentData;

    return (
      <div className="mt-4 md:mt-6">
        <div
          className={`transition-opacity ${
            isFetchingCurrent ? "opacity-50" : "opacity-100"
          }`}
        >
          {items.length > 0 ? (
            <div className="flex flex-col gap-3">
              {items.map((product) => (
                <ProductCard
                  key={product.id}
                  product={product}
                  pageType="myList"
                  cardType="horizontal"
                  saleType={product.listing?.auction ? "auction" : "buyNow"}
                />
              ))}
            </div>
          ) : (
            <NoItemsFound
              Icon={PackageX}
              description="Não foi encontrados produtos para esta categoria"
              title="Não há produtos"
            />
          )}
        </div>

        {items.length > 0 && totalPages > 1 && (
          <Pagination className="mt-6">
            {buildPagination(page, totalPages, setPage)}
          </Pagination>
        )}
      </div>
    );
  };

  return (
    <>
      <Header />
      <main className="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 min-h-dvh my-6 md:my-10">
        {!isLoggedIn ? (
          <Empty className="from-muted/50 to-background from-30% justify-start">
            <EmptyHeader>
              <EmptyMedia variant="icon">
                <LogIn />
              </EmptyMedia>
              <EmptyTitle>
                Faça login para acessar sua lista de observação
              </EmptyTitle>
              <EmptyDescription>
                Entre na sua conta para ver os produtos que está acompanhando.
              </EmptyDescription>
            </EmptyHeader>
            <EmptyContent>
              <div className="flex flex-col sm:flex-row gap-2">
                <Link to={"/login"}>
                  <Button className="w-full sm:w-auto bg-emerald-700 hover:bg-emerald-800">
                    Entrar
                  </Button>
                </Link>
                <Link to={"/register"}>
                  <Button variant="outline" className="w-full sm:w-auto">
                    Criar conta
                  </Button>
                </Link>
              </div>
            </EmptyContent>
          </Empty>
        ) : (
          <div>
            <h1 className="text-2xl sm:text-3xl lg:text-4xl font-bold">
              Minha lista
            </h1>

            {!isLoadingCurrent && (
              <>
                {/* Tabs para desktop (lg e acima) */}
                <div className="mt-4 md:mt-6 bg-neutral-100 rounded-lg p-1 hidden lg:block">
                  <div className="flex flex-row items-center gap-1 overflow-x-auto">
                    {tabs.map((tab) => (
                      <Toggle
                        key={tab.key}
                        pressed={activeTab === tab.key}
                        onPressedChange={() => handleTabChange(tab.key)}
                        className="cursor-pointer flex bg-transparent flex-row data-[state=on]:shadow data-[state=on]:bg-white gap-2 px-3 py-2 rounded-md whitespace-nowrap"
                      >
                        <span className="text-sm">{tab.label}</span>
                      </Toggle>
                    ))}
                  </div>
                </div>

                {/* Select para mobile e tablet (abaixo de lg) */}
                <div className="mt-4 md:mt-6 lg:hidden">
                  <Select
                    value={activeTab}
                    onValueChange={(v: TabKey) => handleTabChange(v)}
                  >
                    <SelectTrigger className="w-full">
                      <SelectValue placeholder="Filtrar produtos..." />
                    </SelectTrigger>
                    <SelectContent>
                      {tabs.map((tab) => (
                        <SelectItem key={tab.key} value={tab.key}>
                          {tab.label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </>
            )}

            <div className="flex-1 justify-center">{renderContent()}</div>
          </div>
        )}
      </main>
      <Footer />
    </>
  );
}
