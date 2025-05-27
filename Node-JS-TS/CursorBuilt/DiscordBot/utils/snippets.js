const fs = require('fs');
const path = require('path');

// Path to store code snippets
const SNIPPETS_FILE = path.join(__dirname, '../data/snippets.json');

// Ensure the snippets file exists
if (!fs.existsSync(SNIPPETS_FILE)) {
  fs.writeFileSync(SNIPPETS_FILE, JSON.stringify({}, null, 2));
}

// Load snippets
const loadSnippets = () => {
  try {
    const data = fs.readFileSync(SNIPPETS_FILE, 'utf8');
    return JSON.parse(data);
  } catch (error) {
    console.error('Error loading snippets:', error);
    return {};
  }
};

// Save snippets
const saveSnippets = (snippets) => {
  try {
    fs.writeFileSync(SNIPPETS_FILE, JSON.stringify(snippets, null, 2));
    return true;
  } catch (error) {
    console.error('Error saving snippets:', error);
    return false;
  }
};

// Add a new snippet
const addSnippet = (userId, username, name, code, language = 'text') => {
  try {
    const snippets = loadSnippets();
    
    // Initialize user if they don't exist
    if (!snippets[userId]) {
      snippets[userId] = {
        username,
        snippets: {}
      };
    }
    
    // Add or update snippet
    snippets[userId].snippets[name] = {
      code,
      language,
      createdAt: new Date().toISOString()
    };
    
    saveSnippets(snippets);
    return true;
  } catch (error) {
    console.error('Error adding snippet:', error);
    return false;
  }
};

// Get a snippet
const getSnippet = (userId, name) => {
  const snippets = loadSnippets();
  return snippets[userId]?.snippets[name] || null;
};

// Delete a snippet
const deleteSnippet = (userId, name) => {
  try {
    const snippets = loadSnippets();
    
    if (!snippets[userId] || !snippets[userId].snippets[name]) {
      return false;
    }
    
    delete snippets[userId].snippets[name];
    saveSnippets(snippets);
    return true;
  } catch (error) {
    console.error('Error deleting snippet:', error);
    return false;
  }
};

// List all snippets for a user
const listUserSnippets = (userId) => {
  const snippets = loadSnippets();
  return snippets[userId]?.snippets || {};
};

// Apply syntax highlighting using Discord markdown
const formatCodeWithSyntax = (code, language) => {
  // Map common language aliases to Discord markdown supported languages
  const languageMap = {
    'js': 'javascript',
    'ts': 'typescript',
    'py': 'python',
    'rb': 'ruby',
    'java': 'java',
    'c': 'c',
    'cpp': 'cpp',
    'cs': 'csharp',
    'php': 'php',
    'html': 'html',
    'css': 'css',
    'json': 'json',
    'yaml': 'yaml',
    'sql': 'sql',
    'shell': 'bash',
    'bash': 'bash',
    'powershell': 'powershell',
    'ps': 'powershell',
    'md': 'markdown'
  };
  
  // Get mapped language or default to original
  const formattedLang = languageMap[language.toLowerCase()] || language;
  
  // Apply Discord markdown formatting
  return `\`\`\`${formattedLang}\n${code}\n\`\`\``;
};

module.exports = {
  addSnippet,
  getSnippet,
  deleteSnippet,
  listUserSnippets,
  formatCodeWithSyntax
}; 