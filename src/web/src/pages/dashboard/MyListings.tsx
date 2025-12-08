import Footer from "@/components/Footer";
import Header from "@/components/Header";
import NoItemsFound from "@/components/NoItemsFound";
import ProductCard from "@/components/ProductCard/ProductCard";
import { SellerAccountStatusBanner } from "@/components/SellerAccountStatusBanner";
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
import { useOnboardingLinkQuery } from "@/hooks/payments/usePaymentAccountsQueries";
import { usePaginatedProductsQuery } from "@/hooks/products/useProductsQueries";
import { PaymentAccountStatus } from "@/types/paymentAccount";
import type { ProductPaginatedList, Product } from "@/types/product";
import { ArchiveX, LogIn, PackageX } from "lucide-react";
import { useMemo, useState } from "react";
import { Link } from "react-router";

export default function MyListings() {
  const TAB_KEYS = [
    "todos",
    "ativos",
    "pausados",
    "vendidos",
    "vendidosEmLeilao",
    "leiloesAtivos",
  ] as const;
  type TabKey = (typeof TAB_KEYS)[number];
  const tabs = useMemo(
    () => [
      {
        key: "todos" as TabKey,
        label: "Todos os anúncios",
      },
      {
        key: "ativos" as TabKey,
        label: "Ativos",
      },
      {
        key: "pausados" as TabKey,
        label: "Pausados",
      },
      {
        key: "vendidos" as TabKey,
        label: "Vendidos",
      },
      {
        key: "vendidosEmLeilao" as TabKey,
        label: "Vendidos em leilão",
      },
      {
        key: "leiloesAtivos" as TabKey,
        label: "Leilões ativos",
      },
    ],
    []
  );

  const { isLoggedIn, user, accountStatus, isLoadingUser } = useAuth();

  const [activeTab, setActiveTab] = useState<TabKey>("todos");
  const [page, setPage] = useState(1);

  const filterParams = useMemo(() => {
    const baseParams = {
      PageNumber: page,
      PageSize: 10,
      SellerId: user?.id,
    };

    switch (activeTab) {
      case "ativos":
        return { ...baseParams, ListingStatus: "Available" as const };
      case "pausados":
        return { ...baseParams, ListingStatus: "Paused" as const };
      case "vendidos":
        return { ...baseParams, ListingStatus: "Sold" as const };
      case "vendidosEmLeilao":
        return { ...baseParams, ListingStatus: "SoldByAuction" as const };
      case "leiloesAtivos":
        return { ...baseParams, IsAuctionActive: true };
      case "todos":
      default:
        return baseParams;
    }
  }, [activeTab, page, user?.id]);

  const {
    data: productsData,
    isLoading: isLoadingProducts,
    isError,
    isFetching: isFetchingProducts,
  } = usePaginatedProductsQuery(filterParams);
  const { data: onboardingLink } = useOnboardingLinkQuery(
    isLoggedIn && !isLoadingUser
  );

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
  const handleTabChange = (tabKey: TabKey) => {
    setActiveTab(tabKey);
    setPage(1);
  };
  const renderContent = () => {
    if (isLoadingProducts || !productsData) {
      return (
        <div className="flex flex-col justify-center items-center gap-4">
          <Skeleton className="w-full h-9" />
          <Skeleton className="w-full h-32 md:h-48" />
          <Skeleton className="w-full h-32 md:h-48" />
          <Skeleton className="w-full h-32 md:h-48" />
        </div>
      );
    }
    if (isError) {
      return (
        <div className="text-red-500 text-center p-4">
          Ocorreu um erro ao buscar seus anúncios.
        </div>
      );
    }

    const { items, totalPages, totalCount } =
      productsData as ProductPaginatedList<Product>;

    if (accountStatus && accountStatus != PaymentAccountStatus.Active) {
      return (
        <SellerAccountStatusBanner
          status={accountStatus}
          onboardingLink={onboardingLink}
          className="mt-10 mb-5"
        />
      );
    }

    if (totalCount === 0 && activeTab === "todos") {
      return (
        <Empty className="from-muted/50 to-background from-30%">
          <EmptyHeader>
            <EmptyMedia variant="icon">
              <ArchiveX />
            </EmptyMedia>
            <EmptyTitle>Você não tem produtos criados.</EmptyTitle>
            <EmptyDescription>
              Que tal começar a vender seus itens usados ou novos agora mesmo?
            </EmptyDescription>
          </EmptyHeader>
          <EmptyContent>
            <Link to={"/product/create"}>
              <Button
                size={"lg"}
                className="cursor-pointer bg-emerald-700 hover:bg-emerald-800"
              >
                Criar meu primeiro anúncio
              </Button>
            </Link>
          </EmptyContent>
        </Empty>
      );
    }

    return (
      <div className="mt-4 md:mt-6">
        <div
          className={`transition-opacity ${isFetchingProducts ? "opacity-50" : "opacity-100"}`}
        >
          {items.length > 0 ? (
            <div className="flex flex-col gap-3">
              {items.map((product) => (
                <ProductCard
                  key={product.id}
                  product={product}
                  pageType="myAds"
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
        {!isLoggedIn && (
          <Empty className="from-muted/50 to-background from-30% justify-start">
            <EmptyHeader>
              <EmptyMedia variant="icon">
                <LogIn />
              </EmptyMedia>
              <EmptyTitle>Faça login para acessar seus anúncios</EmptyTitle>
              <EmptyDescription>
                Entre na sua conta para visualizar, criar e gerenciar seus
                anúncios.
              </EmptyDescription>
            </EmptyHeader>
            <EmptyContent>
              <div className="flex flex-col sm:flex-row gap-2">
                <Link to={"/login"}>
                  <Button className="w-full sm:w-auto cursor-pointer bg-emerald-700 hover:bg-emerald-800">
                    Entrar
                  </Button>
                </Link>
                <Link to={"/register"}>
                  <Button
                    variant="outline"
                    className="w-full sm:w-auto cursor-pointer"
                  >
                    Criar conta
                  </Button>
                </Link>
              </div>
            </EmptyContent>
          </Empty>
        )}
        {isLoggedIn && (
          <div>
            <h1 className="text-2xl sm:text-3xl lg:text-4xl font-bold">
              Meus anúncios
            </h1>

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
                onValueChange={(value: TabKey) => handleTabChange(value)}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Filtrar anúncios..." />
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

            <div className="flex-1 justify-center">{renderContent()}</div>
          </div>
        )}
      </main>
      <Footer />
    </>
  );
}
