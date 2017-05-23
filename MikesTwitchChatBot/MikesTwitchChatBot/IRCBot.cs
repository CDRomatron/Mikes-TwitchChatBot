using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;

namespace MikesTwitchChatBot
{
    public class IRCBot
    {
        // Irc server to connect 
        public static string SERVER;
        // Irc server's port (6667 is default port)
        private static int PORT;
        // User information defined in RFC 2812 (Internet Relay Chat: Client Protocol) is sent to irc server 
        private static string USER;
        // Bot's nickname
        private static string NICK;
        // Channel to join
        private static string CHANNEL;

        private static string PASS;
        // StreamWriter is declared here so that PingSender can access it
        public static StreamWriter writer;

        private Label status;

        public IRCBot(string server, int port, string user, string nick, string pass, string channel, Label status)
        {
            SERVER = server;
            PORT = port;
            USER = user;
            NICK = nick;
            PASS = pass;
            CHANNEL = channel;
            this.status = status;
        }

        public void Run()
        {
            NetworkStream stream;
            TcpClient irc;
            string inputLine;
            StreamReader reader;
            string nickname;
            try
            {
                irc = new TcpClient(SERVER, PORT);
                stream = irc.GetStream();
                using (reader = new StreamReader(stream))
                using (writer = new StreamWriter(stream))
                {
                    // Start PingSender thread
                    updateStatus("Connecting...");
                    PingSender ping = new PingSender();
                    ping.Start();
                    int counter = 0;
                    bool flag = false;
                    while (true)
                    {
                        while ((inputLine = reader.ReadLine()) != null)
                        {
                            if(counter == 1)
                            {
                                writer.WriteLine("PASS " + PASS);
                                writer.Flush();
                                writer.WriteLine("NICK " + NICK);
                                writer.Flush();
                                writer.WriteLine("USER " + USER + " 0 * :" + USER);
                                writer.Flush();
                            }
                            counter++;
                            if (inputLine.EndsWith("JOIN :" + CHANNEL))
                            {
                                // Parse nickname of person who joined the channel
                                nickname = inputLine.Substring(1, inputLine.IndexOf("!") - 1);
                                // Welcome the nickname to channel by sending a notice
                                writer.WriteLine("NOTICE " + nickname + " :Hi " + nickname +
                                " and welcome to " + CHANNEL + " channel!");
                                writer.Flush();
                                // Sleep to prevent excess flood
                                Thread.Sleep(2000);
                            }
                            else if(inputLine.Contains("PING :") && !inputLine.Contains("You"))
                            {
                                string pong = inputLine.Remove(0, 6);
                                writer.WriteLine("PONG :" + pong);
                                writer.Flush();
                                Console.WriteLine("Me: PONG :" + pong);
                            }
                            else if(inputLine.Contains("You are in a maze of twisty passages"))
                            {
                                writer.WriteLine("JOIN " + CHANNEL);
                                writer.Flush();
                                flag = true;
                                updateStatus("Connected!");
                                sendRaw("Welcome to Mike's Twitch Chat Bot!");
                            }

                            if(flag)
                            {
                                try
                                {
                                    string incomingMessage = "";

                                    for (int i = 0; i < inputLine.Count(); i++)
                                    {
                                        if (inputLine[inputLine.Count() - i - 1] != ':')
                                        {
                                            incomingMessage = inputLine[inputLine.Count() - i - 1] + incomingMessage;
                                        }
                                        else
                                        {
                                            Console.WriteLine("incoming message:" + incomingMessage);
                                            if (incomingMessage.StartsWith("!"))
                                            {
                                                writer.WriteLine("PRIVMSG "+ CHANNEL + " :recieved command " + incomingMessage);
                                                writer.Flush();
                                            }
                                            break;
                                        }
                                    }
                                }
                                catch(Exception e)
                                {
                                    Console.WriteLine("I messed up: " + e.Message);
                                }
                                
                            }
                            Console.WriteLine(inputLine);
                        }
                        // Close all streams
                        writer.Close();
                        reader.Close();
                        irc.Close();
                    }
                }
            }
            catch (Exception e)
            {
                // Show the exception, sleep for a while and try to establish a new connection to irc server
                Console.WriteLine(e.ToString());
                Thread.Sleep(5000);
                Run();
            }
        }

        public void sendRaw(string data)
        {
            writer.Flush();
            writer.WriteLine("PRIVMSG " + CHANNEL + " :" + data);
            writer.Flush();
        }

        private void updateStatus(string status)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    this.status.Content = status;
                });
        }
    }
}
