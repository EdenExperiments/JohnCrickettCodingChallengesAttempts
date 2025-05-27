const { SlashCommandBuilder, EmbedBuilder } = require('discord.js');
const challengeManager = require('../utils/challengeManager');

module.exports = {
  data: new SlashCommandBuilder()
    .setName('challenge')
    .setDescription('Manage your coding challenge progress')
    .addSubcommand(subcommand =>
      subcommand
        .setName('status')
        .setDescription('View your current challenge progress'))
    .addSubcommand(subcommand =>
      subcommand
        .setName('add')
        .setDescription('Add or update a challenge')
        .addStringOption(option =>
          option.setName('name')
            .setDescription('Name of the challenge')
            .setRequired(true))
        .addStringOption(option =>
          option.setName('language')
            .setDescription('Programming language used')
            .setRequired(true))
        .addStringOption(option =>
          option.setName('status')
            .setDescription('Current status of the challenge')
            .setRequired(true)
            .addChoices(
              { name: 'Not Started', value: 'not_started' },
              { name: 'In Progress', value: 'in_progress' },
              { name: 'Completed', value: 'completed' },
              { name: 'Stuck', value: 'stuck' }
            ))
        .addStringOption(option =>
          option.setName('difficulty')
            .setDescription('Difficulty level')
            .addChoices(
              { name: 'Easy', value: 'easy' },
              { name: 'Medium', value: 'medium' },
              { name: 'Hard', value: 'hard' },
              { name: 'Very Hard', value: 'very_hard' }
            ))
        .addIntegerOption(option =>
          option.setName('time_taken')
            .setDescription('Time taken in minutes'))
        .addStringOption(option =>
          option.setName('url')
            .setDescription('URL of the challenge (optional)')))
    .addSubcommand(subcommand =>
      subcommand
        .setName('view')
        .setDescription('View details for a specific challenge')
        .addStringOption(option =>
          option.setName('name')
            .setDescription('Name of the challenge')
            .setRequired(true)))
    .addSubcommand(subcommand =>
      subcommand
        .setName('list')
        .setDescription('List all your challenges'))
    .addSubcommand(subcommand =>
      subcommand
        .setName('stats')
        .setDescription('View challenge statistics'))
    .addSubcommand(subcommand =>
      subcommand
        .setName('catalog')
        .setDescription('View all available challenges in the catalog')),
  
  async execute(interaction) {
    const subcommand = interaction.options.getSubcommand();
    
    switch (subcommand) {
      case 'status':
        await handleStatusCommand(interaction);
        break;
      case 'add':
        await handleAddCommand(interaction);
        break;
      case 'view':
        await handleViewCommand(interaction);
        break;
      case 'list':
        await handleListCommand(interaction);
        break;
      case 'stats':
        await handleStatsCommand(interaction);
        break;
      case 'catalog':
        await handleCatalogCommand(interaction);
        break;
    }
  }
};

// Schedule function for auto-posting summaries (can be called from index.js)
async function postChallengeSummary(client, channelId) {
  try {
    const channel = await client.channels.fetch(channelId);
    if (!channel) return;
    
    const stats = challengeManager.getChallengeStats();
    
    // Create a summary embed
    const embed = new EmbedBuilder()
      .setTitle('Coding Challenge Summary')
      .setColor(0x4CAF50)
      .setDescription('Here\'s a summary of coding challenges our members are working on!')
      .addFields(
        { name: 'Total Users', value: stats.totalUsers.toString(), inline: true },
        { name: 'Total Challenges', value: stats.totalChallenges.toString(), inline: true },
        { name: 'In Progress', value: stats.totalInProgress.toString(), inline: true },
        { name: 'Avg. Completion Time', value: `${stats.averageCompletionTime} minutes`, inline: true }
      )
      .setTimestamp();
    
    // Add language stats if any
    const languages = Object.entries(stats.languageStats)
      .sort((a, b) => b[1] - a[1])
      .slice(0, 5)
      .map(([lang, count]) => `${lang}: ${count}`)
      .join('\n');
    
    if (languages) {
      embed.addFields({ name: 'Top Languages', value: languages });
    }
    
    // Add difficulty stats if any
    const difficulties = Object.entries(stats.difficultyStats)
      .sort((a, b) => b[1] - a[1])
      .map(([diff, count]) => `${diff.charAt(0).toUpperCase() + diff.slice(1)}: ${count}`)
      .join('\n');
    
    if (difficulties) {
      embed.addFields({ name: 'Difficulty Breakdown', value: difficulties });
    }
    
    await channel.send({ embeds: [embed] });
  } catch (error) {
    console.error('Error posting challenge summary:', error);
  }
}

