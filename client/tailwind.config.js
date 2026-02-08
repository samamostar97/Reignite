/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        // Backgrounds (dark warm tones)
        'bg': {
          'primary': '#0D0A07',
          'surface': '#1A1410',
          'surface-light': '#231C15',
          'surface-elevated': '#2C2318',
        },
        // Ember accent palette
        'ember': {
          DEFAULT: '#F97316',
          'dim': 'rgba(249, 115, 22, 0.1)',
          'border': 'rgba(249, 115, 22, 0.25)',
          'glow': 'rgba(249, 115, 22, 0.15)',
        },
        'flame': {
          DEFAULT: '#EF4444',
        },
        'amber': {
          DEFAULT: '#F59E0B',
          'dim': 'rgba(245, 158, 11, 0.1)',
        },
        // Warm neutrals
        'warm': {
          50: '#FAF5F0',
          100: '#E8DDD0',
          200: '#C4B5A3',
          300: '#9E8E7A',
          400: '#7A6B59',
          500: '#5C4F40',
          600: '#3D3328',
          700: '#2C2318',
        },
        // Semantic
        'success': {
          DEFAULT: '#A3BE8C',
          'dim': 'rgba(163, 190, 140, 0.1)',
        },
        'warning': {
          DEFAULT: '#EBCB8B',
        },
        'error': {
          DEFAULT: '#BF616A',
          'dim': 'rgba(191, 97, 106, 0.1)',
        },
        'info': {
          DEFAULT: '#D08770',
        },
        // Legacy (keep for existing components)
        'reignite': {
          'ember': '#ff6b35',
          'flame': '#f7931e',
          'ash': '#2c1810',
          'smoke': '#5a3a2a',
          'glow': '#fff5e6',
          'warm': '#ffe0b2',
        },
      },
      fontFamily: {
        'display': ['Cinzel', 'Georgia', 'serif'],
        'body': ['Space Grotesk', 'system-ui', 'sans-serif'],
      },
      backgroundImage: {
        'ember-gradient': 'linear-gradient(135deg, #F97316, #EF4444)',
        'warm-gradient': 'linear-gradient(135deg, #F59E0B, #F97316)',
        'surface-gradient': 'linear-gradient(180deg, #1A1410, #0D0A07)',
      },
      boxShadow: {
        'ember': '0 4px 20px rgba(249, 115, 22, 0.25)',
        'ember-lg': '0 8px 30px rgba(249, 115, 22, 0.35)',
        'ember-glow': '0 0 20px rgba(249, 115, 22, 0.15)',
      },
      animation: {
        'ember-pulse': 'ember-pulse 2s ease-in-out infinite',
        'fade-in': 'fadeIn 0.3s ease-out',
        'slide-up': 'slideUp 0.4s ease-out',
      },
      keyframes: {
        'ember-pulse': {
          '0%, 100%': { opacity: '0.6' },
          '50%': { opacity: '1' },
        },
        'fadeIn': {
          from: { opacity: '0', transform: 'translateY(10px)' },
          to: { opacity: '1', transform: 'translateY(0)' },
        },
        'slideUp': {
          from: { opacity: '0', transform: 'translateY(20px)' },
          to: { opacity: '1', transform: 'translateY(0)' },
        },
      },
    },
  },
  plugins: [],
}
