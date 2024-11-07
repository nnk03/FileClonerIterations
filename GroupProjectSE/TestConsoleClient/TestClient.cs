using GroupProjectSE.FileCloner;
using GroupProjectSE.FileCloning.FileSharing;
using Networking;
using Networking.Communication;

namespace TestConsoleClient
{
    public class TestClient
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            foreach (string arg in args)
            {
                Console.WriteLine(arg);
            }

            Thread clientThread = new(() =>
            {
                ClientProgram(args[0], args[1]);
            });
            clientThread.Start();

        }
        public static void ClientProgram(string serverIP, string serverPort)
        {
            CommunicatorClient client =
                (CommunicatorClient)CommunicationFactory.GetCommunicator(isClientSide: true);
            if (client == null)
            {
                Console.WriteLine("NULLCLIENT");
            }

            client.Start(serverIP, serverPort);
            FileSender fileSender = FileCloner.GetFileSender();

            Console.WriteLine($"Client address is {client.GetMyIP()}:{client.GetMyPort()}");

            while (true)
            {
                try
                {
                    string? message = Console.ReadLine();
                    if (message != null)
                    {
                        client.Send(message, "SERVER", null);
                    }
                }
                catch (Exception ex)
                {
                    break;
                }

            }



            client.Stop();
        }
    }
}
