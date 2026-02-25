# stop.ps1
Write-Host "Остановка компонентов..." -ForegroundColor Yellow

# Остановка Nginx
Write-Host "Остановка Nginx..." -ForegroundColor Yellow
& "..\DISTRIBUTED-PROGRAMMING\nginx\nginx.exe" -s stop

# Остановка процессов dotnet
Write-Host "Остановка веб-приложений..." -ForegroundColor Yellow
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host "Все компоненты остановлены!" -ForegroundColor Green