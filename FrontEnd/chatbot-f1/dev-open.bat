@echo off
echo 🚀 Iniciando servidor Next.js e abrindo navegador...
echo.
echo 🔗 URLs do projeto:
echo   Frontend: http://localhost:3000
echo   Backend HTTPS: https://localhost:5001
echo   Backend HTTP: http://localhost:5000
echo.

:: Abrir navegador em background
start "" http://localhost:3000

:: Aguardar um pouco para o servidor inicializar
timeout /t 3 /nobreak > nul

:: Iniciar servidor Next.js
npm run dev
