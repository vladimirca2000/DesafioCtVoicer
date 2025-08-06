const https = require('https');
const axios = require('axios');

process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const API_URL = 'https://localhost:7000/api';
const SIGNALR_URL = 'https://localhost:7000/chathub';

async function checkBackend() {
  console.log('🔍 Verificando conectividade com o backend...\n');

  try {
    console.log('📡 Testando API...');
    const response = await axios.get(`${API_URL}/Users/by-email?email=test@test.com`, {
      timeout: 5000,
      httpsAgent: new https.Agent({
        rejectUnauthorized: false
      })
    });
    console.log('✅ API está respondendo!');
  } catch (error) {
    if (error.response && error.response.status === 404) {
      console.log('✅ API está respondendo (usuário não encontrado é esperado)');
    } else if (error.code === 'ECONNREFUSED') {
      console.log('❌ Backend não está rodando em https://localhost:7000');
      console.log('   Certifique-se de que o backend .NET está ativo');
    } else {
      console.log('⚠️  Erro na API:', error.message);
    }
  }

  try {
    console.log('\n🔗 Testando SignalR Hub...');
    const response = await axios.get(SIGNALR_URL, {
      timeout: 5000,
      httpsAgent: new https.Agent({
        rejectUnauthorized: false
      })
    });
    console.log('✅ SignalR Hub está acessível!');
  } catch (error) {
    if (error.response && error.response.status === 405) {
      console.log('✅ SignalR Hub está acessível (método GET não permitido é esperado)');
    } else if (error.code === 'ECONNREFUSED') {
      console.log('❌ SignalR Hub não está acessível');
    } else {
      console.log('⚠️  Erro no SignalR Hub:', error.message);
    }
  }

  console.log('\n📋 URLs configuradas:');
  console.log(`   API: ${API_URL}`);
  console.log(`   SignalR: ${SIGNALR_URL}`);
  
  console.log('\n💡 Para iniciar o backend:');
  console.log('   1. Navegue até o diretório do backend');
  console.log('   2. Execute: dotnet run');
  console.log('   3. Ou abra no Visual Studio e pressione F5');
}

checkBackend().catch(console.error);
