using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IO = System.IO;
using System.Timers;
using TOB.Plugin;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;

namespace TOBPlugin.WebPreview
{
    /// <summary>
    /// Interaction logic for WebPreviewUC.xaml
    /// </summary>
    public partial class WebPreviewUC : UserControl
    {
        private Uri _webLink = null;
        private System.Windows.Forms.WebBrowser localWB = new System.Windows.Forms.WebBrowser();
        private string _cachePath = null;
        private ITOBPlugin _parent = null;
        
        public WebPreviewUC()
        {
            InitializeComponent();
        }

        public WebPreviewUC(ITOBPlugin parent, string user, Uri link)
        {
            InitializeComponent();

            _parent = parent;

            InfoBlock.Text = user;
            _webLink = link;

            _cachePath = IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\WebPreviewCache\\" + _webLink.AbsoluteUri.GetHashCode() + ".bmp";
            
            if (IO.File.Exists(_cachePath))
            {
                WebPagePreviewImg.Source = new BitmapImage(new Uri(_cachePath));
                PopupImage.Source = WebPagePreviewImg.Source;
                localWB.Dispose();
                localWB = null;
                return;
            }
            
            localWB.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(localWB_DocumentCompleted);
            localWB.FileDownload += new EventHandler(localWB_FileDownload);
            localWB.NewWindow += new CancelEventHandler(localWB_NewWindow);

            localWB.ScriptErrorsSuppressed = true;
            localWB.ScrollBarsEnabled = false;
            localWB.AllowNavigation = false;
            localWB.AllowWebBrowserDrop = false;
            localWB.WebBrowserShortcutsEnabled = false;
            localWB.IsWebBrowserContextMenuEnabled = false;
            
            localWB.Width = 1024;
            localWB.Height = 1024;

            localWB.Navigate(_webLink);
        }

        void localWB_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        void localWB_FileDownload(object sender, EventArgs e)
        {
            
        }

        void localWB_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            //Hack to let JavaScript based pages finish rendering.
            if (localWB.ReadyState != System.Windows.Forms.WebBrowserReadyState.Complete)
            {
                System.Threading.Thread.Sleep(0);
            }

            System.Drawing.Bitmap bp = new System.Drawing.Bitmap(1024, 1024);

            localWB.DrawToBitmap(bp, new System.Drawing.Rectangle(0, 0, 1024, 1024));

            if (!IO.File.Exists(_cachePath))
            {
                bp.Save(_cachePath);
            }

            bp.Dispose();

            WebPagePreviewImg.Source = new BitmapImage(new Uri(_cachePath));
            PopupImage.Source = WebPagePreviewImg.Source;

            localWB.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(localWB_DocumentCompleted);
            localWB.FileDownload -= new EventHandler(localWB_FileDownload);
            localWB.NewWindow -= new CancelEventHandler(localWB_NewWindow);

            //localWB.Navigate(@"about:empty");

            localWB.Dispose();
            localWB = null;


        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _parent.RemoveControlFromPanel(this as UserControl);
            WebPagePreviewImg.Source = null;
        }

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            ImageExpansionPopup.IsOpen = true;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            ImageExpansionPopup.IsOpen = false;
        }

        private void WebPagePreviewImg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_webLink != null)
            {
                Process.Start(new ProcessStartInfo(_webLink.AbsoluteUri));
            }
        } 
    }
}
