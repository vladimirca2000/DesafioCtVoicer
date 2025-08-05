// src/app/layout.tsx
import type { Metadata } from 'next';
import { Inter } from 'next/font/google';
import { Providers } from './providers'; // Importar o Providers
import './globals.css';
import Navbar from '@/components/Navbar'; // Importar o Navbar
import Footer from '@/components/Footer'; // Importar o Footer (será criado a seguir)
import ErrorBoundary from '@/components/ErrorBoundary'; // Importar ErrorBoundary
import ClientLayout from '@/components/ClientLayout'; // Importar ClientLayout
import dynamic from 'next/dynamic';

// Carrega o ChatWidget apenas no lado do cliente
const ChatWidget = dynamic(() => import('@/components/ChatWidget'), {
  ssr: false,
  loading: () => null, // Não mostra loading para o chat widget
});

const inter = Inter({ subsets: ['latin'] });

export const metadata: Metadata = {
  title: 'Meu Projeto de ChatBot', // Título mais descritivo
  description: 'Sistema de Chat com Bot inovador para Vladimir.',
  icons: {
    icon: '/image/chat-icon.png',
    shortcut: '/image/chat-icon.png',
    apple: '/image/chat-icon.png',
  },
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="pt-BR">
      <head>
        <link rel="icon" href="/image/chat-icon.png" sizes="any" />
        <link rel="shortcut icon" href="/image/chat-icon.png" />
        <link rel="apple-touch-icon" href="/image/chat-icon.png" />
      </head>
      <body 
        className={inter.className}
        suppressHydrationWarning={true}
      >
        <ErrorBoundary>
          <Providers>
            <ClientLayout>
              <Navbar />
              {/* Adiciona padding superior para o conteúdo não ficar por baixo do navbar fixo */}
              <main className="pt-16 min-h-screen">
                {children}
              </main>
              <Footer />
              <ChatWidget /> {/* Renderiza o widget de chat em todas as páginas */}
            </ClientLayout>
          </Providers>
        </ErrorBoundary>
      </body>
    </html>
  );
}