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

namespace TOBPlugin.TweetStats
{
    /// <summary>
    /// Interaction logic for TweetStats.xaml
    /// </summary>
    public partial class TweetStatsUC : UserControl
    {
       
        public TweetStatsUC()
        {
            InitializeComponent();
        }

        public TweetStatsUC(TweetStatsData tsd)
        {
            InitializeComponent();

            ucListbox.ItemsSource = tsd.tweetStatsData;
            
        }

        public void SetItemSource(Dictionary<string, int> dict)
        {
            Application.Current.Dispatcher.Invoke(new Action(delegate()
            {
                ucListbox.ItemsSource = dict;
            }));
        }
        


    }
}
