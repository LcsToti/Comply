import { useNavigate } from "react-router";
import { Button } from "./ui/button";
import { TermsOfUseModal } from "./TermsOfUse";
import { useState } from "react";

function Footer() {
  const [openTerms, setOpenTerms] = useState(false);
  const navigate = useNavigate();
  return (
    <>
      <footer className="bg-emerald-700 text-white py-6">
        <div className="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex flex-col sm:flex-row justify-between items-center">
          {/* Seção de Copyright */}
          <div className="text-sm text-white">
            &copy; {new Date().getFullYear()} Comply. Todos os direitos
            reservados.
          </div>

          {/* Seção de Links (potencial) */}
          <div className="flex space-x-6 mt-4 sm:mt-0">
            <Button
              onClick={() => {
                window.scrollTo({ top: 0, behavior: "smooth" });
                navigate("/faq");
              }}
              className="text-sm text-white transition-colors duration-300"
              variant={"link"}
            >
              Perguntas Frequentes
            </Button>
            <Button
              onClick={() => {
                setOpenTerms(true);
              }}
              variant={"link"}
              className="text-sm text-white cursor-pointer transition-colors duration-300"
            >
              Termos de uso
            </Button>
          </div>
        </div>
      </footer>
      <TermsOfUseModal
        requireScroll={false}
        open={openTerms}
        onOpenChange={setOpenTerms}
        onAccept={() => {}}
      />
    </>
  );
}

export default Footer;
