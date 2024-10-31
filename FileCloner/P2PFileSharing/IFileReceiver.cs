// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareEngineeringGroupProject.FileCloner.P2PFileSharing;
public interface IFileReceiver
{
    /// <summary>
    /// This function is to be called when user wants to request for the files
    /// </summary>
    public void RequestFiles();

    /// <summary>
    /// This function is to be called when, we want to generate the diff
    /// i.e decide which file to be cloned from which server
    /// </summary>
    public void GenerateDiff();

    /// <summary>
    /// This function is to be called after generating the diff, that is
    /// once we know which file to be asked from which server
    /// </summary>
    public void RequestToCloneFiles();
}
