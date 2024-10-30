// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
public class FileReceiver : FileClonerHeaders, INotificationHandler
{
    // this must establish a connection with every file server
    // get serverAddress i.e IP : Port of all file Servers
    // save it somewhere
    // and then request through each socket about the availability of files
    private const string CurrentModuleName = "FileReceiver";
    private Logger.Logger _logger = new(CurrentModuleName);
    private Dictionary<string, TcpClient> _clientDictionary = new();
    private Dictionary<string, TcpClient> _clientIdToSocket;
    private object _syncLock = new();

    // private CommunicatorClient _fileReceiver;
    // CommunicatorServer is much more useful than Communicator Client??
    // for broadcasting messages
    private CommunicatorServer _fileReceiverServer;
    private string _myServerAddress;
    private Serializer _serializer = new Serializer();
    private Dictionary<string, TcpClient> _receiverToSenderMap = new();
    private List<string> _fileServerAddresses = new();

    private const string ReceiverConfigFilePath = ".\\requestConfig.json";
    private const string ResponseOfRequestConfigFilePath = ".\\responseOfRequestConfig.json";
    private object _syncLockForSavingResponse = new();

    private List<string> _requestFilesPath = new();

    // the file to be cloned is saved in this field of the JSON object in the config.json
    private const string ReceiverConfigFilePathKey = "filePath";
    private const string ReceiverConfigSavePathKey = "savePath";
    private const string ReceiverConfigTimeStampKey = "timeStamp";
    public FileReceiver()
    {
        _syncLock = new();
        _clientDictionary = new();
        _clientIdToSocket = new();
        _logger = new(CurrentModuleName);

        // for each file to be received from a particular device D
        // creates a new FileReceiver which handles the receiving and saving of the particular file
        // _fileReceiver = new CommunicatorClient();
        _fileReceiverServer = new CommunicatorServer();
        _myServerAddress = _fileReceiverServer.Start();
        _myServerAddress = _myServerAddress.Replace(':', '_');
        _clientIdToSocket = _fileReceiverServer.GetClientList();

        // Subscribe for messages with module name as "FileReceiver"
        // _fileReceiver.Subscribe(CurrentModuleName, this, false);
        _fileReceiverServer.Subscribe(CurrentModuleName, this, false);

        // no need of broadcast the message of getting all IP
        //_fileReceiverServer.Send(
        //    GetMessage(GetAllIPPortHeader, ""),
        //    CurrentModuleName, null);

        CreateAndCloseFile(ReceiverConfigFilePath);


        // when starting, read the config file
        SaveFileRequests();

    }

    /// <summary>
    /// Mentions what to do when data is received
    /// </summary>
    /// <param name="serializedData"></param>
    public void OnDataReceived(string serializedData)
    {
        // after the header contains the serialized data
        string[] serializedDataList = serializedData.Split(':', MessageSplitLength);
        string header = serializedDataList[HeaderIndex];
        string sendToAddress = serializedDataList[AddressIndex];
        string clientId = GetClientId(sendToAddress);

        if (serializedData.StartsWith(AckFileRequestHeader))
        {
            string serializedJsonData = serializedDataList[MessageIndex];

            Thread saveResponseThread = new Thread(() => {
                SaveResponse(serializedJsonData, sendToAddress);
            });
            saveResponseThread.Start();
        }
        else if (serializedData.StartsWith(AckCloneFilesHeader))
        {
            // format is AckCloneFilesHeader:filePath:count:chunk
        }
        else if (serializedData.StartsWith(RequestOneFileHeader))
        {
            Thread requestFileThread = new Thread(() => {
                RequestFileFromDevices(serializedDataList[MessageIndex]);
            });
            requestFileThread.Start();
        }
        else if (serializedData.StartsWith(AckCloneFilesHeader))
        {
            Thread receiveFilesThroughNetwork = new Thread(() => {
                ReceiveFileOverNetwork(serializedData[MessageIndex]); // put the corresponding save file path over here
            });
            receiveFilesThroughNetwork.Start();
        }

    }



