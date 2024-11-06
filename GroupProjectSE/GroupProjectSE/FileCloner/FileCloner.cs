using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupProjectSE.FileCloning.FileSharing;

namespace GroupProjectSE.FileCloner;

public static class FileCloner
{
    private static FileSender s_fileSender = new();
    private static FileReceiver s_fileReceiver = new();

    public static FileSender GetFileSender()
    {
        return s_fileSender;
    }

    public static FileReceiver GetFileReceiver()
    {
        return s_fileReceiver;
    }


}
