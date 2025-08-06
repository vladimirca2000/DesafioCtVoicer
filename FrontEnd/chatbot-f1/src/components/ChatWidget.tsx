'use client';

import React, { useState, useEffect, useRef } from 'react';
import { Send } from 'lucide-react';
import { useAppSelector, useAppDispatch, setUser, setChatSession, addMessage, setChatStatus, setChatError, clearChat, clearUser, Message } from '@/store/store';
import Image from 'next/image';

import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import apiClient from '@/lib/api';
import { createSignalRConnection, signalR } from '@/lib/signalr';

const SIGNALR_HUB_URL = process.env.NEXT_PUBLIC_SIGNALR_HUB_URL;

export default function ChatWidget() {
  const dispatch = useAppDispatch();
  const { id: userId, name: userName, isAuthenticated } = useAppSelector((state) => state.user);
  const { sessionId, messages, status, error } = useAppSelector((state) => state.chat);

  const [isChatOpen, setIsChatOpen] = useState(false);
  const [emailInput, setEmailInput] = useState('');
  const [nameInput, setNameInput] = useState('');
  const [messageInput, setMessageInput] = useState('');
  const [isRegistering, setIsRegistering] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const messageInputRef = useRef<HTMLInputElement>(null);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  useEffect(() => {
    if (status === 'open' && isChatOpen) {
      setTimeout(() => {
        messageInputRef.current?.focus();
      }, 100);
    }
  }, [status, isChatOpen, sessionId, isAuthenticated, userId, userName]);

  useEffect(() => {
    if (isChatOpen && sessionId && !connectionRef.current) {
      const newConnection = createSignalRConnection();
      
      if (!newConnection) {
        dispatch(setChatError('Erro ao configurar conexão de chat em tempo real.'));
        return;
      }

      newConnection.on('ReceiveMessage', (message: Message) => {
        dispatch(addMessage(message));
      });

      newConnection.on('ChatSessionEnded', (data: { chatSessionId: string; reason: string }) => {
        dispatch(addMessage({
          id: `system-${Date.now()}`,
          chatSessionId: data.chatSessionId,
          userId: null,
          content: `Sessão encerrada: ${data.reason}`,
          isFromBot: true,
          sentAt: new Date().toISOString(),
        }));
        
        dispatch(clearChat());
        dispatch(clearUser());
        setIsChatOpen(false);
      });

      newConnection.start()
        .then(() => {
          if (sessionId) {
            newConnection.invoke('JoinChat', sessionId).catch(err => console.error('❌ Erro ao juntar ao grupo:', err));
          }
        })
        .catch(err => {
          dispatch(setChatError('Erro ao conectar ao chat em tempo real.'));
        });

      connectionRef.current = newConnection;
    } else if (!isChatOpen && connectionRef.current) {
      connectionRef.current.stop().then(() => {
        connectionRef.current = null;
      });
    }

    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop();
        connectionRef.current = null;
      }
    };
  }, [isChatOpen, sessionId, dispatch]);

  const handleOpenChat = () => {
    setIsChatOpen(true);
    if (!isAuthenticated) {
      dispatch(setChatStatus('authenticating'));
    } else if (!sessionId) {
      checkOrStartChatSession(userId!, userName).catch((error) => {
        dispatch(setChatError('Erro ao iniciar sessão de chat.'));
      });
      dispatch(setChatStatus('open'));
    } else {
      dispatch(setChatStatus('open'));
      setTimeout(() => {
        messageInputRef.current?.focus();
      }, 100);
    }
  };

  const handleCloseChat = () => {
    setIsChatOpen(false);
    dispatch(setChatStatus('closed'));
    dispatch(setChatError(null));
  };

  const handleEmailSubmit = async () => {
    dispatch(setChatError(null));
    if (!emailInput) {
      dispatch(setChatError('Por favor, insira um e-mail.'));
      return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(emailInput)) {
      dispatch(setChatError('Por favor, insira um e-mail válido.'));
      return;
    }

    try {
      const checkEmailResponse = await apiClient.get(`/Users/by-email?email=${encodeURIComponent(emailInput)}`);
      const userData = checkEmailResponse.data;
      
      if (!userData) {
        dispatch(setChatError('Dados do usuário não encontrados na resposta.'));
        return;
      }
      
      if (!userData.isActive) {
        dispatch(setChatStatus('registering'));
        dispatch(setChatError('E-mail encontrado mas não está ativo. Por favor, insira seu nome para reativar.'));
        return;
      }

      const userIdRaw = userData.id || userData.userId || userData.ID || userData.UserId;
      const userId = userIdRaw ? String(userIdRaw) : null;
      
      if (!userId || userId === 'null' || userId === 'undefined') {
        dispatch(setChatError('Erro: ID do usuário não retornado pela API.'));
        return;
      }
      
      try {
        dispatch(setUser({ 
          id: userId,
          name: userData.name, 
          email: userData.email 
        }));
      } catch (reduxError: any) {
        dispatch(setChatError('Erro interno. Tente novamente.'));
        return;
      }
      
      try {
        await checkOrStartChatSession(userId, userData.name);
      } catch (sessionError: any) {
        dispatch(setChatError('Erro ao verificar sessão de chat.'));
        dispatch(setChatStatus('open'));
      }
    } catch (apiError: any) {
      if (apiError.response && apiError.response.status === 404) {
        dispatch(setChatStatus('registering'));
        const apiMessage = apiError.response?.data?.message || apiError.response?.data?.title;
        dispatch(setChatError(apiMessage || 'E-mail não encontrado. Por favor, insira seu nome para se cadastrar.'));
      } else if (apiError.response && apiError.response.data) {
        let errorMessage = '';
        
        if (apiError.response.data.errors) {
          const errors = Object.values(apiError.response.data.errors).flat();
          errorMessage = errors.join(', ');
        } else if (apiError.response.data.message) {
          errorMessage = apiError.response.data.message;
        } else if (apiError.response.data.title) {
          errorMessage = apiError.response.data.title;
        }
        
        dispatch(setChatError(errorMessage));
      } else {
        dispatch(setChatError('Erro de conexão. Tente novamente.'));
      }
    }
  };

  const handleRegisterUser = async () => {
    if (isRegistering) {
      return;
    }
    
    setIsRegistering(true);
    dispatch(setChatError(null));
    
    if (!nameInput) {
      dispatch(setChatError('Por favor, insira um nome.'));
      setIsRegistering(false);
      return;
    }

    try {
      const registerResponse = await apiClient.post(`/Users`, {
        name: nameInput,
        email: emailInput,
        isActive: true,
      });
      
      if (registerResponse.status !== 201) {
      }
      
      const userData = registerResponse.data;
      
      const userIdRaw = userData.id || userData.userId || userData.ID || userData.UserId;
      const userId = userIdRaw ? String(userIdRaw) : null;
      
      if (!userId || userId === 'null' || userId === 'undefined') {
        dispatch(setChatError('Erro: ID do usuário não retornado pela API.'));
        setIsRegistering(false);
        return;
      }
      
      dispatch(setUser({ 
        id: userId,
        name: userData.name, 
        email: userData.email 
      }));
      
      await new Promise(resolve => setTimeout(resolve, 50));
      
      try {
        await checkOrStartChatSession(userId, userData.name);
        
        setEmailInput('');
        setNameInput('');
        
      } catch (sessionError: any) {
        dispatch(setChatError('Erro ao iniciar sessão de chat para novo usuário.'));
        dispatch(setChatStatus('open'));
      }
      
      setIsRegistering(false);
      
    } catch (apiError: any) {
      if (apiError.response && apiError.response.data) {
        let errorMessage = '';
        
        if (apiError.response.data.errors) {
          const errors = Object.values(apiError.response.data.errors).flat();
          errorMessage = errors.join(', ');
        } else if (apiError.response.data.message) {
          errorMessage = apiError.response.data.message;
        } else if (apiError.response.data.title) {
          errorMessage = apiError.response.data.title;
        }
        
        dispatch(setChatError(errorMessage));
      } else {
        dispatch(setChatError('Erro de conexão. Tente novamente.'));
      }
      
      setIsRegistering(false);
    }
  };

  const checkOrStartChatSession = async (currentUserId: string, currentUserName: string | null) => {
    console.log('🔍 === INICIO checkOrStartChatSession ===');
    console.log('🔍 Parâmetros recebidos:', { currentUserId, currentUserName });
    console.log('🔍 Tipo do currentUserId:', typeof currentUserId);
    console.log('🔍 currentUserId é null?', currentUserId === null);
    console.log('🔍 currentUserId é undefined?', currentUserId === undefined);
    if (!currentUserId || currentUserId === 'null' || currentUserId === 'undefined') {
      dispatch(setChatError('ID do usuário inválido.'));
      return;
    }
    
    dispatch(setChatError(null));
    
    try {
      try {
        const activeSessionResponse = await apiClient.get(`/Chat/active-session?userId=${currentUserId}`);
        
        const sessionData = activeSessionResponse.data;
        
        if (sessionData && sessionData.chatSessionId) {
          dispatch(setChatSession({ sessionId: sessionData.chatSessionId }));
          
          try {
            const historyResponse = await apiClient.get(`/Chat/history?chatSessionId=${sessionData.chatSessionId}`);
            const messages = historyResponse.data || [];
            
            messages.forEach((msg: any) => {
              dispatch(addMessage({
                id: msg.messageId || msg.id,
                chatSessionId: sessionData.chatSessionId,
                userId: msg.userId,
                content: msg.content,
                isFromBot: msg.isFromBot || msg.messageType === 'Bot',
                sentAt: msg.sentAt || msg.createdAt,
              }));
            });
          } catch (historyError: any) {
          }
          
          return;
        }
      } catch (sessionError: any) {
        if (sessionError.response?.status !== 404) {
        }
      }
      
      try {
        await startChatSession(currentUserId, currentUserName, "Olá! Como posso ajudar você hoje?");
      } catch (startSessionError: any) {
        let errorMessage = 'Erro ao iniciar nova sessão de chat.';
        
        if (startSessionError.response && startSessionError.response.data) {
          if (startSessionError.response.data.message) {
            errorMessage = startSessionError.response.data.message;
          } else if (startSessionError.response.data.title) {
            errorMessage = startSessionError.response.data.title;
          } else if (startSessionError.response.data.errors) {
            const errors = Object.values(startSessionError.response.data.errors).flat();
            errorMessage = errors.join(', ');
          }
        }
        
        dispatch(setChatError(errorMessage));
      }
      
    } catch (generalError: any) {
      let errorMessage = 'Erro ao verificar sessão de chat.';
      
      if (generalError.response && generalError.response.data) {
        if (generalError.response.data.message) {
          errorMessage = generalError.response.data.message;
        } else if (generalError.response.data.title) {
          errorMessage = generalError.response.data.title;
        } else if (generalError.response.data.errors) {
          const errors = Object.values(generalError.response.data.errors).flat();
          errorMessage = errors.join(', ');
        }
      }
      
      dispatch(setChatError(errorMessage));
    }
    
    console.log('🔍 === FIM checkOrStartChatSession ===');
  };

  const startChatSession = async (currentUserId: string, currentUserName: string | null, initialMessageContent: string = "Olá, como posso ajudar?") => {
    console.log('🚀 === INICIO startChatSession ===');
    console.log('🚀 Parâmetros recebidos:', { currentUserId, currentUserName, initialMessageContent });
    
    if (!currentUserId) {
      console.error('❌ currentUserId é inválido para startChatSession:', currentUserId);
      dispatch(setChatError('ID do usuário inválido para iniciar sessão.'));
      return;
    }
    
    dispatch(setChatError(null));
    
    try {
      console.log('📤 Enviando requisição para start-session...');
      const sessionResponse = await apiClient.post(`/Chat/start-session`, {
        userId: currentUserId,
        userName: currentUserName,
        initialMessageContent: initialMessageContent,
      });
      
      console.log('📥 Resposta do start-session:', sessionResponse.data);
      const sessionData = sessionResponse.data;
      
      if (!sessionData || !sessionData.chatSessionId) {
        console.error('❌ Dados da sessão inválidos:', sessionData);
        throw new Error('Dados da sessão inválidos retornados pelo servidor');
      }
      
      console.log('✅ Sessão criada com sucesso:', sessionData.chatSessionId);
      console.log('🔄 Salvando sessão no Redux...');
      dispatch(setChatSession({ sessionId: sessionData.chatSessionId }));
      console.log('✅ Sessão salva no Redux com ID:', sessionData.chatSessionId);
      
      if (sessionData.messageId && sessionData.initialMessage) {
        console.log('💬 Adicionando mensagem inicial:', sessionData.initialMessage);
        dispatch(addMessage({
          id: sessionData.messageId,
          chatSessionId: sessionData.chatSessionId,
          userId: sessionData.userId,
          content: sessionData.initialMessage,
          isFromBot: false,
          sentAt: sessionData.startedAt,
        }));
        console.log('✅ Mensagem inicial adicionada');
      } else {
        console.log('ℹ️ Nenhuma mensagem inicial retornada pelo backend');
      }
      
      console.log('📡 Conectando ao grupo SignalR...', sessionData.chatSessionId);
      if (connectionRef.current && connectionRef.current.state === signalR.HubConnectionState.Connected) {
        try {
          await connectionRef.current.invoke('JoinChat', sessionData.chatSessionId);
          console.log('✅ Conectado ao grupo SignalR');
        } catch (signalRError: any) {
          console.error('❌ Erro ao juntar ao grupo SignalR:', signalRError);
        }
      } else {
        console.warn('⚠️ SignalR não conectado, será conectado quando necessário');
        console.warn('⚠️ Estado da conexão:', connectionRef.current?.state);
      }
      
      console.log('🚀 === FIM startChatSession (sucesso) ===');
      
    } catch (apiError: any) {
      console.error('❌ Erro ao iniciar sessão de chat:', apiError);
      console.error('❌ Detalhes do erro:', {
        status: apiError.response?.status,
        statusText: apiError.response?.statusText,
        data: apiError.response?.data,
        message: apiError.message
      });
      
      if (apiError.response && apiError.response.data) {
        let errorMessage = '';
        
        if (apiError.response.data.errors) {
          const errors = Object.values(apiError.response.data.errors).flat();
          errorMessage = errors.join(', ');
        } else if (apiError.response.data.message) {
          errorMessage = apiError.response.data.message;
        } else if (apiError.response.data.title) {
          errorMessage = apiError.response.data.title;
        }
        
        console.error('❌ Mensagem de erro da API:', errorMessage);
        dispatch(setChatError(errorMessage));
      } else {
        console.error('❌ Erro sem resposta da API');
        dispatch(setChatError('Erro de conexão. Tente novamente.'));
      }
      
      console.log('🚀 === FIM startChatSession (erro) ===');
    }
  };

  const handleSendMessage = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!messageInput.trim() || !sessionId || !userId) return;

    const userMessage: Message = {
      id: crypto.randomUUID(),
      chatSessionId: sessionId,
      userId: userId,
      content: messageInput,
      isFromBot: false,
      sentAt: new Date().toISOString(),
    };

    dispatch(addMessage(userMessage));
    setMessageInput('');

    try {
      if (userMessage.content.toLowerCase().trim() === 'sair') {
        console.log('🚪 Usuário digitou "sair" - encerrando sessão...');
        
        try {
          const endSessionResponse = await apiClient.post(`/Chat/end-session`, {
            chatSessionId: sessionId,
            endReason: "Usuário digitou 'sair'",
          });
          
          console.log('✅ Resposta do end-session:', endSessionResponse.status);
          
          if (endSessionResponse.status === 200) {
            console.log('✅ Sessão encerrada com sucesso - fechando modal');
            
            dispatch(addMessage({
              id: `system-farewell-${Date.now()}`,
              chatSessionId: sessionId,
              userId: null,
              content: 'Sessão encerrada. Até logo! 👋',
              isFromBot: true,
              sentAt: new Date().toISOString(),
            }));
            
            setTimeout(() => {
              console.log('🚪 Fechando chat e limpando estado...');
              dispatch(clearChat());
              dispatch(clearUser());
              setIsChatOpen(false);
            }, 1500);
            
            return;
          }
        } catch (endSessionError: any) {
          console.error('❌ Erro ao encerrar sessão:', endSessionError);
          let errorMessage = 'Erro ao encerrar sessão.';
          
          if (endSessionError.response && endSessionError.response.data) {
            if (endSessionError.response.data.message) {
              errorMessage = endSessionError.response.data.message;
            } else if (endSessionError.response.data.title) {
              errorMessage = endSessionError.response.data.title;
            }
          }
          
          dispatch(setChatError(errorMessage));
          return;
        }
      }

      console.log('📤 Enviando mensagem normal para o chat...');
      
      await apiClient.post(`/Chat/send-message`, {
        chatSessionId: sessionId,
        userId: userId,
        content: userMessage.content,
        messageType: 1,
      });

      const botResponse = await apiClient.post(`/Bot/process-message`, {
        chatSessionId: sessionId,
        userId: userId,
        userMessage: userMessage.content,
      });
      
      const botMessageData = botResponse.data;
      dispatch(addMessage({
        id: botMessageData.messageId,
        chatSessionId: botMessageData.chatSessionId,
        userId: null,
        content: botMessageData.botMessageContent,
        isFromBot: true,
        sentAt: botMessageData.sentAt,
      }));

    } catch (apiError: any) {
      console.error('Erro ao enviar mensagem:', apiError);
      
      if (apiError.response && apiError.response.data) {
        let errorMessage = '';
        
        if (apiError.response.data.errors) {
          const errors = Object.values(apiError.response.data.errors).flat();
          errorMessage = errors.join(', ');
        } else if (apiError.response.data.message) {
          errorMessage = apiError.response.data.message;
        } else if (apiError.response.data.title) {
          errorMessage = apiError.response.data.title;
        }
        
        console.error('❌ Mensagem de erro da API:', errorMessage);
        dispatch(setChatError(errorMessage));
      } else {
        console.error('❌ Erro sem resposta da API');
        dispatch(setChatError('Erro de conexão. Tente novamente.'));
      }
    }
  };

  return (
    <>
      <button
        className="fixed bottom-4 right-4 bg-red-600 text-white rounded-full p-4 shadow-lg hover:bg-red-700 transition-all duration-300 z-50 hover:shadow-xl hover:scale-105 chat-icon-mobile"
        onClick={handleOpenChat}
        aria-label="Abrir Chat"
      >
        <Image 
          src="/image/chat-icon.png" 
          alt="Chat Icon" 
          width={28} 
          height={28}
          className="w-7 h-7"
          style={{
            width: 'auto',
            height: 'auto'
          }}
        />
      </button>

      <Dialog open={isChatOpen} onOpenChange={handleCloseChat}>
        <DialogContent className="fixed bottom-4 right-4 w-80 h-[500px] md:w-96 md:h-[550px] flex flex-col bg-white rounded-lg shadow-2xl z-50 p-0 overflow-hidden border-2 border-gray-200 sm:bottom-20 sm:right-4 chat-widget-mobile chat-widget-enter">
          <DialogHeader className="bg-gradient-to-r from-red-600 to-red-700 text-white p-4 shadow-lg">
            <DialogTitle className="text-lg font-bold flex items-center gap-2">
              🏎️ Chat F1 Bot
              {sessionId && (
                <span className="ml-auto text-xs bg-green-500 px-2 py-1 rounded-full status-online">
                  Online
                </span>
              )}
            </DialogTitle>
          </DialogHeader>

          {error && <p className="text-red-500 mt-2 p-4 text-center text-sm">{error}</p>}

          <div className="flex-1 overflow-y-auto p-4 space-y-3 text-sm bg-gray-50 chat-scroll chat-messages-mobile">
            {status === 'authenticating' && (
              <div className="text-center bg-white p-6 rounded-lg shadow-sm border">
                <div className="mb-4">
                  <h3 className="text-lg font-semibold text-gray-800 mb-2">Bem-vindo!</h3>
                  <p className="text-gray-600">Para começar, por favor insira seu e-mail:</p>
                </div>
                <Input
                  type="email"
                  placeholder="seu.email@example.com"
                  value={emailInput}
                  onChange={(e) => setEmailInput(e.target.value)}
                  className="mb-3"
                  onKeyPress={(e) => e.key === 'Enter' && handleEmailSubmit()}
                />
                <Button 
                  onClick={handleEmailSubmit} 
                  className="w-full bg-red-600 hover:bg-red-700"
                  disabled={!emailInput}
                >
                  Continuar
                </Button>
              </div>
            )}

            {status === 'registering' && (
              <div className="text-center bg-white p-6 rounded-lg shadow-sm border">
                <div className="mb-4">
                  <h3 className="text-lg font-semibold text-gray-800 mb-2">Novo Usuário</h3>
                  <p className="text-gray-600 text-sm">
                    {error?.includes('não está ativo') 
                      ? 'Conta encontrada mas inativa. Insira seu nome para reativar:'
                      : 'E-mail não encontrado. Insira seu nome para se cadastrar:'
                    }
                  </p>
                </div>
                <Input
                  type="text"
                  placeholder="Seu Nome Completo"
                  value={nameInput}
                  onChange={(e) => setNameInput(e.target.value)}
                  className="mb-3"
                  disabled={isRegistering}
                  onKeyPress={(e) => e.key === 'Enter' && !isRegistering && handleRegisterUser()}
                />
                <Button 
                  onClick={handleRegisterUser} 
                  className="w-full bg-red-600 hover:bg-red-700"
                  disabled={!nameInput || isRegistering}
                >
                  {isRegistering 
                    ? '⏳ Registrando...'
                    : (error?.includes('não está ativo') ? 'Reativar Conta' : 'Cadastrar e Iniciar Chat')
                  }
                </Button>
              </div>
            )}

            {status === 'open' && (
              <>
                {messages.length === 0 && !error && (
                  <div className="text-center text-gray-500 py-8">
                    <p className="mb-2">👋 Bem-vindo ao Chat!</p>
                    <p className="text-sm">Inicie a conversa digitando uma mensagem abaixo.</p>
                  </div>
                )}
                {[...messages]
                  .sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime())
                  .map((msg) => (
                  <div
                    key={msg.id}
                    className={`flex ${msg.isFromBot ? 'justify-start' : 'justify-end'} mb-3 chat-message-enter`}
                  >
                    <div
                      className={`max-w-[75%] p-3 rounded-lg shadow-sm chat-message-mobile ${
                        msg.isFromBot 
                          ? 'bg-gray-100 text-gray-800 border border-gray-200 chat-message-bot' 
                          : 'bg-red-600 text-white chat-message-user'
                      }`}
                    >
                      <div className="flex items-center justify-between mb-1">
                        <p className="font-semibold text-xs">
                          {msg.isFromBot ? '🤖 Bot F1' : `👤 ${userName || 'Você'}`}
                        </p>
                        <span className="text-xs opacity-75">
                          {new Date(msg.sentAt).toLocaleTimeString([], { 
                            hour: '2-digit', 
                            minute: '2-digit' 
                          })}
                        </span>
                      </div>
                      <p className="leading-relaxed">{msg.content}</p>
                    </div>
                  </div>
                ))}
                <div ref={messagesEndRef} />
              </>
            )}
          </div>

          {status === 'open' && (
            <DialogFooter className="p-4 border-t border-gray-200 bg-gray-50">
              <form onSubmit={handleSendMessage} className="flex w-full space-x-2">
                <Input
                  ref={messageInputRef}
                  type="text"
                  placeholder="Digite sua mensagem... (ou 'sair' para encerrar)"
                  value={messageInput}
                  onChange={(e) => setMessageInput(e.target.value)}
                  className="flex-1 bg-white border-gray-300 focus:border-red-500 focus:ring-red-500 chat-input"
                  disabled={!sessionId || !userId}
                />
                <Button 
                  type="submit" 
                  className="bg-red-600 hover:bg-red-700 px-4"
                  disabled={!messageInput.trim() || !sessionId || !userId}
                >
                  <Send size={18} />
                </Button>
              </form>
            </DialogFooter>
          )}
        </DialogContent>
      </Dialog>
    </>
  );
}