import type { CommonProductCardProps } from "../../ProductCard";
import { useCountdown } from "@/hooks/useCountdownHook";
import { motion } from "motion/react";

const SearchAuctionAction: React.FC<
  Pick<CommonProductCardProps, "formattedDates" | "navigate" | "product">
> = ({ product }) => {
  const countdown = useCountdown(
    new Date(
      product.listing?.auction?.settings?.endDate ?? new Date()
    ).toString()
  );

  const hasAuction = !!product.listing?.auction;

  return (
    <motion.div
      initial={{ opacity: 0, y: 6 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3 }}
      className="flex flex-col items-center md:items-end justify-center w-full md:w-64"
    >
      {hasAuction ? (
        <>
          <p className="text-sm text-gray-500 tracking-wide mb-1">
            Leilão termina em
          </p>
          <motion.span
            key={countdown}
            initial={{ opacity: 0.6 }}
            animate={{ opacity: 1 }}
            transition={{ duration: 0.3 }}
            className="text-xl md:text-2xl font-semibold text-emerald-700"
          >
            {countdown}
          </motion.span>
        </>
      ) : (
        <p className="text-sm text-gray-400 font-medium text-center md:text-right">
          Nenhum leilão ativo
        </p>
      )}
    </motion.div>
  );
};

export default SearchAuctionAction;
