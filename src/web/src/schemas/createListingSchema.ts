import z from "zod";

export const createListingSchema = z.object({
    BuyPrice: z.coerce.number("Determine um preço de compra imediata.").min(0, "O valor deve ser maior que zero."),
}) satisfies z.ZodType<{ BuyPrice: number }>;


export type CreateListingFormValues = z.infer<typeof createListingSchema>;
