# Configuração de Ambiente - Backend .NET

## 🔧 Portas Configuradas

### ✅ Configuração Atual:
- **Frontend**: `http://localhost:3000` (Next.js)
- **Backend HTTPS**: `https://localhost:5001` (Produção/Desenvolvimento Seguro)
- **Backend HTTP**: `http://localhost:5000` (Desenvolvimento/Fallback)

## 🚀 Como Usar

### Opção 1: HTTPS (Recomendado - Padrão)
```bash
# As variáveis padrão já estão configuradas para HTTPS
npm run dev
# ou
npm run dev:open
```

### Opção 2: HTTP (Fallback para desenvolvimento)
Se houver problemas com certificados SSL, você pode criar um arquivo `.env.local.http`:

```bash
# Crie uma cópia para usar HTTP temporariamente
cp .env.local .env.local.backup

# Edite .env.local para usar HTTP:
NEXT_PUBLIC_API_URL="http://localhost:5000/api"
NEXT_PUBLIC_SIGNALR_HUB_URL="http://localhost:5000/chathub"
```

## 🔄 Scripts para Alternar

### Script PowerShell para alternar entre HTTP/HTTPS:

```powershell
# Salvar como switch-backend.ps1
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("http", "https")]
    [string]$Protocol
)

if ($Protocol -eq "https") {
    $apiUrl = "https://localhost:5001/api"
    $signalrUrl = "https://localhost:5001/chathub"
    Write-Host "🔒 Configurando para HTTPS (Seguro)" -ForegroundColor Green
} else {
    $apiUrl = "http://localhost:5000/api"
    $signalrUrl = "http://localhost:5000/chathub"
    Write-Host "🌐 Configurando para HTTP (Desenvolvimento)" -ForegroundColor Yellow
}

# Atualizar .env.local
$envContent = @"
# Database
DATABASE_URL="your-database-url"

# API Keys - Backend .NET
NEXT_PUBLIC_API_URL="$apiUrl"
NEXT_PUBLIC_SIGNALR_HUB_URL="$signalrUrl"
API_SECRET_KEY="your-secret-key"

# Auth
NEXTAUTH_SECRET="your-nextauth-secret"
NEXTAUTH_URL="http://localhost:3000"

# Configurações para desenvolvimento
NODE_ENV="development"
"@

$envContent | Out-File -FilePath ".env.local" -Encoding UTF8
Write-Host "✅ Configuração atualizada!" -ForegroundColor Green
Write-Host "🔗 API URL: $apiUrl" -ForegroundColor Cyan
Write-Host "🔗 SignalR URL: $signalrUrl" -ForegroundColor Cyan
```

## 📋 Comandos Úteis

```bash
# Verificar se o backend está rodando
curl https://localhost:5001/api/health
# ou
curl http://localhost:5000/api/health

# Testar SignalR
# (Abra o DevTools e verifique se conecta)

# Verificar configuração atual
cat .env.local
```

## 🔍 Troubleshooting

### Problema: Certificado SSL
**Solução**: Use HTTP temporariamente
```bash
# Execute o script:
./switch-backend.ps1 http
npm run dev:open
```

### Problema: Backend não encontrado
**Verificar se o backend .NET está rodando nas portas corretas:**
```bash
# Verificar processo nas portas
netstat -an | findstr :5001
netstat -an | findstr :5000
```

### Problema: CORS
**O Next.js já está configurado com proxy, mas se houver problemas:**
```bash
# Verificar next.config.js
# As configurações de CORS já estão incluídas
```

---

**🎯 Configuração Completa!**

O projeto agora está configurado para usar:
- Backend HTTPS: `https://localhost:5001` (padrão)
- Backend HTTP: `http://localhost:5000` (fallback)
- Frontend: `http://localhost:3000`
