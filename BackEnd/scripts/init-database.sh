#!/bin/bash

# Script para inicializar o banco de dados PostgreSQL para o ChatBot
# Execute este script em um novo ambiente para garantir que o banco seja criado corretamente

echo "Inicializando banco de dados ChatBot..."

# Navegar para o diret�rio correto
cd "$(dirname "$0")"

# Verificar se o .NET est� instalado
if ! command -v dotnet &> /dev/null; then
    echo "ERRO: .NET SDK n�o est� instalado."
    exit 1
fi

# Verificar se o projeto pode ser compilado
echo "Verificando se o projeto compila..."
if ! dotnet build; then
    echo "ERRO: Falha na compila��o do projeto."
    exit 1
fi

# Aplicar migra��es
echo "Aplicando migra��es do Entity Framework..."
if ! dotnet ef database update --project src/ChatBot.Infrastructure --startup-project src/ChatBot.Api --verbose; then
    echo "ERRO: Falha ao aplicar migra��es."
    echo "Verifique se:"
    echo "1. O PostgreSQL est� rodando"
    echo "2. A string de conex�o est� correta no appsettings.json"
    echo "3. O usu�rio do banco tem permiss�es adequadas"
    exit 1
fi

echo "? Banco de dados inicializado com sucesso!"
echo ""
echo "Pr�ximos passos:"
echo "1. Execute 'dotnet run --project src/ChatBot.Api' para iniciar a API"
echo "2. Acesse https://localhost:7001/swagger para testar a API"
echo "3. Verifique os logs para confirmar que tudo est� funcionando"