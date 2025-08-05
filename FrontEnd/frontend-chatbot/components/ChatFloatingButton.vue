<!-- components/ChatFloatingButton.vue -->
<template>
  <div class="fixed bottom-4 right-4 z-50">
    <!-- Chat Button -->
    <Transition
      enter-active-class="transform transition duration-300 ease-out"
      enter-from-class="scale-0 rotate-180 opacity-0"
      enter-to-class="scale-100 rotate-0 opacity-100"
      leave-active-class="transform transition duration-200 ease-in"
      leave-from-class="scale-100 rotate-0 opacity-100"
      leave-to-class="scale-0 rotate-180 opacity-0"
    >
      <button
        v-if="!isOpen"
        @click="handleChatButtonClick"
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
    </Transition>

    <!-- Email Modal -->
    <Transition
      enter-active-class="transform transition duration-300 ease-out"
      enter-from-class="translate-y-full opacity-0"
      enter-to-class="translate-y-0 opacity-100"
      leave-active-class="transform transition duration-200 ease-in"
      leave-from-class="translate-y-0 opacity-100"
      leave-to-class="translate-y-full opacity-0"
    >
      <div
        v-if="showEmailModal"
        class="bg-white border border-gray-200 rounded-lg shadow-2xl w-80 sm:w-96 p-6"
        role="dialog"
        aria-labelledby="email-modal-title"
      >
        <!-- Email Modal Header -->
        <div class="flex items-center justify-between mb-4">
          <h3 id="email-modal-title" class="font-bold text-lg text-gray-900">
            {{ needsName ? 'Informações para o Chat' : 'Acesso ao Chat' }}
          </h3>
          <button
            @click="closeEmailModal"
            class="text-gray-400 hover:text-gray-600 transition-colors duration-200"
            aria-label="Fechar modal"
          >
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd"></path>
            </svg>
          </button>
        </div>

        <!-- Email Form -->
        <form @submit.prevent="handleEmailSubmit" class="space-y-4">
          <div>
            <label for="user-email" class="block text-sm font-medium text-gray-700 mb-2">
              E-mail
            </label>
            <input
              id="user-email"
              v-model="userEmail"
              type="email"
              required
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              placeholder="seu.email@exemplo.com"
            />
          </div>

          <div v-if="needsName">
            <label for="user-name" class="block text-sm font-medium text-gray-700 mb-2">
              Nome
            </label>
            <input
              id="user-name"
              v-model="userName"
              type="text"
              required
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              placeholder="Seu nome"
            />
          </div>

          <button
            type="submit"
            :disabled="isLoading"
            class="w-full bg-blue-600 hover:bg-blue-700 disabled:bg-gray-400 text-white py-2 px-4 rounded-md transition-colors duration-200 flex items-center justify-center"
          >
            <span v-if="isLoading" class="mr-2">
              <svg class="animate-spin h-4 w-4" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
            </span>
            {{ isLoading ? 'Verificando...' : 'Continuar' }}
          </button>
        </form>

        <!-- Error Message -->
        <div v-if="errorMessage" class="mt-4 p-3 bg-red-50 border border-red-200 rounded-md">
          <p class="text-sm text-red-600">{{ errorMessage }}</p>
        </div>
      </div>
    </Transition>

    <!-- Chat Modal -->
    <Transition
      enter-active-class="transform transition duration-300 ease-out"
      enter-from-class="translate-y-full opacity-0"
      enter-to-class="translate-y-0 opacity-100"
      leave-active-class="transform transition duration-200 ease-in"
      leave-from-class="translate-y-0 opacity-100"
      leave-to-class="translate-y-full opacity-0"
    >
      <div
        v-if="isOpen"
        class="bg-white border border-gray-200 rounded-lg shadow-2xl w-80 h-96 sm:w-96 sm:h-[28rem] flex flex-col overflow-hidden"
        role="dialog"
        aria-labelledby="chat-title"
      >
        <!-- Chat Header -->
        <div class="bg-blue-600 text-white p-4 flex items-center justify-between">
          <div>
            <h3 id="chat-title" class="font-bold text-lg">Chat</h3>
            <p class="text-blue-100 text-sm">
              {{ currentUser?.name || 'Usuário' }}
            </p>
          </div>
          <button
            @click="closeChat"
            class="text-white hover:text-blue-200 transition-colors duration-200 p-1 rounded focus:outline-none focus:ring-2 focus:ring-blue-300"
            aria-label="Fechar chat"
          >
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd"></path>
            </svg>
          </button>
        </div>

        <!-- Chat Messages -->
        <div
          ref="messagesContainer"
          class="flex-1 overflow-y-auto p-4 space-y-3 bg-gray-50"
        >
          <div
            v-for="message in messages"
            :key="message.id"
            :class="[
              'flex',
              message.messageType === 'user' ? 'justify-end' : 'justify-start'
            ]"
          >
            <div
              :class="[
                'max-w-xs lg:max-w-md px-4 py-2 rounded-lg shadow-sm',
                message.messageType === 'user'
                  ? 'bg-blue-600 text-white rounded-br-none'
                  : 'bg-white text-gray-800 border border-gray-200 rounded-bl-none'
              ]"
            >
              <p class="text-sm break-words">{{ message.content }}</p>
              <div class="flex items-center justify-between mt-1 gap-2">
                <span
                  :class="[
                    'text-xs',
                    message.messageType === 'user' ? 'text-blue-100' : 'text-gray-500'
                  ]"
                >
                  {{ formatTime(message.timestamp) }}
                </span>
                <span class="text-xs text-gray-500">
                  {{ message.messageType === 'user' ? currentUser?.name : 'Bot' }}
                </span>
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <div
            v-if="messages.length === 0"
            class="text-center py-8 text-gray-500"
          >
            <div class="w-12 h-12 mx-auto mb-3 text-gray-300">
              <svg fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M18 10c0 3.866-3.582 7-8 7a8.841 8.841 0 01-4.083-.98L2 17l1.338-3.123C2.493 12.767 2 11.434 2 10c0-3.866 3.582-7 8-7s8 3.134 8 7zM7 9H5v2h2V9zm8 0h-2v2h2V9zM9 9h2v2H9V9z" clip-rule="evenodd"></path>
              </svg>
            </div>
            <p class="text-sm">Inicie uma conversa!</p>
          </div>
        </div>

        <!-- Chat Input -->
        <div class="border-t border-gray-200 p-4 bg-white">
          <form @submit.prevent="sendMessage" class="flex gap-2">
            <input
              ref="messageInput"
              v-model="newMessage"
              type="text"
              placeholder="Digite sua mensagem..."
              class="flex-1 px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm"
              maxlength="500"
            />
            <button
              type="submit"
              :disabled="!newMessage.trim()"
              class="bg-blue-600 hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed text-white px-4 py-2 rounded-lg transition-colors duration-200"
            >
              <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                <path d="M10.894 2.553a1 1 0 00-1.788 0l-7 14a1 1 0 001.169 1.409l5-1.429A1 1 0 009 15.571V11a1 1 0 112 0v4.571a1 1 0 00.725.962l5 1.428a1 1 0 001.17-1.408l-7-14z"></path>
              </svg>
            </button>
          </form>
        </div>
      </div>
    </Transition>

    <!-- Overlay for mobile -->
    <div
      v-if="isOpen || showEmailModal"
      @click="isOpen ? closeChat() : closeEmailModal()"
      class="fixed inset-0 bg-black bg-opacity-25 z-40 sm:hidden"
      aria-hidden="true"
    ></div>
  </div>
