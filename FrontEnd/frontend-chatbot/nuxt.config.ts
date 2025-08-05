export default defineNuxtConfig({
  runtimeConfig: {
    public: {
      googleMapsApiKey: process.env.GOOGLE_MAPS_API_KEY || '',
      apiBaseUrl: process.env.API_BASE_URL || 'https://localhost:5001/api',
      signalrHubUrl: process.env.SIGNALR_HUB_URL || 'https://localhost:5001/chathub',
      enableWebVitals: true
    }
  },

  app: {
    head: {
      charset: 'utf-8',
      viewport: 'width=device-width, initial-scale=1',
      meta: [
        { name: 'format-detection', content: 'telephone=no' },
        { name: 'theme-color', content: '#1a1a2e' }
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
        { rel: 'preconnect', href: 'https://fonts.googleapis.com' },
        { rel: 'preconnect', href: 'https://fonts.gstatic.com', crossorigin: '' }
      ]
    }
  },

  modules: [
    '@nuxt/ui',
    '@pinia/nuxt'
  ],

  typescript: {
    strict: true,
    typeCheck: false
  },

  // Performance optimizations
  nitro: {
    compressPublicAssets: true,
    minify: true,
    compatibilityDate: '2025-08-05'
  },

  vite: {
    server: {
      open: true
    }
  },

  devtools: { enabled: true },
  compatibilityDate: '2025-08-05'
})