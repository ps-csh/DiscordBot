using DiscordBot.DiscordAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiscordBot.ViewTemplates
{
    /// <summary>
    /// Interaction logic for GuildTemplate.xaml
    /// </summary>
    public partial class GuildTemplate : UserControl
    {
        private readonly Guild guildContext;

        public GuildTemplate(Guild guild)
        {
            guildContext = guild;

            DataContext = guild;

            InitializeComponent();
        }
    }
}
