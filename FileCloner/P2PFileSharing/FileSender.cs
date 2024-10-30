using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Networking;
using Networking.Communication;
using Networking.Serialization;

namespace SoftwareEngineeringGroupProject.FileCloner.P2PFileSharing;

public class FileSender : FileClonerHeaders, INotificationHandler
{
    private object _syncLock = new();
    private Dictionary<string, TcpClient> _clientDictionary = new();

    private const string CurrentModule = "FileSender";
    private Logger.Logger _logger = new(CurrentModule);

    private CommunicatorServer _fileServer;
    // private const int FileServerPort = (int)SenderReceiverConstants.FileReceiverPortNumber;
    private Dictionary<string, TcpClient> _clientIdToSocket;


    private const string SenderConfigFilePathKey = "filePath";
    private const string SenderConfigSavePathKey = "savePath";
    private const string SenderConfigTimeStampKey = "timeStamp";

    // should we use serializer??
    private Serializer _serializer = new Serializer();

    private string _myServerAddress;

    // get clientIdToSocket Dictionary
    public FileSender()
    {
        // create a file server in each device to serve the files
        _fileServer = new CommunicatorServer();
        _myServerAddress = _fileServer.Start();

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
            string[] serializedDataList = serializedData.Split(':', 3);

            string serializedRequest = serializedDataList[MessageIndex];
            List<string> fileRequests = _serializer.Deserialize<List<string>>(serializedRequest);
            // fileRequests contains the file requests
            // example ['A.txt', 'B.txt']

            var fileDataList = new List<Dictionary<string, object>>();

            // Now check if the requested files exist and send ACK using thread ??
            // but now we need to send the response to that specific client which gave us
            // the broadcast message

            // response would be a list which contains serialized json
            // json 
            foreach (string file in fileRequests)
            {
                // Check if the file exists
                if (File.Exists(file))
                {
                    // Get the last write time of the file
                    DateTime lastWriteTime = File.GetLastWriteTime(file);

                    // Add file data to the list
                    fileDataList.Add(new Dictionary<string, object>
                    {
                        { SenderConfigFilePathKey, file },
                        { SenderConfigTimeStampKey, lastWriteTime }
                    });
                }
            }
            string jsonResponse = JsonSerializer.Serialize(fileDataList);
            //string jsonResponse = JsonSerializer.Serialize(
            //    fileDataList, new JsonSerializerOptions { WriteIndented = true }
            //);

            // currently assuming its localhost_8888

            string sendToAddress = serializedDataList[AddressIndex];
            string clientId = GetClientId(sendToAddress);

            _fileServer.Send(
                GetMessage(AckFileRequestHeader, jsonResponse),
                CurrentModule, clientId);
        }


    }

    /// <summary>
    /// Adds the key, value pair (address, socket) to the 
    /// `clientDictionary`
    /// </summary>
    /// <param name="socket"></param>
    public void OnClientJoined(TcpClient socket)
    {
        string address = GetAddressFromSocket(socket);
        _logger.Log($"Client Joined : {address}");
        lock (_syncLock)
        {
            _clientDictionary.Add(address, socket);
        }
    }

    /// <summary>
    /// if clientId key is present in the dictionary, remove it
    /// </summary>
    /// <param name="clientId"></param>
    public void OnClientLeft(string clientId)
    {
        lock (_syncLock)
        {
            if (_clientDictionary.ContainsKey(clientId))
            {
                _clientDictionary.Remove(clientId);
            }
        }
    }

    /// <summary>
    /// Gets the client ID from the dictionary _clientIdToSocket
    /// given the TcpClient `socket`
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    private string GetClientId(TcpClient socket)
    {
        foreach (KeyValuePair<string, TcpClient> kvPair in _clientIdToSocket)
        {
            if (kvPair.Value == socket)
            {
                return kvPair.Key;
            }
        }
        return "";
    }

    /// <summary>
    /// Gets the client ID from the dictionary _clientIdToSocket
    /// given the string `address`
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    private string GetClientId(string address)
    {
        foreach (KeyValuePair<string, TcpClient> kvPair in _clientIdToSocket)
        {
            TcpClient socket = kvPair.Value;
            if (GetAddressFromSocket(socket) == address)
            {
                return kvPair.Key;
            }
        }
        return "";
    }

    /// <summary>
    /// Gets the Concatenated address from the socket
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    private string GetAddressFromSocket(TcpClient socket)
    {
        IPEndPoint? remoteEndPoint = (IPEndPoint?)socket.Client.RemoteEndPoint;
        if (remoteEndPoint == null)
        {
            return "";
        }
        string ipAddress = remoteEndPoint.Address.ToString();
        string port = remoteEndPoint.Port.ToString();

        // using underscores since apparently fileNames cannot have :
        string address = GetConcatenatedAddress(ipAddress, port);
        return address;
    }

    /// <summary>
    /// overloads the base functionality since myAddress is known, and thus we don't have to give it every time
    /// when sending a message
    /// </summary>
    /// <param name="header"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private string GetMessage(string header, string message)
    {
        return GetMessage(_myServerAddress, header, message);
    }

}
