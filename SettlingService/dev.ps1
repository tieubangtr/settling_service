# Script chạy môi trường dev: Webpack watch + Dotnet run
# Cách dùng: .\dev.ps1

$projectPath = $PSScriptRoot

# Bước 1: Cài npm packages nếu chưa có
if (-not (Test-Path (Join-Path $projectPath "node_modules"))) {
    Write-Host "[1/3] Đang cài npm packages..." -ForegroundColor Yellow
    npm install
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Cài đặt thất bại!" -ForegroundColor Red
        exit 1
    }
    Write-Host "✅ Đã cài xong" -ForegroundColor Green
    Write-Host ""
}

# Bước 2: Build React component lần đầu (tạo remoteEntry.js)
Write-Host "[2/3] Đang build React component..." -ForegroundColor Yellow
npm run build:remote:dev
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build thất bại!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Build xong" -ForegroundColor Green
Write-Host ""

# Bước 3: Chạy webpack watch (background) và dotnet (foreground)
Write-Host "[3/3] Đang khởi động servers..." -ForegroundColor Yellow
Write-Host ""

# Chạy webpack watch trong cửa sổ riêng (minimized)
# Webpack watch sẽ TỰ ĐỘNG rebuild khi bạn save file React
Write-Host "Đang khởi động Webpack Watch Mode..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$projectPath'; Write-Host '🔍 Webpack Watch đang theo dõi...' -ForegroundColor Green; Write-Host '   Sẽ tự động rebuild khi bạn save file React' -ForegroundColor Yellow; Write-Host ''; npm run watch:remote" -WindowStyle Minimized
Start-Sleep -Seconds 3

Write-Host "✅ Môi trường dev sẵn sàng!" -ForegroundColor Green
Write-Host ""
Write-Host "📦 Services đang chạy:" -ForegroundColor Cyan
Write-Host "  • Webpack Watch: Tự động rebuild khi save React files" -ForegroundColor White
Write-Host "  • ASP.NET MVC:   http://localhost:6504" -ForegroundColor White
Write-Host ""
Write-Host "💡 Quy trình làm việc:" -ForegroundColor Yellow
Write-Host "   1. Sửa file React trong wwwroot/js/components/" -ForegroundColor White
Write-Host "   2. Save file (Ctrl+S)" -ForegroundColor White
Write-Host "   3. Webpack tự động rebuild (xem cửa sổ minimized)" -ForegroundColor White
Write-Host "   4. Refresh browser (F5) để xem thay đổi" -ForegroundColor White
Write-Host ""

# Chạy dotnet (foreground - có thể dừng bằng Ctrl+C)
dotnet run

