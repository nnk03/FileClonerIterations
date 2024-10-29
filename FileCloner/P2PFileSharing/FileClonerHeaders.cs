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
    protected const string FileRequestHeader = "<FILE_REQUESTS>:";
    protected const string AckFileRequestHeader = "<ACK_FILE_REQUESTS>:";
    protected const string CloneFilesHeader = "<CLONE_FILES>:";
    protected const string AckCloneFilesHeader = "<ACK_CLONE_FILES>:";
    protected const string GetAllIPPortHeader = "<GET_ALL_IP_PORT>:";
    protected const string AckGetAllIPPortHeader = "<ACK_GET_ALL_IP_PORT>:";

}
