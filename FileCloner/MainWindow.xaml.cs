using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using FileCloner.P2PNetworking;

//namespace FileCloner
//{
//    /// <summary>
//    /// Interaction logic for MainWindow.xaml
//    /// </summary>
//    public partial class MainWindow : Window
//    {
//        public MainWindow()
//        {
//            InitializeComponent();
//        }

//        private void RequestButton(object sender, RoutedEventArgs e)
//        {
//            string filePath = FilePathTextBox.Text;

//            if (string.IsNullOrEmpty(filePath))
//            {
//                MessageBox.Show("Please enter a valid FilePath", "InputError", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//            else
//            {
//                MessageBox.Show($"FilePath entered is {filePath}", "File Request", MessageBoxButton.OK, MessageBoxImage.Information);
//            }
//        }
//    }
//}

namespace FileCloner;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Dictionary<string, FileReceiver>_fileReceivers = new();
    public MainWindow()
    {
        InitializeComponent();

        // currently _fileReceivers will only have localhost
        _fileReceivers.Add("localhost", new FileReceiver("localhost"));

        // every device will act as a file sender
        FileSender fileSender = new FileSender();

    }

    private void RequestButtonClick(object sender, RoutedEventArgs e)
    {
        // currently just checks if file exists
        string filePath = FilePathTextBox.Text;

        if (string.IsNullOrEmpty(filePath))
        {
            MessageBox.Show("Please enter a valid FilePath", "InputError", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if (File.Exists(filePath))
        {
            MessageBox.Show($"File found: {filePath}", "File Found", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show($"File not found: {filePath}", "File Found", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        FileReceiver fileReceiver = _fileReceivers["localhost"];
        Thread requestForFile = new(() => { fileReceiver.RequestForFile(filePath); });
        requestForFile.Start();
    }

    private void PullFileFromFilePath(string filePath)
    {
        // filePath will be valid
        // P2PNetworking.FilePuller filePuller = new(filePath);




    }

    private void CloneAndSaveButtonClick(object sender, RoutedEventArgs e)
    {
        string filePath = FilePathTextBox.Text;
        string savePath = SavePathTextBox.Text;

        if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(savePath))
        {
            MessageBox.Show("Please enter valid file path and save path", "InputError", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        if (Directory.Exists(savePath))
        {
            Debug.WriteLine($"Directory Exists : {savePath}");
        }
        else
        {
            Debug.WriteLine($"Need to create the file : {savePath}");
        }

        FileReceiver fileReceiver = _fileReceivers["localhost"];
        Thread cloneFileThread = new Thread(() => {
            fileReceiver.CloneFile(filePath, savePath);
        });
        cloneFileThread.Start();

    }
}
