// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Networking;
using Networking.Communication;
using Networking.Serialization;

namespace SoftwareEngineeringGroupProject.FileCloner.P2PFileSharing;
public class FileReceiver : FileClonerHeaders, INotificationHandler
{
    // this must establish a connection with every file server
    // get serverAddress i.e IP : Port of all file Servers
    // save it somewhere
    // and then request through each socket about the availability of files
    private const string CurrentModuleName = "FileReceiver";
    private Logger.Logger _logger = new(CurrentModuleName);

    private CommunicatorClient _fileReceiver;
    private Serializer _serializer = new Serializer();
    private Dictionary<string, TcpClient> _receiverToSenderMap = new();
    private List<string> _fileServerAddresses = new();

    private const string ReceiverConfigFilePath = ".\\requestConfig.json";
    private const string ResponseOfRequestConfigFilePath = ".\\responseOfRequestConfig.json";
    private object _syncLockForSavingResponse = new();

    private List<string> _requestFilesPath = new();

    // the file to be cloned is saved in this field of the JSON object in the config.json
    private const string ReceiverConfigFilePathKey = "filePath";
    private const string ReceiverConfigSavePathKey = "savePath";
    public FileReceiver()
    {
        // for each file to be received from a particular device D
        // creates a new FileReceiver which handles the receiving and saving of the particular file
        _fileReceiver = new CommunicatorClient();

        // Subscribe for messages with module name as "FileReceiver"
        _fileReceiver.Subscribe("FileReceiver", this, false);

        // broadcast the message of getting all IP
        _fileReceiver.Send(GetAllIPPortHeader, "FileCloner", null);

        CreateAndCloseFile(ReceiverConfigFilePath);


        // when starting, read the config file
        SaveFileRequests();
    }

    // send the request for files
    public void SaveFileRequests()
    {
        // check the FILE_REQUEST.json file which contains list of files to be cloned
        // every item in the list is a JSON object with keys being fileName and values being filePath

        _requestFilesPath = new();
        try
        {
            string jsonContent = File.ReadAllText(ReceiverConfigFilePath);
            using JsonDocument doc = JsonDocument.Parse(jsonContent);

            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                string? filePath = element.GetProperty("filePath").GetString();
                if (filePath != null)
                {
                    _requestFilesPath.Add(filePath);
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            Trace.WriteLine("[FileCloner] FileReceiver.SaveFileRequests : " + ex.Message);
            // create and close the file
            try
            {
                File.Create(ReceiverConfigFilePath).Close();
            }
            catch (Exception innerEx)
            {
                _logger.Log(innerEx.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.Log(ex.Message);
        }

    }

    public void OnDataReceived(string serializedData)
    {
        if (serializedData.StartsWith("<ACK_FILE_REQUESTS>:"))
        {
            // find the IP address and port number of the machine
            // now assuming its localhost_9999

            string fromWhichServer = "localhost_9999";
            Thread saveResponseThread = new Thread(() => {
                SaveResponse(serializedData, fromWhichServer);
            });
            saveResponseThread.Start();
        }

    }

    private void SaveResponse(string data, string fromWhichServer)
    {
        string saveFileName = $"{fromWhichServer}.json";
        if (!CreateAndCloseFile(saveFileName))
        {
            _logger.Log($"Not able to create file {saveFileName}");
            return;
        }
        // data is serialized json
        // saving it in the fileName saveFileName

        File.WriteAllText(saveFileName, data);
    }

    public void CloneFile(string filePath, string savePath, string fileServerIP, string fileServerPort)
    {
        // get the file from the fileServer and save it in savePath
    }


    public void RequestFiles()
    {
        // broadcast the request to all file servers
        string sendFileRequests = _serializer.Serialize(_requestFilesPath);
        _fileReceiver.Send(FileRequestHeader + sendFileRequests, CurrentModuleName, null);
    }
    private bool CreateAndCloseFile(string filePath)
    {
        // returns if success or failure
        try
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            return true;
        }
        catch (Exception ex)
        {
            Trace.Write(ex);
        }

        return false;
    }

}
