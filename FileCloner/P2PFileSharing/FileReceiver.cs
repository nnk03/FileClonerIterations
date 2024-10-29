// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Networking;
using Networking.Communication;
using Networking.Serialization;

namespace SoftwareEngineeringGroupProject.FileCloner.P2PFileSharing;
public class FileReceiver : INotificationHandler
{
    // this must establish a connection with every file server
    // get serverAddress i.e IP : Port of all file Servers
    // save it somewhere
    // and then request through each socket about the availability of files


    private CommunicatorClient _fileReceiver;
    private Serializer _serializer = new Serializer();
    private Dictionary<string, TcpClient> _receiverToSenderMap = new();
    private List<string> _fileServerAddresses = new();
    public FileReceiver()
    {
        // for each file to be received from a particular device D
        // creates a new FileReceiver which handles the receiving and saving of the particular file
        _fileReceiver = new CommunicatorClient();
        // broadcast the message of getting all IP
        _fileReceiver.Send("<GET_ALL_IP_PORT>", "FileCloner", null);

    }

    public void OnDataReceived(string serializedData)
    {

    }

    public void CloneFile(string filePath, string savePath, string fileServerIP, string fileServerPort)
    {
        // get the file from the fileServer and save it in savePath
    }


    public void RequestFile(string filePath)
    {
        // broadcast the request to all file servers
        _fileReceiver.Send($"<FILE_REQUEST>:{filePath}", "FileCloner");

    }

}
