using GroupProjectSE.FileCloner;
using GroupProjectSE.FileCloning.FileSharing;
using Networking;
using Networking.Communication;

namespace TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CommunicatorServer server =
                (CommunicatorServer)CommunicationFactory.GetCommunicator(isClientSide: false);
            //CommunicatorClient client =
            //    (CommunicatorClient)CommunicationFactory.GetCommunicator(isClientSide: true);

            string serverAddress = server.Start();
            string serverIP = serverAddress.Split(':')[0];
            string serverPort = serverAddress.Split(":")[1];
            Console.WriteLine($"Server Address is {serverAddress}");

            //client.Start(serverIP, serverPort);
            //Console.WriteLine($"Client address is {client.GetMyIP()}:{client.GetMyPort()}");


            FileReceiver fileReceiver = FileCloner.GetFileReceiver();
            //FileSender fileSender = FileCloner.GetFileSender();

            //fileReceiver.RequestFiles();


            while (true)
            {
            }


            //client.Stop();
            server.Stop();

        }
    }
}
