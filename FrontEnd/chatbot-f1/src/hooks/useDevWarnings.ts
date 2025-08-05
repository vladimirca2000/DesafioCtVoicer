import { useEffect } from 'react';

// Hook para suprimir warnings específicos apenas em desenvolvimento
export const useDevWarnings = () => {
  useEffect(() => {
    if (process.env.NODE_ENV === 'development') {
      // Salvar console.warn original
      const originalWarn = console.warn;
      const originalError = console.error;
      const originalLog = console.log;

      // Override console.warn para filtrar warnings específicos
      console.warn = (...args) => {
        const message = args.join(' ');
        
        // Lista de warnings para suprimir
        const suppressWarnings = [
          'Extra attributes from the server',
          'data-new-gr-c-s-check-loaded',
          'data-gr-ext-installed',
          'Images loaded lazily and replaced with placeholders',
          'Intervention',
          'Setting the NODE_TLS_REJECT_UNAUTHORIZED',
          'makes TLS connections and HTTPS requests insecure',
        ];

        // Verificar se o warning deve ser suprimido
        const shouldSuppress = suppressWarnings.some(warning => 
          message.includes(warning)
        );

        if (!shouldSuppress) {
          originalWarn(...args);
        }
      };

      // Override console.error para filtrar erros específicos
      console.error = (...args) => {
        const message = args.join(' ');
        
        // Lista de erros para suprimir (apenas warnings menores)
        const suppressErrors = [
          'Download the React DevTools',
          'A listener indicated an asynchronous response by returning true',
          'net::ERR_NAME_NOT_RESOLVED',
          'via.placeholder.com',
        ];

        const shouldSuppress = suppressErrors.some(error => 
          message.includes(error)
        );

        if (!shouldSuppress) {
          originalError(...args);
        }
      };

      // Override console.log para filtrar logs específicos
      console.log = (...args) => {
        const message = args.join(' ');
        
        // Lista de logs para suprimir
        const suppressLogs = [
          '{"notify":"init_tab"}',
          'msgport.js',
          'Fast Refresh',
        ];

        const shouldSuppress = suppressLogs.some(log => 
          message.includes(log)
        );

        if (!shouldSuppress) {
          originalLog(...args);
        }
      };

      // Cleanup: restaurar console original quando o componente for desmontado
      return () => {
        console.warn = originalWarn;
        console.error = originalError;
        console.log = originalLog;
      };
    }
  }, []);
};

export default useDevWarnings;