</template>

<script setup>
const { $api } = useNuxtApp()
const route = useRoute()

// State
const isOpen = ref(false)
const showEmailModal = ref(false)
const needsName = ref(false)
const userEmail = ref('')
const userName = ref('')
const isLoading = ref(false)
const errorMessage = ref('')
const currentUser = ref(null)
const currentSession = ref(null)
const messages = ref([])
const newMessage = ref('')

// Refs
const messagesContainer = ref(null)
const messageInput = ref(null)

// Methods
const handleChatButtonClick = async () => {
  // Verificar se já existe uma sessão ativa
  const activeSession = await checkActiveSession()
  
  if (activeSession) {
    // Abrir chat com sessão existente
    currentSession.value = activeSession
    await loadChatHistory()
    isOpen.value = true
    await nextTick()
    focusMessageInput()
  } else {
    // Solicitar email
    showEmailModal.value = true
  }
}

const checkActiveSession = async () => {
  // Implementar lógica para verificar sessão ativa
  // Por enquanto, simular que não há sessão ativa
  return null
}

const handleEmailSubmit = async () => {
  if (!userEmail.value.trim()) return
  
  isLoading.value = true
  errorMessage.value = ''
  
  try {
    // Verificar se o e-mail é válido e ativo no backend
    const userResponse = await $api.get(`/users/by-email?email=${encodeURIComponent(userEmail.value)}`)
    
    if (userResponse.success && userResponse.data) {
      // Usuário encontrado e ativo
      currentUser.value = userResponse.data
      needsName.value = false
      await startChatSession()
    } else {
      // E-mail não encontrado ou inativo, solicitar nome
      needsName.value = true
      if (!userName.value.trim() && needsName.value) {
        // Ainda precisa do nome
        return
      }
      
      // Cadastrar novo usuário
      const createUserResponse = await $api.post('/users', {
        email: userEmail.value,
        name: userName.value
      })
      
      if (createUserResponse.success) {
        currentUser.value = createUserResponse.data
        await startChatSession()
      } else {
        errorMessage.value = 'Erro ao cadastrar usuário. Tente novamente.'
      }
    }
  } catch (error) {
    console.error('Erro ao verificar e-mail:', error)
    if (error.response?.status === 404) {
      // E-mail não encontrado, solicitar nome
      needsName.value = true
      if (userName.value.trim()) {
        // Tentar cadastrar
        try {
          const createUserResponse = await $api.post('/users', {
            email: userEmail.value,
            name: userName.value
          })
          
          if (createUserResponse.success) {
            currentUser.value = createUserResponse.data
            await startChatSession()
          } else {
            errorMessage.value = 'Erro ao cadastrar usuário. Tente novamente.'
          }
        } catch (createError) {
          console.error('Erro ao criar usuário:', createError)
          errorMessage.value = 'Erro ao cadastrar usuário. Tente novamente.'
        }
      }
    } else {
      errorMessage.value = 'Erro ao conectar com o servidor. Tente novamente.'
    }
  } finally {
    isLoading.value = false
  }
}

