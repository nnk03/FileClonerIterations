using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public class SampleNotification : INotificationHandler
    {
        public void OnDataReceived(string message)
        {
            Console.WriteLine(message);
        }

    }
}
