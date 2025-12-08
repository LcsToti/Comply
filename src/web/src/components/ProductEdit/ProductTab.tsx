import { useForm, useFieldArray, Controller } from "react-hook-form";
import { Input } from "../ui/input";
import { Label } from "../ui/label";
import { Textarea } from "../ui/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "../ui/select";
import { Field, FieldLabel } from "../ui/field";
import { Popover, PopoverContent, PopoverTrigger } from "../ui/popover";
import { Button } from "../ui/button";
import { states } from "@/utils/statesLocale";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
} from "../ui/command";
import { Trash2 } from "lucide-react";
import { useState, useEffect } from "react";
import {
  CategoriesLabels,
  ProductConditionLabels,
  type Product,
} from "@/types/product";
import type { ImageItem } from "./EditorSortableItem";
import type { UpdateProductParams } from "@/api/products/products";
import ProductImageEditor from "./ProductImageEditor";
import { isEqual } from "lodash";
import {
  useAddImagesMutation,
  useRemoveImageMutation,
  useReorderImagesMutation,
  useUpdateProductMutation,
} from "@/hooks/products/useProductsMutations";
const mapCharacteristicsToArray = (characteristics: Record<string, string>) => {
  return Object.entries(characteristics).map(([key, value]) => ({
    key,
    value,
  }));
};
const mapCharacteristicsToRecord = (
  characteristics: { key: string; value: string }[]
) => {
  return Object.fromEntries(characteristics.map((c) => [c.key, c.value]));
};
interface FormData {
  title: string;
  description: string;
  condition: string;
  category: string;
  locale: string;
  characteristics: { key: string; value: string }[];
  images: ImageItem[];
}
interface ProductTabProps {
  product: Product;
}

