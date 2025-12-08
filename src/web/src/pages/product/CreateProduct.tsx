import { Button } from "@/components/ui/button";
import { useEffect, useState } from "react";
import Step1 from "@/components/CreateProductSteps/step1";
import Step2 from "@/components/CreateProductSteps/step2";
import Step3 from "@/components/CreateProductSteps/step3";
import Step4 from "@/components/CreateProductSteps/step4";
import AsideBar from "@/components/CreateProductSteps/asideBar";
import {
  createProductSchema,
  type CreateProductFormValues,
} from "@/schemas/createProductSchema";
import { useForm, FormProvider, type Resolver } from "react-hook-form";
import type { CreateProductParams } from "@/api/products/products";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  createListingSchema,
  type CreateListingFormValues,
} from "@/schemas/createListingSchema";
import {
  createAuctionSchema,
  type CreateAuctionFormValues,
} from "@/schemas/createAuctionSchema";
import { useNavigate } from "react-router";
import { isAxiosError } from "axios";
import { useAuth } from "@/contexts/AuthContext";
import { PaymentAccountStatus } from "@/types/paymentAccount";
import { useCreateProductMutation } from "@/hooks/products/useProductsMutations";
import { useCreateListingMutation } from "@/hooks/listings/useListingsMutations";
import { useCreateAuctionMutation } from "@/hooks/listings/useAuctionsMutations";

