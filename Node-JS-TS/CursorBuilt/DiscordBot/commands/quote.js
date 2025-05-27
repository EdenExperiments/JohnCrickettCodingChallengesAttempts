const { SlashCommandBuilder } = require('discord.js');
const quoteUtils = require('../utils/quotes');

module.exports = {
  data: new SlashCommandBuilder()
    .setName('quote')
    .setDescription('Get a random quote'),
  async execute(interaction) {
    const quoteData = await quoteUtils.getRandomQuote();
    await interaction.reply(`"${quoteData.quote}" - ${quoteData.author}`);
  },
}; 