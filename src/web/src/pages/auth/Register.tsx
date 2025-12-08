import { RegisterForm } from "@/components/RegisterForm";
import { StripeModal } from "@/components/StripeModal";
import { useAuth } from "@/contexts/AuthContext";
import { useOnboardingLinkQuery } from "@/hooks/payments/usePaymentAccountsQueries";
import { useState } from "react";
import { Link, useNavigate } from "react-router";
import fullLogo from "@/assets/logo/full-logo.svg";
import loginSvg from "@/assets/images/login.svg"

export default function Register() {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const { isLoggedIn, isLoadingUser } = useAuth();
  const { data: onboardingLink } = useOnboardingLinkQuery(
    isLoggedIn && !isLoadingUser
  );
  const navigate = useNavigate();

  const handleConfirmStripe = () => {
    if (onboardingLink) {
      window.location.href = onboardingLink;
    }
  };

  const handleSkipStripe = () => {
    setIsModalOpen(false);
    navigate("/register/success");
  };

  return (
    <div className="grid min-h-svh lg:grid-cols-2">
      <div className="flex flex-col gap-4 p-6 md:p-10">
        <div className="flex justify-center gap-2 md:justify-start">
          <Link to={"/"} className="flex items-center gap-2 font-medium">
            <img src={fullLogo} className="w-40" />
          </Link>
        </div>
        <div className="flex flex-1 items-center justify-center">
          <div className="w-full max-w-xs">
            <RegisterForm onSuccess={() => setIsModalOpen(true)} />
          </div>
        </div>
      </div>
      <div className="bg-muted relative hidden lg:block">
        <img
          src={loginSvg}
          alt="Image"
          className="absolute inset-0 h-full w-full object-cover dark:brightness-[0.2] dark:grayscale"
        />
      </div>
      <StripeModal
        isOpen={isModalOpen}
        onConfirm={handleConfirmStripe}
        onSkip={handleSkipStripe}
        onClose={() => setIsModalOpen(false)}
      />
    </div>
  );
}
