import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import type { DeliveryAddressParams } from "@/api/user/usersAdresses";
import { useAddAddressMutation } from "@/hooks/user/useUserAdressesMutations";

interface AddAddressModalProps {
  triggerLabel?: string;
}

export default function AddAddressModal({
  triggerLabel = "Adicionar Endereço",
}: AddAddressModalProps) {
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState<DeliveryAddressParams>({
    Street: "",
    Number: "",
    City: "",
    State: "",
    ZipCode: "",
  });

  const addAddress = useAddAddressMutation();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await addAddress.mutateAsync(form);
      setOpen(false);
      setForm({ Street: "", Number: "", City: "", State: "", ZipCode: "" });
    } catch (err) {
      console.error("Erro ao adicionar endereço:", err);
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="outline">{triggerLabel}</Button>
      </DialogTrigger>

      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Adicionar Endereço</DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-3">
          <div>
            <Label htmlFor="street">Rua</Label>
            <Input
              id="street"
              name="Street"
              placeholder="Ex: Avenida Brasil"
              value={form.Street}
              onChange={handleChange}
              required
            />
          </div>

          <div>
            <Label htmlFor="number">Número</Label>
            <Input
              id="number"
              name="Number"
              placeholder="Ex: 123"
              value={form.Number}
              onChange={handleChange}
              required
            />
          </div>

          <div>
            <Label htmlFor="city">Cidade</Label>
            <Input
              id="city"
              name="City"
              placeholder="Ex: Rio de Janeiro"
              value={form.City}
              onChange={handleChange}
              required
            />
          </div>

          <div className="flex gap-3">
            <div className="w-1/2">
              <Label htmlFor="state">Estado</Label>
              <Input
                id="state"
                name="State"
                placeholder="Ex: RJ"
                value={form.State}
                onChange={handleChange}
                required
                maxLength={2}
              />
            </div>

            <div className="w-1/2">
              <Label htmlFor="zipCode">CEP</Label>
              <Input
                id="zipCode"
                name="ZipCode"
                placeholder="00000-000"
                value={form.ZipCode}
                onChange={handleChange}
                required
                pattern="\d{5}-?\d{3}"
              />
            </div>
          </div>

          <Button
            type="submit"
            className="w-full mt-2 cursor-pointer bg-emerald-600 hover:bg-emerald-700"
            disabled={addAddress.isPending}
          >
            {addAddress.isPending ? "Salvando..." : "Salvar Endereço"}
          </Button>
        </form>
      </DialogContent>
    </Dialog>
  );
}
