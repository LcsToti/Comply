import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { ScrollArea } from "@/components/ui/scroll-area";
import { FileText, AlertCircle } from "lucide-react";
import { cn } from "@/lib/utils";

interface TermsOfUseModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onAccept: () => void;
  onDecline?: () => void;
  title?: string;
  requireScroll?: boolean;
}

export function TermsOfUseModal({
  open,
  onOpenChange,
  onAccept,
  onDecline,
  title = "Termos de Uso",
  requireScroll = false,
}: TermsOfUseModalProps) {
  const [hasAgreed, setHasAgreed] = useState(false);
  const [hasScrolledToBottom, setHasScrolledToBottom] =
    useState(!requireScroll);

  const handleAccept = () => {
    if (hasAgreed && hasScrolledToBottom) {
      onAccept();
      handleClose();
    }
  };

  const handleDecline = () => {
    if (onDecline) {
      onDecline();
    }
    handleClose();
  };

  const handleClose = () => {
    setHasAgreed(false);
    setHasScrolledToBottom(!requireScroll);
    onOpenChange(false);
  };

  const handleScroll = (e: React.UIEvent<HTMLDivElement>) => {
    if (!requireScroll) return;

    const target = e.target as HTMLDivElement;
    const isAtBottom =
      target.scrollHeight - target.scrollTop <= target.clientHeight + 10;

    if (isAtBottom && !hasScrolledToBottom) {
      setHasScrolledToBottom(true);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] flex flex-col p-0">
        <DialogHeader className="px-6 pt-6 pb-4 border-b">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-full bg-emerald-100 flex items-center justify-center">
              <FileText className="w-5 h-5 text-emerald-600" />
            </div>
            <div>
              <DialogTitle className="text-xl">{title}</DialogTitle>
              <DialogDescription className="text-sm">
                Leia atentamente antes de continuar
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        {/* Scrollable Content */}
        <ScrollArea
          className="flex-1 px-6 py-4 overflow-auto"
          onScrollCapture={handleScroll}
        >
          <div className="space-y-6 text-sm text-gray-700 leading-relaxed">
            {/* Introdução */}
            <section>
              <h3 className="font-semibold text-base text-gray-900 mb-2">
                1. Aceitação dos Termos
              </h3>
              <p>
                Ao acessar e utilizar esta plataforma de leilões online, você
                concorda em cumprir e estar vinculado aos seguintes termos e
                condições de uso. Se você não concordar com alguma parte destes
                termos, não deverá utilizar nossos serviços.
              </p>
            </section>

            {/* Elegibilidade */}
            <section>
              <h3 className="font-semibold text-base text-gray-900 mb-2">
                2. Elegibilidade e Cadastro
              </h3>
              <p className="mb-2">Para utilizar nossa plataforma, você deve:</p>
              <ul className="list-disc pl-5 space-y-1">
                <li>Ter no mínimo 18 anos de idade</li>
                <li>
                  Fornecer informações verdadeiras e precisas durante o cadastro
                </li>
                <li>
                  Manter suas credenciais de acesso seguras e confidenciais
                </li>
                <li>
                  Notificar imediatamente sobre qualquer uso não autorizado
                </li>
              </ul>
            </section>

            {/* Conduta do Usuário */}
            <section>
              <h3 className="font-semibold text-base text-gray-900 mb-2">
                3. Conduta do Usuário
              </h3>
              <p className="mb-2">
                Ao utilizar a plataforma, você concorda em:
              </p>
              <ul className="list-disc pl-5 space-y-1">
                <li>Não violar leis locais, estaduais ou federais</li>
                <li>Não praticar fraude ou fornecer informações falsas</li>
                <li>Não interferir no funcionamento da plataforma</li>
                <li>Respeitar os direitos de propriedade intelectual</li>
                <li>Não utilizar a plataforma para fins ilícitos</li>
              </ul>
            </section>

            {/* Leilões e Transações */}
            <section>
              <h3 className="font-semibold text-base text-gray-900 mb-2">
                4. Leilões e Transações
              </h3>
              <p className="mb-2">
                <strong>4.1 Lances:</strong> Todos os lances realizados são
                considerados vinculantes. Ao fazer um lance, você se compromete
                a comprar o item pelo valor ofertado caso seja o vencedor.
              </p>
              <p className="mb-2">
                <strong>4.2 Pagamento:</strong> O pagamento deve ser realizado
                dentro do prazo estabelecido através dos métodos aceitos pela
                plataforma.
              </p>
              <p className="mb-2">
                <strong>4.3 Taxas:</strong> A plataforma cobra uma taxa de
                serviço sobre transações concluídas, conforme especificado na
                área de vendedor.
              </p>
              <p>
                <strong>4.4 Entrega:</strong> Vendedores são responsáveis pela
                entrega dos itens. Compradores devem confirmar o recebimento
                através do código de entrega.
              </p>
            </section>

            {/* Responsabilidades do Vendedor */}
            <section>
              <h3 className="font-semibold text-base text-gray-900 mb-2">
                5. Responsabilidades do Vendedor
              </h3>
              <p className="mb-2">Os vendedores devem:</p>
              <ul className="list-disc pl-5 space-y-1">
                <li>Fornecer descrições precisas e honestas dos produtos</li>
                <li>Incluir fotos reais e representativas dos itens</li>
                <li>Estabelecer preços e condições claras</li>
                <li>Enviar itens dentro do prazo acordado</li>
                <li>Responder prontamente a dúvidas de compradores</li>
              </ul>
            </section>

            {/* Propriedade Intelectual */}
            <section>
              <h3 className="font-semibold text-base text-gray-900 mb-2">
                6. Propriedade Intelectual
              </h3>
              <p>
                Todo o conteúdo da plataforma, incluindo mas não limitado a
                textos, gráficos, logos, ícones, imagens, software e design, é
                propriedade exclusiva da empresa e protegido por leis de
                direitos autorais e marcas registradas.
              </p>
            </section>

            {/* Limitação de Responsabilidade */}
            <section>
              <h3 className="font-semibold text-base text-gray-900 mb-2">
                7. Limitação de Responsabilidade
              </h3>
              <p className="mb-2">
                A plataforma atua como intermediária entre compradores e
                vendedores. Não nos responsabilizamos por:
              </p>
              <ul className="list-disc pl-5 space-y-1">
                <li>Qualidade, segurança ou legalidade dos itens anunciados</li>
                <li>Capacidade dos vendedores de completar transações</li>
                <li>Capacidade dos compradores de efetuar pagamentos</li>
                <li>Danos indiretos ou consequenciais</li>
                <li>Perda de dados ou interrupções de serviço</li>
              </ul>
            </section>

            {/* Resolução de Disputas */}
            <section>
              <h3 className="font-semibold text-base text-gray-900 mb-2">
                8. Resolução de Disputas
              </h3>
              <p>
                Em caso de disputas entre usuários, oferecemos um sistema de
                tickets de suporte. Casos não resolvidos podem ser encaminhados
                para mediação ou arbitragem conforme a legislação aplicável.
              </p>
            </section>

            {/* Modificações */}
            <section>
              <h3 className="font-semibold text-base text-gray-900 mb-2">
                9. Modificações dos Termos
              </h3>
              <p>
                Reservamo-nos o direito de modificar estes termos a qualquer
                momento. Alterações significativas serão notificadas por email
                ou através da plataforma. O uso continuado após as modificações
                constitui aceitação dos novos termos.
              </p>
            </section>

            {/* Contato */}
            <section>
              <h3 className="font-semibold text-base text-gray-900 mb-2">
                10. Informações de Contato
              </h3>
              <p className="mb-2">
                Para dúvidas sobre estes termos, entre em contato:
              </p>
              <ul className="list-none space-y-1">
                <li>Email: suporte@comply.com</li>
                <li>Telefone: (21) some-daqui</li>
              </ul>
            </section>
          </div>
        </ScrollArea>

        {/* Footer com Checkbox e Botões */}
        {requireScroll && (
          <DialogFooter className="px-6 py-4 border-t bg-gray-50 flex-col sm:flex-col gap-4">
            {/* Aviso de scroll se necessário */}
            {requireScroll && !hasScrolledToBottom && (
              <div className="flex items-center gap-2 text-xs text-amber-700 bg-amber-50 px-3 py-2 rounded border border-amber-200">
                <AlertCircle className="w-4 h-4 flex-shrink-0" />
                <span>Role até o final do documento para continuar</span>
              </div>
            )}

            {/* Checkbox de aceitação */}
            <div className="flex items-start gap-3 w-full">
              <Checkbox
                id="terms-agree"
                checked={hasAgreed}
                onCheckedChange={(checked) => setHasAgreed(checked === true)}
                disabled={!hasScrolledToBottom}
                className="mt-1"
              />
              <label
                htmlFor="terms-agree"
                className={cn(
                  "text-sm leading-relaxed cursor-pointer",
                  !hasScrolledToBottom && "text-gray-400"
                )}
              >
                Li e concordo com os{" "}
                <span className="font-semibold">Termos de Uso</span> e estou
                ciente de todas as condições estabelecidas acima
              </label>
            </div>

            {/* Botões de ação */}
            <div className="flex gap-3 w-full sm:justify-end">
              <Button
                variant="outline"
                onClick={handleDecline}
                className="flex-1 sm:flex-none"
              >
                Recusar
              </Button>
              <Button
                onClick={handleAccept}
                disabled={!hasAgreed || !hasScrolledToBottom}
                className="flex-1 sm:flex-none bg-emerald-600 hover:bg-emerald-700"
              >
                Aceitar e Continuar
              </Button>
            </div>
          </DialogFooter>
        )}
      </DialogContent>
    </Dialog>
  );
}
