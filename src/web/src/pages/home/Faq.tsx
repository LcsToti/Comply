import { useState } from "react";
import { MessageSquare, Send, Loader2, CheckCircle } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import Header from "@/components/Header";
import Footer from "@/components/Footer";
import { toast } from "sonner";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import { useAuth } from "@/contexts/AuthContext";
import { useCreateTicketMutation } from "@/hooks/notifications/useTicketsMutations";

interface FAQCategory {
  category: string;
  iconColor: string;
  questions: {
    question: string;
    answer: string;
  }[];
}

const faqCategories: FAQCategory[] = [
  {
    category: "Conta e Cadastro",
    iconColor: "text-blue-600",
    questions: [
      {
        question: "Como criar uma conta?",
        answer:
          "Para criar uma conta, clique em 'Cadastrar' no topo da página e preencha o formulário com seus dados pessoais. Você receberá um email de confirmação para ativar sua conta.",
      },
      {
        question: "Esqueci minha senha, o que fazer?",
        answer:
          "Clique em 'Esqueci minha senha' na página de login. Digite seu email e você receberá um link para redefinir sua senha.",
      },
      {
        question: "Como alterar meus dados cadastrais?",
        answer:
          "Acesse seu perfil clicando no seu nome no canto superior direito e selecione 'Configurações'. Lá você pode editar seus dados pessoais.",
      },
    ],
  },
  {
    category: "Compras e Pagamentos",
    iconColor: "text-emerald-600",
    questions: [
      {
        question: "Quais formas de pagamento são aceitas?",
        answer:
          "Aceitamos cartões de crédito (Visa, Mastercard, Amex), PIX e PayPal. Todos os pagamentos são processados de forma segura através do Stripe.",
      },
      {
        question: "Como funciona o processo de compra?",
        answer:
          "Selecione o produto, clique em 'Comprar Agora', escolha sua forma de pagamento e confirme. Após o pagamento, você receberá um email de confirmação com o código de rastreamento.",
      },
      {
        question: "Posso cancelar uma compra?",
        answer:
          "Você pode cancelar uma compra enquanto ela estiver com status 'Aguardando Entrega'. Após o envio pelo vendedor, o cancelamento não é mais possível.",
      },
      {
        question: "Como adicionar um novo método de pagamento?",
        answer:
          "Vá em 'Meu Perfil' > 'Pagamentos' e clique em 'Adicionar nova forma de pagamento'. Preencha os dados do cartão ou configure o PIX.",
      },
    ],
  },
  {
    category: "Leilões",
    iconColor: "text-purple-600",
    questions: [
      {
        question: "Como funcionam os leilões?",
        answer:
          "Os leilões têm data de início e fim definidas. Durante o período, você pode dar lances. O maior lance ao fim do período vence. Alguns leilões têm 'Compra Imediata' que permite ganhar instantaneamente.",
      },
      {
        question: "O que é 'Compra Imediata'?",
        answer:
          "É um valor fixo definido pelo vendedor que permite você ganhar o leilão imediatamente, sem esperar o fim do período de lances.",
      },
      {
        question: "Fui superado no lance, e agora?",
        answer:
          "Você pode dar um novo lance maior até o fim do leilão. Receberá notificações quando for superado.",
      },
      {
        question: "Como sei se ganhei um leilão?",
        answer:
          "Você receberá uma notificação e email quando o leilão terminar. Verifique em 'Meus Leilões Vencidos'.",
      },
    ],
  },
  {
    category: "Vendas",
    iconColor: "text-orange-600",
    questions: [
      {
        question: "Como vender um produto?",
        answer:
          "Clique em 'Vender' no menu, preencha as informações do produto, adicione fotos e defina o preço. Você pode escolher venda direta ou leilão.",
      },
      {
        question: "Qual a taxa para vender?",
        answer:
          "Cobramos uma taxa de 5% sobre o valor final da venda. Esta taxa é descontada automaticamente no momento do pagamento.",
      },
      {
        question: "Como funciona a entrega?",
        answer:
          "Após a venda, você recebe um código de entrega. Marque o produto como 'Enviado' e compartilhe o código com o comprador. Quando ele confirmar o recebimento com o código, o pagamento é liberado.",
      },
    ],
  },
  {
    category: "Entregas e Rastreamento",
    iconColor: "text-amber-600",
    questions: [
      {
        question: "Como rastrear meu pedido?",
        answer:
          "Acesse 'Meus Pedidos' e clique no pedido desejado. Lá você verá o status atual e o código de rastreamento se disponível.",
      },
      {
        question: "Quanto tempo demora a entrega?",
        answer:
          "O prazo depende do acordo entre comprador e vendedor. Geralmente varia de 3 a 15 dias úteis.",
      },
      {
        question: "O que é o código de entrega?",
        answer:
          "É um código único gerado para cada venda. O vendedor compartilha com você ao enviar o produto. Você usa este código para confirmar o recebimento.",
      },
    ],
  },
  {
    category: "Disputas e Problemas",
    iconColor: "text-red-600",
    questions: [
      {
        question: "O que fazer se não recebi o produto?",
        answer:
          "Primeiro, tente contatar o vendedor. Se não houver resposta ou resolução, você pode abrir uma disputa em até 30 dias após a compra.",
      },
      {
        question: "Como abrir uma disputa?",
        answer:
          "Vá até o pedido em 'Meus Pedidos', clique em 'Abrir Disputa' e descreva o problema. Um administrador analisará o caso.",
      },
      {
        question: "Quanto tempo leva para resolver uma disputa?",
        answer:
          "Disputas são geralmente resolvidas em até 7 dias úteis. Você receberá atualizações por email.",
      },
      {
        question: "Posso ter um reembolso?",
        answer:
          "Reembolsos são analisados caso a caso. Se a disputa for favorável a você, o reembolso é processado em até 5 dias úteis.",
      },
    ],
  },
  {
    category: "Segurança",
    iconColor: "text-indigo-600",
    questions: [
      {
        question: "Meus dados de pagamento estão seguros?",
        answer:
          "Sim! Todos os pagamentos são processados pelo Stripe, uma das plataformas mais seguras do mundo. Não armazenamos dados de cartão em nossos servidores.",
      },
      {
        question: "Como denunciar uma fraude?",
        answer:
          "Abra um ticket de suporte imediatamente detalhando a situação. Nossa equipe investigará e tomará as medidas necessárias.",
      },
      {
        question: "Posso confiar nos vendedores?",
        answer:
          "Todos os vendedores são verificados. Você pode ver avaliações de outros compradores antes de comprar.",
      },
    ],
  },
];

