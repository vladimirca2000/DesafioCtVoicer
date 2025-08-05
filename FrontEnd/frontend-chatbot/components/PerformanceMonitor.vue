<!-- Componente de Performance Monitor -->
<template>
  <div v-if="isDev && showMonitor" class="fixed bottom-4 right-4 z-50">
    <div class="bg-black/80 text-white p-4 rounded-lg text-xs font-mono max-w-xs">
      <div class="flex justify-between items-center mb-2">
        <h3 class="font-bold">Performance</h3>
        <button @click="toggleMonitor" class="text-gray-300 hover:text-white">×</button>
      </div>
      
      <!-- Web Vitals -->
      <div class="space-y-1">
        <div v-for="(value, metric) in vitals" :key="metric" class="flex justify-between">
          <span>{{ metric }}:</span>
          <span :class="getScoreColor(metric, value)">
            {{ value ? Math.round(value) : '-' }}{{ getUnit(metric) }}
          </span>
        </div>
      </div>

      <!-- Network Info -->
      <div class="mt-2 pt-2 border-t border-gray-600">
        <div class="flex justify-between">
          <span>Connection:</span>
          <span :class="isOnline ? 'text-green-400' : 'text-red-400'">
            {{ isOnline ? 'Online' : 'Offline' }}
          </span>
        </div>
        
        <div v-if="networkType" class="flex justify-between">
          <span>Type:</span>
          <span class="text-blue-400">{{ networkType }}</span>
        </div>
      </div>

      <!-- Performance Score -->
      <div class="mt-2 pt-2 border-t border-gray-600">
        <div class="flex justify-between">
          <span>Score:</span>
          <span :class="getOverallScoreColor()">
            {{ overallScore }}%
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const isDev = process.env.NODE_ENV === 'development'
const showMonitor = ref(false)
const appStore = useAppStore()

// Simulated vitals (would come from useWebVitals in real implementation)
const vitals = reactive({
  FCP: null,
  LCP: null,
  FID: null,
  CLS: null,
  TTFB: null
})

// Network monitoring
const isOnline = computed(() => appStore.isOnline)
const networkType = ref('')

// Computed properties
const overallScore = computed(() => {
  const thresholds = {
    FCP: { good: 1800, needsImprovement: 3000 },
    LCP: { good: 2500, needsImprovement: 4000 },
    FID: { good: 100, needsImprovement: 300 },
    CLS: { good: 0.1, needsImprovement: 0.25 },
    TTFB: { good: 800, needsImprovement: 1800 }
  }

  let totalScore = 0
  let validMetrics = 0

  Object.entries(vitals).forEach(([metric, value]) => {
    if (value !== null && thresholds[metric]) {
      const threshold = thresholds[metric]
      let score = 0
      
      if (value <= threshold.good) {
        score = 100
      } else if (value <= threshold.needsImprovement) {
        score = 75
      } else {
        score = 50
      }
      
      totalScore += score
      validMetrics++
    }
  })

  return validMetrics > 0 ? Math.round(totalScore / validMetrics) : 0
})

// Methods
const toggleMonitor = () => {
  showMonitor.value = !showMonitor.value
}

const getScoreColor = (metric, value) => {
  if (!value) return 'text-gray-400'
  
  const thresholds = {
    FCP: { good: 1800, needsImprovement: 3000 },
    LCP: { good: 2500, needsImprovement: 4000 },
    FID: { good: 100, needsImprovement: 300 },
    CLS: { good: 0.1, needsImprovement: 0.25 },
    TTFB: { good: 800, needsImprovement: 1800 }
  }

  const threshold = thresholds[metric]
  if (!threshold) return 'text-gray-400'

  if (value <= threshold.good) return 'text-green-400'
  if (value <= threshold.needsImprovement) return 'text-yellow-400'
  return 'text-red-400'
}

const getOverallScoreColor = () => {
  const score = overallScore.value
  if (score >= 90) return 'text-green-400'
  if (score >= 75) return 'text-yellow-400'
  return 'text-red-400'
}

const getUnit = (metric) => {
  switch (metric) {
    case 'CLS':
      return ''
    case 'FCP':
    case 'LCP':
    case 'FID':
    case 'TTFB':
      return 'ms'
    default:
      return ''
  }
}

// Lifecycle
onMounted(() => {
  // Show monitor in development
  if (isDev) {
    setTimeout(() => {
      showMonitor.value = true
    }, 2000)
  }

  // Monitor network
  if (typeof navigator !== 'undefined' && 'connection' in navigator) {
    const connection = navigator.connection
    networkType.value = connection.effectiveType || 'unknown'
    
    connection.addEventListener('change', () => {
      networkType.value = connection.effectiveType || 'unknown'
    })
  }

  // Simulate performance tracking
  setTimeout(() => {
    vitals.FCP = 1200 + Math.random() * 800
    vitals.LCP = 2000 + Math.random() * 1000
    vitals.FID = 50 + Math.random() * 100
    vitals.CLS = Math.random() * 0.2
    vitals.TTFB = 400 + Math.random() * 600
  }, 3000)
})

// Keyboard shortcut to toggle monitor
onMounted(() => {
  const handleKeydown = (e) => {
    if (e.key === 'p' && e.ctrlKey && e.shiftKey) {
      e.preventDefault()
      toggleMonitor()
    }
  }
  
  document.addEventListener('keydown', handleKeydown)
  
  onUnmounted(() => {
    document.removeEventListener('keydown', handleKeydown)
  })
})
</script>
