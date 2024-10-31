// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftwareEngineeringGroupProject.FileCloner.P2PFileSharing;

namespace SoftwareEngineeringGroupProject.FileCloner;
public class FileClonerFactory
{
    public FileSender _fileSender;
    public FileReceiver _fileReceiver;

    public FileClonerFactory()
    {
        // first start the Sending server
        _fileSender = new FileSender();
        _fileReceiver = new FileReceiver();
    }
}
