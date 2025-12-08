import {
  Check,
  XCircle,
  Clock,
  AlertTriangle,
  Loader2,
  Package,
  ShoppingCart,
  Store,
} from "lucide-react";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { SaleStatus } from "@/types/sale";
import type { Sale } from "@/types/sale";
import PurchasedItem from "./PurchasedItem";
import PurchasedGroup from "./PurchasedGroup";
import { useAuth } from "@/contexts/AuthContext";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import NoItemsFound from "@/components/NoItemsFound";
import { useProductQuery } from "@/hooks/products/useProductsQueries";
import { useMySalesQuery } from "@/hooks/sales/useSalesQueries";

const groupSalesByDate = (sales: Sale[]) => {
  const groups: Record<string, Sale[]> = {};

  sales.forEach((sale) => {
    const date = new Date(sale.createdAt).toLocaleDateString("pt-BR", {
      day: "numeric",
      month: "long",
    });

    if (!groups[date]) {
      groups[date] = [];
    }
    groups[date].push(sale);
  });

  return Object.entries(groups).sort(([, salesA], [, salesB]) => {
    const dateA = new Date(salesA[0].createdAt);
    const dateB = new Date(salesB[0].createdAt);
    return dateB.getTime() - dateA.getTime();
  });
};

const getStatusDisplay = (status: SaleStatus) => {
  switch (status) {
    case SaleStatus.Done:
      return {
        icon: <Check className="h-4 w-4 text-green-600" />,
        color: "text-green-600",
        label: "Concluída",
      };
    case SaleStatus.AwaitingDelivery:
      return {
        icon: <Clock className="h-4 w-4 text-amber-600" />,
        color: "text-amber-600",
        label: "Aguardando Entrega",
      };
    case SaleStatus.Dispute:
      return {
        icon: <AlertTriangle className="h-4 w-4 text-red-600" />,
        color: "text-red-600",
        label: "Em Disputa",
      };
    case SaleStatus.Cancelled:
      return {
        icon: <XCircle className="h-4 w-4 text-gray-600" />,
        color: "text-gray-600",
        label: "Cancelada",
      };
    default:
      return {
        icon: <Package className="h-4 w-4 text-gray-600" />,
        color: "text-gray-600",
        label: "Desconhecido",
      };
  }
};

const SaleItemWithProduct = ({
  sale,
  isSeller,
}: {
  sale: Sale;
  isSeller: boolean;
}) => {
  const { data: product } = useProductQuery(sale.productId);
  const statusDisplay = getStatusDisplay(sale.status);

  return (
    <PurchasedItem
      status={statusDisplay.label}
      statusIcon={statusDisplay.icon}
      statusColor={statusDisplay.color}
      title={product?.title || `Pedido #${sale.listingId.slice(0, 8)}`}
      price={sale.productValue}
      imageUrl={product?.images[0] || undefined}
      saleId={sale.id}
      productId={sale.productId}
      hasDispute={!!sale.dispute}
      isSeller={isSeller}
    />
  );
};

