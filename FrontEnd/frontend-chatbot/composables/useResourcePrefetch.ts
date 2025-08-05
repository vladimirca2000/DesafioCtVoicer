export default function useResourcePrefetch() {
  const prefetched = new Set()

  const prefetchResource = (href, type = 'prefetch') => {
    if (typeof window === 'undefined' || prefetched.has(href)) return

    const link = document.createElement('link')
    link.rel = type // 'prefetch', 'preload', 'dns-prefetch', 'preconnect'
    link.href = href
    
    // Set appropriate attributes based on resource type
    if (href.endsWith('.css')) {
      link.as = 'style'
    } else if (href.endsWith('.js')) {
      link.as = 'script'
    } else if (href.match(/\.(jpg|jpeg|png|webp|gif|svg)$/)) {
      link.as = 'image'
    } else if (href.endsWith('.woff2') || href.endsWith('.woff')) {
      link.as = 'font'
      link.crossOrigin = 'anonymous'
    }

    document.head.appendChild(link)
    prefetched.add(href)
  }

  const prefetchPage = (route) => {
    if (typeof window === 'undefined') return

    // Prefetch page assets
    const routeAssets = getRouteAssets(route)
    routeAssets.forEach(asset => prefetchResource(asset, 'prefetch'))
  }

  const preloadCritical = (resources) => {
    resources.forEach(resource => {
      prefetchResource(resource, 'preload')
    })
  }

  const preconnectDomains = (domains) => {
    domains.forEach(domain => {
      prefetchResource(domain, 'preconnect')
    })
  }

  const dnsPrefetch = (domains) => {
    domains.forEach(domain => {
      prefetchResource(domain, 'dns-prefetch')
    })
  }

  // Helper function to get route assets (would need to be customized per app)
  const getRouteAssets = (route) => {
    const assetMap = {
      '/': ['/critical.css', '/homepage.js'],
      '/contact': ['/contact.css', '/maps.js'],
      '/about': ['/about.css']
    }
    return assetMap[route] || []
  }

  return {
    prefetchResource,
    prefetchPage,
    preloadCritical,
    preconnectDomains,
    dnsPrefetch
  }
}
