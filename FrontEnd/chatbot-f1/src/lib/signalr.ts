// src/lib/signalr.ts
import * as signalR from '@microsoft/signalr';

const SIGNALR_HUB_URL = process.env.NEXT_PUBLIC_SIGNALR_HUB_URL;

export const createSignalRConnection = () => {
  if (!SIGNALR_HUB_URL) {
    console.error('SIGNALR_HUB_URL não está definida');
    return null;
  }

  const connection = new signalR.HubConnectionBuilder()
    .withUrl(SIGNALR_HUB_URL, {
      // Configurações para lidar com HTTPS e desenvolvimento
      transport: signalR.HttpTransportType.WebSockets,
      // Configurações específicas para desenvolvimento com HTTPS
      ...(process.env.NODE_ENV === 'development' && {
        skipNegotiation: false,
        // Permitir certificados self-signed apenas em desenvolvimento
        httpClient: undefined, // Deixar o padrão do navegador lidar
      }),
    })
    .withAutomaticReconnect([0, 2000, 10000, 30000])
    .configureLogging(signalR.LogLevel.Information)
    .build();

  // Log de eventos de conexão
  connection.onreconnecting((error) => {
    console.log('🔄 SignalR tentando reconectar...', error);
  });

  connection.onreconnected((connectionId) => {
    console.log('✅ SignalR reconectado! ID:', connectionId);
  });

  connection.onclose((error) => {
    console.log('❌ SignalR conexão fechada:', error);
  });

  return connection;
};

export { signalR };
