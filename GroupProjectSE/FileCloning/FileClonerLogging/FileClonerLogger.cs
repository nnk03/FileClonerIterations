// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GroupProjectSE.FileCloner.FileClonerLogging;
public class FileClonerLogger
{
    private string _moduleName;
    private object _syncLock;
    private const string LogFile = ".\\FileClonerTrace.log";
    private bool _writeToFile;

    /// <summary>
    /// Logger which logs to the Trace, with Module Name
    /// </summary>
    /// <param name="moduleName"></param>
    public FileClonerLogger(string moduleName, bool writeToFile)
    {
        _moduleName = moduleName;
        _syncLock = new();
        _writeToFile = writeToFile;

        try
        {
            if (!File.Exists(LogFile))
            {
                File.Create(LogFile).Close();
            }
            lock (_syncLock)
            {
                File.WriteAllText(LogFile, string.Empty);
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine("FILECLONERTRACE : " + ex.Message);
        }

    }

    /// <summary>
    /// Logs the message to the Trace
    /// </summary>
    /// <param name="message"></param>
    /// <param name="memberName"></param>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    //public void Log(string message,
    //                [CallerMemberName] string memberName = "",
    //                [CallerFilePath] string filePath = "",
    //                [CallerLineNumber] int lineNumber = 0)
    //{
    //    // string moduleName = System.IO.Path.GetFileNameWithoutExtension(filePath);
    //    Trace.WriteLine($"{_moduleName}:{filePath}->{memberName}->{lineNumber} :: {message}");
    //}

    public void Log(string message)
    {
        string logToBeWritten = $"{message}";
        if (_writeToFile)
        {
            lock (_syncLock)
            {
                try
                {
                    File.AppendAllText(LogFile, logToBeWritten);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"FILECLONERTRACE : {ex.Message}");
                }
            }
        }
        Trace.WriteLine(logToBeWritten);
    }

}
