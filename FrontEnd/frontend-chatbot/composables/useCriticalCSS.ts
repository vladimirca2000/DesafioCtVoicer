export default function useCriticalCSS() {
  const loadedStyles = new Set()

  const loadCriticalCSS = (criticalCSS) => {
    if (typeof window === 'undefined' || !criticalCSS) return

    const style = document.createElement('style')
    style.textContent = criticalCSS
    style.setAttribute('data-critical', 'true')
    document.head.appendChild(style)
  }

  const loadNonCriticalCSS = (href) => {
    if (typeof window === 'undefined' || loadedStyles.has(href)) return

    // Load non-critical CSS asynchronously
    const link = document.createElement('link')
    link.rel = 'stylesheet'
    link.href = href
    link.media = 'print' // Load with low priority
    link.onload = function() {
      link.media = 'all' // Apply styles once loaded
    }

    document.head.appendChild(link)
    loadedStyles.add(href)

    // Fallback for browsers that don't support onload
    setTimeout(() => {
      if (link.media !== 'all') {
        link.media = 'all'
      }
    }, 3000)
  }

  const inlineSmallCSS = (css) => {
    if (typeof window === 'undefined') return

    const style = document.createElement('style')
    style.textContent = css
    document.head.appendChild(style)
  }

  const extractCriticalCSS = (content) => {
    // This would typically be done at build time
    // For runtime, we can inline essential styles
    return `
      /* Critical CSS for above-the-fold content */
      .animate-fade-in-up {
        opacity: 0;
        animation: fadeInUp 0.8s ease-out forwards;
      }
      
      @keyframes fadeInUp {
        from {
          opacity: 0;
          transform: translateY(30px);
        }
        to {
          opacity: 1;
          transform: translateY(0);
        }
      }
      
      .lazy-loading {
        opacity: 0;
        filter: blur(5px);
        transition: opacity 0.3s, filter 0.3s;
      }
      
      .lazy-loaded {
        opacity: 1;
        filter: blur(0);
      }
    `
  }

  return {
    loadCriticalCSS,
    loadNonCriticalCSS,
    inlineSmallCSS,
    extractCriticalCSS
  }
}
