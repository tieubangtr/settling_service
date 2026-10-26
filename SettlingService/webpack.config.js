const path = require("path");
const ModuleFederationPlugin = require("webpack/lib/container/ModuleFederationPlugin");

const PRODUCTION_BASE_URL = "http://your-server:6504";
const PRODUCTION_PUBLIC_PATH = `${PRODUCTION_BASE_URL}/js/dist/`;

// Xác định mode dựa trên NODE_ENV hoặc mặc định là development
const isProduction = process.env.NODE_ENV === "production";
const mode = isProduction ? "production" : "development";

// PublicPath: dynamic cho production, localhost cho dev
// Production: dùng relative path hoặc từ env variable
// Dev: dùng localhost:5032
let publicPath;
if (isProduction) {
   if (process.env.PUBLIC_PATH) {
      // Ưu tiên: dùng từ environment variable (cho CI/CD)
      publicPath = process.env.PUBLIC_PATH;
      console.log("✅ Using PUBLIC_PATH from environment:", publicPath);
   } else if (PRODUCTION_PUBLIC_PATH) {
      // Thứ hai: dùng từ biến cấu hình ở trên
      publicPath = PRODUCTION_PUBLIC_PATH;
      console.log("✅ Using PRODUCTION_PUBLIC_PATH from config:", publicPath);
   } else {
      // Cuối cùng: dùng "auto" - webpack sẽ tự động detect từ window.location
      // LƯU Ý: Nếu dùng "auto", cần tạo file set-public-path.js để set __webpack_public_path__
      publicPath = "auto";
      console.log('⚠️  PUBLIC_PATH not set, using "auto" mode');
      console.log("   Khuyến nghị: Set PRODUCTION_BASE_URL ở trên hoặc dùng environment variable");
   }
} else {
   publicPath = "http://localhost:6504/js/dist/"; // Dev localhost
}
module.exports = {
   mode: mode,
   entry: "./wwwroot/js/components/MvcComponent.js",
   devtool: isProduction ? false : "eval-source-map", // Source maps chỉ cho dev
   output: {
      path: path.resolve(__dirname, "wwwroot/js/dist"),
      filename: isProduction ? "[name].[contenthash].js" : "main.js", // Hash cho production cache (trừ remoteEntry.js)
      publicPath: publicPath,
      clean: true, // Clean trong production, không clean trong dev
   },
   resolve: {
      extensions: [".js", ".jsx"],
   },
   module: {
      rules: [
         {
            test: /\.jsx?$/,
            exclude: /node_modules/,
            use: {
               loader: "babel-loader",
               options: {
                  presets: ["@babel/preset-env", "@babel/preset-react"],
               },
            },
         },
         {
            test: /\.css$/i,
            use: ["style-loader", "css-loader"],
         },
      ],
   },
   plugins: [
      new ModuleFederationPlugin({
         name: "settleApp",
         filename: "remoteEntry.js",
         // Expose container to window.settleApp
         library: { type: "var", name: "settleApp" },
         exposes: {
            "./SettleComponent": "./wwwroot/js/components/MvcComponent.js",
            "./React": "react",
            "./ReactDOM": "react-dom/client",
            // './RemoteComponent': './wwwroot/js/site.js', // Vanilla JS (backup)
         },
         shared: {
            react: {
               singleton: true,
               requiredVersion: false,
               eager: true,
            },
            "react-dom": {
               singleton: true,
               requiredVersion: false,
               eager: true,
            },
            "react-router": {
               singleton: true,
               requiredVersion: "^6.0.0",
               eager: false, // Không cần eager vì có thể lazy load
            },
            "react-router-dom": {
               singleton: true,
               requiredVersion: "^6.0.0",
               eager: false, // Không cần eager vì có thể lazy load
            },
            antd: {
               singleton: true,
               requiredVersion: false,
               eager: false,
            },
         },
      }),
   ],
};
