using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Networking.Communication;

namespace SoftwareEngineeringGroupProject.FileCloner.P2PFileSharing;

public class FileSender
{
    private CommunicatorServer _communicatorServer;
    //private const int FileServerPort = (int)SenderReceiverConstants.FileReceiverPortNumber;
    public FileSender()
    {
        // create a file server in each device to serve the files
        _communicatorServer = new CommunicatorServer();
        //Thread fileServerThread = new Thread(() => _fileServer.Start());
        //fileServerThread.Start();
    }
    public void SendFile(string filePath)
    {
        string serverAddress = _communicatorServer.Start(null, null);
        if (serverAddress == "failure")
        {
            Debug.WriteLine("You are a failure!!");
            return;
        }
        string serverIP = serverAddress.Split(':')[0];
        string serverPort = serverAddress.Split(':')[1];
        TcpListener listener = new TcpListener(serverIP, (int)serverPort);
        listener.Start();
        TcpClient client = listener.AcceptTcpClient;
        using NetworkStream networkStream = client.GetStream();
        byte[] buffer = new byte[1024];
        using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        int bytesRead = 0;
        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length) > 0)
        {
            networkStream.Write(buffer, 0, bytesRead);
            Debug.WriteLine($"{bytesRead} bytes of data sent");
        }
    }

}
