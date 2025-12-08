// CreatePaymentMethod.tsx - ATUALIZADO COM CALLBACK DE SUCESSO
import { loadStripe } from "@stripe/stripe-js";
import {
  Elements,
  PaymentElement,
  useStripe,
  useElements,
} from "@stripe/react-stripe-js";
import { Button } from "@/components/ui/button";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { CheckCircle2, Loader2 } from "lucide-react";
import { useCreatePaymentMethodMutation } from "@/hooks/payments/usePaymentAccountsMutations";

const stripePromise = loadStripe(
  "pk_test_51S9AbmKfoIqREVIsGNjYyAMtUph7rKLPxts4fc6oL4MEqjbZB6k5l4cvQWzLNrMz5JkK6sn5DORTCTU2CPGg6ZoJ00OvOxBbKv"
);

interface CheckoutFormProps {
  onSuccess: () => void;
}

const CheckoutForm = ({ onSuccess }: CheckoutFormProps) => {
  const stripe = useStripe();
  const elements = useElements();
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!stripe || !elements) return;

    setLoading(true);

    try {
      const { error } = await stripe.confirmSetup({
        elements,
        redirect: "if_required",
      });

      if (error) {
        console.error(error);
        toast.error(error.message || "Erro ao salvar método de pagamento");
        return;
      }

      toast.success("Método de pagamento adicionado com sucesso!");
      onSuccess();
    } catch {
      toast.error("Erro ao processar pagamento");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4">
      <div className="bg-white rounded-lg p-4 border border-gray-200">
        <PaymentElement />
      </div>

      <Button
        type="submit"
        disabled={!stripe || loading}
        className="w-full bg-emerald-700 hover:bg-emerald-800"
      >
        {loading ? (
          <>
            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
            Salvando...
          </>
        ) : (
          <>
            <CheckCircle2 className="mr-2 h-4 w-4" />
            Salvar método de pagamento
          </>
        )}
      </Button>
    </form>
  );
};

interface CreatePaymentMethodProps {
  onSuccess?: () => void;
}

export default function CreatePaymentMethod({
  onSuccess,
}: CreatePaymentMethodProps) {
  const {
    mutate,
    data: clientSecret,
    isPending,
    isError,
  } = useCreatePaymentMethodMutation();

  useEffect(() => {
    mutate();
  }, [mutate]);

  if (isPending) {
    return (
      <div className="flex flex-col items-center justify-center p-8">
        <Loader2 className="h-8 w-8 animate-spin text-emerald-700 mb-2" />
        <p className="text-gray-600 text-sm">Iniciando...</p>
      </div>
    );
  }

  if (isError || !clientSecret) {
    return (
      <div className="flex flex-col items-center justify-center p-8 text-center">
        <p className="text-red-600 font-medium mb-2">
          Erro ao iniciar pagamento
        </p>
        <Button variant="outline" onClick={() => mutate()} className="mt-2">
          Tentar novamente
        </Button>
      </div>
    );
  }

  return (
    <div className="flex flex-col justify-center items-center">
      <div className="rounded-lg w-full">
        <Elements stripe={stripePromise} options={{ clientSecret }}>
          <CheckoutForm onSuccess={onSuccess || (() => {})} />
        </Elements>
      </div>
    </div>
  );
}
