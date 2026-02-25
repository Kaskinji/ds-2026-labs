# stop.ps1 (упрощенный)
Write-Host "Остановка компонентов..." -ForegroundColor Yellow

# Остановка Nginx
Write-Host "Остановка Nginx..." -ForegroundColor Yellow
& "..\nginx\nginx.exe" -s stop 2>$null
Start-Sleep -Seconds 1
Get-Process -Name "nginx" -ErrorAction SilentlyContinue | Stop-Process -Force

# Остановка Redis
Write-Host "Остановка Redis..." -ForegroundColor Yellow
docker stop valuator-redis 2>$null

# Остановка приложений
Write-Host "Остановка веб-приложений..." -ForegroundColor Yellow
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host "Все компоненты остановлены!" -ForegroundColor Green