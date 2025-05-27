const { SlashCommandBuilder, EmbedBuilder, ActionRowBuilder, ButtonBuilder, ButtonStyle } = require('discord.js');
const quizUtils = require('../utils/quiz');

// Active quiz sessions
const activeQuizzes = new Map();

module.exports = {
  data: new SlashCommandBuilder()
    .setName('quiz')
    .setDescription('Start a coding quiz')
    .addIntegerOption(option =>
      option.setName('questions')
        .setDescription('Number of questions (max 10)')
        .setMinValue(1)
        .setMaxValue(10)
        .setRequired(false)),
  
  async execute(interaction) {
    // Check if user already has an active quiz
    if (activeQuizzes.has(interaction.user.id)) {
      await interaction.reply({
        content: 'You already have an active quiz! Finish that one first or type "cancel" to stop it.',
        ephemeral: true
      });
      return;
    }
    
    // Get number of questions requested
    const questionCount = interaction.options.getInteger('questions') || 5;
    
    // Get random questions
    const questions = quizUtils.getRandomQuestions(questionCount);
    
    // Create quiz session
    const quizSession = {
      questions,
      currentQuestion: 0,
      score: 0,
      startTime: Date.now(),
      messageId: null
    };
    
    // Store the session
    activeQuizzes.set(interaction.user.id, quizSession);
    
    // Start the quiz
    await startQuiz(interaction);
  },
  
  // For use in other files
  activeQuizzes,
  handleAnswer
};

// Function to start the quiz
async function startQuiz(interaction, isFollowUp = false) {
  const userId = interaction.user.id;
  const quizSession = activeQuizzes.get(userId);
  
  if (!quizSession) return;
  
  // If quiz is complete
  if (quizSession.currentQuestion >= quizSession.questions.length) {
    await endQuiz(interaction);
    return;
  }
  
  const currentQ = quizSession.questions[quizSession.currentQuestion];
  
  // Create embed for question
  const embed = new EmbedBuilder()
    .setColor(0x0099FF)
    .setTitle(`Question ${quizSession.currentQuestion + 1}/${quizSession.questions.length}`)
    .setDescription(currentQ.question)
    .setFooter({ text: `Category: ${currentQ.category} | Type your answer in the chat` });
  
  // Create buttons
  const row = new ActionRowBuilder()
    .addComponents(
      new ButtonBuilder()
        .setCustomId('quiz_show_answer')
        .setLabel('Show Answer')
        .setStyle(ButtonStyle.Primary),
      new ButtonBuilder()
        .setCustomId('quiz_skip')
        .setLabel('Skip Question')
        .setStyle(ButtonStyle.Secondary),
      new ButtonBuilder()
        .setCustomId('quiz_cancel')
        .setLabel('Cancel Quiz')
        .setStyle(ButtonStyle.Danger)
    );
  
  // Send the question - use reply for first question, followUp for subsequent questions
  let response;
  if (isFollowUp) {
    response = await interaction.followUp({
      embeds: [embed],
      components: [row],
      fetchReply: true
    });
  } else {
    response = await interaction.reply({
      embeds: [embed],
      components: [row],
      fetchReply: true
    });
  }
  
  // Update the quiz session with the message ID
  quizSession.messageId = response.id;
  activeQuizzes.set(userId, quizSession);
  
  // Create a message collector to listen for answers
  const filter = m => m.author.id === interaction.user.id;
  const collector = interaction.channel.createMessageCollector({ filter, time: 60000 });
  
  collector.on('collect', async message => {
    // Handle "cancel" command
    if (message.content.toLowerCase() === 'cancel') {
      collector.stop();
      await interaction.followUp({
        content: 'Quiz cancelled.',
        ephemeral: true
      });
      activeQuizzes.delete(userId);
      return;
    }
    
    // Check the answer
    const isCorrect = checkAnswer(message.content, currentQ.answer);
    await handleAnswer(interaction, isCorrect, currentQ.answer);
    collector.stop();
  });
  
  collector.on('end', (collected, reason) => {
    // If no answers were collected and the quiz wasn't cancelled
    if (collected.size === 0 && reason === 'time' && activeQuizzes.has(userId)) {
      // Move to the next question without awarding points
      quizSession.currentQuestion++;
      activeQuizzes.set(userId, quizSession);
      
      // Send timeout message
      interaction.followUp({
        content: `Time's up! The correct answer was: "${currentQ.answer}"`,
        ephemeral: false
      }).then(() => {
        // Continue with the next question
        startQuiz(interaction, true);
      });
    }
  });
}

