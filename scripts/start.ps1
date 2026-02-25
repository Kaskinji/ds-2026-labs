# start.ps1
Write-Host "Запуск веб-приложений..." -ForegroundColor Green

# Запуск экземпляров приложения
$projectPath = "..\Valuator\Valuator.csproj" 
#относ пути

Start-Process -WindowStyle Hidden -FilePath "dotnet" -ArgumentList "run --project `"$projectPath`" --urls http://0.0.0.0:5001"
Start-Process -WindowStyle Hidden -FilePath "dotnet" -ArgumentList "run --project `"$projectPath`" --urls http://0.0.0.0:5002"
Start-Process -WindowStyle Hidden -FilePath "dotnet" -ArgumentList "run --project `"$projectPath`" --urls http://0.0.0.0:5003"
Start-Process -WindowStyle Hidden -FilePath "dotnet" -ArgumentList "run --project `"$projectPath`" --urls http://0.0.0.0:5004"

Write-Host "Ожидание запуска приложений..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

Write-Host "Запуск Nginx..."
Start-Process -WindowStyle Hidden -FilePath "C:\Users\MSI\source\repos\DISTRIBUTED-PROGRAMMING\nginx\nginx.exe"

Write-Host "Система запущена!"
Write-Host "- Приложения: http://localhost:5001, http://localhost:5002"
Write-Host "- Прокси: http://localhost:8080"