using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Graphics.Printing.Workflow;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VirtualPdfPrinterPSA
{
    public sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            //var logPath = @"C:\Work\DocuWare\docuware-v2\psa-invoked.txt";
            //Directory.CreateDirectory(Path.GetDirectoryName(logPath));

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string logPath = Path.Combine(localFolder.Path, "psa-invoked.txt");

            File.AppendAllText(logPath, $"VirtualPdfPrinterPSA1 PSA launched at {DateTime.Now}\r\n");
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.PrintSupportJobUI)
            {
                var rootFrame = new Frame();

                rootFrame.Navigate(typeof(JobUIPage));
                Window.Current.Content = rootFrame;

                var jobUI = rootFrame.Content as JobUIPage;

                // Get the activation arguments
                var workflowJobUIEventArgs = args as PrintWorkflowJobActivatedEventArgs;

                PrintWorkflowJobUISession session = workflowJobUIEventArgs.Session;
                session.PdlDataAvailable += jobUI.OnPdlDataAvailable;
                session.JobNotification += jobUI.OnJobNotification;
                session.VirtualPrinterUIDataAvailable += jobUI.VirtualPrinterUIDataAvailable;
                // Start firing events
                session.Start();
            }
        }
    }
}