// Handler for the status subcommand
async function handleStatusCommand(interaction) {
  const userId = interaction.user.id;
  const challenges = challengeManager.getUserChallenges(userId);
  
  if (challenges.length === 0) {
    await interaction.reply({
      content: 'You haven\'t added any challenges yet. Use `/challenge add` to add one!',
      ephemeral: true
    });
    return;
  }
  
  // Count challenges by status
  const statusCounts = {
    not_started: 0,
    in_progress: 0,
    completed: 0,
    stuck: 0
  };
  
  challenges.forEach(challenge => {
    if (challenge.status) {
      statusCounts[challenge.status] = (statusCounts[challenge.status] || 0) + 1;
    }
  });
  
  // Get most recent challenges
  const recentChallenges = [...challenges]
    .sort((a, b) => new Date(b.updatedAt) - new Date(a.updatedAt))
    .slice(0, 3);
  
  // Create the embed
  const embed = new EmbedBuilder()
    .setTitle(`${interaction.user.username}'s Challenge Progress`)
    .setColor(0x3498DB)
    .addFields(
      { name: 'Total Challenges', value: challenges.length.toString(), inline: true },
      { name: 'Completed', value: statusCounts.completed.toString(), inline: true },
      { name: 'In Progress', value: statusCounts.in_progress.toString(), inline: true },
      { name: 'Not Started', value: statusCounts.not_started.toString(), inline: true },
      { name: 'Stuck', value: statusCounts.stuck.toString(), inline: true }
    );
  
  if (recentChallenges.length > 0) {
    const recentList = recentChallenges.map(c => {
      const statusEmoji = getStatusEmoji(c.status);
      return `${statusEmoji} **${c.name}** (${c.language})`;
    }).join('\n');
    
    embed.addFields({ name: 'Recent Challenges', value: recentList });
  }
  
  await interaction.reply({ embeds: [embed] });
}

// Handler for the add subcommand
async function handleAddCommand(interaction) {
  const userId = interaction.user.id;
  const username = interaction.user.username;
  
  const name = interaction.options.getString('name');
  const language = interaction.options.getString('language');
  const status = interaction.options.getString('status');
  const difficulty = interaction.options.getString('difficulty');
  const timeTaken = interaction.options.getInteger('time_taken');
  const url = interaction.options.getString('url');
  
  // Create challenge data object
  const challengeData = {
    name,
    language,
    status,
    ...(difficulty && { difficulty }),
    ...(timeTaken && { timeTaken }),
    ...(url && { url })
  };
  
  // Update the challenge
  const result = challengeManager.updateChallengeProgress(userId, username, challengeData);
  
  if (result) {
    const statusEmoji = getStatusEmoji(status);
    
    await interaction.reply({
      content: `${statusEmoji} Challenge **${name}** has been ${
        challengeManager.getUserChallenge(userId, name) ? 'updated' : 'added'
      }!`,
      ephemeral: false
    });
  } else {
    await interaction.reply({
      content: 'There was an error updating your challenge. Please try again.',
      ephemeral: true
    });
  }
}

