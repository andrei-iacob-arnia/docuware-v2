using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace VirtualPdfPrinterPSA
{
    public sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        /// Updated to launch the WPF helper (Full Trust Process)
        /// </summary>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var logPath = @"C:\Work\DocuWare\docuware-v2\Logs\psa-invoked.txt";
            Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            File.AppendAllText(logPath, $"VirtualPdfPrinterPSA PSA launched at {DateTime.Now}\r\n");
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}