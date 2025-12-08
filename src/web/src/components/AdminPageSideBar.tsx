import {
  Home,
  LayoutDashboard,
  MessageCircleQuestionMark,
  Wallet,
} from "lucide-react";
import { Link, useLocation } from "react-router";
import FullLogo from "@/assets/logo/full-logo.svg";
const sidebarItems = [
  { icon: LayoutDashboard, label: "Dashboard", href: "/admin" },
  {
    icon: MessageCircleQuestionMark,
    label: "Tickets de Suporte",
    href: "/admin/tickets",
  },
  {
    icon: Wallet,
    label: "Disputas Abertas",
    href: "/admin/disputes",
  },
];

function AdminSidebar({ onClose }: { onClose?: () => void }) {
  const location = useLocation();
  const currentPath = location.pathname;
  return (
    <div className="flex flex-col h-full bg-white overflow-auto">
      <div className="p-6 border-b border-gray-200">
        <Link to="/admin" className="flex items-center gap-2" onClick={onClose}>
          <img src={FullLogo} className="w-45" alt="Logo" />
        </Link>
      </div>

      <nav className="flex-1 p-4 space-y-1">
        {sidebarItems.map((item) => {
          const Icon = item.icon;
          const isActive = currentPath === item.href;

          return (
            <Link
              key={item.label}
              to={item.href}
              onClick={onClose}
              className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
                isActive
                  ? "bg-emerald-700 text-white"
                  : "text-gray-700 hover:bg-gray-100"
              }`}
            >
              <Icon className="w-5 h-5" />
              <span className="font-medium">{item.label}</span>
            </Link>
          );
        })}
      </nav>

      <div className="p-4 space-y-1 border-gray-200">
        <Link
          to={"/"}
          className="flex items-center cursor-pointer gap-3 px-4 py-3 rounded-lg text-gray-700 hover:bg-emerald-50 w-full transition-colors"
        >
          <Home className="w-5 h-5" />
          <span className="font-medium">Ir para loja</span>
        </Link>
      </div>
    </div>
  );
}

export default AdminSidebar;
