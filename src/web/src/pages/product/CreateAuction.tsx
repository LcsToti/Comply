import { useEffect, useState } from "react";
import { DateTimePicker } from "@/components/DateTimePicker";
import Footer from "@/components/Footer";
import Header from "@/components/Header";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { AnimatePresence, motion } from "motion/react";
import {
  InputGroup,
  InputGroupAddon,
  InputGroupText,
  InputGroupInput,
} from "@/components/ui/input-group";
import { ArrowDown, Banana, Flame, Leaf } from "lucide-react";
import { useNavigate, useParams } from "react-router";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm, FormProvider } from "react-hook-form";
import {
  createAuctionSchema,
  type CreateAuctionFormValues,
} from "@/schemas/createAuctionSchema";
import { formatCurrency } from "@/utils/formatters/formatCurrency";
import { useProductQuery } from "@/hooks/products/useProductsQueries";
import { useCreateAuctionMutation } from "@/hooks/listings/useAuctionsMutations";
import hammerImg from "@/assets/logo/hammer-green.png"

function CreateAuction() {
  const [showMain, setShowMain] = useState(false);
  const [selectedOption, setSelectedOption] = useState<string | null>(null);
  const { id } = useParams<{ id: string }>();
  const { data: productData } = useProductQuery(id!);
  const { mutateAsync: createAuction, isPending } = useCreateAuctionMutation();
  const navigate = useNavigate();

  const price = productData?.listing?.buyPrice;

  const methods = useForm<CreateAuctionFormValues>({
    resolver: zodResolver(createAuctionSchema),
    defaultValues: {
      StartBidValue: 0,
      WinBidValue: price!,
      StartDate: new Date(Date.now() + 10 * 60 * 1000),
      EndDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000),
    },
  });
  const { register, setValue, handleSubmit, watch, reset, formState } = methods;
  const { errors } = formState;
  useEffect(() => {
    if (price) {
      setValue("WinBidValue", price);
    }
  }, [price, setValue]);
  useEffect(() => {
    const timer = setTimeout(() => setShowMain(true), 2000);
    return () => clearTimeout(timer);
  }, []);

  // Tipos de leilão
  const auctionOptions = [
    {
      id: "flame",
      title: "Mais emoção?",
      icon: <Flame />,
      desc: "Comece com 20% a 30% do valor de mercado.",
      pct: 0.25,
    },
    {
      id: "leaf",
      title: "Equilíbrio é tudo..",
      icon: <Leaf />,
      desc: "Comece com 50% do valor de mercado.",
      pct: 0.5,
    },
    {
      id: "banana",
      title: "Tá com medinho?",
      icon: <Banana />,
      desc: "Comece com 70% do valor de mercado.",
      pct: 0.7,
    },
  ];

  // Selecionar opção e calcular lance inicial
  const handleOptionSelect = (optionId: string) => {
    setSelectedOption(optionId);
    const selected = auctionOptions.find((x) => x.id === optionId);
    if (!selected) return;
    const calculatedBid = Math.round(price! * selected.pct);
    setValue("StartBidValue", calculatedBid);
  };

  // Submissão real
  const onSubmit = async (data: CreateAuctionFormValues) => {
    try {
      if (!productData?.listing.id) {
        console.error("Sem id da listagem do produto..");
        return;
      }
      await createAuction({
        ListingId: productData?.listing.id,
        StartBidValue: data.StartBidValue,
        WinBidValue: data.WinBidValue,
        StartDate: data.StartDate,
        EndDate: data.EndDate,
      });

      navigate("/dashboard/MyListings");
      reset();
      setSelectedOption(null);
    } catch (err) {
      console.error("Erro ao criar leilão:", err);
    }
  };

  return (
    <>
      <Header />

      <FormProvider {...methods}>
        <form
          onSubmit={handleSubmit(onSubmit)}
          className="w-full flex flex-col justify-center items-center min-h-[calc(100dvh-90px)]"
        >
          {/* Tela de entrada animada */}
          <AnimatePresence>
            {!showMain && (
              <motion.div
                key="title"
                initial={{ opacity: 0, y: 40 }}
                animate={{
                  opacity: 1,
                  y: [0, -10, 0],
                  transition: {
                    duration: 1.5,
                    repeat: Infinity,
                    repeatType: "mirror",
                  },
                }}
                exit={{ opacity: 0, y: -80 }}
                transition={{ duration: 1 }}
                className="absolute top-1/2 -translate-y-1/2 text-center flex flex-col justify-center items-center gap-10"
              >
                <img src={hammerImg} className="w-20" />
                <h1 className="text-4xl font-bold select-none">
                  É hora de leiloar seu produto!
                </h1>
              </motion.div>
            )}
          </AnimatePresence>

          {/* Conteúdo principal */}
          <AnimatePresence>
            {showMain && (
              <motion.div
                key="main"
                initial={{ opacity: 0, y: 40 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 1 }}
                className="flex flex-col items-center gap-10"
              >
                <div className="relative flex flex-row items-center gap-4">
                  <ArrowDown />
                  <p className="text-lg font-medium">Escolha uma opção</p>
                  <ArrowDown />
                </div>

                {/* Cards de opções */}
                <div className="flex flex-wrap justify-center items-center gap-5">
                  {auctionOptions.map((opt) => (
                    <Card
                      key={opt.id}
                      onClick={() => handleOptionSelect(opt.id)}
                      className={`w-60 min-h-60 flex flex-col transition-all cursor-pointer hover:scale-102 select-none ${
                        selectedOption === opt.id
                          ? "bg-emerald-700 text-white shadow-md"
                          : "hover:bg-emerald-700 hover:text-white"
                      }`}
                    >
                      <CardHeader>
                        <CardTitle className="text-center flex flex-col justify-center items-center text-lg gap-4">
                          {opt.icon}
                          {opt.title}
                        </CardTitle>
                      </CardHeader>
                      <CardContent className="text-center text-md">
                        {opt.desc}
                      </CardContent>
                    </Card>
                  ))}
                </div>

                {/* Formulário exibido após selecionar opção */}
                <AnimatePresence>
                  {selectedOption && (
                    <motion.div
                      key="details"
                      initial={{ opacity: 0, y: 20 }}
                      animate={{ opacity: 1, y: 0 }}
                      exit={{ opacity: 0 }}
                      transition={{ duration: 0.6 }}
                      className="flex flex-col items-center gap-6"
                    >
                      <div className="text-center">
                        <p className="text-sm text-gray-400">
                          Preço atual de compra imediata:
                        </p>
                        <p className="text-2xl font-semibold text-emerald-600">
                          {price ? formatCurrency(price) : 0}
                        </p>
                      </div>
                      {/* Lance inicial */}
                      <div>
                        <p className="text-sm text-center text-gray-400">
                          Lance inicial:
                        </p>
                        <InputGroup>
                          <InputGroupAddon>
                            <InputGroupText>R$</InputGroupText>
                          </InputGroupAddon>
                          <InputGroupInput
                            value={watch("StartBidValue")}
                            {...register("StartBidValue", {
                              valueAsNumber: true,
                            })}
                          />
                        </InputGroup>
                      </div>{" "}
                      {errors.StartBidValue && (
                        <p className="text-red-500 text-sm mt-1">
                          {errors.StartBidValue.message?.toString()}
                        </p>
                      )}
                      {/* Datas */}
                      <div className="flex flex-col gap-4">
                        <div className="flex flex-col">
                          <p className="text-sm text-gray-400">Começa em:</p>
                          <DateTimePicker
                            value={watch("StartDate")}
                            onChange={(date) => setValue("StartDate", date)}
                          />{" "}
                        </div>
                        {errors.StartDate && (
                          <p className="text-red-500 text-sm mt-1">
                            {errors.StartDate.message?.toString()}
                          </p>
                        )}
                        <div className="flex flex-col">
                          <p className="text-sm text-gray-400">Finaliza em:</p>
                          <DateTimePicker
                            value={watch("EndDate")}
                            onChange={(date) => setValue("EndDate", date)}
                          />
                        </div>
                        {errors.EndDate && (
                          <p className="text-red-500 text-sm mt-1">
                            {errors.EndDate.message?.toString()}
                          </p>
                        )}
                      </div>
                      <button
                        type="submit"
                        disabled={isPending}
                        className={`mt-8 px-6 py-3 rounded-xl text-lg font-semibold transition-all cursor-pointer ${
                          isPending
                            ? "bg-gray-400 text-white cursor-not-allowed"
                            : "bg-emerald-600 hover:bg-emerald-700 text-white"
                        }`}
                      >
                        {isPending ? "Criando..." : "Criar Leilão"}
                      </button>
                    </motion.div>
                  )}
                </AnimatePresence>
              </motion.div>
            )}
          </AnimatePresence>
        </form>
      </FormProvider>

      <Footer />
    </>
  );
}

export default CreateAuction;
