// src/lib/api.ts
import axios from 'axios';

const API_URL = process.env.NEXT_PUBLIC_API_URL;

// Configuração do cliente axios
const apiClient = axios.create({
  baseURL: API_URL,
  timeout: 10000, // 10 segundos
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  },
  // Configuração específica para desenvolvimento com HTTPS localhost
  ...(process.env.NODE_ENV === 'development' && {
    httpsAgent: typeof window === 'undefined' ? require('https').Agent({
      rejectUnauthorized: false // Apenas em desenvolvimento e server-side
    }) : undefined,
  }),
});

// Interceptador de requisição
apiClient.interceptors.request.use(
  (config) => {
    // Adicionar logs para debug
    console.log(`🚀 API Request: ${config.method?.toUpperCase()} ${config.url}`);
    return config;
  },
  (error) => {
    console.error('❌ Request Error:', error);
    return Promise.reject(error);
  }
);

// Interceptador de resposta
apiClient.interceptors.response.use(
  (response) => {
    console.log(`✅ API Response: ${response.status} ${response.config.url}`);
    return response;
  },
  (error) => {
    console.error('❌ Response Error:', {
      status: error.response?.status,
      statusText: error.response?.statusText,
      url: error.config?.url,
      data: error.response?.data,
    });
    
    // Tratar erros específicos do backend .NET
    if (error.response?.data) {
      const { title, errors, detail } = error.response.data;
      
      if (errors) {
        // ValidationProblemDetails - múltiplos erros de validação
        const errorMessages = Object.values(errors).flat();
        error.userMessage = errorMessages.join(', ');
      } else if (title) {
        // ProblemDetails - erro único
        error.userMessage = title;
      } else if (detail) {
        error.userMessage = detail;
      }
    }
    
    return Promise.reject(error);
  }
);

export default apiClient;
export { API_URL };
