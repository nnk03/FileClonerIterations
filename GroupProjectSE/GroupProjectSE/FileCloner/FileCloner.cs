using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupProjectSE.FileCloning.FileSharing;

namespace GroupProjectSE.FileCloner;

public class FileCloner
{
    public FileSender _fileSender;
    public FileReceiver _fileReceiver;

    public FileCloner()
    {
        // first start the Sending server
        _fileSender = new FileSender();
        _fileReceiver = new FileReceiver();
    }

    ~FileCloner()
    {
        _fileReceiver.StopFileReceiver();
        _fileSender.StopFileSender();
    }
}
