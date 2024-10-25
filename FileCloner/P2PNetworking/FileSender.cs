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
using System.Windows;

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
            // Expected header format for file request
            const string RequestHeader = "<GET_FILE_REQUEST>:";
            const string CloneHeader = "<CLONE_FILE>:";
            // bool requestAccepted = false;
            bool requestAccepted = true; // for debugging
            using NetworkStream networkStream = client.GetStream();
            // Buffer to read client request
            byte[] buffer = new byte[(int)SenderReceiverConstants.PacketSize];
            int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
            string requestMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            if (requestMessage.StartsWith(RequestHeader))
            {
                Debug.WriteLine($"Received Request : {requestMessage} from Client");
                // Extract file path from request
                string filePath = requestMessage.Substring(RequestHeader.Length);

                if (File.Exists(filePath))
                {
                    // Send acknowledgment to client
                    byte[] ackResponse = Encoding.ASCII.GetBytes("ACK");
                    networkStream.Write(ackResponse, 0, ackResponse.Length);
                    Debug.WriteLine("File found. ACK sent to client.");
                    requestAccepted = true;

                    // Send the file
                    // SendFile(networkStream, filePath);
                }
                else
                {
                    byte[] nakResponse = Encoding.ASCII.GetBytes("NAK");
                    networkStream.Write(nakResponse, 0, nakResponse.Length);
                    Debug.WriteLine("Requested file does not exist. so sending NAK");
                }
            }
            else if (requestAccepted && requestMessage.StartsWith(CloneHeader))
            {
                Debug.WriteLine($"Received Request : {requestMessage} from Client");
                // Send the file
                SendFile(networkStream, requestMessage);
            }
            //else
            //{
            //    Debug.WriteLine("Invalid request header from client.");
            //}
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
        Debug.WriteLine("Sending File");
        string currentDirectory = Directory.GetCurrentDirectory();
        Debug.WriteLine($"Current WOrking directory is {currentDirectory}");


        const int BufferSize = (int)SenderReceiverConstants.PacketSize; // 4KB buffer size
        byte[] buffer = new byte[BufferSize];

        try
        {
            // need to give the actual file path not sure what is the bug....

            // this is the file to be read and written to
            string debugFilePath = "..\\..\\..\\..\\..\\..\\..\\dummy.txt";
            using FileStream fileStream = new FileStream(debugFilePath, FileMode.Open, FileAccess.Read);
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                networkStream.Write(buffer, 0, bytesRead);
                Debug.WriteLine($"Sent {bytesRead} bytes to client.");
            }
            Debug.WriteLine($"File '{filePath}' sent to client in chunks.");
            Application.Current.Dispatcher.Invoke(new Action(() => {
                MessageBox.Show("Cloned Successfully");
            }));

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error sending file: {ex.Message}");
        }
    }


}
