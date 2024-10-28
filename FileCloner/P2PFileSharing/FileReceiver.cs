// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Networking.Communication;

namespace SoftwareEngineeringGroupProject.FileCloner.P2PFileSharing;
public class FileReceiver
{
    private CommunicatorClient _communicatorClient;
    public FileReceiver()
    {
        // for each file to be received from a particular device D
        // creates a new FileReceiver which handles the receiving and saving of the particular file
        _communicatorClient = new CommunicatorClient();
    }
    public ReceiveFile(string _serverIP, string _serverPort, string _filePath)
    {
        _communicatorClient.Start(_serverIP, _serverPort);
        using FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
        using TcpClient tcpClient = new TcpClient(_serverIP, _serverPort);
        using NetworkStream networkStream = tcpClient.GetStream();
        byte[] fileBuffer = new byte[1024]; //to be changed later
        int bytesRead = 0;
        while ((bytesRead) = networkStream.Read(fileBuffer, 0, fileBuffer.Length) > 0)
        {
            fileStream.Write(fileBuffer, 0, bytesRead);
            Debug.WriteLine($"Received {bytesRead} bytes.");
        }
        _communicatorClient.Stop();
    }

}






