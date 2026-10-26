import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import styles from "./Login.module.css";

export default function Login() {
   const [username, setUsername] = useState("");
   const [password, setPassword] = useState("");
   const [error, setError] = useState("");
   const [loading, setLoading] = useState(false);
   const navigate = useNavigate();

   // Redirect nếu đã đăng nhập
   useEffect(() => {
      if (localStorage.getItem("isAuthenticated") === "true") {
         navigate("/dashboard", { replace: true });
      }
   }, [navigate]);

   const handleSubmit = async (e) => {
      e.preventDefault();
      setError("");
      setLoading(true);

      // TODO: Thay bằng API call thực tế
      // Simulate login
      setTimeout(() => {
         if (username && password) {
            // Lưu token/session (có thể dùng localStorage hoặc cookie)
            localStorage.setItem("isAuthenticated", "true");
            localStorage.setItem("username", username);
            navigate("/dashboard");
         } else {
            setError("Vui lòng nhập đầy đủ thông tin");
         }
         setLoading(false);
      }, 500);
   };

   return (
      <div className={styles.loginContainer}>
         <div className={styles.loginCard}>
            <div className={styles.loginHeader}>
               <h2>Đăng nhập</h2>
               <p>PostTrade Service</p>
            </div>

            <form onSubmit={handleSubmit} className={styles.loginForm}>
               {error && <div className={`${styles.alert} ${styles.alertDanger}`}>{error}</div>}

               <div className={styles.formGroup}>
                  <label htmlFor="username">Tên đăng nhập</label>
                  <input type="text" id="username" className={styles.formControl} value={username} onChange={(e) => setUsername(e.target.value)} placeholder="Nhập tên đăng nhập" required autoFocus />
               </div>

               <div className={styles.formGroup}>
                  <label htmlFor="password">Mật khẩu</label>
                  <input type="password" id="password" className={styles.formControl} value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Nhập mật khẩu" required />
               </div>

               <button type="submit" className={`${styles.btnPrimary} ${styles.btnBlock}`} disabled={loading}>
                  {loading ? "Đang đăng nhập..." : "Đăng nhập"}
               </button>
            </form>

            <div className={styles.loginFooter}>
               <small className={styles.textMuted}>Phiên bản 1.0.0</small>
            </div>
         </div>
      </div>
   );
}
