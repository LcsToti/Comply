import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { Button } from "@/components/ui/button";
import { GripVertical, Trash2 } from "lucide-react";

export interface ItemData {
  id: string;
  file: File;
  urlPreview: string;
}

interface SortableItemProps {
  item: ItemData;
  onRemove: (id: string) => void;
  index: number;
}

export function SortableItem({ item, onRemove, index }: SortableItemProps) {
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
    transition,
    opacity: isDragging ? 0.5 : 1,
    zIndex: isDragging ? 10 : "auto",
  };

  const isCover = index === 0;

  return (
    <div
      ref={setNodeRef}
      style={style}
      className={`
        flex items-center gap-3 rounded-lg border bg-white p-2 shadow-sm
        ${isCover ? "border-emerald-500 border-2" : ""}
      `}
    >
      <Button
        variant="ghost"
        size="icon"
        {...attributes}
        {...listeners}
        className="cursor-grab active:cursor-grabbing"
      >
        <div className="flex flex-row items-center gap-1">
          <span className="text-lg font-semibold text-muted-foreground">
            {index + 1}
          </span>{" "}
          <GripVertical className="h-5 w-5 text-muted-foreground" />
        </div>
      </Button>

      <img
        src={item.urlPreview}
        alt={item.file.name}
        className="h-12 w-12 rounded-md object-cover"
      />

      {/* 3. Nome do Arquivo */}
      <div className="flex-1 truncate text-sm">{item.file.name}</div>

      {/* 4. Ações (Ex: Deletar) */}
      <Button
        variant="ghost"
        size="icon"
        onClick={() => onRemove(item.id)}
        className="text-red-500 hover:text-red-600"
      >
        <Trash2 className="h-4 w-4" />
      </Button>

      {/* (Você pode adicionar um botão para "Definir como principal" aqui) */}
    </div>
  );
}
