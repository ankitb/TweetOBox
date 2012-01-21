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
using TOB.BLL;
using TOB.TweetSharpWrap;
using System.ComponentModel;
using TOB.Entities;
using System.Windows.Threading;
using System.Threading;
using System.Collections;
using TweetOBoxMain.CustomControls;
using TweetOBoxMain.Controller;
using TweetOBoxMain.Model;
using System.Collections.ObjectModel;
using TweetSharp;
using TOB.Utility;
using TOB.Plugin;
using System.Windows.Controls.Primitives;


namespace TweetOBoxMain.UserControls
{
    /// <summary>
    /// Interaction logic for AccountInfo.xaml
    /// </summary>
    public partial class AccountInfo : UserControl
    {
        PanelList _panelList = new PanelList();
        ObservableCollection<SavedFilter> _savedFilerList;
        ObservableCollection<SavedSearch> _savedSearchList;
        ObservableCollection<AccountInformation> _accountInfoList;
        ObservableCollection<UserProfile> _userProfileList;
        ObservableCollection<UserProfile> _userProfileGroupList = new ObservableCollection<UserProfile>();
        ObservableCollection<TwitterListExtended> _twitterListExt;
        Status _status;
        private SavedPluginViewBO _savedPluginViewBO = null;
        private PanelTypeBO _panelTypeBO = null;
        private SavedFilterBO _savedFilterBO = null;
        private SavedSearchBO _savedSearchBO = null;
        private UserProfileBO _UserProfileBO = null;
        private AccountBO _accountBO = null;
        private DispatcherTimer _timerAccountInfo = null;
        private List<object> _listBoxList = new List<object>();
        private UserProfileBO LocalUserProfileBO
        {
            get
            {
                if (_UserProfileBO == null)
                {
                    _UserProfileBO = new UserProfileBO();
                }
                return _UserProfileBO;
            }
        }

        private SavedPluginViewBO LocalSavedPluginViewBO
        {
            get
            {
                if (_savedPluginViewBO == null)
                {
                    _savedPluginViewBO = new SavedPluginViewBO();
                }
                return _savedPluginViewBO;
            }
        }

        private PanelTypeBO LocalPanelTypeBO
        {
            get
            {
                if (_panelTypeBO == null)
                {
                    _panelTypeBO = new PanelTypeBO();
                }
                return _panelTypeBO;
            }
        }

        private SavedFilterBO LocalSavedFilterBO
        {
            get
            {
                if (_savedFilterBO == null)
                {
                    _savedFilterBO = new SavedFilterBO();
                }
                return _savedFilterBO;
            }
        }

        private SavedSearchBO LocalSavedSearchBO
        {
            get
            {
                if (_savedSearchBO == null)
                {
                    _savedSearchBO = new SavedSearchBO();
                }
                return _savedSearchBO;
            }
        }

        private AccountBO LocalAccountBO
        {
            get
            {
                if (_accountBO == null)
                {
                    _accountBO = new AccountBO();
                }
                return _accountBO;
            }
        }

        public AccountInfo()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(userAccountInfo_Loaded);
            foreach (TOBBaseObject tobbase in AccountManager.Instance.TobObjects)
            {
                tobbase.NewList += new TOBBaseObject.ListHandler(tobbase_NewList);
            }
            _timerAccountInfo = new DispatcherTimer();
            _timerAccountInfo.Interval = TimeSpan.FromSeconds(30);
            _timerAccountInfo.Tick += new EventHandler(_timerAccountInfo_Tick);
            _timerAccountInfo.Start();
            _listBoxList.Add(listSavedSearch);
            _listBoxList.Add(listSavedFilter);
            _listBoxList.Add(listPluginPanels);
            _listBoxList.Add(listGroupsNames);
        }

        void _timerAccountInfo_Tick(object sender, EventArgs e)
        {
            BindUserAccounts();
        }

