import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { ArrowRight } from "lucide-react";

interface StripeModalProps {
  isOpen: boolean;
  onConfirm: () => void;
  onSkip: () => void;
  onClose: () => void;
}

export function StripeModal({
  isOpen,
  onConfirm,
  onSkip,
  onClose,
}: StripeModalProps) {
  return (
    <AlertDialog open={isOpen} onOpenChange={onClose}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Estamos quase terminando</AlertDialogTitle>
          <AlertDialogDescription>
            Você será redirecionado para a plataforma
            <a className="text-purple-700 font-bold"> Stripe</a>, onde irá
            configurar a conta para receber pagamentos. (obrigatória para a
            venda de produtos).
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel onClick={onSkip}>
            Não desejo vender produtos agora
          </AlertDialogCancel>
          <AlertDialogAction onClick={onConfirm} className="bg-emerald-700">
            Ir para o Stripe
            <ArrowRight />
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
