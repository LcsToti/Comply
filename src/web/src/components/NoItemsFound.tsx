export type NoItemsFoundProps = {
  Icon: React.ElementType;
  title: string;
  description: string;
};
function NoItemsFound({ Icon, title, description }: NoItemsFoundProps) {
  return (
    <div className="flex flex-col items-center justify-center p-12 text-center bg-gray-50 rounded-lg border-1 border-dashed border-gray-300">
      <Icon className="h-6 w-6 md:h-16 md:w-16 text-gray-400 mb-4" />
      <h3 className="text-sm md:text-xl font-semibold text-gray-700 mb-2">
        {title}
      </h3>
      <p className="text-gray-500 text-xs md:text-sm">{description}</p>
    </div>
  );
}

export default NoItemsFound;
