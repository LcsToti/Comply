import ComplyBanner from "@/components/ComplyBanner";
import Footer from "@/components/Footer";
import Header from "@/components/Header";
import ProductFilters from "@/components/ProductQuickFilters";
import ProductSection, {
  type ProductSectionFilterParams,
} from "@/components/ProductsSection";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { CategoriesLabels } from "@/types/product";
import { Gavel, Box, Target, Shapes } from "lucide-react";
import { useState } from "react";
import { Link } from "react-router";

function HomePage() {
  const [open, setOpen] = useState(false);

  const trendingFilterParams: ProductSectionFilterParams = {
    PageSize: 8,
    SortBy: "Popularity",
    ListingStatus: "Available",
  };

  const endingSoonFilterParams: ProductSectionFilterParams = {
    PageSize: 8,
    SortBy: "AuctionEndingSoon",
    ListingStatus: "Available",
    IsAuctionActive: true,
  };

  return (
    <div className="flex flex-col min-h-screen w-full">
      <Header />
      <main className="flex-grow">
        <ComplyBanner />

        {/* Barra de navegação sticky */}
        <div className="w-full sticky z-20 top-16 px-4 sm:px-5 py-3 bg-emerald-700">
          <div className="max-w-7xl mx-auto flex justify-center gap-3 md:gap-5">
            {/* Desktop: Botões de filtro rápido */}
            <div className="flex items-center gap-3 md:gap-5 overflow-x-auto scrollbar-hide">
              {/* Botões de ordenação */}
              {[
                { label: "Popularity", value: "Mais popular" },
                { label: "MostBids", value: "Mais lances" },
              ].map((sort) => (
                <Link
                  key={sort.label}
                  to={`/search?sort=${encodeURIComponent(sort.label)}`}
                  className="text-xs md:text-sm rounded-lg flex-shrink-0 cursor-pointer text-white bg-emerald-600 hover:bg-emerald-500 transition-all py-1.5 md:py-2 px-3 md:px-4 font-medium"
                >
                  {sort.value}
                </Link>
              ))}

              {Object.entries(CategoriesLabels).map(([value, label]) => (
                <Link
                  key={value}
                  to={`/search?category=${encodeURIComponent(value)}`}
                  className="hidden 2xl:flex text-sm rounded-lg flex-shrink-0 cursor-pointer text-white hover:underline py-2 px-3 transition font-medium"
                >
                  {label}
                </Link>
              ))}
            </div>

            {/* Mobile/Tablet: Dropdown de categorias */}
            <div className="flex 2xl:hidden">
              <DropdownMenu open={open} modal={false}>
                <DropdownMenuTrigger
                  onMouseEnter={() => setOpen(true)}
                  onMouseLeave={() => setOpen(false)}
                  className="text-xs md:text-sm rounded-lg flex-shrink-0 cursor-pointer text-white bg-emerald-600 hover:bg-emerald-500 transition-all py-1.5 md:py-2 px-3 md:px-4 font-medium"
                >
                  Categorias
                </DropdownMenuTrigger>

                <DropdownMenuContent
                  onMouseEnter={() => setOpen(true)}
                  onMouseLeave={() => setOpen(false)}
                  className="w-48 bg-white text-black"
                >
                  <DropdownMenuLabel>Categorias</DropdownMenuLabel>
                  <DropdownMenuSeparator />
                  {Object.entries(CategoriesLabels).map(([value, label]) => (
                    <DropdownMenuItem key={label}>
                      <Link
                        to={`/search?category=${encodeURIComponent(label)}`}
                        className="w-full block"
                      >
                        {value}
                      </Link>
                    </DropdownMenuItem>
                  ))}
                </DropdownMenuContent>
              </DropdownMenu>
            </div>
          </div>
        </div>

        {/* Seção: Produtos em alta */}
        <div className="mt-8 md:mt-12">
          <ProductSection
            title="Produtos em alta"
            subtitle="Veja o que outros estão mais buscando hoje"
            icon="flame"
            seeMoreHref="/search?sort=Popularity"
            filterParams={trendingFilterParams}
          />
        </div>

        {/* Seção: Leilões acabando em breve */}
        <ProductSection
          title="Leilões acabando em breve"
          subtitle="Não perca a chance de dar um lance e ganhar"
          icon="clock"
          seeMoreHref="/search?isAuction=true"
          filterParams={endingSoonFilterParams}
        />

        {/* Seção: Como funciona */}
        <section className="w-full bg-emerald-100 py-12 md:py-16">
          <div className="max-w-6xl mx-auto px-4 sm:px-5 text-center">
            <h2 className="text-md md:text-3xl font-semibold mb-6 md:mb-8 text-emerald-900">
              Novo por aqui? Aprenda como funciona o leilão
            </h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6 md:gap-8">
              {/* Etapa 1 */}
              <div className="flex flex-col items-center">
                <Box size={40} className="md:size-12 mb-4 text-emerald-900" />
                <h3 className="text-md md:text-xl font-bold mb-2 text-emerald-700">
                  Escolha um item
                </h3>
                <p className="text-xs md:text-base text-emerald-700">
                  Encontre o produto que deseja e dê seu lance ou compre
                  imediatamente pelo valor "Compre Já".
                </p>
              </div>
              {/* Etapa 2 */}
              <div className="flex flex-col items-center">
                <Gavel size={40} className="md:size-12 mb-4 text-emerald-900" />
                <h3 className="text-md md:text-xl font-bold mb-2 text-emerald-700">
                  Dê seu lance
                </h3>
                <p className="text-xs md:text-base text-emerald-700">
                  Digite o valor que deseja ofertar. Se alguém cobrir o lance,
                  você terá chance de dar outro.
                </p>
              </div>
              {/* Etapa 3 */}
              <div className="flex flex-col items-center">
                <Target
                  size={40}
                  className="md:size-12 mb-4 text-emerald-900"
                />
                <h3 className="text-md md:text-xl font-bold mb-2 text-emerald-700">
                  Acompanhe o leilão
                </h3>
                <p className="text-xs md:text-base text-emerald-700">
                  Fique de olho no tempo e nos lances. O leilão encerra assim
                  que o tempo zerar.
                </p>
              </div>
            </div>
            <Link
              to="/faq"
              className="mt-6 md:mt-8 inline-block text-sm md:text-base text-emerald-900 font-semibold border-2 border-emerald-700 rounded-full px-5 md:px-6 py-2 hover:bg-emerald-700 hover:text-white transition-colors"
            >
              Tenho mais dúvidas
            </Link>
          </div>
        </section>

        {/* Seção: Filtros rápidos */}
        <section className="w-full max-w-6xl mx-auto px-4 sm:px-5 py-8 md:py-12">
          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
            <div className="flex flex-row items-center gap-3">
              <Shapes className="md:size-12 text-emerald-700" />
              <div>
                <h2 className="text-md md:text-2xl font-bold">
                  Filtros rápidos
                </h2>
                <h3 className="text-xs md:text-sm font-light text-neutral-600">
                  Facilitamos alguns filtros recorrentes para você!
                </h3>
              </div>
            </div>
          </div>
          <ProductFilters />
        </section>
      </main>
      <Footer />
    </div>
  );
}

export default HomePage;
