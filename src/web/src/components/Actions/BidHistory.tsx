import { History } from "lucide-react";
import { Button } from "../ui/button";
import { Popover, PopoverContent, PopoverTrigger } from "../ui/popover";
import type { ProductAuction } from "@/types/product";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "../ui/table";
import { formatCurrency } from "@/utils/formatters/formatCurrency";

type BidHistoryProps = {
  auction: ProductAuction;
};
function BidHistory({ auction }: BidHistoryProps) {
  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          size={"sm"}
          variant="outline"
          className="w-full border-emerald-700 text-emerald-700 hover:bg-emerald-50 cursor-pointer flex items-center gap-2 "
        >
          <History size={18} />
          Histórico de lances
        </Button>
      </PopoverTrigger>

      <PopoverContent
        className="w-full max-w-[22rem] sm:max-w-[28rem] md:max-w-[34rem] overflow-x-auto"
        align="center"
      >
        <div className="min-w-[400px]">
          <Table className="text-xs sm:text-sm">
            <TableCaption>Histórico de lances deste produto</TableCaption>
            <TableHeader>
              <TableRow>
                <TableHead className="min-w-[120px]">Lance em</TableHead>
                <TableHead className="w-[100px]">Status</TableHead>
                <TableHead className="text-right min-w-[80px]">Valor</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {auction.bids &&
                auction.bids
                  .slice()
                  .sort((a, b) => b.value - a.value)
                  .map((bid) => (
                    <TableRow key={bid.id}>
                      <TableCell className="truncate">
                        {new Date(bid.biddedAt).toLocaleString()}
                      </TableCell>
                      <TableCell>{bid.status}</TableCell>
                      <TableCell className="text-right">
                        {formatCurrency(bid.value)}
                      </TableCell>
                    </TableRow>
                  ))}
            </TableBody>
          </Table>
        </div>
      </PopoverContent>
    </Popover>
  );
}
export default BidHistory;
