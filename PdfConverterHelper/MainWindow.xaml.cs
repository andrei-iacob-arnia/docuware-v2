using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace PdfConverterHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ConvertXpsToPdf();
            Application.Current.Shutdown();
        }

        private void ConvertXpsToPdf()
        {
            var openDlg = new OpenFileDialog
            {
                Filter = "XPS/OXPS files|*.xps;*.oxps",
                Title = "Select XPS File"
            };

            if (openDlg.ShowDialog() != true) return;
            string xpsPath = openDlg.FileName;

            var saveDlg = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Save as PDF",
                FileName = System.IO.Path.GetFileNameWithoutExtension(xpsPath) + ".pdf"
            };

            if (saveDlg.ShowDialog() != true) return;
            string pdfPath = saveDlg.FileName;

            PdfDocument pdf = new PdfDocument();

            using (var xpsDoc = new XpsDocument(xpsPath, FileAccess.Read))
            {
                var fixedDocSeq = xpsDoc.GetFixedDocumentSequence();
                foreach (var docRef in fixedDocSeq.References)
                {
                    var doc = docRef.GetDocument(false);
                    foreach (var pageRef in doc.Pages)
                    {
                        var page = pageRef.GetPageRoot(false);
                        var rtb = new RenderTargetBitmap(816, 1056, 96, 96, PixelFormats.Pbgra32);
                        rtb.Render(page);

                        var encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(rtb));
                        using var ms = new MemoryStream();
                        encoder.Save(ms);

                        PdfPage pdfPage = pdf.AddPage();
                        XGraphics gfx = XGraphics.FromPdfPage(pdfPage);
                        using XImage img = XImage.FromStream(ms);
                        gfx.DrawImage(img, 0, 0, pdfPage.Width, pdfPage.Height);
                    }
                }
            }

            pdf.Save(pdfPath);
            MessageBox.Show("PDF saved to: " + pdfPath);
        }
    }
}