// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileCloner.DiffGenerator;
class Program
{
    static void Main(string[] args)
    {
        // Assuming you have a list of JSON file paths
        var jsonFiles = new List<string> { "C:\\users\\evans samuel biju\\192.168.1.1,8080.json", "C:\\Users\\EVANS SAMUEL BIJU\\192.168.1.2,8081.json" };

        Dictionary<string, FileName> files = new Dictionary<string, FileName>();

        // We are converting each JSON file into a class which has a Dictionary of relative file paths and their timestamps
        foreach (var file in jsonFiles)
        {
            try
            {
                string text = File.ReadAllText(file);
                var jsonFile = JsonSerializer.Deserialize<List<AtomicJsonClass>>(text);

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

                foreach (var item in jsonFile)
                {
                    Console.WriteLine($"File name: {item.File_name}, Timestamp: {item.Timestamp}, IP: {ipAddress}, Port: {port}");

                    // Check if the file already exists in the dictionary
                    if (files.TryGetValue(item.File_name, out var existingFileName))
                    {
                        // Update the existing entry if the current timestamp is more recent
                        if (item.Timestamp > existingFileName.Date)
                        {
                            existingFileName.UpdateDate(item.Timestamp);
                            Console.WriteLine($"Updated timestamp for {item.File_name} to {item.Timestamp}");
                        }

                    }
                    else
                    {
                        // Create a new FileName instance and add it to the dictionary if it doesn't exist
                        files[item.File_name] = new FileName(item.File_name, item.Timestamp, ipAddress, port);
                        Console.WriteLine($"Added new file {item.File_name} with timestamp {item.Timestamp}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading or deserializing file {file}: {ex.Message}");
            }
        }

        // Call method to write all files to a text file
        WriteAllFilesToFile(files, "output.txt");
    }

    // Method to write all files in the dictionary to a file
    static void WriteAllFilesToFile(Dictionary<string, FileName> files, string outputFilePath)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            writer.WriteLine("List of all files:");
            foreach (var file in files.Values)
            {
                writer.WriteLine($"File Name: {file.RelativeFileName}, IP Address: {file.IPAddress}, Port: {file.Port}, Timestamp: {file.Date}");
            }
        }

        Console.WriteLine($"File information written to {outputFilePath}");
    }
}
