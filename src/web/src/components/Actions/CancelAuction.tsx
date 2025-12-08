import { useState } from "react";
import { XCircle } from "lucide-react";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "../ui/alert-dialog";
import { Button } from "../ui/button";
import { Input } from "../ui/input";
import { useCancelAuctionMutation } from "@/hooks/listings/useAuctionsMutations";

type CancelAuctionProps = {
  auctionId: string;
};
function CancelAuction({ auctionId }: CancelAuctionProps) {
  const [confirmationText, setConfirmationText] = useState("");
  const requiredPhrase = "Eu quero confirmar";
  const isConfirmed = confirmationText.trim() === requiredPhrase;

  const { mutate: cancelData, isPending } = useCancelAuctionMutation();

  const handleCancel = () => {
    if (!isConfirmed) return;
    cancelData(auctionId);
  };

  return (
    <AlertDialog>
      <AlertDialogTrigger asChild>
        <Button
          size={"sm"}
          variant="outline"
          className="w-full border-red-600 hover:bg-red-100 text-red-500 cursor-pointer flex items-center gap-2"
        >
          <XCircle size={22} />
          Cancelar Leilão
        </Button>
      </AlertDialogTrigger>

      <AlertDialogContent className="max-w-sm">
        <AlertDialogHeader>
          <AlertDialogTitle>Você tem certeza?</AlertDialogTitle>
          <AlertDialogDescription className="space-y-3">
            <p>
              Essa ação <strong>não pode ser desfeita</strong>. Ao cancelar,
              você estará removendo o leilão deste produto da base de dados.
            </p>
            <p>
              Para confirmar, digite exatamente:{" "}
              <span className="font-semibold text-red-600">
                {requiredPhrase}
              </span>
            </p>
            <Input
              placeholder="Digite aqui..."
              value={confirmationText}
              onChange={(e) => setConfirmationText(e.target.value)}
              className="mt-2"
            />
          </AlertDialogDescription>
        </AlertDialogHeader>

        <AlertDialogFooter>
          <AlertDialogCancel>Cancelar</AlertDialogCancel>
          <AlertDialogAction
            disabled={!isConfirmed || isPending}
            onClick={handleCancel}
            className={`${
              isConfirmed
                ? "bg-red-600 hover:bg-red-700"
                : "bg-gray-400 cursor-not-allowed"
            }`}
          >
            Confirmar Cancelamento
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}

export default CancelAuction;
