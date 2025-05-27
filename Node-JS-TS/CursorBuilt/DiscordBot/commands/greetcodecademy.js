const { SlashCommandBuilder } = require('discord.js');

// Array of motivational messages for coding learners
const motivationalMessages = [
  "Keep coding! Every line of code you write is a step toward mastery. 💻✨",
  "Bugs are just opportunities to learn something new. Embrace the challenge! 🐛→🦋",
  "The best error message is the one that never shows up. The second best is the one that helps you debug. Keep learning! 🔍",
  "Code like nobody's watching, debug like everyone is. You've got this! 👩‍💻👨‍💻",
  "Remember: even experienced developers spend most of their time searching for solutions. You're not alone! 🌐",
  "Today's frustrations are tomorrow's expertise. Keep pushing forward! 🚀",
  "Your code doesn't have to be perfect, it just has to work. Refactor later, learn now! 📝",
  "Programming is the closest thing we have to magic. You're learning to be a wizard! 🧙‍♂️",
  "Every programmer you admire started exactly where you are now. One day at a time! ⏱️",
  "Coding is like exercise - it gets easier the more you do it, but you have to keep showing up! 💪"
];

module.exports = {
  data: new SlashCommandBuilder()
    .setName('greetcodecademy')
    .setDescription('Provides a motivating message to fellow coding learners'),
  async execute(interaction) {
    // Get a random motivational message
    const randomIndex = Math.floor(Math.random() * motivationalMessages.length);
    const message = motivationalMessages[randomIndex];
    
    await interaction.reply(`Hey Codecademy learners! ${message}`);
  },
}; 