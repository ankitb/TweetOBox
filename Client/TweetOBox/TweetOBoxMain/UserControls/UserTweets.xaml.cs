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
using TOB.TweetSharpWrap;
using TOB.BLL;
using TweetOBoxMain.Utility;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using TweetOBoxMain.Controller;

namespace TweetOBoxMain.UserControls
{
    /// <summary>
    /// Interaction logic for UserTweets.xaml
    /// </summary>
    public partial class UserTweets : UserControl
    {
        public event EventHandler OnCollapseUserDetails;
        private TOBEntityEnum _currentMode = TOBEntityEnum.Status;
        SearchTableEnum _searchTableEnum = SearchTableEnum.Tweets;
        private Status status = null;
        private DirectMessage _directMessage = null;
        //UserAllTweets _tweetsArea;
        StatusPanelController _panelController;
        //AccountInfo _accountInfo;
        public UserTweets()
        {
            InitializeComponent();
        }

        private void userControlTweets_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {            
            if(!DesignerProperties.GetIsInDesignMode(this))
            {
                BindAccounts();
            }
        }

        void BindAccounts()
        {
            var tobAcc = AccountManager.Instance.GetCurrentAccounts();
            
            if (tobAcc != null)
            {
                listAccountNames.DataContext = tobAcc;
                listAccountNames.SelectedIndex = 0;
            }
        }

        void AccountClick(object sender, RoutedEventArgs e)
        {
            TweetOBoxAccount accWindow = new TweetOBoxAccount(true);
            accWindow.ShowDialog();
            BindAccounts();
        }            

        private void btnClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //ClientKUI.Instance.TOBStop();
            Application.Current.MainWindow.Hide();
        }