export default function Faq() {
  const [openTicketDialog, setOpenTicketDialog] = useState(false);
  const [ticketTitle, setTicketTitle] = useState("");
  const [ticketDescription, setTicketDescription] = useState("");
  const [searchQuery, setSearchQuery] = useState("");
  const { isLoggedIn } = useAuth();
  const { mutate: createTicket, isPending } = useCreateTicketMutation();

  const handleSubmitTicket = () => {
    if (!ticketTitle.trim() || !ticketDescription.trim()) {
      toast.error("Preencha todos os campos");
      return;
    }

    createTicket(
      {
        Title: ticketTitle.trim(),
        Description: ticketDescription.trim(),
      },
      {
        onSuccess: () => {
          setOpenTicketDialog(false);
          setTicketTitle("");
          setTicketDescription("");
        },
      }
    );
  };

  const filteredCategories = faqCategories
    .map((category) => ({
      ...category,
      questions: category.questions.filter(
        (q) =>
          q.question.toLowerCase().includes(searchQuery.toLowerCase()) ||
          q.answer.toLowerCase().includes(searchQuery.toLowerCase())
      ),
    }))
    .filter((category) => category.questions.length > 0);

  return (
    <div className="min-h-screen flex flex-col">
      <Header />

      <main className="flex-1 container mx-auto px-4 py-12 max-w-5xl">
        {/* Header */}
        <div className="text-center mb-6">
          <h1 className="text-4xl md:text-5xl font-bold text-gray-900 mb-4">
            Perguntas Frequentes
          </h1>
          <p className="text-lg text-gray-600 max-w-2xl mx-auto">
            Encontre respostas rápidas para as dúvidas mais comuns sobre nossa
            plataforma
          </p>
        </div>

        {/* Search Bar */}
        <div className="mb-8 flex">
          <Input
            type="text"
            placeholder="Buscar pergunta..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="w-full max-w-2xl mx-auto text-lg h-8"
          />
        </div>

        {/* FAQ Categories */}
        <div className="flex flex-col gap-2 mb-12">
          {filteredCategories.length > 0 ? (
            filteredCategories.map((category, idx) => {
              return (
                <Accordion
                  type="single"
                  collapsible
                  className="w-full border-gray-100 px-5 rounded-2xl border"
                >
                  <AccordionItem key={idx} value={`cat-${idx}`}>
                    <AccordionTrigger className="flex items-center gap-3 hover:text-emerald-700 text-xl hover:no-underline font-semibold cursor-pointer">
                      {category.category}
                    </AccordionTrigger>
                    <AccordionContent>
                      <Accordion type="single" collapsible className="w-full">
                        {category.questions.map((item, qIdx) => (
                          <AccordionItem
                            key={qIdx}
                            value={`item-${idx}-${qIdx}`}
                          >
                            <AccordionTrigger className="text-left hover:text-emerald-700 transition-colors cursor-pointer hover:no-underline">
                              {item.question}
                            </AccordionTrigger>
                            <AccordionContent className="text-gray-600 leading-relaxed">
                              {item.answer}
                            </AccordionContent>
                          </AccordionItem>
                        ))}
                      </Accordion>
                    </AccordionContent>
                  </AccordionItem>
                </Accordion>
              );
            })
          ) : (
            <div className="text-center py-12">
              <p className="text-gray-500 text-lg">
                Nenhuma pergunta encontrada para "{searchQuery}"
              </p>
            </div>
          )}
        </div>

        {/* Support Ticket Section */}
        {isLoggedIn ? (
          <Card className="shadow-none">
            <CardHeader className="text-center">
              <CardTitle className="text-lg flex items-center justify-center gap-2">
                <MessageSquare className="h-6 w-6 text-emerald-700 text-lg" />
                Não encontrou sua resposta?
              </CardTitle>
              <CardDescription className="text-xs">
                Nossa equipe de suporte está pronta para ajudar você!
              </CardDescription>
            </CardHeader>
            <CardContent className="flex justify-center">
              <Dialog
                open={openTicketDialog}
                onOpenChange={setOpenTicketDialog}
              >
                <DialogTrigger asChild>
                  <Button
                    size="sm"
                    className="bg-emerald-700 hover:bg-emerald-800 text-sm cursor-pointer px-8"
                  >
                    <Send className="mr-2 h-5 w-5" />
                    Enviar Ticket de Suporte
                  </Button>
                </DialogTrigger>
                <DialogContent className="sm:max-w-[550px]">
                  <DialogHeader>
                    <DialogTitle>Criar Ticket de Suporte</DialogTitle>
                    <DialogDescription>
                      Descreva seu problema e nossa equipe entrará em contato em
                      breve.
                    </DialogDescription>
                  </DialogHeader>

                  <div className="space-y-4 py-4">
                    <div className="space-y-2">
                      <Label htmlFor="ticket-title">Assunto *</Label>
                      <Input
                        id="ticket-title"
                        placeholder="Ex: Problema com pagamento"
                        value={ticketTitle}
                        onChange={(e) => setTicketTitle(e.target.value)}
                        maxLength={100}
                        minLength={10}
                      />
                      <p className="text-xs text-gray-500 text-right">
                        {ticketTitle.length}/100 caracteres
                      </p>
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="ticket-description">Descrição *</Label>
                      <Textarea
                        id="ticket-description"
                        placeholder="Descreva seu problema em detalhes..."
                        value={ticketDescription}
                        onChange={(e) => setTicketDescription(e.target.value)}
                        rows={6}
                        maxLength={1000}
                        minLength={10}
                      />
                      <p className="text-xs text-gray-500 text-right">
                        {ticketDescription.length}/1000 caracteres
                      </p>
                    </div>

                    <div className="bg-blue-50 border border-blue-200 rounded-lg p-3">
                      <p className="text-sm text-blue-800">
                        <strong>💡 Dica:</strong> Seja o mais específico
                        possível para que possamos ajudá-lo melhor. Inclua
                        números de pedido, prints ou outros detalhes relevantes.
                      </p>
                    </div>
                  </div>

                  <div className="flex justify-end gap-2">
                    <Button
                      variant="outline"
                      onClick={() => setOpenTicketDialog(false)}
                    >
                      Cancelar
                    </Button>
                    <Button
                      onClick={handleSubmitTicket}
                      disabled={
                        isPending ||
                        !ticketTitle.trim() ||
                        !ticketDescription.trim()
                      }
                      className="bg-emerald-700 hover:bg-emerald-800"
                    >
                      {isPending ? (
                        <>
                          <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                          Enviando...
                        </>
                      ) : (
                        <>
                          <CheckCircle className="mr-2 h-4 w-4" />
                          Enviar Ticket
                        </>
                      )}
                    </Button>
                  </div>
                </DialogContent>
              </Dialog>
            </CardContent>
          </Card>
        ) : (
          ""
        )}

        {/* Additional Help */}
        <div className="mt-12 text-center">
          <p className="text-gray-600 mb-4">Ou entre em contato diretamente:</p>
          <div className="flex flex-wrap justify-center gap-6 text-sm">
            <a href="mailto:suporte@comply.com" className="text-gray-500">
              suporte@comply.com
            </a>
            <a href="tel:+551140028922" className="text-gray-500">
              (11) 4002-8922
            </a>
            <span className="text-gray-500">Seg-Sex: 9h às 18h</span>
          </div>
        </div>
      </main>

      <Footer />
    </div>
  );
}
