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
using TOB.BLL;
using TOB.TweetSharpWrap;
using TweetOBoxMain.Utility;
using TweetOBoxMain.UserControls;
using TOB.Plugin;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using TweetOBoxMain.Controller;
using TweetOBoxMain.Model;
using TweetOBoxMain.Notifications;
using System.Windows.Controls.Primitives;
using TweetSharp;
using TOB.Utility;
using System.Windows.Threading;
using TweetOBoxMain.TOBKUIService;
using System.Threading;

namespace TweetOBoxMain
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class TOBMain: Window
    {
        StatusPanelController _panelController;        
        public TOBMain()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {                                
                this.Loaded += new RoutedEventHandler(TOBMain_Loaded);
                this.Closed += new EventHandler(TOBMain_Closed);              
            }
           
        }

        void IntializeInActiveFeature()
        {
            HwndSource windowSpecificOSMessageListener = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            windowSpecificOSMessageListener.AddHook(new HwndSourceHook(CallBackMethod));
            MarkAsRead.MakeLogOffEvent += new MarkAsRead.MakeLogOff(MarkAsRead_MakeLogOffEvent);
            MarkAsRead.Instance.StartActive();
        }

        void MarkAsRead_MakeLogOffEvent()
        {
            MarkAsRead.Instance.IsNotActive = true;
        }

        private IntPtr CallBackMethod(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //  Listening OS message to test whether it is a user activity
            if ((msg >= 0x0200 && msg <= 0x020A) || (msg <= 0x0106 && msg >= 0x00A0) || msg == 0x0021)
            {
                MarkAsRead.Instance.ResetTimer();                                               
            }
            //else
            //{            
            //    System.Diagnostics.Debug.WriteLine(msg.ToString());
            //}
            return IntPtr.Zero;
        }

        void TOBMain_Closed(object sender, EventArgs e)
        {
            ClientKUI.Instance.Dispose();
            AccountManager.Instance.Dispose();
        }
        
        void TOBMain_Loaded(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            var splash = new SplashScreen("/Images/splashBg.png");
            splash.Show(false);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded, (Action)(() => { splash.Close(TimeSpan.Zero); this.Visibility = Visibility.Visible; }));
          
            TOB.Utility.MessageNotifier.Instance.NotifyMessage("Populating Statuses...");
            _panelController = StatusPanelController.Current;
            AccountManager.Instance.Start();
            IntializeInActiveFeature();

        }

        void AccountClick(object sender, RoutedEventArgs e)
        {
            TweetOBoxAccount accWindow = new TweetOBoxAccount(true);
            accWindow.ShowDialog();          
        }

        private void btnClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
            this.WindowState = WindowState.Minimized;
        }

        private void UserAllTweets_OnTweetAction(object sender, TweetActionArgs e)
        {                  
            userTweets.TweetActions(e.StatusList, e.TweetType);           
        }

        #region sizing event handlers

        public void OnSizeSouth(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Window wnd = ((FrameworkElement)sender).TemplatedParent as Window;
            if (wnd != null)
            {
                Resizer resize = new Resizer();               
                WindowInteropHelper helper = new WindowInteropHelper(wnd);
                resize.DragSize(helper.Handle, TOB.Utility.SizingAction.South);
            }
        }

        public void OnSizeNorth(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Window wnd = ((FrameworkElement)sender).TemplatedParent as Window;
            if (wnd != null)
            {
                Resizer resize = new Resizer();
                WindowInteropHelper helper = new WindowInteropHelper(wnd);
                resize.DragSize(helper.Handle, TOB.Utility.SizingAction.North);
            }
        }       
           
        #endregion            

   
        private void CommandReplyTweet_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandReplyTweet_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            userTweets.TweetActions(e.Parameter, TOBEntityEnum.Replies);           
        }

        private void CommandReTweet_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            userTweets.TweetActions(e.Parameter, TOBEntityEnum.ReTweet);           
        }

        private void CommandReTweet_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = true;
        }

        private void CommandDirectTweet_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandDirectTweet_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            userTweets.TweetActions(e.Parameter, TOBEntityEnum.DirectMessages);         
        }

        private void userTweets_OnCollapseUserDetails(object sender, EventArgs e)
        {
            userProfileTOB.Visibility = Visibility.Collapsed;
        }
                       
        private void CommandShowUserProfile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandShowUserProfile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TOBEntityBase tobEntity = e.Parameter as TOBEntityBase;
            _panelController.ShowUserProfile(tobEntity);
            if (tobEntity is Status)
            {
                Status status = tobEntity as Status;
                btnHome.Content = status.UserProfile.ScreenName;
            }
            else if (tobEntity is DirectMessage)
            {
                DirectMessage directMessage = tobEntity as DirectMessage;
                btnHome.Content = directMessage.UserProfile.ScreenName;
            }
        }

        private void CommandShowDefaultView_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandShowUserProfileForTags_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string userName = e.Parameter as string;
            _panelController.ShowUserProfileForTag(userName);
        }

        private void CommandShowUserProfileForTags_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandShowFilterForTags_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string filterText = e.Parameter as string;        
            SavedSearch ss = new SavedSearch();
            ss.Id = -1;
            ss.SearchText = filterText.Substring(1);
            _panelController.LoadSavedSearchView(ss);
        }

        private void CommandShowFilterForTags_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandShowDefaultView_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Panel panel = e.Parameter as Panel;
            _panelController.LoadHomeView(panel);
            btnHome.Content = panel == null ? TOBEntityEnum.Home : TOBEntityEnum.Home ;
        }

        private void CommandShowRepliesView_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandShowRepliesView_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _panelController.LoadReplyView(e.Parameter as Panel);
            btnHome.Content = (e.Parameter as Panel).PanelName;           
        }

        private void CommandShowRetweetView_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandShowRetweetView_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _panelController.LoadReTweetView(e.Parameter as Panel);
            btnHome.Content = (e.Parameter as Panel).PanelName;
        }

        private TOBBaseObject GetTwitterBaseObject(ExecutedRoutedEventArgs e)
        {
            //if (e.Parameter is Status || e.Parameter is DirectMessage)
            {
                return AccountManager.Instance.GetTOBObjectUser(e.Parameter);
            }
            //else
            //{
            //    // If param is neither a Status or DM, get the Account ID mapping from UserAllTweets
            //    UserAllTweets uat = (e.Source as UserAllTweets);    
                
            //}

            //return null;
        }

        private void CommandUnFollowUser_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TOBBaseObject Twitter = GetTwitterBaseObject(e);
            if(Twitter != null)
                Twitter.UnFollowUser(e.Parameter);
        }

        private void CommandUnFollowUser_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBlockUser_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TOBBaseObject Twitter = GetTwitterBaseObject(e);
            if (Twitter != null)
                Twitter.BlockUser(e.Parameter);
        }

        private void CommandBlockUser_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandReportSpamUser_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TOBBaseObject Twitter = GetTwitterBaseObject(e);
            if (Twitter != null)
                Twitter.ReportSpamUser(e.Parameter);
        }

        private void CommandReportSpamUser_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandMarkAsFavourite_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TOBBaseObject Twitter = GetTwitterBaseObject(e);
            if (Twitter != null)
                Twitter.MarkAsFavorite(e.Parameter);
        }

        private void CommandMarkAsFavourite_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandShowMarkasFavourite_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _panelController.LoadMarkasFavoriteView(e.Parameter as Panel);
            btnHome.Content = (e.Parameter as Panel).PanelName;
        }

        private void CommandShowMarkasFavourite_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandShowDirectMessageView_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _panelController.LoadDirectMessageView(e.Parameter as Panel);
            btnHome.Content = (e.Parameter as Panel).PanelName;
        }

        private void CommandShowDirectMessageView_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
       
        private void CommandPluginCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavedPluginView savedPluginView = e.Parameter as SavedPluginView;
            if (savedPluginView.PluginStream.StartsWith("["))
            {
                savedPluginView.PluginStream = savedPluginView.PluginStream.Substring(1);
                savedPluginView.PluginStream = savedPluginView.PluginStream.Substring(0, savedPluginView.PluginStream.Length - 1);
            }
            _panelController.LoadPluginPanelView(savedPluginView);
            btnHome.Content = savedPluginView.PluginName;
        }

        private void CommandPluginCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandSaveFilterCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _panelController.SaveFilter();
        }

        private void CommandSaveFilterCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandFilterCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavedFilter savedFilter = e.Parameter as SavedFilter;
            //savedFilter.FilterText = savedFilter.FilterText.Substring(1);
            //savedFilter.FilterText = savedFilter.FilterText.Substring(0, savedFilter.FilterText.Length - 1);
            _panelController.LoadSavedFilterView(savedFilter);
            btnHome.Content = savedFilter.FilterText;
        }

        private void CommandFilterCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandSearchCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavedSearch savedSearch = e.Parameter as SavedSearch;
            //savedSearch.SearchText = savedSearch.SearchText.Substring(1);
            //savedSearch.SearchText = savedSearch.SearchText.Substring(0, savedSearch.SearchText.Length - 1);
            _panelController.LoadSavedSearchView(savedSearch);
            btnHome.Content = savedSearch.SearchText;
        }

        private void CommandSearchCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandDeleteSearchCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _panelController.DeleteSavedSearch(e.Parameter as SavedSearch);
        }

        private void CommandDeleteSearchCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandDeleteFilterCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _panelController.DeleteSavedFilter(e.Parameter as SavedFilter);
        }

        private void CommandDeleteFilterCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandGroupsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {     
            //loading animation 
            //this.spinner.IsRunning = true; 
          
            //Action action = delegate()
            //{
            TOB.Utility.MessageNotifier.Instance.NotifyMessage("Loading List...");
            TwitterListExtended twitterListExt = e.Parameter as TwitterListExtended;
            _panelController.LoadListView(twitterListExt);
            btnHome.Content = twitterListExt.Name;
            //    //this.spinner.IsRunning = false;
            //};

            //// The Work to perform on another thread
            //ThreadStart start = delegate()
            //{
            //    // ...

            //    // Sets the Text on a TextBlock Control.
            //    // This will work as its using the dispatcher
            //    Dispatcher.Invoke(DispatcherPriority.Background,
            //                      action);
            //};

            //// Create the thread and kick it started!
            //new Thread(start).Start();
            //Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, action);

         //   TOB.Utility.MessageNotifier.Instance.NotifyMessage("Loading List...");
            //TwitterListExtended twitterListExt = e.Parameter as TwitterListExtended;
           // _panelController.LoadingObjects += new StatusPanelController.LoadingHandler(_panelController_LoadingObjects);

            //ParameterizedThreadStart t = new ParameterizedThreadStart(OnLoaded);
            //t.Invoke(twitterListExt);

            //Thread t1 = new Thread(t);
            //t1.Start(twitterListExt);
            //btnHome.Content = twitterListExt.Name;            
        }
               
        private void CommandGroupsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandDeleteGroupCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TwitterListExtended twitterListExt = e.Parameter as TwitterListExtended;
            _panelController.DeleteList(twitterListExt);
        }

        private void CommandDeleteGroupCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandAddToGroupCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Status status = e.Parameter as Status;
            _panelController.GroupsPopUpDisplay(status);
        }

        private void CommandAddToGroupCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
        private void CommandShowListUserProfile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TwitterStatus twitterStatus = e.Parameter as TwitterStatus;
            _panelController.ShowListUserProfile(twitterStatus);
        }

        private void CommandShowListUserProfile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandRemoveFromList_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Status status = e.Parameter as Status;
            _panelController.RemoveUserFromList(status);
        }

        private void CommandRemoveFromList_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandDeletePluginCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavedPluginView savedPluginView = e.Parameter as SavedPluginView;
            _panelController.DeletePlugin(savedPluginView);
        }

        private void CommandDeletePluginCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Window_StateChanged(object sender, System.EventArgs e)
        {
            MarkAsRead.Instance.IsMinimized = true;
        }

        private void borderNavPanel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            borderAccountInfo.Visibility = Visibility.Visible;
        }

        private void borderNavPanel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            borderAccountInfo.Visibility = Visibility.Collapsed;
        }

        private void CommandDeleteTweets_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _panelController.DeleteTweets(e.Parameter as Status);
        }

        private void CommandDeleteTweets_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandDeleteDirectMessages_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _panelController.DeleteDirectMessages(e.Parameter as DirectMessage);
        }

        private void CommandDeleteDirectMessages_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandDismissUIObject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _panelController.DismissUIObject(e.Parameter);    
        }

        //private void CommandDismissUIObject_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = true;
        //}

        private void CommandFollowUser_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TwitterUser twitterUser = e.Parameter as TwitterUser;
            _panelController.FollowUser(twitterUser);
            TweetOBoxMain.Utility.MessageNotifier.Instance.NotifyMessage("You are now following - @" + twitterUser.ScreenName);
        }

        private void CommandFollowUser_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        /// <summary>
        /// This functino is required in order to refresh the account info in two cases.
        /// When a new account is added or deleted this usercontrol needs to be recreated to refrect 
        /// the actual tob account inside the database.
        /// </summary>

        public void RefreshAccountInfoControl()
        {
            Button button = borderAccountInfo.Child as Button;
            accountInfo = new AccountInfo();
            button.Content = accountInfo;
        }
      
    }
}


