const fs = require('fs');
const path = require('path');

// Paths to the relevant files
const OLD_CHALLENGES_PATH = path.join(__dirname, '../data/challenges.json');
const OLD_PROGRESS_PATH = path.join(__dirname, '../data/challenge_progress.json');
const NEW_UNIFIED_PATH = path.join(__dirname, '../data/challenges.json');

// Function to migrate data
function migrateData() {
  console.log('Starting migration...');
  
  // Initialize the new unified structure
  const unifiedData = {
    catalog: [],
    userProgress: {}
  };
  
  // Load existing challenges if available
  if (fs.existsSync(OLD_CHALLENGES_PATH)) {
    try {
      const challenges = JSON.parse(fs.readFileSync(OLD_CHALLENGES_PATH, 'utf8'));
      console.log(`Found ${challenges.length} challenges in the old challenges file.`);
      
      // Map old challenges to the new format
      unifiedData.catalog = challenges.map(challenge => ({
        id: challenge.id || Date.now().toString() + Math.random().toString(36).substring(2, 9),
        title: challenge.title,
        url: challenge.url,
        createdAt: challenge.createdAt || new Date().toISOString(),
        updatedAt: challenge.updatedAt || new Date().toISOString()
      }));
      
      console.log(`Migrated ${unifiedData.catalog.length} challenges to the new catalog.`);
    } catch (error) {
      console.error('Error loading old challenges:', error);
    }
  } else {
    console.log('No old challenges file found.');
  }
  
  // Load existing progress if available
  if (fs.existsSync(OLD_PROGRESS_PATH)) {
    try {
      const progress = JSON.parse(fs.readFileSync(OLD_PROGRESS_PATH, 'utf8'));
      console.log(`Found progress data for ${Object.keys(progress).length} users.`);
      
      // Copy progress data
      unifiedData.userProgress = progress;
      
      // Make sure all challenges in user progress are also in the catalog
      Object.values(unifiedData.userProgress).forEach(user => {
        if (user.challenges) {
          user.challenges.forEach(challenge => {
            const challengeExists = unifiedData.catalog.some(
              c => c.title.toLowerCase() === challenge.name.toLowerCase()
            );
            
            if (!challengeExists) {
              unifiedData.catalog.push({
                id: Date.now().toString() + Math.random().toString(36).substring(2, 9),
                title: challenge.name,
                url: challenge.url || '',
                createdAt: challenge.createdAt || new Date().toISOString(),
                updatedAt: challenge.updatedAt || new Date().toISOString()
              });
            }
          });
        }
      });
      
      console.log(`Migrated user progress to the new structure.`);
    } catch (error) {
      console.error('Error loading old progress data:', error);
    }
  } else {
    console.log('No old progress file found.');
  }
  
  // Save the unified data
  try {
    fs.writeFileSync(NEW_UNIFIED_PATH, JSON.stringify(unifiedData, null, 2));
    console.log(`Saved unified data to ${NEW_UNIFIED_PATH}`);
    console.log(`Migration complete! The unified data structure now contains:`);
    console.log(`- ${unifiedData.catalog.length} challenges in the catalog`);
    console.log(`- Progress data for ${Object.keys(unifiedData.userProgress).length} users`);
  } catch (error) {
    console.error('Error saving unified data:', error);
  }
  
  // Backup the old files
  try {
    if (fs.existsSync(OLD_CHALLENGES_PATH)) {
      fs.copyFileSync(OLD_CHALLENGES_PATH, `${OLD_CHALLENGES_PATH}.bak`);
      console.log(`Backed up old challenges file to ${OLD_CHALLENGES_PATH}.bak`);
    }
    
    if (fs.existsSync(OLD_PROGRESS_PATH)) {
      fs.copyFileSync(OLD_PROGRESS_PATH, `${OLD_PROGRESS_PATH}.bak`);
      console.log(`Backed up old progress file to ${OLD_PROGRESS_PATH}.bak`);
    }
  } catch (error) {
    console.error('Error creating backups:', error);
  }
}

// Run the migration
migrateData(); 