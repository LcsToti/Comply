import React, { useMemo } from "react";
import { useCountdown } from "@/hooks/useCountdownHook";
import { formatCurrency } from "@/utils/formatters/formatCurrency";
import { formatUtcToLocal } from "@/utils/formatters/formatLocalData";
import { useAuth } from "@/contexts/AuthContext";
import type {
  Product,
  ProductAuction,
  ProductAuctionSettings,
  ProductListing,
} from "@/types/product";
import type { User } from "@/types/user";
import ProductCardHorizontal from "./Horizontal/ProductCardHorizontal";
import { useNavigate, type NavigateFunction } from "react-router";
import ProductCardVertical from "./Vertical/ProductCardVertical";
import { getUserBidStatus } from "@/utils/UserBidStatus";
import { getFinalSalePrice } from "@/utils/LastProductPrice";
export interface CommonProductCardProps {
  saleType: SaleType;
  pageType?: PageType;
  product: Product;
  user: User | undefined;
  mainImageUrl: string;
  formattedDates: {
    created: {
      formattedDate: string;
      formattedTime: string;
    };
    sold: {
      formattedDate: string;
      formattedTime: string;
    };
    scheduled: {
      formattedDate: string;
      formattedTime: string;
    };
    auctionEnd: {
      formattedDate: string;
      formattedTime: string;
    };
    updated: {
      formattedDate: string;
      formattedTime: string;
    };
  };
  formattedPrices: {
    buyNow: string;
    startBid: string;
    finalPrice: string | null;
    userHighestBid: string | null;
  };
  remainingTimeEnd: string;
  remainingTimeStart: string;
  userBidStatus: ReturnType<typeof getUserBidStatus>;
  auction: ProductAuction | null;
  auctionSettings: ProductAuctionSettings | undefined;
  listings: ProductListing | null;
  navigate: NavigateFunction;
}
export type SaleType = "auction" | "buyNow";
export type CardType = "vertical" | "horizontal";
export type PageType = "search" | "myList" | "myAds";

export interface ProductCardProps {
  saleType: SaleType;
  pageType?: PageType;
  cardType?: CardType;
  product: Product;
}

const ProductCard: React.FC<ProductCardProps> = ({
  saleType,
  pageType,
  cardType,
  product,
}) => {
  const { user } = useAuth();
  const listings = useMemo(() => product.listing, [product.listing]);
  const auction = useMemo(() => listings?.auction, [listings]);
  const auctionSettings = useMemo(() => auction?.settings, [auction]);
  const userBidStatus = useMemo(
    () => getUserBidStatus(product, user?.id),
    [product, user?.id]
  );
  const finalPrice = useMemo(() => getFinalSalePrice(product), [product]);

  const formattedDates = useMemo(() => {
    const status = product.listing?.status;
    const soldAtDate =
      status === "Sold" || status === "SoldByAuction"
        ? product.listing?.updatedAt
        : null;

    const startDateToUse = auction?.startedAt ?? auctionSettings?.startDate;

    return {
      created: formatUtcToLocal(product.createdAt),
      updated: formatUtcToLocal(product.updatedAt),
      sold: formatUtcToLocal(soldAtDate),
      scheduled: formatUtcToLocal(startDateToUse),
      auctionEnd: formatUtcToLocal(auctionSettings?.endDate),
    };
  }, [
    product.createdAt,
    product.updatedAt,
    product.listing?.status,
    product.listing?.updatedAt,
    auction?.startedAt,
    auctionSettings?.startDate,
    auctionSettings?.endDate,
  ]);

  const formattedPrices = useMemo(
    () => ({
      buyNow: formatCurrency(product.listing?.buyPrice ?? 0),
      startBid: formatCurrency(auctionSettings?.startBidValue ?? 0),
      finalPrice: finalPrice ? formatCurrency(finalPrice ?? 0) : null,
      userHighestBid: userBidStatus.highestBidValue
        ? formatCurrency(userBidStatus.highestBidValue ?? 0)
        : null,
    }),
    [
      product.listing?.buyPrice,
      auctionSettings,
      finalPrice,
      userBidStatus.highestBidValue,
    ]
  );

  const remainingTimeEnd = useCountdown(
    auction?.status === "Active" ? auctionSettings?.endDate : null
  );
  const remainingTimeStart = useCountdown(
    auction?.status === "Awaiting" ? auctionSettings?.startDate : null
  );

  const mainImageUrl =
    product.images && product.images.length > 0
      ? product.images[0]
      : "../src/assets/product.jpg";
  const navigate = useNavigate();
  const commonProps: CommonProductCardProps = {
    saleType,
    pageType,
    product,
    user,
    mainImageUrl,
    formattedDates,
    formattedPrices,
    remainingTimeEnd,
    remainingTimeStart,
    userBidStatus,
    auction,
    auctionSettings,
    listings,
    navigate,
  };

  if (cardType === "vertical") {
    return <ProductCardVertical {...commonProps} />;
  }

  return <ProductCardHorizontal {...commonProps} />;
};

export default ProductCard;
