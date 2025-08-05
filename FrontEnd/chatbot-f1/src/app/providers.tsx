// src/app/providers.tsx
'use client';

import { Provider } from 'react-redux';
import { store } from '@/store/store'; // Ajuste o caminho conforme necessário

export function Providers({ children }: { children: React.ReactNode }) {
  return <Provider store={store}>{children}</Provider>;
}