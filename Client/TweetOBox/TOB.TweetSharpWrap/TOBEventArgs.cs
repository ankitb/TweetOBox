using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOB.Entities;

namespace TOB.TweetSharpWrap
{
    public class TOBEventArgs : EventArgs
    {
        private List<TOBEntityBase> _entityList;
        private Account _account;
        private TOBEntityEnum _tweetEnum;

        public TOBEntityEnum TweetEnum
        {
            get { return _tweetEnum; }
            set { _tweetEnum = value; }
        }

        public List<TOBEntityBase> EntityList
        {
            get { return _entityList; }
            set { _entityList = value; }
        }

        public Account TOBAccount
        {
            get { return _account; }
            set { _account = value; }
        }
    }
}
