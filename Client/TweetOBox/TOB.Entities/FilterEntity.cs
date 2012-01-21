using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace TOB.Entities
{
    public partial class SavedFilter
    {
        private RoutedUICommand _commandFilter;

        public RoutedUICommand CommandFilter
        {
            get { return _commandFilter; }
            set { _commandFilter = value; }
        }

        private RoutedUICommand _commandDeleteFilter;
        
        public RoutedUICommand CommandDeleteFilter
        {
            get { return _commandDeleteFilter; }
            set { _commandDeleteFilter = value; }
        }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _panelTypeText;

        public string PanelTypeText
        {
            get { return _panelTypeText; }
            set { _panelTypeText = value; }
        }

        private string _accountNames;

        public string AccountNames
        {
            get { return _accountNames; }
            set { _accountNames = value; }
        }
    }

    public partial class SavedSearch
    {
        private RoutedUICommand _commandSearch;

        public RoutedUICommand CommandSearch
        {
            get { return _commandSearch; }
            set { _commandSearch = value; }
        }

        private RoutedUICommand _commandSearchDelete;

        public RoutedUICommand CommandSearchDelete
        {
            get { return _commandSearchDelete; }
            set { _commandSearchDelete = value; }
        }
    }

    public partial class Group
    {
        private RoutedUICommand _commandGroups;

        public RoutedUICommand CommandGroups
        {
            get { return _commandGroups; }
            set { _commandGroups = value; }
        }

        private RoutedUICommand _commandDeleteGroups;

        public RoutedUICommand CommandDeleteGroups
        {
            get { return _commandDeleteGroups; }
            set { _commandDeleteGroups = value; }
        }
    }  

    public partial class Account
    {
        private string _decryptedPassword;

        public string DecryptedPassword
        {
            get { return _decryptedPassword; }
            set { _decryptedPassword = value; }
        }
    }

    public partial class UserProfile
    {
        private RoutedUICommand _commandAddtoGroup;

        public RoutedUICommand CommandAddtoGroup
        {
            get { return _commandAddtoGroup; }
            set { _commandAddtoGroup = value; }
        }
    }

    public partial class PanelType
    {
        private string _displayName;

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if ((this._displayName != value))
                {
                    this.SendPropertyChanging();
                    this._displayName = value;
                    this.SendPropertyChanged("DisplayName");
                }
            }
        }

        partial void OnLoaded()
        {
            DisplayName = this.Name;
        }
    }

    public partial class SavedFilter
    {
        private string _displayName;        

        public string DisplayName
        {
            get { return _displayName; }
            set {
                if ((this._displayName != value))
				{
					this.SendPropertyChanging();
                    this._displayName = value;
                    this.SendPropertyChanged("DisplayName");
				} }
        }

        partial void OnLoaded()
        {
            DisplayName = this.FilterText;        
        }
    }

    public partial class SavedSearch
    {
        private string _displayName;
        
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if ((this._displayName != value))
                {
                    this.SendPropertyChanging();
                    this._displayName = value;
                    this.SendPropertyChanged("DisplayName");
                }
            }
        }

        partial void OnLoaded()
        {
            DisplayName = this.SearchText;          
        }
    }
       
    public partial class SavedPluginView
    {
        private RoutedUICommand _command;

        public RoutedUICommand Command
        {
            get { return _command; }
            set { _command = value; }
        }

        private RoutedUICommand _deletePluginCommand;

        public RoutedUICommand DeletePluginCommand
        {
            get { return _deletePluginCommand; }
            set { _deletePluginCommand = value; }
        }
    }
}