        private void btnTweets_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            borderTweetText.Visibility = Visibility.Visible;            
        }
               
        private void btnSharePicture_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();
                openFile.ShowDialog();
                if (openFile.FileName == "")
                {
                    return;
                }
                TwitPic tp = new TwitPic();
                //byte[] imageBuffer = tp.ReadImage(openFile.FileName, new string[] { ".bmp", ".png", ".jpg", ".jpeg", ".gif" });
                foreach (Account Accountlist in listAccountNames.SelectedItems)
                {
                    txtTweets.Text += tp.UploadPhoto(openFile.FileName, "ecd6bea2c904d8a8163656008fbf2f4c", Accountlist.AccessToken, Accountlist.AccessTokenSecret, ConfigurationManager.AppSettings["ConsumerKey"], ConfigurationManager.AppSettings["ConsumerSecret"]) + " ";
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSendTweets_Click(object sender, System.Windows.RoutedEventArgs e)
        {           
            try
            {
                string tweetText = txtTweets.Text;
                if (tweetText.Length > 140)
                {
                    return;
                }               
                if (status != null && (_currentMode == TOBEntityEnum.DirectMessages || _currentMode == TOBEntityEnum.Replies))
                {                   
                    TOBBaseObject Twitter = AccountManager.Instance.GetTOBObject(status.Account);
                    Twitter.UploadNewStatus(txtTweets.Text);
                    MessageNotifier.Instance.NotifyMessage("Direct Message sent");
                }
                else
                {                   
                    foreach (Account listItem in listAccountNames.SelectedItems)
                    {
                        TOBBaseObject Twitter = AccountManager.Instance.GetTOBObject(listItem);
                        Twitter.UploadNewStatus(txtTweets.Text);
                        TOB.Utility.MessageNotifier.Instance.NotifyMessage("Tweet sent");
                    }
                }
                CollapseTweetBox();
                btnTweet.IsChecked = false;

            }
            catch (AuthFailureException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                txtTweets.Text = string.Empty;
                _currentMode = TOBEntityEnum.Status;
            }
        }

        private void CheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (listAccountNames.SelectedItems.Count == 0)
            {
                CheckBox chk = sender as CheckBox;
                chk.IsChecked = true;
            }
        }

        private void btnMinimize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TOBMain ToBMain = new TOBMain();
            ToBMain.WindowState = WindowState.Minimized;            
        }

        public void ExpandTweetBox()
        {
            Storyboard _sb = (Storyboard)this.FindResource("OnTweetButtonChecked");
            //Storyboard _sbExpand = (Storyboard)this.FindResource("TweetsExpandStoryBoard");
           // _sbExpand.Begin();
            _sb.Begin();
            txtTweets.Focus();
        }

        public void CollapseTweetBox()
        {
            Storyboard _sb = (Storyboard)this.FindResource("TweetsCollapseStoryBoard");
            _sb.Begin();
        }
        
        public void TweetActions(object entitybase, TOBEntityEnum tweetEnum)
        {
            _currentMode = tweetEnum;
            btnTweet.IsChecked = true;

            string screenname = null;
            string text = null;

            if (entitybase is Status)
            {
                this.status = entitybase as Status;
                screenname = status.UserProfile.ScreenName;
                text = status.Text;
            }
            else if (entitybase is TweetSharp.TwitterStatus)
            {
                TweetSharp.TwitterStatus ts = (entitybase as TweetSharp.TwitterStatus);
                screenname = ts.User.ScreenName;
                text = ts.Text;
            }
            else if (entitybase is TweetSharp.TwitterSearchStatus)
            {
                TweetSharp.TwitterSearchStatus ts = (entitybase as TweetSharp.TwitterSearchStatus);
                screenname = ts.FromUserScreenName;
                text = ts.Text;
            }
            else if (entitybase is DirectMessage)
            {
                this._directMessage = entitybase as DirectMessage;
                screenname = _directMessage.UserProfile.ScreenName;
            }
            else if (entitybase is string)
            {
                txtTweets.Text = "d " + entitybase + " ";
            }

            switch (tweetEnum)
            {
                case TOBEntityEnum.ReTweet:
                    {
                        txtTweets.Text = "RT " + "@" + screenname + ": " + text;
                        break;
                    }
                case TOBEntityEnum.DirectMessages:
                    {
                        txtTweets.Text = "d " + screenname + " ";
                        break;
                    }
                case TOBEntityEnum.Replies:
                    {
                        txtTweets.Text = "@" + screenname + " ";
                        break;
                    }
            }             

            ExpandTweetBox();
       
        }

        public void DirectMessageAction(string username)
        {

        }
               
        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CollapseTweetBox();
            btnTweet.IsChecked = false;
        	txtTweets.Text="";
            _currentMode = TOBEntityEnum.Status;
        }

        public void ChangeGlobelSearchType(string searchTyprString)
        {
            switch (searchTyprString.ToLower())
            {
                case "tweets search":
                    ChangeGlobelSearchType(SearchTableEnum.Tweets);
                    break;
                case "peoples search":
                    ChangeGlobelSearchType(SearchTableEnum.Peoples);
                    break;               
                default:
                    break;
            }
        }

        public void ChangeGlobelSearchType(SearchTableEnum searchType)
        {
            _searchTableEnum = searchType;
        }

        public void ChangeGlobelSearchType(int ChangeBy)
        {
            var SearchTypeComboObject = txtSearchBox.Template.FindName("cboSearchTableSelector", txtSearchBox);
            if (SearchTypeComboObject == null)
            {
                return;
            }

            ComboBox SearchTypeCombo = SearchTypeComboObject as ComboBox;

            int toBeSelectedIndex = SearchTypeCombo.SelectedIndex + ChangeBy;
            if (toBeSelectedIndex < 0 || toBeSelectedIndex >= SearchTypeCombo.Items.Count)
            {
                return;
            }

            SearchTypeCombo.SelectedIndex = toBeSelectedIndex;
        }

        private void txtSearchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    ChangeGlobelSearchType(1);
                    break;
                case Key.Up:
                    ChangeGlobelSearchType(-1);
                    break;
            }
        }

        private void PART_ContentHost_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    break;
                default:
                    break;
            }
        }

        private void cboSearchTableSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            txtSearchBox.Text = "";
            if (txtSearchBox == null)
                return;

            foreach (var item in e.AddedItems)
            {
                var searchType = item as  GlobalSearchProviderType;

                if (searchType == null)
                    continue;

                ChangeGlobelSearchType(searchType.SearchType);
            }
        }

        private void btnMInimise_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Title == "TweetOBox")
                {
                    window.WindowState = WindowState.Minimized;
                    break;
                }
            }            
        }

        public void UserSearch(UserProfile tup)
        {
            ChangeGlobelSearchType(1);
            txtSearchBox.Text = tup.ScreenName;         
        }

        private void btnMainSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangeGlobelSearchType(-1);
            txtSearchBox.Text = "";
        }

        private void txtSearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
        	if (e.Key == Key.Enter)
            {
                if (_searchTableEnum == SearchTableEnum.Peoples)
                {
                    string peopleName = txtSearchBox.Text;
                    _panelController = StatusPanelController.Current;
                    _panelController.SearchUserByUserName(txtSearchBox.Text);
                   // _panelController.LoadFindPeople(peopleName);
                }
                else
                {
                    _panelController = StatusPanelController.Current;
                    SavedSearch ss = new SavedSearch();
                    ss.Id = -1;
                    ss.SearchText = txtSearchBox.Text;                 
                    _panelController.LoadSavedSearchView(ss);                  
                }
            }
        }

        private void CommandSaveSearchCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _panelController = StatusPanelController.Current;
            _panelController.SaveSearch(txtSearchBox.Text);
           
        }

        private void CommandSaveSearchCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
               
        private void txtTweets_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {            
            if (e.Key == Key.Enter)
            {
                btnSendTweets_Click(sender, e);
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left)
            {
                Application.Current.MainWindow.DragMove();
            }
        }
    }
}

public enum SearchTableEnum
{ 
    Tweets,
    Peoples    
}