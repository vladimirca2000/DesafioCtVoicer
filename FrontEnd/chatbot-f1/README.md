# ChatBot F1 - Frontend

## Como usar este projeto

### Pré-requisitos
1. **Backend .NET 9** deve estar rodando em `https://localhost:7000`
2. **Node.js** versão 18 ou superior
3. **npm** ou **yarn**

### Configuração Inicial

1. **Instalar dependências**:
```bash
npm install
```

2. **Configurar variáveis de ambiente**:
   - O arquivo `.env.local` já está configurado para:
     - API Backend: `https://localhost:7000/api`
     - SignalR Hub: `https://localhost:7000/chathub`

3. **Iniciar o backend**:
   - Navegue até o diretório do backend (.NET)
   - Execute: `dotnet run` ou use o Visual Studio

### Desenvolvimento

**Iniciar o frontend**:
```bash
npm run dev
```

O aplicativo estará disponível em `http://localhost:3000`

### Estrutura do Projeto

```
src/
├── components/
│   ├── ChatWidget.tsx      # Componente principal do chat
│   ├── ui/                 # Componentes UI (shadcn/ui)
├── lib/
│   ├── api.ts             # Cliente axios configurado
│   └── utils.ts           # Utilitários
├── store/
│   └── store.ts           # Estado global (Redux Toolkit)
└── app/
    ├── layout.tsx         # Layout principal
    ├── page.tsx           # Página inicial
    └── globals.css        # Estilos globais
```

### Funcionalidades

- ✅ **Autenticação por e-mail**
- ✅ **Cadastro de novos usuários**
- ✅ **Chat em tempo real** (SignalR)
- ✅ **Respostas automáticas do bot**
- ✅ **Comando 'sair' para encerrar chat**
- ✅ **Interface responsiva**

### Troubleshooting

1. **Erro de CORS**:
   - Verifique se o backend está configurado para aceitar requisições de `http://localhost:3000`

2. **Erro de SSL/HTTPS**:
   - O `.env.local` está configurado com `NODE_TLS_REJECT_UNAUTHORIZED=0` para desenvolvimento

3. **SignalR não conecta**:
   - Verifique se o hub está disponível em `https://localhost:7000/chathub`
   - Confirme se o backend tem SignalR configurado

4. **API não responde**:
   - Confirme se o backend está rodando em `https://localhost:7000`
   - Verifique os logs do console do navegador

### Endpoints do Backend

- `GET /api/Users/by-email?email={email}` - Buscar usuário por e-mail
- `POST /api/Users` - Criar novo usuário
- `POST /api/Chat/start-session` - Iniciar sessão de chat
- `POST /api/Chat/send-message` - Enviar mensagem
- `POST /api/Chat/end-session` - Encerrar sessão
- `POST /api/Bot/process-message` - Processar mensagem do bot

### Tecnologias Utilizadas

- **Next.js 14** - Framework React
- **TypeScript** - Tipagem estática
- **Tailwind CSS** - Estilização
- **Redux Toolkit** - Gerenciamento de estado
- **Axios** - Cliente HTTP
- **SignalR** - Comunicação em tempo real
- **Shadcn/UI** - Componentes de interface
