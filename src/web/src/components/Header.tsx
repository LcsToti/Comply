import React, { useState } from "react";
import { Link, useNavigate } from "react-router";
import {
  Search,
  MapPin,
  ChevronDown,
  LayoutGrid,
  Eye,
  Plus,
  Menu,
} from "lucide-react";
import FullLogo from "../assets/logo/full-logo.svg";
import { Button } from "./ui/button";
import { Avatar, AvatarFallback, AvatarImage } from "./ui/avatar";
import { useAuth } from "@/contexts/AuthContext";
import { NotificationsButton } from "./NotificationsButton";

export type HeaderProps = {
  isCheckoutPage?: boolean;
};

export default function Header({ isCheckoutPage }: HeaderProps) {
  const [mobileOpen, setMobileOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const navigate = useNavigate();
  const { user } = useAuth(); // pega o user

  return (
    <header className="sticky top-0 z-50 bg-white/90 backdrop-blur border-b">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="flex h-16 items-center justify-between">
          {/* LEFT */}
          <div className="flex items-center gap-4 min-w-0">
            {isCheckoutPage ? null : (
              <button
                className="mr-2 inline-flex items-center rounded-md p-2 text-gray-700 hover:bg-gray-100 lg:hidden"
                onClick={() => setMobileOpen(!mobileOpen)}
                aria-label="Abrir menu"
              >
                <Menu className="h-5 w-5" />
              </button>
            )}
            <Logo />

            {isCheckoutPage ? null : (
              <div className="hidden items-center gap-3 rounded-full border bg-gray-50 px-3 py-1 lg:flex min-w-0">
                <input
                  className="w-72 max-w-[44ch] min-w-0 bg-transparent text-sm outline-none placeholder-gray-400"
                  placeholder="Digite aqui..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter") {
                      e.preventDefault();
                      navigate(`/search?q=${searchQuery}`);
                    }
                  }}
                />
                <div className="mx-2 h-6 w-px bg-gray-200" />
                <Button
                  variant={"ghost"}
                  className="flex items-center gap-1 rounded-md rounded-r-2xl px-2 py-1 text-sm text-gray-600 hover:bg-gray-200 cursor-pointer"
                  onClick={() => navigate(`/search?q=${searchQuery}`)}
                >
                  <Search className="h-4 w-4 text-gray-500" />
                  <span>Pesquisar</span>
                </Button>
              </div>
            )}
          </div>

          {/* RIGHT */}
          {isCheckoutPage ? null : (
            <>
              <div className="hidden items-center gap-3 lg:flex min-w-0">
                <nav className="flex items-center gap-2">
                  <NavButton
                    to="/dashboard/MyListings"
                    icon={<LayoutGrid className="h-4 w-4" />}
                  >
                    Meus anúncios
                  </NavButton>
                  <NavButton
                    to="/dashboard/MyWatchList"
                    icon={<Eye className="h-4 w-4" />}
                  >
                    Minha lista
                  </NavButton>
                  <NotificationsButton />
                </nav>

                {/* Se o usuário está logado */}
                {user ? (
                  <>
                    <UserMenuSimple />
                    <PublishButton />
                  </>
                ) : (
                  <Button
                    onClick={() => navigate("/login")}
                    className="bg-emerald-700 text-white hover:bg-emerald-600 cursor-pointer"
                  >
                    Fazer Login
                  </Button>
                )}
              </div>

              {/* Mobile */}
              <div className="flex items-center gap-2 lg:hidden">
                {user ? (
                  <UserMenuSimple />
                ) : (
                  <Button
                    onClick={() => navigate("/login")}
                    className="bg-emerald-700 text-white hover:bg-emerald-600"
                  >
                    Fazer Login
                  </Button>
                )}
              </div>
            </>
          )}
        </div>
      </div>

      {/* Mobile drawer */}
      {mobileOpen && (
        <div className="lg:hidden">
          <div className="mx-auto max-w-7xl px-4 pb-4">
            <div className="flex flex-col gap-3 rounded-md bg-white p-3 shadow">
              <div className="flex items-center gap-2">
                <Search className="h-4 w-4 text-gray-500" />
                <input
                  className="w-full h-9 bg-transparent text-sm outline-none placeholder-gray-400"
                  placeholder="Digite aqui..."
                />
                <button className="ml-2 flex items-center gap-1 rounded-md px-2 py-1 text-sm text-gray-600 hover:bg-gray-100">
                  <MapPin className="h-4 w-4" />
                  <span className="max-w-[120px] truncate block">MG</span>
                  <ChevronDown className="h-4 w-4" />
                </button>
              </div>

              <div className="flex flex-col gap-1 border-t pt-2">
                <MobileNavItem
                  to="/dashboard/MyListings"
                  icon={<LayoutGrid className="h-4 w-4" />}
                >
                  Meus anúncios
                </MobileNavItem>
                <MobileNavItem
                  to="/dashboard/MyWatchList"
                  icon={<Eye className="h-4 w-4" />}
                >
                  Minha lista
                </MobileNavItem>
                <NotificationsButton />

                {user ? (
                  <PublishButton full />
                ) : (
                  <Button
                    onClick={() => navigate("/login")}
                    className="bg-emerald-700 text-white hover:bg-emerald-600 w-full"
                  >
                    Logar
                  </Button>
                )}
              </div>
            </div>
          </div>
        </div>
      )}
    </header>
  );
}

