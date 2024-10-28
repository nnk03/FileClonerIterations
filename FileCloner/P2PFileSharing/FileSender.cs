using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking.Communication;

namespace SoftwareEngineeringGroupProject.FileCloner.P2PFileSharing;

public class FileSender
{
    private CommunicatorServer _fileServer;
    private const int FileServerPort = (int)SenderReceiverConstants.FileReceiverPortNumber;
    public FileSender()
    {
        // create a file server in each device to serve the files
        _fileServer = new CommunicatorServer();
        Thread fileServerThread = new Thread(() => _fileServer.Start());
        fileServerThread.Start();
    }

}
