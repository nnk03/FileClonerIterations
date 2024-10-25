// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileCloner.P2PNetworking;
public class FileReceiver
{
    private const int FileServerPortNumber = (int)SenderReceiverConstants.FileServerPortNumber;
    private string _serverHostname;

    public FileReceiver(string serverHostname)
    {
        _serverHostname = serverHostname;
    }

    public void RequestForFile(string filePath)
    {
        try
        {
            // Establish a connection to the server
            using TcpClient client = new TcpClient(_serverHostname, FileServerPortNumber);
            using NetworkStream networkStream = client.GetStream();

            // Prepare the request message
            string requestMessage = $"<GET_FILE_REQUEST>:{filePath}";
            byte[] requestBytes = Encoding.ASCII.GetBytes(requestMessage);

            // Send the request to the server
            networkStream.Write(requestBytes, 0, requestBytes.Length);
            Console.WriteLine("File request sent to the server.");

            // Wait for an acknowledgment from the server
            byte[] buffer = new byte[256];
            int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            // Process the acknowledgment
            if (response == "ACK")
            {
                Console.WriteLine("Server acknowledged the file request.");
                // Additional file transfer logic can be handled here
            }
            else
            {
                Console.WriteLine("Failed to receive acknowledgment from the server.");
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Socket error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }


}
