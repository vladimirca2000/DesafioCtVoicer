<!-- components/AppNotifications.vue -->
<template>
  <Teleport to="body">
    <div
      class="fixed top-4 right-4 z-50 space-y-3 max-w-sm w-full"
      aria-live="polite"
      aria-label="Notificações"
    >
      <TransitionGroup
        enter-active-class="transform transition duration-300 ease-out"
        enter-from-class="translate-x-full opacity-0"
        enter-to-class="translate-x-0 opacity-100"
        leave-active-class="transform transition duration-200 ease-in"
        leave-from-class="translate-x-0 opacity-100"
        leave-to-class="translate-x-full opacity-0"
        move-class="transition-transform duration-300"
      >
        <div
          v-for="notification in appStore.activeNotifications"
          :key="notification.id"
          :class="[
            'relative bg-white rounded-lg shadow-xl border-l-4 p-4 pointer-events-auto overflow-hidden',
            notificationStyles[notification.type]
          ]"
          role="alert"
          :aria-labelledby="`notification-title-${notification.id}`"
          :aria-describedby="`notification-message-${notification.id}`"
        >
          <!-- Progress bar -->
          <div
            :class="[
              'absolute bottom-0 left-0 h-1 transition-all duration-linear',
              progressBarStyles[notification.type]
            ]"
            :style="{ 
              width: getProgressWidth(notification), 
              animationDuration: `${notification.duration || 5000}ms` 
            }"
          ></div>

          <div class="flex items-start space-x-3">
            <!-- Icon -->
            <div :class="['flex-shrink-0 mt-0.5', iconStyles[notification.type]]">
              <UIcon :name="notificationIcons[notification.type]" class="w-5 h-5" />
            </div>

            <!-- Content -->
            <div class="flex-1 min-w-0">
              <h4 
                :id="`notification-title-${notification.id}`"
                :class="['text-sm font-semibold', titleStyles[notification.type]]"
              >
                {{ notification.title }}
              </h4>
              <p 
                :id="`notification-message-${notification.id}`"
                :class="['mt-1 text-sm', messageStyles[notification.type]]"
              >
                {{ notification.message }}
              </p>
            </div>

            <!-- Close button -->
            <button
              @click="appStore.removeNotification(notification.id)"
              :class="['flex-shrink-0 rounded-md p-1.5 transition-colors duration-200', closeButtonStyles[notification.type]]"
              :aria-label="`Fechar notificação: ${notification.title}`"
            >
              <UIcon name="i-heroicons-x-mark" class="w-4 h-4" />
            </button>
          </div>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<script setup>
const appStore = useAppStore()

// Styles for different notification types
const notificationStyles = {
  success: 'border-green-500 bg-green-50',
  error: 'border-red-500 bg-red-50',
  warning: 'border-yellow-500 bg-yellow-50',
  info: 'border-blue-500 bg-blue-50'
}

const iconStyles = {
  success: 'text-green-500',
  error: 'text-red-500',
  warning: 'text-yellow-500',
  info: 'text-blue-500'
}

const titleStyles = {
  success: 'text-green-900',
  error: 'text-red-900',
  warning: 'text-yellow-900',
  info: 'text-blue-900'
}

const messageStyles = {
  success: 'text-green-700',
  error: 'text-red-700',
  warning: 'text-yellow-700',
  info: 'text-blue-700'
}

const closeButtonStyles = {
  success: 'text-green-500 hover:bg-green-100 focus:bg-green-100',
  error: 'text-red-500 hover:bg-red-100 focus:bg-red-100',
  warning: 'text-yellow-500 hover:bg-yellow-100 focus:bg-yellow-100',
  info: 'text-blue-500 hover:bg-blue-100 focus:bg-blue-100'
}

const progressBarStyles = {
  success: 'bg-green-500',
  error: 'bg-red-500',
  warning: 'bg-yellow-500',
  info: 'bg-blue-500'
}

const notificationIcons = {
  success: 'i-heroicons-check-circle',
  error: 'i-heroicons-exclamation-circle',
  warning: 'i-heroicons-exclamation-triangle',
  info: 'i-heroicons-information-circle'
}

// Calculate progress bar width based on time elapsed
const getProgressWidth = (notification) => {
  const duration = notification.duration || 5000
  const elapsed = Date.now() - notification.createdAt.getTime()
  const remaining = Math.max(0, duration - elapsed)
  const percentage = (remaining / duration) * 100
  return `${percentage}%`
}

// Update progress bars periodically
let progressInterval = null

onMounted(() => {
  progressInterval = setInterval(() => {
    // Force reactivity update for progress bars
    appStore.activeNotifications.forEach(() => {})
  }, 100)
})

onUnmounted(() => {
  if (progressInterval) {
    clearInterval(progressInterval)
  }
})
</script>

<style scoped>
.transition-all {
  transition-property: all;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
}

.duration-linear {
  transition-timing-function: linear;
}
</style>
