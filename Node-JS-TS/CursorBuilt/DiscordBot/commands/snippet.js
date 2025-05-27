const { SlashCommandBuilder, EmbedBuilder } = require('discord.js');
const snippetUtils = require('../utils/snippets');

module.exports = {
  data: new SlashCommandBuilder()
    .setName('snippet')
    .setDescription('Manage your code snippets')
    .addSubcommand(subcommand =>
      subcommand
        .setName('upload')
        .setDescription('Upload a new code snippet')
        .addStringOption(option =>
          option.setName('name')
            .setDescription('Name for the snippet')
            .setRequired(true))
        .addStringOption(option =>
          option.setName('code')
            .setDescription('The code to save')
            .setRequired(true))
        .addStringOption(option =>
          option.setName('language')
            .setDescription('Programming language (for syntax highlighting)')
            .setRequired(false)))
    .addSubcommand(subcommand =>
      subcommand
        .setName('view')
        .setDescription('View a saved code snippet')
        .addStringOption(option =>
          option.setName('name')
            .setDescription('Name of the snippet to view')
            .setRequired(true)))
    .addSubcommand(subcommand =>
      subcommand
        .setName('list')
        .setDescription('List all your saved snippets'))
    .addSubcommand(subcommand =>
      subcommand
        .setName('delete')
        .setDescription('Delete a saved snippet')
        .addStringOption(option =>
          option.setName('name')
            .setDescription('Name of the snippet to delete')
            .setRequired(true))),
    
  async execute(interaction) {
    const subcommand = interaction.options.getSubcommand();
    
    switch (subcommand) {
      case 'upload':
        await handleUploadCommand(interaction);
        break;
      case 'view':
        await handleViewCommand(interaction);
        break;
      case 'list':
        await handleListCommand(interaction);
        break;
      case 'delete':
        await handleDeleteCommand(interaction);
        break;
    }
  }
};

// Handler for the upload subcommand
async function handleUploadCommand(interaction) {
  const userId = interaction.user.id;
  const username = interaction.user.username;
  
  const name = interaction.options.getString('name');
  let code = interaction.options.getString('code');
  const language = interaction.options.getString('language') || 'text';
  
  // Clean the code if it has Discord code block formatting
  if (code.startsWith('```') && code.endsWith('```')) {
    // Remove the opening and closing backticks
    const lines = code.split('\n');
    
    // Remove the first line (which might include the language)
    lines.shift();
    
    // Remove the last line (closing backticks)
    lines.pop();
    
    code = lines.join('\n');
  }
  
  // Add the snippet
  const result = snippetUtils.addSnippet(userId, username, name, code, language);
  
  if (result) {
    await interaction.reply({
      content: `📋 Snippet **${name}** has been saved! Use \`/snippet view ${name}\` to view it.`,
      ephemeral: false
    });
  } else {
    await interaction.reply({
      content: 'There was an error saving your snippet. Please try again.',
      ephemeral: true
    });
  }
}

// Handler for the view subcommand
async function handleViewCommand(interaction) {
  const userId = interaction.user.id;
  const name = interaction.options.getString('name');
  
  const snippet = snippetUtils.getSnippet(userId, name);
  
  if (!snippet) {
    await interaction.reply({
      content: `Could not find a snippet named "${name}". Please check the name and try again.`,
      ephemeral: true
    });
    return;
  }
  
  // Format the code with syntax highlighting
  const formattedCode = snippetUtils.formatCodeWithSyntax(snippet.code, snippet.language);
  
  // Create the embed
  const embed = new EmbedBuilder()
    .setTitle(`Snippet: ${name}`)
    .setColor(0x00BFFF)
    .setDescription(`Language: ${snippet.language}`)
    .setFooter({ text: `Created: ${new Date(snippet.createdAt).toLocaleDateString()}` });
  
  // Send the snippet
  await interaction.reply({
    embeds: [embed],
    content: formattedCode
  });
}

// Handler for the list subcommand
async function handleListCommand(interaction) {
  const userId = interaction.user.id;
  const snippets = snippetUtils.listUserSnippets(userId);
  
  if (Object.keys(snippets).length === 0) {
    await interaction.reply({
      content: 'You haven\'t saved any snippets yet. Use `/snippet upload` to add one!',
      ephemeral: true
    });
    return;
  }
  
  // Create the embed
  const embed = new EmbedBuilder()
    .setTitle(`${interaction.user.username}'s Code Snippets`)
    .setColor(0x00BFFF)
    .setDescription('Here are all your saved code snippets:');
  
  // Add snippets to the embed
  const snippetList = Object.entries(snippets)
    .map(([name, data]) => {
      const language = data.language || 'text';
      const codePreview = data.code.length > 30 
        ? `${data.code.substring(0, 30)}...` 
        : data.code;
      
      return `**${name}** (${language})\n\`\`\`${language}\n${codePreview}\n\`\`\``;
    })
    .join('\n\n');
  
  if (snippetList.length <= 4096) {
    embed.setDescription(snippetList);
  } else {
    // If the list is too long, just show the names
    const namesList = Object.entries(snippets)
      .map(([name, data]) => `**${name}** (${data.language || 'text'})`)
      .join('\n');
    
    embed.setDescription('Here are all your saved code snippets:')
      .addFields({ name: 'Snippets', value: namesList });
  }
  
  await interaction.reply({ embeds: [embed] });
}

// Handler for the delete subcommand
async function handleDeleteCommand(interaction) {
  const userId = interaction.user.id;
  const name = interaction.options.getString('name');
  
  const result = snippetUtils.deleteSnippet(userId, name);
  
  if (result) {
    await interaction.reply({
      content: `✅ Snippet **${name}** has been deleted.`,
      ephemeral: false
    });
  } else {
    await interaction.reply({
      content: `Could not find a snippet named "${name}". Please check the name and try again.`,
      ephemeral: true
    });
  }
} 