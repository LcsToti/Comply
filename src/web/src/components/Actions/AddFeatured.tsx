import { useState } from "react";
import { TrendingUp } from "lucide-react";
import { Button } from "../ui/button";
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "../ui/dialog";
import { Checkbox } from "../ui/checkbox";
import { Label } from "../ui/label";

function AddFeatured() {
  const [confirmed, setConfirmed] = useState(false);

  return (
    <Dialog>
      <DialogTrigger asChild>
        <Button
          size={"sm"}
          className="bg-emerald-700 hover:bg-[#00A884] cursor-pointer flex items-center gap-2 w-full"
        >
          <TrendingUp size={22} />
          Impulsionar
        </Button>
      </DialogTrigger>

      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Tornar produto em destaque</DialogTitle>
          <DialogDescription className="space-y-3 text-sm leading-relaxed">
            <p>
              Ao tornar seu produto <strong>destaque</strong>, ele será
              priorizado nas buscas e aparecerá com mais frequência nas seções
              principais da página inicial.
            </p>
            <p>
              No entanto, outros vendedores também podem destacar seus produtos.
              Todos os produtos em destaque possuem{" "}
              <strong>igual prioridade</strong> — o posicionamento é definido de
              forma dinâmica para garantir equilíbrio entre todos os anúncios.
            </p>
            <p>
              <strong>Essa ação irá gerar um custo adicional.</strong>
            </p>
          </DialogDescription>
        </DialogHeader>

        <div className="flex items-start gap-2 mt-4">
          <Checkbox
            id="confirm-checkbox"
            checked={confirmed}
            onCheckedChange={(value) => setConfirmed(Boolean(value))}
          />
          <Label
            htmlFor="confirm-checkbox"
            className="text-sm text-muted-foreground leading-snug"
          >
            Confirmo que li e entendi as informações acima.
          </Label>
        </div>

        <DialogFooter>
          <DialogClose asChild>
            <Button variant="outline">Cancelar</Button>
          </DialogClose>
          <Button
            disabled={!confirmed}
            className={`${
              confirmed
                ? "bg-emerald-700 hover:bg-[#00A884]"
                : "bg-gray-400 cursor-not-allowed"
            }`}
          >
            Prosseguir
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

export default AddFeatured;
