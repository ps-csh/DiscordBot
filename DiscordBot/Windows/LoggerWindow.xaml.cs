using DiscordBot.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiscordBot.Windows
{
    /// <summary>
    /// Provides a temporary method to view logs at runtime
    /// </summary>
    public partial class LoggerWindow : Window
    {
        public LoggerWindow(ILogger logger)
        {
            InitializeComponent();

            DataContext = logger;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LogsList.Items.Refresh();
        }
    }
}
