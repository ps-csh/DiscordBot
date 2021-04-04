using DiscordBot.DiscordAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.Commands
{
    //TODO: Find a different name
    public class TemporaryCommandHandler
    {
        public DiscordBotManager DiscordBot { get; private set; }
        public CommandHandler ParentCommandHandler { get; private set; }
        // The user that sent the original command
        public User User { get; private set; }

        protected Dictionary<string, BotCommand> BotCommands { get; set; }

        public List<string> CommandIdentifiers { get; private set; }

        public TemporaryCommandHandler(DiscordBotManager discordBot, CommandHandler commandHandler,
            User user, params string[] identifiers)
        {
            DiscordBot = discordBot;
            ParentCommandHandler = commandHandler;
            User = user;
            BotCommands = new Dictionary<string, BotCommand>();
            CommandIdentifiers = new List<string>(identifiers);
        }

        public TemporaryCommandHandler AddCommand(string name, Func<CommandInfo, Task> command)
        {
            try
            {
                BotCommands.Add(name, new BotCommand(command));
                ParentCommandHandler.Logger.LogInfo($"{name}, {command}, {command?.Method?.ToString()}");
            }
            catch(Exception ex)
            {
                ParentCommandHandler.Logger.LogException(ex);
            }
            return this;
        }

        public virtual bool HandleMessage(FullMessage message)
        {
            if (ParseCommand(message, out var command, out var parameters))
            {
                if (BotCommands.TryGetValue(command, out BotCommand botCommand))
                {
                    CommandInfo commandInfo = new CommandInfo()
                    {
                        Message = message,
                        Command = command,
                        Arguments = parameters,
                        CommandHandler = ParentCommandHandler,
                    };

                    botCommand.Execute(commandInfo, commandInfo.ApiClient);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool ParseCommand(FullMessage message, out string command, out string parameters)
        {
            string[] args = message.Content.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length < 1)
            {
                command = null;
                parameters = null;
                return false;
            }

            command = args[0];
            parameters = args.Length == 2 ? args[1] : null;
            return MessageHasCommand(args[0]);
        }

        private bool MessageHasCommand(string message)
        {
            return CommandIdentifiers.Any(c => message.ToLower().StartsWith(c.ToLower()));
        }
    }
}
