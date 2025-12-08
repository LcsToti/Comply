import { formatCurrency } from "@/utils/formatters/formatCurrency";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  ExternalLink,
  AlertCircle,
  ShoppingBag,
  DollarSign,
} from "lucide-react";
import { Link } from "react-router";
import { useState } from "react";
import { PaymentDetailsSheet } from "./PaymentDetailsSheet";

interface PurchasedItemProps {
  status: string;
  statusIcon: React.ReactNode;
  statusColor: string;
  title: string;
  imageUrl: string | undefined;
  price: number;
  saleId?: string;
  productId?: string;
  hasDispute?: boolean;
  isSeller: boolean;
}

const PurchasedItem = ({
  status,
  statusIcon,
  statusColor,
  title,
  imageUrl,
  price,
  saleId,
  productId,
  hasDispute,
  isSeller,
}: PurchasedItemProps) => {
  const [open, setOpen] = useState(false);

  return (
    <>
      <Card className="shadow-none">
        <CardContent>
          <div className="flex items-start gap-4">
            {/* Imagem */}
            <div className="w-20 h-20 rounded-lg overflow-hidden bg-gray-100 flex-shrink-0">
              <img
                src={imageUrl}
                alt={title}
                className="w-full h-full object-cover"
              />
            </div>

            {/* Conteúdo */}
            <div className="flex-1 min-w-0">
              <div className="flex items-start justify-between gap-2 mb-1">
                <h3 className="font-medium text-gray-900 line-clamp-2">
                  {title}
                </h3>
                <div className="flex gap-1 flex-shrink-0">
                  {/* Badge de tipo (Compra/Venda) */}
                  {isSeller ? (
                    <Badge
                      variant="outline"
                      className="bg-blue-50 text-blue-700 border-blue-200"
                    >
                      <DollarSign className="h-3 w-3 mr-1" />
                      Venda
                    </Badge>
                  ) : (
                    <Badge
                      variant="outline"
                      className="bg-emerald-50 text-emerald-700 border-emerald-200"
                    >
                      <ShoppingBag className="h-3 w-3 mr-1" />
                      Compra
                    </Badge>
                  )}
                  {hasDispute && (
                    <Badge variant="destructive">
                      <AlertCircle className="h-3 w-3 mr-1" />
                      Disputa
                    </Badge>
                  )}
                </div>
              </div>

              <div className="flex items-center gap-2 mb-2">
                {statusIcon}
                <span className={`text-sm font-medium ${statusColor}`}>
                  {status}
                </span>
              </div>

              <div className="flex items-center justify-between gap-2 flex-wrap">
                <div>
                  <span
                    className={`text-lg font-semibold ${
                      isSeller ? "text-blue-700" : "text-emerald-700"
                    }`}
                  >
                    {formatCurrency(price)}
                  </span>
                  {isSeller && (
                    <span className="text-xs text-gray-500 ml-2">
                      (Você vendeu)
                    </span>
                  )}
                </div>

                <div className="flex items-center gap-2">
                  {productId && (
                    <Link to={`/product/${productId}`}>
                      <Button
                        className="cursor-pointer"
                        variant="outline"
                        size="sm"
                      >
                        <ExternalLink className="h-4 w-4 mr-1" />
                        Ver Produto
                      </Button>
                    </Link>
                  )}
                  {saleId && (
                    <Button
                      onClick={() => setOpen(true)}
                      variant="default"
                      size="sm"
                      className={`cursor-pointer ${
                        isSeller
                          ? "bg-blue-600 hover:bg-blue-700"
                          : "bg-emerald-700 hover:bg-emerald-800"
                      }`}
                    >
                      Ver Detalhes
                    </Button>
                  )}
                </div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
      <PaymentDetailsSheet
        onOpenChange={setOpen}
        open={open}
        saleId={saleId!}
      />
    </>
  );
};

export default PurchasedItem;
