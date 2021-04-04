using DiscordBot.Utility.Web;
using DiscordBot.Utility.Web.cUrl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for WebRequestWindow.xaml
    /// </summary>
    //DELETE:
    public partial class WebRequestWindow : Window
    {
        public ObservableCollection<string> Results { get; set; } = new ObservableCollection<string>();

        public WebRequestWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        private async void UrlRequestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = UrlInput.Text;

                var response = await new CurlCommandHandler(null).GetRequest(url);

                ResponseBox.Text = response.Message;
            }
            catch (Exception ex)
            {
                ResponseBox.Text = "Error: " + ex.Message;
            }
        }
    }
}
