import hammerImg from "@/assets/logo/comply-icon.png";

export default function SessionLoading() {
  return (
    <div className="fixed inset-0 flex items-center justify-center z-50 bg-white/80 backdrop-blur">
      <div className="flex flex-col items-center gap-4">
        <img src={hammerImg} alt="Carregando" className="w-30 h-30" />

        <div className="w-48 h-[3px] bg-gray-200 overflow-hidden rounded">
          <div className="h-full w-full bg-emerald-600 animate-progress" />
        </div>
      </div>
    </div>
  );
}
