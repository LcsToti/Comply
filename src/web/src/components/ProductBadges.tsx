import { Clock } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import type { StatusBadge } from "@/hooks/useProductBadges";

type ProductBadgesProps = {
  primary: StatusBadge | null;
  additional: StatusBadge[];
  className?: string;
};

export const ProductBadges: React.FC<ProductBadgesProps> = ({
  primary,
  additional,
  className = "",
}) => {
  return (
    <div className={`flex flex-wrap gap-1.5 sm:gap-2 ${className}`}>
      {primary && (
        <Badge
          variant={primary.variant}
          className={`text-xs ${primary.pulse ? "animate-pulse" : ""}`}
        >
          {primary.pulse && <Clock size={10} className="sm:size-3 mr-1" />}
          {primary.label}
        </Badge>
      )}
      {additional.map((badge, index) => (
        <Badge key={index} variant={badge.variant} className="text-xs">
          {badge.label}
        </Badge>
      ))}
    </div>
  );
};
