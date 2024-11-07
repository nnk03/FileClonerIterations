// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProjectSE.FileCloning.FileSharing;
public interface IFileReceiver
{
    // this interface is for the device which request to clone files
    // in the current project, it is the meeting host, who requests to clone the files

    /// <summary>
    /// This function is to be called when user wants to request for the files
    /// </summary>
    public void RequestFiles();

    public void RequestFiles(string description);

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
