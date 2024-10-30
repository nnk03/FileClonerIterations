﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareEngineeringGroupProject.FileCloner.P2PFileSharing;
public class FileClonerHeaders
{
    protected object _syncLock;
    protected Logger.Logger _logger;
    protected Dictionary<string, TcpClient> _clientDictionary;
    // protected const int FileServerPort = (int)SenderReceiverConstants.FileReceiverPortNumber;
    protected Dictionary<string, TcpClient> _clientIdToSocket;

    protected const string FileRequestHeader = "<FILE_REQUESTS>";
    protected const string AckFileRequestHeader = "<ACK_FILE_REQUESTS>";
    protected const string CloneFilesHeader = "<CLONE_FILES>";
    protected const string AckCloneFilesHeader = "<ACK_CLONE_FILES>";
    protected const string GetAllIPPortHeader = "<GET_ALL_IP_PORT>";
    protected const string AckGetAllIPPortHeader = "<ACK_GET_ALL_IP_PORT>";
    protected const string RequestOneFileHeader = "<REQUEST_ONE_FILE>";

    protected const int PacketSize = 4096;

    protected const int HeaderIndex = 0;
    protected const int AddressIndex = 1;
    protected const int MessageIndex = 2;
    protected const int MessageSplitLength = 3;

    /// <summary>
    /// Helper function to return the address in concatenated form
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    protected string GetConcatenatedAddress(string ipAddress, string port)
    {
        return $"{ipAddress}_{port}";
    }


    /// <summary>
    /// returns a concatenated version of myAddress, header and message to be sent
    /// </summary>
    /// <param name="myAddress"></param>
    /// <param name="header"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    protected string GetMessage(string myAddress, string header, string message)
    {
        return $"{header}:{myAddress}:{message}";
    }

    /// <summary>
    /// Gets the client ID from the dictionary _clientIdToSocket
    /// given the TcpClient `socket`
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    protected string GetClientId(TcpClient socket)
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
    protected string GetClientId(string address)
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
    protected string GetAddressFromSocket(TcpClient socket)
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

}
