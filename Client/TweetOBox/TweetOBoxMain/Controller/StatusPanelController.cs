using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TweetOBoxMain.UserControls;
using TweetOBoxMain.Model;
using TOB.Entities;
using TOB.BLL;
using System.Windows;
using System.Collections.ObjectModel;
using TOB.TweetSharpWrap;
using System.Windows.Threading;
using TweetOBoxMain.Utility;
using System.ComponentModel;
using TOB.Plugin;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Xml;
using TweetOBoxMain.Notifications;
using System.Windows.Controls.Primitives;
using System.Threading;
using TweetSharp;

namespace TweetOBoxMain.Controller
{
    public class StatusPanelController
    {
        //TOB CollectionFilter linq expression . TODO - Merge into an enum collection or something
        private Func<TOBEntityBase, bool> ReplyCollectionFilter = (s => (s as Status).Text.StartsWith("@"));
        private Func<TOBEntityBase, bool> ReTweetCollectionFilter = (s => (s as Status).Text.Contains("RT "));
        private Func<TOBEntityBase, bool> FavoriteCollectionFilter = (s => (s as Status).IsFavorited == true);

        static StatusPanelController _instance = null;
        TOBMain _mainWindow;
        UserAllTweets _currentTweetsView;
        UserProfileDetails _userProfileView;        
        Collection<TOBEntityBase> _statusCollection;
        Collection<TOBEntityBase> _directMessagesCollection;
        UserTweets _tweetSender;
        AccountInfo _accountInfo;
        Panel _currentPanel;
        //lstPanelSelectorControl _lstPanelSelector;
        Dictionary<String, UserAllTweets> _panelViewDict = new Dictionary<string, UserAllTweets>();

        private int MAX_COLLECTION_OBJECTS = 700;

        public static StatusPanelController Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StatusPanelController((TOBMain) App.Current.MainWindow);
                }

