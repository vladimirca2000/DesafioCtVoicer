// composables/useChat.ts
import type { ChatMessage, ChatSession, SignalRConnection } from '~/types'

export const useChat = () => {
  const config = useRuntimeConfig()
  const signalrUrl = config.public.signalrHubUrl

  // Estado reativo do chat
  const isOpen = ref(false)
  const messages = ref<ChatMessage[]>([])
  const isConnected = ref(false)
  const isTyping = ref(false)
  const connectionError = ref<string | null>(null)

  // Controles do chat
  const toggleChat = () => {
    isOpen.value = !isOpen.value
  }

  const openChat = () => {
    isOpen.value = true
  }

  const closeChat = () => {
    isOpen.value = false
  }

  // Adicionar mensagem
  const addMessage = (content: string, isUser = false): ChatMessage => {
    const message: ChatMessage = {
      id: Date.now().toString(),
      content,
      timestamp: new Date(),
      isUser,
      status: isUser ? 'sending' : 'sent'
    }
    
    messages.value.push(message)
    return message
  }

  // Enviar mensagem
  const sendMessage = async (content: string) => {
    if (!content.trim()) return

    // Adicionar mensagem do usuário
    const userMessage = addMessage(content, true)

    try {
      // Aqui você pode implementar a lógica do SignalR
      // Por enquanto, simular resposta do bot
      setTimeout(() => {
        userMessage.status = 'sent'
        addMessage(`Resposta para: "${content}"`, false)
      }, 1000)

    } catch (error) {
      userMessage.status = 'error'
      console.error('Erro ao enviar mensagem:', error)
    }
  }

  // Limpar chat
  const clearMessages = () => {
    messages.value = []
  }

  return {
    // Estado
    isOpen: readonly(isOpen),
    messages: readonly(messages),
    isConnected: readonly(isConnected),
    isTyping: readonly(isTyping),
    connectionError: readonly(connectionError),

    // Ações
    toggleChat,
    openChat,
    closeChat,
    sendMessage,
    clearMessages,
    addMessage
  }
}
