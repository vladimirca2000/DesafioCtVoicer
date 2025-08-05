// src/app/page.tsx
import Image from 'next/image'; // Importa o componente Image do Next.js

export default function Home() {
  return (
    <div className="container mx-auto px-4 py-8 flex flex-col items-center justify-center min-h-screen-minus-navbar-footer">
      <div className="bg-gray-100 p-8 rounded-lg shadow-md text-center">
        <h1 className="text-4xl font-bold mb-4">Vladimir</h1>
        <p className="text-lg text-gray-700 mb-8">Fã de Formula 1</p>
        {/* Logo da Fórmula 1 com otimizações */}
        <Image
          src="/image/f1-logo.png"
          alt="Logo da Formula 1"
          width={250}
          height={150}
          priority={true} // Adiciona prioridade para LCP
          className="mx-auto"
          style={{
            width: 'auto',
            height: 'auto'
          }}
        />
      </div>
    </div>
  );
}