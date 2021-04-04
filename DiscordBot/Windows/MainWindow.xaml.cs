using DiscordBot.Bot;
using DiscordBot.DiscordAPI;
using DiscordBot.DiscordAPI.Structures;
using DiscordBot.Models;
using DiscordBot.Utility;
using DiscordBot.Utility.Web;
using DiscordBot.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiscordBot.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DiscordBotManager discordBot;
        private LoggerWindow loggerWindow;
        private ImageSorterWindow imageSorterWindow;

        public MainWindow(DiscordBotManager discordBot, LoggerWindow loggerWindow, ImageSorterWindow imageSorter)
        {
            InitializeComponent();

            this.loggerWindow = loggerWindow;
            loggerWindow.Closed += LogsWindow_Closed;

            this.imageSorterWindow = imageSorter;
            imageSorterWindow.Closed += ImagesWindow_Closed;

            this.discordBot = discordBot;

            DataContext = discordBot;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            discordBot.GatewayClient.OpenWebSocket();
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            discordBot.GatewayClient.CloseWebSocket();
        }

        private void GuildMenuChannelButton_Click(object sender, RoutedEventArgs e)
        {
            ChannelList.Visibility = Visibility.Visible;
            UserList.Visibility = Visibility.Hidden;
            RoleList.Visibility = Visibility.Hidden;
            EmojiList.Visibility = Visibility.Hidden;
        }

        private void GuildMenuUserButton_Click(object sender, RoutedEventArgs e)
        {
            UserList.Visibility = Visibility.Visible;
            ChannelList.Visibility = Visibility.Hidden;
            RoleList.Visibility = Visibility.Hidden;
            EmojiList.Visibility = Visibility.Hidden;
        }

        private void GuildMenuRoleButton_Click(object sender, RoutedEventArgs e)
        {
            RoleList.Visibility = Visibility.Visible;
            ChannelList.Visibility = Visibility.Hidden;
            UserList.Visibility = Visibility.Hidden;
            EmojiList.Visibility = Visibility.Hidden;
        }

        private void GuildMenuEmojiButton_Click(object sender, RoutedEventArgs e)
        {
            EmojiList.Visibility = Visibility.Visible;
            ChannelList.Visibility = Visibility.Hidden;
            UserList.Visibility = Visibility.Hidden;
            RoleList.Visibility = Visibility.Hidden;
        }

        private void LogsButton_Click(object sender, RoutedEventArgs e)
        {
            loggerWindow.Show();
        }

        private void LogsWindow_Closed(object sender, EventArgs e)
        {
            loggerWindow.Hide();
        }

        private void ImagesButton_Click(object sender, RoutedEventArgs e)
        {
            imageSorterWindow.Show();
        }

        private void ImagesWindow_Closed(object sender, EventArgs e)
        {
            imageSorterWindow.Hide();
        }
    }
}
