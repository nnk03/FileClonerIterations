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

            Thread serverThread = new Thread(ServerProgram);
            Thread clientThread = new Thread(ClientProgram);

            serverThread.Start();
            //clientThread.Start();


        }

        public static void ClientProgram()
        {

            CommunicatorClient client =
                (CommunicatorClient)CommunicationFactory.GetCommunicator(isClientSide: true);
            FileSender fileSender = FileCloner.GetFileSender();

            string serverIP = "10.128.9.66";
            string serverPort = "60469";
            client.Start(serverIP, serverPort);
            Console.WriteLine($"Client address is {client.GetMyIP()}:{client.GetMyPort()}");



            while (true)
            {
                try
                {
                    string? message = Console.ReadLine();
                    if (message != null)
                    {
                        client.Send(message, "FileReceiver", null);
                    }
                }
                catch (Exception ex)
                {
                    break;
                }

            }



            client.Stop();
        }
        public static void ServerProgram()
        {
            CommunicatorServer server =
                (CommunicatorServer)CommunicationFactory.GetCommunicator(isClientSide: false);
            FileReceiver fileReceiver = FileCloner.GetFileReceiver();

            string serverAddress = server.Start();
            string serverIP = serverAddress.Split(':')[0];
            string serverPort = serverAddress.Split(":")[1];
            Console.WriteLine($"Server Address is {serverAddress}");

            while (true)
            {
                try
                {
                    string? c = Console.ReadLine();
                    if (c == null)
                    {
                        continue;
                    }
                    if (c == "request")
                    {
                        fileReceiver.RequestFiles();
                    }
                    else if (c == "diffGen")
                    {
                        fileReceiver.GenerateDiff();
                    }
                    else if (c == "requestClone")
                    {
                        fileReceiver.RequestToCloneFiles();
                    }
                    else
                    {
                        server.Send(c, "FileSender", null);
                    }



                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exiting");
                    break;
                }
            }

            server.Stop();

        }
    }
}
