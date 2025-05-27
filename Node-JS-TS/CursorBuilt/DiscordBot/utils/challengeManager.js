const fs = require('fs');
const path = require('path');

// Path to store unified challenge data
const CHALLENGES_FILE = path.join(__dirname, '../data/challenges.json');

// Ensure the challenges file exists
if (!fs.existsSync(CHALLENGES_FILE)) {
  const initialStructure = {
    catalog: [],  // List of all known challenges
    userProgress: {}  // User progress by userId
  };
  fs.writeFileSync(CHALLENGES_FILE, JSON.stringify(initialStructure, null, 2));
}

// Load challenges data
const loadChallengesData = () => {
  try {
    const data = fs.readFileSync(CHALLENGES_FILE, 'utf8');
    const parsed = JSON.parse(data);
    
    // Ensure the structure is valid
    if (!parsed.catalog) parsed.catalog = [];
    if (!parsed.userProgress) parsed.userProgress = {};
    
    return parsed;
  } catch (error) {
    console.error('Error loading challenges data:', error);
    return { catalog: [], userProgress: {} };
  }
};

// Save challenges data
const saveChallengesData = (data) => {
  try {
    fs.writeFileSync(CHALLENGES_FILE, JSON.stringify(data, null, 2));
    return true;
  } catch (error) {
    console.error('Error saving challenges data:', error);
    return false;
  }
};

// Add a challenge to the catalog
const addChallengeToCatalog = async (url, title) => {
  try {
    const data = loadChallengesData();
    
    // Check if challenge already exists
    const existingIndex = data.catalog.findIndex(
      c => c.url.toLowerCase() === url.toLowerCase() || 
           c.title.toLowerCase() === title.toLowerCase()
    );
    
    if (existingIndex >= 0) {
      // Update existing challenge
      data.catalog[existingIndex] = {
        ...data.catalog[existingIndex],
        title,
        url,
        updatedAt: new Date().toISOString()
      };
    } else {
      // Add new challenge
      data.catalog.push({
        id: Date.now().toString(),
        title,
        url,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      });
    }
    
    saveChallengesData(data);
    return { success: true, challenge: data.catalog.find(c => c.url === url) };
  } catch (error) {
    console.error('Error adding challenge to catalog:', error);
    return { success: false, message: 'Failed to add challenge' };
  }
};

// Get a random challenge from the catalog
const getRandomChallenge = () => {
  const data = loadChallengesData();
  
  if (!data.catalog || data.catalog.length === 0) {
    return null;
  }
  
  const randomIndex = Math.floor(Math.random() * data.catalog.length);
  return data.catalog[randomIndex];
};

// Get all challenges from the catalog
const getAllChallenges = () => {
  const data = loadChallengesData();
  return data.catalog || [];
};

// Update challenge progress for a user
const updateChallengeProgress = (userId, username, challengeData) => {
  try {
    const data = loadChallengesData();
    
    // Initialize user if they don't exist
    if (!data.userProgress[userId]) {
      data.userProgress[userId] = {
        username,
        challenges: []
      };
    }
    
    // Check if challenge already exists
    const existingIndex = data.userProgress[userId].challenges.findIndex(
      c => c.name.toLowerCase() === challengeData.name.toLowerCase()
    );
    
    if (existingIndex >= 0) {
      // Update existing challenge
      data.userProgress[userId].challenges[existingIndex] = {
        ...data.userProgress[userId].challenges[existingIndex],
        ...challengeData,
        updatedAt: new Date().toISOString()
      };
    } else {
      // Add new challenge
      data.userProgress[userId].challenges.push({
        ...challengeData,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      });
      
      // If this is a new challenge not in the catalog, add it
      const catalogHasChallenge = data.catalog.some(
        c => c.title.toLowerCase() === challengeData.name.toLowerCase()
      );
      
      if (!catalogHasChallenge) {
        data.catalog.push({
          id: Date.now().toString(),
          title: challengeData.name,
          url: challengeData.url || '',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        });
      }
    }
    
    saveChallengesData(data);
    return data.userProgress[userId];
  } catch (error) {
    console.error('Error updating challenge progress:', error);
    return null;
  }
};

// Get all challenges for a user
const getUserChallenges = (userId) => {
  const data = loadChallengesData();
  return data.userProgress[userId]?.challenges || [];
};

// Get a specific challenge for a user
const getUserChallenge = (userId, challengeName) => {
  const challenges = getUserChallenges(userId);
  return challenges.find(c => c.name.toLowerCase() === challengeName.toLowerCase());
};

// Get statistics about challenges
const getChallengeStats = () => {
  const data = loadChallengesData();
  const stats = {
    totalUsers: Object.keys(data.userProgress).length,
    totalChallenges: data.catalog.length,
    totalInProgress: 0,
    languageStats: {},
    difficultyStats: {},
    averageCompletionTime: 0
  };
  
  let totalTime = 0;
  let timeCount = 0;
  
  // Process all user challenges
  Object.values(data.userProgress).forEach(user => {
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
      
      // Count in-progress challenges
      if (challenge.status === 'in_progress') {
        stats.totalInProgress++;
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

// Function to migrate existing data to the new structure
const migrateExistingData = () => {
  try {
    // Load existing challenge_progress.json if it exists
    const progressFilePath = path.join(__dirname, '../data/challenge_progress.json');
    let userProgress = {};
    
    if (fs.existsSync(progressFilePath)) {
      try {
        userProgress = JSON.parse(fs.readFileSync(progressFilePath, 'utf8'));
      } catch (e) {
        console.error('Error reading challenge_progress.json:', e);
      }
    }
    
    // Load existing challenges
    const data = loadChallengesData();
    
    // Merge user progress
    data.userProgress = { ...data.userProgress, ...userProgress };
    
    // Make sure all user challenges are in the catalog
    Object.values(data.userProgress).forEach(user => {
      user.challenges.forEach(challenge => {
        const catalogHasChallenge = data.catalog.some(
          c => c.title.toLowerCase() === challenge.name.toLowerCase()
        );
        
        if (!catalogHasChallenge) {
          data.catalog.push({
            id: Date.now().toString() + Math.random().toString(36).substring(2, 9),
            title: challenge.name,
            url: challenge.url || '',
            createdAt: challenge.createdAt || new Date().toISOString(),
            updatedAt: challenge.updatedAt || new Date().toISOString()
          });
        }
      });
    });
    
    // Save the merged data
    saveChallengesData(data);
    
    return true;
  } catch (error) {
    console.error('Error migrating data:', error);
    return false;
  }
};

// Try to migrate existing data on load
migrateExistingData();

module.exports = {
  addChallengeToCatalog,
  getRandomChallenge,
  getAllChallenges,
  updateChallengeProgress,
  getUserChallenges,
  getUserChallenge,
  getChallengeStats,
  migrateExistingData
}; 