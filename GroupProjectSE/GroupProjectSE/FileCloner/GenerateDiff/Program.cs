// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GroupProjectSE.GenerateDiff;
public class Program
{
    public static void DiffGenerator(List<string>jsonFiles,string saveFileName)
    {
        // Assuming you have a list of JSON file paths
       // List<string> jsonFiles = new List<string> { "C:\\users\\evans samuel biju\\192.168.1.1,8080.json", "C:\\Users\\EVANS SAMUEL BIJU\\192.168.1.2,8081.json" };

        Dictionary<string, FileName> files = new Dictionary<string, FileName>();

        // We are converting each JSON file into a class which has a Dictionary of relative file paths and their timestamps
        foreach (string file in jsonFiles)
        {
            try
            {
                string text = File.ReadAllText(file);
                List<AtomicJsonClass> jsonFile = JsonSerializer.Deserialize<List<AtomicJsonClass>>(text);

                // Extract IP and Port from the filename
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                var parts = fileNameWithoutExtension.Split(',');

                if (parts.Length < 2)
                {
                    Console.WriteLine($"Filename '{fileNameWithoutExtension}' does not contain both IP and Port.");
                    continue; // Skip this file if it does not have both parts
                }

                string ipAddress = parts[0];
                if (!int.TryParse(parts[1], out int port))
                {
                    Console.WriteLine($"Invalid port number '{parts[1]}' in filename '{fileNameWithoutExtension}'.");
                    continue; // Skip this file if the port is invalid
                }

                foreach (AtomicJsonClass item in jsonFile)
                {
                    Console.WriteLine($"File name: {item.file_name}, Timestamp: {item.timestamp}, IP: {ipAddress}, Port: {port}");

                    // Check if the file already exists in the dictionary
                    if (files.TryGetValue(item.file_name, out FileName existingFileName))
                    {
                        // Update the existing entry if the current timestamp is more recent
                        if (item.timestamp > existingFileName.Date)
                        {
                            existingFileName.UpdateDate(item.timestamp);
                            Console.WriteLine($"Updated timestamp for {item.file_name} to {item.timestamp}");
                        }

                    }
                    else
                    {
                        // Create a new FileName instance and add it to the dictionary if it doesn't exist
                       files[item.file_name] = new FileName(item.file_name, item.timestamp, ipAddress, port);
                        Console.WriteLine($"Added new file {item.file_name} with timestamp {item.timestamp}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading or deserializing file {file}: {ex.Message}");
            }
        }

        // Call method to write all files to a text file
        WriteAllFilesToFile(files, $"{saveFileName}\\diffFile");
    }

    // Method to write all files in the dictionary to a file
    static void WriteAllFilesToFile(Dictionary<string, FileName> files, string outputFilePath)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            writer.WriteLine("List of all files:");
            foreach (FileName file in files.Values)
            {
                writer.WriteLine($"File Name: {file.RelativeFileName}, IP Address: {file.IP_Address}, Port: {file.Port}, Timestamp: {file.Date}");
            }
        }

        Console.WriteLine($"File information written to {outputFilePath}");
    }
}
