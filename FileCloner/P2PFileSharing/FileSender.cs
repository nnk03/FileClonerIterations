using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Networking;
using Networking.Communication;
using Networking.Serialization;

namespace SoftwareEngineeringGroupProject.FileCloner.P2PFileSharing;

public class FileSender : FileClonerHeaders, INotificationHandler
{
    private object _syncLock = new();
    private Dictionary<string, TcpClient> _clientDictionary;

    private Logger.Logger _logger = new("FileSender");

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

        // subscribe to messages with module name as "FileSender"
        _fileServer.Subscribe("FileSender", this, false);

        // gets the reference of the map
        _clientIdToSocket = _fileServer.GetClientList();
    }

    public void OnDataReceived(string serializedData)
    {
        if (serializedData.StartsWith(FileRequestHeader))
        {
            // after the header contains the serialized data
            string serializedRequest = serializedData.Split(':', 2)[1];
            List<string> fileRequests = _serializer.Deserialize<List<string>>(serializedRequest);
            // fileRequests contains the file requests
            // example ['A.txt', 'B.txt']

            // Now check if the requested files exist and send ACK using thread ??
            // but now we need to send the response to that specific client which gave us
            // the broadcast message

        }
    }

    public void ResponseToFileRequest(string filePath)
    {
        if (File.Exists(filePath))
        {

        }
    }
    public void OnClientJoined(TcpClient socket)
    {
        IPEndPoint? remoteEndPoint = (IPEndPoint)socket.Client.RemoteEndPoint;
        if (remoteEndPoint == null)
        {
            return;
        }
        string ipAddress = remoteEndPoint.Address.ToString();
        string port = remoteEndPoint.Port.ToString();
        // using underscores since apparently fileNames cannot have :
        string address = $"{ipAddress}_{port}";

        _logger.Log($"Client Joined : {address}");
        lock (_syncLock)
        {
            _clientDictionary.Add(address, socket);
        }
    }

    public void OnClientLeft(string clientId)
    {

    }

}
