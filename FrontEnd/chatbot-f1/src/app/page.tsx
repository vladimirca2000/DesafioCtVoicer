import Image from 'next/image';

export default function Home() {
  return (
    <div className="container mx-auto px-4 py-8 flex flex-col items-center justify-center min-h-screen-minus-navbar-footer">
      <div className="bg-gray-100 p-8 rounded-lg shadow-md text-center">
        <h1 className="text-4xl font-bold mb-4">Vladimir</h1>
        <p className="text-lg text-gray-700 mb-8">Fã de Formula 1</p>
        <Image
          src="/image/f1-logo.png"
          alt="F1 Logo"
          width={200}
          height={120}
          className="mx-auto mb-6"
          priority={true}
        />
        <h2 className="text-3xl font-bold text-red-600 mb-4">🏎️ Chat F1 Bot</h2>
        <p className="text-lg text-gray-700 mb-8">
          Bem-vindo ao nosso chatbot especializado em Fórmula 1! Tire suas dúvidas sobre corridas, pilotos, equipes e muito mais.
        </p>
        <p className="text-sm text-gray-600">
          Clique no ícone de chat no canto inferior direito para começar uma conversa.
        </p>
      </div>
    </div>
  );
}