/* ----------------- Subcomponentes ----------------- */

function Logo() {
  return (
    <Link to="/" className="flex items-center">
      <div className="h-8 w-auto">
        <img src={FullLogo} alt="Comply Logo" className="h-full w-auto" />
      </div>
    </Link>
  );
}

function NavButton({
  to,
  icon,
  children,
}: {
  to: string;
  icon: React.ReactNode;
  children: React.ReactNode;
}) {
  return (
    <Link
      to={to}
      className="inline-flex items-center gap-2 rounded-md px-3 py-2 text-sm text-gray-700 hover:bg-gray-200"
    >
      {icon}
      <span className="hidden whitespace-nowrap xl:inline">{children}</span>
    </Link>
  );
}

export function UserMenuSimple() {
  const { user } = useAuth();
  const initials =
    user?.name
      ?.split(" ")
      .map((n) => n[0])
      .join("")
      .substring(0, 2)
      .toUpperCase() || "US";
  return (
    <Link
      to="/dashboard/profile"
      className="inline-flex items-center gap-2 rounded-full p-1 hover:bg-gray-100"
    >
      <Avatar>
        <AvatarImage src={user?.profilePic || undefined} alt={user?.name} />
        <AvatarFallback className="bg-emerald-100">{initials}</AvatarFallback>
      </Avatar>
    </Link>
  );
}

function PublishButton({ full }: { full?: boolean }) {
  const base =
    "inline-flex items-center gap-2 rounded-md px-4 py-2 text-sm font-medium";
  const style = "bg-emerald-700 text-white hover:bg-emerald-600";

  // Versão mobile (full)
  if (full)
    return (
      <Link
        to="/product/create"
        className={`${base}
       ${style} w-full justify-center`}
      >
        <Plus className="h-4 w-4" />
        Anuncie grátis
      </Link>
    );

  // Versão desktop
  return (
    <Link to="/product/create" className={`${base} ${style}`}>
      <Plus className="h-4 w-4" />
      <span className="hidden whitespace-nowrap xl:inline">Anuncie grátis</span>
    </Link>
  );
}

function MobileNavItem({
  to,
  icon,
  children,
}: {
  to: string;
  icon: React.ReactNode;
  children: React.ReactNode;
}) {
  return (
    <Link
      to={to}
      className="flex items-center gap-2 rounded-md px-3 py-2 text-sm text-gray-700 hover:bg-gray-50"
    >
      {icon}
      <span>{children}</span>
    </Link>
  );
}
