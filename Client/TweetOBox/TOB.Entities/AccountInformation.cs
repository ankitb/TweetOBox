using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TOB.Entities
{
    public class AccountInformation
    {
        public string UserName
        {
            get;
            set;
        }

        public int FollowersCount
        {
            get;
            set;
        }

        public int FriendsCount
        {
            get;
            set;
        }

        public int TweetsCount
        {
            get;
            set;
        }

        public int AccountID
        {
            get;
            set;
        }
    }

    //public partial class Account
    //{
    //    private bool _isSelected;

    //    [DataMember]
    //    public bool IsSelected
    //    {
    //        get { return this._isSelected; }
    //        set 
    //        { 
    //            this._isSelected = value;
    //            this.SendPropertyChanged("IsSelected");
    //        }
    //    }
    //}
}
