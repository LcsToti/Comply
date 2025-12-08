import { z } from "zod";

export const createAuctionSchema = z.object({
    StartBidValue: z.number("Determine o valor do primeiro lance.").min(0, "O valor deve ser maior que zero."),
    WinBidValue: z.number("Determine um valor de compra imediata.").min(0, "O valor deve ser maior que zero."),
    StartDate: z.date("Determine uma data válida.").refine(
        (date) => date.getTime() >= Date.now(),
        {
            message: "A data de início deve ser de agora pra frente.",
        }
    ),
    EndDate: z.date("Determine uma data válida."),
})
    .superRefine((data, ctx) => {
        if (data.EndDate < new Date(data.StartDate.getTime() + 30 * 60 * 1000)) {
            ctx.addIssue({
                code: z.ZodIssueCode.custom,
                path: ["EndDate"],
                message: "A data de término deve ser pelo menos 30 minutos após a data de início.",
            });
        }

        if (data.StartBidValue > data.WinBidValue) {
            ctx.addIssue({
                code: z.ZodIssueCode.custom,
                path: ["StartBidValue"],
                message: "O lance inicial não pode ser maior que o valor de compra imediata.",
            });
        }
    }) satisfies z.ZodType<{
        StartBidValue: number;
        WinBidValue: number;
        StartDate: Date;
        EndDate: Date;
    }>;


export type CreateAuctionFormValues = z.infer<typeof createAuctionSchema>;