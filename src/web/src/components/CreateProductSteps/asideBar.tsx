import { cn } from "@/lib/utils";
import { Label } from "@/components/ui/label";
import { CircleArrowLeft, Info, XCircle, Zap } from "lucide-react";
import { useNavigate } from "react-router";
import ResumeCreateProduct from "./resume";
import { Button } from "../ui/button";
import {
  Empty,
  EmptyContent,
  EmptyDescription,
  EmptyHeader,
  EmptyMedia,
  EmptyTitle,
} from "../ui/empty";
import { Spinner } from "../ui/spinner";
import { Progress } from "../ui/progress";
import hammerImg from "@/assets/logo/hammer-white.png";
import { useEffect, useState } from "react";
import { RadioGroup, RadioGroupItem } from "../ui/radio-group";
import type { UseFormReturn } from "react-hook-form";
import type { CreateProductFormValues } from "@/schemas/createProductSchema";
import type { CreateListingFormValues } from "@/schemas/createListingSchema";
import type { CreateAuctionFormValues } from "@/schemas/createAuctionSchema";
import complyIconWhite from "@/assets/logo/comply-icon-white.png";

interface AsideBarProps {
  step: number;
  saleType: "direct" | "comply";
  onChangeStep: (step: number) => void;
  onChangeSaleType: (type: "direct" | "comply") => void;
  totalSteps: number;
  submissionError: string | null;
  submissionSuccess: boolean;
  onAnimationComplete: () => void;
  productForm: UseFormReturn<CreateProductFormValues>;
  listingForm: UseFormReturn<CreateListingFormValues>;
  auctionForm: UseFormReturn<CreateAuctionFormValues>;
}

const sidebarWidths: { [key: number]: string } = {
  1: "lg:w-[40%]",
  2: "lg:w-[45%]",
  3: "lg:w-[50%]",
  4: "lg:w-[60%]",
  5: "lg:w-[85%]",
  6: "lg:w-[100%]",
};

const stepContent = [
  {
    step: 1,
    title: "Informações Básicas",
    description:
      "Descreva mais sobre o produto. Por exemplo: Quanto tempo de uso? Acompanha algo a mais? Tem defeitos?",
  },
  {
    step: 2,
    title: "Adicione imagens ao seu anúncio",
    description:
      "Coloque na ordem que desejar. A primeira será a imagem principal.",
  },
  {
    step: 3,
    title: "Detalhes do produto",
    description:
      "Conte-nos mais sobre o produto. Ajude compradores a encontrar seu produto mais rápido.",
  },
  {
    step: 4,
    title: "Hora da venda",
    description: "Configure seu modelo de venda e leilão.",
  },
  {
    step: 5,
    title: "Resumo",
    description: "Revise todas as informações antes de publicar seu anúncio.",
  },
];

