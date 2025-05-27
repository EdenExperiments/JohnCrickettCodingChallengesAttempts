const { SlashCommandBuilder } = require('discord.js');
const challengeUtils = require('../utils/challenges');

module.exports = {
  data: new SlashCommandBuilder()
    .setName('add')
    .setDescription('Add a new challenge')
    .addStringOption(option => 
      option.setName('url')
        .setDescription('URL of the challenge to add (must be from codingchallenges.fyi)')
        .setRequired(true)),
  async execute(interaction, challengesRef) {
    const url = interaction.options.getString('url');
    
    const result = await challengeUtils.addChallenge(url);
    
    if (result.success) {
      // Update the challenges reference if provided
      if (challengesRef && typeof challengesRef.update === 'function') {
        challengesRef.update(challengeUtils.loadChallenges());
      }
      await interaction.reply(`Challenge added: ${result.challenge.title}`);
    } else {
      await interaction.reply(`Failed to add challenge: ${result.message}`);
    }
  },
}; 