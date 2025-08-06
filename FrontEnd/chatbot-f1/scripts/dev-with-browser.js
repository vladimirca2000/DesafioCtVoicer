#!/usr/bin/env node

const { spawn, exec } = require('child_process');
const http = require('http');

console.log('🚀 Iniciando servidor Next.js...');

function checkServer(callback) {
  const req = http.get('http://localhost:3000', (res) => {
    callback(true);
  });
  
  req.on('error', () => {
    callback(false);
  });
  
  req.setTimeout(1000, () => {
    req.destroy();
    callback(false);
  });
}

function openBrowser() {
  const platform = process.platform;
  let command;
  
  if (platform === 'win32') {
    command = 'start http://localhost:3000';
  } else if (platform === 'darwin') {
    command = 'open http://localhost:3000';
  } else {
    command = 'xdg-open http://localhost:3000';
  }
  
  exec(command, (error) => {
    if (error) {
      console.log('❌ Erro ao abrir navegador:', error.message);
      console.log('🌐 Abra manualmente: http://localhost:3000');
      console.log('🔗 Backend HTTPS: https://localhost:5001');
      console.log('🔗 Backend HTTP: http://localhost:5000');
    } else {
      console.log('✅ Navegador aberto!');
      console.log('🔗 Frontend: http://localhost:3000');
      console.log('🔗 Backend HTTPS: https://localhost:5001');
      console.log('🔗 Backend HTTP: http://localhost:5000');
    }
  });
}

const nextProcess = spawn('npm', ['run', 'dev'], {
  stdio: 'inherit',
  shell: true
});

let attempts = 0;
const maxAttempts = 30;

const checkInterval = setInterval(() => {
  attempts++;
  
  checkServer((isReady) => {
    if (isReady) {
      clearInterval(checkInterval);
      console.log('🌐 Servidor pronto! Abrindo navegador...');
      setTimeout(() => {
        openBrowser();
      }, 1000);
    } else if (attempts >= maxAttempts) {
      clearInterval(checkInterval);
      console.log('⏰ Timeout: Abrindo navegador mesmo assim...');
      openBrowser();
    }
  });
}, 1000);

process.on('SIGINT', () => {
  console.log('\n🛑 Encerrando servidor...');
  nextProcess.kill();
  process.exit();
});

process.on('SIGTERM', () => {
  nextProcess.kill();
});

nextProcess.on('exit', (code) => {
  process.exit(code);
});
