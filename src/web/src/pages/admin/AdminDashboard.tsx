import {
  Users,
  Package,
  Gavel,
  CreditCard,
  TrendingUp,
  ShoppingCart,
  Activity,
  TicketIcon,
} from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { useAuth } from "@/contexts/AuthContext";
import { useState } from "react";
import { TicketStatus, type Ticket } from "@/types/ticket";
import type { Sale } from "@/types/sale";
import { useUsersCountQuery } from "@/hooks/user/useUsersQueries";
import {
  useActiveAuctionsCountQuery,
  useProductsCountQuery,
} from "@/hooks/products/useProductsQueries";
import { useLastSuccessfulPaymentsQuery } from "@/hooks/payments/usePaymentsQueries";
import { useAllTicketsQuery } from "@/hooks/notifications/useTicketsQueries";
import { useAllSalesQuery } from "@/hooks/sales/useSalesQueries";
export default function AdminDashboard() {
  const { user } = useAuth();
  const [amount] = useState(25);

  const { data: usersCountData = 0 } = useUsersCountQuery();
  const { data: productsCountData = 0 } = useProductsCountQuery();
  const { data: activeAuctionsCountData = 0 } = useActiveAuctionsCountQuery();
  const { data: lastSuccessfulPaymentsData = 0 } =
    useLastSuccessfulPaymentsQuery(amount);
  const { data: allTicketsData = [] } = useAllTicketsQuery();
  const { data: allSalesData = [] } = useAllSalesQuery();

  const statsCards = [
    {
      title: "Total de Usuários",
      value: usersCountData ?? 0,
      icon: Users,
      color: "text-blue-600",
      bgColor: "bg-blue-50",
      description: "Usuários cadastrados",
    },
    {
      title: "Produtos Listados",
      value: productsCountData ?? 0,
      icon: Package,
      color: "text-emerald-600",
      bgColor: "bg-emerald-50",
      description: "Total no sistema, incluindo os já vendidos",
    },
    {
      title: "Leilões Ativos",
      value: activeAuctionsCountData ?? 0,
      icon: Gavel,
      color: "text-purple-600",
      bgColor: "bg-purple-50",
      description: "Em andamento",
    },
    {
      title: "Pagamentos Recentes",
      value: lastSuccessfulPaymentsData ?? 0,
      icon: CreditCard,
      color: "text-green-600",
      bgColor: "bg-green-50",
      description: "Transações concluídas",
    },
  ];

  const openTickets = allTicketsData.filter(
    (t: Ticket) => t.status === TicketStatus.Open
  ).length;

  const resolvedPercentage =
    allTicketsData.length === 0
      ? 0
      : (
          ((allTicketsData.length - openTickets) / allTicketsData.length) *
          100
        ).toFixed(0);

  const pendingSales = allSalesData.filter((s: Sale) => s.status === 0).length;

  return (
    <div className="flex-1 flex flex-col overflow-hidden w-full">
      <main className="flex-1 overflow-y-auto p-4 lg:p-8">
        {/* Header Section */}
        <div className="mb-8">
          <h2 className="text-xl lg:text-3xl font-bold text-gray-900 mb-2">
            Bem-vindo, {user?.name}
          </h2>
          <p className="text-gray-600 text-sm lg:text-base">
            Visão geral da plataforma e métricas principais
          </p>
        </div>

        {/* Main Stats Grid */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 lg:gap-6 mb-8">
          {statsCards.map((stat, index) => {
            const Icon = stat.icon;
            return (
              <Card key={index} className="border-1 shadow-none">
                <CardHeader className="flex flex-row items-center justify-between pb-2">
                  <CardTitle className="text-sm font-medium text-gray-600">
                    {stat.title}
                  </CardTitle>
                  <div className={`${stat.bgColor} p-2.5 rounded-lg`}>
                    <Icon className={`h-5 w-5 ${stat.color}`} />
                  </div>
                </CardHeader>
                <CardContent>
                  <div className="flex flex-col">
                    <span className="text-3xl font-bold text-gray-900 mb-1">
                      {stat.value.toLocaleString()}
                    </span>
                    <div className="flex items-center justify-between">
                      <span className="text-xs text-gray-500 flex items-center gap-1">
                        <Activity className="h-3 w-3" />
                        {stat.description}
                      </span>
                    </div>
                  </div>
                </CardContent>
              </Card>
            );
          })}
        </div>

        {/* Secondary Info Grid */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-4 lg:gap-6 mb-8">
          {/* Tickets Card */}
          <Card className="border-1 shadow-none">
            <CardHeader>
              <CardTitle className="text-base font-semibold flex items-center gap-2">
                <TicketIcon className="h-5 w-5 text-orange-600" />
                Tickets de Suporte
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-3xl font-bold text-gray-900">
                    {allTicketsData.length}
                  </p>
                  <p className="text-sm text-gray-500 mt-1">Total de tickets</p>
                </div>
                {openTickets > 0 && (
                  <Badge variant="destructive" className="text-xs">
                    {openTickets} abertos
                  </Badge>
                )}
              </div>
            </CardContent>
          </Card>
          {/* Sales Card */}
          <Card className="border-1 shadow-none">
            <CardHeader>
              <CardTitle className="text-base font-semibold flex items-center gap-2">
                <ShoppingCart className="h-5 w-5 text-emerald-600" />
                Vendas Totais
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-3xl font-bold text-gray-900">
                    {allSalesData.length}
                  </p>
                  <p className="text-sm text-gray-500 mt-1">
                    Vendas realizadas
                  </p>
                </div>
                {pendingSales > 0 && (
                  <Badge
                    variant="secondary"
                    className="text-xs bg-yellow-100 text-yellow-800"
                  >
                    {pendingSales} pendentes
                  </Badge>
                )}
              </div>
            </CardContent>
          </Card>
          {/* Activity Summary */}
          <Card className="border-0 shadow-none bg-gradient-to-br from-emerald-50 to-emerald-100">
            <CardHeader>
              <CardTitle className="text-base font-semibold flex items-center gap-2">
                <TrendingUp className="h-5 w-5 text-emerald-700" />
                Status da Plataforma
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                <div className="flex items-center justify-between">
                  <span className="text-sm text-gray-700">
                    Leilões/Produtos
                  </span>
                  <span className="text-sm font-semibold text-emerald-700">
                    {activeAuctionsCountData}/{productsCountData}
                  </span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-sm text-gray-700">
                    Taxa de conversão
                  </span>
                  <span className="text-sm font-semibold text-emerald-700">
                    {activeAuctionsCountData &&
                      productsCountData &&
                      (
                        (activeAuctionsCountData / productsCountData) *
                        100
                      ).toFixed(1)}
                    %
                  </span>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Quick Stats Bar */}
        <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
          <div className="bg-white rounded-lg border-1 p-4 shadow-none">
            <p className="text-xs text-gray-500 mb-1">Média usuários/produto</p>
            <p className="text-xl font-bold text-gray-900">
              {(usersCountData / productsCountData).toFixed(2)}
            </p>
          </div>
          <div className="bg-white rounded-lg border-1 p-4 shadow-none">
            <p className="text-xs text-gray-500 mb-1">Taxa pagamentos</p>
            <p className="text-xl font-bold text-gray-900">
              {(
                (lastSuccessfulPaymentsData / allSalesData.length) *
                100
              ).toFixed(0)}
              %
            </p>
          </div>
          <div className="bg-white rounded-lg border-1 p-4 shadow-none">
            <p className="text-xs text-gray-500 mb-1">Tickets resolvidos</p>
            <p className="text-xl font-bold text-gray-900">
              {resolvedPercentage}%
            </p>
          </div>
          <div className="bg-white rounded-lg border-1 p-4 shadow-none">
            <p className="text-xs text-gray-500 mb-1">Taxa de vendas</p>
            <p className="text-xl font-bold text-gray-900">
              {((allSalesData.length / productsCountData) * 100).toFixed(0)}%
            </p>
          </div>
        </div>
      </main>
    </div>
  );
}
