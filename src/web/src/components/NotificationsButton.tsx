import {
  Popover,
  PopoverTrigger,
  PopoverContent,
} from "@/components/ui/popover";
import { Button } from "@/components/ui/button";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Badge } from "@/components/ui/badge";
import { Bell } from "lucide-react";
import { formatDistanceToNow } from "date-fns";
import { ptBR } from "date-fns/locale";
import {
  NotificationTypesLabel,
  type Notification,
} from "@/types/notification";
import { useNotificationsQuery } from "@/hooks/notifications/useNotificationsQueries";
import { useMarkNotificationAsReadMutation } from "@/hooks/notifications/useNotificationsMutations";
import { useAuth } from "@/contexts/AuthContext";
export function NotificationsButton() {
  const { isLoggedIn } = useAuth();
  const { data, isLoading, isError } = useNotificationsQuery(
    undefined,
    isLoggedIn
  );

  const markAsReadMutation = useMarkNotificationAsReadMutation();

  const notifications = (data ?? []) as Notification[];

  const unreadNotifications = notifications.filter((n) => !n.read);
  const unreadCount = unreadNotifications.length;

  const handleMarkAsRead = (id: string) => {
    markAsReadMutation.mutate(id);
  };

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          variant="ghost"
          size="icon"
          className="relative cursor-pointer hover:bg-gray-200"
          aria-label="Notificações"
        >
          <Bell className="h-5 w-5 text-gray-600" />
          {unreadCount > 0 && (
            <span className="absolute -top-1 -right-1 inline-flex h-4 min-w-4 items-center justify-center rounded-full bg-red-500 px-1 text-[10px] font-semibold text-white">
              {unreadCount}
            </span>
          )}
        </Button>
      </PopoverTrigger>

      <PopoverContent className="w-80 p-0">
        <div className="flex items-center justify-between border-b px-4 py-2">
          <span className="text-sm font-semibold">Notificações</span>
          {unreadCount > 0 && (
            <Badge variant="outline" className="text-xs">
              {unreadCount} novas
            </Badge>
          )}
        </div>

        <ScrollArea className="max-h-80 overflow-y-auto">
          {isLoading && (
            <div className="px-4 py-3 text-sm text-gray-500">
              Carregando notificações...
            </div>
          )}

          {isError && (
            <div className="px-4 py-3 text-sm text-red-500">
              Não foi possível carregar as notificações.
            </div>
          )}

          {!isLoading && !isError && unreadNotifications.length === 0 && (
            <div className="px-4 py-3 text-sm text-gray-500">
              Você não tem notificações novas.
            </div>
          )}

          {!isLoading &&
            !isError &&
            unreadNotifications.map((n) => (
              <button
                key={n.id}
                className="flex w-full flex-col items-start gap-1 border-b px-4 py-3 text-left last:border-b-0 bg-gray-50 hover:bg-emerald-100 border-l-2 border-l-emerald-500 transition-all cursor-pointer"
                onClick={() => handleMarkAsRead(n.id)}
              >
                <div className="flex w-full items-center justify-between gap-2">
                  <span className="flex items-center gap-1 text-xs font-semibold uppercase text-emerald-800">
                    <span className="h-2 w-2 rounded-full bg-emerald-500" />
                    {NotificationTypesLabel[n.type]}
                  </span>
                  <span className="text-[11px] text-gray-400">
                    {formatDistanceToNow(new Date(n.createdAt), {
                      addSuffix: true,
                      locale: ptBR,
                    })}
                  </span>
                </div>
                <p className="text-sm text-gray-900">{n.message}</p>
                <span className="mt-1 text-[11px] font-medium text-emerald-700">
                  Marcar como lida
                </span>
              </button>
            ))}
        </ScrollArea>
      </PopoverContent>
    </Popover>
  );
}
