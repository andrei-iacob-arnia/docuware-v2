using System;
using System.Diagnostics;
using Windows.Graphics.Printing.Workflow;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VirtualPdfPrinterPSA
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class JobUIPage : Page
    {
        public JobUIPage()
        {
            this.InitializeComponent();
        }

        public void VirtualPrinterUIDataAvailable(PrintWorkflowJobUISession session, PrintWorkflowVirtualPrinterUIEventArgs args)
        {
            using (args.GetDeferral())
            {
                string jobTitle = args.Configuration.JobTitle;
                string sourceApplicationName = args.Configuration.SourceAppDisplayName;
                string printerName = args.Printer.PrinterName;

                // Get pdl stream and content type
                IInputStream pdlContent = args.SourceContent.GetInputStream();
                string contentType = args.SourceContent.ContentType;
                this.ShowPrintPreview(jobTitle, pdlContent, contentType);
            }
        }

        private void ShowPrintPreview(string jobTitle, IInputStream pdlContent, string contentType)
        {
            // Show preview to the user
        }

        public async void OnPdlDataAvailable(PrintWorkflowJobUISession sender, PrintWorkflowPdlDataAvailableEventArgs args)
        {
            try
            {
                StorageFile targetFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("jobui-pdl-data.bin", CreationCollisionOption.GenerateUniqueName);
                using (var outputStream = await targetFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await RandomAccessStream.CopyAsync(args.SourceContent.GetInputStream(), outputStream);
                }

                Debug.WriteLine("PdlDataAvailable: Data saved to " + targetFile.Path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in OnPdlDataAvailable: " + ex.Message);
            }
        }

        public void OnJobNotification(PrintWorkflowJobUISession sender, PrintWorkflowJobNotificationEventArgs args)
        {
            Debug.WriteLine("JobNotification: JobId = " + args.PrinterJob.JobId);
        }
    }
}
