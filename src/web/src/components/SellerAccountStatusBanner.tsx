import {
  Card,
  CardContent,
  CardDescription,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import {
  AlertCircle,
  Clock,
  CheckCircle2,
  XCircle,
  Store,
  ExternalLink,
  SquareArrowRight,
} from "lucide-react";
import { PaymentAccountStatus } from "@/types/paymentAccount";
import { cn } from "@/lib/utils";

interface SellerAccountStatusBannerProps {
  status: PaymentAccountStatus;
  onboardingLink?: string;
  className?: string;
  showButton?: boolean;
}

export function SellerAccountStatusBanner({
  status,
  onboardingLink,
  className,
  showButton,
}: SellerAccountStatusBannerProps) {
  const statusConfig = {
    [PaymentAccountStatus.None]: {
      icon: Store,
      title: "Configure sua conta de vendedor",
      description:
        "Para começar a vender, você precisa criar e configurar sua conta de vendedor com informações de pagamento.",
      buttonText: "Criar conta de vendedor",
      buttonIcon: Store,
      colors: {
        border: "border-red-400",
        bg: "bg-red-50/70 dark:bg-red-400/20",
        title: "text-red-700 dark:text-red-300",
        description: "text-red-600 dark:text-red-200",
        icon: "text-red-600",
      },
      buttonVariant: "default" as const,
    },
    [PaymentAccountStatus.Incomplete]: {
      icon: SquareArrowRight,
      title: "Ative sua conta de vendedor",
      description:
        "Para começar a vender, finalize a configuração da sua conta de vendedor no Stripe.",
      buttonText: "Iniciar configuração",
      buttonIcon: SquareArrowRight,
      colors: {
        border: "border-orange-400",
        bg: "bg-orange-50/70 dark:bg-orange-400/20",
        title: "text-orange-700 dark:text-orange-300",
        description: "text-orange-600 dark:text-orange-200",
        icon: "text-orange-600",
      },
      buttonVariant: "default" as const,
    },
    [PaymentAccountStatus.PendingReview]: {
      icon: Clock,
      title: "Conta em análise",
      description:
        "Suas informações estão sendo revisadas pelo Stripe. Este processo geralmente leva até 48 horas.",
      buttonText: "Ver status",
      buttonIcon: ExternalLink,
      colors: {
        border: "border-blue-400",
        bg: "bg-blue-50/70 dark:bg-blue-400/20",
        title: "text-blue-700 dark:text-blue-300",
        description: "text-blue-600 dark:text-blue-200",
        icon: "text-blue-600",
      },
      buttonVariant: "default" as const,
    },
    [PaymentAccountStatus.Issues]: {
      icon: XCircle,
      title: "Ação necessária na sua conta",
      description:
        "Foram identificados problemas que precisam ser resolvidos para ativar sua conta de vendedor. Clique para ver os detalhes.",
      buttonText: "Resolver problemas",
      buttonIcon: AlertCircle,
      colors: {
        border: "border-red-500",
        bg: "bg-red-50/70 dark:bg-red-500/20",
        title: "text-red-700 dark:text-red-300",
        description: "text-red-600 dark:text-red-200",
        icon: "text-red-600",
      },
      buttonVariant: "destructive" as const,
    },
    [PaymentAccountStatus.Active]: {
      icon: CheckCircle2,
      title: "Conta de vendedor ativa",
      description:
        "Sua conta está configurada e você já pode criar anúncios. Acesse o dashboard do Stripe para mais detalhes.",
      buttonText: "Ver dashboard",
      buttonIcon: ExternalLink,
      colors: {
        border: "border-emerald-400",
        bg: "bg-emerald-50/70 dark:bg-emerald-400/20",
        title: "text-emerald-700 dark:text-emerald-300",
        description: "text-emerald-600 dark:text-emerald-200",
        icon: "text-emerald-600",
      },
      buttonVariant: "default" as const,
    },
  };

  const config = statusConfig[status];
  const Icon = config.icon;
  const ButtonIcon = config.buttonIcon;

  if (status === PaymentAccountStatus.Active) {
    return null;
  }

  return (
    <Card
      className={cn(
        "shadow-none border border-dashed p-3",
        config.colors.border,
        config.colors.bg,
        className
      )}
    >
      <CardContent className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 p-0">
        <div className="flex items-start gap-3 flex-1">
          <div className="flex-shrink-0 mt-0.5">
            <Icon className={cn("w-5 h-5", config.colors.icon)} />
          </div>
          <div className="space-y-1 flex-1 min-w-0">
            <CardTitle
              className={cn("text-md font-semibold", config.colors.title)}
            >
              {config.title}
            </CardTitle>
            <CardDescription
              className={cn("text-xs max-w-2xl", config.colors.description)}
            >
              {config.description}
            </CardDescription>
          </div>
        </div>
        {showButton !== false ? (
          <div className="flex-shrink-0 w-full sm:w-auto">
            {onboardingLink ? (
              <Button
                onClick={() => (window.location.href = onboardingLink)}
                size="lg"
                variant={config.buttonVariant}
                className={cn(
                  "cursor-pointer font-medium px-6 w-full sm:w-auto",
                  config.buttonVariant === "default" &&
                    "bg-emerald-700 hover:bg-emerald-800 text-white"
                )}
              >
                <ButtonIcon className="w-4 h-4 mr-2" />
                {config.buttonText}
              </Button>
            ) : (
              <Button
                size="lg"
                disabled
                className="cursor-not-allowed font-medium px-6 w-full sm:w-auto opacity-50"
              >
                <ButtonIcon className="w-4 h-4 mr-2" />
                {config.buttonText}
              </Button>
            )}
          </div>
        ) : (
          ""
        )}
      </CardContent>
    </Card>
  );
}
