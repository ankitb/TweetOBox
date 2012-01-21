using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOB.TweetSharpWrap
{
    public enum TOBEntityEnum
    {
        Home ,
        Replies,
        DirectMessages,
        ReTweet,
        Favorite,       
        Status,         
        UserProfile,
        Any,
        None,
        ListStatus
    }

    public enum PluginTypeEnum
    {
        PanelType,
        SavedSearch,
        SavedFilter        
    }
}
