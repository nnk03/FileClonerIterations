using Networking;
using Networking.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleClient
{
    public class SampleNotification : INotificationHandler
    {
        private int clientId = 1;
        public void OnDataReceived(string message)
        {
            Console.WriteLine(message);
        }

        public void OnClientJoined(TcpClient socket)
        {
            CommunicatorServer server =
                (CommunicatorServer)CommunicationFactory.GetCommunicator(isClientSide: false);
            server.AddClient(clientId.ToString(), socket);
            Console.WriteLine($"CLIENT JOINED {clientId}");
            ++clientId;
        }

    }
}
