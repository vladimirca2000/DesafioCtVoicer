// stores/app.ts
import { defineStore } from 'pinia'
import type { ContactInfo, F1Info, LoadingState } from '~/types'

interface AppState {
  loading: LoadingState
  contactInfo: ContactInfo
  f1Info: F1Info
  theme: 'light' | 'dark'
  notifications: Notification[]
}

interface Notification {
  id: string
  type: 'success' | 'error' | 'warning' | 'info'
  title: string
  message: string
  duration?: number
  createdAt: Date
}

export const useAppStore = defineStore('app', {
  state: (): AppState => ({
    loading: {
      isLoading: false,
      error: null
    },
    contactInfo: {
      name: 'Vladimir Carlos Alves',
      email: 'vladimir@example.com',
      phone: '(XX) XXXXX-XXXX',
      linkedin: 'linkedin.com/in/vladimir-carlos-alves',
      github: 'github.com/vladimir-carlos-alves',
      location: 'Brasil'
    },
    f1Info: {
      favoriteTeam: 'Red Bull Racing',
      favoriteDriver: 'Max Verstappen',
      favoriteSeason: '2021',
      currentChampion: 'Max Verstappen'
    },
    theme: 'light',
    notifications: []
  }),

  getters: {
    isLoading: (state) => state.loading.isLoading,
    hasError: (state) => !!state.loading.error,
    activeNotifications: (state) => state.notifications.filter(n => 
      Date.now() - n.createdAt.getTime() < (n.duration || 5000)
    )
  },

  actions: {
    setLoading(isLoading: boolean, error: string | null = null) {
      this.loading = { isLoading, error }
    },

    setError(error: string) {
      this.loading.error = error
      this.loading.isLoading = false
    },

    clearError() {
      this.loading.error = null
    },

    updateContactInfo(info: Partial<ContactInfo>) {
      this.contactInfo = { ...this.contactInfo, ...info }
    },

    updateF1Info(info: Partial<F1Info>) {
      this.f1Info = { ...this.f1Info, ...info }
    },

    toggleTheme() {
      this.theme = this.theme === 'light' ? 'dark' : 'light'
    },

    addNotification(notification: Omit<Notification, 'id' | 'createdAt'>) {
      const newNotification: Notification = {
        ...notification,
        id: `notif_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
        createdAt: new Date()
      }
      
      this.notifications.push(newNotification)

      // Auto remove notification
      setTimeout(() => {
        this.removeNotification(newNotification.id)
      }, notification.duration || 5000)

      return newNotification.id
    },

    removeNotification(id: string) {
      const index = this.notifications.findIndex(n => n.id === id)
      if (index > -1) {
        this.notifications.splice(index, 1)
      }
    },

    clearNotifications() {
      this.notifications = []
    }
  }
})
