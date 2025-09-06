if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "Docker não está instalado. Por favor, instale o Docker Desktop." -ForegroundColor Red
    exit 1
}

Write-Host "Iniciando a aplicação..."
docker-compose up --build -d

Write-Host "Aguardando a API iniciar..."
Start-Sleep -Seconds 5
Start-Process "http://localhost:5000/swagger"