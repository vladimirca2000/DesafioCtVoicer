<!-- components/AppNavbar.vue -->
<template>
  <nav 
    class="fixed top-0 left-0 w-full bg-f1-red text-white shadow-lg z-30 transition-all duration-300"
    role="navigation"
    aria-label="Navegação principal"
  >
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between items-center h-16">
        <!-- Left side - Menu items -->
        <div class="flex items-center space-x-8">
          <NuxtLink
            to="/"
            class="px-3 py-2 rounded-md text-base font-medium hover:text-red-200 transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-red-300 focus:ring-opacity-50"
            :class="{ 'text-red-200': $route.path === '/' }"
          >
            Home
          </NuxtLink>
          <NuxtLink
            to="/contact"
            class="px-3 py-2 rounded-md text-base font-medium hover:text-red-200 transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-red-300 focus:ring-opacity-50"
            :class="{ 'text-red-200': $route.path === '/contact' }"
          >
            Contato
          </NuxtLink>
        </div>

        <!-- Right side - Vladimir name -->
        <div class="flex items-center">
          <NuxtLink 
            to="/" 
            class="px-3 py-2 rounded-md text-base font-medium hover:text-red-200 transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-red-300 focus:ring-opacity-50"
          >
            Vladimir
          </NuxtLink>
        </div>

        <!-- Mobile menu button -->
        <div class="md:hidden">
          <button
            @click="toggleMobileMenu"
            class="inline-flex items-center justify-center p-2 rounded-md text-white hover:text-red-200 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-red-300 transition-colors duration-200"
            :aria-expanded="isMobileMenuOpen"
            aria-label="Abrir menu de navegação"
          >
            <UIcon 
              :name="isMobileMenuOpen ? 'i-heroicons-x-mark' : 'i-heroicons-bars-3'" 
              class="w-6 h-6 transition-transform duration-200"
              :class="{ 'rotate-180': isMobileMenuOpen }"
            />
          </button>
        </div>
      </div>
    </div>

    <!-- Mobile Navigation Menu -->
    <Transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="transform scale-95 opacity-0"
      enter-to-class="transform scale-100 opacity-100"
      leave-active-class="transition duration-75 ease-in"
      leave-from-class="transform scale-100 opacity-100"
      leave-to-class="transform scale-95 opacity-0"
    >
      <div 
        v-if="isMobileMenuOpen"
        class="md:hidden bg-red-700 border-t border-red-600"
        role="menu"
        aria-orientation="vertical"
        aria-labelledby="mobile-menu"
      >
        <div class="px-2 pt-2 pb-3 space-y-1">
          <NuxtLink
            to="/"
            @click="closeMobileMenu"
            class="block px-3 py-2 rounded-md text-base font-medium text-white hover:text-red-200 hover:bg-red-600 transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-red-300 focus:ring-opacity-50"
            :class="{ 'bg-red-600 text-red-200': $route.path === '/' }"
            role="menuitem"
          >
            Home
          </NuxtLink>
          <NuxtLink
            to="/contact"
            @click="closeMobileMenu"
            class="block px-3 py-2 rounded-md text-base font-medium text-white hover:text-red-200 hover:bg-red-600 transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-red-300 focus:ring-opacity-50"
            :class="{ 'bg-red-600 text-red-200': $route.path === '/contact' }"
            role="menuitem"
          >
            Contato
          </NuxtLink>
          <NuxtLink
            to="/"
            @click="closeMobileMenu"
            class="block px-3 py-2 rounded-md text-base font-medium text-white hover:text-red-200 hover:bg-red-600 transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-red-300 focus:ring-opacity-50"
            role="menuitem"
          >
            Vladimir
          </NuxtLink>
        </div>
      </div>
    </Transition>

    <!-- Mobile Overlay -->
    <div
      v-if="isMobileMenuOpen"
      @click="closeMobileMenu"
      class="fixed inset-0 bg-black bg-opacity-25 z-20 md:hidden"
      aria-hidden="true"
    ></div>
  </nav>
</template>

<script setup>
const route = useRoute()

// Mobile menu state
const isMobileMenuOpen = ref(false)

// Mobile menu methods
const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}

const closeMobileMenu = () => {
  isMobileMenuOpen.value = false
}

// Close mobile menu on route change
watch(() => route.path, () => {
  closeMobileMenu()
})

// Close mobile menu on escape key
onMounted(() => {
  const handleKeydown = (event) => {
    if (event.key === 'Escape' && isMobileMenuOpen.value) {
      closeMobileMenu()
    }
  }
  
  document.addEventListener('keydown', handleKeydown)
  
  onUnmounted(() => {
    document.removeEventListener('keydown', handleKeydown)
  })
})
</script>