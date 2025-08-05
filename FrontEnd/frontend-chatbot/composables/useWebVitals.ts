export default function useWebVitals() {
  const vitals = reactive({
    FCP: null, // First Contentful Paint
    LCP: null, // Largest Contentful Paint
    FID: null, // First Input Delay
    CLS: null, // Cumulative Layout Shift
    TTFB: null // Time to First Byte
  })

  const trackWebVitals = () => {
    if (typeof window === 'undefined') return

    // Track TTFB
    if (window.performance && window.performance.timing) {
      const timing = window.performance.timing
      vitals.TTFB = timing.responseStart - timing.requestStart
    }

    // Track FCP using Performance Observer
    if ('PerformanceObserver' in window) {
      try {
        // FCP Observer
        const fcpObserver = new PerformanceObserver((list) => {
          const entries = list.getEntries()
          const fcpEntry = entries.find(entry => entry.name === 'first-contentful-paint')
          if (fcpEntry) {
            vitals.FCP = fcpEntry.startTime
            fcpObserver.disconnect()
          }
        })
        fcpObserver.observe({ entryTypes: ['paint'] })

        // LCP Observer
        const lcpObserver = new PerformanceObserver((list) => {
          const entries = list.getEntries()
          const lastEntry = entries[entries.length - 1]
          vitals.LCP = lastEntry.startTime
        })
        lcpObserver.observe({ entryTypes: ['largest-contentful-paint'] })

        // CLS Observer
        let clsValue = 0
        const clsObserver = new PerformanceObserver((list) => {
          for (const entry of list.getEntries()) {
            if (!entry.hadRecentInput) {
              clsValue += entry.value
              vitals.CLS = clsValue
            }
          }
        })
        clsObserver.observe({ entryTypes: ['layout-shift'] })

        // FID Observer
        const fidObserver = new PerformanceObserver((list) => {
          for (const entry of list.getEntries()) {
            vitals.FID = entry.processingStart - entry.startTime
            fidObserver.disconnect()
          }
        })
        fidObserver.observe({ entryTypes: ['first-input'] })

      } catch (error) {
        console.warn('Performance tracking not supported:', error)
      }
    }
  }

  const sendVitalsToAnalytics = () => {
    // Send to Google Analytics or other analytics service
    if (typeof window !== 'undefined' && 'gtag' in window) {
      Object.entries(vitals).forEach(([metric, value]) => {
        if (value !== null) {
          window.gtag('event', 'web_vitals', {
            metric_name: metric,
            metric_value: Math.round(value),
            custom_parameter: 'performance'
          })
        }
      })
    }
  }

  const getPerformanceScore = () => {
    const thresholds = {
      FCP: { good: 1800, needsImprovement: 3000 },
      LCP: { good: 2500, needsImprovement: 4000 },
      FID: { good: 100, needsImprovement: 300 },
      CLS: { good: 0.1, needsImprovement: 0.25 },
      TTFB: { good: 800, needsImprovement: 1800 }
    }

    const scores = {}
    
    Object.entries(vitals).forEach(([metric, value]) => {
      if (value !== null && thresholds[metric]) {
        const threshold = thresholds[metric]
        if (value <= threshold.good) {
          scores[metric] = 'good'
        } else if (value <= threshold.needsImprovement) {
          scores[metric] = 'needs-improvement'
        } else {
          scores[metric] = 'poor'
        }
      }
    })

    return scores
  }

  const logPerformanceIssues = () => {
    const scores = getPerformanceScore()
    const issues = Object.entries(scores)
      .filter(([_, score]) => score === 'poor')
      .map(([metric]) => metric)

    if (issues.length > 0) {
      console.warn('Performance issues detected:', issues)
      console.table(vitals)
    }
  }

  return {
    vitals: readonly(vitals),
    trackWebVitals,
    sendVitalsToAnalytics,
    getPerformanceScore,
    logPerformanceIssues
  }
}
