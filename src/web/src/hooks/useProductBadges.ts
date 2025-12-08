import { useMemo } from "react";
import type { VariantProps } from "class-variance-authority";
import { badgeVariants } from "@/components/ui/badge";
import type { Product, ProductAuction } from "@/types/product";
type BadgeVariant = NonNullable<VariantProps<typeof badgeVariants>["variant"]>;

type StatusBadge = {
    label: string;
    variant: BadgeVariant;
    pulse?: boolean;
};

type UseProductBadgesParams = {
    product: Product;
    auction: ProductAuction | null;
    saleType: "buyNow" | "auction";
};

export const useProductBadges = ({
    product,
    auction,
    saleType,
}: UseProductBadgesParams) => {
    const status = product.listing?.status;
    const auctionStatus = auction?.status;

    const getListingStatusBadge = (): StatusBadge | null => {
        if (status === "Sold") return { label: "Vendido", variant: "sold" };
        if (status === "SoldByAuction")
            return { label: "Vendido por Leilão", variant: "soldAuction" };
        if (status === "Paused") return { label: "Pausado", variant: "paused" };
        if (status === "Available") return { label: "Ativo", variant: "active" };
        return null;
    };

    const getAuctionStatusBadge = (): StatusBadge | null => {
        if (saleType !== "auction") return null;

        if (auctionStatus === "Active")
            return { label: "Leilão Ativo", variant: "auctionActive" };
        if (auctionStatus === "Ending")
            return { label: "Terminando", variant: "ending", pulse: true };
        if (auctionStatus === "Awaiting")
            return { label: "Leilão Agendado", variant: "auctionScheduled" };
        if (auctionStatus === "Success")
            return { label: "Leilão Finalizado", variant: "auctionSuccess" };
        if (auctionStatus === "Failed")
            return { label: "Leilão Sem Venda", variant: "auctionFailed" };
        if (auctionStatus === "Cancelled")
            return { label: "Leilão Cancelado", variant: "auctionCancelled" };

        return null;
    };

    const getAdditionalBadges = (): StatusBadge[] => {
        const badges: StatusBadge[] = [];

        if (product.featured) {
            const now = new Date();
            const expirationDate = new Date(product.expirationFeatureDate);

            if (expirationDate > now) {
                badges.push({ label: "Impulsionado", variant: "featured" });
            }
        }

        return badges;
    };

    const badges = useMemo(() => {
        const listingBadge = getListingStatusBadge();
        const auctionBadge = getAuctionStatusBadge();
        const additionalBadges = getAdditionalBadges();

        const primaryBadge =
            saleType === "auction" && auctionBadge ? auctionBadge : listingBadge;

        return {
            primary: primaryBadge,
            additional: additionalBadges,
            all: primaryBadge
                ? [primaryBadge, ...additionalBadges]
                : additionalBadges,
        };
    }, [status, auctionStatus, saleType, product.featured, product.expirationFeatureDate]);

    const isAuctionActive =
        auctionStatus === "Active" || auctionStatus === "Ending";
    const isSold = status === "Sold" || status === "SoldByAuction";
    const isListed = !!product.listing;
    const isPaused = status === "Paused";
    const isAvailable = status === "Available";

    return {
        badges,
        status: {
            listing: status,
            auction: auctionStatus,
        },
        computed: {
            isAuctionActive,
            isSold,
            isListed,
            isPaused,
            isAvailable,
        },
    };
};

export type { StatusBadge };
