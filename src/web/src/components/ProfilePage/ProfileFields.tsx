import { Pencil, Save } from "lucide-react";
import { Button } from "../ui/button";
import { useState } from "react";

export type ProfileFieldsProps = {
  label: string;
  value: string | number | undefined;
  editable?: boolean;
  onEdit?: (newValue: string) => void;
};

function ProfileFields({
  label,
  value,
  editable = false,
  onEdit = () => {},
}: ProfileFieldsProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [current, setCurrent] = useState(value);

  const handleSave = () => {
    onEdit(String(current ?? ""));
    setIsEditing(false);
  };

  return (
    <div className="flex items-center justify-between rounded-lg bg-gray-50 p-4">
      <div>
        <span className="text-sm text-gray-500">{label}</span>
        {isEditing ? (
          <input
            className="mt-1 w-full rounded-md border border-gray-300 px-2 py-1 text-base text-gray-900"
            value={current}
            onChange={(e) => setCurrent(e.target.value)}
          />
        ) : (
          <p className="text-base font-medium text-gray-900">{value}</p>
        )}
      </div>
      <div className="flex items-center gap-3">
        {editable && (
          <Button
            variant="ghost"
            size="icon"
            onClick={() => (isEditing ? handleSave() : setIsEditing(true))}
            className="text-gray-600 hover:text-gray-900"
          >
            {isEditing ? (
              <Save className="h-4 w-4 text-emerald-600" />
            ) : (
              <Pencil className="h-4 w-4" />
            )}
          </Button>
        )}
      </div>
    </div>
  );
}

export default ProfileFields;
