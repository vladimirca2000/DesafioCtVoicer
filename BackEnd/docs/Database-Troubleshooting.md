# Troubleshooting - Problemas de Banco de Dados

## Problema Resolvido: Banco de dados n�o era criado em novos ambientes

### Causa Raiz
O Entity Framework estava detectando mudan�as pendentes no modelo que n�o estavam refletidas nas migra��es, causando o erro:
```
The model for context 'ChatBotDbContext' has pending changes. Add a new migration before updating the database.
```

### Solu��o Implementada

1. **Movido seed data para configura��o**: Os dados de seed foram movidos das migra��es para a configura��o da entidade `BotResponseConfiguration.cs`, garantindo que sejam sempre consistentes.

2. **Melhorado Program.cs**: Adicionada l�gica mais robusta para cria��o e migra��o do banco:
   - Verifica se pode conectar ao banco
   - Cria o banco se n�o existir (`EnsureCreatedAsync`)
   - Aplica migra��es pendentes automaticamente
   - Tratamento de erro melhorado com logs detalhados

3. **Scripts de inicializa��o**: Criados scripts (`init-database.bat` e `init-database.sh`) para facilitar a inicializa��o em novos ambientes.

### Como Configurar em um Novo Ambiente

#### Pr�-requisitos
- PostgreSQL instalado e rodando
- .NET 9 SDK instalado
- Usu�rio do PostgreSQL com permiss�es de cria��o de banco

#### Configura��o do appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=SEU_HOST;Database=ChatBotDb;Username=SEU_USUARIO;Password=SUA_SENHA;Include Error Detail=true"
  }
}
```

#### Op��o 1: Inicializa��o Autom�tica
Execute a aplica��o normalmente:
```bash
dotnet run --project src/ChatBot.Api
```
O banco ser� criado automaticamente na primeira execu��o.

#### Op��o 2: Script Manual
Execute o script de inicializa��o:
```bash
# Linux/Mac
./scripts/init-database.sh

# Windows
scripts\init-database.bat
```

#### Op��o 3: Comando Manual
```bash
cd "BackEnd"
dotnet ef database update --project src/ChatBot.Infrastructure --startup-project src/ChatBot.Api
```

### Verifica��o
Ap�s a inicializa��o, verifique se as tabelas foram criadas:
- Users
- ChatSessions
- Messages  
- BotResponses (deve conter dados de seed)

### Logs �teis
A aplica��o agora produz logs detalhados durante a inicializa��o do banco:
- "Attempting to connect to database and apply migrations..."
- "Database created successfully." (se criou um novo banco)
- "Database migrations applied successfully." (se aplicou migra��es)
- "Database is up to date." (se n�o havia mudan�as)

### Problemas Comuns e Solu��es

#### Erro de Conex�o
```
Npgsql.NpgsqlException: Connection refused
```
**Solu��o**: Verifique se o PostgreSQL est� rodando e a string de conex�o est� correta.

#### Erro de Permiss�o
```
Permission denied for database
```
**Solu��o**: Garantir que o usu�rio PostgreSQL tem permiss�es para criar bancos e tabelas.

#### Erro de Migra��o
```
The model for context has pending changes
```
**Solu��o**: Este problema foi resolvido. Se aparecer novamente, execute:
```bash
dotnet ef migrations add NovaMigracao --project src/ChatBot.Infrastructure --startup-project src/ChatBot.Api
dotnet ef database update --project src/ChatBot.Infrastructure --startup-project src/ChatBot.Api
```

### Estrutura das Tabelas Criadas

#### BotResponses (com dados de seed)
- 3 respostas aleat�rias de sauda��o
- 9 respostas baseadas em palavras-chave para diferentes contextos

#### Users, ChatSessions, Messages
- Tabelas relacionais com chaves estrangeiras
- Suporte a soft delete
- Campos de auditoria (CreatedAt, UpdatedAt, etc.)

### Configura��es de Ambiente

#### Development
- Logs sensitivos habilitados
- Detalhes de erro habilitados
- Continua��o mesmo com problemas de banco

#### Production  
- Logs sensitivos desabilitados
- Falha imediata se houver problemas de banco
- Configura��es otimizadas para performance