const startChatSession = async () => {
  try {
    const sessionResponse = await $api.post('/chat/start', {
      userId: currentUser.value.id
    })
    
    if (sessionResponse.success) {
      currentSession.value = sessionResponse.data
      showEmailModal.value = false
      isOpen.value = true
      
      await nextTick()
      focusMessageInput()
    } else {
      errorMessage.value = 'Erro ao iniciar sessão de chat.'
    }
  } catch (error) {
    console.error('Erro ao iniciar sessão:', error)
    errorMessage.value = 'Erro ao iniciar sessão de chat.'
  }
}

const loadChatHistory = async () => {
  try {
    const historyResponse = await $api.get(`/chat/history/${currentSession.value.id}`)
    
    if (historyResponse.success) {
      messages.value = historyResponse.data || []
      await nextTick()
      scrollToBottom()
    }
  } catch (error) {
    console.error('Erro ao carregar histórico:', error)
  }
}

const sendMessage = async () => {
  if (!newMessage.value.trim() || !currentSession.value) return
  
  const messageContent = newMessage.value.trim()
  newMessage.value = ''
  
  // Adicionar mensagem do usuário imediatamente
  const userMessage = {
    id: Date.now(),
    content: messageContent,
    messageType: 'user',
    timestamp: new Date(),
    sessionId: currentSession.value.id,
    userId: currentUser.value.id
  }
  
  messages.value.push(userMessage)
  
  await nextTick()
  scrollToBottom()
  
  try {
    // Enviar mensagem para o backend
    const sendResponse = await $api.post('/chat/send', {
      sessionId: currentSession.value.id,
      userId: currentUser.value.id,
      content: messageContent,
      messageType: 'user'
    })
    
    if (sendResponse.success) {
      // Verificar se é comando de saída
      if (messageContent.toLowerCase() === 'sair') {
        // Processar comando de saída
        const botResponse = {
          id: Date.now() + 1,
          content: 'Chat encerrado. Obrigado pela conversa!',
          messageType: 'bot',
          timestamp: new Date(),
          sessionId: currentSession.value.id
        }
        
        messages.value.push(botResponse)
        
        // Encerrar sessão
        setTimeout(async () => {
          await endChatSession()
        }, 2000)
      } else {
        // Processar mensagem normal do bot
        const botResponse = await $api.post('/bot/process', {
          userId: currentUser.value.id,
          sessionId: currentSession.value.id,
          userMessage: messageContent
        })
        
        if (botResponse.success && botResponse.data) {
          const botMessage = {
            id: Date.now() + 1,
            content: botResponse.data.response,
            messageType: 'bot',
            timestamp: new Date(),
            sessionId: currentSession.value.id
          }
          
          messages.value.push(botMessage)
        }
      }
      
      await nextTick()
      scrollToBottom()
    }
  } catch (error) {
    console.error('Erro ao enviar mensagem:', error)
    // Remover mensagem em caso de erro
    const index = messages.value.findIndex(m => m.id === userMessage.id)
    if (index > -1) {
      messages.value.splice(index, 1)
    }
  }
}

const endChatSession = async () => {
  try {
    await $api.post('/chat/end', {
      sessionId: currentSession.value.id
    })
  } catch (error) {
    console.error('Erro ao encerrar sessão:', error)
  } finally {
    closeChat()
  }
}

const closeEmailModal = () => {
  showEmailModal.value = false
  userEmail.value = ''
  userName.value = ''
  needsName.value = false
  errorMessage.value = ''
  isLoading.value = false
}

const closeChat = () => {
  isOpen.value = false
  messages.value = []
  currentSession.value = null
  currentUser.value = null
  newMessage.value = ''
}

const focusMessageInput = () => {
  if (messageInput.value) {
    messageInput.value.focus()
  }
}

const scrollToBottom = () => {
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

const formatTime = (timestamp) => {
  return new Date(timestamp).toLocaleTimeString('pt-BR', {
    hour: '2-digit',
    minute: '2-digit'
  })
}

// Keyboard shortcuts
onMounted(() => {
  const handleKeydown = (event) => {
    if (event.key === 'Escape') {
      if (isOpen.value) {
        closeChat()
      } else if (showEmailModal.value) {
        closeEmailModal()
      }
    }
  }
  
  document.addEventListener('keydown', handleKeydown)
  
  onUnmounted(() => {
    document.removeEventListener('keydown', handleKeydown)
  })
})
</script>