// plugins/api.client.ts
// Plugin simplificado para evitar problemas de tipos
const createApiPlugin = () => {
  // URL base da API (hardcoded para evitar problemas de config)
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

  const get = <T>(endpoint: string, options: any = {}) => 
    apiCall<T>(endpoint, { method: 'GET', ...options })

  const post = <T>(endpoint: string, body: any, options: any = {}) => 
    apiCall<T>(endpoint, { method: 'POST', body, ...options })

  const put = <T>(endpoint: string, body: any, options: any = {}) => 
    apiCall<T>(endpoint, { method: 'PUT', body, ...options })

  const del = <T>(endpoint: string, options: any = {}) => 
    apiCall<T>(endpoint, { method: 'DELETE', ...options })

  return {
    get,
    post,
    put,
    delete: del,
    apiCall
  }
}

export default createApiPlugin
