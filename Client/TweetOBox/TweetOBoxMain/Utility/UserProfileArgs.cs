using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOB.Entities;

namespace TweetOBoxMain.Utility
{
    public class UserProfileArgs : System.EventArgs
    {
        private Status statuslist;

        public UserProfileArgs(Status _status)
        {
            statuslist = _status;            
        }

        public Status StatusList
        {
            get
            {
                return statuslist;
            }
        }
    }
}
