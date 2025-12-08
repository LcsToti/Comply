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
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import {
  restrictToVerticalAxis,
  restrictToParentElement,
} from "@dnd-kit/modifiers";
import { useDropzone } from "react-dropzone";
import { SortableItem, type ItemData } from "@/components/SortableItem";
import { useMemo } from "react";
import { cn } from "@/lib/utils";
import { Upload } from "lucide-react";
import { useController, useFormContext } from "react-hook-form";
import type { CreateProductFormValues } from "@/schemas/createProductSchema";
import { v4 as uuidv4 } from "uuid";

export default function Step2() {
  const { formState: errors } = useFormContext<CreateProductFormValues>();
  const { field } = useController({
    name: "ImageUrls",
  });

  const currentFiles = field.value || [];
  const MAX_IMAGES = 10;

  const images: ItemData[] = useMemo(() => {
    return currentFiles.map((file: File & { __id?: string }) => ({
      id: file.__id ?? `${file.name}-${file.lastModified}-${file.size}`,
      file,
      urlPreview: URL.createObjectURL(file),
    }));
  }, [currentFiles]);

  //dnd-kit setup
  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: { distance: 8 },
    })
  );

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event;
    if (!over) return;

    const files = (field.value || []) as (File & { __id?: string })[];
    const activeId = active.id as string;
    const overId = over.id as string;

    const oldIndex = files.findIndex((f) => f.__id === activeId);
    const newIndex = files.findIndex((f) => f.__id === overId);
    if (oldIndex === -1 || newIndex === -1) {
      console.error("dnd-kit: Índices não encontrados.");
      return;
    }
    const reorderedFiles = arrayMove(files, oldIndex, newIndex);
    field.onChange(reorderedFiles);
  }
  const handleRemoveItem = (id: string) => {
    const imageToRemove = images.find((img) => img.id === id);
    if (imageToRemove) {
      URL.revokeObjectURL(imageToRemove.urlPreview);
    }

    const newFiles = currentFiles.filter(
      (f: File & { __id?: string }) => f.__id !== id
    );

    field.onChange(newFiles);
  };

  const onDrop = (acceptedFiles: File[]) => {
    const remainingSlots = MAX_IMAGES - currentFiles.length;
    if (remainingSlots <= 0) return;

    if (acceptedFiles?.length) {
      const filesToProcess = acceptedFiles.slice(0, remainingSlots);

      const uniqueFiles = filesToProcess.map((file) => {
        const id = `${file.name}-${file.lastModified}-${file.size}-${uuidv4()}`;
        return Object.assign(file, { __id: id }); // adiciona metadado ao File
      });

      const newFiles = [...currentFiles, ...uniqueFiles];
      field.onChange(newFiles);
    }
  };

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: { "image/jpeg": [], "image/png": [], "image/webp": [] },
    maxFiles: MAX_IMAGES,
    disabled: currentFiles.length >= MAX_IMAGES,
  });

  const imageIds = useMemo(() => images.map((img) => img.id), [images]);

  return (
    <div className="flex flex-col justify-center items-center gap-8 min-h-full">
      <div className="grid w-full gap-2.5">
        <h2 className="text-emerald-700 font-bold text-2xl">
          Adicione imagens ao seu anúncio
        </h2>
        <div
          {...getRootProps()}
          className={cn(
            "flex items-center justify-center w-full h-32 border-2 border-dashed rounded-lg cursor-pointer bg-gray-50 hover:bg-gray-100",
            isDragActive ? "border-emerald-500 bg-emerald-50" : ""
          )}
        >
          <input
            {...getInputProps({ name: field.name, onBlur: field.onBlur })}
          />

          <div className="flex flex-col items-center justify-center pt-5 pb-6">
            <Upload className="w-8 h-8 mb-4 text-gray-500" />
            {isDragActive ? (
              <p className="font-semibold text-emerald-600">
                Solte os arquivos aqui!
              </p>
            ) : (
              <p className="mb-2 text-sm text-gray-500">
                <span className="font-semibold">Clique ou arraste</span> para
                adicionar
              </p>
            )}
          </div>
        </div>
        {errors.errors && (
          <p className="text-sm text-red-500">
            {errors.errors.ImageUrls?.message}
          </p>
        )}
      </div>

      <DndContext
        sensors={sensors}
        collisionDetection={closestCenter}
        onDragEnd={handleDragEnd}
        modifiers={[restrictToVerticalAxis, restrictToParentElement]}
      >
        <SortableContext
          items={imageIds}
          strategy={verticalListSortingStrategy}
        >
          <div className="flex flex-col w-full gap-3">
            {images.map((item, index) => (
              <SortableItem
                key={item.id}
                item={item}
                onRemove={handleRemoveItem}
                index={index}
              />
            ))}
          </div>
        </SortableContext>
      </DndContext>
    </div>
  );
}
