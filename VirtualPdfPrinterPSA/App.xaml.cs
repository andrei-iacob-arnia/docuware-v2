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
            try
            {
                // Log indirectly by creating a command flag
                var triggerFile = @"C:\Work\DocuWare\docuware-v2\Logs\psa-invoked.txt";
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(triggerFile));
                File.WriteAllText(triggerFile, $"UWP PSA launched at {DateTime.Now}");

                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
            catch (Exception ex)
            {
                // Log indirectly by creating a command flag
                var triggerFile = @"C:\Work\DocuWare\docuware-v2\Logs\psa-invoked.txt";
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(triggerFile));
                File.WriteAllText(triggerFile, $"Failed to launch full trust process: {ex.Message}");

                // Optional: handle error launching the helper
                //System.Diagnostics.Debug.WriteLine("Failed to launch full trust process: " + ex.Message);
            }

            // Exit UWP after triggering the WPF app
            Application.Current.Exit();
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}