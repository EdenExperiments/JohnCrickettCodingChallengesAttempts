const { Client, GatewayIntentBits, Events, REST, Routes, Collection } = require('discord.js');
const fs = require('node:fs');
const path = require('node:path');
const config = require('./config');
const quoteUtils = require('./utils/quotes');
const challengeManager = require('./utils/challengeManager');

// Create a new client instance
const client = new Client({
  intents: [
    GatewayIntentBits.Guilds,
    GatewayIntentBits.GuildMessages,
    GatewayIntentBits.MessageContent
  ]
});

// Store challenges in memory with an update function
const challengesRef = {
  data: [],
  update: function(newChallenges) {
    this.data = newChallenges;
  }
};

// Load commands from files
client.commands = new Collection();
const commandsPath = path.join(__dirname, 'commands');
const commandFiles = fs.readdirSync(commandsPath).filter(file => file.endsWith('.js'));

for (const file of commandFiles) {
  const filePath = path.join(commandsPath, file);
  const command = require(filePath);
  // Set a new item in the Collection with the key as the command name and the value as the exported module
  if ('data' in command && 'execute' in command) {
    client.commands.set(command.data.name, command);
  } else {
    console.log(`[WARNING] The command at ${filePath} is missing a required "data" or "execute" property.`);
  }
}

// When the client is ready, run this code (only once)
client.once(Events.ClientReady, async () => {
  console.log(`Logged in as ${client.user.tag}!`);
  
  // Load challenges from our unified manager
  const challenges = challengeManager.getAllChallenges();
  console.log('Raw challenges data:', JSON.stringify(challenges, null, 2));
  challengesRef.data = challenges;
  console.log(`Loaded ${challengesRef.data.length} challenges from unified catalog`);
  
  // Register slash commands
  try {
    console.log('Started refreshing application (/) commands.');
    
    const commands = Array.from(client.commands.values()).map(command => command.data.toJSON());
    const rest = new REST({ version: '10' }).setToken(config.token);
    
    await rest.put(
      Routes.applicationCommands(config.clientId),
      { body: commands },
    );
    
    console.log('Successfully reloaded application (/) commands.');
  } catch (error) {
    console.error('Error refreshing application commands:', error);
  }
  
  // Set up the challenge summary scheduler - post every day at 9 AM
  // Replace 'CHANNEL_ID_HERE' with the actual channel ID for summaries
  setupChallengeSummaryScheduler(client, process.env.SUMMARY_CHANNEL_ID || config.summaryChannelId);
});

// Function to set up the challenge summary scheduler
function setupChallengeSummaryScheduler(client, channelId) {
  if (!channelId) {
    console.log('No summary channel ID provided. Challenge summaries will not be posted automatically.');
    return;
  }
  
  console.log(`Setting up challenge summary scheduler for channel ID: ${channelId}`);
  
  // Function to post the summary
  const postSummary = async () => {
    try {
      const challengeStatusCommand = require('./commands/challengeStatus');
      await challengeStatusCommand.postChallengeSummary(client, channelId);
      console.log('Posted challenge summary successfully.');
    } catch (error) {
      console.error('Error posting challenge summary:', error);
    }
  };
  
  // Calculate time until 9 AM
  const calculateDelay = () => {
    const now = new Date();
    const next9AM = new Date(
      now.getFullYear(),
      now.getMonth(),
      now.getHours() >= 9 ? now.getDate() + 1 : now.getDate(),
      9, 0, 0
    );
    
    return next9AM - now;
  };
  
  // Function to schedule the next summary
  const scheduleNext = () => {
    const delay = calculateDelay();
    console.log(`Next summary scheduled in ${Math.floor(delay / (1000 * 60 * 60))} hours.`);
    
    setTimeout(() => {
      postSummary().then(scheduleNext);
    }, delay);
  };
  
  // Start the scheduling
  scheduleNext();
}

// Handle slash command interactions
client.on(Events.InteractionCreate, async interaction => {
  if (interaction.isChatInputCommand()) {
    const command = client.commands.get(interaction.commandName);
    
    if (!command) {
      console.error(`No command matching ${interaction.commandName} was found.`);
      return;
    }
    
    try {
      // Pass in challengesRef for commands that need it
      await command.execute(interaction, challengesRef);
    } catch (error) {
      console.error(`Error executing ${interaction.commandName}`);
      console.error(error);
      if (interaction.replied || interaction.deferred) {
        await interaction.followUp({ content: 'There was an error while executing this command!', ephemeral: true });
      } else {
        await interaction.reply({ content: 'There was an error while executing this command!', ephemeral: true });
      }
    }
  } else if (interaction.isButton()) {
    // Handle button interactions
    if (interaction.customId.startsWith('quiz_')) {
      try {
        const quizCommand = require('./commands/quiz');
        await quizCommand.handleQuizButtons(interaction);
      } catch (error) {
        console.error('Error handling quiz button:', error);
      }
    }
  }
});

