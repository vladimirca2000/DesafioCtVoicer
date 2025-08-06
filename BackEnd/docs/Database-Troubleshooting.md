# Troubleshooting - Problemas de Banco de Dados

## Problema Resolvido: Banco de dados não era criado em novos ambientes

### Causa Raiz
O Entity Framework estava detectando mudanças pendentes no modelo que não estavam refletidas nas migrações, causando o erro:
```
The model for context 'ChatBotDbContext' has pending changes. Add a new migration before updating the database.
```

### Solução Implementada

1. **Movido seed data para configuração**: Os dados de seed foram movidos das migrações para a configuração da entidade `BotResponseConfiguration.cs`, garantindo que sejam sempre consistentes.

2. **Melhorado Program.cs**: Adicionada lógica mais robusta para criação e migração do banco:
   - Verifica se pode conectar ao banco
   - Cria o banco se não existir (`EnsureCreatedAsync`)
   - Aplica migrações pendentes automaticamente
   - Tratamento de erro melhorado com logs detalhados

3. **Scripts de inicialização**: Criados scripts (`init-database.bat` e `init-database.sh`) para facilitar a inicialização em novos ambientes.

### Como Configurar em um Novo Ambiente

#### Pré-requisitos
- PostgreSQL instalado e rodando
- .NET 9 SDK instalado
- Usuário do PostgreSQL com permissões de criação de banco

#### Configuração do appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=SEU_HOST;Database=ChatBotDb;Username=SEU_USUARIO;Password=SUA_SENHA;Include Error Detail=true"
  }
}
```

#### Opção 1: Inicialização Automática
Execute a aplicação normalmente:
```bash
dotnet run --project src/ChatBot.Api
```
O banco será criado automaticamente na primeira execução.

#### Opção 2: Script Manual
Execute o script de inicialização:
```bash
# Linux/Mac
./scripts/init-database.sh

# Windows
scripts\init-database.bat
```

#### Opção 3: Comando Manual
```bash
cd "BackEnd"
dotnet ef database update --project src/ChatBot.Infrastructure --startup-project src/ChatBot.Api
```

### Verificação
Após a inicialização, verifique se as tabelas foram criadas:
- Users
- ChatSessions
- Messages  
- BotResponses (deve conter dados de seed)

### Logs Úteis
A aplicação agora produz logs detalhados durante a inicialização do banco:
- "Attempting to connect to database and apply migrations..."
- "Database created successfully." (se criou um novo banco)
- "Database migrations applied successfully." (se aplicou migrações)
- "Database is up to date." (se não havia mudanças)

### Problemas Comuns e Soluções

#### Erro de Conexão
```
Npgsql.NpgsqlException: Connection refused
```
**Solução**: Verifique se o PostgreSQL está rodando e a string de conexão está correta.

#### Erro de Permissão
```
Permission denied for database
```
**Solução**: Garantir que o usuário PostgreSQL tem permissões para criar bancos e tabelas.

#### Erro de Migração
```
The model for context has pending changes
```
**Solução**: Este problema foi resolvido. Se aparecer novamente, execute:
```bash
dotnet ef migrations add NovaMigracao --project src/ChatBot.Infrastructure --startup-project src/ChatBot.Api
dotnet ef database update --project src/ChatBot.Infrastructure --startup-project src/ChatBot.Api
```

### Estrutura das Tabelas Criadas

#### BotResponses (com dados de seed)
- 3 respostas aleatórias de saudação
- 9 respostas baseadas em palavras-chave para diferentes contextos

#### Users, ChatSessions, Messages
- Tabelas relacionais com chaves estrangeiras
- Suporte a soft delete
- Campos de auditoria (CreatedAt, UpdatedAt, etc.)

### Configurações de Ambiente

#### Development
- Logs sensitivos habilitados
- Detalhes de erro habilitados
- Continuação mesmo com problemas de banco

#### Production  
- Logs sensitivos desabilitados
- Falha imediata se houver problemas de banco
- Configurações otimizadas para performance