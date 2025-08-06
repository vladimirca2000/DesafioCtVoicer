'use client';

import { useDevWarnings } from '@/hooks/useDevWarnings';

export default function ClientLayout({ children }: { children: React.ReactNode }) {
  useDevWarnings();

  return <>{children}</>;
}
