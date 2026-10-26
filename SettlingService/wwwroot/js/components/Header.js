import React, { useState, useEffect, useRef } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";

export default function Header() {
   const [isMenuOpen, setIsMenuOpen] = useState(false);
   const [isDropdownOpen, setIsDropdownOpen] = useState(false);
   const navigate = useNavigate();
   const location = useLocation();
   const dropdownRef = useRef(null);
   const isAuthenticated = localStorage.getItem("isAuthenticated") === "true";
   const username = localStorage.getItem("username") || "";

   // Đóng dropdown khi click bên ngoài
   useEffect(() => {
      const handleClickOutside = (event) => {
         if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
            setIsDropdownOpen(false);
         }
      };

      if (isDropdownOpen) {
         document.addEventListener("mousedown", handleClickOutside);
      }

      return () => {
         document.removeEventListener("mousedown", handleClickOutside);
      };
   }, [isDropdownOpen]);

   const handleLogout = (e) => {
      e.preventDefault();
      e.stopPropagation();
      localStorage.removeItem("isAuthenticated");
      localStorage.removeItem("username");
      setIsDropdownOpen(false);
      navigate("/login", { replace: true });
   };

   const toggleMenu = () => {
      setIsMenuOpen(!isMenuOpen);
   };

   const toggleDropdown = (e) => {
      e.preventDefault();
      e.stopPropagation();
      setIsDropdownOpen(!isDropdownOpen);
   };

   return (
      <header>
         <nav className="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow">
            <div className="container-fluid">
               <Link className="navbar-brand" to="/dashboard">
                  Settlement Service
               </Link>
               <button className="navbar-toggler" type="button" onClick={toggleMenu} aria-controls="navbarSupportedContent" aria-expanded={isMenuOpen} aria-label="Toggle navigation">
                  <span className="navbar-toggler-icon"></span>
               </button>
               <div className={`navbar-collapse collapse ${isMenuOpen ? "show" : ""} d-sm-inline-flex justify-content-between`}>
                  <ul className="navbar-nav flex-grow-1">
                     {isAuthenticated && (
                        <>
                           <li className="nav-item">
                              <Link className={`nav-link ${location.pathname === "/dashboard" ? "text-primary fw-semibold" : "text-dark"}`} to="/dashboard" onClick={() => setIsMenuOpen(false)}>
                                 Dashboard
                              </Link>
                           </li>
                        </>
                     )}
                  </ul>
                  {isAuthenticated && (
                     <ul className="navbar-nav">
                        <li className={`nav-item dropdown ${isDropdownOpen ? "show" : ""}`} ref={dropdownRef}>
                           <a className="nav-link dropdown-toggle" href="#" role="button" onClick={toggleDropdown} aria-expanded={isDropdownOpen} style={{ cursor: "pointer" }}>
                              {username ? `Xin chào, ${username}` : "User"}
                           </a>
                           <ul className={`dropdown-menu dropdown-menu-end ${isDropdownOpen ? "show" : ""}`}>
                              <li>
                                 <a className="dropdown-item" href="#" onClick={handleLogout} style={{ cursor: "pointer" }}>
                                    Đăng xuất
                                 </a>
                              </li>
                           </ul>
                        </li>
                     </ul>
                  )}
               </div>
            </div>
         </nav>
      </header>
   );
}
