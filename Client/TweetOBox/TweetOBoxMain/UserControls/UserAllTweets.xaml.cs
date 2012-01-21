using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.ComponentModel;
using System.Windows.Threading;
using TOB.Entities;
using TOB.TweetSharpWrap;
using System.Threading;
using TweetOBoxMain.Utility;
using System.Diagnostics;
using TOB.Plugin;
using TweetOBoxMain.Model;
using System.Collections;
using TweetOBoxMain.Notifications;
using System.Windows.Controls.Primitives;
using TweetSharp;
using TOB.Utility;

namespace TweetOBoxMain.UserControls
{
    /// <summary>
    /// Interaction logic for UserAllTweets.xaml
    /// 
    /// Design details - This class is the main Panel that shows the list of MessageObjects for any stream. 
    /// Stream is referred to as _collection in this class
    /// Filtering of the streams occurs in 2 stages - Default/Base filter and UserAccount Filter
    /// By Default, no CollectionFilter is specified. A CollectionFilter can be specified which filters out all new objects that don't meet the
    /// specified criteria.
    /// UserAccount Filter is linked to the Panel that contains the UserList selection. 
    /// </summary>
    public partial class UserAllTweets : UserControl
    {      
        private ITOBPlugin _plugin = null;
        private bool _isPluginEnabled = false;
        Button _saveFilterButton;
        private DispatcherTimer _timerIsReadDm = null;
        private SortableObservableCollection<TOBEntityBase> _collection;
        private SortableObservableCollection<TwitterSearchStatus> _searchcollection;
        private SortableObservableCollection<TwitterUser> _peoplecollection;
        private SortableObservableCollection<TwitterStatus> _listStatuscollection;

        private TOBEntityEnum _collectionTypeFilter = TOBEntityEnum.Any;
        private Type _baseType = null;
        private Panel _navPanel = null;
        private Func<TOBEntityBase, bool> _collectionFilter = null;
        private string _internalStringFilter = null;

        private int MAX_PLUGIN_COUNT = 25;
        private int MAX_STATUS_COUNT = 500;
        private SavedFilterBO _savedFilterBO = null;
        //private DispatcherTimer _timerTweetsRefresh = null;
        
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

        private DirectMessageBO _directMessageBO = null;

        private DirectMessageBO LocalDirectMessageBO
        {
            get
            {
                if (_directMessageBO == null)
                {
                    _directMessageBO = new DirectMessageBO();
                }
                return _directMessageBO;
            }
        }

        public UserAllTweets()
        {
            InitializeComponent();

            //_timerTweetsRefresh = new DispatcherTimer();
            //_timerTweetsRefresh.Interval = TimeSpan.FromSeconds(60);
            //_timerTweetsRefresh.Tick += new EventHandler(_timerTweetsRefresh_Tick);
            //_timerTweetsRefresh.Start();

            _timerIsReadDm = new DispatcherTimer();
            _timerIsReadDm.Interval = TimeSpan.FromSeconds(1.5);
            _timerIsReadDm.Tick += new EventHandler(_timerIsReadDm_Tick);
        }

        //void _timerTweetsRefresh_Tick(object sender, EventArgs e)
        //{
        //    if(Collection != null)
        //        listTweets.Items.Refresh();
        //}

        void _navPanel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //System.Windows.MessageBox.Show("Something changed!");
        }

        public Panel NavBindingPanel
        {
            get
            {
                return _navPanel;
            }
            set
            {
                _navPanel = value;
                //_navPanel.PropertyChanged += new PropertyChangedEventHandler(_navPanel_PropertyChanged);
            }
        }

        public string InternalStringFilter
        {
            set
            {
                _internalStringFilter = value.ToLower();

                if (_collectionFilter == null)
                {
                    _collectionFilter = (s => s.SearchableString.Contains(_internalStringFilter));
                }
                else
                {
                    _collectionFilter += (s => s.SearchableString.Contains(_internalStringFilter));
                }
            }
        }

        public UserAllTweets(ITOBPlugin plugin)
        {
            InitializeComponent();
            DataTemplate = null;
            listTweets.ItemTemplateSelector = null;
            
            //Bug with 1 item ScrollViewer doesn't activate
            listTweets.Items.Insert(0, new Label() { Height = 2 });
            
            if (plugin != null)
            {
                _plugin = plugin;
                _plugin.RemoveControlFromPanel = new PanelInteractionDelegate(_plugin_RemoveObjectFromUI);
                _plugin.AddControlToPanel = new PanelInteractionDelegate(_plugin_AddControlToPanel);
                _isPluginEnabled = true;
            }
        }
              
        //void userAllTweets_Loaded(object sender, RoutedEventArgs e)
        //{
        //    DirectMessage directMessage = userAllTweets.DataContext as DirectMessage;

