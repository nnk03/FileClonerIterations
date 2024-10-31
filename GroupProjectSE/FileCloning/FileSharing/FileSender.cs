// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Networking.Communication;
using Networking;

namespace GroupProjectSE.FileCloning.FileSharing;
public class FileSender : FileClonerHeaders, INotificationHandler
{
    private const string CurrentModule = "FileSender";
    private CommunicatorServer _fileServer;

    private string _myServerAddress;

    // get clientIdToSocket Dictionary
    public FileSender() : base(CurrentModule)
    {

        // create a file server in each device to serve the files
        _fileServer = new CommunicatorServer();
        _myServerAddress = _fileServer.Start();
        _myServerAddress = _myServerAddress.Replace(':', '_');

        // subscribe to messages with module name as "FileSender"
        _fileServer.Subscribe(CurrentModule, this, false);

        // gets the reference of the map
        _clientIdToSocket = _fileServer.GetClientList();
    }

    public void OnDataReceived(string serializedData)
    {
        // after the header contains the serialized data
        string[] serializedDataList = serializedData.Split(':', MessageSplitLength);
        string header = serializedDataList[HeaderIndex];
        string sendToAddress = serializedDataList[AddressIndex];
        string clientId = GetClientId(sendToAddress);

        if (header == FileRequestHeader)
        {
            string serializedRequest = serializedDataList[MessageIndex];
            Thread ackFileRequestThread = new(() => {
                ResponseToFileRequest(serializedRequest, clientId);
            });
            ackFileRequestThread.Start();

        }
        else if (header == CloneFilesHeader)
        {
            string filePath = serializedDataList[MessageIndex];
            // take the contents of the file path and send it in chunks

            Thread sendFileThread = new(() => {
                SendFileOverNetwork(filePath, clientId);
            });
            sendFileThread.Start();
        }

    }

    private void SendFileOverNetwork(string filePath, string clientId)
    {
        try
        {
            // Open the file with a StreamReader
            using StreamReader reader = new StreamReader(filePath);
            char[] buffer = new char[PacketSize];
            int charsRead;

            FileInfo fileInfo = new(filePath);
            // get the size of file in bytes
            long totalFileSize = fileInfo.Length;

            long count = 0;
            long numberOfTransmissionsRequired =
                (long)Math.Ceiling((double)totalFileSize / (double)buffer.Length);

            _logger.Log(
                $"Number of transmissions required for file {filePath} is {numberOfTransmissionsRequired}"
            );

            // Read the file in chunks
            while ((charsRead = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                // Convert the characters read into a string
                string chunk = new string(buffer, 0, charsRead);

                // Send the chunk over the network
                _fileServer.Send(
                    GetMessage(AckCloneFilesHeader, $"{filePath}:{count}/{numberOfTransmissionsRequired}:{chunk}"),
                    CurrentModule, clientId);
                ++count;
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (file not found, network issues, etc.)
            Console.WriteLine($"Error sending file: {ex.Message}");
        }

    }

    private void ResponseToFileRequest(string serializedRequest, string clientId)
    {
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

        _fileServer.Send(
            GetMessage(AckFileRequestHeader, jsonResponse),
            CurrentModule, clientId);
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

    public void StopFileSender()
    {
        _fileServer.Stop();
    }

}
