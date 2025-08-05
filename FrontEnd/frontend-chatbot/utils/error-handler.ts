// plugins/error-handler.client.ts
export default defineNuxtPlugin(() => {
  // Handler global simplificado para erros
  if (process.client) {
    window.addEventListener('unhandledrejection', (event) => {
      console.error('Unhandled promise rejection:', event.reason)
      event.preventDefault()
    })

    window.addEventListener('error', (event) => {
      console.error('JavaScript error:', event.error)
    })
  }
})
