using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DiscordBot.Bot;
using DiscordBot.Models;
using DiscordBot.Startup;
using Microsoft.EntityFrameworkCore;
using DiscordBot.Configuration;
using DiscordBot.Utility;
using DiscordBot.DiscordAPI;
using DiscordBot.Modules;
using DiscordBot.Utility.Web.cUrl;
using DiscordBot.Bot.Commands;
using System.Runtime.Loader;
using DiscordBot.Utility.Web;
using DiscordBot.Windows;
using DiscordBot.ViewModels;

namespace DiscordBot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        DiscordBotManager discordBot { get; set; }
        DatabaseContext databaseContext { get; set; }

        ServiceProvider serviceProvider;

        ILogger logger;

        public void ApplicationStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            IConfiguration configuration = CreateConfiguration();

            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services, configuration);
            serviceProvider = services.BuildServiceProvider();

            logger = serviceProvider.GetRequiredService<ILogger>();

            serviceProvider.GetRequiredService<CommandService>();

            discordBot = serviceProvider.GetRequiredService<DiscordBotManager>();

            MainWindow = serviceProvider.GetService<MainWindow>();
            MainWindow.Show();

            //TODO: Separate MainWindow so bot can run without it (aka no GUI mode)
            // Shutdown the application when main window closes
            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        //private IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureServices((_, services) =>
        //            services.AddTransient<ITransientOperation, DefaultOperation>()
        //                    .AddScoped<IScopedOperation, DefaultOperation>()
        //                    .AddSingleton<ISingletonOperation, DefaultOperation>()
        //                    .AddTransient<OperationLogger>());

        private IConfiguration CreateConfiguration()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddUserSecrets<App>();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("userinfo.json"); //TODO: Merge these files
            builder.AddJsonFile("appsettings.json");

            return builder.Build();
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // The extension of Configure<T>() in Microsoft.Extensions.Options.ConfigurationExtensions package
            // provides a shorthand way to Bind the configuration section to the templated class 'T'
            services.Configure<BotSecretsConfiguration>(configuration.GetSection("Authentication"))
                .Configure<BotSettingsConfiguration>(configuration.GetSection("Bot"))
                .Configure<LoggerConfiguration>(configuration.GetSection("Logger"))
                .Configure<AssemblyConfiguration>(configuration.GetSection("Assembly"))
                .Configure<DirectoriesConfiguration>(configuration.GetSection("Directories"));
            
            services.AddDbContext<DatabaseContext>(options => 
                options.UseSqlite(configuration.GetConnectionString("default")));

            //TODO: Move addition of concrete classes to extensions methods
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<DiscordApiClient>();
            services.AddSingleton<DiscordGatewayClient>();
            services.AddScoped<CurlCommandHandler>();
            services.AddScoped<HttpRequestHandler>();
            services.AddSingleton<DiscordApiRequestHandler>(); //Using singletons to limit calls to these APIs across the program
            services.AddSingleton<YandereAPIClient>();
            services.AddSingleton<CommandService>();
            services.AddSingleton<CommandHandler>();
            services.AddSingleton<DiscordBotManager>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<LoggerWindow>();
            services.AddSingleton<ImageSorterViewModel>();
            services.AddSingleton<ImageSorterWindow>();
        }

        private void OnApplicationExit(object sender, ExitEventArgs args)
        {
            logger?.LogInfo($"Application exiting with code: {args.ApplicationExitCode}");
            logger?.WriteLogsToFile();
        }


        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger?.LogException(e.ExceptionObject as Exception);
            logger?.WriteLogsToFile();
        }
    }
}
