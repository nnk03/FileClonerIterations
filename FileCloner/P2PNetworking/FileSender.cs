// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileCloner.P2PNetworking;
public class FileSender
{
    private const int FileServerPortNumber = (int)SenderReceiverConstants.FileServerPortNumber;
    public FileSender()
    {

        Thread serverThread = new Thread(StartFileServer) {
            IsBackground = true
        };
        serverThread.Start();
    }

    public void StartFileServer()
    {
        // listen for connections with a particular signature message and send the requested files to that connection
        try
        {

            // listen from any client
            TcpListener listener = new TcpListener(IPAddress.Any, FileServerPortNumber);

            listener.Start();
            Debug.WriteLine($"Server started and listening on port {FileServerPortNumber}");

            while (true)
            {
                TcpClient incomingClient = listener.AcceptTcpClient();
                Debug.WriteLine("Client connected.");

                // Start a new thread to handle each client
                Thread clientThread = new Thread(() => HandleClient(incomingClient)) {
                    IsBackground = true
                };
                clientThread.Start();

            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in Starting File server : {ex.Message}");
        }

    }

    private void HandleClient(TcpClient client)
    {
        try
        {
            using NetworkStream networkStream = client.GetStream();
            // Buffer to read client request
            byte[] buffer = new byte[256];
            int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
            string requestMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            // Expected header format for file request
            const string RequestHeader = "<GET_FILE_REQUEST>:";
            if (requestMessage.StartsWith(RequestHeader))
            {
                // Extract file path from request
                string filePath = requestMessage.Substring(RequestHeader.Length);

                if (File.Exists(filePath))
                {
                    // Send acknowledgment to client
                    byte[] ackResponse = Encoding.ASCII.GetBytes("ACK");
                    networkStream.Write(ackResponse, 0, ackResponse.Length);
                    Debug.WriteLine("File found. ACK sent to client.");

                    // Send the file
                    // SendFile(networkStream, filePath);
                }
                else
                {
                    Debug.WriteLine("Requested file does not exist.");
                }
            }
            else
            {
                Debug.WriteLine("Invalid request header from client.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error handling client: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }

    private void SendFile(NetworkStream networkStream, string filePath)
    {
        try
        {
            // Read file into byte array
            byte[] fileBytes = File.ReadAllBytes(filePath);
            networkStream.Write(fileBytes, 0, fileBytes.Length);
            Debug.WriteLine($"File '{filePath}' sent to client.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error sending file: {ex.Message}");
        }
    }


}
