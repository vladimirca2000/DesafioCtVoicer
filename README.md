# 🤖 ChatBot Solution - Sistema de Chat Inteligente

<div align="center">

![ChatBot](https://img.shields.io/badge/ChatBot-v1.0.0-blue)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![Next.js](https://img.shields.io/badge/Next.js-15.0-black)
![TypeScript](https://img.shields.io/badge/TypeScript-5.0-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15.0-blue)

**Sistema moderno de chat em tempo real com bot inteligente**

[Demo](#-executando-o-projeto) • [Documentação](#-documentação) • [Arquitetura](#-arquitetura) • [Contribuição](#-contribuição)

</div>

---

## 📋 Sobre o Projeto

O **ChatBot Solution** é um sistema completo de chat em tempo real que simula interações com um bot de atendimento inteligente. O projeto demonstra práticas modernas de desenvolvimento, incluindo arquitetura limpa, CQRS, comunicação em tempo real e design responsivo.

### ✨ Funcionalidades Principais

- 💬 **Chat em Tempo Real** - Comunicação instantânea via SignalR/WebSockets
- 🤖 **Bot Inteligente** - Respostas automáticas com diferentes estratégias
- 👥 **Múltiplos Usuários** - Suporte a sessões simultâneas
- 📱 **Design Responsivo** - Interface adaptável para desktop e mobile
- 🔄 **Arquitetura Robusta** - Clean Architecture com CQRS e Domain Events
- ⚡ **Alta Performance** - Otimizado para baixa latência

### 🛠️ Stack Tecnológica

#### Back-End
- **Framework**: ASP.NET Core 9.0
- **Banco de Dados**: PostgreSQL
- **ORM**: Entity Framework Core
- **Comunicação Real-Time**: SignalR
- **Padrões**: CQRS, Mediator, Repository, Strategy, Factory

#### Front-End
- **Framework**: Next.js 15.0
- **Linguagem**: TypeScript
- **Estilização**: Tailwind CSS
- **Componentes**: Radix UI + Shadcn/ui
- **Estado**: Zustand
- **Comunicação**: SignalR Client

---

## 🚀 Executando o Projeto

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [PostgreSQL 15+](https://www.postgresql.org/)
- [Git](https://git-scm.com/)

### 1️⃣ Clonando o Repositório

```bash
git clone https://github.com/vladimirca2000/DesafioCtVoicer.git
cd DesafioCtVoicer
```

### 2️⃣ Configurando o Back-End

```bash
# Navegar para o diretório do back-end
cd BackEnd

# Configurar string de conexão no appsettings.json
# Edite: src/ChatBot.Api/appsettings.json

# Executar migrations
dotnet ef database update --project src/ChatBot.Infrastructure --startup-project src/ChatBot.Api

# Executar o back-end
cd src/ChatBot.Api
dotnet run
```

O back-end estará disponível em:
- API: `https://localhost:5001`
- Swagger: `https://localhost:5001/swagger`

### 3️⃣ Configurando o Front-End

```bash
# Em um novo terminal, navegar para o front-end
cd FrontEnd/chatbot-f1

# Instalar dependências
npm install

# Executar em modo desenvolvimento
npm run dev
```

O front-end estará disponível em: `http://localhost:3000`

### 4️⃣ Populando o Banco de Dados

Para que o bot funcione corretamente, é necessário popular a tabela `BotResponses` com respostas pré-configuradas:

#### Executando os Inserts no PostgreSQL

1. **Localize o arquivo SQL**: [📄 INSERT_Table_public.BotResponses.sql](./INSERT_Table_public.BotResponses.sql)

2. **Execute via psql** (linha de comando):
```bash
# Conecte ao banco PostgreSQL
psql -h localhost -p 5432 -U seu_usuario -d chatbot_db

# Execute o arquivo SQL
\i INSERT_Table_public.BotResponses.sql
```

3. **Execute via pgAdmin** (interface gráfica):
   - Conecte ao servidor PostgreSQL
   - Abra o Query Tool
   - Copie e cole o conteúdo do arquivo SQL
   - Execute (F5)

4. **Execute via DBeaver ou outro cliente**:
   - Abra uma nova conexão SQL
   - Importe e execute o arquivo `INSERT_Table_public.BotResponses.sql`

#### Conteúdo das Respostas

O arquivo contém respostas categorizadas para:
- 🙋 **Saudações** - "olá", "oi", "bom dia"
- ❓ **Perguntas frequentes** - "horário", "preço", "como funciona"
- 🆘 **Suporte** - "ajuda", "problema", "erro"
- 👋 **Despedidas** - "tchau", "obrigado", "até logo"

> **💡 Dica**: Após executar os inserts, reinicie a aplicação para garantir que o cache seja atualizado.

---

## 📚 Documentação

### 📖 Documentação Completa

| Componente | README | Documentação | Arquitetura |
|------------|--------|--------------|-------------|
| **Back-End** | - | [📄 BACKEND_DOCUMENTATION.md](./BackEnd/BACKEND_DOCUMENTATION.md) | [🏗️ Arquitetura.md](./BackEnd/Arquitetura.md) |
| **Front-End** | [📄 README.md](./FrontEnd/chatbot-f1/README.md) | [📄 FRONTEND_DOCUMENTATION.md](./FrontEnd/chatbot-f1/FRONTEND_DOCUMENTATION.md) | [🏗️ ARQUITETURA.md](./FrontEnd/chatbot-f1/ARQUITETURA.md) |

### 🎯 Guias Rápidos

- **[Como Executar](./docs/QuickStart.md)** - Guia rápido de instalação
- **[API Reference](./BackEnd/src/ChatBot.Api/README.md)** - Documentação da API
- **[Desenvolvimento](./docs/Development.md)** - Guia para desenvolvedores

---

## 🏗️ Arquitetura

### Visão Geral da Solução

```text
+-------------------+    +--------------------+    +-------------------+
|   Front-End       |    |     Back-End       |    |   Banco de        |
|   (Next.js)       |<-->|   (ASP.NET)        |<-->|   Dados           |
|                   |    |                    |    |   (PostgreSQL)    |
| • TypeScript      |    | • Clean Arch       |    |                   |
| • Tailwind CSS    |    | • CQRS + MediatR   |    | • Entity          |
| • SignalR         |    | • SignalR          |    |   Framework       |
| • Zustand         |    | • Domain Events    |    | • Migrations      |
+-------------------+    +--------------------+    +-------------------+
```

### Arquitetura Detalhada do Back-End

```text
+---------------------------------------------------------------------+
|                     ChatBot.Api (Presentation)                     |
|     Controllers • Hubs SignalR • Middleware • Filters             |
+----------------------------------+----------------------------------+
                                   |
+----------------------------------v----------------------------------+
|                ChatBot.Application (Business Logic)                |
|     CQRS Handlers • MediatR • Behaviors • Validators              |
+----------------------------------+----------------------------------+
                                   |
+----------------------------------v----------------------------------+
|                 ChatBot.Domain (Core Business)                     |
|   Entities • Value Objects • Domain Events • Business Rules       |
+----------------------------------+----------------------------------+
                                   |
+----------------------------------v----------------------------------+
|              ChatBot.Infrastructure (Data Access)                  |
|   Repositories • EF Core • External Services • Database           |
+---------------------------------------------------------------------+
```

### Principais Padrões Implementados

- **🏛️ Clean Architecture** - Separação clara de responsabilidades
- **📨 CQRS** - Segregação de comandos e consultas  
- **🎭 Mediator** - Desacoplamento via MediatR
- **📦 Repository** - Abstração do acesso a dados
- **🎯 Strategy** - Diferentes estratégias de resposta do bot
- **🏭 Factory** - Criação dinâmica de estratégias
- **📢 Observer** - Eventos de domínio para notificações

---

## 🌟 Funcionalidades Detalhadas

### Back-End Features

- ✅ **API RESTful** com Swagger
- ✅ **SignalR Hub** para comunicação real-time
- ✅ **CQRS** com MediatR
- ✅ **Domain Events** para notificações
- ✅ **Validation Pipeline** com FluentValidation
- ✅ **Logging** estruturado com Serilog
- ✅ **Exception Handling** centralizado
- ✅ **Unit of Work** para transações
- ✅ **Soft Delete** e Auditoria automática

### Front-End Features

- ✅ **Interface Responsiva** mobile-first
- ✅ **Componentes Reutilizáveis** com Radix UI
- ✅ **Estado Global** com Zustand
- ✅ **TypeScript** para type safety
- ✅ **SignalR Integration** para real-time
- ✅ **Loading States** e Error Handling
- ✅ **Animações Suaves** com Framer Motion
- ✅ **Dark/Light Mode** (configurável)

---

## 🧪 Testes

### Back-End Testing

```bash
cd BackEnd

# Testes unitários
dotnet test tests/ChatBot.UnitTests

# Testes de integração
dotnet test tests/ChatBot.IntegrationTests

# Testes de arquitetura
dotnet test tests/ChatBot.ArchitectureTests

# Coverage report
dotnet test --collect:"XPlat Code Coverage"
```

### Front-End Testing

```bash
cd FrontEnd/chatbot-f1

# Testes unitários e de componentes
npm run test

# Testes E2E
npm run test:e2e

# Coverage
npm run test:coverage
```

---

## 📦 Estrutura do Projeto

```
DesafioCtVoicer/
├── 📁 BackEnd/                    # API e Serviços
│   ├── 📁 src/
│   │   ├── 📁 ChatBot.Api/        # Controllers e Hub SignalR
│   │   ├── 📁 ChatBot.Application/# CQRS, Handlers, Features
│   │   ├── 📁 ChatBot.Domain/     # Entidades e Regras de Negócio
│   │   ├── 📁 ChatBot.Infrastructure/ # Repositórios e EF Core
│   │   └── 📁 ChatBot.Shared/     # DTOs e Contratos
│   ├── 📁 tests/                  # Testes Automatizados
│   └── 📄 BACKEND_DOCUMENTATION.md
├── 📁 FrontEnd/
│   └── 📁 chatbot-f1/             # Aplicação Next.js
│       ├── 📁 src/
│       │   ├── 📁 app/            # App Router (Next.js 13+)
│       │   ├── 📁 components/     # Componentes React
│       │   ├── 📁 hooks/          # Custom Hooks
│       │   ├── 📁 lib/            # Utilities e Configurações
│       │   └── 📁 store/          # Estado Global (Zustand)
│       └── 📄 FRONTEND_DOCUMENTATION.md
├── 📄 README.md                   # Este arquivo
└── 📄 INSERT_INTO_public.BotResponses.txt
```

---

## 🤝 Contribuição

### Como Contribuir

1. **Fork** o projeto
2. **Clone** seu fork
3. **Crie** uma branch para sua feature (`git checkout -b feature/nova-feature`)
4. **Commit** suas mudanças (`git commit -m 'Adiciona nova feature'`)
5. **Push** para a branch (`git push origin feature/nova-feature`)
6. **Abra** um Pull Request

### Padrões de Código

- **Back-End**: Siga as convenções C# e use EditorConfig
- **Front-End**: Use ESLint + Prettier para formatação
- **Commits**: Use [Conventional Commits](https://www.conventionalcommits.org/)
- **Testes**: Mantenha cobertura acima de 80%

---

## 📄 Licença

Este projeto está sob a licença [MIT](./LICENSE). Veja o arquivo `LICENSE` para mais detalhes.

---

## 👤 Autor

### Vladimir CA

- GitHub: [@vladimirca2000](https://github.com/vladimirca2000)
- LinkedIn: [Vladimir CA](https://linkedin.com/in/vladimirca2000)


<div align="center">

### ⭐ Se este projeto foi útil para você, deixe uma estrela! ⭐

[🔝 Voltar ao topo](#-chatbot-solution---sistema-de-chat-inteligente)

</div>