// Handler for the view subcommand
async function handleViewCommand(interaction) {
  const userId = interaction.user.id;
  const name = interaction.options.getString('name');
  
  const challenge = challengeManager.getUserChallenge(userId, name);
  
  if (!challenge) {
    await interaction.reply({
      content: `Could not find a challenge named "${name}". Please check the name and try again.`,
      ephemeral: true
    });
    return;
  }
  
  // Format dates
  const createdDate = new Date(challenge.createdAt).toLocaleDateString();
  const updatedDate = new Date(challenge.updatedAt).toLocaleDateString();
  
  // Get status emoji
  const statusEmoji = getStatusEmoji(challenge.status);
  
  // Create the embed
  const embed = new EmbedBuilder()
    .setTitle(`Challenge: ${challenge.name}`)
    .setColor(getStatusColor(challenge.status))
    .addFields(
      { name: 'Status', value: `${statusEmoji} ${formatStatus(challenge.status)}`, inline: true },
      { name: 'Language', value: challenge.language, inline: true },
      ...(challenge.difficulty ? [{ name: 'Difficulty', value: formatDifficulty(challenge.difficulty), inline: true }] : []),
      ...(challenge.timeTaken ? [{ name: 'Time Taken', value: `${challenge.timeTaken} minutes`, inline: true }] : []),
      { name: 'Added On', value: createdDate, inline: true },
      { name: 'Last Updated', value: updatedDate, inline: true }
    );
  
  if (challenge.url) {
    embed.setURL(challenge.url)
      .addFields({ name: 'Challenge URL', value: challenge.url });
  }
  
  await interaction.reply({ embeds: [embed] });
}

// Handler for the list subcommand
async function handleListCommand(interaction) {
  const userId = interaction.user.id;
  const challenges = challengeManager.getUserChallenges(userId);
  
  if (challenges.length === 0) {
    await interaction.reply({
      content: 'You haven\'t added any challenges yet. Use `/challenge add` to add one!',
      ephemeral: true
    });
    return;
  }
  
  // Group challenges by status
  const grouped = {
    in_progress: [],
    stuck: [],
    not_started: [],
    completed: []
  };
  
  challenges.forEach(challenge => {
    if (challenge.status && grouped[challenge.status]) {
      grouped[challenge.status].push(challenge);
    }
  });
  
  // Create the embed
  const embed = new EmbedBuilder()
    .setTitle(`${interaction.user.username}'s Challenges`)
    .setColor(0x3498DB);
  
  // Add each status group
  for (const [status, items] of Object.entries(grouped)) {
    if (items.length > 0) {
      const statusEmoji = getStatusEmoji(status);
      const list = items.map(c => `**${c.name}** (${c.language})`).join('\n');
      embed.addFields({ name: `${statusEmoji} ${formatStatus(status)} (${items.length})`, value: list || 'None' });
    }
  }
  
  await interaction.reply({ embeds: [embed] });
}

