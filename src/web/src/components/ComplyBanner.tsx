import complyIcon from "@/assets/logo/comply-icon.png"

function ComplyBanner() {
  return (
    <div className="w-full p-10 flex justify-center items-center bg-emerald-100 select-none">
      <div className="flex items-center gap-6 ">
        <img
          src={complyIcon}
          alt="Notificações"
          className="w-20 md:w-45 object-cover"
        />
        <h1 className="text-xl md:text-5xl text-emerald-700 font-bold">
          Anúncio parado?
          <h1 className="text-xl md:text-5xl text-emerald-500 font-bold">
            Leiloa, que vende rápido.
          </h1>
        </h1>
      </div>
    </div>
  );
}

export default ComplyBanner;
