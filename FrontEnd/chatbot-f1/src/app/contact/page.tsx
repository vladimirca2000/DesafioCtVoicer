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
    </div>
  );
}