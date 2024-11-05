using GroupProjectSE.FileCloner;
using GroupProjectSE.FileCloning.FileSharing;

namespace TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //FileCloner cloner = new();
            //FileReceiver fileReceiver = cloner._fileReceiver;
            //FileSender fileSender = cloner._fileSender;

            //fileReceiver.RequestFiles();

            //File.Create("C:\\dev2\\dummy.txt").Close();

            //Directory.CreateDirectory("C:\\dev2\\dummy.txt");
            string filePath = "C:\\dev2\\dummy.txt";
            Console.WriteLine(Path.GetDirectoryName(filePath));

        }
    }
}
