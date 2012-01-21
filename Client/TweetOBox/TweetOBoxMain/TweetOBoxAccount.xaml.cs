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
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using TOB.TweetSharpWrap;
using TOB.Entities;
using System.Net;
using TOB.BLL;
using TOB.Logger;


namespace TweetOBoxMain
{
    /// <summary>
    /// Interaction logic for TweetOBoxAccount.xaml
    /// </summary>
    public partial class TweetOBoxAccount : Window
    {       
        private OAuth _oAuth;
        private bool _hideLaunchButton;
        public TweetOBoxAccount()
        {
            InitializeComponent();       
        }      

        public TweetOBoxAccount(bool hideLaunchButton)
        {
            _hideLaunchButton = hideLaunchButton;
            InitializeComponent();           
            if (hideLaunchButton == true)
            {
                btnLaunchTweetBox.Visibility = Visibility.Hidden;
            }
            borderUsers.Visibility = Visibility.Visible;
            listBoxUser.DataContext = AccountManager.Instance.GetCurrentAccounts();
        }

        private void btnAddContact_Click(object sender, System.Windows.RoutedEventArgs e)
        {        	
            try
            {
                NormalAuth normalAuth = new NormalAuth();
                Account ac = normalAuth.Authenticate(txtUserName.Text, txtPassword.Password);
                if (ac != null)
                {
                    listBoxUser.DataContext = AccountManager.Instance.GetCurrentAccounts();                    
                    borderUsers.Visibility = Visibility.Visible;
                    txtPassword.Password = "";
                    txtUserName.Text = "";
                    lblMessage.Visibility = Visibility.Collapsed;
                    btnLaunchTweetBox.IsEnabled = true;
                    if (listBoxUser.Items.Count > 0)
                    {
                        lblAddAcc.Content = "Add another";
                        btnLaunchTweetBox.IsEnabled = true;
                    }
                }
                RefreshAccountInfo();

                //Ask to follow @beneflux and @tweetobox
                MessageBoxResult result = MessageBox.Show("Would you like to follow @beneflux for future updates regarding TweetOBox?", "Follow @beneflux?", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    TOBBaseObject tobObj = AccountManager.Instance.GetTOBObject(ac);

                    if(tobObj != null)
                        tobObj.FollowUser("beneflux");
                }
            }
            catch (AuthFailureException ex)
            {
                MessageBox.Show(ex.Message, "Add account failed",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Add account failed",
                                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                TOBLogger.WriteDebugInfo(ex.ToString());
            }
        }
		     
		 private void txtPassword_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
		 {		 	
             if (txtPassword.Password != "" && txtUserName.Text != "")
             {
                 object btnTemp = FindResource("ButtonControlTemplateMouseOver");
                 btnAddContact.Template = btnTemp as ControlTemplate;
                 btnAddContact.IsEnabled = true;
             }
             else
             {
                 object o = FindResource("ButtonControlTemplateNormal");
                 btnAddContact.Template = o as ControlTemplate;
                 btnAddContact.IsEnabled = false;
             }

		 }

		 private void txtUserName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		 {		 	
             if (txtPassword.Password != "" && txtUserName.Text != "")
             {
                 object btnTemp = FindResource("ButtonControlTemplateMouseOver");
                 btnAddContact.Template = btnTemp as ControlTemplate;
                 btnAddContact.IsEnabled = true;
             }
             else
             {
                 object o = FindResource("ButtonControlTemplateNormal");
                 btnAddContact.Template = o as ControlTemplate;
                 btnAddContact.IsEnabled = false;
             }
		 }

		 private void btnoAuthentication_Click(object sender, System.Windows.RoutedEventArgs e)
		 {
             ControlsVisibility();
             txtUserName.Text = string.Empty;
             txtPassword.Name = string.Empty;
             string x = btnoAuthentication.Content.ToString();
             if (btnoAuthentication.Content.ToString() == "Use Open Authentication?")
             {
                 borderAccount.Visibility = Visibility.Collapsed;
                 borderOAuth.Visibility = Visibility.Visible;
                 btnoAuthentication.Content = "Use regular authentication?";
             }
             else
             {
                 borderAccount.Visibility = Visibility.Visible;
                 borderOAuth.Visibility = Visibility.Collapsed;
                 btnoAuthentication.Content = "Use Open Authentication?";
             }

             //Create new object of oAuthentication everytime when user clicks ont he oAuthentication
             //Section. A new object is required in order to overwrite previous object.
             _oAuth = new OAuth();
		 }

		 private void btnAuthorize_Click(object sender, System.Windows.RoutedEventArgs e)
		 {
             lbltext1.Visibility = Visibility.Collapsed;
             lbltext2.Visibility = Visibility.Hidden;
             lbltext3.Visibility = Visibility.Collapsed;
             btnAuthorize.Visibility = Visibility.Collapsed;
             txtEnterPin.Visibility = Visibility.Visible;
             lblPin.Visibility = Visibility.Visible;
             btnAuthorizeOk.Visibility = Visibility.Visible;
             if (_oAuth != null)
             {
                 _oAuth.StartAuthorization();
             }
             
		 }

         void ControlsVisibility()
         {
             borderUsers.Visibility = Visibility.Visible;
             lbltext1.Visibility = Visibility.Visible;
             lbltext2.Visibility = Visibility.Visible;
             lbltext3.Visibility = Visibility.Visible;
             btnAuthorize.Visibility = Visibility.Visible;
             txtEnterPin.Visibility = Visibility.Collapsed;
             lblPin.Visibility = Visibility.Collapsed;
             btnAuthorizeOk.Visibility = Visibility.Collapsed;
             lblMessage.Visibility = Visibility.Collapsed;
         }

		 private void btnAuthorizeOk_Click(object sender, System.Windows.RoutedEventArgs e)
		 {
             if (string.IsNullOrEmpty(txtEnterPin.Text))
             {
                 MessageBox.Show("Enter the PIN provided by twitter.com", "Can't complete Authorization",
                                 MessageBoxButton.OK, MessageBoxImage.Error);
                 return;
             }

             string pin = txtEnterPin.Text;

             try
             {
                 _oAuth.ProcessAuthentication(pin);
                 listBoxUser.DataContext = AccountManager.Instance.GetCurrentAccounts();
                 ControlsVisibility();

                 btnLaunchTweetBox.IsEnabled = true;

                 RefreshAccountInfo();
             }
             catch (AuthFailureException oAuthEx)
             {
                 MessageBox.Show(oAuthEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                 return;
             }
		 }

         private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
         {
             this.DragMove();
         }

         private void btnClose_Click(object sender, System.Windows.RoutedEventArgs e)
         {
             if (_hideLaunchButton == false)
             {
                 Application.Current.Shutdown();
             }
             else
             {
                 this.Close();
             }
         }

         private void ListBox_MouseDown(object sender, MouseButtonEventArgs e)
         {
             //(sender as ListBox).SelectedItem = null;
         }
       
         private void btnLaunchTweetBox_Click(object sender, System.Windows.RoutedEventArgs e)
         {
             this.Hide();
             TweetOBoxMain.TOBMain main = new TOBMain();
             Application.Current.MainWindow = main;
             main.Show();            
         }

         private void btnContactDelete_Click(object sender, System.Windows.RoutedEventArgs e)
         {
             try
             {
                 Account acc = (sender as Button).DataContext as Account;
                 if (acc != null)
                 {
                     MessageBoxResult messageResult;
                     messageResult = MessageBox.Show("Are you sure you want to delete this account?", "Delete Confirmation", MessageBoxButton.YesNo);
                     if (messageResult == MessageBoxResult.Yes)
                     {
                         AccountManager.Instance.DeleteAccount(acc);
                         List<Account> currentAccount = AccountManager.Instance.GetCurrentAccounts();
                         if (currentAccount.Count == 0)
                         {
                             //Need to improve.
                             System.Windows.Forms.Application.Restart();
                             Application.Current.Shutdown();
                         }
                         else
                         {
                             listBoxUser.DataContext = currentAccount;
                         }
                     }
                 }
                 if (listBoxUser.Items.Count < 1)
                 {
                     btnLaunchTweetBox.IsEnabled = false;
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message);
             }

             finally
             {
                 RefreshAccountInfo();
             }
         }

         private void txtPassword_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
         {
             switch (e.Key)
             {
                 case Key.Enter:
                     btnAddContact_Click(sender, e);
                     txtUserName.Focus();
                     break;              
             }
         }

         //This will be called to refresh the account data as well account info usercontrol
         private void RefreshAccountInfo()
         {
             if (btnLaunchTweetBox.Visibility == Visibility.Hidden)
             {
                 AccountManager.Instance.Start();
                 TOBMain tobMain = Application.Current.MainWindow as TOBMain;
                 if(tobMain != null)
                    tobMain.RefreshAccountInfoControl();
             }

             if (AccountManager.Instance.GetCurrentAccounts().Count == 1)
             {
                 SavedPluginViewBO pluginBO = new SavedPluginViewBO();

                 //return if plugins already exists as not at startup
                 if (pluginBO.GetAll().Count > 0)
                     return;

                 //Create 2 default plugins on the home stream.
                 //Picture plugin
                 TOB.Plugin.TOBPluginInfo plugin = PluginManager.Current.GetPluginInfos().Where(s => s.Key.PluginGUID == new Guid("B9BDFA9B-107C-46dd-B7C4-38C0A7043468")).Select(s=>s.Key).First();
                 if (plugin != null)
                     pluginBO.Insert(new SavedPluginView() { PluginId = plugin.PluginGUID, PluginName = plugin.PluginName, PluginStream = "Home", PluginStreamType = 0 });

                 //TweetStat plugin
                 plugin = PluginManager.Current.GetPluginInfos().Where(s => s.Key.PluginGUID == new Guid("399A3529-D9D4-4042-8920-197A208F1E99")).Select(s => s.Key).First();
                 if (plugin != null)
                     pluginBO.Insert(new SavedPluginView() { PluginId = plugin.PluginGUID, PluginName = plugin.PluginName, PluginStream = "Home", PluginStreamType = 0 });

                 pluginBO.SaveChanges();
             }
         }
    }
}
