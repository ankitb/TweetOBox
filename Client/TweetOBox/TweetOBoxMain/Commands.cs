using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace TweetOBoxMain
{
    public class TOBCommands
    {
        static TOBCommands()
        {
            ReplyTweet = new RoutedUICommand("Reply Tweet", "ReplyTweet", typeof(TOBCommands));
            ReTweet = new RoutedUICommand("Re Tweet", "ReTweet", typeof(TOBCommands));
            DirectTweet = new RoutedUICommand("Direct Message", "DirectTweet", typeof(TOBCommands));
            ShowUserOnlyTweets = new RoutedUICommand("Show User's Tweets", "ShowUserOnlyTweets", typeof(TOBCommands));
            ShowUserProfile = new RoutedUICommand("Show User's Profile", "ShowUserProfile", typeof(TOBCommands));
            ShowHomeView = new RoutedUICommand("Show Home View", "ShowHomeView", typeof(TOBCommands));
            ShowRetweetsView = new RoutedUICommand("Show Retweets View", "ShowRetweetsView", typeof(TOBCommands));
            ShowRepliesView = new RoutedUICommand("Show Replies View", "ShowRepliesView", typeof(TOBCommands));
            ShowUserProfileForTags = new RoutedUICommand("ShowUserProfileForTag", "Show User Profile ForTags", typeof(TOBCommands));
            DismissUIObject = new RoutedUICommand("DismissUIObject", "Dismiss UI Object from List", typeof(TOBCommands));

            FollowUser = new RoutedUICommand("FollowUser", "FollowUser", typeof(TOBCommands));
            BlockUser = new RoutedUICommand("Block User", "BlockUser", typeof(TOBCommands));
            UnFollowUser = new RoutedUICommand("UnFollow User", "UnFollowUser", typeof(TOBCommands));
            ReportSpamUser = new RoutedUICommand("Report Spam User", "ReportSpamUser", typeof(TOBCommands));
            MarkAsFavourite = new RoutedUICommand("Mark As Favorite", "MarkAsFavorite", typeof(TOBCommands));
            ShowMarkasFavourite = new RoutedUICommand("Show MarkasFavourite", "ShowMarkasFavourite", typeof(TOBCommands));
            ShowDirectMessageView = new RoutedUICommand("Show DirectMessage View", "ShowDirectMessageView", typeof(TOBCommands));
            DeleteTweets = new RoutedUICommand("Delete Tweets", "DeleteTweets", typeof(TOBCommands));        
            DeleteDirectMessages = new RoutedUICommand("Delete DirectMessages", "DeleteDirectMessages", typeof(TOBCommands));
            
            //Plugin Command.
            PluginCommand = new RoutedUICommand("Plugin Command", "PluginCommand", typeof(TOBCommands));
            DeletePluginCommand = new RoutedUICommand("Delete Plugin Command", "DeletePluginCommand", typeof(TOBCommands));

            //Filter Commands.
            SaveFilterCommand = new RoutedUICommand("Save Filter Command", "SaveFilterCommand", typeof(TOBCommands));
            FilterCommand = new RoutedUICommand("Filter Command", "FilterCommand", typeof(TOBCommands));
            DeleteFilterCommand = new RoutedUICommand("Delete Filter Command", "DeleteFilterCommand", typeof(TOBCommands));
            ShowFilterForTags = new RoutedUICommand("Show Filter For Tags", "ShowFilterForTags", typeof(TOBCommands)); 
            // Search Command.
            SaveSearchCommand = new RoutedUICommand("Save Search Command", "SaveSearchCommand", typeof(TOBCommands));
            SearchCommand = new RoutedUICommand("Search Command", "SearchCommand", typeof(TOBCommands));
            DeleteSearchCommand = new RoutedUICommand("Delete Search Command", "DeleteSearchCommand", typeof(TOBCommands)); 

            //Groups
            GroupsCommand = new RoutedUICommand("Groups Command", "GroupsCommand", typeof(TOBCommands));           
            DeleteGroupCommand = new RoutedUICommand("Delete Group Command", "DeleteGroupCommand", typeof(TOBCommands));
            AddToGroupCommand = new RoutedUICommand("Add To Group Command", "AddToGroupCommand", typeof(TOBCommands)); 

            //List
            ShowListUserProfile = new RoutedUICommand("Show List User Profile", "ShowListUserProfile", typeof(TOBCommands));
            RemoveFromList = new RoutedUICommand("Remove From List", "RemoveFromList", typeof(TOBCommands));


        }
        public static RoutedUICommand ReplyTweet { get; private set; }
        public static RoutedUICommand ReTweet { get; private set; }
        public static RoutedUICommand DirectTweet { get; private set; }
        public static RoutedUICommand ShowUserProfile { get; private set; }
        public static RoutedUICommand ShowUserOnlyTweets { get; private set; }
        public static RoutedUICommand ShowHomeView { get; private set; }
        public static RoutedUICommand ShowRepliesView { get; private set; }
        public static RoutedUICommand ShowRetweetsView { get; private set; }
        public static RoutedUICommand ShowUserProfileForTags { get; private set; }
        public static RoutedUICommand DismissUIObject { get; private set; }

        public static RoutedUICommand FollowUser { get; private set; }
        public static RoutedUICommand BlockUser { get; private set; }
        public static RoutedUICommand UnFollowUser { get; private set; }
        public static RoutedUICommand ReportSpamUser { get; private set; }
        public static RoutedUICommand MarkAsFavourite { get; private set; }
        public static RoutedUICommand ShowMarkasFavourite { get; private set; }
        public static RoutedUICommand ShowDirectMessageView { get; private set; }
        public static RoutedUICommand DeleteTweets { get; private set; }   
        public static RoutedUICommand DeleteDirectMessages { get; private set; }

        //Plugin Commands.
        public static RoutedUICommand PluginCommand { get; private set; }
        public static RoutedUICommand DeletePluginCommand { get; private set; }

        //Filter Commands.
        public static RoutedUICommand SaveFilterCommand { get; private set; }
        public static RoutedUICommand FilterCommand { get; private set; }
        public static RoutedUICommand DeleteFilterCommand { get; private set; }
        public static RoutedUICommand ShowFilterForTags { get; private set; }  
        //Search Commands
        public static RoutedUICommand SaveSearchCommand { get; private set; }
        public static RoutedUICommand SearchCommand { get; private set; }
        public static RoutedUICommand DeleteSearchCommand { get; private set; }

        //Groups
        public static RoutedUICommand GroupsCommand { get; private set; }        
        public static RoutedUICommand DeleteGroupCommand { get; private set; }
        public static RoutedUICommand AddToGroupCommand { get; private set; }

        //List
        public static RoutedUICommand ShowListUserProfile { get; private set; }
        public static RoutedUICommand RemoveFromList { get; private set; }
    }
}
