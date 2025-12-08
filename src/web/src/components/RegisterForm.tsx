import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import {
  Field,
  FieldDescription,
  FieldGroup,
  FieldLabel,
  FieldSeparator,
} from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Link } from "react-router";
import { useAuth } from "@/contexts/AuthContext";
import { useState } from "react";
import { TermsOfUseModal } from "./TermsOfUse";
import { toast } from "sonner";
import { CheckCircle2, AlertCircle, FileText } from "lucide-react";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { useRegisterMutation } from "@/hooks/user/useAuthMutations";

export function RegisterForm({
  className,
  onSuccess,
  ...props
}: React.ComponentProps<"form"> & { onSuccess?: () => void }) {
  const { login } = useAuth();
  const { mutate: register, isPending, isError, error } = useRegisterMutation();
  const [showTerms, setShowTerms] = useState(false);
  const [termsAccepted, setTermsAccepted] = useState(false);

  const handleAccept = () => {
    setTermsAccepted(true);
    toast.success("Termos aceitos com sucesso!", {
      description: "Agora você pode concluir o cadastro",
      icon: <CheckCircle2 className="w-5 h-5 text-emerald-600" />,
    });
  };

  const handleDecline = () => {
    setTermsAccepted(false);
    toast.error("Você precisa aceitar os termos para continuar", {
      description: "Leia e aceite os termos de uso para criar sua conta",
    });
  };

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    // Validação adicional de segurança
    if (!termsAccepted) {
      toast.error("Aceite os termos de uso", {
        description: "Você precisa ler e aceitar os termos antes de continuar",
      });
      return;
    }

    const formData = new FormData(e.currentTarget);
    const password = formData.get("password") as string;
    const confirmPassword = formData.get("confirm-password") as string;

    // Validação de senha
    if (password !== confirmPassword) {
      toast.error("As senhas não coincidem", {
        description: "Por favor, verifique se as senhas digitadas são iguais",
      });
      return;
    }

    register(
      {
        Name: formData.get("name") as string,
        Email: formData.get("email") as string,
        Password: password,
      },
      {
        onSuccess: (res) => {
          onSuccess?.();
          login(res.token);
        },
      }
    );
  };

  return (
    <form
      onSubmit={handleSubmit}
      className={cn("flex flex-col gap-6", className)}
      {...props}
    >
      <FieldGroup>
        <div className="flex flex-col items-center gap-1 text-center">
          <h1 className="text-2xl font-bold">Crie sua conta</h1>
          <p className="text-muted-foreground text-sm text-balance">
            Compre, Leiloe e Venda com a garantia Comply{" "}
          </p>
        </div>

        <Field>
          <FieldLabel htmlFor="name">Nome completo</FieldLabel>
          <Input
            id="name"
            name="name"
            type="text"
            placeholder="Matheus Zaita"
            required
            disabled={isPending}
          />
        </Field>

        <Field>
          <FieldLabel htmlFor="email">Email</FieldLabel>
          <Input
            id="email"
            type="email"
            name="email"
            placeholder="seu-email@example.com"
            required
            disabled={isPending}
          />
          <FieldDescription>
            Não compartilharemos seu e-mail com ninguém.
          </FieldDescription>
        </Field>

        <Field>
          <FieldLabel htmlFor="password">Senha</FieldLabel>
          <Input
            id="password"
            name="password"
            type="password"
            required
            disabled={isPending}
            minLength={8}
          />
          <FieldDescription>
            Deve conter pelo menos 8 caracteres, incluindo uma letra maiúscula,
            um número e um caractere especial.
          </FieldDescription>
        </Field>

        <Field>
          <FieldLabel htmlFor="confirm-password">Confirme sua senha</FieldLabel>
          <Input
            id="confirm-password"
            name="confirm-password"
            type="password"
            required
            disabled={isPending}
            minLength={8}
          />
          <FieldDescription>Por favor confirme sua senha.</FieldDescription>
        </Field>

        {/* Seção de Termos de Uso */}
        <Field>
          <div className="space-y-3">
            {/* Botão para abrir termos */}
            <Button
              type="button"
              variant={termsAccepted ? "outline" : "secondary"}
              className={cn(
                "w-full relative",
                termsAccepted && "border-emerald-600 text-emerald-700"
              )}
              onClick={() => setShowTerms(true)}
              disabled={isPending}
            >
              <FileText className="w-4 h-4 mr-2" />
              {termsAccepted ? "Termos Aceitos" : "Ler e Aceitar Termos de Uso"}
              {termsAccepted && (
                <CheckCircle2 className="w-4 h-4 ml-2 text-emerald-600" />
              )}
            </Button>

            {/* Alert de status */}
            {termsAccepted ? (
              <Alert className="border-emerald-200 bg-emerald-50">
                <CheckCircle2 className="h-4 w-4 text-emerald-600" />
                <AlertDescription className="text-sm text-emerald-800">
                  Você aceitou os termos de uso. Prossiga com o cadastro.
                </AlertDescription>
              </Alert>
            ) : (
              <Alert className="border-amber-200 bg-amber-50">
                <AlertCircle className="h-4 w-4 text-amber-600" />
                <AlertDescription className="text-sm text-amber-800">
                  <span className="font-semibold">Obrigatório:</span> Você deve
                  ler e aceitar os termos de uso para criar sua conta.
                </AlertDescription>
              </Alert>
            )}
          </div>
        </Field>

        {/* Modal de Termos */}
        <TermsOfUseModal
          open={showTerms}
          onOpenChange={setShowTerms}
          onAccept={handleAccept}
          onDecline={handleDecline}
          requireScroll={true}
        />

        {/* Botão de Submit */}
        <Field>
          <Button
            type="submit"
            className={cn(
              "w-full",
              termsAccepted
                ? "bg-emerald-700 hover:bg-emerald-800 cursor-pointer"
                : "bg-gray-400 hover:bg-gray-400 cursor-not-allowed"
            )}
            disabled={isPending || !termsAccepted}
          >
            {isPending ? (
              <>
                <span className="animate-pulse">Registrando...</span>
              </>
            ) : termsAccepted ? (
              "Criar Conta"
            ) : (
              <>
                <AlertCircle className="w-4 h-4 mr-2" />
                Aceite os Termos para Continuar
              </>
            )}
          </Button>
          {isError && (
            <Alert className="border-red-200 bg-red-50 mt-2">
              <AlertCircle className="h-4 w-4 text-red-600" />
              <AlertDescription className="text-sm text-red-800">
                {error.message}
              </AlertDescription>
            </Alert>
          )}
        </Field>

        <FieldSeparator>Já possui uma conta?</FieldSeparator>

        <Field>
          <Link to={"/login"}>
            <Button
              type="button"
              variant={"outline"}
              className="w-full cursor-pointer"
              disabled={isPending}
            >
              Entre na sua conta
            </Button>
          </Link>
        </Field>
      </FieldGroup>
    </form>
  );
}
