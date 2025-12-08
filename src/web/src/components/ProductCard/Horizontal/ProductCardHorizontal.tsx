import type { CommonProductCardProps } from "../ProductCard";
import MyAdsAction from "./MyAds/MyAdsAction";
import MyAdsInfo from "./MyAds/MyAdsInfo";
import MyListAction from "./MyList/MyListAction";
import MyListInfo from "./MyList/MyListInfo";
import ProductCardHorizontalBase from "./ProductCardHorizontalBase";
import SearchAuctionAction from "./Search/SearchAuctionAction";
import SearchAuctionInfo from "./Search/SearchAuctionInfo";
import SearchBuyNowAction from "./Search/SearchBuyNowAction";
import SearchBuyNowInfo from "./Search/SearchBuyNowInfo";

const ProductCardHorizontal: React.FC<CommonProductCardProps> = (props) => {
  const { pageType, saleType, mainImageUrl } = props;

  const allProps = { ...props };

  if (pageType === "search") {
    return saleType === "auction" ? (
      <ProductCardHorizontalBase
        imageUrl={mainImageUrl}
        infoColumn={<SearchAuctionInfo {...allProps} />}
        actionColumn={<SearchAuctionAction {...allProps} />}
      />
    ) : (
      <ProductCardHorizontalBase
        imageUrl={mainImageUrl}
        infoColumn={<SearchBuyNowInfo {...allProps} />}
        actionColumn={<SearchBuyNowAction {...allProps} />}
      />
    );
  }

  if (pageType === "myAds") {
    return (
      <ProductCardHorizontalBase
        imageUrl={mainImageUrl}
        infoColumn={<MyAdsInfo {...allProps} />}
        actionColumn={<MyAdsAction {...allProps} />}
      />
    );
  }

  if (pageType === "myList") {
    return (
      <ProductCardHorizontalBase
        imageUrl={mainImageUrl}
        infoColumn={<MyListInfo {...allProps} />}
        actionColumn={<MyListAction {...allProps} />}
      />
    );
  }

  return <div>Tipo de card desconhecido.</div>;
};

export default ProductCardHorizontal;
