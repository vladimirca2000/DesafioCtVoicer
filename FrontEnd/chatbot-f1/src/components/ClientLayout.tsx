'use client';

import { useDevWarnings } from '@/hooks/useDevWarnings';

export default function ClientLayout({ children }: { children: React.ReactNode }) {
  // Usar o hook para gerenciar warnings de desenvolvimento
  useDevWarnings();

  return <>{children}</>;
}