// Check if an answer is correct (with some flexibility)
function checkAnswer(userAnswer, correctAnswer) {
  // Convert both to lowercase and remove punctuation for comparison
  const normalize = (str) => str.toLowerCase().replace(/[.,\/#!$%\^&\*;:{}=\-_`~()]/g, "").trim();
  
  const normalizedUser = normalize(userAnswer);
  const normalizedCorrect = normalize(correctAnswer);
  
  // Check for exact match
  if (normalizedUser === normalizedCorrect) {
    return true;
  }
  
  // Check if correct answer contains user answer (for partial matches)
  if (normalizedUser.length > 3 && normalizedCorrect.includes(normalizedUser)) {
    return true;
  }
  
  // For very short answers, require exact match
  return false;
}

// Handle a user's answer
async function handleAnswer(interaction, isCorrect, correctAnswer) {
  const userId = interaction.user.id;
  const quizSession = activeQuizzes.get(userId);
  
  if (!quizSession) return;
  
  if (isCorrect) {
    // Award a point
    quizSession.score++;
    
    // Send correct message
    await interaction.followUp({
      content: `✅ Correct! Your current score: ${quizSession.score}/${quizSession.questions.length}`,
      ephemeral: false
    });
  } else {
    // Send incorrect message
    await interaction.followUp({
      content: `❌ Incorrect. The correct answer was: "${correctAnswer}"`,
      ephemeral: false
    });
  }
  
  // Move to next question
  quizSession.currentQuestion++;
  activeQuizzes.set(userId, quizSession);
  
  // Continue the quiz with the followUp flag set to true
  setTimeout(() => {
    startQuiz(interaction, true);
  }, 1500);
}

// End the quiz and show results
async function endQuiz(interaction) {
  const userId = interaction.user.id;
  const quizSession = activeQuizzes.get(userId);
  
  if (!quizSession) return;
  
  // Calculate time taken
  const timeElapsed = ((Date.now() - quizSession.startTime) / 1000).toFixed(1);
  
  // Save score
  const userStats = quizUtils.saveScore(
    userId,
    interaction.user.username,
    quizSession.score
  );
  
  // Create results embed
  const embed = new EmbedBuilder()
    .setColor(0x00FF00)
    .setTitle('Quiz Results')
    .setDescription(`Quiz completed, ${interaction.user.username}!`)
    .addFields(
      { name: 'Final Score', value: `${quizSession.score}/${quizSession.questions.length}`, inline: true },
      { name: 'Time Taken', value: `${timeElapsed} seconds`, inline: true },
      { name: 'High Score', value: userStats ? `${userStats.highScore}` : 'N/A', inline: true },
      { name: 'Quizzes Taken', value: userStats ? `${userStats.quizzesTaken}` : '0', inline: true },
      { name: 'Total Score', value: userStats ? `${userStats.totalScore}` : '0', inline: true }
    )
    .setTimestamp();
  
  // Send results
  await interaction.followUp({ embeds: [embed] });
  
  // Clean up the session
  activeQuizzes.delete(userId);
}

// Handle button interactions - needs to be called from index.js
async function handleQuizButtons(interaction) {
  const userId = interaction.user.id;
  const quizSession = activeQuizzes.get(userId);
  
  if (!quizSession) {
    await interaction.reply({
      content: "You don't have an active quiz session.",
      ephemeral: true
    });
    return;
  }
  
  // Get current question
  const currentQ = quizSession.questions[quizSession.currentQuestion];
  
  switch (interaction.customId) {
    case 'quiz_show_answer':
      await interaction.reply({
        content: `The answer is: "${currentQ.answer}"`,
        ephemeral: true
      });
      break;
      
    case 'quiz_skip':
      // First, update the buttons to disable them
      try {
        await interaction.update({ components: [] });
      } catch (error) {
        console.error("Error updating buttons:", error);
        // If updating fails, continue anyway
      }
      
      await interaction.followUp({
        content: `Question skipped. The answer was: "${currentQ.answer}"`,
        ephemeral: false
      });
      
      // Move to next question
      quizSession.currentQuestion++;
      activeQuizzes.set(userId, quizSession);
      
      // Continue quiz
      setTimeout(() => {
        startQuiz(interaction, true);
      }, 1500);
      break;
      
    case 'quiz_cancel':
      try {
        await interaction.update({ components: [] });
      } catch (error) {
        console.error("Error updating buttons:", error);
        // If updating fails, continue anyway
      }
      
      await interaction.followUp({
        content: 'Quiz cancelled.',
        ephemeral: false
      });
      activeQuizzes.delete(userId);
      break;
  }
} 