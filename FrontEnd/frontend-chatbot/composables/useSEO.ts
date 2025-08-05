// composables/useSEO.ts
export const useSEO = () => {
  const route = useRoute()
  const appStore = useAppStore()

  // Base SEO configuration
  const baseSEO = {
    siteName: 'Vladimir Carlos Alves - Desenvolvedor & Fã de F1',
    siteUrl: 'https://vladimir-carlos-alves.dev', // Update with your domain
    description: 'Desenvolvedor Full Stack apaixonado por tecnologia e Formula 1. Especializado em Vue.js, Nuxt.js, TypeScript e .NET.',
    image: '/images/og-image.jpg', // Add this image to public folder
    twitterHandle: '@vladimir_dev'
  }

  // Dynamic SEO for pages
  const setSEO = (options: {
    title?: string
    description?: string
    image?: string
    type?: string
    keywords?: string[]
    canonical?: string
    noindex?: boolean
  } = {}) => {
    const title = options.title 
      ? `${options.title} | ${baseSEO.siteName}`
      : baseSEO.siteName

    const description = options.description || baseSEO.description
    const image = options.image || baseSEO.image
    const url = `${baseSEO.siteUrl}${route.path}`
    const canonical = options.canonical || url

    useHead({
      title,
      meta: [
        // Basic meta tags
        { name: 'description', content: description },
        { name: 'keywords', content: options.keywords?.join(', ') || 'desenvolvedor, formula 1, vue.js, nuxt.js, typescript, frontend, backend' },
        { name: 'author', content: appStore.contactInfo.name },
        { name: 'robots', content: options.noindex ? 'noindex,nofollow' : 'index,follow' },
        
        // Open Graph
        { property: 'og:site_name', content: baseSEO.siteName },
        { property: 'og:title', content: title },
        { property: 'og:description', content: description },
        { property: 'og:image', content: image },
        { property: 'og:url', content: url },
        { property: 'og:type', content: options.type || 'website' },
        { property: 'og:locale', content: 'pt_BR' },
        
        // Twitter Card
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:site', content: baseSEO.twitterHandle },
        { name: 'twitter:creator', content: baseSEO.twitterHandle },
        { name: 'twitter:title', content: title },
        { name: 'twitter:description', content: description },
        { name: 'twitter:image', content: image },
        
        // Additional meta
        { name: 'format-detection', content: 'telephone=no' },
        { name: 'theme-color', content: '#E10600' },
        { name: 'msapplication-TileColor', content: '#E10600' },
        { name: 'apple-mobile-web-app-capable', content: 'yes' },
        { name: 'apple-mobile-web-app-status-bar-style', content: 'default' },
      ],
      link: [
        { rel: 'canonical', href: canonical },
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
        { rel: 'apple-touch-icon', href: '/images/apple-touch-icon.png' },
        { rel: 'manifest', href: '/manifest.json' }
      ]
    })
  }

  // Structured data for rich snippets
  const setStructuredData = (type: 'Person' | 'WebSite' | 'Organization', data: any) => {
    const structuredData = {
      '@context': 'https://schema.org',
      '@type': type,
      ...data
    }

    useHead({
      script: [
        {
          type: 'application/ld+json',
          children: JSON.stringify(structuredData)
        }
      ]
    })
  }

  // Person structured data for about pages
  const setPersonStructuredData = () => {
    setStructuredData('Person', {
      name: appStore.contactInfo.name,
      jobTitle: 'Desenvolvedor Full Stack',
      description: 'Desenvolvedor especializado em Vue.js, Nuxt.js, TypeScript e .NET',
      image: `${baseSEO.siteUrl}/images/profile.jpg`,
      url: baseSEO.siteUrl,
      sameAs: [
        `https://${appStore.contactInfo.linkedin}`,
        `https://${appStore.contactInfo.github}`,
      ],
      knowsAbout: ['Vue.js', 'Nuxt.js', 'TypeScript', '.NET', 'Formula 1'],
      email: appStore.contactInfo.email,
      telephone: appStore.contactInfo.phone,
      address: {
        '@type': 'PostalAddress',
        addressCountry: 'BR',
        addressLocality: 'Brasil'
      }
    })
  }

  // Website structured data
  const setWebsiteStructuredData = () => {
    setStructuredData('WebSite', {
      name: baseSEO.siteName,
      url: baseSEO.siteUrl,
      description: baseSEO.description,
      author: {
        '@type': 'Person',
        name: appStore.contactInfo.name
      },
      potentialAction: {
        '@type': 'SearchAction',
        target: `${baseSEO.siteUrl}/search?q={search_term_string}`,
        'query-input': 'required name=search_term_string'
      }
    })
  }

  return {
    setSEO,
    setStructuredData,
    setPersonStructuredData,
    setWebsiteStructuredData,
    baseSEO
  }
}
