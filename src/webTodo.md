# 🧾 To-Do List — Front-End (React) - Versão Aprimorada

## ⚙️ Estrutura Base e Configuração
- [\(\checkmark \)] Criar projeto React com Vite (`npm create vite@latest`)
- [ ] Configurar **ESLint** e **Prettier** para padronização de código
- [ ] Configurar **React Router DOM** para gerenciamento de rotas
- [ ] Configurar **Axios** com `baseURL` e interceptors (para injeção e refresh de JWT)
- [ ] Criar **Context/Auth Provider** para estado de autenticação e dados do usuário
- [ ] Criar **Context/Notification Provider** para notificações globais (Toast/Snackbar)
- [ ] Configurar **React Query (TanStack Query)** para `server state management` (caching, fetching, etc.)
- [ ] Considerar **Zustand** ou **Jotai** para `client state management` complexo (filtros, estado de modais, etc.)
- [ ] Criar **tema global** e **estilos base** (ex: Tailwind CSS + shadcn/ui)
- [\(\checkmark \)] Definir estrutura de pastas:
  - [\(\checkmark \)] `components/` (componentes reutilizáveis)
  - [\(\checkmark \)] `pages/` (páginas da aplicação)
  - [\(\checkmark \)] `services/` ou `api/` (chamadas de API, ex: `authService.ts`)
  - [\(\checkmark \)] `hooks/` (hooks customizados, ex: `useAuth.ts`)
  - [\(\checkmark \)] `utils/` ou `helpers/` (funções utilitárias)
  - [\(\checkmark \)] `contexts/` (provedores de contexto)
  - [\(\checkmark \)] `lib/` (configuração de libs, ex: `axios.ts`)
  - [ ] `types/` (interfaces e tipos do TypeScript, se aplicável)

---

## 🔑 Autenticação
### Páginas
- [ ] **LoginPage** — formulário de login
- [ ] **RegisterPage** — formulário de registro

### Componentes e Lógica
- [ ] `AuthForm` — componente genérico para login/registro
- [ ] `ProtectedRoute` / `RouteGuard` — HOC ou componente para proteger rotas privadas
- [ ] `authService` com funções `login()`, `register()`, `logout()`, `refreshToken()`
- [ ] Lógica para guardar JWT no `localStorage` ou `HttpOnly cookie`
- [ ] Axios interceptor para injetar o `Authorization header` nas requisições
- [ ] Axios interceptor para lidar com erro 401 e executar o `refreshToken()`
- [ ] Lógica de auto-logout no cliente se o refresh token expirar

---

## 🏠 Home e Catálogo de Produtos
### Páginas
- [ ] **HomePage / ExplorePage** — listagem de todos os leilões/produtos
- [ ] **MyListingsPage** — listagem de produtos criados pelo usuário logado

### Componentes
- [ ] `ProductCard` — card de produto (imagem, título, preço/lance atual, tempo restante)
- [ ] `ProductList` — grid ou lista que renderiza os `ProductCard`
- [ ] `SearchBar` — campo de busca
- [ ] `FilterBar` — filtros de categoria, preço, status (ativo, finalizado), etc.
- [ ] `Pagination` — componente para navegar entre as páginas de resultados
- [ ] `EmptyState` para quando nenhuma busca ou filtro retorna resultados

---

## 📦 Página de Detalhes do Produto
### Página
- [ ] **ProductDetailPage**

### Componentes
- [ ] `ImageGallery` — galeria de imagens do produto
- [ ] `Timer` — contador regressivo para o fim do leilão
- [ ] `BidForm` — formulário para submeter um novo lance (usando **React Hook Form** + **Zod**)
- [ ] `BidHistory` — lista com o histórico de lances
- [ ] `BuyNowButton` — botão para compra imediata (se aplicável)
- [ ] `SellerInfo` — informações sobre o vendedor

---

## ➕ Cadastro de Novo Produto
### Página
- [ ] **CreateListingPage**

### Componentes
- [ ] `ProductForm` — formulário completo (nome, descrição, preço inicial, duração, etc.)
  - [ ] Implementar com **React Hook Form** para gerenciar estado e validação
  - [ ] Usar **Zod** para o schema de validação
- [ ] `ImageUpload` — componente para upload de imagens com preview

---

## 👤 Conta do Usuário
### Página
- [ ] **AccountPage** — página de perfil com múltiplas seções/abas

### Componentes
- [ ] `UserInfoCard` — exibe dados do usuário
- [ ] `EditProfileForm` — formulário para editar informações do usuário
- [ ] `UserBidsList` — histórico de lances feitos pelo usuário
- [ ] `UserAuctionsWonList` — histórico de leilões que o usuário venceu
- [ ] `LogoutButton`

---

## 🔔 Notificações
### UI (Página ou Modal)
- [ ] `NotificationsPage` ou `NotificationsDrawer`

### Componentes
- [ ] `NotificationList` — lista as notificações
- [ ] `NotificationItem` — item individual da notificação
- [ ] `MarkAsReadButton` / `MarkAllAsReadButton`

---

## 🧭 UI Global e Componentes Gerais
- [ ] `Navbar` — cabeçalho com logo, busca, links de navegação e menu do usuário
- [ ] `Footer` — rodapé
- [ ] `Spinner` / `Loader` — componente de feedback de carregamento
- [ ] `Toast` / `Snackbar` — para feedbacks rápidos (ex: "Lance realizado com sucesso!")
- [ ] `ErrorBoundary` — componente para capturar erros de renderização e evitar que a aplicação quebre

---

## 🧪 Testes
- [ ] Configurar **Jest** com **React Testing Library (RTL)**
- [ ] Criar testes unitários para funções em `utils/` e `hooks/`
- [ ] Criar testes de integração para componentes críticos (`AuthForm`, `BidForm`, `ProductCard`)
- [ ] Considerar testes **End-to-End (E2E)** com **Cypress** ou **Playwright** para os fluxos principais (login, dar lance, etc.)

---

## ♿ Acessibilidade (a11y)
- [ ] Realizar auditoria de contraste de cores
- [ ] Garantir que todos os elementos interativos são acessíveis via teclado
- [ ] Usar HTML semântico (`<main>`, `<nav>`, `<button>`)
- [ ] Adicionar `aria-labels` e outros atributos ARIA quando necessário

---

## ✨ Otimização e Finalização
- [ ] Implementar **Code Splitting (Lazy Loading)** nas rotas com `React.lazy()`
- [ ] Otimizar o carregamento de imagens (compressão, formato `.webp`)
- [ ] Revisar e garantir o **Design Responsivo** (mobile, tablet, desktop)
- [ ] Implementar **Dark Mode** (opcional)
- [ ] Configurar o build de produção e processo de deploy
