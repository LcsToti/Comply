import AdminSidebar from "@/components/AdminPageSideBar";
import { Button } from "@/components/ui/button";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";
import { useAuth } from "@/contexts/AuthContext";
import { Menu } from "lucide-react";
import { useEffect, useState } from "react";
import { Outlet, useNavigate } from "react-router";

function AdminLayout() {
  const { user, isLoadingUser } = useAuth();
  const navigate = useNavigate();
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  useEffect(() => {
    if (isLoadingUser) return;

    if (user?.role == 1) {
      navigate("/", { replace: true });
    }
  }, [user, isLoadingUser, navigate]);

  return (
    <div className="flex h-screen overflow-hidden">
      <aside className="hidden lg:block w-64 border-r border-gray-200 flex-shrink-0">
        <AdminSidebar />
      </aside>
      <div className="flex-1 flex flex-col min-w-0">
        {/* HEADER MOBILE APENAS */}
        <header className="lg:hidden bg-white border-b border-gray-200 px-4 py-4 flex items-center gap-4">
          <Sheet open={mobileMenuOpen} onOpenChange={setMobileMenuOpen}>
            <SheetTrigger asChild>
              <Button variant="ghost" size="icon">
                <Menu className="w-6 h-6" />
              </Button>
            </SheetTrigger>

            <SheetContent side="left" className="p-0 w-64">
              <AdminSidebar onClose={() => setMobileMenuOpen(false)} />
            </SheetContent>
          </Sheet>

          <h1 className="text-lg font-semibold text-gray-900">Admins</h1>
        </header>

        {/* CONTEÚDO */}
        <main className="flex-1 overflow-auto">
          <Outlet />
        </main>
      </div>
    </div>
  );
}

export default AdminLayout;
