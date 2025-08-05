export default function useLazyImages() {
  const lazyImages = ref([])
  const imageObserver = ref(null)

  const setupLazyLoading = () => {
    if (typeof window === 'undefined') return

    // Performance: Use modern intersection observer
    imageObserver.value = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            const img = entry.target
            const src = img.dataset.src
            
            if (src) {
              // Performance: Preload image before setting src
              const imageLoader = new Image()
              imageLoader.onload = () => {
                img.src = src
                img.classList.remove('lazy-loading')
                img.classList.add('lazy-loaded')
              }
              imageLoader.src = src
              
              imageObserver.value.unobserve(img)
            }
          }
        })
      },
      {
        // Load images 200px before they enter viewport
        rootMargin: '200px 0px',
        threshold: 0.01
      }
    )

    // Observe all lazy images
    const images = document.querySelectorAll('[data-src]')
    images.forEach((img) => {
      img.classList.add('lazy-loading')
      imageObserver.value.observe(img)
    })
  }

  const destroyLazyLoading = () => {
    if (imageObserver.value) {
      imageObserver.value.disconnect()
    }
  }

  onMounted(() => {
    setupLazyLoading()
  })

  onUnmounted(() => {
    destroyLazyLoading()
  })

  return {
    setupLazyLoading,
    destroyLazyLoading
  }
}
