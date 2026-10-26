// Bootstrap file để load và render React component từ Module Federation
// File này sẽ được load sau remoteEntry.js

(async function () {
   const containerId = "settle-app-container";
   const containerElement = document.getElementById(containerId);

   if (!containerElement) {
      console.warn(`Container element #${containerId} not found`);
      return;
   }

   try {
      // Đợi settleApp được expose từ remoteEntry.js
      // Module Federation expose container với tên "settleApp"
      let settleApp = window.settleApp;

      // Nếu chưa có trên window, thử đợi một chút (script có thể đang load)
      if (!settleApp) {
         console.log("Đang đợi settleApp container...");
         let retries = 10;
         while (!settleApp && retries > 0) {
            await new Promise((resolve) => setTimeout(resolve, 100));
            settleApp = window.settleApp;
            retries--;
         }
      }

      if (!settleApp) {
         throw new Error("settleApp chưa được load. Đảm bảo remoteEntry.js đã được load trước và expose container với tên 'settleApp'.");
      }

      console.log("✅ settleApp container đã sẵn sàng");

      // Load React và ReactDOM từ Module Federation exposed modules
      // React và ReactDOM đã được expose trong webpack config
      const [ReactModule, ReactDOMModule] = await Promise.all([settleApp.get("./React"), settleApp.get("./ReactDOM")]);

      const React = ReactModule();
      const ReactDOM = ReactDOMModule();

      if (!React || !ReactDOM) {
         throw new Error("Không thể load React hoặc ReactDOM từ Module Federation");
      }

      // Load React component từ Module Federation - sửa tên module từ RemoteComponent sang SettleComponent
      const moduleFactory = await settleApp.get("./SettleComponent");
      const RemoteComponent = moduleFactory();

      // Render component
      // React và ReactDOM từ Module Federation
      const ReactLib = React.default || React;
      const ReactDOMLib = ReactDOM.default || ReactDOM;

      // createElement và createRoot
      const createElement = ReactLib.createElement;
      const createRoot = ReactDOMLib.createRoot;

      if (!createElement) {
         throw new Error("React.createElement không tồn tại");
      }
      if (!createRoot) {
         throw new Error("ReactDOM.createRoot không tồn tại. Kiểm tra React version >= 18");
      }

      const root = createRoot(containerElement);
      const Component = RemoteComponent.default || RemoteComponent;
      root.render(createElement(Component));
   } catch (error) {
      console.error("❌ Error loading React component:", error);
      console.error("Error details:", error.stack);
      containerElement.innerHTML =
         '<div class="alert alert-warning">' +
         "<strong>⚠️ Không thể tải React component</strong><br>" +
         "<small>Vui lòng chạy: <code>npm run watch:remote</code> trong thư mục SettlingService/SettlingService</small><br>" +
         "<small>Lỗi: " +
         error.message +
         "</small><br>" +
         "<small>Kiểm tra Console để xem chi tiết lỗi</small>" +
         "</div>";
   }
})();