        public void tobbase_NewList(TOBBaseObject from, List<TwitterListExtended> twitterLists)
        {
            Action action = delegate()
            {
                foreach (TwitterListExtended tle in twitterLists)
                {
                    tle.CommandDeleteGroups = TOBCommands.DeleteGroupCommand;
                    tle.CommandGroups = TOBCommands.GroupsCommand;

                    TwitterListExtended existingTLE = _twitterListExt.Where(t => t.Id == tle.Id).FirstOrDefault();

                    if (existingTLE != null)
                    {
                        _twitterListExt[_twitterListExt.IndexOf(existingTLE)] = tle;
                    }
                    else
                    {
                        _twitterListExt.Add(tle);
                    }
                }
            };
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }
               

        public PanelList CurrentPanelList
        {
            get
            {
                return _panelList;
            }
        }
               
        public void SelectPanel()
        {
            _panelList.SelectPanel();
        }

        public Panel SelectedPanel
        {
            get
            {
                return _panelList.Where(p => p.IsSelected == true).FirstOrDefault();
            }
        }

        private void userAccountInfo_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    panelSelectorControl.DataContext = _panelList;
                    BindUserAccounts();
                    BindGroups();
                    BindPluginPanels();
                    BindSavedFilters();
                    BindSavedSearch();
                    BindAccounts();
                    BindPluginStreams();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void BindPluginPanels()
        {            
            List<SavedPluginView> savedPluginViewList = LocalSavedPluginViewBO.GetAll();
            foreach (SavedPluginView savedPluginView in savedPluginViewList)
            {
               // savedPluginView.PluginStream = "[" + savedPluginView.PluginStream + "]";
                savedPluginView.Command = TOBCommands.PluginCommand;
                savedPluginView.DeletePluginCommand = TOBCommands.DeletePluginCommand;
            }
            listPluginPanels.DataContext = savedPluginViewList;
            //listPluginPanels.SelectedIndex = 0;
        }

        private void BindPluginStreams()
        {
            List<TOBEntityBase> tobEntitybaseList = new List<TOBEntityBase>();
            //Panel Types
            List<PanelType> panelTypeList = LocalPanelTypeBO.GetAll();
            tobEntitybaseList.AddRange(panelTypeList.Cast<TOBEntityBase>());            
            //Saved Filters
            List<SavedFilter> savedFilterList = LocalSavedFilterBO.GetAll();
            tobEntitybaseList.AddRange(savedFilterList.Cast<TOBEntityBase>());
            //Saved Search.
            //List<SavedSearch> savedSearchList = LocalSavedSearchBO.GetAll();
            //tobEntitybaseList.AddRange(savedSearchList.Cast<TOBEntityBase>());

            cmbPluginStream.ItemsSource = tobEntitybaseList;
            cmbPluginStream.SelectedIndex = 0;
        }

        private void BindPlugins()
        {
            List<KeyValuePair<TOBPluginInfo, Type>> pluginInfos;
            pluginInfos = PluginManager.Current.GetPluginInfos();
            listBoxPlugin.ItemsSource = pluginInfos.Select(pi => pi.Key);
            listBoxPlugin.SelectedIndex = 0;
        }
     
        //void listPluginPanels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //TEMP HACKED CODE. NEEDS TO BE FIXED WITH PROPER HANDLING
        //    if (e.AddedItems.Count > 0)
        //    {
        //        Plugin plugin = listPluginPanels.SelectedItem as Plugin;
        //        StatusPanelController.Current.LoadPluginPanelView(plugin);

               
        //    }
        //}
       
