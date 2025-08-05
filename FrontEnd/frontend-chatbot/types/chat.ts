// types/chat.ts
export interface ChatMessage {
  id: string
  content: string
  timestamp: Date
  isUser: boolean
  status?: 'sending' | 'sent' | 'error'
}

export interface ChatSession {
  id: string
  messages: ChatMessage[]
  isActive: boolean
  createdAt: Date
}

export interface SignalRConnection {
  connectionId?: string
  isConnected: boolean
  lastActivity?: Date
}
