// composables/usePerformance.ts
export const usePerformance = () => {
  // Intersection Observer for lazy loading
  const useIntersectionObserver = (
    target: Ref<Element | null>,
    callback: IntersectionObserverCallback,
    options: IntersectionObserverInit = {}
  ) => {
    const isSupported = typeof window !== 'undefined' && 'IntersectionObserver' in window
    
    if (!isSupported) return { isSupported: false }

    const observer = ref<IntersectionObserver | null>(null)

    const cleanup = () => {
      if (observer.value) {
        observer.value.disconnect()
        observer.value = null
      }
    }

    const observe = () => {
      if (target.value && !observer.value) {
        observer.value = new IntersectionObserver(callback, {
          rootMargin: '50px',
          threshold: 0.1,
          ...options
        })
        observer.value.observe(target.value)
      }
    }

    onMounted(() => {
      nextTick(observe)
    })

    onUnmounted(cleanup)

    return { isSupported: true, cleanup, observe }
  }

  // Image lazy loading with blur placeholder
  const useLazyImage = (src: string) => {
    const imageRef = ref<HTMLImageElement | null>(null)
    const isLoaded = ref(false)
    const isInView = ref(false)
    const error = ref<string | null>(null)

    const { isSupported } = useIntersectionObserver(
      imageRef,
      ([entry]) => {
        if (entry.isIntersecting) {
          isInView.value = true
        }
      }
    )

    const loadImage = () => {
      if (!src || !isInView.value) return

      const img = new Image()
      
      img.onload = () => {
        isLoaded.value = true
        error.value = null
      }
      
      img.onerror = () => {
        error.value = 'Failed to load image'
      }
      
      img.src = src
    }

    watch(isInView, (inView) => {
      if (inView && !isLoaded.value) {
        loadImage()
      }
    })

    return {
      imageRef,
      isLoaded,
      isInView,
      error,
      isSupported
    }
  }

  // Debounce function for performance
  const useDebounceFn = <T extends (...args: any[]) => any>(
    fn: T,
    delay: number = 300
  ) => {
    let timeoutId: number | null = null

    const debouncedFn = (...args: Parameters<T>) => {
      if (timeoutId) {
        clearTimeout(timeoutId)
      }
      
      timeoutId = setTimeout(() => {
        fn(...args)
        timeoutId = null
      }, delay)
    }

    const cancel = () => {
      if (timeoutId) {
        clearTimeout(timeoutId)
        timeoutId = null
      }
    }

    onUnmounted(() => {
      cancel()
    })

    return { debouncedFn, cancel }
  }

  // Throttle function for performance
  const useThrottleFn = <T extends (...args: any[]) => any>(
    fn: T,
    delay: number = 300
  ) => {
    let isThrottled = false

    const throttledFn = (...args: Parameters<T>) => {
      if (isThrottled) return

      fn(...args)
      isThrottled = true

      setTimeout(() => {
        isThrottled = false
      }, delay)
    }

    return throttledFn
  }

  // Prefetch resources
  const prefetchResource = (href: string, as: string = 'fetch') => {
    if (typeof window === 'undefined') return

    const link = document.createElement('link')
    link.rel = 'prefetch'
    link.href = href
    link.as = as
    document.head.appendChild(link)
  }

  // Preload critical resources
  const preloadResource = (href: string, as: string, type?: string) => {
    if (typeof window === 'undefined') return

    const link = document.createElement('link')
    link.rel = 'preload'
    link.href = href
    link.as = as
    if (type) link.type = type
    document.head.appendChild(link)
  }

  // Memory usage monitoring
  const useMemoryInfo = () => {
    const memoryInfo = ref<any>(null)

    const updateMemoryInfo = () => {
      if ('memory' in performance) {
        memoryInfo.value = (performance as any).memory
      }
    }

    onMounted(() => {
      updateMemoryInfo()
      const interval = setInterval(updateMemoryInfo, 5000)
      
      onUnmounted(() => {
        clearInterval(interval)
      })
    })

    return { memoryInfo }
  }

  // Performance metrics
  const usePerformanceMetrics = () => {
    const metrics = ref({
      navigationStart: 0,
      domContentLoaded: 0,
      loadComplete: 0,
      firstPaint: 0,
      firstContentfulPaint: 0
    })

    onMounted(() => {
      // Wait for page to load
      setTimeout(() => {
        const navigation = performance.getEntriesByType('navigation')[0] as PerformanceNavigationTiming
        
        if (navigation) {
          metrics.value = {
            navigationStart: navigation.navigationStart,
            domContentLoaded: navigation.domContentLoadedEventEnd - navigation.navigationStart,
            loadComplete: navigation.loadEventEnd - navigation.navigationStart,
            firstPaint: 0,
            firstContentfulPaint: 0
          }
        }

        // Get paint metrics
        const paintEntries = performance.getEntriesByType('paint')
        paintEntries.forEach((entry) => {
          if (entry.name === 'first-paint') {
            metrics.value.firstPaint = entry.startTime
          } else if (entry.name === 'first-contentful-paint') {
            metrics.value.firstContentfulPaint = entry.startTime
          }
        })
      }, 1000)
    })

    return { metrics }
  }

  return {
    useIntersectionObserver,
    useLazyImage,
    useDebounceFn,
    useThrottleFn,
    prefetchResource,
    preloadResource,
    useMemoryInfo,
    usePerformanceMetrics
  }
}
