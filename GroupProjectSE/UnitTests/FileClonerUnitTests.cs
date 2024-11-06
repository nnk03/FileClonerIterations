﻿using GroupProjectSE.FileCloner;
using GroupProjectSE.FileCloner.FileClonerLogging;
using GroupProjectSE.FileCloning.FileSharing;

namespace UnitTests;

[TestClass]
public class FileClonerUnitTests
{
    [TestMethod]
    public void LoggerTest()
    {
        // logs will be in the path
        // C:\Users\<username\AppData\Local\GroupProjectSE\FileCloner
        FileClonerLogger logger = new("UnitTests", true);
        logger.Log("HELLO WORLD");
    }

    [TestMethod]
    public void FileClonerConfigFileCreation()
    {
        // config files are in the path
        // C:\Users\<username\AppData\Local\GroupProjectSE\FileCloner

        FileReceiver fileReceiver = FileCloner.GetFileReceiver();
        FileSender fileSender = FileCloner.GetFileSender();

        //fileReceiver.RequestFiles();
    }

    [TestMethod]
    public void DiffGeneratorTest()
    {
        // create dummy files with dummy content
        // test it using the API
        FileReceiver fileReceiver = FileCloner.GetFileReceiver();
        FileSender fileSender = FileCloner.GetFileSender();

        fileReceiver.GenerateDiff();

    }
}