export default function AsideBar({
  step,
  saleType,
  onChangeStep,
  totalSteps,
  onChangeSaleType,
  submissionError,
  submissionSuccess,
  onAnimationComplete,
  productForm,
  listingForm,
  auctionForm,
}: AsideBarProps) {
  const currentContent = stepContent[step - 1];
  const currentWidth = sidebarWidths[step];
  const navigate = useNavigate();

  const [showText, setShowText] = useState(false);
  const [showIcon, setShowIcon] = useState(true);

  useEffect(() => {
    if (step === 6 && submissionSuccess) {
      setShowText(false);
      setShowIcon(true);

      const animationTimer = setTimeout(() => {
        setShowText(true);
        setShowIcon(false);
      }, 1200);

      const redirectTimer = setTimeout(() => {
        onAnimationComplete();
      }, 2500);

      return () => {
        clearTimeout(animationTimer);
        clearTimeout(redirectTimer);
      };
    }
  }, [step, submissionSuccess, onAnimationComplete]);

  const prevStep = () => {
    if (step > 1) {
      onChangeStep(step - 1);
      window.scrollTo({
        top: 0,
        behavior: "smooth",
      });
    }
  };

  return (
    <aside
      className={cn(
        "bg-emerald-700 text-white lg:p-10",
        "lg:h-svh lg:sticky lg:top-0",
        "transition-all duration-600 delay-80 ease-in-out",
        currentWidth
      )}
    >
      {step === 6 ? (
        submissionError ? (
          <Empty className="w-full h-dvh flex-col items-center justify-center p-0">
            <EmptyHeader>
              <EmptyMedia
                variant="icon"
                className="bg-transparent text-red-300"
              >
                <XCircle className="size-12" />
              </EmptyMedia>
              <EmptyTitle className="text-xl">Erro na Submissão</EmptyTitle>
              <EmptyDescription className="text-white text-lg">
                {submissionError}
              </EmptyDescription>
            </EmptyHeader>
            <EmptyContent>
              <Button
                variant="outline"
                className="bg-transparent hover:text-emerald-700 cursor-pointer"
                onClick={prevStep}
                size="sm"
              >
                Voltar e Corrigir
              </Button>
            </EmptyContent>
          </Empty>
        ) : submissionSuccess ? (
          <div className="w-full h-dvh flex flex-col items-center justify-center p-0 relative overflow-hidden">
            <div className="relative z-10 flex flex-col items-center">
              {showIcon &&
                (saleType === "comply" ? (
                  <img
                    src={hammerImg}
                    alt="Martelo"
                    className="w-24 h-24 animate-hammer"
                  />
                ) : (
                  <Zap size={96} className="text-white animate-zap" />
                ))}
              {showText && (
                <h1 className="text-white text-3xl font-bold mt-6 animate-fadeIn">
                  Produto criado!
                </h1>
              )}
            </div>
          </div>
        ) : (
          <Empty className="w-full h-dvh flex-col items-center justify-center p-0">
            <EmptyHeader>
              <EmptyMedia variant="icon" className="bg-transparent text-white">
                <Spinner className="size-12" />
              </EmptyMedia>
              <EmptyTitle className="text-xl">Criando seu produto</EmptyTitle>
              <EmptyDescription className="text-white text-lg">
                Por favor aguarde enquanto tentamos criar o produto. Não
                recarregue a página.
              </EmptyDescription>
            </EmptyHeader>
            <EmptyContent>
              <Button
                variant="outline"
                className="bg-transparent hover:text-emerald-700 cursor-pointer"
                onClick={prevStep}
                size="sm"
              >
                Cancelar
              </Button>
            </EmptyContent>
          </Empty>
        )
      ) : (
        <div className="flex h-full flex-row justify-between p-6">
          <div className="flex flex-col justify-between">
            <div
              className="flex items-center justify-start w-full cursor-pointer gap-12"
              onClick={
                currentContent.step == 1 ? () => navigate("/") : prevStep
              }
            >
              <div className="flex items-center gap-2">
                <CircleArrowLeft className="size-12" />
                {currentContent.step == 1 && (
                  <span className="cursor-pointer font-bold">
                    Página Inicial
                  </span>
                )}
              </div>

              <Progress
                value={(step / totalSteps) * 100}
                className=" bg-emerald-900 lg:hidden"
              />
            </div>
            <div className="p-8">
              <div>
                <h1 className=" text-3xl font-bold leading-tight lg:text-5xl">
                  {currentContent.title}
                </h1>
                <p className="mt-4 text-2xl text-emerald-100">
                  {currentContent.description}
                </p>
              </div>

              {step === 5 && (
                <ResumeCreateProduct
                  onChangeStep={onChangeStep}
                  productForm={productForm}
                  listingForm={listingForm}
                  auctionForm={auctionForm}
                  saleType={saleType}
                />
              )}

              {step === 4 && (
                <div className="mt-10 flex flex-col gap-6">
                  <RadioGroup value={saleType} onValueChange={onChangeSaleType}>
                    <div className="flex items-center gap-3">
                      <RadioGroupItem
                        value="direct"
                        id="sale-direct"
                        className="h-10 w-10 data-[state=checked]:border-white data-[state=checked]:bg-white data-[state=checked]:text-emerald-700"
                      />
                      <Label
                        htmlFor="sale-direct"
                        className="text-xl font-medium text-white cursor-pointer"
                      >
                        Quero apenas vender, sem leilão.
                      </Label>
                    </div>

                    <div className="flex items-center gap-3">
                      <RadioGroupItem
                        value="comply"
                        id="sale-comply"
                        className="h-10 w-10 data-[state=checked]:border-white data-[state=checked]:bg-white data-[state=checked]:text-emerald-700"
                      />
                      <Label
                        htmlFor="sale-comply"
                        className="text-xl font-medium text-white cursor-pointer"
                      >
                        Desejo a experiência Comply, leilão e venda rápida.
                      </Label>
                    </div>
                  </RadioGroup>
                  <div className="mt-4 rounded-lg bg-emerald-900/20 p-4">
                    <h4 className="flex items-center gap-2 font-bold text-white text-xl">
                      <Info className="h-5 w-5" />
                      Como funciona a experiência Comply?
                    </h4>
                    <p className="mt-2 text-lg text-emerald-100">
                      O leilão inicia na data que desejar, depois que começa
                      ainda será possível que alguém compre imediatamente o
                      produto e interrompa o leilão. Dessa forma, garantimos que
                      seu produto seja vendido da forma mais rápida e justa
                      possível.
                    </p>
                  </div>
                </div>
              )}
            </div>
            <div className="mt-10 flex items-center justify-between gap-3">
              <div className="items-center hidden lg:flex">
                <img src={complyIconWhite} />
                <span className="text-lg font-medium">
                  {step === 5
                    ? "Revise suas informações"
                    : `Passo ${step} de ${totalSteps}`}
                </span>
              </div>
            </div>
          </div>
        </div>
      )}
    </aside>
  );
}
