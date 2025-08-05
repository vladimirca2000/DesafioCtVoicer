// src/store/store.ts
import { configureStore, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, TypedUseSelectorHook, useSelector } from 'react-redux';

// Definições de interfaces para os estados das slices
interface UserState {
  id: string | null;
  name: string | null;
  email: string | null;
  isAuthenticated: boolean;
}

export interface Message { // Exportado para ser usado em ChatWidget
  id: string;
  chatSessionId: string;
  userId: string | null; // Null para mensagens do bot
  content: string;
  isFromBot: boolean;
  sentAt: string; // ISO string
}

interface ChatState {
  sessionId: string | null;
  messages: Message[];
  status: 'closed' | 'open' | 'authenticating' | 'registering';
  error: string | null;
}

// Definição das slices
const userSlice = createSlice({
  name: 'user',
  initialState: {
    id: null,
    name: null,
    email: null,
    isAuthenticated: false,
  } as UserState,
  reducers: {
    setUser: (state, action: PayloadAction<{ id: string; name: string; email: string }>) => {
      state.id = action.payload.id;
      state.name = action.payload.name;
      state.email = action.payload.email;
      state.isAuthenticated = true;
    },
    clearUser: (state) => {
      state.id = null;
      state.name = null;
      state.email = null;
      state.isAuthenticated = false;
    },
  },
});

const chatSlice = createSlice({
  name: 'chat',
  initialState: {
    sessionId: null,
    messages: [],
    status: 'closed',
    error: null,
  } as ChatState,
  reducers: {
    openChat: (state) => {
      state.status = 'open';
    },
    closeChat: (state) => {
      state.status = 'closed';
    },
    setChatSession: (state, action: PayloadAction<{ sessionId: string }>) => {
      state.sessionId = action.payload.sessionId;
      state.messages = []; // Limpa mensagens para nova sessão
      state.status = 'open';
    },
    addMessage: (state, action: PayloadAction<Message>) => {
      state.messages.push(action.payload);
    },
    setChatStatus: (state, action: PayloadAction<ChatState['status']>) => {
      state.status = action.payload;
    },
    setChatError: (state, action: PayloadAction<string | null>) => {
      state.error = action.payload;
    },
    clearChat: (state) => {
        state.sessionId = null;
        state.messages = [];
        state.status = 'closed';
        state.error = null;
    }
  },
});

// Exporta as ações das slices
export const { setUser, clearUser } = userSlice.actions;
export const { openChat, closeChat, setChatSession, addMessage, setChatStatus, setChatError, clearChat } = chatSlice.actions;

// Configura o store do Redux
export const store = configureStore({
  reducer: {
    user: userSlice.reducer,
    chat: chatSlice.reducer,
  },
});

// Tipos inferidos do store para uso em hooks
export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>; // Correção: remova a interface RootState duplicada aqui

// Hooks tipados para uso fácil em componentes
export const useAppDispatch: () => AppDispatch = useDispatch;
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;