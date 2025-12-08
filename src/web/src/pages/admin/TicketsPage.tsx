import { useState } from "react";
import {
  Search,
  Filter,
  Clock,
  Send,
  CheckCircle2,
  XCircle,
  AlertCircle,
  MessageSquare,
  Calendar,
  ArrowLeft,
  Loader2,
  Lock,
  Info,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { ScrollArea } from "@/components/ui/scroll-area";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { cn } from "@/lib/utils";
import { TicketStatus, type Ticket } from "@/types/ticket";
import { useAllTicketsQuery } from "@/hooks/notifications/useTicketsQueries";
import { useUserQuery } from "@/hooks/user/useUsersQueries";
import {
  useAddCommentMutation,
  useUpdateTicketStatusMutation,
} from "@/hooks/notifications/useTicketsMutations";

export default function TicketsPage() {
  const [selectedTicket, setSelectedTicket] = useState<Ticket | null>(null);
  const [statusFilter, setStatusFilter] = useState<TicketStatus | null>(null);
  const [searchQuery, setSearchQuery] = useState("");
  const [newComment, setNewComment] = useState("");
  const [showDetailOnMobile, setShowDetailOnMobile] = useState(false);
  const [confirmDialog, setConfirmDialog] = useState<{
    open: boolean;
    status: TicketStatus | null;
    title: string;
    description: string;
  }>({
    open: false,
    status: null,
    title: "",
    description: "",
  });

  const { data: tickets = [], isLoading, error } = useAllTicketsQuery(true);

  const { data: userData } = useUserQuery(
    selectedTicket ? selectedTicket?.userId : "",
    !!selectedTicket
  );
  const { data: adminData } = useUserQuery(
    selectedTicket?.assignedAdminId ? selectedTicket.assignedAdminId : "",
    !!selectedTicket?.assignedAdminId
  );

  const addCommentMutation = useAddCommentMutation();
  const updateStatusMutation = useUpdateTicketStatusMutation();

  const statusConfig = {
    "Open": { color: "bg-blue-100 text-blue-800", icon: AlertCircle },
    "InProgress": { color: "bg-yellow-100 text-yellow-800", icon: Clock },
    "Resolved": { color: "bg-green-100 text-green-800", icon: CheckCircle2 },
    "Closed": { color: "bg-gray-100 text-gray-800", icon: XCircle },
  };

  const filteredTickets = tickets.filter((ticket: Ticket) => {
    const matchesStatus =
      statusFilter === null || ticket.status === statusFilter;
    const matchesSearch =
      ticket.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      ticket.id.toLowerCase().includes(searchQuery.toLowerCase());
    return matchesStatus && matchesSearch;
  });

  const handleTicketClick = (ticket: Ticket) => {
    setSelectedTicket(ticket);
    setShowDetailOnMobile(true);
  };

  const handleBackToList = () => {
    setShowDetailOnMobile(false);
    setSelectedTicket(null);
  };

  // Verificar se pode comentar
  const canComment = () => {
    if (!selectedTicket) return false;

    // Não pode comentar se o ticket estiver fechado
    if (selectedTicket.status === TicketStatus.Closed) return false;

    // Precisa ter um admin atribuído (status InProgress ou superior)
    if (
      !selectedTicket.assignedAdminId &&
      selectedTicket.status === TicketStatus.Open
    ) {
      return false;
    }

    return true;
  };

  const handleSendComment = async () => {
    if (!newComment.trim() || !selectedTicket) return;

    if (!canComment()) return;

    await addCommentMutation.mutateAsync({
      ticketId: selectedTicket.id,
      content: newComment.trim(),
    });

    setNewComment("");
  };

  const handleStatusChange = async (newStatus: TicketStatus) => {
    if (!selectedTicket) return;

    const status = newStatus as TicketStatus;

    // Confirmação para Resolved e Closed
    if (status === TicketStatus.Resolved || status === TicketStatus.Closed) {
    
    const config = {
        // Chave 'Resolved'
        [TicketStatus.Resolved]: { 
            title: "Marcar ticket como resolvido?",
            description:
              "O ticket será marcado como resolvido. Você ainda poderá adicionar comentários e alterar o status se necessário.",
        },
        // Chave 'Closed'
        [TicketStatus.Closed]: { 
            title: "Encerrar ticket definitivamente?",
            description:
              "⚠️ Esta ação é final. Após encerrar, não será possível adicionar comentários ou alterar o status. Certifique-se de que o problema foi completamente resolvido.",
        },
    } as const;

    setConfirmDialog({
        open: true,
        status,
        // Agora 'status' (string) é uma chave válida no objeto 'config'.
        title: config[status].title,
        description: config[status].description,
    });
    return;
}

    // Para Open e InProgress, mudar diretamente
    await executeStatusChange(status);
  };

  const executeStatusChange = async (newStatus: TicketStatus) => {
    if (!selectedTicket) return;

    await updateStatusMutation.mutateAsync({
      ticketId: selectedTicket.id,
      newStatus,
    });

    setSelectedTicket((prev) => (prev ? { ...prev, status: newStatus } : null));
  };

  const confirmStatusChange = async () => {
    if (confirmDialog.status) {
      await executeStatusChange(confirmDialog.status);
    }
    setConfirmDialog({ open: false, status: null, title: "", description: "" });
  };

  // Verificar se pode mudar status
  const canChangeStatus = () => {
    if (!selectedTicket) return false;
    return selectedTicket.status !== TicketStatus.Closed;
  };

  // Mensagem de ajuda para comentários
  const getCommentHelperMessage = () => {
    if (!selectedTicket) return null;

    if (selectedTicket.status === TicketStatus.Closed) {
      return {
        type: "locked",
        message: "Este ticket está encerrado e não aceita mais comentários.",
      };
    }

    if (
      !selectedTicket.assignedAdminId &&
      selectedTicket.status === TicketStatus.Open
    ) {
      return {
        type: "assign",
        message:
          "Para responder, altere o status para 'Em Progresso' e assuma este ticket.",
      };
    }

    return null;
  };

  // Loading State
  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-screen">
        <Loader2 className="w-8 h-8 animate-spin text-emerald-600" />
      </div>
    );
  }

  // Error State
  if (error) {
    return (
      <div className="flex items-center justify-center h-screen">
        <div className="text-center text-red-600">
          <XCircle className="w-12 h-12 mx-auto mb-4" />
          <p>Erro ao carregar tickets</p>
          <p className="text-sm text-gray-500 mt-2">
            {error instanceof Error ? error.message : "Erro desconhecido"}
          </p>
        </div>
      </div>
    );
  }

  const helperMessage = getCommentHelperMessage();

  return (
    <>
      <div className="flex flex-1 h-screen overflow-auto">
        {/* Back Button Mobile */}
        {showDetailOnMobile && (
          <Button
            variant="outline"
            size="icon"
            className="lg:hidden fixed top-20 left-4 z-50 shadow-lg bg-white"
            onClick={handleBackToList}
          >
            <ArrowLeft className="w-5 h-5" />
          </Button>
        )}

        {/* Tickets List Sidebar */}
        <div
          className={cn(
            "w-full lg:w-96 border-r border-gray-200 bg-white flex flex-col flex-shrink-0",
            showDetailOnMobile && "hidden lg:flex"
          )}
        >
          {/* Filters */}
          <div className="flex flex-row gap-3 p-3 lg:p-4 border-b border-gray-200 flex-shrink-0">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
              <Input
                type="text"
                placeholder="Buscar tickets..."
                className="pl-10"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
              />
            </div>

            <Select
              value={statusFilter?.toString()}
              onValueChange={(v) =>
                setStatusFilter(
                   v === "all" ? null : (v as TicketStatus)
                )
              }
            >
              <SelectTrigger>
                <Filter className="w-4 h-4 mr-2 flex-shrink-0" />
                <SelectValue placeholder="Filtrar por status" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Todos</SelectItem>
                <SelectItem value="Open">Abertos</SelectItem>
                <SelectItem value="InProgress">Em progresso</SelectItem>
                <SelectItem value="Resolved">Resolvidos</SelectItem>
                <SelectItem value="Closed">Fechados</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Tickets List */}
          <ScrollArea className="flex-1 overflow-y-auto">
            <div className="p-2 lg:p-3">
              {filteredTickets?.length === 0 ? (
                <div className="text-center py-8 text-gray-500">
                  <MessageSquare className="w-10 h-10 lg:w-12 lg:h-12 mx-auto mb-2 opacity-50" />
                  <p className="text-sm lg:text-base">
                    Nenhum ticket encontrado
                  </p>
                </div>
              ) : (
                filteredTickets.map((ticket: Ticket) => {
                  const statusKey = ticket.status as keyof typeof statusConfig;
                  const StatusIcon = statusConfig[statusKey].icon;
                  const isSelected = selectedTicket?.id === ticket.id;

                  return (
                    <Card
                      key={ticket.id}
                      className={cn(
                        "mb-2 cursor-pointer transition-all shadow-none hover:border-emerald-400",
                        isSelected && "border-emerald-600"
                      )}
                      onClick={() => handleTicketClick(ticket)}
                    >
                      <CardContent className="px-3 py-3">
                        <div className="flex items-start justify-between mb-2">
                          <div className="flex-1 min-w-0">
                            <div className="flex items-center gap-2 mb-1 flex-wrap">
                              <span className="text-xs font-mono text-gray-500">
                                #{ticket.id.slice(0, 8)}
                              </span>
                              <Badge
                                className={cn(
                                  statusConfig[statusKey].color,
                                  "text-xs"
                                )}
                              >
                                <StatusIcon className="w-3 h-3 mr-1" />
                                {statusKey}
                              </Badge>
                              {ticket.status === TicketStatus.Closed && (
                                <Lock className="w-3 h-3 text-gray-400" />
                              )}
                            </div>
                            <h3 className="font-semibold text-sm text-gray-900 line-clamp-1 break-words">
                              {ticket.title}
                            </h3>
                          </div>
                        </div>

                        <div className="flex items-center gap-2 text-xs text-gray-500 flex-wrap">
                          <div className="flex items-center gap-1">
                            <Calendar className="w-3 h-3 flex-shrink-0" />
                            <span className="whitespace-nowrap">
                              {new Date(ticket.createdAt).toLocaleDateString(
                                "pt-BR"
                              )}
                            </span>
                          </div>
                        </div>

                        <div className="mt-2 text-xs text-gray-500 flex items-center gap-1">
                          <MessageSquare className="w-3 h-3 flex-shrink-0" />
                          <span>
                            {ticket.comments?.length > 0
                              ? `${ticket.comments?.length} resposta(s)`
                              : "Sem respostas"}
                          </span>
                        </div>
                      </CardContent>
                    </Card>
                  );
                })
              )}
            </div>
          </ScrollArea>
        </div>

        {/* Ticket Details */}
        <div
          className={cn(
            "flex-1 flex flex-col min-w-0 overflow-auto",
            !showDetailOnMobile && "hidden lg:flex"
          )}
        >
          {selectedTicket ? (
            <>
              {/* Header */}
              <div className="bg-white border-b border-gray-200 p-4 lg:p-6 flex-shrink-0">
                <div className="flex flex-col lg:flex-row lg:items-start lg:justify-between gap-4 mb-4">
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2 lg:gap-3 mb-2 flex-wrap">
                      <span className="text-xs lg:text-sm font-mono text-gray-500">
                        #{selectedTicket.id.slice(0, 8)}
                      </span>
                      <Badge
                        className={cn(
                          statusConfig[
                            selectedTicket.status as keyof typeof statusConfig
                          ].color,
                          "text-xs"
                        )}
                      >
                        {selectedTicket.status}
                      </Badge>
                      {selectedTicket.status === TicketStatus.Closed && (
                        <Badge
                          variant="outline"
                          className="text-xs flex items-center gap-1"
                        >
                          <Lock className="w-3 h-3" />
                          Encerrado
                        </Badge>
                      )}
                    </div>
                    <h2 className="text-lg lg:text-xl font-semibold text-gray-900 mb-2 break-words">
                      {selectedTicket.title}
                    </h2>
                    <div className="flex flex-col sm:flex-row sm:items-center gap-2 sm:gap-4 text-sm text-gray-600">
                      <div className="flex items-center gap-2">
                        <Avatar className="w-6 h-6">
                          <AvatarFallback className="text-xs bg-blue-100 text-blue-700">
                            {userData?.name
                              ?.split(" ")
                              .map((n) => n[0])
                              .join("")}
                          </AvatarFallback>
                        </Avatar>
                        <span className="truncate">{userData?.name}</span>
                      </div>
                      <span className="hidden sm:inline">•</span>
                      <div className="flex items-center gap-1">
                        <Calendar className="w-4 h-4 flex-shrink-0" />
                        <span className="text-xs lg:text-sm">
                          {new Date(selectedTicket.createdAt).toLocaleString(
                            "pt-BR"
                          )}
                        </span>
                      </div>
                    </div>
                  </div>

                  <Select
                    value={selectedTicket.status.toString()}
                    onValueChange={(value) =>
                      handleStatusChange(value as TicketStatus)
                    }
                    disabled={
                      updateStatusMutation.isPending || !canChangeStatus()
                    }
                  >
                    <SelectTrigger className="w-full sm:w-[180px] h-9 lg:h-10">
                      {updateStatusMutation.isPending ? (
                        <Loader2 className="w-4 h-4 animate-spin" />
                      ) : canChangeStatus() ? (
                        <SelectValue />
                      ) : (
                        <div className="flex items-center gap-2">
                          <Lock className="w-4 h-4" />
                          <SelectValue />
                        </div>
                      )}
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="Open">Aberto</SelectItem>
                      <SelectItem value="InProgress">Em Progresso</SelectItem>
                      <SelectItem value="Resolved">Resolvido</SelectItem>
                      <SelectItem value="Closed">Encerrado</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <Card className="bg-gray-50 shadow-none p-0 border-gray-100">
                  <CardContent className="py-3 lg:py-4">
                    <p className="text-xs lg:text-sm text-gray-700 whitespace-pre-wrap break-words">
                      {selectedTicket.description}
                    </p>
                  </CardContent>
                </Card>
              </div>

              {/* Comments Section */}
              <ScrollArea className="flex-1 p-3 lg:p-5 overflow-y-auto">
                <div className="max-w-4xl mx-auto">
                  <h3 className="text-sm font-semibold text-gray-900 mb-3 lg:mb-4 flex items-center gap-2">
                    <MessageSquare className="w-4 h-4 lg:w-5 lg:h-5" />
                    Respostas ({selectedTicket.comments?.length})
                  </h3>

                  <div className="flex flex-col gap-3">
                    {selectedTicket.comments?.map((comment) => {
                      const isFromAdmin =
                        comment.authorId === selectedTicket.assignedAdminId;
                      console.log(
                        comment.content + "Veio do admin? " + isFromAdmin
                      );

                      const authorName = isFromAdmin
                        ? adminData?.name
                        : userData?.name;

                      return (
                        <Card
                          key={comment.createdAt}
                          className={cn(
                            "shadow-none p-0",
                            isFromAdmin && "border-emerald-200 bg-emerald-50"
                          )}
                        >
                          <CardContent className="p-3 lg:p-4">
                            <div className="flex items-start gap-2 lg:gap-3">
                              <Avatar className="w-8 h-8 lg:w-10 lg:h-10 flex-shrink-0">
                                <AvatarFallback
                                  className={
                                    isFromAdmin
                                      ? "bg-emerald-600 text-white text-xs"
                                      : "bg-gray-200 text-xs"
                                  }
                                >
                                  {authorName
                                    ?.split(" ")
                                    .map((n) => n[0])
                                    .join("")}
                                </AvatarFallback>
                              </Avatar>

                              <div className="flex-1 min-w-0">
                                <div className="flex flex-col sm:flex-row sm:items-center gap-1 sm:gap-2 mb-1">
                                  <span className="font-semibold text-sm lg:text-base text-gray-900 truncate">
                                    {authorName}
                                  </span>

                                  {isFromAdmin && (
                                    <Badge
                                      variant="secondary"
                                      className="text-xs w-fit"
                                    >
                                      Admin
                                    </Badge>
                                  )}

                                  <span className="text-xs text-gray-500">
                                    {new Date(comment.createdAt).toLocaleString(
                                      "pt-BR"
                                    )}
                                  </span>
                                </div>

                                <p className="text-sm text-gray-700 break-words">
                                  {comment.content}
                                </p>
                              </div>
                            </div>
                          </CardContent>
                        </Card>
                      );
                    })}
                  </div>
                </div>
              </ScrollArea>

              {/* Reply Section */}
              <div className="bg-white border-t border-gray-200 p-3 flex-shrink-0">
                <div className="max-w-4xl mx-auto">
                  {helperMessage && (
                    <Alert
                      className={cn(
                        "mb-3",
                        helperMessage.type === "locked"
                          ? "border-gray-300 bg-gray-50"
                          : "border-blue-300 bg-blue-50"
                      )}
                    >
                      {helperMessage.type === "locked" ? (
                        <Lock className="h-4 w-4" />
                      ) : (
                        <Info className="h-4 w-4" />
                      )}
                      <AlertDescription className="text-xs lg:text-sm">
                        {helperMessage.message}
                      </AlertDescription>
                    </Alert>
                  )}

                  <div className="flex gap-2 lg:gap-3">
                    <Avatar className="w-8 h-8 lg:w-10 lg:h-10 flex-shrink-0 hidden sm:flex">
                      <AvatarFallback className="bg-emerald-600 text-white text-xs">
                        AD
                      </AvatarFallback>
                    </Avatar>
                    <div className="flex-1 min-w-0">
                      <Textarea
                        placeholder={
                          canComment()
                            ? "Digite sua resposta..."
                            : "Não é possível comentar neste momento"
                        }
                        className="mb-3 text-sm lg:text-base resize-none"
                        value={newComment}
                        onChange={(e) => setNewComment(e.target.value)}
                        disabled={!canComment() || addCommentMutation.isPending}
                      />
                      <div className="flex justify-end gap-2">
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => setNewComment("")}
                          className="cursor-pointer text-xs lg:text-sm"
                          disabled={
                            !canComment() || addCommentMutation.isPending
                          }
                        >
                          Cancelar
                        </Button>
                        <Button
                          size="sm"
                          className="bg-emerald-600 hover:bg-emerald-700 cursor-pointer text-xs lg:text-sm"
                          onClick={handleSendComment}
                          disabled={
                            !newComment.trim() ||
                            !canComment() ||
                            addCommentMutation.isPending
                          }
                        >
                          {addCommentMutation.isPending ? (
                            <Loader2 className="w-3 h-3 lg:w-4 lg:h-4 mr-1 lg:mr-2 animate-spin" />
                          ) : (
                            <Send className="w-3 h-3 lg:w-4 lg:h-4 mr-1 lg:mr-2" />
                          )}
                          Enviar
                        </Button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </>
          ) : (
            <div className="flex-1 flex items-center justify-center text-gray-500 p-4">
              <div className="text-center">
                <MessageSquare className="w-12 h-12 lg:w-16 lg:h-16 mx-auto mb-4 opacity-50" />
                <p className="text-sm lg:text-lg">
                  Selecione um ticket para ver os detalhes
                </p>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* Confirmation Dialog */}
      <AlertDialog
        open={confirmDialog.open}
        onOpenChange={(open) =>
          !open &&
          setConfirmDialog({
            open: false,
            status: null,
            title: "",
            description: "",
          })
        }
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{confirmDialog.title}</AlertDialogTitle>
            <AlertDialogDescription className="text-sm leading-relaxed">
              {confirmDialog.description}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmStatusChange}
              className={cn(
                confirmDialog.status === TicketStatus.Closed
                  ? "bg-red-600 hover:bg-red-700"
                  : "bg-emerald-600 hover:bg-emerald-700"
              )}
            >
              {confirmDialog.status === TicketStatus.Closed
                ? "Sim, encerrar"
                : "Sim, marcar como resolvido"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
