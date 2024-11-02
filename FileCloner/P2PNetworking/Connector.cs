// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FileCloner.P2PNetworking;
public class Connector
{
    private string _serverHostname = "localhost";
    private int _serverPortNumber = 8888;

    public Connector()
    {
        Thread serverThread = new Thread(StartFileServer) {
            IsBackground = true
        };
        serverThread.Start();
    }

    public void CheckClient(TcpClient incomingClient)
    {
        if (incomingClient == null) { return; };




    }

    public void StartFileServer()
    {
        // listen for connections with a particular signature message and send the requested files to that connection
        try
        {

            // listen from any client
            TcpListener listener = new TcpListener(IPAddress.Any, _serverPortNumber);

            listener.Start();
            Debug.WriteLine($"Server started and listening on {_serverHostname}:{_serverPortNumber}");

            while (true)
            {
                TcpClient incomingClient = listener.AcceptTcpClient();

            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in Starting File server : {ex.Message}");
        }
    }

    public void ServeFiles(TcpClient client)
    {
        // receive file names from the client and send the requested files and close the connection

    }

    public bool ConnectToServer()
    {
        return false;
    }

}
