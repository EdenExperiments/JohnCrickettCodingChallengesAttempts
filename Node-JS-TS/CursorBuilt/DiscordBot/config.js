require('dotenv').config();

module.exports = {

  
  // Channel ID for automatic challenge summaries
  summaryChannelId: process.env.SUMMARY_CHANNEL_ID || '',
  
  // API endpoints
  quotesApi: 'https://dummyjson.com/quotes/random',
  
  // Local storage for challenges
  challengesFile: './data/challenges.json',
  
  // Valid challenge domain
  validChallengeDomain: 'codingchallenges.fyi'
}; 