        //    if (directMessage != null && directMessage.IsRead == false)
        //    {
        //        DataTemplate dt = this.FindResource("DirectMessageDataTemplate") as DataTemplate;
        //        Border content = dt.LoadContent() as Border;
        //        TweetsTextBlock txt = content.FindName("tbdirectMsgText") as TweetsTextBlock;
        //        txt.FontWeight = FontWeights.Bold;
        //    }
        //}

        void _plugin_AddControlToPanel(UserControl obj)
        {
            if (_isPluginEnabled)
            {
                if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                {
                    listTweets.Items.Insert(0, obj);
                }
                else
                {
                    Action action = delegate()
                    {
                        listTweets.Items.Insert(0, obj);
                    };

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
                }
            }
        }

        void _plugin_RemoveObjectFromUI(UserControl obj)
        {
            if (_isPluginEnabled)
            {
                if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                {
                    if (listTweets.Items.Contains(obj))
                    {
                        listTweets.Items.Remove(obj);
                    }
                }
                else
                {
                    Action action = delegate()
                    {
                        if (listTweets.Items.Contains(obj))
                        {
                            listTweets.Items.Remove(obj);
                        }
                    };

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
                }
                
            }
        }

        public void RemoveUIObjectFromList(object obj)
        {
            if (obj is TOBEntityBase)
            {
                TOBEntityBase bObj = obj as TOBEntityBase;
                if (_collection.Contains(bObj))
                {
                    _collection.Remove(bObj);
                }
            }
            else if (obj is TwitterStatus)
            {
                TwitterStatus bObj = obj as TwitterStatus;
                if (_listStatuscollection.Contains(bObj))
                {
                    _listStatuscollection.Remove(bObj);
                }
            }
        }

        private List<TOBEntityBase> _tempInsertList = new List<TOBEntityBase>();

        public void AddList(List<TOBEntityBase> beList, AccountList accountList)
        {
                if (CollectionTypeFilter == TOBEntityEnum.None)
                    return;

                //TOB.Logger.TOBLogger.WriteInfo("AddList recieved - " + beList.Count);

                List<TOBEntityBase> newObjList = null;

                if (CollectionFilter != null)
                {
                    newObjList = beList.Where(CollectionFilter).ToList();
                }
                else
                {
                    newObjList = beList;
                }

                if (_isPluginEnabled)
                {
                    if(newObjList.Count < 10)
                        _plugin.ProcessInStream(newObjList);
                    else
                        for (int i = 0; i < newObjList.Count - 1; i = i + 10)
                        {
                            _plugin.ProcessInStream(newObjList.GetRange(i, ((newObjList.Count - i) > 10) ? 10 : newObjList.Count - i));

                            if (listTweets.Items.Count > MAX_PLUGIN_COUNT)
                            {
                                break;
                            }
                        }

                    Action action = delegate()
                    {
                        //Optimization ot only flush objects when over 10% of threshold
                        if (listTweets.Items.Count > (int)(MAX_PLUGIN_COUNT * 1.1))
                        {
                            while (listTweets.Items.Count > MAX_PLUGIN_COUNT)
                            {
                                listTweets.Items.RemoveAt(listTweets.Items.Count - 1);
                            }

                            listTweets.Items.Refresh();
                        }
                        
                    };

                    this.Dispatcher.Invoke(action, DispatcherPriority.Background);
                }
                else
                {
                    _tempInsertList.Clear();

                    Action action = delegate()
                    {
                        foreach (TOBEntityBase item in newObjList)
                        {
                            if ((CollectionTypeFilter == TOBEntityEnum.Any) || (item.GetType() == _baseType))
                            {
                                _collection.Insert(0, item);
                                _tempInsertList.Add(item);
                            }
                        }

                        if (accountList != null)
                        {
                            List<int> accList = accountList.Where(a => a.IsSelected == true).Select(a => a.Id).ToList();
                            _collection = new SortableObservableCollection<TOBEntityBase>(_collection.Where(c => accList.Contains(c.AccountsId)));
                            
                            //Bad hack
                            if (_collection.Count <= (int)(MAX_STATUS_COUNT * 1.2))
                                listTweets.ItemsSource = _collection;
                        }

                        if (newObjList.Count > 1)
                        {
                            //Get distinct account;
                            _collection.LocalSort(ListSortDirection.Descending);
                        }

                        //Optimization to only flush objects when over 20% of threshold
                        if (_collection.Count > (int)(MAX_STATUS_COUNT * 1.2))
                        {
                            _collection = new SortableObservableCollection<TOBEntityBase>(_collection.Take(MAX_STATUS_COUNT));
                            listTweets.ItemsSource = _collection;

                            if (NavBindingPanel != null)
                            {
                                NavBindingPanel.UnReadCount = (uint)_collection.Where(coll => coll.IsMarkAsRead == false).Count();
                            }

                            //GC.Collect();
                            //GC.WaitForPendingFinalizers();
                            //GC.Collect();
                        }
                        else
                        {
                            if (NavBindingPanel != null)
                            {
                                NavBindingPanel.UnReadCount = NavBindingPanel.UnReadCount + (uint)_tempInsertList.Where(coll => coll.IsMarkAsRead == false).Count();
                            }
                        }
                    };

                    this.Dispatcher.Invoke(action, DispatcherPriority.Background);
                }
        }

