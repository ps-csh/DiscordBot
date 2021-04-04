using DiscordBot.Bot;
using DiscordBot.Bot.Commands;
using DiscordBot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    /// <summary>
    /// Loads Commands from an assembly
    /// </summary>
    public class CommandService
    {
        private readonly List<Assembly> Assemblies;
        private List<CommandModule> commandModules;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public CommandService(IServiceProvider provider, ILogger logger)
        {
            //TODO: Get Assemblies from configuration
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new System.Reflection.AssemblyName("DiscordBot"));
            Assemblies = new List<Assembly>() { assembly };
            commandModules = new List<CommandModule>();
            
            serviceProvider = provider;
            this.logger = logger;

            LoadClasses();
            
        }

        public void LoadClasses()
        {
            var moduleType = typeof(CommandModule);
            // Find all classes that inherit from CommandModule
            var types = Assemblies.SelectMany(s => s.GetTypes())
                .Where(s => moduleType.IsAssignableFrom(s));
            foreach (var type in types)
            {
                try
                {
                    if (!type.IsAbstract)
                    {
                        var module = (CommandModule)Activator.CreateInstance(type, serviceProvider);
                        commandModules.Add(module);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogException(ex);
                }               
            }
        }

        public IEnumerable<KeyValuePair<string, Func<CommandInfo, Task>>> GetCommands()
        {
            List<KeyValuePair<string, Func<CommandInfo, Task>>> commands = new List<KeyValuePair<string, Func<CommandInfo, Task>>>();
            foreach (var module in commandModules)
            {
                try
                {
                    var methods = module.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                        .Select(m => new KeyValuePair<MethodInfo, CommandAttribute>(m, m.GetCustomAttribute<CommandAttribute>()))
                        .Where(m => m.Value != null)
                        .ToList();            

                    foreach (var method in methods)
                    {
                        //Create delegate will fail to bind instance methods without a target
                        //Null target will work as long as the method doesn't try to access any fields in the class
                        try
                        {
                            var commandDelegate = (Func<CommandInfo, Task>)method.Key.CreateDelegate(typeof(Func<CommandInfo, Task>), module);
                            new BotCommand(commandDelegate);
                            commands.Add(new KeyValuePair<string, Func<CommandInfo, Task>>(method.Value.Name, commandDelegate));
                        }
                        catch (Exception ex)
                        {
                            logger.LogException(ex, "Failed to bind delegate");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogException(ex);
                }
            }

            logger.LogInfo($"Commands: {commands.Count}");

            return commands;

            /*
             * try
            {
                var moduleType = typeof(CommandModule);
                // Find all classes that inherit from CommandModule
                var types = commandModules.Select(s => s.GetType());
                //.Where((typeof(CommandModule).IsAssignableFrom));

                var methods = types.SelectMany(m => m.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
                    .Select(m => new KeyValuePair<MethodInfo, CommandAttribute>(m, m.GetCustomAttribute<CommandAttribute>()))
                    .Where(m => m.Value != null)
                    .ToList();

                List<KeyValuePair<string, Func<CommandInfo, Task>>> commands2 = new List<KeyValuePair<string, Func<CommandInfo, Task>>>();

                foreach (var method in methods)
                {
                    //Create delegate will fail to bind instance methods without a target
                    //Null target will work as long as the method doesn't try to access any fields in the class
                    var commandDelegate = (Func<CommandInfo, Task>)method.Key.CreateDelegate(typeof(Func<CommandInfo, Task>), null);
                    new BotCommand(commandDelegate);
                    commands2.Add(new KeyValuePair<string, Func<CommandInfo, Task>>(method.Value.Name, commandDelegate));
                    //method.Key.GetParameters()[0]
                }

                logger.LogInfo($"Commands: {commands2.Count}");

                return commands2;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
            */

            return null;
        }
    }
}
