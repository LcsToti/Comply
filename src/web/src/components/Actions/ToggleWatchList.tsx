import { Eye, EyeClosed } from "lucide-react";
import { Button } from "../ui/button";
import { useWatchlistQuery } from "@/hooks/notifications/useWatchListQueries";
import {
  useAddToWatchlistMutation,
  useRemoveFromWatchlistMutation,
} from "@/hooks/notifications/useWatchListMutations";

interface ToggleWatchListProps {
  productId: string;
  listingId: string;
}

export default function ToggleWatchList({
  productId,
  listingId,
}: ToggleWatchListProps) {
  const { data: myWatchlist, isFetching } = useWatchlistQuery();

  const addToWatchlist = useAddToWatchlistMutation();
  const removeFromWatchlist = useRemoveFromWatchlistMutation();

  const isWatching = myWatchlist?.includes(productId) ?? false;

  const isLoading =
    addToWatchlist.isPending || removeFromWatchlist.isPending || isFetching;

  const handleToggle = () => {
    if (isLoading) return;

    if (isWatching) {
      removeFromWatchlist.mutate({ ProductId: productId });
    } else {
      addToWatchlist.mutate({ ProductId: productId, ListingId: listingId });
    }
  };

  return (
    <Button
      variant="outline"
      size={"sm"}
      className="cursor-pointer"
      onClick={handleToggle}
      disabled={isLoading}
    >
      {isWatching ? (
        <Eye className="size-5 stroke-emerald-700" />
      ) : (
        <EyeClosed className="size-5 text-gray-500" />
      )}
      <span className="hidden sm:inline ml-2 text-sm">Ficar de olho</span>
    </Button>
  );
}
