# 🧪 Guia de Testes - Chat Widget F1

## 🚀 Preparação para Testes

### 1. **Iniciar o Backend**
```bash
cd BackEnd
dotnet run
```
Verificar se está rodando em: `https://localhost:7000`

### 2. **Iniciar o Frontend**
```bash
cd FrontEnd/chatbot-f1
npm install
npm run dev
```
Verificar se está rodando em: `http://localhost:3000`

## 📋 Cenários de Teste

### ✅ **Teste 1: Novo Usuário**

1. **Abrir a aplicação** em `http://localhost:3000`
2. **Clicar no ícone do chat** (canto inferior direito)
3. **Inserir e-mail inexistente**: `novo@teste.com`
4. **Clicar em "Continuar"**
5. **Verificar**: Deve solicitar nome para cadastro
6. **Inserir nome**: `João Silva`
7. **Clicar em "Cadastrar e Iniciar Chat"**
8. **Verificar**: Chat deve abrir e iniciar sessão

**Resultado Esperado:**
- ✅ E-mail não encontrado → Formulário de cadastro
- ✅ Usuário cadastrado com sucesso
- ✅ Chat iniciado automaticamente
- ✅ Foco no campo de mensagem

### ✅ **Teste 2: Usuário Existente Ativo**

1. **Fechar e reabrir o chat**
2. **Inserir e-mail existente**: `novo@teste.com`
3. **Clicar em "Continuar"**

**Resultado Esperado:**
- ✅ E-mail reconhecido
- ✅ Chat aberto diretamente
- ✅ Sessão recuperada (se existir)
- ✅ Histórico de mensagens mantido

### ✅ **Teste 3: Conversação Básica**

1. **Com chat aberto, digitar**: `Olá!`
2. **Pressionar Enter ou clicar em enviar**
3. **Aguardar resposta do bot**
4. **Continuar conversação**

**Resultado Esperado:**
- ✅ Mensagem do usuário aparece imediatamente
- ✅ Bot responde automaticamente
- ✅ Mensagens em ordem cronológica
- ✅ Scroll automático para última mensagem
- ✅ Identificação clara do autor (emoji + nome)

### ✅ **Teste 4: Comando de Saída**

1. **Digitar**: `sair`
2. **Pressionar Enter**

**Resultado Esperado:**
- ✅ Sessão encerrada
- ✅ Mensagem de confirmação
- ✅ Chat fechado automaticamente
- ✅ Estado limpo para próxima sessão

### ✅ **Teste 5: Responsividade**

#### Desktop (> 1024px)
- ✅ Chat como widget lateral
- ✅ Ícone no canto inferior direito
- ✅ Hover effects funcionando

#### Tablet (640px - 1024px)
- ✅ Chat em overlay
- ✅ Tamanho intermediário
- ✅ Touch funcionando

#### Mobile (< 640px)
- ✅ Chat em tela cheia
- ✅ Ícone redimensionado
- ✅ Interface touch-friendly

### ✅ **Teste 6: Reconexão SignalR**

1. **Abrir DevTools** (F12)
2. **Ir para Network tab**
3. **Simular perda de conexão**
4. **Verificar logs de reconexão**

**Resultado Esperado:**
- ✅ Tentativas automáticas de reconexão
- ✅ Logs de status no console
- ✅ Indicador visual de status

### ✅ **Teste 7: Validações**

#### E-mail Inválido
- **Digitar**: `email-invalido`
- **Resultado**: Mensagem de erro de formato

#### Campo Vazio
- **Deixar campo vazio** e tentar continuar
- **Resultado**: Mensagem solicitando preenchimento

#### Nome Vazio
- **No cadastro, deixar nome vazio**
- **Resultado**: Botão desabilitado

### ✅ **Teste 8: Performance**

1. **Abrir DevTools → Performance**
2. **Gravar sessão de uso**
3. **Verificar métricas**

**Métricas Esperadas:**
- ✅ First Paint < 1s
- ✅ Interactive < 2s
- ✅ Sem memory leaks
- ✅ CPU usage baixo

## 🐛 **Cenários de Erro para Testar**

### Backend Offline
1. **Parar o backend**
2. **Tentar usar o chat**
3. **Verificar**: Mensagens de erro apropriadas

### Rede Lenta
1. **Simular rede lenta** (DevTools → Network → Slow 3G)
2. **Usar o chat normalmente**
3. **Verificar**: Loading states e timeouts

### Dados Inválidos
1. **Interceptar requests** e modificar responses
2. **Verificar**: Tratamento de erros robusto

## 📊 **Checklist de Qualidade**

### Funcionalidade
- [ ] ✅ Todos os fluxos principais funcionando
- [ ] ✅ Validações apropriadas
- [ ] ✅ Tratamento de erros
- [ ] ✅ Estados de loading

### UX/UI
- [ ] ✅ Interface intuitiva
- [ ] ✅ Feedback visual adequado
- [ ] ✅ Responsive design
- [ ] ✅ Acessibilidade

### Performance
- [ ] ✅ Carregamento rápido
- [ ] ✅ Animações suaves
- [ ] ✅ Sem travamentos
- [ ] ✅ Memory usage controlado

### Integração
- [ ] ✅ Backend communication
- [ ] ✅ SignalR funcionando
- [ ] ✅ State management
- [ ] ✅ Error handling

## 🎯 **Critérios de Sucesso**

Para considerar o teste **APROVADO**, todos os itens devem funcionar:

1. ✅ **Fluxo completo** de novo usuário
2. ✅ **Recuperação** de sessão existente
3. ✅ **Chat em tempo real** funcionando
4. ✅ **Comando sair** encerrando corretamente
5. ✅ **Responsividade** em todos os dispositivos
6. ✅ **Performance** dentro dos parâmetros
7. ✅ **Tratamento de erros** apropriado
8. ✅ **Acessibilidade** básica funcionando

---

## 🏁 **Resultado Final**

Se todos os testes passarem, o **Chat Widget F1** está **pronto para produção** e atende a todos os requisitos especificados! 🎉
