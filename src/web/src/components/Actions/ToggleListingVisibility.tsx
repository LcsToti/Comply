import { useState } from "react";
import { Pause, Play } from "lucide-react";
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
import { useToggleListingAvailabilityMutation } from "@/hooks/listings/useListingsMutations";

type ToggleListingVisibilityProps = {
  listingId: string;
  action: "pause" | "resume";
};

export default function ToggleListingVisibility({
  listingId,
  action,
}: ToggleListingVisibilityProps) {
  const [confirmationText, setConfirmationText] = useState("");

  const isPause = action === "pause";
  const requiredPhrase = isPause ? "Eu quero pausar" : "Eu quero retomar";
  const isConfirmed = confirmationText.trim() === requiredPhrase;

  const { mutate: toggleListing, isPending } =
    useToggleListingAvailabilityMutation();

  const handleClick = () => toggleListing(listingId);

  const icon = isPause ? <Pause size={22} /> : <Play size={22} />;
  const btnColor = isPause
    ? "border-yellow-600 text-yellow-600 hover:bg-yellow-50"
    : "border-emerald-700 text-emerald-700 hover:bg-emerald-50";
  const dialogBtnColor = isPause
    ? "bg-yellow-500 hover:bg-yellow-600"
    : "bg-emerald-600 hover:bg-emerald-700";
  const label = isPause ? "Pausar anúncio" : "Retomar anúncio";

  return (
    <AlertDialog>
      <AlertDialogTrigger asChild>
        <Button
          size={"sm"}
          variant="outline"
          className={`${btnColor} cursor-pointer flex items-center gap-2 w-full`}
        >
          {icon}
          {label}
        </Button>
      </AlertDialogTrigger>

      <AlertDialogContent className="max-w-sm">
        <AlertDialogHeader>
          <AlertDialogTitle>Você tem certeza?</AlertDialogTitle>
          <AlertDialogDescription className="space-y-3">
            <p>
              {isPause
                ? "Você está prestes a pausar este anúncio. Enquanto estiver pausado, ele não ficará visível para os compradores."
                : "Você está prestes a retomar este anúncio. Ele voltará a ficar visível para os compradores."}
            </p>
            <p>
              Para confirmar, digite exatamente:{" "}
              <span
                className={`font-semibold ${
                  isPause ? "text-yellow-600" : "text-emerald-600"
                }`}
              >
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
            onClick={handleClick}
            className={`${
              isConfirmed ? dialogBtnColor : "bg-gray-400 cursor-not-allowed"
            }`}
          >
            {label}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
