import {
  ArrowLeft,
  BanknoteArrowUp,
  Lock,
  ShieldCheck,
  Store,
  Zap,
  AlertCircle,
  Clock,
  CheckCircle2,
  XCircle,
  RefreshCw,
} from "lucide-react";
import Header from "../../components/Header";
import { Link } from "react-router";
import { Button } from "../../components/ui/button";
import { useAuth } from "@/contexts/AuthContext";
import StripeLogo from "@/assets/stripe.svg";
import ComplyIcon from "@/assets/logo/comply-icon.png";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { PaymentAccountStatus } from "@/types/paymentAccount";
import SessionLoading from "@/components/Loading/SessionLoading";
import {
  useOnboardingLinkQuery,
  useSellerAccountStatusQuery,
} from "@/hooks/payments/usePaymentAccountsQueries";

function CreateSellerAccount() {
  const { isLoggedIn, isLoadingUser } = useAuth();
  const { data: accountStatus, isLoading: isLoadingStatus } =
    useSellerAccountStatusQuery(isLoggedIn && !isLoadingUser);
  const { data: onboardingLink } = useOnboardingLinkQuery(
    isLoggedIn && !isLoadingUser
  );

  const getStatusConfig = () => {
    switch (accountStatus) {
      case PaymentAccountStatus.None:
        return {
          title: "Torne-se um vendedor",
          description:
            "Configure sua conta de vendedor em minutos e comece a receber pagamentos de forma segura através do Stripe.",
          buttonText: "Finalizar conta de vendedor",
          buttonIcon: <Store className="w-5 h-5 mr-2" />,
          showBenefits: true,
          alert: null,
        };

      case PaymentAccountStatus.Incomplete:
        return {
          title: "Ative sua conta de vendedor",
          description:
            "Antes de começar a vender, você precisa finalizar a configuração da sua conta de vendedor no Stripe.",
          buttonText: "Continuar configuração",
          buttonIcon: <RefreshCw className="w-5 h-5 mr-2" />,
          showBenefits: false,
          alert: {
            variant: "default" as const,
            icon: <Clock className="h-5 w-5" />,
            title: "Configuração necessária",
            description:
              "Conclua as informações obrigatórias no Stripe para ativar sua conta de vendedor.",
          },
        };

      case PaymentAccountStatus.PendingReview:
        return {
          title: "Conta em análise",
          description:
            "Estamos revisando suas informações. Este processo geralmente leva até 48 horas.",
          buttonText: "Ver status no Stripe",
          buttonIcon: <Clock className="w-5 h-5 mr-2" />,
          showBenefits: false,
          alert: {
            variant: "default" as const,
            icon: <Clock className="h-5 w-5 text-blue-600" />,
            title: "Revisão em andamento",
            description:
              "O Stripe está verificando suas informações. Você receberá um e-mail assim que a análise for concluída.",
          },
        };

      case PaymentAccountStatus.Issues:
        return {
          title: "Ação necessária",
          description:
            "Identificamos alguns problemas que precisam ser resolvidos para ativar sua conta de vendedor.",
          buttonText: "Resolver problemas",
          buttonIcon: <AlertCircle className="w-5 h-5 mr-2" />,
          showBenefits: false,
          alert: {
            variant: "destructive" as const,
            icon: <XCircle className="h-5 w-5" />,
            title: "Problemas detectados",
            description:
              "Sua conta possui pendências que precisam ser resolvidas. Clique no botão abaixo para ver os detalhes e corrigir.",
          },
        };

      case PaymentAccountStatus.Active:
        return {
          title: "Conta ativa!",
          description:
            "Sua conta de vendedor está ativa e pronta para receber pagamentos.",
          buttonText: "Acessar dashboard Stripe",
          buttonIcon: <CheckCircle2 className="w-5 h-5 mr-2" />,
          showBenefits: false,
          alert: {
            variant: "default" as const,
            icon: <CheckCircle2 className="h-5 w-5 text-emerald-600" />,
            title: "Tudo pronto!",
            description:
              "Sua conta está completamente configurada. Você já pode criar anúncios e receber pagamentos.",
          },
        };

      default:
        return {
          title: "Torne-se um vendedor",
          description:
            "Configure sua conta de vendedor em minutos e comece a receber pagamentos de forma segura através do Stripe.",
          buttonText: "Finalizar conta de vendedor",
          buttonIcon: <Store className="w-5 h-5 mr-2" />,
          showBenefits: true,
          alert: null,
        };
    }
  };

  const config = getStatusConfig();

  if (isLoadingStatus) {
    return (
      <>
        <Header isCheckoutPage />
        <SessionLoading />
      </>
    );
  }

  return (
    <>
      <Header isCheckoutPage />
      <main className="relative w-full overflow-hidden">
        <div className="relative w-full max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-12 md:py-20">
          <div className="flex flex-col items-center justify-center text-center space-y-6">
            {/* Logo */}
            <div className="relative">
              <img
                src={ComplyIcon}
                alt="Comply"
                className="relative w-24 h-24 md:w-32 md:h-32 object-contain"
              />
            </div>

            {/* Title Section */}
            <div className="space-y-2 max-w-3xl">
              <h1 className="text-4xl sm:text-5xl md:text-6xl font-bold bg-gradient-to-r from-emerald-700 via-emerald-600 to-emerald-700 bg-clip-text text-transparent leading-tight">
                {config.title}
              </h1>
              <p className="text-lg sm:text-xl text-neutral-600 max-w-2xl mx-auto leading-relaxed">
                {config.description}
              </p>
            </div>
            {/* Alert de Status */}
            {config.alert && (
              <Alert
                variant={config.alert.variant}
                className="max-w-2xl text-left"
              >
                {config.alert.icon}
                <AlertTitle>{config.alert.title}</AlertTitle>
                <AlertDescription>{config.alert.description}</AlertDescription>
              </Alert>
            )}

            {/* Benefits Cards - apenas para None */}
            {config.showBenefits && (
              <div className="select-none grid grid-cols-1 md:grid-cols-3 gap-6 w-full mt-4">
                <div className="group relative flex flex-col items-center bg-white/80 backdrop-blur-sm rounded-2xl p-6 transition-all duration-300 hover:-translate-y-1 border border-emerald-100">
                  <img
                    src={StripeLogo}
                    alt="Stripe"
                    className="absolute -top-3 -left-3 w-20 h-15 bg-white rounded-2xl px-2 shadow-sm border border-neutral-200"
                  />

                  <div className="w-fit p-3 bg-emerald-600 rounded-xl mb-4 group-hover:scale-110 transition-transform">
                    <Lock className="text-white" />
                  </div>

                  <h3 className="text-lg font-semibold text-neutral-900 mb-2">
                    100% Seguro
                  </h3>
                  <p className="text-sm text-neutral-600">
                    Pagamentos processados com criptografia de ponta através do
                    Stripe
                  </p>
                </div>

                <div className="group flex flex-col items-center bg-white/80 backdrop-blur-sm rounded-2xl p-6 transition-all duration-300 hover:-translate-y-1 border border-emerald-100">
                  <div className="w-fit p-3 bg-emerald-600 rounded-xl mb-4 group-hover:scale-110 transition-transform">
                    <Zap className="text-white" />
                  </div>
                  <h3 className="text-lg font-semibold text-neutral-900 mb-2">
                    Configuração Rápida
                  </h3>
                  <p className="text-sm text-neutral-600">
                    Complete seu cadastro em poucos minutos e comece a vender
                  </p>
                </div>

                <div className="group flex flex-col items-center bg-white/80 backdrop-blur-sm rounded-2xl p-6 transition-all duration-300 hover:-translate-y-1 border border-emerald-100">
                  <div className="w-fit p-3 bg-emerald-600 rounded-xl mb-4 group-hover:scale-110 transition-transform">
                    <BanknoteArrowUp className="text-white" />
                  </div>
                  <h3 className="text-lg font-semibold text-neutral-900 mb-2">
                    Recebimentos Automáticos
                  </h3>
                  <p className="text-sm text-neutral-600">
                    Receba seus pagamentos automaticamente após cada venda
                  </p>
                </div>
              </div>
            )}

            {/* CTA Buttons */}
            <div className="flex flex-col sm:flex-row gap-4 w-full sm:w-auto mt-5">
              <Link to="/">
                <Button
                  variant="outline"
                  size="lg"
                  className="w-full sm:w-auto cursor-pointer border-2 border-neutral-300 hover:border-emerald-700 hover:text-emerald-700 transition-all group"
                >
                  <ArrowLeft className="w-4 h-4 mr-2 group-hover:-translate-x-1 transition-transform" />
                  Voltar à tela inicial
                </Button>
              </Link>

              <Button
                onClick={() =>
                  onboardingLink ? (window.location.href = onboardingLink) : ""
                }
                size="lg"
                disabled={!onboardingLink}
                className={`w-full sm:w-auto cursor-pointer transition-all group ${
                  accountStatus === PaymentAccountStatus.Issues
                    ? "bg-gradient-to-r from-red-600 to-red-500 hover:from-red-700 hover:to-red-600"
                    : "bg-gradient-to-r from-emerald-700 to-emerald-600 hover:from-emerald-800 hover:to-emerald-700"
                } ${!onboardingLink ? "opacity-50 cursor-not-allowed" : "hover:scale-101"}`}
              >
                {config.buttonIcon}
                {config.buttonText}
              </Button>
            </div>

            {/* Trust Badge */}
            <div className="flex items-center gap-2 text-sm text-neutral-500">
              <ShieldCheck className="w-4 h-4" />
              <span>Powered by Stripe • Seus dados estão protegidos</span>
            </div>
          </div>
        </div>
      </main>
    </>
  );
}

export default CreateSellerAccount;