const PaymentsTab = () => {
  const { user } = useAuth();
  const { data: sales, isLoading, isError, error } = useMySalesQuery();

  const purchases = sales?.filter((sale) => sale.buyerId === user?.id) || [];
  const soldItems = sales?.filter((sale) => sale.sellerId === user?.id) || [];

  return (
    <>
      <header className="mb-8">
        <h1 className="text-4xl font-bold text-gray-900">Pedidos e Vendas</h1>
        <p className="mt-1 text-base text-gray-600">
          Gerencie suas compras e acompanhe suas vendas
        </p>
      </header>

      <Tabs defaultValue="purchases" className="w-full">
        <TabsList className="grid w-full max-w-md grid-cols-2 mb-6">
          <TabsTrigger value="purchases" className="flex items-center gap-2">
            <ShoppingCart className="h-4 w-4" />
            Minhas Compras
            {purchases.length > 0 && (
              <span className="ml-1 rounded-full bg-emerald-700 text-white text-xs px-2 py-0.5">
                {purchases.length}
              </span>
            )}
          </TabsTrigger>
          <TabsTrigger value="sales" className="flex items-center gap-2">
            <Store className="h-4 w-4" />
            Minhas Vendas
            {soldItems.length > 0 && (
              <span className="ml-1 rounded-full bg-blue-600 text-white text-xs px-2 py-0.5">
                {soldItems.length}
              </span>
            )}
          </TabsTrigger>
        </TabsList>

        {/* Tab: Minhas Compras */}
        <TabsContent value="purchases">
          <section>
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-2xl font-semibold text-gray-900">
                Histórico de Compras
              </h2>
              {purchases && (
                <span className="text-sm text-gray-500">
                  {purchases.length}{" "}
                  {purchases.length === 1 ? "compra" : "compras"}
                </span>
              )}
            </div>

            {isLoading && (
              <div className="flex items-center justify-center p-12 bg-gray-50 rounded-lg">
                <Loader2 className="h-8 w-8 animate-spin text-emerald-700" />
                <span className="ml-2 text-gray-600">
                  Carregando compras...
                </span>
              </div>
            )}

            {isError && (
              <Alert variant="destructive" className="mb-4">
                <AlertTriangle className="h-4 w-4" />
                <AlertDescription>
                  Erro ao carregar compras:{" "}
                  {error instanceof Error ? error.message : "Erro desconhecido"}
                </AlertDescription>
              </Alert>
            )}

            {!isLoading && !isError && purchases.length === 0 && (
              <NoItemsFound
                Icon={ShoppingCart}
                description="Suas compras aparecerão aqui quando você adquirir produtos"
                title="Nenhuma compra realizada"
              />
            )}

            {!isLoading && !isError && purchases.length > 0 && (
              <>
                {groupSalesByDate(purchases).map(([date, dateSales]) => (
                  <PurchasedGroup key={date} date={date}>
                    {dateSales.map((sale) => (
                      <SaleItemWithProduct
                        key={sale.id}
                        sale={sale}
                        isSeller={false}
                      />
                    ))}
                  </PurchasedGroup>
                ))}
              </>
            )}
          </section>
        </TabsContent>

        {/* Tab: Minhas Vendas */}
        <TabsContent value="sales">
          <section>
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-2xl font-semibold text-gray-900">
                Histórico de Vendas
              </h2>
              {soldItems && (
                <span className="text-sm text-gray-500">
                  {soldItems.length}{" "}
                  {soldItems.length === 1 ? "venda" : "vendas"}
                </span>
              )}
            </div>

            {isLoading && (
              <div className="flex items-center justify-center p-12 bg-gray-50 rounded-lg">
                <Loader2 className="h-8 w-8 animate-spin text-blue-600" />
                <span className="ml-2 text-gray-600">Carregando vendas...</span>
              </div>
            )}

            {isError && (
              <Alert variant="destructive" className="mb-4">
                <AlertTriangle className="h-4 w-4" />
                <AlertDescription>
                  Erro ao carregar vendas:{" "}
                  {error instanceof Error ? error.message : "Erro desconhecido"}
                </AlertDescription>
              </Alert>
            )}

            {!isLoading && !isError && soldItems.length === 0 && (
              <NoItemsFound
                Icon={Store}
                description="Suas vendas aparecerão aqui quando alguém comprar seus produtos"
                title="Nenhuma venda realizada"
              />
            )}

            {!isLoading && !isError && soldItems.length > 0 && (
              <>
                {groupSalesByDate(soldItems).map(([date, dateSales]) => (
                  <PurchasedGroup key={date} date={date}>
                    {dateSales.map((sale) => (
                      <SaleItemWithProduct
                        key={sale.id}
                        sale={sale}
                        isSeller={true}
                      />
                    ))}
                  </PurchasedGroup>
                ))}
              </>
            )}
          </section>
        </TabsContent>
      </Tabs>
    </>
  );
};

export default PaymentsTab;
