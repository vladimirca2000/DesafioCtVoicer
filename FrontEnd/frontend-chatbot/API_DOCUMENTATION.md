# Documentação da API - Frontend ChatBot

Esta documentação descreve os endpoints da API utilizados pelo frontend para integração com o backend .NET.

## Base URL

```
https://localhost:5001/api
```

## Endpoints Utilizados

### 1. Verificação de Usuário por E-mail

**Endpoint:** `GET /users/by-email`

**Descrição:** Verifica se um usuário existe e está ativo pelo e-mail.

**Parâmetros de Query:**
- `email` (string, obrigatório): E-mail do usuário

**Exemplo de Request:**
```
GET /api/users/by-email?email=usuario@exemplo.com
```

**Resposta de Sucesso (200):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "email": "usuario@exemplo.com",
    "name": "Nome do Usuário",
    "isActive": true,
    "createdAt": "2025-01-01T10:00:00Z"
  },
  "message": "Usuário encontrado com sucesso"
}
```

**Resposta de Erro (404):**
```json
{
  "success": false,
  "message": "Usuário não encontrado",
  "errors": ["E-mail não está cadastrado no sistema"]
}
```

---

### 2. Cadastro de Usuário

**Endpoint:** `POST /users`

**Descrição:** Cadastra um novo usuário no sistema.

**Body:**
```json
{
  "email": "usuario@exemplo.com",
  "name": "Nome do Usuário"
}
```

**Exemplo de Request:**
```
POST /api/users
Content-Type: application/json

{
  "email": "novo.usuario@exemplo.com",
  "name": "Novo Usuário"
}
```

**Resposta de Sucesso (201):**
```json
{
  "success": true,
  "data": {
    "id": 2,
    "email": "novo.usuario@exemplo.com",
    "name": "Novo Usuário",
    "isActive": true,
    "createdAt": "2025-01-01T10:05:00Z"
  },
  "message": "Usuário cadastrado com sucesso"
}
```

**Resposta de Erro (400):**
```json
{
  "success": false,
  "message": "Dados inválidos",
  "errors": [
    "E-mail é obrigatório",
    "Nome deve ter pelo menos 2 caracteres"
  ]
}
```

---

### 3. Iniciar Sessão de Chat

**Endpoint:** `POST /chat/start`

**Descrição:** Inicia uma nova sessão de chat para o usuário.

**Body:**
```json
{
  "userId": 1
}
```

**Exemplo de Request:**
```
POST /api/chat/start
Content-Type: application/json

{
  "userId": 1
}
```

**Resposta de Sucesso (201):**
```json
{
  "success": true,
  "data": {
    "id": 123,
    "userId": 1,
    "status": "Active",
    "startedAt": "2025-01-01T10:10:00Z"
  },
  "message": "Sessão de chat iniciada com sucesso"
}
```

---

### 4. Enviar Mensagem

**Endpoint:** `POST /chat/send`

**Descrição:** Envia uma mensagem na sessão de chat.

**Body:**
```json
{
  "sessionId": 123,
  "userId": 1,
  "content": "Olá, como você está?",
  "messageType": "user"
}
```

**Exemplo de Request:**
```
POST /api/chat/send
Content-Type: application/json

{
  "sessionId": 123,
  "userId": 1,
  "content": "Olá, como você está?",
  "messageType": "user"
}
```

**Resposta de Sucesso (200):**
```json
{
  "success": true,
  "data": {
    "id": 456,
    "sessionId": 123,
    "userId": 1,
    "content": "Olá, como você está?",
    "messageType": "user",
    "timestamp": "2025-01-01T10:15:00Z"
  },
  "message": "Mensagem enviada com sucesso"
}
```

---

### 5. Processar Mensagem do Bot

**Endpoint:** `POST /bot/process`

**Descrição:** Processa uma mensagem do usuário e gera resposta do bot.

**Body:**
```json
{
  "userId": 1,
  "sessionId": 123,
  "userMessage": "Olá, como você está?"
}
```

**Exemplo de Request:**
```
POST /api/bot/process
Content-Type: application/json

