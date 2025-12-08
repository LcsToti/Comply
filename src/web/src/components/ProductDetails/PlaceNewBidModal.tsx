import { useState, useMemo } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { X, BanknoteArrowUp, Plus, CreditCard } from "lucide-react";
import type { PaymentMethod } from "@/types/paymentAccount";
import VisaIcon from "@/assets/creditcards/visa.svg";
import MastercardIcon from "@/assets/creditcards/mastercard.svg";
import PixIcon from "@/assets/creditcards/pix.svg";
import AmexIcon from "@/assets/creditcards/amex.svg";
import DiscoverIcon from "@/assets/creditcards/discover.svg";
import PaypalIcon from "@/assets/creditcards/paypal.svg";
import GenericCard from "@/assets/creditcards/generic.svg";
import CreatePaymentMethod from "../CreatePaymentMethod";
import { usePaymentMethodsQuery } from "@/hooks/payments/usePaymentAccountsQueries";
import hammerWhiteImg from "@/assets/logo/hammer-white.png";

interface PlaceNewBidModalProps {
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
  minBid: number;
  maxBid?: number;
  onConfirm?: (amount: number, paymentMethodId: string) => void;
}

export default function PlaceNewBidModal({
  open,
  onOpenChange,
  minBid,
  maxBid,
  onConfirm,
}: PlaceNewBidModalProps) {
  const brandIcons: Record<string, string> = {
    visa: VisaIcon,
    americanexpress: AmexIcon,
    discover: DiscoverIcon,
    mastercard: MastercardIcon,
    pix: PixIcon,
    paypal: PaypalIcon,
    default: GenericCard,
  };

  const [amount, setAmount] = useState<number>(minBid);
  const [loading, setLoading] = useState(false);
  const [confirmationText, setConfirmationText] = useState("");
  const [selectedMethod, setSelectedMethod] = useState<string | null>(null);
  const [showCreatePayment, setShowCreatePayment] = useState(false);

  const { data: paymentMethodsData, refetch: refetchPaymentMethods } =
    usePaymentMethodsQuery();

  const getIcon = (method: PaymentMethod) => {
    if (method.type === "pix") return brandIcons.pix;
    if (method.type === "paypal") return brandIcons.paypal;
    if (method.type && brandIcons[method.brand.toLowerCase()])
      return brandIcons[method.brand.toLowerCase()];
    return brandIcons.default;
  };

  const presets = useMemo(
    () => [minBid, Math.round(minBid * 1.1), Math.round(minBid * 1.2)],
    [minBid]
  );

  function applyPreset(v: number) {
    const limited = maxBid ? Math.min(v, maxBid) : v;
    setAmount(limited);
  }

  async function handleConfirm() {
    if (!selectedMethod) return;
    if (amount < minBid) return setAmount(minBid);
    if (maxBid && amount > maxBid) return setAmount(maxBid);

    setLoading(true);
    try {
      await new Promise((r) => setTimeout(r, 600));
      onConfirm?.(amount, selectedMethod);
      onOpenChange?.(false);
    } finally {
      setLoading(false);
      setConfirmationText("");
    }
  }

  const handlePaymentMethodCreated = () => {
    setShowCreatePayment(false);
    refetchPaymentMethods();
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogTrigger asChild>
        <Button
          size="lg"
          className="w-full bg-emerald-600 hover:bg-emerald-700 cursor-pointer"
        >
          <img src={hammerWhiteImg} className="w-4 h-4" />
          Dar lance
        </Button>
      </DialogTrigger>

      <DialogContent
        showCloseButton={false}
        className="w-full max-w-sm p-4 max-h-[90vh] overflow-y-auto"
      >
        <DialogHeader>
          <div className="flex items-center justify-between">
            <DialogTitle className="text-lg font-semibold text-emerald-800">
              Lance rápido
            </DialogTitle>
            <Button
              variant="ghost"
              size="icon"
              className="text-emerald-700"
              onClick={() => {
                onOpenChange?.(false);
                setShowCreatePayment(false);
              }}
            >
              <X />
            </Button>
          </div>
        </DialogHeader>

        {showCreatePayment ? (
          <div className="py-4">
            <div className="flex items-center gap-2 mb-4">
              <Button
                variant="ghost"
                size="sm"
                onClick={() => setShowCreatePayment(false)}
                className="text-emerald-700"
              >
                ← Voltar
              </Button>
            </div>
            <CreatePaymentMethod onSuccess={handlePaymentMethodCreated} />
          </div>
        ) : (
          <Card className="border-none shadow-none">
            <CardContent>
              <Label className="text-sm text-slate-500">
                Valor do lance (R$)
              </Label>
              <div className="mt-2 flex gap-2">
                <Input
                  type="number"
                  min={minBid}
                  max={maxBid}
                  step={1}
                  value={String(amount)}
                  onChange={(e) => {
                    const v = Number(e.target.value);
                    if (maxBid) setAmount(Math.min(v, maxBid));
                    else setAmount(v);
                  }}
                  className="w-full bg-white"
                  aria-label="Valor do lance"
                />
                <Button
                  className="flex items-center gap-2 px-3 cursor-pointer border-emerald-700 hover:bg-emerald-50"
                  onClick={() =>
                    setAmount((v) => {
                      const newValue = Math.round(v * 1.03);
                      return maxBid ? Math.min(newValue, maxBid) : newValue;
                    })
                  }
                  aria-label="Aumentar lance em 3%"
                  variant="outline"
                >
                  <BanknoteArrowUp
                    size={20}
                    className="w-4 h-4 text-emerald-700"
                  />
                </Button>
              </div>

              <div className="mt-3 grid grid-cols-3 gap-2">
                {presets.map((p) => (
                  <button
                    key={p}
                    onClick={() => applyPreset(p)}
                    className={`rounded-md border p-2 text-sm font-medium ${
                      p === amount
                        ? "border-emerald-700 bg-emerald-50 text-emerald-800"
                        : "border-emerald-200 text-emerald-700 hover:bg-emerald-50"
                    }`}
                  >
                    R$ {p}
                  </button>
                ))}
              </div>

              <div className="mt-4">
                <div className="flex items-center justify-between mb-2">
                  <Label className="text-sm text-slate-500">Pagamento</Label>
                  {paymentMethodsData && paymentMethodsData.length > 0 && (
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => setShowCreatePayment(true)}
                      className="h-auto p-1 text-xs text-emerald-700 hover:text-emerald-800"
                    >
                      <Plus size={14} className="mr-1" />
                      Adicionar
                    </Button>
                  )}
                </div>

                {!paymentMethodsData ? (
                  <div className="flex items-center justify-center p-4 bg-gray-50 rounded-lg">
                    <p className="text-gray-500 text-sm italic">
                      Carregando formas de pagamento...
                    </p>
                  </div>
                ) : paymentMethodsData.length === 0 ? (
                  <div className="flex flex-col items-center gap-3 p-6 bg-gray-50 border-2 border-dashed border-gray-300 rounded-lg">
                    <CreditCard className="h-10 w-10 text-gray-400" />
                    <div className="text-center">
                      <p className="text-gray-700 font-medium text-sm mb-1">
                        Nenhum método de pagamento
                      </p>
                      <p className="text-gray-500 text-xs mb-3">
                        Adicione um cartão ou PIX para dar lances
                      </p>
                    </div>
                    <Button
                      size="sm"
                      onClick={() => setShowCreatePayment(true)}
                      className="bg-emerald-600 hover:bg-emerald-700 text-white"
                    >
                      <Plus size={16} className="mr-1" />
                      Adicionar método
                    </Button>
                  </div>
                ) : (
                  <div className="mt-2 space-y-2">
                    {paymentMethodsData.map((method: PaymentMethod) => {
                      const isSelected = selectedMethod === method.id;

                      return (
                        <button
                          key={method.id}
                          onClick={() => setSelectedMethod(method.id)}
                          className={`w-full flex items-center justify-between gap-2 p-2 rounded-lg border transition-all ${
                            isSelected
                              ? "border-emerald-600 bg-emerald-50"
                              : "border-slate-200 hover:bg-slate-50"
                          }`}
                        >
                          <div className="flex items-center gap-2">
                            <img
                              src={getIcon(method)}
                              alt={method.brand || method.type}
                              className="w-7 h-7 object-contain p-1"
                            />
                            <div className="text-left">
                              <div className="text-sm font-medium text-emerald-800">
                                {method.type === "pix"
                                  ? "PIX"
                                  : `${method.brand?.toUpperCase()} •••• ${method.last4}`}
                              </div>
                            </div>
                          </div>

                          <div
                            className={`w-4 h-4 rounded-full border-2 ${
                              isSelected
                                ? "border-emerald-600 bg-emerald-600"
                                : "border-slate-300"
                            }`}
                          />
                        </button>
                      );
                    })}
                  </div>
                )}
              </div>

              <div className="mt-6 flex items-center justify-between">
                <div>
                  <div className="text-xs text-slate-400">Total</div>
                  <div className="text-lg font-semibold text-emerald-800">
                    R$ {amount}
                  </div>
                </div>
              </div>

              <div className="mt-5">
                <Label className="text-sm text-slate-500">
                  Confirmação de segurança
                </Label>
                <Input
                  placeholder='Digite "CONFIRMAR" para prosseguir'
                  value={confirmationText}
                  onChange={(e) => setConfirmationText(e.target.value)}
                  className="mt-2 bg-white"
                />
              </div>

              <Button
                className="mt-4 w-full bg-emerald-700 hover:bg-emerald-800 text-white"
                onClick={handleConfirm}
                disabled={
                  loading ||
                  !selectedMethod ||
                  confirmationText.trim().toUpperCase() !== "CONFIRMAR"
                }
              >
                {loading ? "Processando..." : "Confirmar & Pagar"}
              </Button>
            </CardContent>
          </Card>
        )}
      </DialogContent>
    </Dialog>
  );
}
