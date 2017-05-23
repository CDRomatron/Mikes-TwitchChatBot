using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MikesTwitchChatBot
{
    class PingSender
    {
        static string PING = "PING :";
        private Thread pingSender;
        // Empty constructor makes instance of Thread
        public PingSender()
        {
            pingSender = new Thread(new ThreadStart(this.Run));
        }
        // Starts the thread
        public void Start()
        {
            pingSender.Start();
        }
        // Send PING to irc server every 15 seconds
        public void Run()
        {
            while (true)
            {
                IRCBot.writer.WriteLine(PING + IRCBot.SERVER);
                IRCBot.writer.Flush();
                Thread.Sleep(15000);
            }
        }
    }
}
