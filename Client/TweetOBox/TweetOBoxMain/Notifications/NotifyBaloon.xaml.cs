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
using TOB.Entities;

using System.Collections;
using System.Windows.Threading;

namespace TweetOBoxMain.Notifications
{
    /// <summary>
    /// Interaction logic for NotifyBaloon.xaml
    /// </summary>
    public partial class NotifyBaloon : UserControl
    {
        private DispatcherTimer _timer = null;
        public NotifyBaloon()
        {
            InitializeComponent();
            
        }

        public void GetNotifications(List<TOBEntityBase> tobEntityBase)
        {
            try
            {                
                NotifylistPager.ItemsSource = tobEntityBase;
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(2);
                _timer.Tick += new EventHandler(_timer_Tick);
                _timer.Start();
                tbCount.Text = "1";
                tbTotalCount.Text = tobEntityBase.Count.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        ListPager _notifylistPager;
        ListPager NotifylistPager
        {
            get
            { 
                if(_notifylistPager == null)
                    _notifylistPager = (ListPager)this.FindResource("listPager");

                return _notifylistPager;
            }
        }

        IEnumerable<TOBEntityBase> ListEntityBase
        {
            get
            {
                return NotifylistPager.ItemsSource as IEnumerable<TOBEntityBase>;
            }
        }

        int currentIndex = 0;

        void _timer_Tick(object sender, EventArgs e)
        {
            if (currentIndex < ListEntityBase.Count())
            {
                tbCount.Text = (currentIndex + 1).ToString();
                NotifylistPager.CurrentPageIndex = currentIndex;
                currentIndex++;
            }
            else
            {
                _timer.Stop();
            }            
        }

        private void btnNotifyClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            this.Visibility = Visibility.Collapsed;
        }
    }
}