// Handler for the stats subcommand
async function handleStatsCommand(interaction) {
  const userId = interaction.user.id;
  const challenges = challengeManager.getUserChallenges(userId);
  
  if (challenges.length === 0) {
    await interaction.reply({
      content: 'You haven\'t added any challenges yet. Use `/challenge add` to add one!',
      ephemeral: true
    });
    return;
  }
  
  // Calculate statistics
  const languageCounts = {};
  const difficultyCounts = {};
  let totalTime = 0;
  let timeCount = 0;
  
  challenges.forEach(challenge => {
    // Count languages
    if (challenge.language) {
      languageCounts[challenge.language] = (languageCounts[challenge.language] || 0) + 1;
    }
    
    // Count difficulties
    if (challenge.difficulty) {
      difficultyCounts[challenge.difficulty] = (difficultyCounts[challenge.difficulty] || 0) + 1;
    }
    
    // Sum time taken
    if (challenge.timeTaken) {
      totalTime += parseInt(challenge.timeTaken, 10);
      timeCount++;
    }
  });
  
  // Calculate average time
  const averageTime = timeCount > 0 ? Math.round(totalTime / timeCount) : 0;
  
  // Get top languages
  const topLanguages = Object.entries(languageCounts)
    .sort((a, b) => b[1] - a[1])
    .slice(0, 5)
    .map(([lang, count]) => `${lang}: ${count}`)
    .join('\n');
  
  // Get difficulty breakdown
  const difficultyBreakdown = Object.entries(difficultyCounts)
    .sort((a, b) => b[1] - a[1])
    .map(([diff, count]) => `${formatDifficulty(diff)}: ${count}`)
    .join('\n');
  
  // Create the embed
  const embed = new EmbedBuilder()
    .setTitle(`${interaction.user.username}'s Challenge Statistics`)
    .setColor(0x9B59B6)
    .addFields(
      { name: 'Total Challenges', value: challenges.length.toString(), inline: true },
      { name: 'Languages Used', value: Object.keys(languageCounts).length.toString(), inline: true },
      ...(averageTime ? [{ name: 'Average Time', value: `${averageTime} minutes`, inline: true }] : [])
    );
  
  if (topLanguages) {
    embed.addFields({ name: 'Top Languages', value: topLanguages });
  }
  
  if (difficultyBreakdown) {
    embed.addFields({ name: 'Difficulty Breakdown', value: difficultyBreakdown });
  }
  
  await interaction.reply({ embeds: [embed] });
}

// Handler for the catalog subcommand
async function handleCatalogCommand(interaction) {
  const catalog = challengeManager.getAllChallenges();
  
  if (catalog.length === 0) {
    await interaction.reply({
      content: 'The challenge catalog is empty. Add challenges with `/challenge add` to populate it!',
      ephemeral: true
    });
    return;
  }
  
  // Create the embed
  const embed = new EmbedBuilder()
    .setTitle('Challenge Catalog')
    .setColor(0x2ECC71)
    .setDescription('Here are all available challenges in our catalog:');
  
  // Paginate if necessary
  const MAX_FIELDS = 25; // Discord limits embeds to 25 fields
  const challengesToShow = catalog.slice(0, MAX_FIELDS);
  
  // Add challenges to the embed
  challengesToShow.forEach((challenge, index) => {
    const value = challenge.url ? `[Link](${challenge.url})` : 'No URL provided';
    embed.addFields({ name: `${index + 1}. ${challenge.title}`, value, inline: true });
  });
  
  if (catalog.length > MAX_FIELDS) {
    embed.setFooter({ text: `Showing ${MAX_FIELDS} of ${catalog.length} challenges` });
  }
  
  await interaction.reply({ embeds: [embed] });
}

// Helper function to get emoji for status
function getStatusEmoji(status) {
  switch (status) {
    case 'not_started': return '⚪';
    case 'in_progress': return '🔵';
    case 'completed': return '✅';
    case 'stuck': return '🔴';
    default: return '⚪';
  }
}

// Helper function to get color for status
function getStatusColor(status) {
  switch (status) {
    case 'not_started': return 0xCCCCCC;
    case 'in_progress': return 0x3498DB;
    case 'completed': return 0x2ECC71;
    case 'stuck': return 0xE74C3C;
    default: return 0xCCCCCC;
  }
}

// Helper function to format status
function formatStatus(status) {
  switch (status) {
    case 'not_started': return 'Not Started';
    case 'in_progress': return 'In Progress';
    case 'completed': return 'Completed';
    case 'stuck': return 'Stuck';
    default: return status;
  }
}

// Helper function to format difficulty
function formatDifficulty(difficulty) {
  switch (difficulty) {
    case 'easy': return 'Easy';
    case 'medium': return 'Medium';
    case 'hard': return 'Hard';
    case 'very_hard': return 'Very Hard';
    default: return difficulty;
  }
}

module.exports.postChallengeSummary = postChallengeSummary; 