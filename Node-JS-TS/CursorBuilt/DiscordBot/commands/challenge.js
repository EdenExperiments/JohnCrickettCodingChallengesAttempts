const { SlashCommandBuilder } = require('discord.js');
const challengeUtils = require('../utils/challenges');

module.exports = {
  data: new SlashCommandBuilder()
    .setName('challenge')
    .setDescription('Get a random coding challenge'),
  async execute(interaction, challengesRef) {
    const challenge = challengeUtils.getRandomChallenge(challengesRef.data);
    if (challenge) {
      await interaction.reply(`Challenge: ${challenge.title}\n${challenge.url}`);
    } else {
      await interaction.reply('No challenges available. Try adding some with /add command.');
    }
  },
}; 