        void BindUserAccounts()
        {
            var tob = AccountManager.Instance.GetCurrentAccounts();
            Action action = delegate()
            {
                if (tob != null)
                {
                    var localUserProfiles = LocalAccountBO.GetAll();
                    _accountInfoList = new ObservableCollection<AccountInformation>();
                    
                    foreach (Account acc in localUserProfiles)
                    {
                        var tup = LocalUserProfileBO.Get(s => s.Account == acc && s.ScreenName == acc.Username);

                        if (tup != null)
                        {
                            AccountInformation accountInfo = new AccountInformation();
                            accountInfo.UserName = tup.ScreenName;
                            accountInfo.FollowersCount = (int)tup.FollowersCount;
                            accountInfo.FriendsCount = (int)tup.FriendsCount;
                            accountInfo.TweetsCount = (int)tup.StatusesCount;
                            accountInfo.AccountID = (int)tup.Id;
                            _accountInfoList.Add(accountInfo);
                        }
                    }
                    listAccounts.ItemsSource = _accountInfoList;                    
                }
            };
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }

        public void BindGroups()
        {
            List<Account> accountList = AccountManager.Instance.GetCurrentAccounts();

            _twitterListExt = new ObservableCollection<TwitterListExtended>();
            
            foreach (Account acc in accountList)
            {                
                TOBBaseObject Twitter = AccountManager.Instance.GetTOBObject(acc);
                
                List<TwitterListExtended> twitterLists = Twitter.GetLists();

                foreach (TwitterListExtended tle in twitterLists)
                {
                    tle.CommandDeleteGroups = TOBCommands.DeleteGroupCommand;
                    tle.CommandGroups = TOBCommands.GroupsCommand;

                    _twitterListExt.Add(tle);
                }
            }

            listGroupsNames.ItemsSource = _twitterListExt;
        }

        
        public void DeleteLists(TwitterListExtended tListExt)
        {
            _twitterListExt.Remove(tListExt);
            listGroupsNames.ItemsSource = _twitterListExt;
        }

        public void BindSavedFilters()
        {
            _savedFilerList = new ObservableCollection<SavedFilter>(LocalSavedFilterBO.GetAll());
            foreach (SavedFilter filter in _savedFilerList)
            {
                filter.CommandFilter = TOBCommands.FilterCommand;
                filter.CommandDeleteFilter = TOBCommands.DeleteFilterCommand;
                List<Account> accList = filter.AccountFilterMappings.Select(ac => ac.Account).ToList();
                filter.PanelTypeText = "[" + ((TOBEntityEnum)filter.PanelTypeId).ToString() + "]";
                StringBuilder accName = new StringBuilder();
                foreach (Account acc in accList)
                {
                    accName.Append(acc.Username +" ");
                }
                filter.AccountNames = accName.ToString();
                
            }
            listSavedFilter.ItemsSource = _savedFilerList;
        }

        void BindSavedSearch()
        {
            _savedSearchList = new ObservableCollection<SavedSearch>(LocalSavedSearchBO.GetAll());
            foreach (SavedSearch search in _savedSearchList)
            {
                //search.SearchText = "(" + search.SearchText + ")";
                search.CommandSearch = TOBCommands.SearchCommand;
                search.CommandSearchDelete = TOBCommands.DeleteSearchCommand;
            }
            listSavedSearch.ItemsSource = _savedSearchList;
        }

        void BindAccounts()
        {
            cmbAccount.ItemsSource = AccountManager.Instance.GetCurrentAccounts();
            cmbAccount.SelectedIndex = 0;
        }

        void AddGroupMembers(UserProfile UserProfile)
        {
            _userProfileGroupList.Add(UserProfile);
            listGroupMembers.ItemsSource = _userProfileGroupList;
        }

        void AddFollowingList(UserProfile tup)
        {
            _userProfileList.Add(tup);
            listGroupsFollowing.ItemsSource = _userProfileList;
        }

        public void RemoveFollowing(UserProfile tup)
        {
            _userProfileList.Remove(tup);
            listGroupsFollowing.ItemsSource = _userProfileList;
        }

        void RemoveGroupMember(UserProfile tup)
        {
            _userProfileGroupList.Remove(tup);
            listGroupMembers.ItemsSource = _userProfileGroupList;
        }

