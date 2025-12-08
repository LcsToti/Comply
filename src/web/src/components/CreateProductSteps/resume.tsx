import { Label } from "../ui/label";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { CircleDollarSign, Info, Pencil } from "lucide-react";
import { Popover, PopoverContent, PopoverTrigger } from "../ui/popover";
import { formatCurrency } from "@/utils/formatters/formatCurrency";
import type { CreateAuctionFormValues } from "@/schemas/createAuctionSchema";
import type { CreateListingFormValues } from "@/schemas/createListingSchema";
import type { CreateProductFormValues } from "@/schemas/createProductSchema";
import type { UseFormReturn } from "react-hook-form";

interface ResumeCreateProductProps {
  onChangeStep: (step: number) => void;
  productForm: UseFormReturn<CreateProductFormValues>;
  listingForm: UseFormReturn<CreateListingFormValues>;
  auctionForm: UseFormReturn<CreateAuctionFormValues>;
  saleType: "direct" | "comply";
}

export default function ResumeCreateProduct({
  onChangeStep,
  productForm,
  listingForm,
  auctionForm,
  saleType,
}: ResumeCreateProductProps) {
  const { watch: listingWatch } = listingForm;
  const { watch: productWatch } = productForm;
  const { watch: auctionWatch } = auctionForm;

  const title = productWatch("Title");
  const condition = productWatch("Condition");
  const category = productWatch("Category");
  const characteristics = productWatch("Characteristics");
  const firstImage = productWatch("ImageUrls")?.[0];
  const firstImageUrl =
    firstImage instanceof File ? URL.createObjectURL(firstImage) : firstImage;
  const buyPrice = listingWatch("BuyPrice");
  const startBidValue = auctionWatch("StartBidValue");
  const startDate = auctionWatch("StartDate");
  const endDate = auctionWatch("EndDate");

  const getConditionLabel = (value: string) => {
    if (value === "New") return "Novo";
    if (value === "Used") return "Usado";
    if (value === "NotWorking") return "Não funciona";
    if (value === "Refurbished") return "Recondicionado";
    return "N/A";
  };

  const getCategoryLabel = (value: string) => {
    switch (value) {
      case "Electronics":
        return "Eletrônicos";
      case "Computers":
        return "Computadores";
      case "HomeAppliances":
        return "Eletrodomésticos";
      case "FurnitureDecor":
        return "Móveis e Decoração";
      case "FashionBeauty":
        return "Moda e Beleza";
      case "Sports":
        return "Esportes";
      case "Collectibles":
        return "Colecionáveis";
      case "Tools":
        return "Ferramentas";
      case "Games":
        return "Jogos";
      case "Services":
        return "Serviços";
      case "Others":
        return "Outros";
      default:
        return "N/A";
    }
  };

  return (
    <div className="w-full text-white mt-10">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 lg:gap-6">
        <div className="flex flex-col w-full h-[350px] md:h-[420px] justify-between md:col-span-2 lg:col-span-1 lg:row-span-2">
          <img
            src={firstImageUrl}
            alt="Imagem de capa"
            className="aspect-square h-full rounded-lg object-cover"
          />
          <div className="mt-2 flex items-center justify-between">
            <Badge className="bg-emerald-100 text-emerald-800">
              Imagem de capa
            </Badge>
            <Button
              variant="link"
              onClick={() => onChangeStep(2)}
              className="h-auto p-0 text-white"
            >
              Editar
            </Button>
          </div>
        </div>

        <div className="flex items-start justify-between gap-6 mt-6 lg:mt-0 md:col-span-2 lg:col-span-2">
          <h2 className="text-2xl font-bold lg:text-3xl">{title}</h2>
          <Button
            variant="ghost"
            size="icon"
            onClick={() => onChangeStep(1)}
            className="text-white hover:bg-emerald-500"
          >
            <Pencil className="h-5 w-5" />
          </Button>
        </div>
        <div className="flex flex-col justify-start gap-6 lg:col-span-1">
          <div className="flex items-center justify-between">
            <div className="flex gap-2 items-center">
              <Info />
              <h3 className="text-xl font-semibold">Informações</h3>
            </div>

            <Button
              variant="ghost"
              size="icon"
              onClick={() => onChangeStep(3)}
              className="text-white hover:bg-emerald-500"
            >
              <Pencil className="h-5 w-5" />
            </Button>
          </div>

          <div className="rounded-md bg-emerald-700">
            <div className="grid grid-cols-2 gap-2">
              <div className="bg-white/10 rounded-xl p-4">
                <Label className="text-sm font-bold text-emerald-200">
                  Condição
                </Label>
                <p className="text-md">{getConditionLabel(condition)}</p>
              </div>

              <div className="bg-white/10 rounded-xl p-4">
                <Label className="text-sm font-bold text-emerald-200">
                  Categoria
                </Label>
                <p className="text-md capitalize truncate">
                  {getCategoryLabel(category)}
                </p>
              </div>
            </div>

            <div className="mt-4">
              <div className="flex justify-between">
                <Label className="text-sm font-bold text-emerald-200">
                  Características
                </Label>
                {characteristics.length > 6 && (
                  <Popover>
                    <PopoverTrigger asChild>
                      <Button
                        variant="link"
                        className="text-emerald-100 p-0 h-auto justify-start font-medium hover:text-white"
                      >
                        + Ver mais {characteristics.length - 6}
                      </Button>
                    </PopoverTrigger>
                    <PopoverContent className="bg-emerald-800 text-white border-emerald-700 max-h-60 overflow-y-auto">
                      <div className="flex flex-col gap-3">
                        <h4 className="font-bold">Mais características</h4>
                        {characteristics.slice(6).map((char) => (
                          <div key={char.key} className="flex flex-col text-sm">
                            <span className="font-medium text-emerald-200">
                              {char.key}:
                            </span>
                            <span className="text-emerald-100">
                              {char.value || "N/A"}
                            </span>
                          </div>
                        ))}
                      </div>
                    </PopoverContent>
                  </Popover>
                )}
              </div>

              <div className="mt-2 grid grid-cols-1 sm:grid-cols-2 gap-2 bg-white/10 rounded-xl p-4">
                {characteristics.slice(0, 6).map((char) => (
                  <div key={char.key} className="flex flex-col justify-between">
                    <span className="font-medium">{char.key}:</span>
                    <span className="text-emerald-100 truncate">
                      {char.value || "N/A"}
                    </span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>

        <div className="flex flex-col justify-start gap-6 lg:col-span-1">
          <div className="flex items-center justify-between">
            <div className="flex gap-2 items-center">
              <CircleDollarSign />
              <h3 className="text-xl font-semibold">Valores</h3>
            </div>

            <Button
              variant="ghost"
              size="icon"
              onClick={() => onChangeStep(4)}
              className="text-white hover:bg-emerald-500"
            >
              <Pencil className="h-5 w-5" />
            </Button>
          </div>

          <div className="grid grid-cols-1 gap-2 rounded-md bg-emerald-700">
            <div className="bg-white/10 rounded-xl p-4 flex justify-between">
              <Label className="text-sm font-bold text-emerald-200">
                {saleType && "Compra Imediata"}
              </Label>
              <p className="text-md">{formatCurrency(buyPrice)}</p>
            </div>

            {saleType === "comply" && (
              <>
                <div className="bg-white/10 rounded-xl p-4 flex justify-between">
                  <Label className="text-sm font-bold text-emerald-200">
                    Lance inicial
                  </Label>
                  <p className="text-md">{formatCurrency(startBidValue)}</p>
                </div>

                <div className="bg-white/10 rounded-xl p-4">
                  <Label className="text-sm font-bold text-emerald-200">
                    O leilão inicia em:
                  </Label>
                  <p className="text-md">
                    {" "}
                    {startDate.toLocaleString("pt-BR", {
                      weekday: "short",
                      day: "2-digit",
                      month: "long",
                      year: "numeric",
                      hour: "2-digit",
                      minute: "2-digit",
                    })}
                  </p>
                </div>

                <div className="bg-white/10 rounded-xl p-4">
                  <Label className="text-sm font-bold text-emerald-200">
                    Termina em:
                  </Label>
                  <p className="text-md">
                    {" "}
                    {endDate.toLocaleString("pt-BR", {
                      weekday: "short",
                      day: "2-digit",
                      month: "long",
                      year: "numeric",
                      hour: "2-digit",
                      minute: "2-digit",
                    })}
                  </p>
                </div>
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
