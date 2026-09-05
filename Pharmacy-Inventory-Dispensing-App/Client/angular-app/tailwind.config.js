/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: 'var(--primary-color)',
          hover: 'var(--primary-color-hover)',
          light: 'var(--primary-color-light)',
          dark: 'var(--primary-color-dark)',
        },
        accent: {
          DEFAULT: 'var(--accent-color)',
          light: 'var(--accent-color-light)',
        },
        surface: {
          0: 'var(--surface-0)',
          50: 'var(--surface-50)',
          100: 'var(--surface-100)',
          200: 'var(--surface-200)',
          300: 'var(--surface-300)',
          400: 'var(--surface-400)',
          500: 'var(--surface-500)',
          600: 'var(--surface-600)',
          700: 'var(--surface-700)',
          800: 'var(--surface-800)',
          900: 'var(--surface-900)',
        }
      },
      borderColor: {
        DEFAULT: 'var(--border-color)',
      },
      boxShadow: {
        sm: 'var(--shadow-sm)',
        DEFAULT: 'var(--shadow)',
        md: 'var(--shadow-md)',
        lg: 'var(--shadow-lg)',
        glow: 'var(--shadow-glow)',
      },
      borderRadius: {
        DEFAULT: 'var(--border-radius-sm)',
        lg: 'var(--border-radius)',
        xl: 'var(--border-radius-lg)',
      }
    },
  },
  plugins: [],
}
