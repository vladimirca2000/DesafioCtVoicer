import type { Metadata } from 'next';
import { Inter } from 'next/font/google';
import { Providers } from './providers';
import './globals.css';
import Navbar from '@/components/Navbar';
import Footer from '@/components/Footer';
import ErrorBoundary from '@/components/ErrorBoundary';
import ClientLayout from '@/components/ClientLayout';
import dynamic from 'next/dynamic';

const ChatWidget = dynamic(() => import('@/components/ChatWidget'), {
  ssr: false,
  loading: () => null,
});

const inter = Inter({ subsets: ['latin'] });

export const metadata: Metadata = {
  title: 'Meu Projeto de ChatBot',
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
              <main className="pt-16 min-h-screen">
                {children}
              </main>
              <Footer />
              <ChatWidget />
            </ClientLayout>
          </Providers>
        </ErrorBoundary>
      </body>
    </html>
  );
}