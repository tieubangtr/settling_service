import React from "react";
import { Link } from "react-router-dom";

export default function Footer() {
   const currentYear = new Date().getFullYear();

   return (
      <footer className="border-top footer text-muted mt-auto">
         <div className="container">
            &copy; {currentYear} - PTPService - <Link to="/privacy">Privacy</Link>
         </div>
      </footer>
   );
}

