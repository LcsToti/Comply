import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { GripVertical, Trash } from "lucide-react";
import { cn } from "@/lib/utils";

export type ImageItem = {
  id: string;
  url: string;
  file?: File;
};

interface EditorSortableItemProps {
  item: ImageItem;
  onRemove: (id: string) => void;
  isOverlay?: boolean;
}

export function EditorSortableItem({
  item,
  onRemove,
  isOverlay,
}: EditorSortableItemProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: item.id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition: transition || undefined,
  };

  return (
    <div
      ref={setNodeRef}
      style={style}
      className={cn(
        "relative group w-24 h-24 rounded-lg shadow-sm flex-shrink-0",
        isDragging && "opacity-50 z-10",
        isOverlay && "shadow-lg"
      )}
    >
      <img
        src={item.url}
        alt="Preview"
        className="w-full h-full object-cover rounded-lg"
      />

      {/* Botão de Remover */}
      <button
        type="button"
        onClick={() => onRemove(item.id)}
        className="absolute -top-2 -right-2 z-10 p-1 bg-red-500 text-white rounded-full shadow-md opacity-0 group-hover:opacity-100 cursor-pointer hover:scale-105 transition-all"
        aria-label="Remover imagem"
      >
        <Trash className="w-4 h-4" />
      </button>

      {/* Alça de Arraste */}
      <button
        type="button"
        {...attributes}
        {...listeners}
        className="absolute -top-2 -left-2 p-1 bg-gray-900/50 text-white rounded-full opacity-0 group-hover:opacity-100 transition-opacity cursor-grab active:cursor-grabbing"
        aria-label="Reordenar imagem"
      >
        <GripVertical className="w-4 h-4" />
      </button>
    </div>
  );
}
