#!/bin/bash

# Script para inicializar o banco de dados PostgreSQL para o ChatBot
# Execute este script em um novo ambiente para garantir que o banco seja criado corretamente

echo "Inicializando banco de dados ChatBot..."

# Navegar para o diretório correto
cd "$(dirname "$0")"

# Verificar se o .NET está instalado
if ! command -v dotnet &> /dev/null; then
    echo "ERRO: .NET SDK não está instalado."
    exit 1
fi

# Verificar se o projeto pode ser compilado
echo "Verificando se o projeto compila..."
if ! dotnet build; then
    echo "ERRO: Falha na compilação do projeto."
    exit 1
fi

# Aplicar migrações
echo "Aplicando migrações do Entity Framework..."
if ! dotnet ef database update --project src/ChatBot.Infrastructure --startup-project src/ChatBot.Api --verbose; then
    echo "ERRO: Falha ao aplicar migrações."
    echo "Verifique se:"
    echo "1. O PostgreSQL está rodando"
    echo "2. A string de conexão está correta no appsettings.json"
    echo "3. O usuário do banco tem permissões adequadas"
    exit 1
fi

echo "? Banco de dados inicializado com sucesso!"
echo ""
echo "Próximos passos:"
echo "1. Execute 'dotnet run --project src/ChatBot.Api' para iniciar a API"
echo "2. Acesse https://localhost:7001/swagger para testar a API"
echo "3. Verifique os logs para confirmar que tudo está funcionando"