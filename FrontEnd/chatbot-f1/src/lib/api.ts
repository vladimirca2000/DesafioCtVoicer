import axios from 'axios';

const API_URL = process.env.NEXT_PUBLIC_API_URL;

const apiClient = axios.create({
  baseURL: API_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  },
  ...(process.env.NODE_ENV === 'development' && {
    httpsAgent: typeof window === 'undefined' ? require('https').Agent({
      rejectUnauthorized: false
    }) : undefined,
  }),
});

apiClient.interceptors.request.use(
  (config) => {
    if (config.method?.toUpperCase() !== 'OPTIONS') {
      const timestamp = new Date().toISOString();
      console.log(`🚀 [${timestamp}] API Request: ${config.method?.toUpperCase()} ${config.url}`);
      console.log(`🚀 [${timestamp}] Request data:`, config.data);
    }
    return config;
  },
  (error) => {
    console.error('❌ Request Error:', error);
    return Promise.reject(error);
  }
);

apiClient.interceptors.response.use(
  (response) => {
    if (!(response.config.method?.toUpperCase() === 'OPTIONS' && response.status === 204)) {
      const timestamp = new Date().toISOString();
      console.log(`✅ [${timestamp}] API Response: ${response.status} ${response.config.url}`);
      console.log(`✅ [${timestamp}] Response data:`, response.data);
      
      if (response.config.method?.toUpperCase() === 'POST' && response.status === 201) {
        console.log(`🎯 [${timestamp}] *** CRIAÇÃO BEM-SUCEDIDA (201) ***`);
      }
    }
    return response;
  },
  (error) => {
    console.error('❌ Response Error:', {
      status: error.response?.status,
      statusText: error.response?.statusText,
      url: error.config?.url,
      data: error.response?.data,
    });
    
    if (error.response?.data) {
      const { title, errors, detail } = error.response.data;
      
      if (errors) {
        const errorMessages = Object.values(errors).flat();
        error.userMessage = errorMessages.join(', ');
      } else if (title) {
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
