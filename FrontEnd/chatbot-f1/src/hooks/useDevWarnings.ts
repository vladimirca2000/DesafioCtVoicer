import { useEffect } from 'react';

export const useDevWarnings = () => {
  useEffect(() => {
    if (process.env.NODE_ENV === 'development') {
      const originalWarn = console.warn;
      const originalError = console.error;
      const originalLog = console.log;

      console.warn = (...args) => {
        const message = args.join(' ');
        
        const suppressWarnings = [
          'Extra attributes from the server',
          'data-new-gr-c-s-check-loaded',
          'data-gr-ext-installed',
          'Images loaded lazily and replaced with placeholders',
          'Intervention',
          'Setting the NODE_TLS_REJECT_UNAUTHORIZED',
          'makes TLS connections and HTTPS requests insecure',
        ];

        const shouldSuppress = suppressWarnings.some(warning => 
          message.includes(warning)
        );

        if (!shouldSuppress) {
          originalWarn(...args);
        }
      };

      console.error = (...args) => {
        const message = args.join(' ');
        
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

      console.log = (...args) => {
        const message = args.join(' ');
        
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

      return () => {
        console.warn = originalWarn;
        console.error = originalError;
        console.log = originalLog;
      };
    }
  }, []);
};

export default useDevWarnings;
