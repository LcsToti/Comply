import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import {
  Field,
  FieldGroup,
  FieldLabel,
  FieldSeparator,
} from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Link, useNavigate } from "react-router";
import { useAuth } from "@/contexts/AuthContext";
import { useLoginMutation } from "@/hooks/user/useAuthMutations";

export function LoginForm({
  className,
  ...props
}: React.ComponentProps<"form">) {
  const { login } = useAuth();
  const { mutate: loginQuery, isPending, isError, error } = useLoginMutation();
  const navigate = useNavigate();

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    loginQuery(
      {
        Email: formData.get("email") as string,
        Password: formData.get("password") as string,
      },
      {
        onSuccess: (res) => {
          login(res.token);
          navigate("/");
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
          <h1 className="text-2xl font-bold">Entre na sua conta</h1>
          <p className="text-muted-foreground text-sm text-balance">
            Entre com seu email abaixo para logar na sua conta
          </p>
        </div>
        <Field>
          <FieldLabel htmlFor="email">Email</FieldLabel>
          <Input
            id="email"
            name="email"
            type="email"
            placeholder="seu-email@exemplo.com"
            required
          />
        </Field>
        <Field>
          <div className="flex items-center">
            <FieldLabel htmlFor="password">Senha</FieldLabel>
            <a
              href="#"
              className="ml-auto text-sm underline-offset-4 hover:underline"
            >
              Esqueceu sua senha?
            </a>
          </div>
          <Input
            id="password"
            name="password"
            type="password"
            placeholder="Senha"
            required
          />
        </Field>
        <Field>
          <Button type="submit" className="bg-emerald-700 cursor-pointer" disabled={isPending}>
            {isPending ? "Entrando..." : "Entrar"}
          </Button>
          {isError && <p className="text-red-500">{error.message}</p>}
        </Field>
        <FieldSeparator>Não tem uma conta?</FieldSeparator>
        <Field>
          <Link to={"/register"}>
            <Button variant="outline" type="button" className="w-full cursor-pointer">
              Crie uma nova conta
            </Button>
          </Link>
        </Field>
      </FieldGroup>
    </form>
  );
}
