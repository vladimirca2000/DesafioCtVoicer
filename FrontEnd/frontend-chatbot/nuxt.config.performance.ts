export default {
  runtimeConfig: {
    public: {
      // Analytics IDs (example values)
      googleAnalyticsId: process.env.GOOGLE_ANALYTICS_ID || 'G-XXXXXXXXXX',
      googleMapsApiKey: process.env.GOOGLE_MAPS_API_KEY || '',
      
      // API Configuration
      apiBaseUrl: process.env.API_BASE_URL || 'http://localhost:5000/api',
      signalrHubUrl: process.env.SIGNALR_HUB_URL || 'http://localhost:5000/chathub',
      
      // Performance Configuration
      enableServiceWorker: process.env.NODE_ENV === 'production',
      enableWebVitals: true,
      enablePerformanceMonitoring: true
    }
  },

  // SEO and Performance
  app: {
    head: {
      charset: 'utf-8',
      viewport: 'width=device-width, initial-scale=1',
      meta: [
        { name: 'format-detection', content: 'telephone=no' },
        { name: 'theme-color', content: '#1a1a2e' },
        { name: 'apple-mobile-web-app-capable', content: 'yes' },
        { name: 'apple-mobile-web-app-status-bar-style', content: 'black-translucent' }
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
        { rel: 'preconnect', href: 'https://fonts.googleapis.com' },
        { rel: 'preconnect', href: 'https://fonts.gstatic.com', crossorigin: '' },
        { rel: 'dns-prefetch', href: 'https://api.example.com' }
      ]
    }
  },

  // Modules
  modules: [
    '@nuxt/ui',
    '@pinia/nuxt',
    '@vueuse/nuxt'
  ],

  // CSS
  css: ['~/assets/css/main.css'],

  // TypeScript
  typescript: {
    strict: true,
    typeCheck: true
  },

  // Build Configuration
  nitro: {
    compressPublicAssets: true,
    minify: true
  },

  // Vite Configuration
  vite: {
    build: {
      cssCodeSplit: true,
      rollupOptions: {
        output: {
          manualChunks: {
            vendor: ['vue', 'vue-router'],
            ui: ['@nuxt/ui']
          }
        }
      }
    },
    optimizeDeps: {
      include: ['vue', 'vue-router', 'pinia']
    }
  },

  // Performance optimizations
  experimental: {
    payloadExtraction: false,
    inlineSSRStyles: false
  },

  // Route rules for caching
  routeRules: {
    // Homepage pre-rendered at build time
    '/': { prerender: true },
    // Contact page generated on-demand, revalidates in background
    '/contact': { ssr: true },
    // Static assets cached for 1 year
    '/images/**': { headers: { 'cache-control': 'max-age=31536000' } },
    // API routes cached for 5 minutes
    '/api/**': { headers: { 'cache-control': 'max-age=300' } }
  },

  // Development configuration
  devtools: { enabled: true },

  // Compatibility date
  compatibilityDate: '2025-08-05'
}
