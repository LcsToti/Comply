import SessionLoading from "@/components/Loading/SessionLoading";
import { SellerAccountStatusBanner } from "@/components/SellerAccountStatusBanner";
import { Button } from "@/components/ui/button";
import {
  Item,
  ItemMedia,
  ItemContent,
  ItemTitle,
  ItemDescription,
  ItemActions,
} from "@/components/ui/item";
import { useAuth } from "@/contexts/AuthContext";
import { useOnboardingLinkQuery } from "@/hooks/payments/usePaymentAccountsQueries";
import { PaymentAccountStatus } from "@/types/paymentAccount";
import { Handbag, SquarePlus, TriangleAlert } from "lucide-react";
import { useEffect } from "react";
import { Link, useNavigate, useSearchParams } from "react-router";
import successImg from "@/assets/images/success.jpg";
import fullLogoSvg from "@/assets/logo/full-logo.svg";

export default function RegisterSuccess() {
  const navigate = useNavigate();
  const { accountStatus, isLoadingAccountStatus } = useAuth();
  const { data: onboardingLink } = useOnboardingLinkQuery(
    !isLoadingAccountStatus
  );
  const [searchParams] = useSearchParams();
  const userWithStripe = searchParams.get("type") === "stripeSuccess";

  const successDescription = userWithStripe
    ? "Você pode começar a comprar e vender produtos na nossa plataforma."
    : "Você pode começar a comprar produtos na nossa plataforma.";

  const successAction = userWithStripe
    ? "Por onde deseja começar?"
    : "Comece a explorar nossos produtos!";

  useEffect(() => {
    if (isLoadingAccountStatus) return;

    if (userWithStripe) {
      const shouldRedirect =
        accountStatus !== PaymentAccountStatus.Active &&
        accountStatus !== PaymentAccountStatus.PendingReview;

      if (shouldRedirect) {
        navigate("/register/seller", { replace: true });
      }
    }
  }, [userWithStripe, accountStatus, isLoadingAccountStatus, navigate]);

  if (isLoadingAccountStatus) {
    return <SessionLoading />;
  }

  return (
    <div className="grid min-h-svh lg:grid-cols-2">
      <div className="bg-muted relative hidden lg:block">
        <img
          src={successImg}
          alt="Image"
          className="absolute inset-0 h-full w-full object-cover dark:brightness-[0.2] dark:grayscale"
        />
      </div>
      <div className="flex flex-col gap-4 p-6 md:p-10">
        <div className="flex justify-center gap-2 md:justify-start">
          <Link to={"/"} className="flex items-center gap-2 font-medium">
            <img src={fullLogoSvg} className="w-40" />
          </Link>
        </div>
        <div className="flex flex-1 flex-col gap-6 items-center justify-center">
          <div className="w-full max-w-lg flex flex-col gap-6">
            {accountStatus && (
              <SellerAccountStatusBanner
                status={accountStatus}
                onboardingLink={onboardingLink}
              />
            )}

            <h1 className="text-5xl font-bold text-emerald-700">
              Cadastro concluído!
            </h1>
            <h2 className="text-2xl font-medium text-neutral-500">
              {successDescription}
            </h2>
            <h3 className="text-2xl font-bold text-neutral-700">
              {successAction}
            </h3>
          </div>
          <div className="flex w-full max-w-lg flex-col gap-6">
            <Item variant="muted" className="">
              <ItemMedia variant="image">
                <Handbag className="text-emerald-700" />
              </ItemMedia>
              <ItemContent>
                <ItemTitle className="text-emerald-700 font-bold">
                  Procure por produtos
                </ItemTitle>
                <ItemDescription className="text-emerald-700">
                  Veja os produtos e leilões em alta.
                </ItemDescription>
              </ItemContent>
              <ItemActions>
                <Button
                  size="sm"
                  variant="outline"
                  className="bg-emerald-700 text-white cursor-pointer"
                  onClick={() => {
                    navigate("/");
                  }}
                >
                  Ir para produtos
                </Button>
              </ItemActions>
            </Item>
            {!userWithStripe && (
              <Item variant="outline" className="bg-blue-500">
                <ItemMedia variant="image">
                  <TriangleAlert className="text-white" />
                </ItemMedia>
                <ItemContent>
                  <ItemTitle className="text-white">
                    Sua conta é de comprador.
                  </ItemTitle>
                  <ItemDescription className="text-white">
                    Para anunciar produtos, conecte-se ao Stripe.
                  </ItemDescription>
                </ItemContent>
                <ItemActions>
                  <Button
                    size="sm"
                    variant="outline"
                    className="bg-blue-500 text-white cursor-pointer"
                    onClick={() => {
                      navigate("/register/seller");
                    }}
                  >
                    Ir para Stripe
                  </Button>
                </ItemActions>
              </Item>
            )}
            {userWithStripe && (
              <Item variant="outline">
                <ItemMedia variant="image">
                  <SquarePlus />
                </ItemMedia>
                <ItemContent>
                  <ItemTitle>Venda um produto</ItemTitle>
                  <ItemDescription>
                    Coloque aquele item empoeirado à venda.
                  </ItemDescription>
                </ItemContent>
                <ItemActions>
                  <Button
                    size="sm"
                    variant="outline"
                    className="cursor-pointer"
                    onClick={() => {
                      navigate("/product/create");
                    }}
                  >
                    Anunciar
                  </Button>
                </ItemActions>
              </Item>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
