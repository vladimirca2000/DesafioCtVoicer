#!/usr/bin/env pwsh
# Script para iniciar o servidor Next.js e abrir o navegador automaticamente

Write-Host "🚀 Iniciando servidor de desenvolvimento..." -ForegroundColor Green

# Iniciar o servidor Next.js em background
Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    npm run dev 
} -Name "NextDevServer"

Write-Host "⏳ Aguardando servidor inicializar..." -ForegroundColor Yellow

# Aguardar o servidor estar pronto (verificar se a porta 3000 está respondendo)
$timeout = 30 # segundos
$elapsed = 0
$serverReady = $false

while ($elapsed -lt $timeout -and -not $serverReady) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:3000" -TimeoutSec 2 -ErrorAction Stop
        if ($response.StatusCode -eq 200) {
            $serverReady = $true
        }
    }
    catch {
        Start-Sleep -Seconds 1
        $elapsed++
    }
}

if ($serverReady) {
    Write-Host "✅ Servidor pronto! Abrindo navegador..." -ForegroundColor Green
    Start-Process "http://localhost:3000"
    
    Write-Host "🌐 Aplicação disponível em: http://localhost:3000" -ForegroundColor Cyan
    Write-Host "❌ Para parar o servidor, pressione Ctrl+C" -ForegroundColor Yellow
    
    # Manter o script rodando e mostrar logs do servidor
    Receive-Job -Name "NextDevServer" -Wait
} else {
    Write-Host "❌ Timeout: Servidor não respondeu em $timeout segundos" -ForegroundColor Red
    Stop-Job -Name "NextDevServer"
    Remove-Job -Name "NextDevServer"
    exit 1
}

# Limpeza ao finalizar
Stop-Job -Name "NextDevServer" -ErrorAction SilentlyContinue
Remove-Job -Name "NextDevServer" -ErrorAction SilentlyContinue
