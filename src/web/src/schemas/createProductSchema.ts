import { z } from "zod";

export const createProductSchema = z.object({
    Title: z.string().min(5, "O título precisa ter pelo menos 5 caracteres").max(100, "O título pode ter no máximo 100 caracteres."),
    Description: z.string().min(10, "A descrição precisa ter pelo menos 10 caracteres").max(3000, "A descrição pode ter no máximo 3000 caracteres."),
    Condition: z.enum(["New", "Used", "NotWorking", "Refurbished"], "Selecione a condição do produto."),
    Category: z.enum(
        [
            "Electronics",
            "Computers",
            "HomeAppliances",
            "FurnitureDecor",
            "FashionBeauty",
            "Sports",
            "Collectibles",
            "Tools",
            "Games",
            "Services",
            "Others"
        ],
        "Selecione a categoria que o produto se encaixa."
    ),

    Characteristics: z.array(z.object({
        key: z.string().min(1, "O nome da característica é obrigatório"),
        value: z.string().min(1, "O valor é obrigatório"),
    })).min(1, "Adicione pelo menos uma característica."),

    Locale: z.string().min(2, "Localização é obrigatória"),
    DeliveryPreference: z.string(),
    ImageUrls: z.array(z.instanceof(File))
        .min(1, "Envie pelo menos uma imagem.")
        .max(10, "Você pode enviar no máximo 10 imagens."),
});

export type CreateProductFormValues = z.infer<typeof createProductSchema>;