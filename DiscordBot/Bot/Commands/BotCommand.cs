using DiscordBot.Bot.Commands;
using DiscordBot.DiscordAPI;
using DiscordBot.DiscordAPI.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot
{
    public class BotCommand
    {
        public delegate void CommandHandler(CommandInfo commandInfo);

        public Func<CommandInfo, Task> Command { get; set; }

        public Permissions Permissions { get; set; }

        public Status Status { get; protected set; }

        public BotCommand(Func<CommandInfo, Task> command, Permissions permissions = null) 
        {
            Command = command;
            Permissions = permissions;
        }

        public virtual async void Execute(CommandInfo commandInfo, DiscordApiClient apiClient)
        {
            Command?.Invoke(commandInfo);
        }

        public bool HasPermissions(CommandInfo commandInfo)
        {
            if (Permissions?.Users.Any() == true || Permissions?.Roles.Any() == true || Permissions?.Channels.Any() == true)
            {
                return Permissions.Users.Any(u => u == commandInfo.Sender.ID) || 
                    Permissions.Roles.Any(r => commandInfo.Sender.Roles.Any(sr => sr.ID == r)) ||
                    Permissions.Channels.Any(c => c == commandInfo.Channel.ID);
            }

            return true;
        }

        public virtual void Cancel()
        {
            Status = Status.Cancelled;
        }
    }

    public enum Status
    {
        /// <summary>
        /// Command is waiting for input.
        /// Can be interrupted.
        /// </summary>
        Waiting,

        /// <summary>
        /// Command is busy, cannot be interrupted.
        /// </summary>
        Busy,

        /// <summary>
        /// Command was cancelled
        /// </summary>
        Cancelled,

        /// <summary>
        /// Command completed without issue
        /// </summary>
        Completed
    }

    public class Permissions
    {
        public List<string> Roles = new List<string>();
        public List<string> Users = new List<string>();
        public List<string> Channels = new List<string>();

        public Permissions SetRoles(params string[] roles)
        {
            Roles = new List<string>(roles);
            return this;
        }

        public Permissions SetUsers(params string[] users)
        {
            Users = new List<string>(users);
            return this;
        }

        public Permissions SetChannels(params string[] channels)
        {
            Channels = new List<string>(channels);
            return this;
        }
    }
}