    /// <summary>
    /// gets the list of files to be cloned and broadcasts the request to all file servers
    /// </summary>
    public void RequestFiles()
    {
        SaveFileRequests();
        // broadcast the request to all file servers
        string sendFileRequests = _serializer.Serialize(_requestFilesPath);
        // client can't really send broadcast, hence using the server
        _fileReceiverServer.Send(
            GetMessage(FileRequestHeader, sendFileRequests),
            CurrentModuleName, null);
    }

    public void RequestFileFromDevices(string jsonRequest)
    {
        List<string> serializedDataRequest = _serializer.Deserialize<List<string>>(jsonRequest);
        //Assuming that the json file consists of data which is of the form 
        //fileName:address
        for(string s in serializedDataRequest)
        {

            _fileReceiverServer.Send(
                GetMessage(CloneFilesHeader, s.Split(':')[0]),
                CurrentModuleName, GetClientId(s.Split(':')[1])
                );
        }
    }

 

    private void ReceiveFileOverNetwork(string data)
    {
        try
        {
            string filePath = data.Split(':')[0];
            string count = data.Split(':')[1];
            string dataChunk = data.Split(':')[2];

            if ((int)count == 0) 
            {
                using (StreamWriter writer = new StreamWriter(filePath, false)) //false means that it will overwrite onto the file
                {
                    writer.Write(data);
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(filePath, true)) // true implies that it will get appended
                {
                    writer.Write(data);
                }
            }
            Trace.WriteLine($"Chunk number {count} written succesfully onto the file.");
        }
        catch(Exception e) 
        {
            Trace.WriteLine(e);
        }
    }



    /// <summary>
    /// Saves the response from the file Server `fromWhichServer` and saves it
    /// in the file "fromWhichServer".json
    /// the server will be returning a json file, containing the available files and its
    /// timestamp
    /// </summary>
    /// <param name="data"></param>
    /// <param name="fromWhichServer"></param>
    private void SaveResponse(string data, string fromWhichServer)
    {
        string saveFileName = $"{fromWhichServer}.json";
        if (!CreateAndCloseFile(saveFileName))
        {
            _logger.Log($"Not able to create file {saveFileName}");
            return;
        }
        // data is serialized json
        // saving it in the fileName saveFileName

        File.WriteAllText(saveFileName, data);
    }

    //public void CloneFile(string filePath, string savePath, string fileServerIP, string fileServerPort)
    //{
    //    // get the file from the fileServer and save it in savePath
    //}

    /// <summary>
    /// Helper function to create and close the file
    /// taking care of error handling
    /// creates a new file only if the given file path does not exist
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>
    /// a boolean indicating if Creation of file was successful
    /// </returns>
    private bool CreateAndCloseFile(string filePath)
    {
        // returns if success or failure
        try
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            return true;
        }
        catch (Exception ex)
        {
            Trace.Write(ex);
        }

        return false;
    }

    /// <summary>
    /// extracts ip address and port from the socket
    /// </summary>
    /// <param name="socket"></param>
    /// <returns>
    /// a string in the format IPAddress_Port
    /// </returns>
    private string GetMyAddress(TcpClient socket)
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
    ///  reads the config file which contains the list of files to be cloned and 
    ///  saves it in a list
    /// </summary>
    private void SaveFileRequests()
    {
        // check the FILE_REQUEST.json file which contains list of files to be cloned
        // every item in the list is a JSON object with keys being fileName and values being filePath

        _requestFilesPath = new();
        try
        {
            string jsonContent = File.ReadAllText(ReceiverConfigFilePath);
            using JsonDocument doc = JsonDocument.Parse(jsonContent);

            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                string? filePath = element.GetProperty(ReceiverConfigFilePathKey).GetString();
                if (filePath != null)
                {
                    _requestFilesPath.Add(filePath);
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            _logger.Log(ex.Message);
            // create and close the file
            CreateAndCloseFile(ReceiverConfigFilePath);
        }
        catch (Exception ex)
        {
            _logger.Log(ex.Message);
        }
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






