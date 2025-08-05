// stores/chat.ts
import { defineStore } from 'pinia'
import type { ChatMessage, ChatSession } from '~/types'

interface ChatState {
  messages: ChatMessage[]
  isOpen: boolean
  isConnected: boolean
  isTyping: boolean
  connectionError: string | null
  currentSession: ChatSession | null
}

export const useChatStore = defineStore('chat', {
  state: (): ChatState => ({
    messages: [],
    isOpen: false,
    isConnected: false,
    isTyping: false,
    connectionError: null,
    currentSession: null
  }),

  getters: {
    lastMessage: (state) => {
      return state.messages[state.messages.length - 1] || null
    },
    
    messageCount: (state) => state.messages.length,
    
    hasMessages: (state) => state.messages.length > 0,
    
    userMessages: (state) => {
      return state.messages.filter(msg => msg.isUser)
    },
    
    botMessages: (state) => {
      return state.messages.filter(msg => !msg.isUser)
    }
  },

  actions: {
    toggleChat() {
      this.isOpen = !this.isOpen
    },

    openChat() {
      this.isOpen = true
    },

    closeChat() {
      this.isOpen = false
    },

    addMessage(content: string, isUser = false): ChatMessage {
      const message: ChatMessage = {
        id: `msg_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
        content: content.trim(),
        timestamp: new Date(),
        isUser,
        status: isUser ? 'sending' : 'sent'
      }
      
      this.messages.push(message)
      return message
    },

    updateMessageStatus(messageId: string, status: ChatMessage['status']) {
      const message = this.messages.find(msg => msg.id === messageId)
      if (message) {
        message.status = status
      }
    },

    async sendMessage(content: string) {
      if (!content.trim()) return null

      const userMessage = this.addMessage(content, true)

      try {
        // Simular delay da API
        await new Promise(resolve => setTimeout(resolve, 500))
        
        // Marcar como enviada
        this.updateMessageStatus(userMessage.id, 'sent')
        
        // Simular resposta do bot
        this.isTyping = true
        await new Promise(resolve => setTimeout(resolve, 1000))
        
        this.isTyping = false
        this.addMessage(`Obrigado pela sua mensagem: "${content}". Como posso ajudar?`, false)
        
        return userMessage
      } catch (error) {
        this.updateMessageStatus(userMessage.id, 'error')
        this.connectionError = 'Erro ao enviar mensagem'
        throw error
      }
    },

    clearMessages() {
      this.messages = []
    },

    setConnectionStatus(isConnected: boolean) {
      this.isConnected = isConnected
      if (isConnected) {
        this.connectionError = null
      }
    },

    setConnectionError(error: string | null) {
      this.connectionError = error
      if (error) {
        this.isConnected = false
      }
    },

    startNewSession() {
      const session: ChatSession = {
        id: `session_${Date.now()}`,
        messages: [],
        isActive: true,
        createdAt: new Date()
      }
      
      this.currentSession = session
      this.clearMessages()
      return session
    }
  }
})
