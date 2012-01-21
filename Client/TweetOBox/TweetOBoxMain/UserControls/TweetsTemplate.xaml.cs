using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using TOB.TweetSharpWrap;
using TOB.Entities;
using TweetSharp;
using TOB.Utility;
using TOB.BLL;
using System.Windows.Threading;

namespace TweetOBoxMain
{
	/// <summary>
	/// Interaction logic for AllTweetsTemplate.xaml
	/// </summary>
	public partial class TweetsTemplate : UserControl
	{
        private DispatcherTimer _timer = null;
        private bool _isRead = false;
		
        public TweetsTemplate()
		{
			this.InitializeComponent();

            if (tweetControlTemplate.DataContext is Status)
            {
                _isRead = (bool) (tweetControlTemplate.DataContext as Status).IsRead;
            }

            if (!_isRead)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(1.5);
                _timer.Tick += new EventHandler(_timer_Tick);
            }
        }        
                               
        private StatusBO _statusBO = null;

        private StatusBO LocalStatusBO
        {
            get
            {
                if (_statusBO == null)
                {
                    _statusBO = new StatusBO();
                }
                return _statusBO;
            }
        }
        void HandleLinkClick(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri.IsAbsoluteUri != false)
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
        }

        private void grdOptionsContainer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Button)
            {
                PopupTweetActions.IsOpen = false;
            }
        }

        private void grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //btnDismissUIObj.IsEnabled = true;
            //btnDismissUIObj.Visibility = Visibility.Visible;

            if (!_isRead)
            {
                _timer.Start();
            }
            
        }

        private void grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_isRead)
            {
                _timer.Stop();
            }
            
            //btnDismissUIObj.IsEnabled = false;
            //btnDismissUIObj.Visibility = Visibility.Hidden;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (tweetControlTemplate.DataContext is Status)
                {
                    Status status = tweetControlTemplate.DataContext as Status;
                    if (status.IsRead == false)
                    {
                        UpdateIsMarkAsRead(status);
                    }
                    else
                    {
                        _isRead = true;
                    }
                }
            }
            catch (Exception ex)
            {
                TOB.Logger.TOBLogger.WriteDebugInfo(ex.ToString());
            }
        }

        void UpdateIsMarkAsRead(Status status)
        {
            if (status != null)
            {
                tbtweetsText.FontWeight = FontWeights.Normal;
                _isRead = true;
                status.IsRead = true;
                LocalStatusBO.SaveChanges();
                status.OnUnReadPropertyChanged(status);
                borderBg.Background = new SolidColorBrush(Colors.White);
            }          
        }

	}
}