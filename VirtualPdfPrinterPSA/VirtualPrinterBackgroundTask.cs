using System;
using System.IO;
using Windows.ApplicationModel.Background;
using Windows.Devices.Printers;
using Windows.Graphics.Printing.Workflow;
using Windows.Storage;
using Windows.Storage.Streams;

namespace VirtualPdfPrinterPSA
{
    public sealed class VirtualPrinterBackgroundTask : IBackgroundTask
    {
        private BackgroundTaskDeferral taskDeferral;
        private IppPrintDevice printDevice;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var logFile = @"C:\Work\DocuWare\docuware-v2\Logs\psa-invoked.txt";
            Directory.CreateDirectory(Path.GetDirectoryName(logFile));
            File.AppendAllText(logFile, $"VirtualPrinterBackgroundTask launched at {DateTime.Now}\r\n");

            var virtualPrinterDetails = taskInstance.TriggerDetails as PrintWorkflowVirtualPrinterTriggerDetails;
            taskDeferral = taskInstance.GetDeferral();

            var session = virtualPrinterDetails.VirtualPrinterSession;
            session.VirtualPrinterDataAvailable += VirtualPrinterDataAvailable;
            printDevice = session.Printer;

            session.Start();
        }

        private async void VirtualPrinterDataAvailable(PrintWorkflowVirtualPrinterSession sender, PrintWorkflowVirtualPrinterDataAvailableEventArgs args)
        {
            PrintWorkflowSubmittedStatus jobStatus = PrintWorkflowSubmittedStatus.Failed;

            try
            {
                var sourceContent = args.SourceContent;
                if (sourceContent.ContentType != "application/oxps")
                    throw new InvalidDataException("Expected OXPS content type");

                var printerPath = this.printDevice.PrinterUri.AbsolutePath.Trim('/').ToLowerInvariant();

                // Route job based on the port name
                StorageFile targetFile = await args.GetTargetFileAsync();
                IRandomAccessStream outputStream = await targetFile.OpenAsync(FileAccessMode.ReadWrite);

                if (printerPath == "virtualpdfprinterqueuenameport")
                {
                    var converter = args.GetPdlConverter(PrintWorkflowPdlConversionType.XpsToPdf);
                    await converter.ConvertPdlAsync(args.GetJobPrintTicket(), sourceContent.GetInputStream(), outputStream.GetOutputStreamAt(0));
                    jobStatus = PrintWorkflowSubmittedStatus.Succeeded;
                }
                else
                {
                    // Optionally handle more ports
                    throw new InvalidDataException("Unrecognized printer port: " + printerPath);
                }
            }
            catch (Exception ex)
            {
                var errorLog = @"C:\Work\DocuWare\docuware-v2\Logs\psa-error.txt";
                File.AppendAllText(errorLog, $"[{DateTime.Now}] Error: {ex.Message}\r\n");
            }
            finally
            {
                args.CompleteJob(jobStatus);
                taskDeferral.Complete();
            }
        }
    }
}
