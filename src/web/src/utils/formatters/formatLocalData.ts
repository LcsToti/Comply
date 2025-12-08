export const formatUtcToLocal = (
    isoDateString: string | null | undefined
): { formattedDate: string; formattedTime: string } => {
    if (!isoDateString) {
        return { formattedDate: "N/A", formattedTime: "N/A" };
    }

    try {
        const date = new Date(isoDateString);

        const formatterDate = new Intl.DateTimeFormat("pt-BR", {
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
        });

        const formatterTime = new Intl.DateTimeFormat("pt-BR", {
            hour: "2-digit",
            minute: "2-digit",
            second: "2-digit",
            hour12: false,
        });

        return {
            formattedDate: formatterDate.format(date),
            formattedTime: formatterTime.format(date),
        };
    } catch {
        return { formattedDate: "Data Inválida", formattedTime: "Hora Inválida" };
    }
};