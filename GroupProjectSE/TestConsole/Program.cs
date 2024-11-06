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
            FileReceiver fileReceiver = FileCloner.GetFileReceiver();

            string serverAddress = server.Start();
            string serverIP = serverAddress.Split(':')[0];
            string serverPort = serverAddress.Split(":")[1];
            Console.WriteLine($"Server Address is {serverAddress}");

            //CommunicatorClient client =
            //    (CommunicatorClient)CommunicationFactory.GetCommunicator(isClientSide: true);
            //client.Start(serverIP, serverPort);
            //Console.WriteLine($"Client address is {client.GetMyIP()}:{client.GetMyPort()}");


            //FileSender fileSender = FileCloner.GetFileSender();

            //fileReceiver.RequestFiles();


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


                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exiting");
                    break;
                }
            }


            //client.Stop();
            server.Stop();

        }
    }
}
