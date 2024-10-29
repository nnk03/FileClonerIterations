using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Networking;
using Networking.Communication;
using Networking.Serialization;

namespace SoftwareEngineeringGroupProject.FileCloner.P2PFileSharing;

public class FileSender : INotificationHandler
{
    private CommunicatorServer _fileServer;
    // private const int FileServerPort = (int)SenderReceiverConstants.FileReceiverPortNumber;
    private Dictionary<string, TcpClient> _clientIdToSocket;

    // should we use serializer??
    private Serializer _serializer = new Serializer();

    private string _myAddress;

    // get clientIdToSocket Dictionary
    public FileSender()
    {
        // create a file server in each device to serve the files
        _fileServer = new CommunicatorServer();
        _myAddress = _fileServer.Start();

        // gets the reference of the map
        _clientIdToSocket = _fileServer.GetClientList();

    }



    //public string SendFile(string filePath)
    //{
    //    if (!File.Exists(filePath))
    //    {
    //        return "FILE_NOT_FOUND";
    //    }

    //}


    public void ResponseToFileRequest(string filePath)
    {
        if (File.Exists(filePath))
        {

        }
    }

    //public string OnDataReceived(string data)
    //{
    //    // string deserializedData = _serializer.Deserialize(serializedData);
    //    if (data.StartsWith("<FILE_REQUEST>:"))
    //    {
    //        // second element will be the filePath
    //        string[] headerAndFilePath = data.Split(':', 2);
    //        string filePath = headerAndFilePath[1];

    //        if (File.Exists(filePath))
    //        {
    //            // if file exists, then return filePath and timestamp
    //            string returnMessage = "true";
    //            returnMessage += $":{filePath}:{File.GetLastWriteTime(filePath).ToString()}";
    //            return returnMessage;

    //        }
    //        else
    //        {
    //            return "false";
    //        }
    //    }
    //    else if (data.StartsWith("<CLONE_FILE>:"))
    //    {

    //    }
    //else if (data.startswith("<GET_ALL_IP>:")){
    //    return myIP;
    //    }

    //}

    //private void HandleClient(TcpClient socket)
    //{
    //    try
    //    {
    //        NetworkStream stream = socket.GetStream();
    //        byte[] buffer = new byte[(int)SenderReceiverConstants.PacketSize];

    //        int bytesRead;

    //        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
    //        {
    //            // Convert received bytes to string
    //            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
    //            Trace.WriteLine("Received: " + receivedData);

    //            // Process the data
    //            string response = OnDataReceived(receivedData);

    //            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
    //            stream.Write(responseBytes, 0, responseBytes.Length);
    //            Trace.WriteLine("ACK sent: " + response);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Trace.WriteLine($"[FileCloner] : Error Handling Client {ex.Message}");
    //    }


    //}

    //public void OnClientJoined(TcpClient socket)
    //{
    //    if (socket == null)
    //    {
    //        return;
    //    }
    //    Thread handleClientThread = new Thread(() => {
    //        HandleClient(socket);
    //    });
    //    handleClientThread.Start();

    //}

}
