const fs = require('fs');
const path = require('path');

// Quiz questions covering various programming topics
const quizQuestions = [
  {
    question: "What design pattern defines a family of algorithms, encapsulates each one, and makes them interchangeable?",
    answer: "Strategy Pattern",
    category: "Design Patterns"
  },
  {
    question: "What design pattern provides a surrogate or placeholder for another object to control access to it?",
    answer: "Proxy Pattern",
    category: "Design Patterns"
  },
  {
    question: "Which design pattern separates an algorithm from an object structure on which it operates?",
    answer: "Visitor Pattern",
    category: "Design Patterns"
  },
  {
    question: "What is the time complexity of binary search?",
    answer: "O(log n)",
    category: "Big O Notation"
  },
  {
    question: "What is the space complexity of merge sort?",
    answer: "O(n)",
    category: "Big O Notation"
  },
  {
    question: "What is the time complexity of quicksort in the worst case?",
    answer: "O(n²)",
    category: "Big O Notation"
  },
  {
    question: "In C#, what keyword is used to make a method override a base class method?",
    answer: "override",
    category: "C# Trivia"
  },
  {
    question: "What C# feature allows a class to inherit from multiple interfaces but only one class?",
    answer: "Interface inheritance",
    category: "C# Trivia"
  },
  {
    question: "What does LINQ stand for in C#?",
    answer: "Language Integrated Query",
    category: "C# Trivia"
  },
  {
    question: "What JavaScript method creates a new array with the results of calling a provided function on every element?",
    answer: "map()",
    category: "JavaScript"
  },
  {
    question: "What is the output of typeof null in JavaScript?",
    answer: "object",
    category: "JavaScript"
  },
  {
    question: "What does REST stand for in REST API?",
    answer: "Representational State Transfer",
    category: "Web Development"
  },
  {
    question: "What HTTP status code represents 'Not Found'?",
    answer: "404",
    category: "Web Development"
  },
  {
    question: "What does SQL stand for?",
    answer: "Structured Query Language",
    category: "Databases"
  },
  {
    question: "What type of join returns rows when there is at least one match in both tables?",
    answer: "INNER JOIN",
    category: "Databases"
  },
  {
    question: "What is the difference between a stack and a queue?",
    answer: "Stack is LIFO (Last In First Out), Queue is FIFO (First In First Out)",
    category: "Data Structures"
  },
  {
    question: "What is a hash collision and how can it be resolved?",
    answer: "A hash collision occurs when two different keys hash to the same value. It can be resolved using techniques like chaining or open addressing.",
    category: "Data Structures"
  },
  {
    question: "What is the singleton design pattern?",
    answer: "A design pattern that restricts the instantiation of a class to one single instance",
    category: "Design Patterns"
  },
  {
    question: "What does the acronym SOLID stand for in object-oriented design?",
    answer: "Single Responsibility, Open-Closed, Liskov Substitution, Interface Segregation, Dependency Inversion",
    category: "Design Principles"
  },
  {
    question: "What is dependency injection?",
    answer: "A technique where an object receives other objects that it depends on, rather than creating them internally",
    category: "Design Principles"
  }
];

// Path to store user scores
const SCORES_FILE = path.join(__dirname, '../data/quiz_scores.json');

// Ensure the scores file exists
if (!fs.existsSync(SCORES_FILE)) {
  fs.writeFileSync(SCORES_FILE, JSON.stringify({}, null, 2));
}

// Get random quiz questions
const getRandomQuestions = (count = 5) => {
  // Limit count to maximum available questions or 10, whichever is less
  const maxCount = Math.min(count, 10, quizQuestions.length);
  
  // Shuffle the questions
  const shuffled = [...quizQuestions].sort(() => 0.5 - Math.random());
  
  // Return requested number of questions
  return shuffled.slice(0, maxCount);
};

// Load user scores
const loadScores = () => {
  try {
    const data = fs.readFileSync(SCORES_FILE, 'utf8');
    return JSON.parse(data);
  } catch (error) {
    console.error('Error loading quiz scores:', error);
    return {};
  }
};

// Save user scores
const saveScore = (userId, username, score) => {
  try {
    const scores = loadScores();
    
    // Initialize user if they don't exist
    if (!scores[userId]) {
      scores[userId] = {
        username,
        totalScore: 0,
        quizzesTaken: 0,
        highScore: 0
      };
    }
    
    // Update scores
    scores[userId].totalScore += score;
    scores[userId].quizzesTaken += 1;
    scores[userId].highScore = Math.max(scores[userId].highScore, score);
    
    // Save back to file
    fs.writeFileSync(SCORES_FILE, JSON.stringify(scores, null, 2));
    
    return scores[userId];
  } catch (error) {
    console.error('Error saving quiz score:', error);
    return null;
  }
};

// Get user stats
const getUserStats = (userId) => {
  const scores = loadScores();
  return scores[userId] || null;
};

// Get top scores
const getTopScores = (limit = 5) => {
  const scores = loadScores();
  
  return Object.values(scores)
    .sort((a, b) => b.highScore - a.highScore)
    .slice(0, limit);
};

module.exports = {
  getRandomQuestions,
  saveScore,
  getUserStats,
  getTopScores
}; 