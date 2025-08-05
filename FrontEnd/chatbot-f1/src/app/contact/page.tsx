// src/app/contact/page.tsx
import Image from 'next/image';

export default function Contact() {
  return (
    <div className="container mx-auto px-4 py-8 flex flex-col items-center min-h-screen-minus-navbar-footer">
      <h1 className="text-4xl font-bold text-center mb-8">
        Entre em Contato!
      </h1>
      <div className="bg-gray-100 p-8 rounded-lg shadow-md w-full max-w-lg mb-8">
        <p className="text-lg mb-4">
          Nome: Vladimir Carlos Alves
        </p>
        <p className="text-lg mb-4">
          WhatsApp: <a href="https://wa.me/+5535920014611"  rel="noopener noreferrer" className="text-blue-600 hover:underline">{`+55 (35) 92001-4611`}</a>
        </p>
        <p className="text-lg mb-4">
          E-mail: <a href="mailto:vladimirca2000@gmail.com"  rel="noopener noreferrer" className="text-blue-600 hover:underline">vladimirca2000@gmail.com</a>
        </p>
        <p className="text-lg mb-4">
          LinkedIn: <a href="https://www.linkedin.com/in/vladimirca2000/"  rel="noopener noreferrer" className="text-blue-600 hover:underline">linkedin.com/in/vladimirca2000</a>
        </p>
        <p className="text-lg mb-4">
          GitHub: <a href="https://github.com/vladimirca2000"  rel="noopener noreferrer" className="text-blue-600 hover:underline">github.com/vladimirca2000</a>
        </p>
      </div>

      <h2 className="text-3xl font-bold text-center mb-4">Nossa Localização</h2>
      <div className="w-full max-w-lg h-80 bg-gray-200 rounded-lg overflow-hidden shadow-md">
        {/* Exemplo de mapa estático - substitua pela URL do seu mapa */}
        <img
          src="https://via.placeholder.com/600x400?text=Mapa+da+Localizacao"
          alt="Mapa da localização"
          className="w-full h-full object-cover"
        />
        {/* Para um mapa mais real, você pode usar um iframe do Google Maps, por exemplo: */}
        {/*
        <iframe
          src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3...YOUR_EMBED_CODE...!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x...YOUR_LOCATION_COORDS...!2z...YOUR_LOCATION_NAME...!5e0!3m2!1spt-BR!2sbr!4v..."
          width="100%"
          height="100%"
          style={{ border: 0 }}
          allowFullScreen
          loading="lazy"
          referrerPolicy="no-referrer-when-downgrade"
        ></iframe>
        */}
      </div>
    </div>
  );
}