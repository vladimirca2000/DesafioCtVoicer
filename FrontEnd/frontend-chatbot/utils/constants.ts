// utils/constants.ts

// Configurações da aplicação
export const APP_CONFIG = {
  name: 'Vladimir - Fã de Formula 1',
  version: '1.0.0',
  description: 'Portfólio pessoal com chat integrado',
  author: 'Vladimir Carlos Alves'
} as const

// Temas da F1
export const F1_COLORS = {
  red: '#E10600',
  dark: '#15151E', 
  silver: '#C0C0C0',
  white: '#FFFFFF',
  gold: '#FFD700'
} as const

// Configurações do chat
export const CHAT_CONFIG = {
  maxMessageLength: 500,
  typingDelay: 1000,
  messageRetryAttempts: 3,
  connectionTimeout: 30000,
  maxMessagesHistory: 100
} as const

// Links externos
export const EXTERNAL_LINKS = {
  f1Official: 'https://www.formula1.com',
  linkedin: 'https://linkedin.com/in/vladimir-carlos-alves',
  github: 'https://github.com/vladimir-carlos-alves',
  email: 'mailto:vladimir@example.com'
} as const

// Navegação
export const NAVIGATION_ITEMS = [
  { name: 'Home', href: '/', icon: 'home' },
  { name: 'Contato', href: '/contact', icon: 'contact' }
] as const

// Estados de conexão
export const CONNECTION_STATUS = {
  CONNECTING: 'connecting',
  CONNECTED: 'connected',
  DISCONNECTED: 'disconnected',
  RECONNECTING: 'reconnecting',
  ERROR: 'error'
} as const

// Tipos de notificação
export const NOTIFICATION_TYPES = {
  SUCCESS: 'success',
  ERROR: 'error', 
  WARNING: 'warning',
  INFO: 'info'
} as const
