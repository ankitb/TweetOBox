using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TOB.Entities;
using TOB.TweetSharpWrap;
using System.Windows.Input;
using System.Collections.ObjectModel;
using TweetOBoxMain.Utility;
using System.Windows;

namespace TweetOBoxMain
{
    public class Panel : INotifyPropertyChanged
    {
        private TOBEntityEnum _panelName;
        private RoutedUICommand _command;
        private bool _isSelected = true;
        private uint _unreadCount = 0;

        public bool IsSelected
        {
            get 
            { 
                return _isSelected; 
            }
            set
            {
                _isSelected = value;
                RaiseNotifyPropertyChanged("IsSelected");
            }
        }

        public uint UnReadCount
        {
            get { return _unreadCount; }
            set { 
                _unreadCount = value;
                RaiseNotifyPropertyChanged("UnReadCount");
            }
        }

        private void RaiseNotifyPropertyChanged(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,new PropertyChangedEventArgs(p));
            }
        }

        public RoutedUICommand Command
        {
            get { return _command; }
            set { _command = value; }
        }
        
        AccountList _accoutList;
        List<Account> _tobAccounts;
        List<Account> TobAccounts
        {
            get 
            {
                if (_tobAccounts == null && !DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    _tobAccounts = AccountManager.Instance.GetCurrentAccounts();
                }

                return _tobAccounts;
            }
        }

        KeyValuePair<TOBEntityEnum, String> _namePair;

        public KeyValuePair<TOBEntityEnum, String> NamePair
        {
            get { return _namePair; }
            set { _namePair = value; }
        }

        public Panel(KeyValuePair<TOBEntityEnum, String> namePair, RoutedUICommand command, bool Isselected)
        {
            _isSelected = Isselected;
            _namePair = namePair;
            _panelName = namePair.Key;
            _command = command;
            _accoutList = new AccountList();

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                foreach (Account acc in TobAccounts)
                {
                    _accoutList.Add(new AccountPanel(acc.Id, acc.Username, this));
                }
            }
        }

        public Panel(TOBEntityEnum name)
        {
            _panelName = name;
            _command = null; ;
            _accoutList = new AccountList();
            foreach (Account acc in TobAccounts)
            {
                _accoutList.Add(new AccountPanel(acc.Id, acc.Username));
            }
        }

        public TOBEntityEnum PanelName
        {
            get { return _panelName; }
            set { _panelName = value; }
        }


        public AccountList AccountList
        {
            get
            {
                return _accoutList;
            }
            set
            {
                _accoutList = value;
            }
        }

        public void CheckAllAccounts()
        {
            _accoutList.CheckAllAccount();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        
    }


    public class PanelList : ObservableCollection<Panel>
    {
        public PanelList()
        {
            Panel all = new Panel(new KeyValuePair<TOBEntityEnum,string>(TOBEntityEnum.Home, "[Home]"), TOBCommands.ShowHomeView,true);
            all.CheckAllAccounts();
            all.IsSelected = true;
            this.Add(all);
            Panel retweet = new Panel(new KeyValuePair<TOBEntityEnum, string>(TOBEntityEnum.ReTweet, "[ReTweet]"), TOBCommands.ShowRetweetsView, false);
            retweet.CheckAllAccounts();
            this.Add(retweet);
            Panel reply = new Panel(new KeyValuePair<TOBEntityEnum, string>(TOBEntityEnum.Replies, "[Replies]"), TOBCommands.ShowRepliesView, false);
            reply.CheckAllAccounts();
            this.Add(reply);
            Panel dm = new Panel(new KeyValuePair<TOBEntityEnum, string>(TOBEntityEnum.DirectMessages, "[Direct Messages]"), TOBCommands.ShowDirectMessageView, false);
            dm.CheckAllAccounts();
            this.Add(dm);
            //Panel Favourite = new Panel(new KeyValuePair<TOBEntityEnum, string>(TOBEntityEnum.Favorite, "[Favorite]"), TOBCommands.ShowMarkasFavourite, false);
            //this.Add(Favourite);          
        }


        internal void SelectPanel()
        {
            Panel selectedPanel = this.Where(p=>p.IsSelected == true).FirstOrDefault();
            if (selectedPanel != null)
            {
                selectedPanel.IsSelected = false;
            }            
        }
    }

    public class AccountList : ObservableCollection<AccountPanel>
    {        
        public void CheckAllAccount()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[i].IsSelected = true;
            }
        }
    }   



    public class AccountPanel : INotifyPropertyChanged
    {
        int _id;
        Panel _parentPanel;

        public Panel ParentPanel
        {
            get { return _parentPanel; }
            set { _parentPanel = value; }
        }

        public AccountPanel(int id, string name)
        {
            _id = id;
            _accName = name;
        }

        public AccountPanel(int id, string name, Panel parentPanel)
        {
            _id = id;
            _accName = name;
            _parentPanel = parentPanel;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        string _accName;

        public string AccName
        {
            get { return _accName; }
            set { _accName = value; }
        }
        bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value;
            RaisePropertyChanged("IsSelected");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        #endregion

        //public override bool Equals(object obj)
        //{
        //    AccountPanel currentPanel = obj as AccountPanel;
        //    if ( currentPanel.PanelName != this.Panel
        //}
    }
}