function ProductTab({ product }: ProductTabProps) {
  const { mutateAsync: updateProduct } = useUpdateProductMutation();
  const { mutateAsync: addImages } = useAddImagesMutation();
  const { mutateAsync: removeImage } = useRemoveImageMutation();
  const { mutateAsync: reorderImages } = useReorderImagesMutation();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const { register, control, handleSubmit, reset, watch } = useForm<FormData>();

  const { fields, remove } = useFieldArray({
    control,
    name: "characteristics",
  });

  useEffect(() => {
    if (product) {
      reset({
        title: product.title,
        description: product.description,
        condition: ProductConditionLabels[product.condition],
        category: CategoriesLabels[product.category],
        locale: product.locale,
        characteristics: mapCharacteristicsToArray(product.characteristics),
        images: product.images.map((url) => ({
          id: url,
          url,
          file: undefined,
        })),
      });
    }
  }, [product, reset]);

  const onSubmit = async (data: FormData) => {
    setIsSubmitting(true);
    const mutationPromises = [];

    // --- 1. Calcular mudanças nas Imagens ---
    const originalUrls = product.images;
    const finalItems = data.images;

    const newFiles = finalItems
      .filter((item) => !!item.file)
      .map((item) => item.file as File);

    const finalExistingUrls = finalItems
      .filter((item) => !item.file)
      .map((item) => item.url);

    // REMOÇÃO: Imagens que estavam no original mas não estão mais na lista final
    const removedUrls = originalUrls.filter(
      (url) => !finalExistingUrls.includes(url)
    );
    if (removedUrls.length > 0) {
      mutationPromises.push(
        removeImage({
          productId: product.id,
          params: { ImageUrls: removedUrls },
        })
      );
    }

    // ADIÇÃO: Novos arquivos
    if (newFiles.length > 0) {
      mutationPromises.push(
        addImages({
          productId: product.id,
          params: { Images: newFiles },
        })
      );
    }

    // REORDENAÇÃO:
    // Filtra a lista original para conter apenas os que sobraram
    const originalRemainingUrls = originalUrls.filter((url) =>
      finalExistingUrls.includes(url)
    );
    // Compara se a ordem dos que sobraram mudou
    if (!isEqual(originalRemainingUrls, finalExistingUrls)) {
      mutationPromises.push(
        reorderImages({
          productId: product.id,
          params: { ImageUrls: finalExistingUrls },
        })
      );
    }

    // --- 2. Calcular mudanças nos Metadados (UpdateProduct) ---
    const finalCharacteristics = mapCharacteristicsToRecord(
      data.characteristics
    );

    // Monta o payload SÓ com o que mudou
    const updatePayload: Partial<UpdateProductParams> = {};

    if (data.title !== product.title) updatePayload.Title = data.title;
    if (data.description !== product.description)
      updatePayload.Description = data.description;
    if (data.locale !== product.locale) updatePayload.Locale = data.locale;
    if (data.condition !== ProductConditionLabels[product.condition])
      updatePayload.Condition = data.condition;
    if (data.category !== CategoriesLabels[product.category])
      updatePayload.Category = data.category;
    if (!isEqual(finalCharacteristics, product.characteristics))
      updatePayload.Characteristics = finalCharacteristics;

    // Só chama o Update se algo realmente mudou
    if (Object.keys(updatePayload).length > 0) {
      mutationPromises.push(
        updateProduct({
          productId: product.id,
          params: updatePayload,
        })
      );
    }

    // --- 3. Executar Mutações ---
    try {
      await Promise.all(mutationPromises);
      console.log("✅ Produto atualizado com sucesso!");
      // TODO: Adicionar toast de sucesso
    } catch (error) {
      console.error("❌ Erro ao atualizar produto:", error);
      // TODO: Adicionar toast de erro
    } finally {
      setIsSubmitting(false);
    }
  };

  const currentLocale = watch("locale");

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-10">
      {/* --- Título --- */}
      <div>
        <Label htmlFor="title" className="text-emerald-700 font-bold text-xl">
          Título do anúncio
        </Label>
        <Input
          id="title"
          placeholder="Título do anúncio..."
          {...register("title")}
        />
      </div>

      {/* --- Descrição --- */}
      <div>
        <Label
          htmlFor="description"
          className="text-emerald-700 font-bold text-xl"
        >
          Descreva o produto
        </Label>
        <Textarea
          id="description"
          placeholder="Digite aqui..."
          {...register("description")}
        />
      </div>

      {/* --- Editor de imagens (Controlado) --- */}
      <div>
        <Controller
          control={control}
          name="images"
          render={({ field }) => (
            <ProductImageEditor
              value={field.value || []}
              onChange={field.onChange}
              disabled={isSubmitting}
            />
          )}
        />
      </div>

      {/* --- Condição / Categoria / Localização --- */}
      <div className="flex flex-row gap-10 justify-start items-center flex-wrap">
        {/* Condição */}
        <Field>
          <FieldLabel className="text-emerald-700 font-bold text-2xl">
            Condição do produto
          </FieldLabel>
          <Controller
            control={control}
            name="condition"
            render={({ field }) => (
              <Select
                onValueChange={field.onChange}
                value={field.value}
                disabled={isSubmitting}
              >
                <SelectTrigger className="shadow-sm w-full">
                  <SelectValue placeholder="Condição" />
                </SelectTrigger>
                <SelectContent>
                  {Object.entries(ProductConditionLabels).map(
                    ([value, label]) => (
                      <SelectItem key={value} value={value}>
                        {label}
                      </SelectItem>
                    )
                  )}
                </SelectContent>
              </Select>
            )}
          />
        </Field>

        {/* Categoria */}
        <Field>
          <FieldLabel className="text-emerald-700 font-bold text-2xl">
            Categoria do produto
          </FieldLabel>
          <Controller
            control={control}
            name="category"
            render={({ field }) => (
              <Select
                onValueChange={field.onChange}
                value={field.value}
                disabled={isSubmitting}
              >
                <SelectTrigger className="shadow-sm w-full">
                  <SelectValue placeholder="Categoria" />
                </SelectTrigger>
                <SelectContent>
                  {Object.entries(CategoriesLabels).map(([value, label]) => (
                    <SelectItem key={value} value={value}>
                      {label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
        </Field>

        {/* Localização (agora 'locale') */}
        <Field>
          <FieldLabel className="text-emerald-700 font-bold text-2xl">
            Localização
          </FieldLabel>
          <Controller
            control={control}
            name="locale"
            render={({ field }) => (
              <Popover>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    role="combobox"
                    className="w-full justify-between"
                    disabled={isSubmitting}
                  >
                    {currentLocale || "Localização"}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-full p-0">
                  <Command>
                    <CommandInput placeholder="Buscar estado..." />
                    <CommandEmpty>Nenhum estado encontrado.</CommandEmpty>
                    <CommandGroup className="max-h-32 overflow-y-auto">
                      {states.map((state) => (
                        <CommandItem
                          key={state.value}
                          value={state.value}
                          onSelect={() => field.onChange(state.label)}
                        >
                          {state.label}
                        </CommandItem>
                      ))}
                    </CommandGroup>
                  </Command>
                </PopoverContent>
              </Popover>
            )}
          />
        </Field>
      </div>

      {/* --- Características --- */}
      <div className="w-full max-w-md flex flex-col gap-4">
        <h2 className="text-emerald-700 font-bold text-2xl">Características</h2>

        <Popover>
          {/* ... (o seu popover de adicionar característica) ... */}
        </Popover>

        {/* Lista de características */}
        <div className="flex flex-col gap-3">
          {fields.length === 0 && (
            <p className="text-sm text-muted-foreground text-center py-4">
              Nenhuma característica adicionada.
            </p>
          )}

          {fields.map((field, index) => (
            <div key={field.id} className="flex items-center gap-3">
              <Label className="w-[100px] flex-none text-left font-medium">
                {field.key}
              </Label>
              <Input
                placeholder={`Valor para ${field.key}...`}
                {...register(`characteristics.${index}.value` as const)}
                className="text-base"
                disabled={isSubmitting}
              />
              <Button
                type="button"
                variant="ghost"
                size="icon"
                className="text-muted-foreground hover:text-red-500"
                onClick={() => remove(index)}
                disabled={isSubmitting}
              >
                <Trash2 className="h-4 w-4" />
              </Button>
            </div>
          ))}
        </div>
      </div>

      {/* --- Botão de envio --- */}
      <Button
        type="submit"
        className="bg-emerald-600 hover:bg-emerald-700 text-white mt-4 self-start"
        disabled={isSubmitting}
      >
        {isSubmitting ? "Salvando..." : "Salvar Alterações"}
      </Button>
    </form>
  );
}

export default ProductTab;
