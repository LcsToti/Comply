import { AlertCircle } from "lucide-react";
import { DateTimePicker } from "../DateTimePicker";
import { Alert, AlertTitle, AlertDescription } from "../ui/alert";
import {
  InputGroup,
  InputGroupAddon,
  InputGroupText,
  InputGroupInput,
} from "../ui/input-group";
import { Label } from "../ui/label";
import { useEffect, useState } from "react";
import { Button } from "../ui/button";
import { Controller, type UseFormReturn } from "react-hook-form";
import type { CreateListingFormValues } from "@/schemas/createListingSchema";
import type { CreateAuctionFormValues } from "@/schemas/createAuctionSchema";
import { formatCurrency } from "@/utils/formatters/formatCurrency";

function FieldError({ message }: { message?: string }) {
  if (!message) return null;
  return <p className="text-sm text-red-600 mt-1">{message}</p>;
}

interface Step4Props {
  saleType: string;
  listingForm: UseFormReturn<CreateListingFormValues>;
  auctionForm: UseFormReturn<CreateAuctionFormValues>;
}

export default function Step4({
  saleType,
  listingForm,
  auctionForm,
}: Step4Props) {
  const {
    control: listingControl,
    watch: listingWatch,
    formState: { errors: listingErrors },
  } = listingForm;

  const {
    control: auctionControl,
    setValue: auctionSetValue,
    formState: { errors: auctionErrors },
  } = auctionForm;

  const [selectedSuggestion, setSelectedSuggestion] = useState("balanced");

  const watchedBuyPrice = listingWatch("BuyPrice");
  const sellingPriceValue =
    typeof watchedBuyPrice === "number" && !isNaN(watchedBuyPrice)
      ? watchedBuyPrice
      : 0;

  useEffect(() => {
    auctionSetValue("WinBidValue", sellingPriceValue);
  }, [sellingPriceValue, auctionSetValue]);

  const receivedAmount = sellingPriceValue * 0.92;

  return (
    <div className="flex flex-col itens-center justify-center gap-6 min-h-full">
      <div className="grid items-center w-full gap-2.5">
        <Label
          htmlFor="sellingPrice"
          className="text-emerald-700 font-bold text-2xl"
        >
          Preço de venda:
        </Label>
        <div className="relative">
          <Controller
            name="BuyPrice"
            control={listingControl}
            render={({ field }) => (
              <InputGroup>
                <InputGroupAddon>
                  <InputGroupText>R$</InputGroupText>
                </InputGroupAddon>
                <InputGroupInput
                  {...field}
                  id="sellingPrice"
                  type="number"
                  placeholder="0.00"
                  onChange={(e) => field.onChange(e.target.valueAsNumber)}
                  value={field.value ?? ""}
                />
              </InputGroup>
            )}
          />
        </div>
        <FieldError message={listingErrors.BuyPrice?.message} />
      </div>
      <Alert className=" bg-blue-500">
        <AlertCircle color="white" />
        <AlertTitle className="font-bold text-white text-lg">
          Toda venda possui uma taxa de 8%.
        </AlertTitle>
        <AlertDescription className="text-white text-md">
          Caso venda pelo valor de venda, vai receber:
          <span className="font-bold text-white">
            {formatCurrency(receivedAmount)}
          </span>
        </AlertDescription>
      </Alert>
      {saleType === "comply" && (
        <div className="flex flex-col gap-6">
          <div className="flex flex-col gap-2">
            <Label className="text-emerald-700 font-bold text-2xl">
              Escolha o valor inicial do leilão:
            </Label>
            <Button
              type="button"
              variant={selectedSuggestion === "fast" ? "default" : "outline"}
              onClick={() => {
                auctionSetValue(
                  "StartBidValue",
                  Math.round(sellingPriceValue * 0.25 * 100) / 100
                );
                setSelectedSuggestion("fast");
              }}
              className={`py-6 text-base text-gray-700 ${
                selectedSuggestion === "fast"
                  ? "bg-emerald-600 hover:bg-emerald-700 text-white"
                  : ""
              }`}
            >
              Quer vender rápido? Comece com 20% a 30% do valor de mercado.
            </Button>
            <Button
              type="button"
              variant={
                selectedSuggestion === "balanced" ? "default" : "outline"
              }
              onClick={() => {
                auctionSetValue(
                  "StartBidValue",
                  Math.round(sellingPriceValue * 0.5 * 100) / 100
                );
                setSelectedSuggestion("balanced");
              }}
              className={`py-6 text-base text-gray-700 ${
                selectedSuggestion === "balanced"
                  ? "bg-emerald-600 hover:bg-emerald-700 text-white"
                  : ""
              }`}
            >
              Quer mais equilibrado? Comece com cerca de 50% do valor.
            </Button>
            <Button
              type="button"
              variant={selectedSuggestion === "safe" ? "default" : "outline"}
              onClick={() => {
                auctionSetValue(
                  "StartBidValue",
                  Math.round(sellingPriceValue * 0.7 * 100) / 100
                );
                setSelectedSuggestion("safe");
              }}
              className={`py-6 text-base text-gray-700 ${
                selectedSuggestion === "safe"
                  ? "bg-emerald-600 hover:bg-emerald-700 text-white"
                  : ""
              }`}
            >
              Prefere ir com segurança? Comece com 70% do valor.
            </Button>
          </div>
          <div className="grid w-full gap-2.5">
            <Label
              htmlFor="auctionStartPrice"
              className="text-emerald-700 font-bold text-2xl"
            >
              Valor inicial do leilão:
            </Label>
            <Controller
              name="StartBidValue"
              control={auctionControl}
              render={({ field }) => (
                <InputGroup>
                  <InputGroupAddon>
                    <InputGroupText>R$</InputGroupText>
                  </InputGroupAddon>
                  <InputGroupInput
                    {...field}
                    id="StartBidValue"
                    type="number"
                    placeholder="0.00"
                    onChange={(e) => field.onChange(e.target.valueAsNumber)}
                    value={field.value ?? ""}
                  />
                </InputGroup>
              )}
            />
            <FieldError message={auctionErrors.StartBidValue?.message} />
          </div>
          <div className="flex flex-col gap-3">
            <div className="grid grid-cols-[1fr_auto_auto] items-center gap-2">
              <Label className="text-emerald-700 font-bold text-lg">
                Data de inicio:
              </Label>
              <Controller
                name="StartDate"
                control={auctionControl}
                render={({ field }) => (
                  <DateTimePicker
                    value={field.value}
                    onChange={field.onChange}
                  />
                )}
              />
            </div>
            <FieldError message={auctionErrors.StartDate?.message} />
            <div className="grid grid-cols-[1fr_auto_auto] items-center gap-2">
              <Label className="text-emerald-700 font-bold text-lg">
                Data de termino:
              </Label>
              <Controller
                name="EndDate"
                control={auctionControl}
                render={({ field }) => (
                  <DateTimePicker
                    value={field.value}
                    onChange={field.onChange}
                  />
                )}
              />
            </div>
            <FieldError message={auctionErrors.EndDate?.message} />
          </div>
        </div>
      )}
    </div>
  );
}
