const fs = require('fs');
const path = require('path');
const axios = require('axios');
const config = require('../config');

// Ensure the data directory exists
const dataDir = path.dirname(config.challengesFile);
if (!fs.existsSync(dataDir)) {
  fs.mkdirSync(dataDir, { recursive: true });
}

// Initialize challenges file if it doesn't exist
if (!fs.existsSync(config.challengesFile)) {
  fs.writeFileSync(config.challengesFile, JSON.stringify([], null, 2));
}

// Load challenges from the file
const loadChallenges = () => {
  try {
    const data = fs.readFileSync(config.challengesFile, 'utf8');
    return JSON.parse(data);
  } catch (error) {
    console.error('Error loading challenges:', error);
    return [];
  }
};

// Save challenges to the file
const saveChallenges = (challenges) => {
  try {
    fs.writeFileSync(config.challengesFile, JSON.stringify(challenges, null, 2));
    return true;
  } catch (error) {
    console.error('Error saving challenges:', error);
    return false;
  }
};

// Get a random challenge
const getRandomChallenge = (challenges) => {
  if (!challenges || challenges.length === 0) {
    return null;
  }
  const randomIndex = Math.floor(Math.random() * challenges.length);
  return challenges[randomIndex];
};

// Validate and add a new challenge
const addChallenge = async (url) => {
  try {
    // Check if URL is valid
    const urlObj = new URL(url);
    
    // Check if URL is from the valid domain
    if (!urlObj.hostname.includes(config.validChallengeDomain)) {
      return { success: false, message: 'The URL must be from codingchallenges.fyi' };
    }
    
    // Fetch the page to get the title
    const response = await axios.get(url);
    const html = response.data;
    
    // Extract title from HTML - use h1 tags instead of title
    const titleMatch = html.match(/<h1[^>]*>(.*?)<\/h1>/i);
    const title = titleMatch ? titleMatch[1].trim() : 'Unknown Challenge';
    
    // Create new challenge object
    const newChallenge = {
      id: Date.now().toString(),
      title,
      url
    };
    
    // Add to existing challenges
    const challenges = loadChallenges();
    challenges.push(newChallenge);
    saveChallenges(challenges);
    
    return { success: true, challenge: newChallenge };
  } catch (error) {
    console.error('Error adding challenge:', error);
    if (error.code === 'ERR_INVALID_URL') {
      return { success: false, message: 'Invalid URL format' };
    }
    return { success: false, message: 'Failed to add challenge' };
  }
};

module.exports = {
  loadChallenges,
  saveChallenges,
  getRandomChallenge,
  addChallenge
}; 