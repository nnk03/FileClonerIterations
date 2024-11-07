using GroupProjectSE.FileCloner;
using GroupProjectSE.FileCloning.FileSharing;
using Networking;
using Networking.Communication;
using System.Diagnostics;

namespace TestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            Thread serverThread = new Thread(ServerProgram);
            serverThread.Start();


        }

        public static void ServerProgram()
        {
            CommunicatorServer server =
                (CommunicatorServer)CommunicationFactory.GetCommunicator(isClientSide: false);
            FileReceiver fileReceiver = FileCloner.GetFileReceiver();
            server.Subscribe("SERVER", fileReceiver, false);

            string serverAddress = server.Start();
            string serverIP = serverAddress.Split(':')[0];
            string serverPort = serverAddress.Split(":")[1];
            Console.WriteLine($"Server Address is {serverAddress}");

            StartWorkerAppInSeparateConsole([serverIP, serverPort]);


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
        public static void StartWorkerAppInSeparateConsole(string[] argsToPass)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "TestConsoleClient.exe", // Path to your console app executable
                //Arguments = string.Join(" ", argsToPass),
                Arguments = $" {argsToPass[0]} {argsToPass[1]}",
                UseShellExecute = true,     // Ensures it opens in a separate terminal
                CreateNoWindow = false      // Set to false to ensure the window is visible
            };

            Process process = Process.Start(startInfo);
            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) =>
            {
                Console.WriteLine("WorkerApp has exited.");
            };
        }
    }
}
