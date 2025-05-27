const { SlashCommandBuilder } = require('discord.js');

module.exports = {
  data: new SlashCommandBuilder()
    .setName('hello')
    .setDescription('Get a personalized greeting'),
  async execute(interaction) {
    await interaction.reply(`Hello, ${interaction.user.username}!`);
  },
}; 