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
using System.Diagnostics;

namespace TOBPlugin.PictureHelper
{
    /// <summary>
    /// Interaction logic for PicHelperControl.xaml
    /// </summary>
    public partial class PicHelperControl : UserControl
    {
        static Dictionary<Uri, BitmapImage> ImageCache = new Dictionary<Uri, BitmapImage>();

        public PicHelperControl()
        {
            InitializeComponent();
        }

        public bool AddItem(PicHelperClass item)
        {
            if (Picture1.DataContext == null)
            {
                Picture1.DataContext = item;
                Picture1.Visibility = System.Windows.Visibility.Visible;
            }
            else if (Picture2.DataContext == null)
            {
                Picture2.DataContext = item;
                Picture2.Visibility = System.Windows.Visibility.Visible;
            }
            else
                return false;

            return true;
        }

        private void Picture_MouseEnter(object sender, MouseEventArgs e)
        {
            PicHelperClass item = (sender as Button).DataContext as PicHelperClass;

            if (!ImageCache.ContainsKey(item.Image))
            {
                ImageCache.Add(item.Image, new BitmapImage(item.Image));
            }

            PopupImage.Source = ImageCache.Where(s=>s.Key==item.Image).FirstOrDefault().Value;

            if (!ImageCache.ContainsKey(item.ProfileImage))
            {
                ImageCache.Add(item.ProfileImage, new BitmapImage(item.ProfileImage));
            }

            PopupProfileImage.Source = ImageCache.Where(s => s.Key == item.ProfileImage).FirstOrDefault().Value;
            PopupText.Text = item.Text;

            ImageExpansionPopup.IsOpen = true;
        }

        private void Picture_MouseLeave(object sender, MouseEventArgs e)
        {
            ImageExpansionPopup.IsOpen = false;
        }

        private void Picture_Click(object sender, RoutedEventArgs e)
        {
            PicHelperClass item = (sender as Button).DataContext as PicHelperClass;
            Process.Start(new ProcessStartInfo(item.OrigUri.AbsoluteUri));
        }
    }

    public class PicHelperClass
    {
        public string User { get; set; }
        public string Text {get; set; }
        public Uri Image { get; set; }
        public Uri ProfileImage { get; set; }
        public Uri OrigUri { get; set; }
    }
}
