const fs = require('fs');
const path = require('path');

// Path to store challenge progress
const CHALLENGE_PROGRESS_FILE = path.join(__dirname, '../data/challenge_progress.json');

// Ensure the progress file exists
if (!fs.existsSync(CHALLENGE_PROGRESS_FILE)) {
  fs.writeFileSync(CHALLENGE_PROGRESS_FILE, JSON.stringify({}, null, 2));
}

// Load challenge progress
const loadChallengeProgress = () => {
  try {
    const data = fs.readFileSync(CHALLENGE_PROGRESS_FILE, 'utf8');
    return JSON.parse(data);
  } catch (error) {
    console.error('Error loading challenge progress:', error);
    return {};
  }
};

// Save challenge progress
const saveChallengeProgress = (progress) => {
  try {
    fs.writeFileSync(CHALLENGE_PROGRESS_FILE, JSON.stringify(progress, null, 2));
    return true;
  } catch (error) {
    console.error('Error saving challenge progress:', error);
    return false;
  }
};

// Update challenge progress for a user
const updateChallengeProgress = (userId, username, challengeData) => {
  try {
    const progress = loadChallengeProgress();
    
    // Initialize user if they don't exist
    if (!progress[userId]) {
      progress[userId] = {
        username,
        challenges: []
      };
    }
    
    // Check if challenge already exists
    const existingIndex = progress[userId].challenges.findIndex(
      c => c.name.toLowerCase() === challengeData.name.toLowerCase()
    );
    
    if (existingIndex >= 0) {
      // Update existing challenge
      progress[userId].challenges[existingIndex] = {
        ...progress[userId].challenges[existingIndex],
        ...challengeData,
        updatedAt: new Date().toISOString()
      };
    } else {
      // Add new challenge
      progress[userId].challenges.push({
        ...challengeData,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      });
    }
    
    saveChallengeProgress(progress);
    return progress[userId];
  } catch (error) {
    console.error('Error updating challenge progress:', error);
    return null;
  }
};

// Get all challenges for a user
const getUserChallenges = (userId) => {
  const progress = loadChallengeProgress();
  return progress[userId]?.challenges || [];
};

// Get a specific challenge for a user
const getUserChallenge = (userId, challengeName) => {
  const challenges = getUserChallenges(userId);
  return challenges.find(c => c.name.toLowerCase() === challengeName.toLowerCase());
};

// Get all users' challenge progress
const getAllChallengeProgress = () => {
  return loadChallengeProgress();
};

// Get statistics about challenges
const getChallengeStats = () => {
  const progress = loadChallengeProgress();
  const stats = {
    totalUsers: Object.keys(progress).length,
    totalChallenges: 0,
    languageStats: {},
    difficultyStats: {},
    averageCompletionTime: 0
  };
  
  let totalTime = 0;
  let timeCount = 0;
  
  // Process all challenges
  Object.values(progress).forEach(user => {
    stats.totalChallenges += user.challenges.length;
    
    user.challenges.forEach(challenge => {
      // Count languages
      if (challenge.language) {
        stats.languageStats[challenge.language] = 
          (stats.languageStats[challenge.language] || 0) + 1;
      }
      
      // Count difficulties
      if (challenge.difficulty) {
        stats.difficultyStats[challenge.difficulty] = 
          (stats.difficultyStats[challenge.difficulty] || 0) + 1;
      }
      
      // Sum time taken
      if (challenge.timeTaken) {
        totalTime += parseInt(challenge.timeTaken, 10);
        timeCount++;
      }
    });
  });
  
  // Calculate average time
  if (timeCount > 0) {
    stats.averageCompletionTime = Math.round(totalTime / timeCount);
  }
  
  return stats;
};

module.exports = {
  updateChallengeProgress,
  getUserChallenges,
  getUserChallenge,
  getAllChallengeProgress,
  getChallengeStats
};