                return _instance;
            }
        }
        private SavedFilterBO _savedFilterBO = null;
        private SavedPluginViewBO _savedPluginViewBO = null;
        private SavedSearchBO _savedSearchBO = null;
        private AccountBO _accountBO = null;
        private UserProfileBO _UserProfile = null;

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

        private UserProfileBO LocalUserProfileBO
        {
            get
            {
                if (_UserProfile == null)
                {
                    _UserProfile = new UserProfileBO();
                }
                return _UserProfile;
            }
        }

        public StatusPanelController(TOBMain mainWindow)
        {
            //Initialize views
            _mainWindow = mainWindow;
            _userProfileView = mainWindow.userProfileTOB;
            _accountInfo = mainWindow.accountInfo;
            _tweetSender = mainWindow.userTweets;

            List<Status> statuses = new StatusBO().GetLimitedListBySorting((t => t.AccountId != 0), (t => t.TwitterCreatedDate), System.Data.SqlClient.SortOrder.Descending, MAX_COLLECTION_OBJECTS);
            List<TOBEntityBase> baseEntities = statuses.ConvertAll<TOBEntityBase>(s => s as TOBEntityBase);
            _statusCollection = new Collection<TOBEntityBase>(baseEntities);

            List<DirectMessage> dms = new DirectMessageBO().GetLimitedListBySorting((t => t.AccountId != 0 && t.Recieved == true),(t => t.TwitterCreatedDate), System.Data.SqlClient.SortOrder.Descending, MAX_COLLECTION_OBJECTS);
            List<TOBEntityBase> baseDmsEntities = dms.ConvertAll<TOBEntityBase>(s => s as TOBEntityBase);
            _directMessagesCollection = new Collection<TOBEntityBase>(baseDmsEntities);

            foreach (TOBBaseObject tobbaseobject in AccountManager.Instance.TobObjects)
            {
                tobbaseobject.NewStatus += new TOBBaseObject.StatusHandler(tobbaseobject_NewStatus);
            }

            PanelList pl = _mainWindow.accountInfo.CurrentPanelList as PanelList;
            LoadReplyView(pl.Where(s => s.PanelName == TOBEntityEnum.Replies).FirstOrDefault());
            LoadReTweetView(pl.Where(s => s.PanelName == TOBEntityEnum.ReTweet).FirstOrDefault());
            LoadMarkasFavoriteView(pl.Where(s => s.PanelName == TOBEntityEnum.Favorite).FirstOrDefault());
            LoadDirectMessageView(pl.Where(s => s.PanelName == TOBEntityEnum.DirectMessages).FirstOrDefault());
            LoadHomeView(pl.Where(s => s.PanelName == TOBEntityEnum.Home).FirstOrDefault());
            
        }
        void tobbaseobject_NewStatus(TOBBaseObject from, TOBEventArgs args)
        {
            //TOB.Logger.TOBLogger.WriteInfo("NewStatus - " + args.EntityList.Count);

            //Update internal Collection (Might not be needed at all)
            foreach (TOBEntityBase item in args.EntityList)
            {
                if (args.TweetEnum == TOBEntityEnum.Status)
                {
                    _statusCollection.Insert(0, item);
                }
                else if (args.TweetEnum == TOBEntityEnum.DirectMessages)
                {
                    _directMessagesCollection.Insert(0, item);
                }
            }

            //Flush out old objects in _statusCollection
            if (_statusCollection.Count > (int)(MAX_COLLECTION_OBJECTS * 1.1))
            {
                while (_statusCollection.Count > MAX_COLLECTION_OBJECTS)
                {
                    _statusCollection.RemoveAt(_statusCollection.Count - 1);
                }
            }
            //Flush out old objects in _directMessageCollection
            if (_directMessagesCollection.Count > (int)(MAX_COLLECTION_OBJECTS * 1.1))
            {
                while (_directMessagesCollection.Count > MAX_COLLECTION_OBJECTS)
                {
                    _directMessagesCollection.RemoveAt(_directMessagesCollection.Count - 1);
                }
            }

            if (_panelViewDict.Values.Count == 0)
            {
                TOB.Logger.TOBLogger.WriteDebugInfo("ERROR - PanelViewDict Values are 0 ... Potential dataloss can occur");
            }

            foreach (KeyValuePair<string,UserAllTweets> catchedPanelPair in _panelViewDict)
            {
              //  cachedPanel.AddList(args.EntityList,_currentPanel.AccountList);
                UserAllTweets cachedPanel = catchedPanelPair.Value;
                if (catchedPanelPair.Key == _currentPanel.PanelName.ToString())
                {
                    cachedPanel.AddList(args.EntityList, _currentPanel.AccountList);
                }
                else
                {
                    cachedPanel.AddList(args.EntityList, null);
                }

            }
            
        }
               
        public void ShowUserProfile(TOBEntityBase tobEntityBase)
        {
            try
            {
                if (tobEntityBase is Status)
                {
                    Status status = tobEntityBase as Status;
                    SetUserProfile(status.UserProfile);                 
                }
                else if (tobEntityBase is DirectMessage)
                {
                    DirectMessage directMessage = tobEntityBase as DirectMessage;
                    SetUserProfile(directMessage.UserProfile);
                }
             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ShowUserProfileForTag(string userName)
        {
            try
            {
                userName = userName.Substring(1);

                UserProfile UserProfile = null;

                foreach (TOBBaseObject tobObj in AccountManager.Instance.TobObjects)
                {
                    UserProfile = tobObj.GetUserProfile(userName);
                    
                    if (UserProfile != null)
                    {
                        break;
                    }
                }

                if (UserProfile == null)
                {
                    MessageBox.Show("No user found", "Not found");
                    return;
                }

                SetUserProfile(UserProfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetUserProfile(UserProfile tup)
        {
            UserAllTweets profileView;
            
            if (_panelViewDict.ContainsKey("ProfileView"))
            {
                profileView = _panelViewDict["ProfileView"];
            }
            else
            {
                profileView = new UserAllTweets();
                profileView.DataTemplate = profileView.FindResource("SearchUsersListDataTemplate") as DataTemplate;
                _panelViewDict.Add("ProfileView", profileView);
            }
            
            //SortableObservableCollection<TOBEntityBase> collection = new SortableObservableCollection<TOBEntityBase>(_statusCollection.Where(s => (s as Status).UserProfile.ScreenName == tup.ScreenName).Cast<TOBEntityBase>());
            //profileView.Collection = collection;
            
            profileView.CollectionFilter = s => (s as Status).UserProfile.ScreenName == tup.ScreenName;
            profileView.Collection = _statusCollection;
            _mainWindow.frmTOBMain.Content = profileView;
            _tweetSender.UserSearch(tup);
            _userProfileView.SetUserDetails(tup);
            _userProfileView.Visibility = Visibility.Visible;
            _currentTweetsView = profileView;
            
        }

        public void ShowListUserProfile(TwitterStatus twitterStatus)
        {
            try
            {
                UserProfile UserProfile = null;

                foreach (TOBBaseObject tobObj in AccountManager.Instance.TobObjects)
                {
                    UserProfile = tobObj.GetUserProfile(twitterStatus.User.ScreenName);
                    if (UserProfile != null)
                    {
                        break;
                    }
                }

                if (UserProfile == null)
                {
                    MessageBox.Show("No user found", "Not found");
                    return;
                }

                SetUserProfile(UserProfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //public void LoadDefaultView(Panel info)
        //{
        //    SortableObservableCollection<TOBEntityBase> collection;
        //    if (info != null)
        //    {
                //List<int> accList = info.AccountList.Where(a => a.IsSelected == true).Select(a => a.Id).ToList();
                //collection = new SortableObservableCollection<TOBEntityBase>(_statusCollection.Where(s => accList.Contains((int)(s as TOBEntityBase).AccountId)));
        //    }
        //    else
        //    {
        //        collection = new SortableObservableCollection<TOBEntityBase>(_statusCollection);
        //    }
        //    _currentPanel = info;
        //    LoadDefaultView(collection);

        //}


        //private void LoadDefaultView(SortableObservableCollection<TOBEntityBase> collection)
        //{
        //    UserAllTweets allView;
        //    if (_panelViewDict.ContainsKey("Home"))
        //    {
        //        allView = _panelViewDict["Home"];
        //    }
        //    else
        //    {
        //        allView = new UserAllTweets();
        //        //allView.DataTemplate = allView.FindResource("TweetsControlsDataTemplate") as DataTemplate;
        //        _panelViewDict.Add("Home", allView);
        //    }
        //    allView.Collection = collection;
        //    _currentTweetsView = allView;
        //    _mainWindow.frmTOBMain.Content = allView;
        //    _userProfileView.Visibility = Visibility.Collapsed;

        //}

        public void LoadHomeView(Panel info)
        {
            
            _accountInfo.UnSelectListBoxes();
            UserAllTweets allView;
            _accountInfo.SelectHomePanel();
            if (_panelViewDict.ContainsKey(TOBEntityEnum.Home.ToString()))
            {
                allView = _panelViewDict[TOBEntityEnum.Home.ToString()];
                if (info != null)
                {
                    List<int> accList = info.AccountList.Where(a => a.IsSelected == true).Select(a => a.Id).ToList();
                    if (info.AccountList.Count > 1)
                    {
                        List<TOBEntityBase> homeCollection = new List<TOBEntityBase>();
                        homeCollection.AddRange(_statusCollection.Where(s => accList.Contains(s.AccountsId)));
                        homeCollection.AddRange(_directMessagesCollection.Where(d => accList.Contains(d.AccountsId)));
                        allView.Collection = new Collection<TOBEntityBase>(homeCollection);
                    }
                }
            }
            else
            {
                allView = new UserAllTweets();
                //Moving it higher to protect against a timing issue where _panelViewDict.Values is empty on NewStatus
                _panelViewDict.Add(TOBEntityEnum.Home.ToString(), allView);
               // allView.DataTemplate = allView.FindResource("TweetsControlsDataTemplate") as DataTemplate;
                allView.NavBindingPanel = info;
                //allView.CollectionTypeFilter = TOBEntityEnum.None;
                List<TOBEntityBase> homeColl = new List<TOBEntityBase>();
                homeColl.AddRange(_statusCollection);
                homeColl.AddRange(_directMessagesCollection);

                allView.Collection = new Collection<TOBEntityBase>(homeColl);
            }
            _currentPanel = info;
            _currentTweetsView = allView;
            _mainWindow.frmTOBMain.Content = allView;
            _userProfileView.Visibility = Visibility.Collapsed;
            _tweetSender.txtSearchBox.Text = "";
        }

        public void LoadReplyView(Panel info)
        {
            if (info == null)
            {
                return;
            }
            _currentPanel = info;
            _accountInfo.UnSelectListBoxes();
            UserAllTweets replyView;
            
            if (_panelViewDict.ContainsKey(TOBEntityEnum.Replies.ToString()))
            {
                replyView = _panelViewDict[TOBEntityEnum.Replies.ToString()];
                if (info != null)
                {
                    List<int> accList = info.AccountList.Where(a => a.IsSelected == true).Select(a => a.Id).ToList();
                    if (info.AccountList.Count > 1)
                    {
                        replyView.Collection = new SortableObservableCollection<TOBEntityBase>(_statusCollection.Where(s => accList.Contains((int)(s as Status).AccountId)));
                    }
                }
            }
            else
            {
                replyView = new UserAllTweets();
                replyView.CollectionFilter = ReplyCollectionFilter;
                replyView.NavBindingPanel = info;
                replyView.Collection = _statusCollection;
                _panelViewDict.Add(TOBEntityEnum.Replies.ToString(), replyView);
            }
            _currentTweetsView = replyView;
            _mainWindow.frmTOBMain.Content = replyView;
            _userProfileView.Visibility = Visibility.Collapsed;
        }

        public void LoadReTweetView(Panel info)
        {
            if (info == null)
            {
                return;
            }
            
            _currentPanel = info;
            _accountInfo.UnSelectListBoxes();
            UserAllTweets retweetView;
            if (_panelViewDict.ContainsKey(TOBEntityEnum.ReTweet.ToString()))
            {
                retweetView = _panelViewDict[TOBEntityEnum.ReTweet.ToString()];
                if (info != null)
                {
                    List<int> accList = info.AccountList.Where(a => a.IsSelected == true).Select(a => a.Id).ToList();
                    if (info.AccountList.Count > 1)
                    {
                        retweetView.Collection = new SortableObservableCollection<TOBEntityBase>(_statusCollection.Where(s => accList.Contains((int)(s as Status).AccountId)));
                    }                   
                }
            }
            else
            {
                retweetView = new UserAllTweets();
                //NEEDS TO BE FIXED TO USE TWITTER BASED RT
                retweetView.CollectionFilter = ReTweetCollectionFilter; 
                retweetView.NavBindingPanel = info;
                retweetView.Collection = _statusCollection;
                _panelViewDict.Add(TOBEntityEnum.ReTweet.ToString(), retweetView);
            }
            _currentTweetsView = retweetView;
            _mainWindow.frmTOBMain.Content = retweetView;
            _userProfileView.Visibility = Visibility.Collapsed;
        }

        public void LoadMarkasFavoriteView(Panel info)
        {
            if (info == null)
            {
                return;
            }
            
            _currentPanel = info;

            UserAllTweets markasFavoriteView;
            if (_panelViewDict.ContainsKey(TOBEntityEnum.Favorite.ToString()))
            {
                markasFavoriteView = _panelViewDict[TOBEntityEnum.Favorite.ToString()];
            }
            else
            {
                markasFavoriteView = new UserAllTweets();
             //   markasFavoriteView.DataTemplate = markasFavoriteView.FindResource("TweetsControlsDataTemplate") as DataTemplate;
                markasFavoriteView.NavBindingPanel = info;
                markasFavoriteView.CollectionFilter = FavoriteCollectionFilter;
                markasFavoriteView.Collection = _statusCollection;
                _panelViewDict.Add(TOBEntityEnum.Favorite.ToString(), markasFavoriteView);
            }
            _currentTweetsView = markasFavoriteView;
            _mainWindow.frmTOBMain.Content = markasFavoriteView;
            _userProfileView.Visibility = Visibility.Collapsed;
        }

        public void LoadDirectMessageView(Panel info)
        {
            if (info == null)
            {
                return;
            }
            
            _currentPanel = info;
            _accountInfo.UnSelectListBoxes();
            UserAllTweets directMessageView;
            if (_panelViewDict.ContainsKey(TOBEntityEnum.DirectMessages.ToString()))
            {
                directMessageView = _panelViewDict[TOBEntityEnum.DirectMessages.ToString()];
                List<int> accList = info.AccountList.Where(a => a.IsSelected == true).Select(a => a.Id).ToList();
                if (info.AccountList.Count > 1)
                {
                    directMessageView.Collection = new SortableObservableCollection<TOBEntityBase>(_directMessagesCollection.Where(dm => accList.Contains(dm.AccountsId)).ToList());
                }
            }
            else
            {
                directMessageView = new UserAllTweets();
                directMessageView.DataTemplate = directMessageView.FindResource("DirectMessageDataTemplate") as DataTemplate;
                directMessageView.CollectionTypeFilter = TOBEntityEnum.DirectMessages;
                directMessageView.NavBindingPanel = info;
                directMessageView.Collection = _directMessagesCollection;
                _panelViewDict.Add(TOBEntityEnum.DirectMessages.ToString(), directMessageView);
            }
            _currentTweetsView = directMessageView;
            _mainWindow.frmTOBMain.Content = directMessageView;
            _userProfileView.Visibility = Visibility.Collapsed;
        }

        public void LoadPluginPanelView(SavedPluginView savedPluginView)
        {
            _accountInfo.SelectPanel();
            UserAllTweets pluginView = null;
            //PluginName and PluginStream are not unique. This is a bug. Once guids are implemented, only use guids with streamname.
            string plugin = "PLUGIN" + savedPluginView.PluginStream + savedPluginView.PluginId.ToString();
            if (_panelViewDict.ContainsKey(plugin))
            {
                pluginView = _panelViewDict[plugin];
            }
            else
            {
                //Plugin Impementation.
                List<KeyValuePair<TOBPluginInfo, Type>> pluginInfos = PluginManager.Current.GetPluginInfos();
                ITOBPlugin defaultPlugin;
                Type pluginType = pluginInfos.Where(kvpair => kvpair.Key.PluginGUID == savedPluginView.PluginId).FirstOrDefault().Value;

                if (pluginType == null)
                {
                    //NEEDS - Logging to TOBLogger and needs to contain more info on which plugin failed on which stream. 
                    MessageBox.Show("The plugin corresponding to this view is either deleted or corrupted. You can also delete this SavePlugin View by pressing the delete button.");
                    return;
                }

                defaultPlugin = PluginManager.Current.GetPluginObject(pluginType);
                pluginView = new UserAllTweets(defaultPlugin);

                if (pluginView != null)
                {
                    PluginTypeEnum pluginStreamType = (PluginTypeEnum)savedPluginView.PluginStreamType;
                    TOBEntityEnum panelEnum = TOBEntityEnum.None; 

                    switch (pluginStreamType)
                    {
                        case PluginTypeEnum.PanelType:
                            {
                                panelEnum = (TOBEntityEnum)Enum.Parse(typeof(TOBEntityEnum), savedPluginView.PluginStream, true);
                                break;
                            }
                        case PluginTypeEnum.SavedFilter:
                            {
                                string filterText = savedPluginView.PluginStream;
                                SavedFilter savedFilter = LocalSavedFilterBO.Get(sf => sf.FilterText == filterText);
                                
                                if (savedFilter != null)
                                {
                                    panelEnum = (TOBEntityEnum)savedFilter.PanelTypeId;
                                    pluginView.InternalStringFilter = filterText;
                                }
                                break;
                            }
                        case PluginTypeEnum.SavedSearch:
                            {
                                //IMPLEMENTATION TO BE COMPLETED IN FUTURE
                                //List<Search> searchList = GetSearchList(savedPluginView.PluginStream);
                                //pluginView.SearchCollection = new SortableObservableCollection<Search>(searchList);
                                break;
                            }
                    }

                    if (panelEnum != TOBEntityEnum.None)
                    {
                        SetMessageObjectPanelCollection(pluginView, panelEnum, null);
                    }
                    
                }
                _panelViewDict.Add(plugin, pluginView);
            }
            _currentTweetsView = pluginView;
            _mainWindow.frmTOBMain.Content = pluginView;
        }

        private void SetMessageObjectPanelCollection(UserAllTweets moPanel, TOBEntityEnum tweetEnum, List<int> accList)
        {
            Collection<TOBEntityBase> collection = null;

            switch (tweetEnum)
            {
                case TOBEntityEnum.Home:
                    {
                        break;
                    }
                case TOBEntityEnum.ReTweet:
                    {
                        moPanel.CollectionFilter = ReTweetCollectionFilter;
                        break;
                    }
                case TOBEntityEnum.DirectMessages:
                    {
                        if (accList != null)
                        {
                            collection = new Collection<TOBEntityBase>(_directMessagesCollection.Where(s => accList.Contains((int)(s as DirectMessage).AccountId)).ToList());
                        }
                        else
                        {
                            collection = new Collection<TOBEntityBase>(_directMessagesCollection);
                        }
                        
                        moPanel.CollectionTypeFilter = TOBEntityEnum.DirectMessages;

                        break;
                    }
                case TOBEntityEnum.Replies:
                    {
                        moPanel.CollectionFilter = ReplyCollectionFilter;
                        break;
                    }
                case TOBEntityEnum.Favorite:
                    {
                        moPanel.CollectionFilter = FavoriteCollectionFilter;
                        break;
                    }
            }

            if (collection == null)
            {
                if (accList != null)
                {
                    collection = new Collection<TOBEntityBase>(_statusCollection.Where(s => accList.Contains((int)(s as Status).AccountId)).ToList());
                }
                else
                {
                    collection = _statusCollection;
                }

                moPanel.Collection = collection;
            }
        }


        public void DeletePlugin(SavedPluginView savedPluginView)
        {
            MessageBoxResult messageResult;
            messageResult = MessageBox.Show("Are you sure you want to delete this plugin stream?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageResult == MessageBoxResult.Yes)
            {
                SavedPluginView savedPlugin = LocalSavedPluginViewBO.Get(pl => pl.Id == savedPluginView.Id);
                if (savedPlugin != null)
                {
                  LocalSavedPluginViewBO.Delete(savedPlugin);
                  LocalSavedPluginViewBO.SaveChanges();
                }
                string plugin = "PLUGIN" + savedPluginView.PluginStream + savedPluginView.PluginId.ToString();
                if (_panelViewDict.ContainsKey(plugin))
                {
                    _panelViewDict.Remove(plugin);
                }
            }
            _accountInfo.BindPluginPanels();
            Panel info = null; //Need to change this
            LoadHomeView(info);
        }

        public void SaveFilter()
        {
            SavedFilter savedFilter =  _currentTweetsView.SaveFilter(_currentPanel);
            _accountInfo.AddFilter(savedFilter);
        }

        public void LoadSavedFilterView(SavedFilter savedFilter)
        {           
            TOBEntityEnum panelCollectionType = (TOBEntityEnum)savedFilter.PanelTypeId;
            _accountInfo.SelectPanel();

            string savedfilterPanelKey = "SAVEDFILTERPANEL" + savedFilter.Id;
            UserAllTweets savedFilterView;
                
            if (_panelViewDict.ContainsKey(savedfilterPanelKey))
            {
                savedFilterView = _panelViewDict[savedfilterPanelKey];
            }
            else
            {
                savedFilterView = new UserAllTweets();
             //   savedFilterView.DataTemplate = savedFilterView.FindResource("TweetsControlsDataTemplate") as DataTemplate;

                SetMessageObjectPanelCollection(savedFilterView, panelCollectionType, savedFilter.AccountFilterMappings.Select(a => (int)a.AccountId).ToList());

                _panelViewDict.Add(savedfilterPanelKey, savedFilterView);
            }

            _currentTweetsView = savedFilterView;
            _mainWindow.frmTOBMain.Content = savedFilterView;
            _userProfileView.Visibility = Visibility.Collapsed;
            _currentTweetsView.SetFilterBoxText(savedFilter.FilterText);
            _tweetSender.txtSearchBox.Text = "";
        }
                
        public void SearchUserByUserName(string username)
        {    
            //Anand: Temp hack, need to fix asap.
            TwitterUser tup = null;
            List<TwitterSearchStatus> tupStatus = null;
            foreach (TOBBaseObject tobObj in AccountManager.Instance.TobObjects)
            {                
                tup = tobObj.SearchUser(username);                
                if (tup == null)
                {
                    continue;
                }
                tupStatus = tobObj.GetStatuses(username);
            }

            if (tup == null)
                return;

            _userProfileView.SetUserDetails(tup);
            _userProfileView.Visibility = Visibility.Visible;
            UserAllTweets savedsearchView;
            savedsearchView = new UserAllTweets();
            savedsearchView.DataTemplate = savedsearchView.FindResource("SavedSearchDataTemplate") as DataTemplate;
            SortableObservableCollection<TwitterSearchStatus> searchCollections;
            searchCollections = new SortableObservableCollection<TwitterSearchStatus>(tupStatus);
            savedsearchView.SearchCollection = searchCollections;
            _currentTweetsView = savedsearchView;
            _mainWindow.frmTOBMain.Content = savedsearchView;
        }

        public void LoadFindPeople(string query)
        {
            try
            {
                List<TwitterUser> users = null;

                foreach (TOBBaseObject tobObj in AccountManager.Instance.TobObjects)
                {
                    users = tobObj.QueryUsers(query);

                    if (users != null)
                    {
                        break;
                    }
                }

                if (users == null)
                {
                    MessageBox.Show("No users found");
                    return;
                }

                string queryUserPanelKey = "QUERYUSERPANEL";
                UserAllTweets queryUserView;

                if (_panelViewDict.ContainsKey(queryUserPanelKey))
                {
                    queryUserView = _panelViewDict[queryUserPanelKey];
                }
                else
                {
                    queryUserView = new UserAllTweets();
                    //queryUserView.DataTemplate = queryUserView.FindResource("QueryUserDataTemplate") as DataTemplate;

                    queryUserView.CollectionTypeFilter = TOBEntityEnum.None;

                    _panelViewDict.Add(queryUserPanelKey, queryUserView);
                }

                SortableObservableCollection<TwitterUser> peopleCollections;

                peopleCollections = new SortableObservableCollection<TwitterUser>(users);
                queryUserView.PeopleCollection = peopleCollections;

                _currentTweetsView = queryUserView;
                _mainWindow.frmTOBMain.Content = queryUserView;
                _tweetSender.ChangeGlobelSearchType(1);
                _tweetSender.txtSearchBox.Text = query;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadSavedSearchView(SavedSearch ssPanel)
        {
            _accountInfo.SelectPanel();           
            string savedsearchPanelKey = "SAVEDSEARCHPANEL" + ssPanel.Id;
            UserAllTweets savedsearchView;

            if (_panelViewDict.ContainsKey(savedsearchPanelKey))
            {
                savedsearchView = _panelViewDict[savedsearchPanelKey];
            }
            else
            {
                savedsearchView = new UserAllTweets();
                //savedsearchView.DataTemplate = savedsearchView.FindResource("SavedSearchDataTemplate") as DataTemplate;
                
                savedsearchView.CollectionTypeFilter = TOBEntityEnum.None;

                _panelViewDict.Add(savedsearchPanelKey, savedsearchView);
            }
            
            SortableObservableCollection<TwitterSearchStatus> searchCollections;
            searchCollections = new SortableObservableCollection<TwitterSearchStatus>(GetSearchList(ssPanel.SearchText));
            
            savedsearchView.SearchCollection = searchCollections;
            _currentTweetsView = savedsearchView;
            _mainWindow.frmTOBMain.Content = savedsearchView;
            _tweetSender.ChangeGlobelSearchType(-1);
            _tweetSender.txtSearchBox.Text = ssPanel.SearchText;           
            
        }

        private List<TwitterSearchStatus> GetSearchList(string searchText)
        {
            List<TwitterSearchStatus> results = null;

            try
            {
                foreach (TOBBaseObject tobObj in AccountManager.Instance.TobObjects)
                {
                    results = tobObj.QuerySearchResults(searchText);
                    
                    if (results != null)
                    {
                        break;
                    }
                }

                if (results == null)
                {
                    MessageBox.Show("No user found", "Not found");
                    results = new List<TwitterSearchStatus>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            return results;
        }

        public void DismissUIObject(object obj)
        {
            _currentTweetsView.RemoveUIObjectFromList(obj);
        }

        public void SaveSearch(string searchText)
        {
            //_accountInfo.SelectPanel();
            SavedSearch savedSearch = new SavedSearch();
            savedSearch.SearchText = searchText;
            LocalSavedSearchBO.Insert(savedSearch);
            LocalSavedSearchBO.SaveChanges();
            _accountInfo.AddSearch(savedSearch);
            _tweetSender.txtSearchBox.Text = "";
            Panel info = null; //Need to change this
            LoadHomeView(info);
        }

        public void DeleteSavedSearch(SavedSearch ssPanel)
        {
            MessageBoxResult messageResult;
            messageResult = MessageBox.Show("Are you sure you want to delete this saved search?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageResult == MessageBoxResult.Yes)
            {
                SavedSearch savedSearch = LocalSavedSearchBO.Get(search => search.Id == ssPanel.Id);
                if (savedSearch != null)
                {
                    LocalSavedSearchBO.Delete(savedSearch);
                    LocalSavedSearchBO.SaveChanges();
                    _accountInfo.DeleteSearch(savedSearch);
                }
            }
            _tweetSender.txtSearchBox.Text = "";
            Panel info = null; //Need to change this
            LoadHomeView(info);
        }

        public void DeleteSavedFilter(SavedFilter saveFilter)
        {
            MessageBoxResult messageResult;
            messageResult = MessageBox.Show("Are you sure you want to delete this saved filter?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageResult == MessageBoxResult.Yes)
            {
                SavedFilter savedFilter = LocalSavedFilterBO.Get(filter => filter.Id == saveFilter.Id);
                if (savedFilter != null)
                {
                    AccountFilterMappingBO accountMappingBO = new AccountFilterMappingBO();
                    List<AccountFilterMapping> accountMappingList = accountMappingBO.GetAll().Where(map => map.FilterId == saveFilter.Id).ToList();
                    if (accountMappingList != null)
                    {
                        accountMappingBO.DeleteAll(accountMappingList);
                        accountMappingBO.SaveChanges();
                    }
                    LocalSavedFilterBO.Delete(savedFilter);
                    LocalSavedFilterBO.SaveChanges();
                }
            }            
            _currentTweetsView.txtFilter.Text = "";
            _accountInfo.BindSavedFilters();
            Panel info = null; //Need to change this
            LoadHomeView(info);
        }

        public void LoadListView(TwitterListExtended twitterListExt)
        { 
            _tweetSender.txtSearchBox.Text = "";
            _userProfileView.Visibility = Visibility.Collapsed;
            _accountInfo.SelectPanel();

            SortableObservableCollection<TwitterStatus> listStatusCollections;

            TOBBaseObject Twitter = AccountManager.Instance.GetTOBObject(twitterListExt.UserAccount);
            var twitterLists = Twitter.GetStatusesFromList(twitterListExt.User.ScreenName, (long)twitterListExt.Id);
            if (twitterLists == null)
            {
                return;
            }

            string listPanelKey = "LISTPANEL" + twitterListExt.Id;
            UserAllTweets listView;

            if (_panelViewDict.ContainsKey(listPanelKey))
            {
                listView = _panelViewDict[listPanelKey];
            }
            else
            {
                listView = new UserAllTweets();
                //listView.DataTemplate = listView.FindResource("GroupsStatusesDataTemplate") as DataTemplate;

                listView.CollectionTypeFilter = TOBEntityEnum.None;

                _panelViewDict.Add(listPanelKey, listView);
            }

            listStatusCollections = new SortableObservableCollection<TwitterStatus>(twitterLists.ToList());
            listView.ListStatusCollection = listStatusCollections;
            _currentTweetsView = listView;
            _mainWindow.frmTOBMain.Content = listView;

        }
                             
        public void DeleteList(TwitterListExtended twitterListExt)
        {
            MessageBoxResult messageResult;
            messageResult = MessageBox.Show("Are you sure you want to delete this group?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageResult == MessageBoxResult.Yes)
            {
                Account account = LocalAccountBO.Get(acc => acc.Username == twitterListExt.User.ScreenName);
                TOBBaseObject Twitter = AccountManager.Instance.GetTOBObject(account);
                Twitter.DeleteList(twitterListExt.User.ScreenName, (long)twitterListExt.Id);         
                _accountInfo.DeleteLists(twitterListExt);
            }
            Panel info = null; //Need to change this
            LoadHomeView(info);
        }

        public void GroupsPopUpDisplay(Status status)
        {
            _accountInfo.GroupsPopup(status);
        }

        public void RemoveUserFromList(Status status)
        {
            _accountInfo.ListUserRemovePopUp(status);
        }

        public void DeleteTweets(Status status)
        {
            if (status != null)
            {
                MessageBoxResult messageResult;
                messageResult = MessageBox.Show("Are you sure you want to delete?", "Delete Confirmation", MessageBoxButton.YesNo);
                if (messageResult == MessageBoxResult.Yes)
                {
                    TOBTwitterO Twitter = new TOBTwitterO(status.Account);
                    Twitter.DeleteStatus(status.TwitterStatusId.Value);
                    _statusCollection.Remove(status as TOBEntityBase);                    
                    _currentTweetsView.Collection = _statusCollection;
                }
            }
        }

        public void DeleteDirectMessages(DirectMessage directMessage)
        {
            if (directMessage != null)
            {
                MessageBoxResult messageResult;
                messageResult = MessageBox.Show("Are you sure you want to delete?", "Delete Confirmation", MessageBoxButton.YesNo);
                if (messageResult == MessageBoxResult.Yes)
                {
                    TOBTwitterO Twitter = new TOBTwitterO(directMessage.Account);
                    Twitter.DeleteDM((long)directMessage.TwitterId);
                    _directMessagesCollection.Remove(directMessage as TOBEntityBase);
                    _currentTweetsView.Collection = _directMessagesCollection;
                }
            }
        }

        public void FollowUser(TwitterUser twitterUser)
        {
            foreach (TOBBaseObject tobObj in AccountManager.Instance.TobObjects)
            {
                //users = tobObj.QueryUsers(query);
                tobObj.FollowUser(twitterUser.ScreenName);
                //if (users != null)
                //{
                //    break;
                //}
            }
        }
                      
    }
}
