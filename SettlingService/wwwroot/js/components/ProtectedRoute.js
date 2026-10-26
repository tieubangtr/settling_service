import React from "react";
import { Navigate, useLocation } from "react-router-dom";

export default function ProtectedRoute({ children, accessToken }) {
   const isAuthenticated = localStorage.getItem("isAuthenticated") === "true";
   const location = useLocation();

   const getLoginPath = () => {
      if (location.pathname.includes("/settle-app")) {
         return "/settle-app/login";
      }
      return "/login";
   };
   if (!accessToken) {
      if (!isAuthenticated) {
         return <Navigate to={getLoginPath()} replace />;
      }
   }

   return children;
}
