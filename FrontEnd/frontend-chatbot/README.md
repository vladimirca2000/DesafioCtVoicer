# Frontend ChatBot - Vue 3/Nuxt 3

Sistema de chat com bot integrado ao backend .NET, desenvolvido com Vue 3, Nuxt 3 e TypeScript conforme especificação técnica.

## 🚀 Funcionalidades

- ✅ **Portfólio responsivo** com tema Formula 1
- ✅ **Chat em tempo real** com SignalR
- ✅ **Integração com backend .NET**
- ✅ **Google Maps** na página de contato
- ✅ **TypeScript** com tipagem completa
- ✅ **Pinia** para gerenciamento de estado
- ✅ **Tailwind CSS** com tema customizado
- ✅ **Componentes reutilizáveis**

## 🛠️ Tecnologias

- **Framework**: Nuxt.js 4
- **Language**: TypeScript
- **Styling**: Tailwind CSS + Nuxt UI
- **State Management**: Pinia
- **Real-time**: SignalR
- **API**: .NET Backend
- **Icons**: Nuxt Icon
- **Images**: Nuxt Image

## 📋 Pré-requisitos

- Node.js 18+ 
- npm ou yarn
- Backend .NET rodando (para chat)

## 🚀 Instalação

```bash
# Clonar o repositório
git clone <url-do-repositorio>
cd frontend-chatbot

# Instalar dependências
npm install

# Copiar arquivo de ambiente
cp .env.example .env

# Configurar variáveis no .env:
# - NUXT_PUBLIC_API_BASE_URL (URL do backend)
# - NUXT_PUBLIC_SIGNALR_HUB_URL (URL do SignalR Hub)
# - NUXT_PUBLIC_GOOGLE_MAPS_API_KEY (Chave do Google Maps)
```

## 🔧 Desenvolvimento

```bash
# Iniciar servidor de desenvolvimento
npm run dev

# Build para produção
npm run build

# Preview da build de produção
npm run preview

# Geração estática
npm run generate
```

## 📁 Estrutura do Projeto

```
├── components/          # Componentes Vue reutilizáveis
├── composables/         # Lógica reutilizável
├── layouts/            # Layouts da aplicação
├── pages/              # Páginas/rotas
├── plugins/            # Plugins do Nuxt
├── stores/             # Stores Pinia
├── types/              # Definições TypeScript
├── utils/              # Utilitários e helpers
├── assets/css/         # Estilos globais
└── public/             # Arquivos estáticos
```

## 🎨 Personalização

### Cores da Formula 1
```typescript
// tailwind.config.js
colors: {
  'f1-red': '#E10600',
  'f1-dark': '#15151E', 
  'f1-silver': '#C0C0C0',
}
```

### Configurações do Chat
```typescript
// utils/constants.ts
CHAT_CONFIG: {
  maxMessageLength: 500,
  typingDelay: 1000,
  connectionTimeout: 30000
}
```

## 🔌 Integração com Backend

O projeto está configurado para integrar com um backend .NET:

- **API REST**: Comunicação HTTP padrão
- **SignalR**: Chat em tempo real
- **Tipagem TypeScript**: Interfaces para todas as APIs

## 📱 Responsividade

- Mobile First Design
- Breakpoints Tailwind
- Componentes adaptativos
- Touch-friendly interface

## 🧪 Testes

```bash
# Executar testes (quando configurados)
npm run test

# Coverage de testes
npm run test:coverage
```

## 📦 Deploy

### Netlify/Vercel
```bash
npm run build
```

### Docker
```dockerfile
# Criar Dockerfile conforme necessário
```

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/nova-funcionalidade`)
3. Commit suas mudanças (`git commit -am 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/nova-funcionalidade`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para detalhes.

## 👨‍💻 Autor

**Vladimir Carlos Alves**
- LinkedIn: [vladimir-carlos-alves](https://linkedin.com/in/vladimir-carlos-alves)
- GitHub: [vladimir-carlos-alves](https://github.com/vladimir-carlos-alves)
- Email: vladimir@example.com

---

⚡ Desenvolvido com paixão pela Formula 1! 🏎️

## Development Server

Start the development server on `http://localhost:3000`:

```bash
# npm
npm run dev

# pnpm
pnpm dev

# yarn
yarn dev

# bun
bun run dev
```

## Production

Build the application for production:

```bash
# npm
npm run build

# pnpm
pnpm build

# yarn
yarn build

# bun
bun run build
```

Locally preview production build:

```bash
# npm
npm run preview

# pnpm
pnpm preview

# yarn
yarn preview

# bun
bun run preview
```

Check out the [deployment documentation](https://nuxt.com/docs/getting-started/deployment) for more information.