        public Func<TOBEntityBase, bool> CollectionFilter
        {
            get
            {
                return _collectionFilter;
            }
            set
            {
                if (_collectionFilter == null)
                {
                    _collectionFilter = value;
                }
                else
                {
                    _collectionFilter += value;
                }

                if (_collection != null && _collection.Count > 0)
                {
                    _collection = new SortableObservableCollection<TOBEntityBase>(_collection.Where(_collectionFilter));
                }
            }
        }

        public TOBEntityEnum CollectionTypeFilter
        {
            get
            {
                return _collectionTypeFilter;
            }
            set
            {
                _collectionTypeFilter = value;
            }
        }

        public Collection<TOBEntityBase> Collection
        {
            get
            {
                return _collection;
            }
            set
            {
                BackgroundWorker SetCollectionWorker = new BackgroundWorker();
                SetCollectionWorker.DoWork += new DoWorkEventHandler(SetCollectionWorker_DoWork);
                SetCollectionWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SetCollectionWorker_RunWorkerCompleted);
                SetCollectionWorker.RunWorkerAsync(value);
            }
            
        }

        void SetCollectionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!_isPluginEnabled)
            {
                listTweets.ItemsSource = _collection;

                TOBEntityBase.UnReadPropertyChanged += new TOBEntityBase.UnReadPropertyChangedDelegate(UnReadPropertyChangedHandler);

                if (NavBindingPanel != null)
                {
                    NavBindingPanel.UnReadCount = (uint)_collection.Where(coll => coll.IsMarkAsRead == false).Count();
                }
            }
        }

        void SetCollectionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Collection<TOBEntityBase> incomingColl = (Collection<TOBEntityBase>) e.Argument;

            if (CollectionTypeFilter == TOBEntityEnum.None)
                return;

            if (CollectionFilter != null)
            {
                _collection = new SortableObservableCollection<TOBEntityBase>(incomingColl.Where(CollectionFilter));
            }
            else
            {
                _collection = new SortableObservableCollection<TOBEntityBase>(incomingColl.Take(MAX_STATUS_COUNT));
            }

            _collection.LocalSort(ListSortDirection.Descending);
            _baseType = (_collection.Count > 0) ? _collection[0].GetType() : null;

            if (_isPluginEnabled)
            {
                //Action action = delegate()
                //{
                //Batch process _collection for performance issues.
                List<TOBEntityBase> ppList = _collection.ToList();
                ppList.Reverse();

                for (int i = 0; i < ppList.Count - 1; i = i + 10)
                {
                    _plugin.ProcessInStream(ppList.GetRange(i, ((ppList.Count - i) > 10) ? 10 : ppList.Count - i));

                    if (listTweets.Items.Count > MAX_PLUGIN_COUNT)
                    {
                        break;
                    }
                }
                //};

                //this.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);
            }
                
        }

        public void UnReadPropertyChangedHandler(TOBEntityBase obj)
        {
            if (_collection.Contains(obj))
            {
                if (NavBindingPanel != null)
                {
                    NavBindingPanel.UnReadCount--;
                }
            }
        }

        public SortableObservableCollection<TwitterSearchStatus> SearchCollection
        {
            get
            {
                return _searchcollection;
            }
            set
            {
                _searchcollection = value;
                listTweets.ItemsSource = _searchcollection;
            }
        }

        public SortableObservableCollection<TwitterUser> PeopleCollection
        {
            get
            {
                return _peoplecollection;
            }
            set
            {
                _peoplecollection = value;
                listTweets.ItemsSource = value;            
            }
        }

        public SortableObservableCollection<TwitterStatus> ListStatusCollection
        {
            get
            {
                return _listStatuscollection;
            }
            set
            {
                _listStatuscollection = value;
                listTweets.ItemsSource = value;
            }
        }
                     
        private void btnMore_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }

        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                string filterText = txtFilter.Text.ToLower();
                if (filterText.Length > 3 || e.Changes.Where(temp => temp.RemovedLength > 0).FirstOrDefault() != null)
                {
                    FilterTweets(filterText);
                }
            }
            catch (Exception ex)
            {
                TOB.Logger.TOBLogger.WriteDebugInfo(ex.ToString());
            }
        }

        public void FilterTweets(string filterText)
        {
            listTweets.Items.Filter = delegate(object obj)
            {
                TOBEntityBase entityBase = obj as TOBEntityBase;               
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

        private void btnFilterTweets_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	txtFilter.Text="";
        }

        void HandleLinkClick(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri.IsAbsoluteUri != false)
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
           }
        }            
     
      
        public DataTemplate DataTemplate
        {
            get
            {
                return listTweets.ItemTemplate;
            }
            set
            {
                listTweets.ItemTemplate = value;
            }
        }

        public SavedFilter SaveFilter(Panel currentPanel)
        {
            SavedFilter savedFilter = null;
            try
            {
                //Temp fix. this is not working for first run, bcoz currentpanel is null for the first time.
                if (currentPanel != null)
                {
                    savedFilter = new SavedFilter();
                    savedFilter.FilterText = txtFilter.Text.Trim();
                    savedFilter.PanelTypeId = (int)currentPanel.PanelName;
                    AccountFilterMappingBO accountFilterMappingBO = new AccountFilterMappingBO();
                    savedFilter.AccountFilterMappings = new System.Data.Linq.EntitySet<AccountFilterMapping>();
                    var accountPanel = currentPanel.AccountList.Where(acc => acc.IsSelected == true).ToList();
                    //Select(acc => acc.IsSelected = true).ToList();
                    foreach (AccountPanel account in accountPanel)
                    {
                        AccountFilterMapping accountFilterMapping = new AccountFilterMapping();
                        accountFilterMapping.AccountId = account.Id;
                        accountFilterMapping.FilterId = savedFilter.Id;
                        savedFilter.AccountFilterMappings.Add(accountFilterMapping);
                    }
                    LocalSavedFilterBO.Insert(savedFilter);
                    LocalSavedFilterBO.SaveChanges();
                    TOB.Utility.MessageNotifier.Instance.NotifyMessage("New filter saved...");
                }
                else
                {
                    TOB.Utility.MessageNotifier.Instance.NotifyMessage("No panel is selected...");
 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {               
                txtFilter.Text = "";
            }
            return savedFilter;
        }

        public void SetFilterBoxText(string filterText)
        {
            txtFilter.Text = filterText;
            ControlTemplate template = txtFilter.Template;//this.FindResource("FilterTextBoxControlTemplate") as ControlTemplate;
            Border content = template.LoadContent() as Border;
            _saveFilterButton = content.FindName("btnFilterSave") as Button;
            _saveFilterButton.Visibility = Visibility.Hidden;
        }

        private void btnMarkAsRead_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                SortableObservableCollection<TOBEntityBase> tobCollections = listTweets.ItemsSource as SortableObservableCollection<TOBEntityBase>;
                var coll = tobCollections.Where(t => t.IsMarkAsRead == false);
                //Bug fix 68 Hiren: 
                foreach (var p in coll)
                {
                    p.IsMarkAsRead = true;
                }
                //Entities bound here belong to and originating from the TobObject. Call Save to TobObjects
                foreach (TOBTwitterO tob in AccountManager.Instance.TobObjects)
                {
                    tob.Save();
                }
                
                listTweets.ItemsSource = tobCollections;
                
                if (NavBindingPanel != null)
                {
                    NavBindingPanel.UnReadCount = (uint)tobCollections.Where(c => c.IsMarkAsRead == false).Count();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }       
       
        void _timerIsReadDm_Tick(object sender, EventArgs e)
        {
            if (directMessage != null)
            {
                DirectMessage directMsg = LocalDirectMessageBO.Get(d => d.Id == directMessage.Id);
                if (directMsg.IsRead == false)
                {                  
                    ListBoxItem listBoxItem =
                    (ListBoxItem)(listTweets.ItemContainerGenerator.ContainerFromItem(directMessage));
                    ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(listBoxItem);
                    DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
                    if (myDataTemplate == null)
                    {
                        return;
                    }
                    directMsg.IsRead = true;
                    LocalDirectMessageBO.SaveChanges();
                    TextBlock tbDirectmsg = (TextBlock)myDataTemplate.FindName("tbdirectMsgText", myContentPresenter);
                    tbDirectmsg.FontWeight = FontWeights.Normal;
                    Border bdDirectmsg = (Border)myDataTemplate.FindName("borderBg", myContentPresenter);
                    bdDirectmsg.Background = new SolidColorBrush(Colors.White);
                }
            }
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
        where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
               
        DirectMessage directMessage;
        private void grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _timerIsReadDm.Start();
            directMessage = (sender as Grid).DataContext as DirectMessage;
        }

        private void grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _timerIsReadDm.Stop();
        }
    }
}


