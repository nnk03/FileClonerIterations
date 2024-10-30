// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareEngineeringGroupProject.FileCloner.P2PFileSharing;
public class FileClonerHeaders
{
    protected const string FileRequestHeader = "<FILE_REQUESTS>";
    protected const string AckFileRequestHeader = "<ACK_FILE_REQUESTS>";
    protected const string CloneFilesHeader = "<CLONE_FILES>";
    protected const string AckCloneFilesHeader = "<ACK_CLONE_FILES>";
    protected const string GetAllIPPortHeader = "<GET_ALL_IP_PORT>";
    protected const string AckGetAllIPPortHeader = "<ACK_GET_ALL_IP_PORT>";

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

}
