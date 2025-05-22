using System;
using System.IO;
using Windows.ApplicationModel.Background;

namespace VirtualPdfPrinterWinUI
{
    public sealed class VirtualPrinterBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                var logFile = @"C:\Work\DocuWare\docuware-v2\Logs\psa-invoked.txt";
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logFile));
                File.AppendAllText(logFile, $"VirtualPrinterBackgroundTask launched at {DateTime.Now}\r\n");
            }
            finally
            {
                taskInstance.GetDeferral().Complete();
            }
        }
    }
}
