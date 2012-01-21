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
using System.ComponentModel;
using TOB.Entities;
using TOB.TweetSharpWrap;

namespace TweetOBoxMain.UserControls
{
    /// <summary>
    /// Interaction logic for lstAccountControl.xaml
    /// </summary>
    public partial class lstAccountControl : UserControl
    {
        public lstAccountControl()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(lstAccountControl_Loaded);
        }

        void lstAccountControl_Loaded(object sender, RoutedEventArgs e)
        {
            List<Account> account = AccountManager.Instance.GetCurrentAccounts();
            if (account.Count == 1)
            {
                lstAccount.Visibility = Visibility.Collapsed;
            }
        }

    
        private void lstAccount_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                foreach (AccountPanel acc in lstAccount.ItemsSource)
                {
                    acc.IsSelected = lstAccount.IsEnabled;
                }
            }
        }

        private void chkAccountName_Click(object sender, RoutedEventArgs e)
        {
            foreach (AccountPanel acc in lstAccount.ItemsSource)
            {
                if (acc.IsSelected == true)
                {
                    return;
                }
            }
            CheckBox chk = sender as CheckBox;
            chk.IsChecked = true;
        }
    }
}
