const { SlashCommandBuilder } = require('discord.js');
const challengeUtils = require('../utils/challenges');

module.exports = {
  data: new SlashCommandBuilder()
    .setName('list')
    .setDescription('List all available challenges'),
  async execute(interaction) {
    const localChallenges = challengeUtils.loadChallenges();
    
    if (localChallenges.length === 0) {
      await interaction.reply('No challenges available. Try adding some with /add command.');
      return;
    }
    
    const challengeList = localChallenges
      .map((challenge, index) => `${index + 1}. ${challenge.title} : ${challenge.url}`)
      .join('\n');
    
    await interaction.reply(`Available Challenges:\n${challengeList}`);
  },
}; 