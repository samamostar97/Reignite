/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
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
        'display': ['system-ui', '-apple-system', 'sans-serif'],
      },
      backgroundImage: {
        'ember-gradient': 'linear-gradient(135deg, #ff6b35 0%, #f7931e 100%)',
        'warm-gradient': 'linear-gradient(135deg, #fff8f0 0%, #ffe0b2 100%)',
      },
    },
  },
  plugins: [],
}
