interface BaseCardProps {
  imageUrl: string;
  infoColumn: React.ReactNode;
  actionColumn: React.ReactNode;
}

const ProductCardHorizontalBase: React.FC<BaseCardProps> = ({
  imageUrl,
  infoColumn,
  actionColumn,
}) => {
  return (
    <div className="group flex flex-col md:flex-row w-full rounded-2xl overflow-hidden border border-gray-200 bg-white hover:border-emerald-700/20  transition-all duration-300">
      <div className="relative h-40 w-full md:w-48 md:h-48 bg-gradient-to-br from-gray-50 to-gray-100 flex-shrink-0 overflow-hidden">
        <img
          src={imageUrl}
          alt="Produto"
          className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
        />
        <div className="absolute inset-0 bg-gradient-to-t from-black/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
      </div>

      <div className="flex flex-col md:flex-row flex-1 min-w-0">
        <div className="flex flex-1 flex-col p-5 md:p-6 min-w-0 gap-2.5">
          {infoColumn}
        </div>
        <div className="flex flex-1 flex-col p-5 md:p-6 pt-0 md:pt-6 justify-end md:justify-between items-stretch md:items-end md:min-w-fit gap-3 border-t md:border-t-0 md:border-l border-gray-100/80">
          {actionColumn}
        </div>
      </div>
    </div>
  );
};

export default ProductCardHorizontalBase;
