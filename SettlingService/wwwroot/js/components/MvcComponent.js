import React from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Layout from "./Layout";
import Login from "./login/Login";
import ProtectedRoute from "./ProtectedRoute";
import SettlementPage from "./page/Settlement";

function RootRedirect({ to }) {
   // Dùng Navigate component thay vì useNavigate để tránh vòng lặp
   return <Navigate to={to} replace />;
}

function getDashboardPath(isEmbedded) {
   return isEmbedded ? "/settle-app/dashboard" : "/dashboard";
}

function AppRoutes({ isRemote, accessToken }) {
   if (accessToken) {
      return (
         // <BrowserRouter>
         <Routes>
            {/* <Route path="/login" element={<Login />} /> */}

            <Route element={<Layout accessToken={accessToken} />}>
               <Route
                  path="dashboard"
                  element={
                     <ProtectedRoute accessToken={accessToken}>
                        <SettlementPage accessToken={accessToken} />
                     </ProtectedRoute>
                  }
               />
            </Route>

            {/* Redirect root to dashboard */}
            <Route index element={<RootRedirect to={getDashboardPath(true)} />} />
            <Route path="*" element={<RootRedirect to={getDashboardPath(true)} />} />
         </Routes>
         // </BrowserRouter>
      );
   } else {
      return (
         <BrowserRouter>
            <Routes>
               <Route path="/login" element={<Login />} />
               <Route element={<Layout />}>
                  <Route
                     path="/dashboard"
                     element={
                        <ProtectedRoute>
                           <SettlementPage />
                        </ProtectedRoute>
                     }
                  />
               </Route>
               {/* Root redirect */}
               <Route path="/" element={<RootRedirect to="/dashboard" />} />
               <Route path="*" element={<RootRedirect to="/dashboard" />} />
            </Routes>
         </BrowserRouter>
      );
   }
}

export default function MvcComponent({ isRemote, accessToken }) {
   return <AppRoutes isRemote={isRemote} accessToken={accessToken} />;
}
