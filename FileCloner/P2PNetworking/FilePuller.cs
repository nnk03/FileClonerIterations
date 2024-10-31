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
public class FilePuller
{
    private Connector _connector;
    public FilePuller(string filePath, string hostName, int portNumber)
    {
        // create a connector and use this to send and receive files
        // currently only pulling
        // _connector = new(hostName, portNumber);


    }



}
