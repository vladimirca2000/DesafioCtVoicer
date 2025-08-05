// Performance optimization utilities for production
export const performanceConfig = {
  // Lighthouse performance thresholds
  thresholds: {
    performance: 90,
    accessibility: 95,
    bestPractices: 90,
    seo: 95
  },
  
  // Critical resources to preload
  criticalResources: [
    '/critical.css',
    '/fonts/inter-var.woff2'
  ],
  
  // Domains to preconnect
  preconnectDomains: [
    'https://fonts.googleapis.com',
    'https://fonts.gstatic.com',
    'https://api.example.com'
  ],
  
  // Images to lazy load
  lazyLoadImages: true,
  
  // Service worker configuration
  serviceWorker: {
    enabled: true,
    cacheStrategy: 'networkFirst',
    offlinePage: '/offline'
  }
}

export const webVitalsThresholds = {
  FCP: { good: 1800, needsImprovement: 3000 }, // First Contentful Paint
  LCP: { good: 2500, needsImprovement: 4000 }, // Largest Contentful Paint
  FID: { good: 100, needsImprovement: 300 },   // First Input Delay
  CLS: { good: 0.1, needsImprovement: 0.25 },  // Cumulative Layout Shift
  TTFB: { good: 800, needsImprovement: 1800 }   // Time to First Byte
}

export const cacheStrategies = {
  static: {
    maxAge: 31536000, // 1 year
    immutable: true
  },
  api: {
    maxAge: 300, // 5 minutes
    staleWhileRevalidate: 86400 // 1 day
  },
  images: {
    maxAge: 2592000, // 30 days
    staleWhileRevalidate: 86400 // 1 day
  }
}
