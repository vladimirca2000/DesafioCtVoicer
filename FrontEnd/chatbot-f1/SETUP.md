# 🚀 Guia de Configuração e Execução - ChatBot F1

## ✅ Configurações Realizadas

### 1. **Configuração de API**
- ✅ URLs atualizadas para `https://localhost:7000`
- ✅ Cliente axios configurado com interceptadores
- ✅ Tratamento de erros do backend .NET
- ✅ Configuração SSL para desenvolvimento

### 2. **Configuração SignalR**
- ✅ Cliente SignalR otimizado
- ✅ Reconexão automática
- ✅ Logs de debug

### 3. **Estrutura de Dados**
- ✅ Compatibilidade com backend .NET
- ✅ Tratamento de `Result<T>` pattern
- ✅ Mapeamento correto de UserIds

## 🛠️ Como Executar

### Passo 1: Verificar Backend
```bash
npm run check-backend
```

### Passo 2: Iniciar Frontend
```bash
npm run dev
```

### Passo 3: Testar
1. Abra `http://localhost:3000`
2. Clique no ícone de chat (canto inferior direito)
3. Digite um e-mail para teste
4. Se o e-mail não existir, será solicitado o nome para cadastro

## 📋 Checklist de Verificação

### Backend (.NET)
- [ ] Backend rodando em `https://localhost:7000`
- [ ] CORS configurado para `http://localhost:3000`
- [ ] SignalR hub disponível em `/chathub`
- [ ] Endpoints funcionais:
  - [ ] `GET /api/Users/by-email`
  - [ ] `POST /api/Users`
  - [ ] `POST /api/Chat/start-session`
  - [ ] `POST /api/Chat/send-message`
  - [ ] `POST /api/Bot/process-message`

### Frontend (Next.js)
- [ ] Dependências instaladas (`npm install`)
- [ ] Variáveis de ambiente configuradas (`.env.local`)
- [ ] Aplicação rodando em `http://localhost:3000`

## 🔧 Troubleshooting

### Problema: "CORS Error"
**Solução**: Verificar se o backend .NET tem CORS configurado para `http://localhost:3000`

### Problema: "SignalR não conecta"
**Solução**: 
1. Verificar se o hub está em `https://localhost:7000/chathub`
2. Verificar logs do console para erros específicos

### Problema: "API não responde"
**Solução**:
1. Executar `npm run check-backend`
2. Verificar se o backend está rodando
3. Confirmar URLs no `.env.local`

### Problema: "SSL Certificate Error"
**Solução**: 
- Já configurado `NODE_TLS_REJECT_UNAUTHORIZED=0` no `.env.local`
- Se persistir, verificar configuração HTTPS do backend

## 📂 Arquivos Modificados

- ✅ `.env.local` - URLs atualizadas
- ✅ `src/components/ChatWidget.tsx` - Integração com backend
- ✅ `src/lib/api.ts` - Cliente axios configurado
- ✅ `src/lib/signalr.ts` - Cliente SignalR otimizado
- ✅ `next.config.js` - Configurações CORS e HTTPS
- ✅ `package.json` - Script de verificação
- ✅ `README.md` - Documentação atualizada

## 🎯 Funcionalidades Implementadas

1. **Autenticação por E-mail**
   - Verificação se usuário existe
   - Cadastro automático se não existir

2. **Chat em Tempo Real**
   - SignalR para comunicação bidirecional
   - Reconexão automática

3. **Bot Inteligente**
   - Respostas automáticas
   - Comando "sair" para encerrar

4. **Interface Responsiva**
   - Design moderno com Tailwind CSS
   - Componentes shadcn/ui

## 🚀 Próximos Passos

1. Iniciar o backend .NET
2. Executar `npm run check-backend` para verificar conectividade
3. Executar `npm run dev` para iniciar o frontend
4. Testar o fluxo completo do chat

---

**💡 Dica**: Use as ferramentas de desenvolvedor do navegador (F12) para monitorar requisições e logs do SignalR.
