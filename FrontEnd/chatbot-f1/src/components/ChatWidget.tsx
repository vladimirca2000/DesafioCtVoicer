// src/components/ChatWidget.tsx
'use client';

import React, { useState, useEffect, useRef } from 'react';
import { Send } from 'lucide-react'; // Ícones
import { useAppSelector, useAppDispatch, setUser, setChatSession, addMessage, setChatStatus, setChatError, clearChat, Message } from '@/store/store'; // Adicionado 'Message' import
import Image from 'next/image'; // Para usar a imagem customizada

// Importações dos componentes Shadcn UI (assumindo que foram adicionados via npx shadcn-ui add)
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import apiClient from '@/lib/api'; // Cliente axios configurado
import { createSignalRConnection, signalR } from '@/lib/signalr'; // Para SignalR

const SIGNALR_HUB_URL = process.env.NEXT_PUBLIC_SIGNALR_HUB_URL;

export default function ChatWidget() {
  const dispatch = useAppDispatch();
  const { id: userId, name: userName, isAuthenticated } = useAppSelector((state) => state.user);
  const { sessionId, messages, status, error } = useAppSelector((state) => state.chat);

  const [isChatOpen, setIsChatOpen] = useState(false);
  const [emailInput, setEmailInput] = useState('');
  const [nameInput, setNameInput] = useState('');
  const [messageInput, setMessageInput] = useState('');
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const messageInputRef = useRef<HTMLInputElement>(null);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    // Scroll para o final das mensagens
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  useEffect(() => {
    console.log('🔄 Status do chat mudou para:', status);
    console.log('🔄 Chat está aberto:', isChatOpen);
    console.log('🔄 Session ID atual:', sessionId);
    console.log('🔄 Usuário autenticado:', isAuthenticated);
    
    // Focar no campo de input quando o status for 'open' e o chat estiver aberto
    if (status === 'open' && isChatOpen) {
      setTimeout(() => {
        messageInputRef.current?.focus();
      }, 100);
    }
  }, [status, isChatOpen]);

  useEffect(() => {
    console.log('🔧 SignalR useEffect executado:', { isChatOpen, sessionId, hasConnection: !!connectionRef.current });
    
    if (isChatOpen && sessionId && !connectionRef.current) {
      // Inicia a conexão SignalR se o chat estiver aberto e houver uma sessão
      console.log('🚀 Iniciando nova conexão SignalR...');
      const newConnection = createSignalRConnection();
      
      if (!newConnection) {
        console.error('❌ Falha ao criar conexão SignalR');
        dispatch(setChatError('Erro ao configurar conexão de chat em tempo real.'));
        return;
      }

      console.log('📡 Configurando eventos SignalR...');
      newConnection.on('ReceiveMessage', (message: Message) => {
        console.log('📨 Mensagem recebida via SignalR:', message);
        dispatch(addMessage(message));
      });

      newConnection.on('ChatSessionEnded', (data: { chatSessionId: string; reason: string }) => {
        console.log('🔔 SignalR: ChatSessionEnded recebido:', data);
        console.log('🔔 Razão do encerramento:', data.reason);
        console.log('🔔 Session ID:', data.chatSessionId);
        
        dispatch(addMessage({
          id: `system-${Date.now()}`,
          chatSessionId: data.chatSessionId,
          userId: null,
          content: `Sessão encerrada: ${data.reason}`,
          isFromBot: true,
          sentAt: new Date().toISOString(),
        }));
        
        console.log('🔔 Executando clearChat() e setIsChatOpen(false)');
        dispatch(clearChat()); // Limpa o estado da sessão e fecha o chat
        setIsChatOpen(false);
      });

      newConnection.start()
        .then(() => {
          console.log('✅ SignalR Connected!');
          // Se reconectando, junte-se ao grupo da sessão novamente
          if (sessionId) {
            console.log('🔗 Juntando-se ao grupo da sessão:', sessionId);
            newConnection.invoke('JoinChat', sessionId).catch(err => console.error('❌ Erro ao juntar ao grupo:', err));
          }
        })
        .catch(err => {
          console.error('❌ Error while connecting to SignalR: ', err);
          dispatch(setChatError('Erro ao conectar ao chat em tempo real.'));
        });

      connectionRef.current = newConnection;
    } else if (!isChatOpen && connectionRef.current) {
      // Fecha a conexão SignalR quando o chat é fechado
      console.log('🔌 Fechando conexão SignalR...');
      connectionRef.current.stop().then(() => {
        console.log('✅ SignalR Disconnected!');
        connectionRef.current = null;
      });
    }

    // Limpeza ao desmontar o componente
    return () => {
      if (connectionRef.current) {
        console.log('🧹 Limpeza: Fechando conexão SignalR...');
        connectionRef.current.stop();
        connectionRef.current = null;
      }
    };
  }, [isChatOpen, sessionId, dispatch]); // Adicione sessionId para que o effect re-execute se a sessão mudar

  const handleOpenChat = () => {
    setIsChatOpen(true);
    if (!isAuthenticated) {
      dispatch(setChatStatus('authenticating'));
    } else if (!sessionId) {
      // Se autenticado mas sem sessão ativa, tentar buscar uma sessão existente ou iniciar uma nova.
      checkOrStartChatSession(userId!, userName).catch((error) => {
        console.error('Erro ao abrir chat:', error);
        dispatch(setChatError('Erro ao iniciar sessão de chat.'));
      });
      dispatch(setChatStatus('open'));
    } else {
      // Sessão ativa - abrir diretamente e focar no campo de mensagem
      dispatch(setChatStatus('open'));
      // Focar no campo de mensagem após um pequeno delay para garantir que o dialog foi renderizado
      setTimeout(() => {
        messageInputRef.current?.focus();
      }, 100);
    }
  };

  const handleCloseChat = () => {
    console.log('🚪 handleCloseChat chamado - usuário fechou manualmente');
    setIsChatOpen(false);
    dispatch(setChatStatus('closed'));
    dispatch(setChatError(null));
    // Não limpa user/session aqui, apenas esconde o widget.
    // A sessão só será limpa se o backend a encerrar explicitamente.
  };

  const handleEmailSubmit = async () => {
    dispatch(setChatError(null));
    if (!emailInput) {
      dispatch(setChatError('Por favor, insira um e-mail.'));
      return;
    }

    // Validação básica de e-mail
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(emailInput)) {
      dispatch(setChatError('Por favor, insira um e-mail válido.'));
      return;
    }

    try {
      // 1. Verificar e-mail no backend (se é válido e ativo)
      console.log('🔍 Verificando email:', emailInput);
      const checkEmailResponse = await apiClient.get(`/Users/by-email?email=${encodeURIComponent(emailInput)}`);
      console.log('📡 Status da resposta:', checkEmailResponse.status);
      console.log('📡 Headers da resposta:', checkEmailResponse.headers);
      console.log('📡 Dados brutos da resposta:', checkEmailResponse.data);
      
      const userData = checkEmailResponse.data; // Backend retorna objeto diretamente, não Result<T>
      console.log('📧 Dados do usuário extraídos:', userData);
      console.log('📧 Tipo de userData:', typeof userData);
      console.log('📧 userData é null?', userData === null);
      console.log('📧 userData é undefined?', userData === undefined);
      console.log('📧 Estrutura completa de checkEmailResponse.data:', JSON.stringify(checkEmailResponse.data, null, 2));
      
      // Verificar se userData existe
      if (!userData) {
        console.error('❌ userData é null ou undefined');
        console.error('❌ Possíveis razões:');
        console.error('  1. Backend não retornou propriedade "value"');
        console.error('  2. Propriedade "value" está null/undefined'); 
        console.error('  3. Estrutura da resposta mudou');
        console.error('❌ Estrutura da resposta completa:', checkEmailResponse.data);
        
        // Tentar outras propriedades comuns
        if (checkEmailResponse.data.data) {
          console.log('🔍 Tentando usar checkEmailResponse.data.data:', checkEmailResponse.data.data);
        }
        if (checkEmailResponse.data.user) {
          console.log('🔍 Tentando usar checkEmailResponse.data.user:', checkEmailResponse.data.user);
        }
        if (checkEmailResponse.data.result) {
          console.log('🔍 Tentando usar checkEmailResponse.data.result:', checkEmailResponse.data.result);
        }
        
        dispatch(setChatError('Dados do usuário não encontrados na resposta.'));
        return;
      }
      
      // Verificar se o usuário está ativo
      if (!userData.isActive) {
        console.log('⚠️ Usuário encontrado mas inativo');
        dispatch(setChatStatus('registering'));
        // Usar mensagem padrão, pois esta é uma verificação local baseada na propriedade isActive
        dispatch(setChatError('E-mail encontrado mas não está ativo. Por favor, insira seu nome para reativar.'));
        return;
      }

      // Usuário válido e ativo - recuperar dados
      console.log('✅ Usuário válido encontrado:', userData);
      console.log('🔍 ID do usuário:', userData.id); // Usar 'id' conforme estrutura real
      console.log('🔍 Nome do usuário:', userData.name);
      console.log('🔍 Email do usuário:', userData.email);
      
      try {
        console.log('🔄 Salvando dados do usuário no Redux...');
        dispatch(setUser({ 
          id: userData.id, // Usar 'id' conforme estrutura real
          name: userData.name, 
          email: userData.email 
        }));
        console.log('✅ Dados do usuário salvos no Redux');
      } catch (reduxError: any) {
        console.error('❌ Erro ao salvar dados no Redux:', reduxError);
        dispatch(setChatError('Erro interno. Tente novamente.'));
        return;
      }
      
      // Verificar se há sessão ativa existente ou iniciar nova
      console.log('🔍 Iniciando verificação de sessão para usuário autenticado...');
      try {
        await checkOrStartChatSession(userData.id, userData.name); // Usar 'id' conforme estrutura real
        console.log('✅ Processo de verificação/início de sessão concluído');
        dispatch(setChatStatus('open'));
      } catch (sessionError: any) {
        console.error('❌ Erro CAPTURADO na verificação de sessão:', sessionError);
        // Este erro pode estar sendo lançado pela checkOrStartChatSession
        dispatch(setChatError('Erro ao verificar sessão de chat.'));
        // Ainda permite abrir o chat mesmo com erro de sessão
        dispatch(setChatStatus('open'));
      }
    } catch (apiError: any) {
      console.error('❌ Erro ao verificar e-mail/usuário:', apiError);
      console.error('❌ Detalhes do erro de email:', {
        status: apiError.response?.status,
        statusText: apiError.response?.statusText,
        data: apiError.response?.data,
        message: apiError.message
      });
      
      if (apiError.response && apiError.response.status === 404) {
        // E-mail não encontrado, solicitar nome para cadastro
        console.log('ℹ️ Email não encontrado (404), redirecionando para registro');
        dispatch(setChatStatus('registering'));
        // Usar mensagem da API se disponível
        const apiMessage = apiError.response?.data?.message || apiError.response?.data?.title;
        dispatch(setChatError(apiMessage || 'E-mail não encontrado. Por favor, insira seu nome para se cadastrar.'));
      } else if (apiError.response && apiError.response.data) {
        // Usar mensagem da API
        let errorMessage = '';
        
        if (apiError.response.data.errors) {
          // Erros de validação do .NET
          const errors = Object.values(apiError.response.data.errors).flat();
          errorMessage = errors.join(', ');
        } else if (apiError.response.data.message) {
          // Mensagem padrão da API
          errorMessage = apiError.response.data.message;
        } else if (apiError.response.data.title) {
          // Título do erro
          errorMessage = apiError.response.data.title;
        }
        
        console.error('❌ Mensagem de erro da API:', errorMessage);
        dispatch(setChatError(errorMessage));
      } else {
        // Fallback apenas se não houver resposta da API
        console.error('❌ Erro sem resposta da API');
        dispatch(setChatError('Erro de conexão. Tente novamente.'));
      }
    }
  };

  const handleRegisterUser = async () => {
    dispatch(setChatError(null));
    if (!nameInput) {
      dispatch(setChatError('Por favor, insira um nome.'));
      return;
    }

    try {
      // 2. Cadastrar novo usuário
      const registerResponse = await apiClient.post(`/Users`, {
        name: nameInput,
        email: emailInput,
        isActive: true, // Ou defina a lógica de ativação
      });
      const userData = registerResponse.data; // Backend retorna objeto diretamente, não Result<T>
      
      // Usuário criado com sucesso - recuperar dados (mesmo comportamento do email existente)
      console.log('✅ Usuário criado com sucesso:', userData);
      dispatch(setUser({ 
        id: userData.id, // Usar 'id' conforme estrutura real
        name: userData.name, 
        email: userData.email 
      }));
      
      // Verificar se há sessão ativa existente ou iniciar nova (mesmo fluxo do email existente)
      console.log('🔍 Iniciando verificação de sessão para usuário recém-criado...');
      await checkOrStartChatSession(userData.id, userData.name); // Usar 'id' conforme estrutura real
      console.log('✅ Processo de verificação/início de sessão concluído para novo usuário');
      dispatch(setChatStatus('open'));
      
    } catch (apiError: any) {
      console.error('Erro ao registrar usuário:', apiError);
      
      if (apiError.response && apiError.response.data) {
        // Usar mensagem da API
        let errorMessage = '';
        
        if (apiError.response.data.errors) {
          // Erros de validação do .NET
          const errors = Object.values(apiError.response.data.errors).flat();
          errorMessage = errors.join(', ');
        } else if (apiError.response.data.message) {
          // Mensagem padrão da API
          errorMessage = apiError.response.data.message;
        } else if (apiError.response.data.title) {
          // Título do erro
          errorMessage = apiError.response.data.title;
        }
        
        console.error('❌ Mensagem de erro da API:', errorMessage);
        dispatch(setChatError(errorMessage));
      } else {
        // Fallback apenas se não houver resposta da API
        console.error('❌ Erro sem resposta da API');
        dispatch(setChatError('Erro de conexão. Tente novamente.'));
      }
    }
  };

  const checkOrStartChatSession = async (currentUserId: string, currentUserName: string | null) => {
    console.log('🔍 === INICIO checkOrStartChatSession ===');
    console.log('🔍 Parâmetros recebidos:', { currentUserId, currentUserName });
    
    // Verificar se os parâmetros são válidos
    if (!currentUserId) {
      console.error('❌ currentUserId é inválido:', currentUserId);
      dispatch(setChatError('ID do usuário inválido.'));
      return;
    }
    
    dispatch(setChatError(null));
    
    try {
      // Primeiro, tenta recuperar sessão ativa existente
      try {
        console.log('🔍 Buscando sessão ativa...');
        console.log('🔍 URL da requisição:', `/Chat/active-session?userId=${currentUserId}`);
        const activeSessionResponse = await apiClient.get(`/Chat/active-session?userId=${currentUserId}`);
        console.log('📡 Status da resposta de sessão ativa:', activeSessionResponse.status);
        console.log('📡 Dados brutos da resposta de sessão:', activeSessionResponse.data);
        
        const sessionData = activeSessionResponse.data; // Backend retorna objeto diretamente
        console.log('📝 Dados da sessão extraídos:', sessionData);
        
        if (sessionData && sessionData.chatSessionId) {
          // Sessão ativa encontrada - recuperar dados
          console.log('✅ Sessão ativa encontrada:', sessionData.chatSessionId);
          dispatch(setChatSession({ sessionId: sessionData.chatSessionId }));
          
          // Recuperar histórico de mensagens
          console.log('📚 Recuperando histórico de mensagens...');
          try {
            const historyResponse = await apiClient.get(`/Chat/history?chatSessionId=${sessionData.chatSessionId}`);
            const messages = historyResponse.data || []; // Backend retorna array diretamente
            
            console.log('📚 Mensagens recuperadas:', messages.length);
            
            // Adicionar mensagens ao estado em ordem cronológica
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
            console.warn('⚠️ Erro ao recuperar histórico, mas sessão será mantida:', historyError.message);
            // Não falha por causa do histórico - sessão ainda é válida
          }
          
          console.log('✅ Sessão ativa recuperada com sucesso:', sessionData.chatSessionId);
          console.log('🔍 === FIM checkOrStartChatSession (sessão encontrada) ===');
          return;
        } else {
          console.log('ℹ️ Nenhuma sessão ativa retornada pelo servidor');
        }
      } catch (sessionError: any) {
        console.log('⚠️ Erro ao buscar sessão ativa:', sessionError.response?.status, sessionError.message);
        console.log('⚠️ Detalhes do erro de sessão:', sessionError);
        // Se não encontrar sessão ativa (404), continua para criar nova
        if (sessionError.response?.status !== 404) {
          console.error('❌ Erro não relacionado a "não encontrado":', sessionError);
          // Não lança erro aqui, apenas log - tenta criar nova sessão
        }
        console.log('ℹ️ Status 404 ou erro - tentando criar nova sessão...');
      }
      
      // Não há sessão ativa - iniciar nova sessão
      console.log('🆕 Iniciando nova sessão para o usuário...');
      try {
        await startChatSession(currentUserId, currentUserName, "Olá! Como posso ajudar você hoje?");
        console.log('✅ Nova sessão criada com sucesso');
      } catch (startSessionError: any) {
        console.error('❌ Erro ao criar nova sessão:', startSessionError);
        console.error('❌ Stack trace do erro de start session:', startSessionError.stack);
        // Usar mensagem da API se disponível
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
        // Não lança erro - deixa o usuário tentar novamente
      }
      
    } catch (generalError: any) {
      console.error('❌ Erro geral ao verificar/iniciar sessão de chat:', generalError);
      // Usar mensagem da API se disponível
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
      // Não lança erro para não quebrar o fluxo principal
    }
    
    console.log('🔍 === FIM checkOrStartChatSession ===');
  };

  const startChatSession = async (currentUserId: string, currentUserName: string | null, initialMessageContent: string = "Olá, como posso ajudar?") => {
    console.log('🚀 === INICIO startChatSession ===');
    console.log('🚀 Parâmetros recebidos:', { currentUserId, currentUserName, initialMessageContent });
    
    // Verificar se os parâmetros são válidos
    if (!currentUserId) {
      console.error('❌ currentUserId é inválido para startChatSession:', currentUserId);
      dispatch(setChatError('ID do usuário inválido para iniciar sessão.'));
      return;
    }
    
    dispatch(setChatError(null));
    
    try {
      // Tenta iniciar uma nova sessão. Se já houver uma, o backend pode retornar a existente ou criar uma nova.
      console.log('📤 Enviando requisição para start-session...');
      const sessionResponse = await apiClient.post(`/Chat/start-session`, {
        userId: currentUserId,
        userName: currentUserName,
        initialMessageContent: initialMessageContent,
      });
      
      console.log('📥 Resposta do start-session:', sessionResponse.data);
      const sessionData = sessionResponse.data; // Backend retorna objeto diretamente
      
      if (!sessionData || !sessionData.chatSessionId) {
        console.error('❌ Dados da sessão inválidos:', sessionData);
        throw new Error('Dados da sessão inválidos retornados pelo servidor');
      }
      
      console.log('✅ Sessão criada com sucesso:', sessionData.chatSessionId);
      console.log('🔄 Salvando sessão no Redux...');
      dispatch(setChatSession({ sessionId: sessionData.chatSessionId }));
      console.log('✅ Sessão salva no Redux com ID:', sessionData.chatSessionId);
      
      // Adicionar mensagem inicial apenas se retornada pelo backend
      if (sessionData.messageId && sessionData.initialMessage) {
        console.log('💬 Adicionando mensagem inicial:', sessionData.initialMessage);
        dispatch(addMessage({
          id: sessionData.messageId, // ID da mensagem inicial
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
      
      // Junte-se ao grupo SignalR para esta nova sessão
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
        // Usar mensagem da API
        let errorMessage = '';
        
        if (apiError.response.data.errors) {
          // Erros de validação do .NET
          const errors = Object.values(apiError.response.data.errors).flat();
          errorMessage = errors.join(', ');
        } else if (apiError.response.data.message) {
          // Mensagem padrão da API
          errorMessage = apiError.response.data.message;
        } else if (apiError.response.data.title) {
          // Título do erro
          errorMessage = apiError.response.data.title;
        }
        
        console.error('❌ Mensagem de erro da API:', errorMessage);
        dispatch(setChatError(errorMessage));
      } else {
        // Fallback apenas se não houver resposta da API
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
      id: crypto.randomUUID(), // Gerar um ID único no frontend para exibição imediata
      chatSessionId: sessionId,
      userId: userId,
      content: messageInput,
      isFromBot: false,
      sentAt: new Date().toISOString(),
    };

    dispatch(addMessage(userMessage)); // Adiciona a mensagem do usuário imediatamente
    setMessageInput(''); // Limpa o input

    try {
      // Envia mensagem do usuário para o backend
      await apiClient.post(`/Chat/send-message`, {
        chatSessionId: sessionId,
        userId: userId,
        content: userMessage.content,
        messageType: 1, // MessageType.Text
      });

      // Se a mensagem for 'sair', chama o endpoint de encerramento
      if (userMessage.content.toLowerCase().trim() === 'sair') {
        await apiClient.post(`/Chat/end-session`, {
          chatSessionId: sessionId,
          endReason: "Usuário digitou 'sair'",
        });
        // O SignalR cuidará da notificação de encerramento e limpeza do estado no frontend
      } else {
        // Solicita resposta do bot
        // O backend deve chamar o endpoint do bot após salvar a mensagem do usuário
        // ou o bot deve processar a mensagem do usuário de forma assíncrona após a gravação
        // e enviar a resposta via SignalR.
        // Por agora, vamos simular uma chamada direta ao bot para agilizar a resposta,
        // mas o ideal é que o `MessageSentEventHandler` dispare a lógica do bot no backend.
        const botResponse = await apiClient.post(`/Bot/process-message`, {
          chatSessionId: sessionId,
          userId: userId, // Bot precisa do userId para contexto
          userMessage: userMessage.content,
        });
        const botMessageData = botResponse.data; // Backend retorna objeto diretamente
        dispatch(addMessage({
          id: botMessageData.messageId,
          chatSessionId: botMessageData.chatSessionId,
          userId: null,
          content: botMessageData.botMessageContent,
          isFromBot: true,
          sentAt: botMessageData.sentAt,
        }));
      }

    } catch (apiError: any) {
      console.error('Erro ao enviar mensagem:', apiError);
      
      if (apiError.response && apiError.response.data) {
        // Usar mensagem da API
        let errorMessage = '';
        
        if (apiError.response.data.errors) {
          // Erros de validação do .NET
          const errors = Object.values(apiError.response.data.errors).flat();
          errorMessage = errors.join(', ');
        } else if (apiError.response.data.message) {
          // Mensagem padrão da API
          errorMessage = apiError.response.data.message;
        } else if (apiError.response.data.title) {
          // Título do erro
          errorMessage = apiError.response.data.title;
        }
        
        console.error('❌ Mensagem de erro da API:', errorMessage);
        dispatch(setChatError(errorMessage));
      } else {
        // Fallback apenas se não houver resposta da API
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

          {error && <p className="text-red-500 mt-2 p-4 text-center text-sm">{error}</p>} {/* Erro global */}

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
                  onKeyPress={(e) => e.key === 'Enter' && handleRegisterUser()}
                />
                <Button 
                  onClick={handleRegisterUser} 
                  className="w-full bg-red-600 hover:bg-red-700"
                  disabled={!nameInput}
                >
                  {error?.includes('não está ativo') ? 'Reativar Conta' : 'Cadastrar e Iniciar Chat'}
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
                {[...messages] // Criar cópia do array antes de ordenar
                  .sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime()) // Ordenar por data
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
                <div ref={messagesEndRef} /> {/* Para scroll automático */}
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