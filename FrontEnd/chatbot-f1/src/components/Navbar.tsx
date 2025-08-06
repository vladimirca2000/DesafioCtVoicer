'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';

export default function Navbar() {
  const pathname = usePathname();

  const navLinks = [
    { name: 'Home', href: '/' },
    { name: 'Contato', href: '/contact' },
  ];

  return (
    <nav className="bg-red-600 text-white p-4 shadow-md flex justify-between items-center fixed w-full top-0 z-50">
      <div>
        <Link href="/" className="text-xl font-bold hover:underline">
          Vladimir
        </Link>
      </div>
      <div className="flex items-center space-x-4">
        {navLinks.map((link) => (
          <Link
            key={link.name}
            href={link.href}
            className={`text-lg font-semibold hover:underline ${
              pathname === link.href ? 'underline' : ''
            }`}
          >
            {link.name}
          </Link>
        ))}
      </div>
    </nav>
  );
}