<template>
  <Head>
    <!-- JSON-LD Structured Data -->
    <script
      type="application/ld+json"
      :innerHTML="JSON.stringify(structuredData)"
    />
  </Head>
</template>

<script setup>
const props = defineProps({
  type: {
    type: String,
    default: 'website',
    validator: (value) => ['website', 'person', 'article', 'service'].includes(value)
  },
  data: {
    type: Object,
    default: () => ({})
  }
})

const structuredData = computed(() => {
  const baseData = {
    '@context': 'https://schema.org'
  }

  switch (props.type) {
    case 'person':
      return {
        ...baseData,
        '@type': 'Person',
        name: 'Vladimir Carlos Alves',
        jobTitle: 'Desenvolvedor Full Stack',
        description: 'Desenvolvedor especializado em soluções web modernas com foco em Vue.js, Nuxt.js e .NET',
        url: 'https://vladimircarlosalves.dev',
        sameAs: [
          'https://github.com/vladimircarlosalves',
          'https://linkedin.com/in/vladimircarlosalves'
        ],
        knowsAbout: [
          'Vue.js',
          'Nuxt.js',
          'TypeScript',
          '.NET',
          'JavaScript',
          'Web Development',
          'Frontend Development',
          'Backend Development'
        ],
        ...props.data
      }

    case 'website':
      return {
        ...baseData,
        '@type': 'WebSite',
        name: 'Vladimir Carlos Alves - Desenvolvedor Full Stack',
        description: 'Portfolio e blog de Vladimir Carlos Alves, desenvolvedor especializado em tecnologias web modernas',
        url: 'https://vladimircarlosalves.dev',
        author: {
          '@type': 'Person',
          name: 'Vladimir Carlos Alves'
        },
        potentialAction: {
          '@type': 'SearchAction',
          target: 'https://vladimircarlosalves.dev/search?q={search_term_string}',
          'query-input': 'required name=search_term_string'
        },
        ...props.data
      }

    case 'service':
      return {
        ...baseData,
        '@type': 'Service',
        name: 'Desenvolvimento Web Full Stack',
        description: 'Serviços de desenvolvimento web especializado em Vue.js, Nuxt.js, .NET e tecnologias modernas',
        provider: {
          '@type': 'Person',
          name: 'Vladimir Carlos Alves'
        },
        areaServed: 'Brasil',
        serviceType: 'Desenvolvimento de Software',
        category: 'Web Development',
        ...props.data
      }

    case 'article':
      return {
        ...baseData,
        '@type': 'Article',
        author: {
          '@type': 'Person',
          name: 'Vladimir Carlos Alves'
        },
        publisher: {
          '@type': 'Person',
          name: 'Vladimir Carlos Alves'
        },
        ...props.data
      }

    default:
      return {
        ...baseData,
        '@type': 'WebPage',
        ...props.data
      }
  }
})
</script>
