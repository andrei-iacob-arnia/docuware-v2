using System;
using System.IO;
using Windows.ApplicationModel.Background;

namespace VirtualPdfPrinterPSA
{
    public sealed class VirtualPrinterBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                var logFile = @"C:\Temp\psa-invoked.txt";
                Directory.CreateDirectory(Path.GetDirectoryName(logFile));
                File.AppendAllText(logFile, $"VirtualPrinterBackgroundTask launched at {DateTime.Now}\r\n");
            }
            finally
            {
                taskInstance.GetDeferral().Complete();
            }
        }
    }
}
