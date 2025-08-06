@echo off
REM Script para inicializar o banco de dados PostgreSQL para o ChatBot
REM Execute este script em um novo ambiente para garantir que o banco seja criado corretamente

echo Inicializando banco de dados ChatBot...

REM Navegar para o diretório correto
cd /d "%~dp0"
cd ..

REM Verificar se o .NET está instalado
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERRO: .NET SDK não está instalado.
    pause
    exit /b 1
)

REM Verificar se o projeto pode ser compilado
echo Verificando se o projeto compila...
dotnet build
if errorlevel 1 (
    echo ERRO: Falha na compilação do projeto.
    pause
    exit /b 1
)

REM Aplicar migrações
echo Aplicando migrações do Entity Framework...
dotnet ef database update --project src\ChatBot.Infrastructure --startup-project src\ChatBot.Api --verbose
if errorlevel 1 (
    echo ERRO: Falha ao aplicar migrações.
    echo Verifique se:
    echo 1. O PostgreSQL está rodando
    echo 2. A string de conexão está correta no appsettings.json
    echo 3. O usuário do banco tem permissões adequadas
    pause
    exit /b 1
)

echo ? Banco de dados inicializado com sucesso!
echo.
echo Próximos passos:
echo 1. Execute 'dotnet run --project src\ChatBot.Api' para iniciar a API
echo 2. Acesse https://localhost:7001/swagger para testar a API
echo 3. Verifique os logs para confirmar que tudo está funcionando
pause