import { settle } from "../env";

// apiClient.js
const defaultHeaders = {
   "Content-Type": "application/json",
};

async function request(endpoint, { method = "GET", body, token } = {}) {
   const api = settle.prod;

   const headers = { ...defaultHeaders };
   if (token) headers["Authorization"] = `Bearer ${token}`;

   const options = {
      method,
      headers,
      body: body ? JSON.stringify(body) : undefined,
   };

   const res = await fetch(api + endpoint, options);
   if (!res.ok) {
      const text = await res.text();
      throw new Error(`HTTP ${res.status}: ${text}`);
   }
   return res.json();
}

// wrappers
export const apiClient = {
   get: (endpoint, token) => request(endpoint, { method: "GET", token }),
   post: (endpoint, body, token) => request(endpoint, { method: "POST", body, token }),
   put: (endpoint, body, token) => request(endpoint, { method: "PUT", body, token }),
   delete: (endpoint, token) => request(endpoint, { method: "DELETE", token }),
};
