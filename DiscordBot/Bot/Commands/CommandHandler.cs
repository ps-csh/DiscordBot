using DiscordBot.Configuration;
using DiscordBot.DiscordAPI;
using DiscordBot.DiscordAPI.Structures;
using DiscordBot.Modules;
using DiscordBot.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DiscordBot.Bot.Commands
{
    public class CommandHandler
    {
        public DiscordApiClient ApiClient { get; private set; }
        public ILogger Logger { get; private set; }
        private readonly string adminID;
        //TODO: Find a way to set this with DI. It is currently set manually through BotManager.
        //BotManager has a depency on this class, so we cannot include BotManager for DI here.
        public DiscordBotUser BotUser { get; set; }

        public Dictionary<string, BotCommand> BotCommands { get; private set; }

        /// <summary>
        /// Strings used to identify the message as a command (such as '!')
        /// </summary>
        public List<string> CommandIdentifiers { get; private set; }

        /// <summary>
        /// Handles temporary add-on commands from a previous command
        /// </summary>
        public Dictionary<string, TemporaryCommandHandler> TemporaryCommandHandlers { get; set; }

        public CommandHandler(IOptions<BotSettingsConfiguration> options, CommandService commandService, 
            DiscordApiClient discordApiClient, ILogger logger)
        {
            CommandIdentifiers = options.Value.CommandIdentifiers;
            adminID = options.Value.AdminID;

            BotCommands = new Dictionary<string, BotCommand>();
            TemporaryCommandHandlers = new Dictionary<string, TemporaryCommandHandler>();

            ApiClient = discordApiClient;
            Logger = logger;

            LoadCommands(commandService);
        }

        public void LoadCommands(CommandService commandService)
        {
            var commands = commandService.GetCommands();
            foreach (var c in commands)
            {
                try
                {
                    // All commands are stored in lower case, bot commands are case insensitive
                    BotCommands.Add(c.Key.ToLower(), new BotCommand(c.Value));
                    Logger.LogInfo($"{c.Key.ToLower()} added to commands");
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }

            Logger.LogInfo($"{BotCommands.Count} commands loaded in CommandHandler ({commands.Count()} expected).");
        }

        public virtual async void HandleMessage(FullMessage message)
        {
            if (TemporaryCommandHandlers.TryGetValue(message.Channel.ID, out TemporaryCommandHandler tempHandler))
            {
                if (tempHandler.HandleMessage(message))
                {
                    // If the temporary handler reads a command, skip any other command handling
                    Logger.LogInfo($"TempHandler handled command: {message.Content}\nIn channel: {message.Channel}");

                    return;
                }
            }

            if (ParseCommand(message, out var command, out var parameters))
            {
                if (BotCommands.TryGetValue(command.ToLower(), out BotCommand botCommand))
                {
                    CommandInfo commandInfo = new CommandInfo()
                    {
                        Message = message,
                        Command = command,
                        Arguments = parameters,
                        CommandHandler = this,
                    };

                    TemporaryCommandHandlers.Remove(message.Channel.ID);

                    botCommand.Execute(commandInfo, commandInfo.ApiClient);
                }
                else
                {
                    await ApiClient.PostMessage($"There is no command for {command}", message.Channel.ID);
                }
            }
            else
            {
                //TODO: Handle non-command responses
                //TEMP: replace bot with a reference to BotUser.UserName
                try
                {
                    if (BotUser?.Username == null)
                    {
                        Logger.LogWarning("BotUser Username was not set");
                        return;
                    }
                    else if (message?.Channel?.ID == null)
                    {
                        Logger.LogWarning("Channel is null");
                        return;
                    }

                    Logger.LogInfo($"Received non command message. {message.Content}, {message.Sender.Nickname ?? message.Sender.Username}, {message.Channel.ID}");

                    if (message.Content.Trim().ToLower() == $"hello {BotUser.Username.ToLower()}"
                        || message.Content.Trim().ToLower() == $"hi {BotUser.Username.ToLower()}"
                        || message.Content.Trim().ToLower() == $"hey {BotUser.Username.ToLower()}")
                    {
                        int x = new Random().Next(0, 3);
                        string greeting = x == 0 ? "Hi" : x == 1 ? "Hello" : "Hey";
                        await ApiClient.PostMessage($"{greeting} {message.Sender.Nickname ?? message.Sender.Username}", message.Channel.ID);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        public void AddTemporaryCommandHandler(TemporaryCommandHandler temporaryCommandHandler, string channelId)
        {
            if (TemporaryCommandHandlers.ContainsKey(channelId))
            {
                TemporaryCommandHandlers[channelId] = temporaryCommandHandler;
                Logger.LogInfo($"TempHandler replaced for channel: {channelId}");
            }
            else
            {
                TemporaryCommandHandlers.Add(channelId, temporaryCommandHandler);
                Logger.LogInfo($"TempHandler created for channel: {channelId}");
            }
        }

        private bool ParseCommand(FullMessage message, out string command, out string parameters)
        {
            string[] args = message.Content.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length < 2)
            {
                command = null;
                parameters = null;
                return false;
            }

            command = args[1];
            parameters = args.Length == 3 ? args[2] : null;
            return MessageHasCommand(args[0]);
        }

        private bool MessageHasCommand(string message)
        {
            return CommandIdentifiers.Any(c => message.ToLower().StartsWith(c.ToLower()));
        }
    }
}
