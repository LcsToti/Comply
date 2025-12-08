import {
  DndContext,
  closestCenter,
  PointerSensor,
  useSensor,
  useSensors,
  type DragEndEvent,
} from "@dnd-kit/core";
import {
  arrayMove,
  SortableContext,
  horizontalListSortingStrategy,
} from "@dnd-kit/sortable";
import {
  restrictToHorizontalAxis,
  restrictToParentElement,
} from "@dnd-kit/modifiers";
import { useDropzone } from "react-dropzone";
import { useMemo, useEffect } from "react";
import { cn } from "@/lib/utils";
import { Upload } from "lucide-react";
import { v4 as uuidv4 } from "uuid";
import { EditorSortableItem, type ImageItem } from "./EditorSortableItem";

interface ProductImageEditorProps {
  value: ImageItem[];
  onChange: (images: ImageItem[]) => void;
  maxImages?: number;
  disabled?: boolean;
}

export default function ProductImageEditor({
  value: images = [],
  onChange,
  maxImages = 10,
  disabled = false,
}: ProductImageEditorProps) {
  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: { distance: 8 },
    })
  );

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event;
    if (!over || active.id === over.id) return;

    const oldIndex = images.findIndex((img) => img.id === active.id);
    const newIndex = images.findIndex((img) => img.id === over.id);

    if (oldIndex === -1 || newIndex === -1) return;

    const newOrder = arrayMove(images, oldIndex, newIndex);
    onChange(newOrder);
  }

  const handleRemoveItem = (id: string) => {
    const imageToRemove = images.find((img) => img.id === id);
    if (imageToRemove?.file) {
      URL.revokeObjectURL(imageToRemove.url);
    }
    const newImages = images.filter((img) => img.id !== id);
    onChange(newImages);
  };

  const onDrop = (acceptedFiles: File[]) => {
    const remainingSlots = maxImages - images.length;
    if (remainingSlots <= 0) return;

    const filesToProcess = acceptedFiles.slice(0, remainingSlots);

    const newImageItems: ImageItem[] = filesToProcess.map((file) => ({
      id: uuidv4(),
      url: URL.createObjectURL(file),
      file: file,
    }));

    onChange([...images, ...newImageItems]);
  };

  useEffect(() => {
    return () => {
      images.forEach((item) => {
        if (item.file) {
          URL.revokeObjectURL(item.url);
        }
      });
    };
  }, []);

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: { "image/jpeg": [], "image/png": [], "image/webp": [] },
    maxFiles: maxImages,
    disabled: images.length >= maxImages || disabled,
  });

  const imageIds = useMemo(() => images.map((img) => img.id), [images]);

  return (
    <div className="w-full flex gap-5 flex-row space-y-4">
      {/* Dropzone */}
      <div
        {...getRootProps()}
        className={cn(
          "flex flex-1 items-center justify-center w-full h-32 border-2 border-dashed rounded-lg cursor-pointer bg-gray-50 hover:bg-gray-100",
          isDragActive ? "border-emerald-500 bg-emerald-50" : "",
          (images.length >= maxImages || disabled) &&
            "cursor-not-allowed opacity-60"
        )}
      >
        <input {...getInputProps()} />
        <div className="flex flex-col items-center justify-center pt-5 pb-6">
          <Upload className="w-8 h-8 mb-4 text-gray-500" />
          <p className="mb-2 text- text-gray-500">
            <span className="font-semibold">Clique ou arraste</span> (Máx.{" "}
            {maxImages})
          </p>
        </div>
      </div>

      {/* Lista de Imagens */}
      {images.length > 0 && (
        <DndContext
          sensors={sensors}
          collisionDetection={closestCenter}
          onDragEnd={handleDragEnd}
          modifiers={[restrictToHorizontalAxis, restrictToParentElement]}
        >
          <SortableContext
            items={imageIds}
            strategy={horizontalListSortingStrategy}
          >
            <div className="flex flex-4 flex-row w-full gap-4 p-2 overflow-x-auto min-h-[8rem] bg-gray-50 rounded-lg border">
              {images.map((item) => (
                <EditorSortableItem
                  key={item.id}
                  item={item}
                  onRemove={handleRemoveItem}
                />
              ))}
            </div>
          </SortableContext>
        </DndContext>
      )}
    </div>
  );
}