export default function CreateProduct() {
  const navigate = useNavigate();
  const { accountStatus, isLoadingAccountStatus } = useAuth();

  const [step, setStep] = useState(1);
  const totalSteps = 6;
  const [saleType, setSaleType] = useState<"direct" | "comply">("comply");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submissionError, setSubmissionError] = useState<string | null>(null);
  const [submissionSuccess, setSubmissionSuccess] = useState(false);

  const { mutateAsync: createProduct } = useCreateProductMutation();
  const { mutateAsync: createListing } = useCreateListingMutation();
  const { mutateAsync: createAuction } = useCreateAuctionMutation();

  useEffect(() => {
    if (step !== 6) {
      setSubmissionError(null);
      setSubmissionSuccess(false);
    }
  }, [step]);
  useEffect(() => {
    if (isLoadingAccountStatus) return;

    if (accountStatus !== PaymentAccountStatus.Active) {
      navigate("/register/seller", { replace: true });
    }
  }, [accountStatus, isLoadingAccountStatus, navigate]);

  const ProductMethods = useForm<CreateProductFormValues>({
    resolver: zodResolver(createProductSchema),
    defaultValues: {
      Title: "",
      Description: "",
      Locale: "RJ",
      DeliveryPreference: "Both",
      ImageUrls: [],
    },
    mode: "onChange",
  });
  const ListingMethods = useForm<CreateListingFormValues>({
    resolver: zodResolver(
      createListingSchema
    ) as unknown as Resolver<CreateListingFormValues>,
    mode: "onChange",
  });
  const AuctionMethods = useForm<CreateAuctionFormValues>({
    resolver: zodResolver(
      createAuctionSchema
    ) as unknown as Resolver<CreateAuctionFormValues>,
    mode: "onChange",
    defaultValues: {
      StartDate: new Date(Date.now() + 1000 * 60 * 3),
      EndDate: new Date(Date.now() + 1000 * 60 * 60 * 24 * 7),
    },
  });
  const handleAnimationComplete = () => {
    navigate("/dashboard/MyListings");
  };

  const handleFinalSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const isProductValid = await ProductMethods.trigger();
    const isListingValid = await ListingMethods.trigger();
    const isAuctionValid =
      saleType === "comply" ? await AuctionMethods.trigger() : true;

    if (!isProductValid || !isListingValid || !isAuctionValid) {
      console.error("Validação falhou em algum passo. O usuário pulou etapas?");
      return;
    }

    setIsSubmitting(true);
    setStep(6);
    setSubmissionError(null);
    setSubmissionSuccess(false);

    try {
      const productData = ProductMethods.getValues();
      const listingData = ListingMethods.getValues();
      const auctionData = AuctionMethods.getValues();
      const apiParams: CreateProductParams = {
        ...productData,
        Characteristics: productData.Characteristics.reduce(
          (acc, char) => {
            if (char.key) acc[char.key] = char.value;
            return acc;
          },
          {} as Record<string, string>
        ),
        ImageUrls: productData.ImageUrls,
      };

      const newProduct = await createProduct({ params: apiParams });
      const newListing = await createListing({
        ProductId: newProduct.id,
        BuyPrice: listingData.BuyPrice,
      });
      if (saleType === "comply") {
        await createAuction({
          ListingId: newListing.id,
          StartBidValue: auctionData.StartBidValue,
          WinBidValue: auctionData.WinBidValue,
          StartDate: auctionData.StartDate,
          EndDate: auctionData.EndDate,
        });
      }
      setSubmissionSuccess(true);
    } catch (error) {
      console.error("Erro na cadeia de submissão:", error);
      setIsSubmitting(false);
      setSubmissionSuccess(false);

      let errorMessage = "Ocorreu um erro inesperado. Tente novamente.";
      const genericModerationError =
        "Este produto não está de acordo com nossa política de vendas. Para mais detalhes, veja os termos de uso.";

      if (isAxiosError(error) && error.response?.data) {
        const responseData = error.response.data as string;

        if (responseData.includes("Product moderation failed")) {
          const match = responseData.match(/Reason: (.*)/);

          if (match && match[1]) {
            const reason = match[1].split("\n")[0].trim();

            if (reason === "INVALID_RESPONSE") {
              errorMessage = genericModerationError;
            } else {
              errorMessage = `Este produto não está de acordo com nossa política de vendas. Motivo: ${reason}`;
            }
          } else {
            errorMessage = genericModerationError;
          }
        } else if (
          typeof responseData === "string" &&
          responseData.length < 200
        ) {
          errorMessage = responseData;
        }
      }
      setSubmissionError(errorMessage);
    }
  };
  const handleNextStep = async () => {
    let isValid = false;

    if (step === 1) {
      isValid = await ProductMethods.trigger(["Title", "Description"]);
    }
    if (step === 2) {
      isValid = await ProductMethods.trigger(["ImageUrls"]);
    }
    if (step === 3) {
      isValid = await ProductMethods.trigger([
        "Condition",
        "Category",
        "Characteristics",
        "Locale",
      ]);
    }
    if (step === 4) {
      isValid = await ListingMethods.trigger();
      if (saleType === "comply" && isValid) {
        isValid = await AuctionMethods.trigger();
      }
    }

    if (isValid && step < totalSteps) {
      setStep((prev) => prev + 1);
      window.scrollTo({ top: 0, behavior: "smooth" });
    }
  };

  if (accountStatus != PaymentAccountStatus.Active) {
    return null;
  }

  return (
    <FormProvider {...ProductMethods}>
      <form onSubmit={handleFinalSubmit} className="block min-h-svh lg:flex">
        <AsideBar
          step={step}
          saleType={saleType}
          totalSteps={totalSteps}
          onChangeStep={setStep}
          onChangeSaleType={setSaleType}
          submissionError={submissionError}
          submissionSuccess={submissionSuccess}
          onAnimationComplete={handleAnimationComplete}
          productForm={ProductMethods}
          listingForm={ListingMethods}
          auctionForm={AuctionMethods}
        />
        {step === 6 ? (
          " "
        ) : (
          <main className="flex flex-col p-6 lg:flex-1 lg:p-20">
            <div className="flex-1">
              {step === 1 && <Step1 />}
              {step === 2 && <Step2 />}
              {step === 3 && <Step3 />}
              {step === 4 && (
                <Step4
                  saleType={saleType}
                  listingForm={ListingMethods}
                  auctionForm={AuctionMethods}
                />
              )}
            </div>

            <footer className="mt-8 flex justify-end">
              {step < 5 && (
                <Button
                  size="lg"
                  type="button"
                  onClick={handleNextStep}
                  className="bg-emerald-700 hover:bg-emerald-800"
                >
                  Avançar
                </Button>
              )}

              {step === 5 && (
                <Button
                  size="lg"
                  type="submit"
                  disabled={isSubmitting}
                  className="bg-emerald-700 hover:bg-emerald-800"
                >
                  {isSubmitting ? "Criando Anúncio..." : "Criar Anúncio"}
                </Button>
              )}

              {step === 6 && ""}
            </footer>
          </main>
        )}
      </form>
    </FormProvider>
  );
}
