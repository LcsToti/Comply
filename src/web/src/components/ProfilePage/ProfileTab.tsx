import type { User, UserAddress } from "@/types/user";
import ProfileFields from "./ProfileFields";
import AddAddressModal from "../NewAddress";
import { Button } from "../ui/button";
import { MapPin, Trash2, User as UserIcon, Shield } from "lucide-react";
import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "../ui/dialog";
import { useDeleteAddressMutation } from "@/hooks/user/useUserAdressesMutations";
import { useUpdateMyProfileMutation } from "@/hooks/user/useUsersMutations";

export type ProfileTabProps = {
  user: User | undefined;
  addresses: UserAddress[] | undefined;
  addressDataIsPending: boolean;
  onAddAddress?: () => void;
};

const ProfileTab = ({
  user,
  addresses,
  addressDataIsPending,
}: ProfileTabProps) => {
  const deleteAddress = useDeleteAddressMutation();
  const updateProfile = useUpdateMyProfileMutation();

  const [open, setOpen] = useState(false);
  const [selectedId, setSelectedId] = useState<string | null>(null);

  const confirmDelete = (id: string) => {
    setSelectedId(id);
    setOpen(true);
  };

  const handleDelete = async () => {
    if (!selectedId) return;
    await deleteAddress.mutateAsync(selectedId);
    setSelectedId(null);
    setOpen(false);
  };

  const handleUpdate = async (field: "Name" | "PhoneNumber", value: string) => {
    await updateProfile.mutateAsync({ [field]: value });
  };

  return (
    <div className="space-y-8">
      {/* Header com gradiente e badge */}
      <header className="mb-8">
        <h1 className="text-4xl font-bold text-gray-900">Meu cadastro</h1>
        <p className="mt-1 text-base text-gray-600">
          Configure seu cadastro e aumente a confiança do seu perfil
        </p>
      </header>

      {/* Dados da Conta Card */}
      <section className="bg-white rounded-2xl border border-neutral-200 duration-300 overflow-hidden">
        <div className="px-6 py-4 border-b border-neutral-200">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-xl bg-emerald-600 flex items-center justify-center">
              <UserIcon className="w-5 h-5 text-white" />
            </div>
            <div>
              <h2 className="text-xl font-semibold text-neutral-900">
                Dados da conta
              </h2>
              <p className="text-sm text-neutral-500">
                Informações pessoais e de contato
              </p>
            </div>
          </div>
        </div>

        <div className="p-6 space-y-4">
          <ProfileFields
            label="Nome completo"
            value={user?.name}
            editable
            onEdit={(newValue) => handleUpdate("Name", newValue)}
          />
          <ProfileFields label="Email" value={user?.email} editable={false} />

          <ProfileFields
            label="Número de telefone"
            value={user?.phoneNumber}
            editable
            onEdit={(newValue) => handleUpdate("PhoneNumber", newValue)}
          />
        </div>
      </section>

      {/* Endereços Card */}
      <section className="bg-white rounded-2xl border border-neutral-200 duration-300 overflow-hidden">
        <div className="px-6 py-4 border-b border-neutral-200">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 rounded-xl bg-emerald-600 flex items-center justify-center">
                <MapPin className="w-5 h-5 text-white" />
              </div>
              <div>
                <h2 className="text-xl font-semibold text-neutral-900">
                  Endereços
                </h2>
                <p className="text-sm text-neutral-500">
                  {Array.isArray(addresses) ? addresses.length : 0} endereço(s)
                  cadastrado(s)
                </p>
              </div>
            </div>
            <AddAddressModal />
          </div>
        </div>

        <div className="p-6">
          {addressDataIsPending ? (
            <div className="space-y-3">
              {[...Array(2)].map((_, i) => (
                <div
                  key={i}
                  className="h-20 w-full animate-pulse rounded-xl bg-gradient-to-r from-neutral-100 to-neutral-50"
                />
              ))}
            </div>
          ) : Array.isArray(addresses) && addresses.length > 0 ? (
            <div className="space-y-3">
              {addresses.map((address) => (
                <div
                  key={address.id}
                  className="group relative border border-neutral-200 rounded-xl p-4 transition-all duration-300"
                >
                  <div className="flex items-start justify-between gap-4">
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-2 mb-1">
                        <MapPin className="w-4 h-4 text-emerald-600 flex-shrink-0" />
                        <p className="font-semibold text-neutral-900">
                          {address.street}, {address.number}
                        </p>
                      </div>
                      <p className="text-sm text-neutral-600 ml-6">
                        {address.city}/{address.state} • CEP {address.zipCode}
                      </p>
                    </div>
                    <Button
                      variant="ghost"
                      size="icon"
                      className="flex-shrink-0 text-neutral-400 hover:text-red-600 hover:bg-red-50 opacity-0 group-hover:opacity-100 transition-all"
                      onClick={() => confirmDelete(address.id)}
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-12">
              <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-neutral-100 mb-4">
                <MapPin className="w-8 h-8 text-neutral-400" />
              </div>
              <p className="text-sm text-neutral-600 mb-4">
                Nenhum endereço cadastrado ainda
              </p>
              <AddAddressModal />
            </div>
          )}
        </div>
      </section>

      {/* Security Badge - opcional */}
      <div className="flex items-center justify-center gap-2 text-sm text-neutral-500">
        <Shield className="w-4 h-4 text-emerald-600" />
        <span>Seus dados estão protegidos e criptografados</span>
      </div>

      {/* Dialog de confirmação de exclusão */}
      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <div className="w-10 h-10 rounded-full bg-red-100 flex items-center justify-center">
                <Trash2 className="w-5 h-5 text-red-600" />
              </div>
              Remover endereço
            </DialogTitle>
            <DialogDescription className="pt-2">
              Tem certeza que deseja excluir este endereço? Essa ação não pode
              ser desfeita.
            </DialogDescription>
          </DialogHeader>

          <DialogFooter className="flex gap-2 sm:gap-2">
            <Button
              variant="outline"
              onClick={() => setOpen(false)}
              className="flex-1 sm:flex-1"
            >
              Cancelar
            </Button>
            <Button
              variant="destructive"
              onClick={handleDelete}
              disabled={deleteAddress.isPending}
              className="flex-1 sm:flex-1"
            >
              {deleteAddress.isPending ? "Removendo..." : "Remover"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default ProfileTab;