        public void AddFilter(SavedFilter savedFilter)
        {
            if (savedFilter != null)
            {
                _savedFilerList.Add(savedFilter);
                savedFilter.CommandFilter = TOBCommands.FilterCommand;
                savedFilter.CommandDeleteFilter = TOBCommands.DeleteFilterCommand;
            }
        }

        public void AddSearch(SavedSearch savedSearch)
        {
            _savedSearchList.Add(savedSearch);
            savedSearch.CommandSearch = TOBCommands.SearchCommand;
        }

        public void DeleteSearch(SavedSearch savedSearch)
        {
            _savedSearchList.Remove(savedSearch);
        }
               

        private void btnAddGroups_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtGroupName.Visibility = Visibility.Visible;
            tbtblstNm.Visibility = Visibility.Visible;
            btnGroupSave.Content = "Save";
            cmbAccount.IsEnabled = true;
            cmbAccount.ItemsSource = null;
            cmbAccount.ItemsSource = AccountManager.Instance.GetCurrentAccounts();
            cmbAccount.SelectedIndex = 0;
            txtGroupName.Text = "";
           // listGroupsFollowing.ItemsSource = null;
            listGroupMembers.ItemsSource = null;
            twitterListPopUp.IsOpen = true;
            _userProfileGroupList = new ObservableCollection<UserProfile>();
            tbListId.Text = "";
        }

        private void btnGroupSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                //Updated at Twitter.
                Account account = LocalAccountBO.Get(ac => ac.Id == Convert.ToInt32(cmbAccount.SelectedValue));
                TOBBaseObject Twitter = AccountManager.Instance.GetTOBObject(account);
                TwitterList twitterList = null;
                if (btnGroupSave.Content.ToString() == "Save")
                {                    
                    if (radioPublic.IsChecked == true)
                    {
                        twitterList = Twitter.CreatePublicLists(cmbAccount.Text, txtGroupName.Text);
                    }
                    else
                    {
                        twitterList = Twitter.CreatePrivateLists(cmbAccount.Text, txtGroupName.Text);
                    }                   
                    if (txtGroupName.Text != "")
                    {                  
                        if (twitterList != null)
                        {
                            foreach (UserProfile tup in _userProfileGroupList)
                            {
                                Twitter.AddMemberToList(account.Username, twitterList.Id, (long)tup.UserId);
                            }                           
                        }
                    }
                }
                else
                {
                    foreach (UserProfile tup in _userProfileGroupList)
                    {
                        Twitter.AddMemberToList(account.Username, Int64.Parse(tbListId.Text), (long)tup.UserId);
                    }                   
                   
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                txtGroupName.Text = "";
                twitterListPopUp.IsOpen = false;
                BindGroups();                
            }
        }

        private void btnGroupCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            cmbAccount.ItemsSource = null;
            cmbAccount.ItemsSource = AccountManager.Instance.GetCurrentAccounts();
            cmbAccount.SelectedIndex = 0;
            listGroupMembers.ItemsSource = null;
            listGroupsFollowing.ItemsSource = null;
            twitterListPopUp.IsOpen = false;
        }

        private void btnFollowres_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                tbFollowText.Text = "Followers";
                var dc = (sender as Button).DataContext;
                AccountInformation userprofile = dc as AccountInformation;
                listFollowers.ItemsSource = LocalUserProfileBO.GetAll().Where(tup => tup.IsFollower == true && tup.AccountId == userprofile.AccountID).ToList();
                PopupFollowes.IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnFollowClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PopupFollowes.IsOpen = false;
        }

        private void btnFollowing_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                tbFollowText.Text = "Following";
                var dc = (sender as Button).DataContext;
                AccountInformation userprofile = dc as AccountInformation;
                listFollowers.ItemsSource = LocalUserProfileBO.GetAll().Where(tup => tup.IsFollowing == true && tup.AccountId == userprofile.AccountID).ToList();
                PopupFollowes.IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
        private void cmbAccount_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int accountId = Convert.ToInt32(cmbAccount.SelectedValue);
            _userProfileList = new ObservableCollection<UserProfile>(LocalUserProfileBO.GetAll().Where(tup => tup.IsFollowing == true && tup.AccountId == accountId).ToList());
           
            listGroupsFollowing.ItemsSource=_userProfileList;
        }

        private void btnAddToGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var tup = (sender as Button).DataContext;
            UserProfile UserProfile = tup as UserProfile;           
            AddGroupMembers(UserProfile);
            RemoveFollowing(UserProfile);
        }

        private void btnRemoveFromGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UserProfile tup = ((sender as Button).DataContext) as UserProfile;
            if (tup.IsFollowing == true)
            {
                RemoveGroupMember(tup);
                AddFollowingList(tup);
                RemoveMemberFromList(tup);
            }
            else
            {
                RemoveMemberFromList(tup);
                RemoveGroupMember(tup);
            }          
        }

        private void RemoveMemberFromList(UserProfile tup)
        {
            if (tbListId.Text != "")
            {
                Account account = LocalAccountBO.Get(ac => ac.Username == cmbAccount.Text);
                TOBBaseObject Twitter = AccountManager.Instance.GetTOBObject(account);
                long listId = Int64.Parse(tbListId.Text);
                Twitter.RemoveMemberFromList(account.Username, listId, (long)tup.UserId);
            }
        }

        private void txtFollowingFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            FilterFollowing(txtFollowingFilter.Text);
        }


        public void FilterFollowing(string filterText)
        {
            listGroupsFollowing.Items.Filter = delegate(object obj)
            {
                TOBEntityBase entityBase = obj as TOBEntityBase;
                filterText = filterText.ToLower();
                if (entityBase != null && !string.IsNullOrEmpty(entityBase.SearchableString))
                {
                    if (entityBase.SearchableString.Contains(filterText))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            };
        }

        private void btnFilterFollowing_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	txtFollowingFilter.Text ="";
        }

        public void GroupsPopup(Status status)
        {
            PopupGroups.IsOpen = true;            
            listboxGroupsPopup.ItemsSource = _twitterListExt;
            if (_twitterListExt.Count < 1)
            {
                tbGroupsError.Visibility = Visibility.Visible;
            }
            else
            {
                tbGroupsError.Visibility = Visibility.Collapsed;
            }
            _status = status;
        }

        public void ListUserRemovePopUp(Status status)
        {
            PopupRemoveUserfromList.IsOpen = true;
            listBoxRemoveUserList.ItemsSource = _twitterListExt;
            _status = status;
        }

        private void btnAddToGroupCanel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PopupGroups.IsOpen = false;
        }

        private void btnAddUser_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                TwitterListExtended twitterListExt = (sender as Button).DataContext as TwitterListExtended;
                if (twitterListExt != null)
                {
                    Account acc = LocalAccountBO.Get(ac => ac.Username == twitterListExt.User.ScreenName);
                    TOBBaseObject Twitter = AccountManager.Instance.GetTOBObject(acc);
                    bool IsMember = Twitter.IsMemberOfList(acc.Username, twitterListExt.Id, (long)_status.TwitterUserId);
                    if (IsMember)
                    {
                        MessageBox.Show("Already a member");
                    }
                    else
                    {
                        Twitter.AddMemberToList(acc.Username, twitterListExt.Id, (long)_status.TwitterUserId);
                        TOB.Utility.MessageNotifier.Instance.NotifyMessage(_status.UserProfile.ScreenName + " is added to list");
                    }                  
                }
                
                PopupGroups.IsOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEditGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                TwitterListExtended list = (sender as Button).DataContext as TwitterListExtended;
                if (list != null)
                {
                    btnGroupSave.Content = "Update";
                    cmbAccount.ItemsSource = AccountManager.Instance.GetCurrentAccounts();
                    cmbAccount.SelectedItem = AccountManager.Instance.GetCurrentAccounts().Where(a => a.Username == list.User.ScreenName).FirstOrDefault();
                    cmbAccount.IsEnabled = false;
                    txtGroupName.Visibility = Visibility.Collapsed;
                    tbtblstNm.Visibility = Visibility.Collapsed;
                    tbListId.Text = list.Id.ToString();
                    tbTwitterListName.Text = list.FullName;
                    Account account = LocalAccountBO.Get(ac => ac.Username == list.User.ScreenName);
                    TOBBaseObject Twitter = AccountManager.Instance.GetTOBObject(account);
                    _userProfileGroupList = new ObservableCollection<UserProfile>(Twitter.GetMembersFromList(list.User.ScreenName, (long)list.Id).ToList());

                    listGroupMembers.ItemsSource = _userProfileGroupList;
                    listGroupsFollowing.ItemsSource = _userProfileList;                   
                    twitterListPopUp.IsOpen = true;                
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGpClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            twitterListPopUp.IsOpen = false;
        }

        private void btnAddPluginPanel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BindPlugins();
            BindPluginStreams();
            PluginPopup.IsOpen = true;
        }

        private void btnPluginClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	PluginPopup.IsOpen = false;
        }

        private void btnRemoveUser_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TwitterListExtended twitterListExt = (sender as Button).DataContext as TwitterListExtended;
            if (twitterListExt != null)
            {
                Account acc = LocalAccountBO.Get(ac => ac.Username == twitterListExt.User.ScreenName);
                TOBBaseObject Twitter = AccountManager.Instance.GetTOBObject(acc);
                Twitter.RemoveMemberFromList(acc.Username, twitterListExt.Id, (long)_status.TwitterUserId);
                PopupRemoveUserfromList.IsOpen = false;
            }
        }

        private void btnPluginCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	PluginPopup.IsOpen =false;
        }

        private void btnPluginSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (listBoxPlugin.SelectedIndex == -1)
                {
                    return;
                }
                TOBPluginInfo pluginInfos = listBoxPlugin.SelectedItem as TOBPluginInfo;
                SavedPluginView savedPluginView = new SavedPluginView();
                
                TOBEntityBase tob = cmbPluginStream.SelectedItem as TOBEntityBase;
                
                if (tob.GetType().Name == PluginTypeEnum.PanelType.ToString())
                {
                    savedPluginView.PluginStreamType = 0;
                }
                else if (tob.GetType().Name == PluginTypeEnum.SavedSearch.ToString())
                {
                    savedPluginView.PluginStreamType = 1;
                }
                else if (tob.GetType().Name == PluginTypeEnum.SavedFilter.ToString())
                {
                    savedPluginView.PluginStreamType = 2;
                }
                savedPluginView.PluginName = pluginInfos.PluginName;         
                savedPluginView.PluginId = pluginInfos.PluginGUID;
                savedPluginView.PluginStream = cmbPluginStream.Text;
                LocalSavedPluginViewBO.Insert(savedPluginView);
                LocalSavedPluginViewBO.SaveChanges();

                PluginPopup.IsOpen = false;
                BindPluginPanels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listSavedFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        	// Implementing unselection of the listboxes, It is a single selectionchanged event for filter, list, search,
            // and plugin.
            ListBox l = sender as ListBox;
            if (l.SelectedIndex != -1)
            {
                foreach (object list in _listBoxList)
                {
                    if (sender != list)
                    {
                        ListBox listBox = list as ListBox;
                        listBox.SelectedIndex = -1;
                    }
                }
            }
        }

        public void UnSelectListBoxes()
        {
            //This function is for unselect other listboxes from the panel list.
            foreach (object list in _listBoxList)
            {
                ListBox listBox = list as ListBox;
                listBox.SelectedIndex = -1;
            }
        }

        private void toggleButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            if (sender is ToggleButton)
            {
                ToggleButton chk = sender as ToggleButton;
                chk.IsChecked = true;
            }
        }

        public void SelectHomePanel()
        {
            panelSelectorControl.SelectHome();
        }
    }
}

