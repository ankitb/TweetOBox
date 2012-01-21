using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOB.Entities
{
    public class TOBEntityBase
    {
        DateTime? _sortableColumn;
        string _searchableString;
        int _accountId;

        public int AccountsId
        {
            get {  return _accountId; }
            set {
                if (_accountId == 0)
                {
                    _accountId = value;
                }
            }
        }

        public string SearchableString
        {
            get { return _searchableString; }
            set { _searchableString = value; }
        }

        public DateTime? SortableColumn
        {
            get { return _sortableColumn; }
            set { _sortableColumn = value; }
        }

        public virtual bool IsMarkAsRead
        {
            get;
            set;
        }

        public delegate void UnReadPropertyChangedDelegate(TOBEntityBase obj);
        
        public static UnReadPropertyChangedDelegate UnReadPropertyChanged;

        public void OnUnReadPropertyChanged(TOBEntityBase obj)
        {
            if (UnReadPropertyChanged != null)
            {
                UnReadPropertyChanged(obj);
            }
        }

    }

    public partial class Status
    {
        partial void OnLoaded()
        {
            this.SortableColumn = this.TwitterCreatedDate;
            this.SearchableString = (this.Text + " " + this.UserProfile.ScreenName + " " + this.UserProfile.Name).ToLower();
            this.AccountsId = (int)this.AccountId;
        }

        public void InternalSetAccount(Account acc)
        {
            this._Account.Entity = acc;
        }

        public override bool IsMarkAsRead
        {
            get
            {
                return this.IsRead == null ? false : (bool)this.IsRead;
            }
            set
            {
                this.IsRead = value;
            }
        }
    }

    public partial class DirectMessage
    {
        partial void OnLoaded()
        {
            this.SortableColumn = this.TwitterCreatedDate;
            this.SearchableString = (this.Text + " " + this.UserScreenName + " " + this.UserProfile.Name).ToLower();
            AccountsId = (int)this.AccountId;
        }
        public override bool IsMarkAsRead
        {
            get
            {
                return this.IsRead == null ? false : (bool)this.IsRead;
            }
            set
            {
                this.IsRead = value;
            }
        }
    }

    public partial class UserProfile
    {
        partial void OnLoaded()
        {
            this.SearchableString = (this.Name + " " + this.ScreenName).ToLower();
        }
    }
}