{
  "userId": 1,
  "sessionId": 123,
  "userMessage": "Olá, como você está?"
}
```

**Resposta de Sucesso (200):**
```json
{
  "success": true,
  "data": {
    "response": "Olá! Estou bem, obrigado por perguntar. Como posso ajudá-lo hoje?",
    "messageId": 457,
    "botResponseType": "Greeting",
    "timestamp": "2025-01-01T10:15:05Z"
  },
  "message": "Resposta do bot gerada com sucesso"
}
```

---

### 6. Encerrar Sessão de Chat

**Endpoint:** `POST /chat/end`

**Descrição:** Encerra uma sessão de chat ativa.

**Body:**
```json
{
  "sessionId": 123
}
```

**Exemplo de Request:**
```
POST /api/chat/end
Content-Type: application/json

{
  "sessionId": 123
}
```

**Resposta de Sucesso (200):**
```json
{
  "success": true,
  "data": {
    "sessionId": 123,
    "status": "Ended",
    "endedAt": "2025-01-01T10:30:00Z",
    "duration": "00:20:00"
  },
  "message": "Sessão encerrada com sucesso"
}
```

---

### 7. Obter Histórico de Chat

**Endpoint:** `GET /chat/history/{sessionId}`

**Descrição:** Obtém o histórico de mensagens de uma sessão.

**Parâmetros de Rota:**
- `sessionId` (integer, obrigatório): ID da sessão

**Exemplo de Request:**
```
GET /api/chat/history/123
```

**Resposta de Sucesso (200):**
```json
{
  "success": true,
  "data": [
    {
      "id": 456,
      "sessionId": 123,
      "userId": 1,
      "content": "Olá, como você está?",
      "messageType": "user",
      "timestamp": "2025-01-01T10:15:00Z"
    },
    {
      "id": 457,
      "sessionId": 123,
      "content": "Olá! Estou bem, obrigado por perguntar. Como posso ajudá-lo hoje?",
      "messageType": "bot",
      "timestamp": "2025-01-01T10:15:05Z"
    }
  ],
  "message": "Histórico recuperado com sucesso"
}
```

---

## Tipos de Dados Utilizados

### Usuário
```typescript
interface User {
  id: number
  email: string
  name: string
  isActive: boolean
  createdAt: string
}
```

### Sessão de Chat
```typescript
interface ChatSession {
  id: number
  userId: number
  status: 'Active' | 'Ended'
  startedAt: string
  endedAt?: string
}
```

### Mensagem
```typescript
interface Message {
  id: number
  sessionId: number
  userId?: number
  content: string
  messageType: 'user' | 'bot'
  timestamp: string
}
```

### Resposta da API
```typescript
interface ApiResponse<T = any> {
  success: boolean
  data?: T
  message?: string
  errors?: string[]
}
```

---

## Códigos de Status HTTP

- **200**: Operação realizada com sucesso
- **201**: Recurso criado com sucesso
- **400**: Requisição inválida (dados malformados)
- **404**: Recurso não encontrado
- **500**: Erro interno do servidor

---

## Tratamento de Erros

Todos os endpoints seguem o mesmo padrão de resposta de erro:

```json
{
  "success": false,
  "message": "Descrição do erro",
  "errors": [
    "Lista detalhada de erros específicos"
  ]
}
```

---

## Autenticação

Atualmente, o sistema não implementa autenticação baseada em tokens. A identificação do usuário é feita através do `userId` nas requisições.

> **Nota**: Em um ambiente de produção, seria recomendável implementar autenticação JWT ou similar.

---

## Rate Limiting

O backend pode implementar rate limiting para evitar spam. Em caso de muitas requisições:

**Resposta (429):**
```json
{
  "success": false,
  "message": "Muitas requisições. Tente novamente em alguns segundos.",
  "errors": ["Rate limit exceeded"]
}
```

---

## Configuração CORS

O backend deve estar configurado para aceitar requisições do frontend:

```csharp
// No backend .NET
app.UseCors(policy => 
  policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
```

---

## Comando "Sair"

Quando o usuário digita "sair", o frontend:

1. Envia a mensagem normalmente via `/chat/send`
2. O backend reconhece o comando e responde adequadamente
3. O frontend automaticamente chama `/chat/end` para encerrar a sessão
4. A interface é fechada após 2 segundos

---

Esta documentação deve ser mantida atualizada conforme a evolução da API do backend.
