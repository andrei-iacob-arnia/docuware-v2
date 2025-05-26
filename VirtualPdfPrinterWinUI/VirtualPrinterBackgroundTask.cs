using System;
using System.IO;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace VirtualPdfPrinterWinUI
{
    public sealed class VirtualPrinterBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                //var logPath = @"C:\Work\DocuWare\docuware-v2\psa-invoked.txt";
                //Directory.CreateDirectory(Path.GetDirectoryName(logFile));

                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                string logPath = Path.Combine(localFolder.Path, "psa-invoked.txt");

                File.AppendAllText(logPath, $"VirtualPrinterBackgroundTask launched at {DateTime.Now}\r\n");
            }
            finally
            {
                taskInstance.GetDeferral().Complete();
            }
        }
    }
}
