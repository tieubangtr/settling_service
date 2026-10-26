import React from "react";
import { Outlet } from "react-router-dom";
import Header from "./Header";
import Footer from "./Footer";
import styles from "./Layout.module.css";

export default function Layout({ accessToken }) {
   return (
      <div className={styles.layoutContainer}>
         {!accessToken && <Header />}
         <main role="main" className={styles.layoutMain}>
            <div className={styles.layoutContent}>
               <Outlet />
            </div>
         </main>
         {!accessToken && <Footer />}
      </div>
   );
}
