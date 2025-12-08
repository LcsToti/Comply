export type Product = {
    id: string;
    sellerId: string;
    title: string;
    description: string;
    locale: string;
    images: string[];
    characteristics: Record<string, string>;
    condition: number;
    category: number;
    deliveryPreference: string;
    watchListCount: number;
    featured: boolean;
    expirationFeatureDate: string;
    createdAt: string;
    updatedAt: string;
    listing: ProductListing;
    qna: QnASection;
}

export type ProductListing = {
    id: string;
    status: string;
    buyPrice: number;
    isAuctionActive: boolean;
    isProcessingPurchase: boolean;
    buyerId: string | null;
    auctionId: string | null;
    listedAt: string;
    updatedAt: string;
    auction: ProductAuction | null;
}

export type ProductAuction = {
    id: string;
    status: string;
    isProcessingBid: boolean;
    editedAt: string | null;
    startedAt: string;
    endedAt: string;
    settings: ProductAuctionSettings;
    bids: ProductBid[] | null;
}

export type ProductAuctionSettings = {
    startBidValue: number;
    winBidValue: number;
    startDate: string;
    endDate: string;
}

export type ProductBid = {
    id: string;
    bidderId: string;
    value: number;
    status: string;
    biddedAt: string;
    outbiddedAt: string | null;
    wonAt: string | null;
}

export type QnASection = {
    totalQuestions: number;
    questions: Question[];
}

export type Question = {
    questionId: string;
    userId: string;
    questionText: string;
    askedAt: string;
    answer: Answer | null;
}

export type Answer = {
    answerText: string;
    answeredAt: string;
}

export type ProductPaginatedList<T> = {
    pageNumber: number,
    pageSize: number,
    totalCount: number,
    totalPages: number,
    items: T[],
    hasPreviousPage: boolean,
    hasNextPage: boolean
}

export const Categories = {
    Electronics: "Electronics",
    Computers: "Computers",
    HomeAppliances: "HomeAppliances",
    FurnitureDecor: "FurnitureDecor",
    FashionBeauty: "FashionBeauty",
    Sports: "Sports",
    Collectibles: "Collectibles",
    Tools: "Tools",
    Games: "Games",
    Services: "Services",
    Others: "Others",
}

export const ProductCondition = {
    New: "New",
    Used: "Used",
    NotWorking: "NotWorking",
    Refurbished: "Refurbished",
}

export const ProductSortBy = {
    Newest: "Newest",
    Oldest: "Oldest",
    MostBids: "MostBids",
    LessBids: "LessBids",
    Popularity: "Popularity",
    PriceAsc: "PriceAsc",
    PriceDesc: "PriceDesc",
    AuctionStartingSoon: "AuctionStartingSoon",
    AuctionEndingSoon: "AuctionEndingSoon",
}

export type Categories = typeof Categories[keyof typeof Categories]
export type ProductCondition = typeof ProductCondition[keyof typeof ProductCondition]
export type ProductSortBy = typeof ProductSortBy[keyof typeof ProductSortBy]

export const CategoriesLabels: Record<Categories, string> = {
    [Categories.Electronics]: "Eletrônicos",
    [Categories.Computers]: "Computadores",
    [Categories.HomeAppliances]: "Eletrodomésticos",
    [Categories.FurnitureDecor]: "Móveis e Decoração",
    [Categories.FashionBeauty]: "Moda e Beleza",
    [Categories.Sports]: "Esportes",
    [Categories.Collectibles]: "Colecionáveis",
    [Categories.Tools]: "Ferramentas",
    [Categories.Games]: "Jogos",
    [Categories.Services]: "Serviços",
    [Categories.Others]: "Outros",
}
export const ProductConditionLabels: Record<ProductCondition, string> = {
    [ProductCondition.New]: "Novo",
    [ProductCondition.Used]: "Usado",
    [ProductCondition.NotWorking]: "Não funciona",
    [ProductCondition.Refurbished]: "Recondicionado",
}
export const ProductSortByLabels: Record<ProductSortBy, string> = {
    [ProductSortBy.Newest]: "Mais recentes",
    [ProductSortBy.Oldest]: "Mais antigos",
    [ProductSortBy.MostBids]: "Mais lances",
    [ProductSortBy.LessBids]: "Menos lances",
    [ProductSortBy.Popularity]: "Mais relevantes",
    [ProductSortBy.PriceAsc]: "Menor preço",
    [ProductSortBy.PriceDesc]: "Maior preço",
    [ProductSortBy.AuctionStartingSoon]: "Leilões começando em breve",
    [ProductSortBy.AuctionEndingSoon]: "Leilões terminando em breve",
}