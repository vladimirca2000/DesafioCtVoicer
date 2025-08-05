<!-- components/ChatFloatingButtonSimple.vue -->
<template>
  <div class="fixed bottom-4 right-4 z-50">
    <!-- Chat Button -->
    <button
      v-if="!isOpen"
      @click="handleChatClick"
      class="group relative bg-blue-600 hover:bg-blue-700 text-white p-4 rounded-full shadow-lg hover:shadow-xl transform hover:scale-105 transition-all duration-300 w-16 h-16 flex items-center justify-center focus:outline-none focus:ring-4 focus:ring-blue-300 focus:ring-opacity-50"
      aria-label="Abrir chat"
    >
      <!-- Chat Icon -->
      <img
        src="/images/chat-icon.png"
        alt="Chat"
        class="w-8 h-8"
      />
      
      <!-- Pulse Animation Ring -->
      <div class="absolute inset-0 rounded-full bg-blue-600 opacity-30 animate-ping"></div>
    </button>

    <!-- Chat Modal -->
    <div
      v-if="isOpen"
      class="bg-white border border-gray-200 rounded-lg shadow-2xl w-80 sm:w-96 h-96"
      role="dialog"
    >
      <!-- Chat Header -->
      <div class="flex items-center justify-between p-4 border-b border-gray-200">
        <h3 class="font-bold text-lg text-gray-900">Chat</h3>
        <button
          @click="isOpen = false"
          class="text-gray-400 hover:text-gray-600 transition-colors duration-200"
          aria-label="Fechar chat"
        >
          ✕
        </button>
      </div>

      <!-- Chat Body -->
      <div class="p-4 h-64 overflow-y-auto">
        <div class="text-center text-gray-500">
          <p>Chat em desenvolvimento...</p>
          <p class="text-sm mt-2">API Status: {{ apiStatus }}</p>
        </div>
      </div>

      <!-- Chat Input -->
      <div class="p-4 border-t border-gray-200">
        <input
          type="text"
          placeholder="Digite sua mensagem..."
          class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
const isOpen = ref(false)
const apiStatus = ref('Verificando...')

const handleChatClick = () => {
  isOpen.value = true
  testApi()
}

const testApi = async () => {
  try {
    // Testar o composable useApi
    const api = useApi()
    
    if (api && api.get) {
      apiStatus.value = 'Composable useApi disponível ✅'
      
      // Fazer um teste real (opcional)
      // const result = await api.get('/test')
      // apiStatus.value += ` - Test: ${result.success ? 'OK' : 'FAIL'}`
    } else {
      apiStatus.value = 'API não encontrada ❌'
    }
  } catch (error) {
    apiStatus.value = `Erro: ${error}`
  }
}
</script>
