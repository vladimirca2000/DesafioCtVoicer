/** @type {import('next').NextConfig} */
const nextConfig = {
  // Configurações para desenvolvimento com HTTPS e CORS
  async rewrites() {
    return [
      // Proxy para API durante desenvolvimento - HTTPS (padrão)
      {
        source: '/api/:path*',
        destination: 'https://localhost:5001/api/:path*',
      },
      // Proxy alternativo para HTTP (fallback)
      {
        source: '/api-http/:path*',
        destination: 'http://localhost:5000/api/:path*',
      },
    ];
  },
  
  async headers() {
    return [
      {
        source: '/(.*)',
        headers: [
          {
            key: 'Access-Control-Allow-Origin',
            value: '*',
          },
          {
            key: 'Access-Control-Allow-Methods',
            value: 'GET, POST, PUT, DELETE, OPTIONS',
          },
          {
            key: 'Access-Control-Allow-Headers',
            value: 'Content-Type, Authorization',
          },
        ],
      },
    ];
  },

  // Configurações de imagem para permitir domínios externos se necessário
  images: {
    domains: ['localhost'],
    unoptimized: false, // Mantém otimização de imagens
    formats: ['image/webp', 'image/avif'], // Formatos modernos
    // Configuração para reduzir warnings de dimensões
    imageSizes: [16, 32, 48, 64, 96, 128, 256, 384],
    deviceSizes: [640, 750, 828, 1080, 1200, 1920, 2048, 3840],
  },

  // Configurações para desenvolvimento
  webpack: (config, { dev, isServer }) => {
    if (dev && !isServer) {
      // Filtrar warnings específicos do webpack
      config.infrastructureLogging = {
        level: 'error',
      };
      
      // Configurar filtros de warnings
      config.ignoreWarnings = [
        /Critical dependency/,
        /Module not found/,
        /Failed to parse source map/,
      ];
    }
    return config;
  },

  // Reduzir warnings de desenvolvimento
  compiler: {
    // Remove console.log em produção
    removeConsole: process.env.NODE_ENV === 'production',
  },

  // Experimental features para melhor performance
  experimental: {
    optimizeCss: true,
    optimizeServerReact: true,
  },

  // Configurações de otimização
  swcMinify: true,
  
  // Suprimir alguns warnings específicos
  typescript: {
    ignoreBuildErrors: false,
  },
  
  eslint: {
    ignoreDuringBuilds: false,
  },

  // Configuração para abrir automaticamente o navegador
  devIndicators: {
    buildActivity: true,
    buildActivityPosition: 'bottom-right',
  },
};

module.exports = nextConfig;
