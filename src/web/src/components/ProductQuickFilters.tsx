import type { GetFilteredProductsParams } from "@/api/products/products";
import { useNavigate } from "react-router";
import nobidImg from "@/assets/images/nobid.png"
import novosImg from "@/assets/images/novos.png"
import recondicionadoImg from "@/assets/images/recondicionado.png"
import usadosImg from "@/assets/images/usados.png"
import eletronicosImg from "@/assets/images/eletronicos.jpg"
import casaImg from "@/assets/images/casa.jpg"

const filters: {
  title: string;
  image: string;
  params: GetFilteredProductsParams;
}[] = [
  {
    title: "Sem Lances",
    image: nobidImg,
    params: { NoBidsOnly: true },
  },
  {
    title: "Novos",
    image: novosImg,
    params: { Condition: "New" },
  },
  {
    title: "Recondicionados",
    image: recondicionadoImg,
    params: { Condition: "Refurbished" },
  },
  {
    title: "Usados",
    image: usadosImg,
    params: { Condition: "Used" },
  },
  {
    title: "Eletrônicos",
    image: eletronicosImg,
    params: { Category: "Electronics" },
  },
  {
    title: "Casa",
    image: casaImg,
    params: { Category: "FurnitureDecor" },
  },
];

export default function ProductFilters() {
  const navigate = useNavigate();

  const handleFilterClick = (params: GetFilteredProductsParams) => {
    const query = new URLSearchParams();

    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        query.append(key.toLowerCase(), String(value));
      }
    });
    navigate(`/search?${query.toString()}`);
  };

  return (
    <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
      {filters.map((filter, idx) => (
        <div
          key={idx}
          onClick={() => handleFilterClick(filter.params)}
          className="border rounded-lg shadow-sm cursor-pointer hover:shadow-md hover:border-emerald-500 hover:ring-1 hover:ring-emerald-400 transition"
        >
          <img
            src={filter.image}
            alt={filter.title}
            className="w-full h-15 md:h-32 object-cover rounded-t-lg"
          />
          <p className="p-3 text-sm font-semibold text-center">
            {filter.title}
          </p>
        </div>
      ))}
    </div>
  );
}
