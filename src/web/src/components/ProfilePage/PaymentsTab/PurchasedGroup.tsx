export type PurchasedGroupProps = {
  date: string;
  children: React.ReactNode;
};

function PurchasedGroup({ date, children }: PurchasedGroupProps) {
  return (
    <section className="mb-8">
      <div className="mb-2 flex items-baseline justify-between">
        <h3 className="text-lg font-semibold text-gray-800">{date}</h3>
      </div>
      <div className="overflow-hidden flex flex-col gap-2">{children}</div>
    </section>
  );
}

export default PurchasedGroup;
