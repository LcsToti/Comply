import { useState } from "react";
import { Dialog, DialogContent } from "../ui/dialog";
import { ZoomIn } from "lucide-react";

function ImageGallery({ images }: { images: string[] }) {
  const [mainImage, setMainImage] = useState(images[0]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isZoomed, setIsZoomed] = useState(false);
  const [mousePosition, setMousePosition] = useState({ x: 0, y: 0 });

  if (!images || images.length === 0) {
    return (
      <div className="w-full h-80 bg-gray-100 rounded-lg flex items-center justify-center text-gray-400">
        Nenhuma imagem disponível
      </div>
    );
  }

  const hasMore = images.length > 5;
  const visibleThumbs = hasMore ? images.slice(0, 4) : images;

  const handleMouseMove = (e: React.MouseEvent<HTMLDivElement>) => {
    const rect = e.currentTarget.getBoundingClientRect();
    const x = ((e.clientX - rect.left) / rect.width) * 100;
    const y = ((e.clientY - rect.top) / rect.height) * 100;
    setMousePosition({ x, y });
  };

  return (
    <>
      <div className="flex flex-col md:flex-row gap-4">
        {/* Thumbs */}
        <div className="flex flex-row md:flex-col gap-2 overflow-x-auto md:w-20 order-2 md:order-1">
          {visibleThumbs.map((img, index) => (
            <img
              key={index}
              src={img}
              alt={`Thumbnail ${index + 1}`}
              className={`w-16 h-16 md:w-20 md:h-20 object-contain rounded-md cursor-pointer border-2 transition-all hover:border-emerald-300 ${
                mainImage === img
                  ? "border-emerald-500 ring-2 ring-emerald-200"
                  : "border-gray-200"
              }`}
              onClick={() => setMainImage(img)}
            />
          ))}

          {hasMore && (
            <div
              className="w-16 h-16 md:w-20 md:h-20 rounded-md bg-gradient-to-br from-emerald-600 to-emerald-700 text-white flex items-center justify-center cursor-pointer hover:from-emerald-700 hover:to-emerald-800 transition-all font-semibold shadow-md"
              onClick={() => setIsModalOpen(true)}
            >
              +{images.length - 4}
            </div>
          )}
        </div>

        <div className="flex-1 order-1 md:order-2 flex justify-center items-center bg-white rounded-lg relative overflow-hidden group">
          {/* Indicador de zoom */}
          <div className="absolute top-4 right-4 z-10 bg-white/90 backdrop-blur-sm px-3 py-2 rounded-lg shadow-lg opacity-0 group-hover:opacity-100 transition-opacity flex items-center gap-2">
            <ZoomIn className="h-4 w-4 text-emerald-700" />
            <span className="text-xs font-medium text-gray-700">
              Passe o mouse para ampliar
            </span>
          </div>

          {/* Imagem normal */}
          <div
            className={`w-full h-64 md:h-96 lg:h-[500px] rounded-lg cursor-zoom-in relative ${
              isZoomed ? "opacity-0" : "opacity-100"
            } transition-opacity`}
            onMouseEnter={() => setIsZoomed(true)}
            onMouseMove={handleMouseMove}
          >
            <img
              src={mainImage}
              alt="Product Main Image"
              className="w-full h-full object-contain rounded-lg"
            />
          </div>

          {isZoomed && (
            <div
              className="absolute inset-0 w-full h-full cursor-zoom-in"
              onMouseMove={handleMouseMove}
              onMouseLeave={() => setIsZoomed(false)}
              style={{
                backgroundImage: `url(${mainImage})`,
                backgroundPosition: `${mousePosition.x}% ${mousePosition.y}%`,
                backgroundSize: "100%",
                backgroundRepeat: "no-repeat",
              }}
            />
          )}
        </div>
      </div>

      {/* Modal com todas as imagens */}
      <Dialog open={isModalOpen} onOpenChange={setIsModalOpen}>
        <DialogContent className="max-w-5xl max-h-[90vh] overflow-y-auto">
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-3 p-4">
            {images.map((img, index) => (
              <div
                key={index}
                className="relative group cursor-pointer"
                onClick={() => {
                  setMainImage(img);
                  setIsModalOpen(false);
                }}
              >
                <img
                  src={img}
                  alt={`Imagem ${index + 1}`}
                  className="w-full h-48 object-contain rounded-lg bg-white border-2 border-gray-200 group-hover:border-emerald-500 transition-all"
                />
                <div className="absolute inset-0 bg-black/0 group-hover:bg-black/10 transition-all rounded-lg flex items-center justify-center">
                  <ZoomIn className="h-8 w-8 text-white opacity-0 group-hover:opacity-100 transition-opacity drop-shadow-lg" />
                </div>
              </div>
            ))}
          </div>
        </DialogContent>
      </Dialog>
    </>
  );
}

export default ImageGallery;
