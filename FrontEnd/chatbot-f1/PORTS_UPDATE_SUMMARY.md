# ✅ Portas Atualizadas - Backend .NET

## 🎯 Mudanças Implementadas

### Antes:
- Backend: `https://localhost:7000`

### ✅ Agora:
- **Backend HTTPS**: `https://localhost:5001` (Padrão)
- **Backend HTTP**: `http://localhost:5000` (Fallback)
- **Frontend**: `http://localhost:3000` (Inalterado)

## 📁 Arquivos Modificados

### 1. `.env.local`
```bash
# Configuração principal (HTTPS)
NEXT_PUBLIC_API_URL="https://localhost:5001/api"
NEXT_PUBLIC_SIGNALR_HUB_URL="https://localhost:5001/chathub"

# Variáveis HTTP disponíveis para fallback
NEXT_PUBLIC_API_URL_HTTP="http://localhost:5000/api"
NEXT_PUBLIC_SIGNALR_HUB_URL_HTTP="http://localhost:5000/chathub"
```

### 2. `next.config.js`
```javascript
// Proxy para HTTPS (padrão)
{
  source: '/api/:path*',
  destination: 'https://localhost:5001/api/:path*',
}
// Proxy para HTTP (fallback)
{
  source: '/api-http/:path*',
  destination: 'http://localhost:5000/api/:path*',
}
```

### 3. `package.json` - Novos Scripts
```json
{
  "switch:https": "pwsh -ExecutionPolicy Bypass -File switch-backend.ps1 https",
  "switch:http": "pwsh -ExecutionPolicy Bypass -File switch-backend.ps1 http"
}
```

## 🚀 Como Usar

### Desenvolvimento Normal (HTTPS - Recomendado):
```bash
npm run dev:open
```

### Alternar para HTTP (se houver problemas SSL):
```bash
npm run switch:http
npm run dev:open
```

### Voltar para HTTPS:
```bash
npm run switch:https
npm run dev:open
```

### Manual PowerShell:
```powershell
# Para HTTPS
./switch-backend.ps1 https

# Para HTTP  
./switch-backend.ps1 http
```

## 🔧 Scripts Criados

1. **`switch-backend.ps1`** - Alterna entre HTTP/HTTPS
2. **`scripts/dev-with-browser.js`** - Atualizado com novas portas
3. **`dev-open.bat`** - Mostra URLs das novas portas

## 🌐 URLs do Projeto

Após executar `npm run dev:open`:

- 🖥️ **Frontend**: http://localhost:3000
- 🔒 **Backend HTTPS**: https://localhost:5001
- 🌐 **Backend HTTP**: http://localhost:5000

## 🔍 Verificar Configuração Atual

```bash
# Ver configuração atual
cat .env.local

# Testar conexão HTTPS
curl https://localhost:5001/api/health

# Testar conexão HTTP
curl http://localhost:5000/api/health
```

## 📋 Compatibilidade Backend .NET

Certifique-se que seu backend .NET está configurado para:

### appsettings.Development.json:
```json
{
  "Urls": "https://localhost:5001;http://localhost:5000",
  "AllowedHosts": "*"
}
```

### Program.cs ou Startup.cs:
```csharp
// CORS configurado para localhost:3000
app.UseCors(policy => policy
    .WithOrigins("http://localhost:3000")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
```

---

**🎉 Configuração Completa!**

Agora o frontend está configurado para usar as portas padrão do .NET:
- HTTPS: 5001 (seguro, padrão)
- HTTP: 5000 (desenvolvimento, fallback)
