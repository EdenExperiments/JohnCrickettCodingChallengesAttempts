const axios = require('axios');
const config = require('../config');

// Fetch a random quote from the dummyJSON API
const getRandomQuote = async () => {
  try {
    const response = await axios.get(config.quotesApi);
    return response.data;
  } catch (error) {
    console.error('Error fetching random quote:', error);
    // Return a fallback quote if the API fails
    return {
      id: 0,
      quote: "The API is currently unavailable. Try again later.",
      author: "Discord Bot"
    };
  }
};

module.exports = {
  getRandomQuote
}; 