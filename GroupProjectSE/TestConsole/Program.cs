using GroupProjectSE.FileCloner;
using GroupProjectSE.FileCloning.FileSharing;

namespace TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FileCloner cloner = new();
            FileReceiver fileReceiver = cloner._fileReceiver;
            FileSender fileSender = cloner._fileSender;

            fileReceiver.RequestFiles();

        }
    }
}
