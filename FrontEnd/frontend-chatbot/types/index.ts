// types/index.ts
export * from './chat'
export * from './api'

// Global types
export interface LoadingState {
  isLoading: boolean
  error?: string | null
}

export interface NavigationItem {
  name: string
  href: string
  icon?: string
  external?: boolean
}
