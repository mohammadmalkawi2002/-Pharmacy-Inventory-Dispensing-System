import { definePreset } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';

export const MediflowPreset = definePreset(Aura, {
  semantic: {
    primary: {
      50: '#fffbeb',
      100: '#fef3c7',
      200: '#fde68a',
      300: '#fcd34d',
      400: '#fbbf24',
      500: '#F59E0B',
      600: '#d97706',
      700: '#b45309',
      800: '#92400e',
      900: '#78350f',
      950: '#451a03'
    },
    colorScheme: {
      light: {
        surface: {
          0: '#ffffff',
          50: '#f8fafc',
          100: '#f1f5f9',
          200: '#e2e8f0',
          300: '#cbd5e1',
          400: '#94a3b8',
          500: '#64748b',
          600: '#475569',
          700: '#334155',
          800: '#1e293b',
          900: '#0f172a',
          950: '#020617'
        }
      },
      dark: {
        surface: {
          0: '#ffffff',
          50: '#0b1120',
          100: '#1e293b',
          200: '#334155',
          300: '#475569',
          400: '#64748b',
          500: '#94a3b8',
          600: '#cbd5e1',
          700: '#e2e8f0',
          800: '#f1f5f9',
          900: '#f8fafc',
          950: '#ffffff'
        }
      }
    }
  }
});
