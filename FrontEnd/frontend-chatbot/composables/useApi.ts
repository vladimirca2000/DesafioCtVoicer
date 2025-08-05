// composables/useApi.ts
// Composable simplificado sem imports problemáticos

interface ApiResponse<T = any> {
  success: boolean
  data?: T
  message?: string
  errors?: string[]
}

export const useApi = () => {
  const baseURL = 'https://localhost:5001/api'

  const apiCall = async <T>(
    endpoint: string, 
    options: any = {}
  ) => {
    try {
      const response = await $fetch<any>(`${baseURL}${endpoint}`, {
        ...options,
        headers: {
          'Content-Type': 'application/json',
          ...options.headers,
        },
      })
      return {
        success: true,
        data: response,
        message: 'Sucesso'
      }
    } catch (error: any) {
      console.error('API Error:', error)
      return {
        success: false,
        message: error.message || 'Erro na comunicação com o servidor',
        errors: [error.message || 'Erro desconhecido']
      }
    }
  }

  return {
    get: <T>(endpoint: string, options: any = {}) => 
      apiCall<T>(endpoint, { method: 'GET', ...options }),

    post: <T>(endpoint: string, body: any, options: any = {}) => 
      apiCall<T>(endpoint, { method: 'POST', body, ...options }),

    put: <T>(endpoint: string, body: any, options: any = {}) => 
      apiCall<T>(endpoint, { method: 'PUT', body, ...options }),

    delete: <T>(endpoint: string, options: any = {}) => 
      apiCall<T>(endpoint, { method: 'DELETE', ...options }),

    apiCall
  }
}
