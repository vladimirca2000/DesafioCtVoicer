#!/usr/bin/env pwsh
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("http", "https")]
    [string]$Protocol
)

Write-Host "🔧 Alterando configuração do backend..." -ForegroundColor Cyan

if ($Protocol -eq "https") {
    $apiUrl = "https://localhost:5001/api"
    $signalrUrl = "https://localhost:5001/chathub"
    Write-Host "🔒 Configurando para HTTPS (Seguro)" -ForegroundColor Green
} else {
    $apiUrl = "http://localhost:5000/api"
    $signalrUrl = "http://localhost:5000/chathub"
    Write-Host "🌐 Configurando para HTTP (Desenvolvimento)" -ForegroundColor Yellow
}

# Backup do arquivo atual
if (Test-Path ".env.local") {
    Copy-Item ".env.local" ".env.local.backup" -Force
    Write-Host "📁 Backup criado: .env.local.backup" -ForegroundColor Gray
}

# Criar novo conteúdo do .env.local
$envContent = @"
# Database
DATABASE_URL="your-database-url"

# API Keys - Backend .NET ($Protocol.ToUpper())
NEXT_PUBLIC_API_URL="$apiUrl"
NEXT_PUBLIC_SIGNALR_HUB_URL="$signalrUrl"
API_SECRET_KEY="your-secret-key"

# Auth
NEXTAUTH_SECRET="your-nextauth-secret"
NEXTAUTH_URL="http://localhost:3000"

# Configurações para desenvolvimento
NODE_ENV="development"
"@

# Escrever arquivo
$envContent | Out-File -FilePath ".env.local" -Encoding UTF8

Write-Host "✅ Configuração atualizada com sucesso!" -ForegroundColor Green
Write-Host ""
Write-Host "🔗 URLs configuradas:" -ForegroundColor Cyan
Write-Host "   Frontend: http://localhost:3000" -ForegroundColor White
Write-Host "   API: $apiUrl" -ForegroundColor White
Write-Host "   SignalR: $signalrUrl" -ForegroundColor White
Write-Host ""
Write-Host "🚀 Para aplicar as mudanças, reinicie o servidor:" -ForegroundColor Yellow
Write-Host "   npm run dev:open" -ForegroundColor White