// For backward compatibility - keep message-based commands
client.on(Events.MessageCreate, async message => {
  // Ignore messages from bots
  if (message.author.bot) return;

  // Hello response
  if (message.content.toLowerCase() === 'hello') {
    await message.reply(`Hello, ${message.author.username}!`);
    return;
  }

  // Random quote command
  if (message.content.toLowerCase() === '!quote') {
    const quoteData = await quoteUtils.getRandomQuote();
    await message.reply(`"${quoteData.quote}" - ${quoteData.author}`);
    return;
  }

  // Random challenge command
  if (message.content.toLowerCase() === '!challenge') {
    const challenge = challengeManager.getRandomChallenge();
    if (challenge) {
      await message.reply(`Challenge: ${challenge.title}\n${challenge.url}`);
    } else {
      await message.reply('No challenges available. Try adding some with /add command.');
    }
    return;
  }

  // List all challenges
  if (message.content.toLowerCase() === '!list') {
    const challenges = challengeManager.getAllChallenges();
    
    if (challenges.length === 0) {
      await message.reply('No challenges available. Try adding some with /add command.');
      return;
    }
    
    // Create a formatted list of challenges with challenge number, name, and URL
    const challengeList = challenges
      .map((challenge, index) => `${index + 1}. ${challenge.title} : ${challenge.url}`)
      .join('\n');
    
    await message.reply(`Available Challenges:\n${challengeList}`);
    return;
  }

  // Add a new challenge
  if (message.content.toLowerCase().startsWith('!add ')) {
    const url = message.content.slice(5).trim();
    
    // Extract title from URL (simplified, you may want more robust extraction)
    const title = url.split('/').pop() || 'Unknown Challenge';
    
    const result = await challengeManager.addChallengeToCatalog(url, title);
    
    if (result.success) {
      // Update the in-memory challenges
      challengesRef.data = challengeManager.getAllChallenges();
      await message.reply(`Challenge added: ${result.challenge.title}`);
    } else {
      await message.reply(`Failed to add challenge: ${result.message}`);
    }
    return;
  }
  
  // Greet Codecademy learners with motivational message
  if (message.content.toLowerCase() === '!greetcodecademy') {
    // Get the greetcodecademy command
    const greetCommand = require('./commands/greetcodecademy');
    
    // Get the response text by simulating the execute function with a fake interaction
    const fakeInteraction = {
      reply: async (text) => {
        await message.reply(text);
      }
    };
    
    // Execute the command
    await greetCommand.execute(fakeInteraction);
    return;
  }
  
  // Start a quiz
  if (message.content.toLowerCase().startsWith('!quiz')) {
    const args = message.content.split(' ');
    
    // Get the quiz command
    const quizCommand = require('./commands/quiz');
    
    // Check if user already has an active quiz
    if (quizCommand.activeQuizzes.has(message.author.id)) {
      await message.reply('You already have an active quiz! Finish that one first or type "cancel" to stop it.');
      return;
    }
    
    // Parse number of questions
    let questionCount = 5;
    if (args.length > 1) {
      const parsed = parseInt(args[1]);
      if (!isNaN(parsed) && parsed > 0 && parsed <= 10) {
        questionCount = parsed;
      }
    }
    
    // Create a fake interaction object
    const fakeInteraction = {
      user: message.author,
      channel: message.channel,
      reply: async (content) => {
        if (typeof content === 'string') {
          return await message.reply(content);
        } else {
          return await message.reply(content);
        }
      },
      followUp: async (content) => {
        if (typeof content === 'string') {
          return await message.channel.send(content);
        } else {
          return await message.channel.send(content);
        }
      },
      options: {
        getInteger: () => questionCount
      }
    };
    
    // Execute the command
    await quizCommand.execute(fakeInteraction);
    return;
  }
});

// Login to Discord with your token
client.login(config.token).catch(error => {
  console.error('Failed to login to Discord:', error);
}); 