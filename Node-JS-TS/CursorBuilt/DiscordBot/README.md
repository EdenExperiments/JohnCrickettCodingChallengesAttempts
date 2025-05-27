# Discord Bot for Coding Challenges

A Discord bot that responds to greetings, provides random quotes, and manages coding challenges.

### Everything in this repo, aside from some small adjustments and cleanup, was complete by Cursor using Claude 3.7 via prompting
### I did debug myself and tell cursor my findings, but most changes to fix bugs was from cursor.

## Features

- **Greeting**: Responds to "Hello" with a personalized greeting.
- **Random Quotes**: Provides random quotes.
- **Random Challenges**: Gives random coding challenges.
- **Challenge Management**: 
  - List all available challenges.
  - Add new challenges from URLs (must be from codingchallenges.fyi).
- **Challenge Tracking**: Track challenge progress, language, difficulty, and time spent.
- **Quiz System**: Interactive flashcard-style coding questions with persistent scores.
- **Code Snippet Storage**: Store and retrieve code snippets with syntax highlighting.

## Setup

1. **Prerequisites**:
   - Node.js (LTS version recommended)
   - Discord Bot Token (from [Discord Developer Portal](https://discord.com/developers/applications))

2. **Installation**:
   ```bash
   # Clone the repository
   git clone <repository-url>
   cd discordbot

   # Install dependencies
   npm install
   ```

3. **Configuration**:
   - Create a `.env` file in the root directory
   - Add your Discord bot token:
     ```
     DISCORD_TOKEN=your_discord_bot_token_here
     ```

4. **Running the Bot**:
   ```bash
   # Start the bot
   npm start

   # For development with auto-restart
   npm run dev
   ```

## Commands

The bot supports both slash commands and text commands:

### Slash Commands (Preferred)
- `/hello` - Bot responds with a greeting including your username.
- `/quote` - Get a random quote from the dummyJSON API.
- `/challenge` - Get a random coding challenge from the catalog.
  - `/challenge catalog` - View all available challenges in the catalog.
  - `/challenge status` - View your current challenge progress.
  - `/challenge add` - Add or update a challenge with language, status, difficulty, etc.
  - `/challenge view` - View details for a specific challenge.
  - `/challenge list` - List all your challenges.
  - `/challenge stats` - View statistics about your challenges.
- `/greetcodecademy` - Provides a motivating message to fellow coding learners.
- `/quiz [questions]` - Start a coding quiz with optional number of questions (max 10).
- `/snippet upload` - Upload a new code snippet with syntax highlighting.
- `/snippet view` - View a saved code snippet.
- `/snippet list` - List all your saved snippets.
- `/snippet delete` - Delete a saved snippet.

### Text Commands (Legacy)
- `Hello` - Bot responds with a greeting including your username.
- `!quote` - Get a random quote from the dummyJSON API.
- `!challenge` - Get a random coding challenge.
- `!list` - List all available challenges.
- `!add <url>` - Add a new challenge from a URL.
- `!greetcodecademy` - Provides a motivating message to fellow coding learners.
- `!quiz [number]` - Start a coding quiz with optional number of questions (max 10).

## Creating a Discord Bot

1. Go to the [Discord Developer Portal](https://discord.com/developers/applications)
2. Create a New Application
3. Navigate to the "Bot" tab and click "Add Bot"
4. Under the "Privileged Gateway Intents" section, enable:
   - Server Members Intent
   - Message Content Intent
5. Under the "OAuth2" tab, select "bot" and "applications.commands" and the necessary permissions:
   - Read Messages/View Channels
   - Send Messages
   - Read Message History
   - Use Slash Commands
6. Use the generated URL to invite your bot to your server

## Data Structure

The bot uses a unified data structure stored in JSON files:

1. **challenges.json** - Contains both the challenge catalog and user progress:
   ```json
   {
     "catalog": [
       {
         "id": "unique-id",
         "title": "Challenge Title",
         "url": "https://challenge-url.com",
         "createdAt": "timestamp",
         "updatedAt": "timestamp"
       }
     ],
     "userProgress": {
       "userId": {
         "username": "Discord Username",
         "challenges": [
           {
             "name": "Challenge Name",
             "language": "Programming Language",
             "status": "in_progress",
             "difficulty": "medium",
             "timeTaken": 60,
             "createdAt": "timestamp",
             "updatedAt": "timestamp"
           }
         ]
       }
     }
   }
   ```

2. **snippets.json** - Stores code snippets:
   ```json
   {
     "userId": {
       "snippets": [
         {
           "id": "unique-id",
           "title": "Snippet Title",
           "language": "javascript",
           "code": "console.log('Hello World');",
           "createdAt": "timestamp"
         }
       ]
     }
   }
   ```

## Additional Setup

For the bot to function properly, make sure to:

1. Create a `data` directory in the root folder
2. The data files will be automatically created as needed 