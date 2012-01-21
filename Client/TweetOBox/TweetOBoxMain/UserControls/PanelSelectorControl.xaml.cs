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
using System.Windows.Controls.Primitives;
using TOB.Entities;

namespace TweetOBoxMain.UserControls
{
    /// <summary>
    /// Interaction logic for lstPanelSelectorControl.xaml
    /// </summary>
    public partial class lstPanelSelectorControl : UserControl
    {
        public lstPanelSelectorControl()
        {
            this.InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.Loaded += new RoutedEventHandler(lstPanelSelectorControl_Loaded);
            }
        }

        void lstPanelSelectorControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                lstPanelSelector.SelectedIndex = 0;                  
            }
        }
        private void toggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (lstPanelSelector.SelectedIndex == -1)
            {
                ToggleButton chk = sender as ToggleButton;
                chk.IsChecked = true;
            }
        }

        public void SelectHome()
        {
            lstPanelSelector.SelectedIndex = 0;           
        }
    }
}
