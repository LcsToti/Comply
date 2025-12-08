import Login from "@/pages/auth/Login";
import Register from "@/pages/auth/Register";
import RegisterSuccess from "@/pages/auth/RegisterSuccess";
import MyListings from "@/pages/dashboard/MyListings";
import MyWatchList from "@/pages/dashboard/MyWatchList";
import Profile from "@/pages/dashboard/Profile";
import CreateProduct from "@/pages/product/CreateProduct";
import EditProduct from "@/pages/product/EditProduct";
import ProductDetails from "@/pages/product/ProductDetails";
import Search from "@/pages/search/Search";
import { createBrowserRouter } from "react-router";
import ProtectedRouter from "./ProtectedRouter";
import HomePage from "@/pages/home/HomePage";
import Payment from "@/pages/checkout/Payment";
import CreateAuction from "@/pages/product/CreateAuction";
import CreateSellerAccount from "@/pages/auth/CreateSellerAccount";
import Faq from "@/pages/home/Faq";
import AdminDashboard from "@/pages/admin/AdminDashboard";
import TicketsPage from "@/pages/admin/TicketsPage";
import AdminLayout from "@/layouts/AdminsLayout";
import DisputesPage from "@/pages/admin/DisputesPage";

export const router = createBrowserRouter([
  { path: "/", Component: HomePage },

  {
    path: "admin",
    Component: AdminLayout,
    children: [
      { path: "", Component: AdminDashboard },
      { path: "tickets", Component: TicketsPage },
      { path: "disputes", Component: DisputesPage },
    ],
  },

  { path: "faq", Component: Faq },

  { path: "login", Component: Login },
  {
    path: "register",
    Component: Register,
  },
  {
    path: "register",
    Component: ProtectedRouter,
    children: [
      { path: "success", Component: RegisterSuccess },
      { path: "seller", Component: CreateSellerAccount },
    ],
  },
  {
    path: "product",
    children: [
      { path: ":id", Component: ProductDetails },
      {
        Component: ProtectedRouter,
        children: [
          { path: "create", Component: CreateProduct },
          { path: ":id/auction", Component: CreateAuction },
          { path: ":id/edit", Component: EditProduct },
          { path: ":id/checkout", Component: Payment },
        ],
      },
    ],
  },
  { path: "search", Component: Search },
  {
    path: "dashboard",
    children: [
      { path: "MyListings", Component: MyListings },
      { path: "MyWatchList", Component: MyWatchList },
      {
        Component: ProtectedRouter,
        children: [{ path: "profile", Component: Profile }],
      },
    ],
  },
]);
