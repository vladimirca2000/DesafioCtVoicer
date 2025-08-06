@echo off
REM Script para inicializar o banco de dados PostgreSQL para o ChatBot
REM Execute este script em um novo ambiente para garantir que o banco seja criado corretamente

echo Inicializando banco de dados ChatBot...

REM Navegar para o diret�rio correto
cd /d "%~dp0"
cd ..

REM Verificar se o .NET est� instalado
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERRO: .NET SDK n�o est� instalado.
    pause
    exit /b 1
)

REM Verificar se o projeto pode ser compilado
echo Verificando se o projeto compila...
dotnet build
if errorlevel 1 (
    echo ERRO: Falha na compila��o do projeto.
    pause
    exit /b 1
)

REM Aplicar migra��es
echo Aplicando migra��es do Entity Framework...
dotnet ef database update --project src\ChatBot.Infrastructure --startup-project src\ChatBot.Api --verbose
if errorlevel 1 (
    echo ERRO: Falha ao aplicar migra��es.
    echo Verifique se:
    echo 1. O PostgreSQL est� rodando
    echo 2. A string de conex�o est� correta no appsettings.json
    echo 3. O usu�rio do banco tem permiss�es adequadas
    pause
    exit /b 1
)

echo ? Banco de dados inicializado com sucesso!
echo.
echo Pr�ximos passos:
echo 1. Execute 'dotnet run --project src\ChatBot.Api' para iniciar a API
echo 2. Acesse https://localhost:7001/swagger para testar a API
echo 3. Verifique os logs para confirmar que tudo est� funcionando
pause