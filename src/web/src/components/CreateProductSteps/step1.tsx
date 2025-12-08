import { Label } from "../ui/label";
import { Input } from "../ui/input";
import { Textarea } from "../ui/textarea";
import type { CreateProductFormValues } from "@/schemas/createProductSchema";
import { useFormContext } from "react-hook-form";

export default function Step1() {
  const {
    register,
    watch,
    formState: { errors },
  } = useFormContext<CreateProductFormValues>();

  const titleValue = watch("Title") || "";
  const descValue = watch("Description") || "";
  const MAX_LENGTH_DESCRIPTION = 3000;
  const MAX_LENGTH_TITLE = 100;

  return (
    <div className="flex flex-col gap-6 h-full  items-center justify-center ">
      <div className="grid w-full items-center gap-3">
        <Label htmlFor="title" className="text-emerald-700 font-bold text-2xl">
          Escreva o título do anúncio
        </Label>
        <Input
          type="text"
          id="title"
          maxLength={MAX_LENGTH_TITLE}
          placeholder="Título do anúncio..."
          {...register("Title")}
        />
        <div className="flex justify-end text-muted-foreground text-sm gap-3">
          {errors.Title ? (
            <span className="text-red-500">{errors.Title.message}</span>
          ) : (
            <span />
          )}
          <span
            className={titleValue.length > 90 ? "font-bold text-red-500" : ""}
          >
            {titleValue.length} / {MAX_LENGTH_TITLE}
          </span>
        </div>
      </div>

      <div className="grid w-full gap-3">
        <Label
          htmlFor="description"
          className="text-emerald-700 font-bold text-2xl"
        >
          Descreva o produto
        </Label>
        <Textarea
          maxLength={MAX_LENGTH_DESCRIPTION}
          placeholder="Digite aqui..."
          id="description"
          {...register("Description")}
        />
        <div className="flex justify-between text-muted-foreground text-sm">
          {errors.Description ? (
            <span className="text-red-500">{errors.Description.message}</span>
          ) : (
            <p>
              <span className="font-bold">Dica:</span> descrições acima de 200
              caracteres vendem 3x mais.
            </p>
          )}
          <span
            className={
              descValue.length > 200 ? "font-bold text-emerald-600" : ""
            }
          >
            {descValue.length} / {MAX_LENGTH_DESCRIPTION}
          </span>
        </div>
      </div>
    </div>
  );
}
