import { useEffect, useState } from "react";
import hammerImg from "@/assets/logo/hammer-white.png";

interface PaymentCompletionProps {
  onComplete?: () => void;
}

export default function PaymentCompletion({
  onComplete,
}: PaymentCompletionProps) {
  const [showText, setShowText] = useState(false);
  const [showHammer, setShowHammer] = useState(true);

  useEffect(() => {
    const animationTimer = setTimeout(() => {
      setShowText(true);
      setShowHammer(false);
    }, 1200);

    const redirectTimer = setTimeout(() => {
      if (onComplete) onComplete();
    }, 2500);

    return () => {
      clearTimeout(animationTimer);
      clearTimeout(redirectTimer);
    };
  }, [onComplete]);

  return (
    <div className="fixed inset-0 flex items-center justify-center z-50 overflow-hidden">
      <div className="absolute inset-0 flex items-center justify-center">
        <div className="bg-emerald-700 rounded-full w-0 h-0 animate-expandOriginCenter origin-center"></div>
      </div>

      <div className="relative z-10 flex flex-col items-center">
        {showHammer && (
          <img
            src={hammerImg}
            alt="Martelo"
            className="w-24 h-24 animate-hammer"
          />
        )}
        {showText && (
          <h1 className="text-white text-3xl font-bold mt-6 animate-fadeIn">
            É seu!
          </h1>
        )}
      </div>
    </div>
  );
}
