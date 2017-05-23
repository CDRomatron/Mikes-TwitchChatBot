using System;
using System.Collections.Generic;
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
using System.Net.Http;
using System.Threading;

namespace MikesTwitchChatBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IRCBot bot;

        public MainWindow()
        {
            InitializeComponent();
            lblStatusData.Content = "Not connected.";
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            bot = new IRCBot("irc.chat.twitch.tv", 6667, tbxAccount.Text.ToLower(), tbxAccount.Text.ToLower(), pbxOAuth.Password, "#" + tbxChannel.Text.ToLower(), lblStatusData);

            Task.Factory.StartNew(() => bot.Run());
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            bot.sendRaw("Hello, this is CDBot!");
        }

        private void btnGetOAuth_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.twitchapps.com/tmi/");
        }
    }
}
