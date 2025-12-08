import Footer from "@/components/Footer";
import Header from "@/components/Header";
import ProductTab from "@/components/ProductEdit/ProductTab";
import { Skeleton } from "@/components/ui/skeleton";
import { useProductQuery } from "@/hooks/products/useProductsQueries";
import { useParams } from "react-router";

function EditProduct() {
  const { id } = useParams<{ id: string }>();
  const { data: productData, isPending } = useProductQuery(id!);

  return (
    <>
      <Header />
      <main className="w-3/5 flex flex-col min-h-screen mx-auto my-10 gap-10">
        <h1 className="text-4xl font-bold text-left">Editar produto</h1>
        {isPending ? (
          <Skeleton className="w-500 h-800" />
        ) : productData ? (
          <ProductTab product={productData} />
        ) : (
          <p>Produto não encontrado.</p>
        )}
      </main>
      <Footer />
    </>
  );
}

export default EditProduct;
