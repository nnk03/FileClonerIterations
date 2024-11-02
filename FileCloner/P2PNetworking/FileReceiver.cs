﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileCloner.P2PNetworking;
public class FileReceiver
{
    private const int FileServerPortNumber = (int)SenderReceiverConstants.FileServerPortNumber;
    private string _serverHostname;
    private TcpClient? _tcpClient;

    public FileReceiver(string serverHostname)
    {
        _serverHostname = serverHostname;
    }

    public void CloneFile(string filePath, string savePath)
    {
        try
        {
            // Establish a connection to the server
            using TcpClient client = new TcpClient(_serverHostname, FileServerPortNumber);
            using NetworkStream networkStream = client.GetStream();
            string cloneRequestMessage = $"<CLONE_FILE>:{filePath}";
            byte[] cloneRequestBytes = Encoding.ASCII.GetBytes(cloneRequestMessage);

            // Send the request to the server
            networkStream.Write(cloneRequestBytes, 0, cloneRequestBytes.Length);
            Debug.WriteLine($"File Clone request for file : {filePath} sent to the server.");

            // Open a file stream to save the received file
            using FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            const int BufferSize = (int)SenderReceiverConstants.PacketSize; // Size of each chunk
            byte[] fileBuffer = new byte[BufferSize];
            int bytesRead = 0;

            // Read the file data from the network stream until the end of the stream
            while ((bytesRead = networkStream.Read(fileBuffer, 0, fileBuffer.Length)) > 0)
            {
                fileStream.Write(fileBuffer, 0, bytesRead);
                Debug.WriteLine($"Received {bytesRead} bytes and written to {savePath}.");
            }

            Debug.WriteLine($"File cloned successfully from {filePath} to {savePath}.");
        }
        catch (SocketException ex)
        {
            Debug.WriteLine($"Socket error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
        }


    }

    public void RequestForFile(string filePath)
    {
        try
        {
            // Establish a connection to the server
            using TcpClient client = new TcpClient(_serverHostname, FileServerPortNumber);
            _tcpClient = client;
            using NetworkStream networkStream = client.GetStream();

            // Prepare the request message
            string requestMessage = $"<GET_FILE_REQUEST>:{filePath}";
            byte[] requestBytes = Encoding.ASCII.GetBytes(requestMessage);

            // Send the request to the server
            networkStream.Write(requestBytes, 0, requestBytes.Length);
            Debug.WriteLine($"File request for file : {filePath} sent to the server.");

            // Wait for an acknowledgment from the server
            byte[] buffer = new byte[256];
            int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            // Process the acknowledgment
            if (response == "ACK")
            {
                Debug.WriteLine($"Server acknowledged the file request for file {filePath}");
                // Additional file transfer logic can be handled here
            }
            else
            {
                Debug.WriteLine("Failed to receive acknowledgment from the server.");
            }
        }
        catch (SocketException ex)
        {
            Debug.WriteLine($"Socket error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
        }

    }


}
