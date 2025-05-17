using System;
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
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
            catch (Exception ex)
            {
                // Optional: handle error launching the helper
                System.Diagnostics.Debug.WriteLine("Failed to launch full trust process: " + ex.Message);
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