import { useState } from "react";
import { Copy, Check } from "lucide-react";
import { Dialog, DialogContent, DialogTitle } from "../ui/dialog";
import { Input } from "../ui/input";
import { Button } from "../ui/button";

type ShareProductProps = {
  open: boolean;
  onOpenChange?: (open: boolean) => void;
};

function ShareProduct({ open, onOpenChange }: ShareProductProps) {
  const [copied, setCopied] = useState(false);
  const productUrl = typeof window !== "undefined" ? window.location.href : "";

  const handleCopy = async () => {
    await navigator.clipboard.writeText(productUrl);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent showCloseButton={false}>
        <DialogTitle>Compartilhar Produto</DialogTitle>
        <div className="flex flex-col gap-4">
          <div className="flex flex-row items-center gap-2">
            <Input
              type="text"
              readOnly
              value={productUrl}
              onFocus={(e) => e.target.select()}
              className="text-sm"
            />
            <Button
              size={"sm"}
              variant="outline"
              onClick={handleCopy}
              className="flex items-center gap-1 cursor-pointer"
              title="Copiar link"
            >
              {copied ? (
                <>
                  <Check size={18} className="text-emerald-600" />
                  Copiado
                </>
              ) : (
                <>
                  <Copy size={18} />
                  Copiar
                </>
              )}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}

export default ShareProduct;
