# start.ps1 (исправленная версия)
Write-Host "Запуск веб-приложений..." -ForegroundColor Green

$projectPath = "..\Valuator\Valuator.csproj" 

# ==== УМНЫЙ ЗАПУСК REDIS ====
Write-Host "Проверка Redis..." -ForegroundColor Yellow

# Проверяем, запущен ли уже контейнер
$runningContainer = docker ps -q -f name=valuator-redis

if ($runningContainer) {
    Write-Host "Redis контейнер уже запущен" -ForegroundColor Green
} else {
    # Проверяем, существует ли контейнер (но остановлен)
    $existingContainer = docker ps -a -q -f name=valuator-redis
    
    if ($existingContainer) {
        Write-Host "Redis контейнер существует, но остановлен. Запускаю..." -ForegroundColor Yellow
        docker start valuator-redis
    } else {
        # Контейнера нет - создаем новый
        Write-Host "Создание нового Redis контейнера..." -ForegroundColor Yellow
        docker-compose up -d
    }
}

# Небольшая пауза для инициализации Redis
Start-Sleep -Seconds 2
# ============================

# Запуск приложений
Write-Host "Запуск приложений..." -ForegroundColor Green
Start-Process -WindowStyle Hidden -FilePath "dotnet" -ArgumentList "run --project `"$projectPath`" --urls http://0.0.0.0:5001"
Start-Process -WindowStyle Hidden -FilePath "dotnet" -ArgumentList "run --project `"$projectPath`" --urls http://0.0.0.0:5002"
Start-Process -WindowStyle Hidden -FilePath "dotnet" -ArgumentList "run --project `"$projectPath`" --urls http://0.0.0.0:5003"
Start-Process -WindowStyle Hidden -FilePath "dotnet" -ArgumentList "run --project `"$projectPath`" --urls http://0.0.0.0:5004"

Write-Host "Ожидание запуска приложений..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Запуск Nginx
Write-Host "Запуск Nginx..." -ForegroundColor Green
Push-Location "..\nginx"
Start-Process -WindowStyle Hidden -FilePath ".\nginx.exe"
Pop-Location

Write-Host "`nСистема запущена!" -ForegroundColor Green
Write-Host "- Redis: localhost:6379" -ForegroundColor Cyan
Write-Host "- Приложения: http://localhost:5001, http://localhost:5002, http://localhost:5003, http://localhost:5004" -ForegroundColor Cyan
Write-Host "- Прокси: http://localhost:8080" -ForegroundColor Cyan

# Проверка Redis
try {
    $redisPing = docker exec valuator-redis redis-cli ping 2>$null
    if ($redisPing -eq "PONG") {
        Write-Host "✓ Redis: OK" -ForegroundColor Green
    }
} catch {
    Write-Host " Redis: проверка не удалась" -ForegroundColor Yellow
}