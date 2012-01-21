using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOB.Entities;
using TOB.TweetSharpWrap;

namespace TweetOBoxMain.Utility
{
    public class TweetActionArgs : System.EventArgs
    {
        private TOBEntityBase statuslist;

        public TweetActionArgs(TOBEntityBase _status, TOBEntityEnum TweetType)
        {
            statuslist = _status;
            this.TweetType = TweetType;
        }

        public TOBEntityBase StatusList
        {
            get
            {
                return statuslist;
            }
        }

        public TOBEntityEnum TweetType { get; set; }
    }
}
