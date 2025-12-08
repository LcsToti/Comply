import { Label } from "../ui/label";
import { Popover, PopoverTrigger, PopoverContent } from "../ui/popover";
import {
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem,
  SelectGroup,
} from "../ui/select";
import { Check, Plus, Trash2 } from "lucide-react";
import { Select } from "../ui/select";
import { Field, FieldDescription, FieldLabel } from "../ui/field";
import { Input } from "../ui/input";
import { useState } from "react";
import { Button } from "../ui/button";
import { Controller, useFieldArray, useFormContext } from "react-hook-form";
import type { CreateProductFormValues } from "@/schemas/createProductSchema";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
} from "../ui/command";
import { states } from "@/utils/statesLocale";

const predefinedCharacteristics = [
  "Marca",
  "Cor",
  "Modelo",
  "Ano de Fabricação",
  "Material",
  "Dimensões (AxLxP)",
  "Condição da Embalagem",
];

export default function Step3() {
  const {
    control,
    register,
    formState: { errors },
  } = useFormContext<CreateProductFormValues>();

  const [popoverOpen, setPopoverOpen] = useState(false);
  const [localePopoverOpen, setLocalePopoverOpen] = useState(false);
  const [customChar, setCustomChar] = useState("");
  const { fields, append, remove } = useFieldArray({
    control,
    name: "Characteristics",
  });

  const handleAddCustomChar = (e: React.FormEvent) => {
    e.preventDefault();
    if (customChar.trim() !== "") {
      append({ key: customChar.trim(), value: "" });
      setCustomChar("");
      setPopoverOpen(false);
    }
  };
  return (
    <div className="flex flex-col gap-24 h-full min-h-[400px] items-center justify-center ">
      <div className="flex flex-col gap-6 w-full items-center">
        <div className="w-full max-w-md">
          <Field>
            <FieldLabel className="text-emerald-700 font-bold text-2xl">
              Condição do produto
            </FieldLabel>
            <Controller
              name="Condition"
              control={control}
              render={({ field }) => (
                <Select
                  name={field.name}
                  onValueChange={field.onChange}
                  value={field.value}
                >
                  <SelectTrigger className="shadow-sm w-full">
                    <SelectValue placeholder="Condição" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="New">Novo</SelectItem>
                    <SelectItem value="Used">Usado</SelectItem>
                    <SelectItem value="NotWorking">Não funciona</SelectItem>
                    <SelectItem value="Refurbished">Recondicionado</SelectItem>
                  </SelectContent>
                </Select>
              )}
            />

            <FieldDescription>
              {errors.Condition && (
                <p className="text-sm text-red-500">
                  {errors.Condition.message}
                </p>
              )}
              Selecione a condição do produto que está vendendo.
            </FieldDescription>
          </Field>
        </div>
        <div className="w-full max-w-md">
          <Field>
            <FieldLabel className="text-emerald-700 font-bold text-2xl">
              Categoria do produto
            </FieldLabel>
            <Controller
              name="Category"
              control={control}
              render={({ field }) => (
                <Select
                  name={field.name}
                  onValueChange={field.onChange}
                  value={field.value}
                >
                  <SelectTrigger className="shadow-sm w-full">
                    <SelectValue placeholder="Categoria" />
                  </SelectTrigger>
                  <SelectContent>
                    {/* --- Grupo 1 --- */}
                    <SelectGroup>
                      <SelectItem value="Electronics">Eletrônicos</SelectItem>
                    </SelectGroup>

                    <SelectGroup>
                      <SelectItem value="Computers">
                        Computadores e Informática
                      </SelectItem>
                    </SelectGroup>

                    <SelectGroup>
                      <SelectItem value="HomeAppliances">
                        Eletrodomésticos
                      </SelectItem>
                    </SelectGroup>

                    <SelectGroup>
                      <SelectItem value="FurnitureDecor">
                        Móveis e Decoração
                      </SelectItem>
                    </SelectGroup>

                    <SelectGroup>
                      <SelectItem value="FashionBeauty">
                        Moda e Beleza
                      </SelectItem>
                    </SelectGroup>

                    <SelectGroup>
                      <SelectItem value="Sports">Esportes</SelectItem>
                    </SelectGroup>

                    <SelectGroup>
                      <SelectItem value="Collectibles">
                        Colecionáveis
                      </SelectItem>
                    </SelectGroup>

                    <SelectGroup>
                      <SelectItem value="Tools">Ferramentas</SelectItem>
                    </SelectGroup>

                    <SelectGroup>
                      <SelectItem value="Games">Jogos</SelectItem>
                    </SelectGroup>

                    <SelectGroup>
                      <SelectItem value="Services">Serviços</SelectItem>
                    </SelectGroup>

                    <SelectGroup>
                      <SelectItem value="Others">Outros</SelectItem>
                    </SelectGroup>
                  </SelectContent>
                </Select>
              )}
            />

            <FieldDescription>
              {errors.Category && (
                <p className="text-sm text-red-500">
                  {errors.Category.message}
                </p>
              )}
              Escolha a categoria que melhor se encaixa com seu produto.
            </FieldDescription>
          </Field>
        </div>
        <div className="w-full max-w-md">
          <Field>
            <FieldLabel className="text-emerald-700 font-bold text-2xl">
              Localização
            </FieldLabel>
            <Controller
              name="Locale"
              control={control}
              render={({ field }) => {
                const selected = states.find(
                  (state) => state.value === field.value
                );

                return (
                  <Popover
                    open={localePopoverOpen}
                    onOpenChange={setLocalePopoverOpen}
                  >
                    <PopoverTrigger asChild>
                      <Button
                        variant="outline"
                        role="combobox"
                        aria-expanded={localePopoverOpen}
                        className="w-full justify-between"
                      >
                        {selected ? selected.label : "Localização"}
                      </Button>
                    </PopoverTrigger>
                    <PopoverContent className="w-full p-0">
                      <Command>
                        <CommandInput placeholder="Buscar estado..." />
                        <CommandEmpty>Nenhum estado encontrado.</CommandEmpty>
                        <CommandGroup>
                          {states.map((state) => (
                            <CommandItem
                              key={state.value}
                              onSelect={() => {
                                field.onChange(state.value);
                                setLocalePopoverOpen(false);
                              }}
                              value={state.value}
                            >
                              {state.label}
                              {field.value === state.value && (
                                <Check className="ml-auto h-4 w-4" />
                              )}
                            </CommandItem>
                          ))}
                        </CommandGroup>
                      </Command>
                    </PopoverContent>
                  </Popover>
                );
              }}
            />
            <FieldDescription>
              {errors.Locale && (
                <p className="text-sm text-red-500">{errors.Locale.message}</p>
              )}
              Selecione o estado onde o produto está localizado.
            </FieldDescription>
          </Field>
        </div>
        <div className="w-full max-w-md flex flex-col gap-4">
          <div className="flex items-center gap-2">
            <h2 className="text-emerald-700 font-bold text-2xl">
              Características
            </h2>
          </div>

          <Popover open={popoverOpen} onOpenChange={setPopoverOpen}>
            <PopoverTrigger asChild>
              <Button
                type="button"
                variant="outline"
                className="flex justify-between items-center text-emerald-600 border-emerald-500 hover:text-emerald-700 py-6"
              >
                Adicione nova característica
                <Plus className="h-5 w-5" />
              </Button>
            </PopoverTrigger>
            {errors.Characteristics && (
              <p className="text-sm text-red-500">
                {errors.Characteristics.message}
              </p>
            )}

            {/* 3. Atualizar o PopoverContent */}
            <PopoverContent className="w-80">
              <div className="grid gap-4">
                <div className="space-y-2">
                  <h4 className="leading-none font-medium">
                    Adicionar característica
                  </h4>
                  <p className="text-muted-foreground text-sm">
                    Crie uma nova ou selecione um modelo.
                  </p>
                </div>

                {/* --- Formulário para Característica Customizada --- */}
                <form
                  onSubmit={handleAddCustomChar}
                  className="flex items-center gap-2"
                >
                  <Input
                    placeholder="Ex: Voltagem"
                    value={customChar}
                    onChange={(e) => setCustomChar(e.target.value)}
                  />
                  <Button type="submit" size="sm" className="px-3">
                    <Plus className="h-4 w-4" />
                  </Button>
                </form>

                {/* --- Divisor --- */}
                <div className="relative">
                  <div className="absolute inset-0 flex items-center">
                    <span className="w-full border-t" />
                  </div>
                  <div className="relative flex justify-center text-xs uppercase">
                    <span className="bg-popover px-2 text-muted-foreground">
                      Ou
                    </span>
                  </div>
                </div>

                {/* --- Lista de Modelos Pré-definidos --- */}
                <div className="flex flex-col gap-2 max-h-48 overflow-y-auto">
                  {predefinedCharacteristics.map((charName) => (
                    <Button
                      key={charName}
                      variant="ghost"
                      className="justify-start"
                      onClick={() => {
                        append({ key: charName, value: "" });
                        setPopoverOpen(false);
                      }}
                    >
                      {charName}
                    </Button>
                  ))}
                </div>
              </div>
            </PopoverContent>
          </Popover>

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
                  {...register(`Characteristics.${index}.value`)}
                  className="text-base"
                />
                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  onClick={() => {
                    remove(index);
                  }}
                  className="text-muted-foreground hover:text-red-500"
                >
                  <Trash2 className="h-4 w-4" />
                </Button>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
