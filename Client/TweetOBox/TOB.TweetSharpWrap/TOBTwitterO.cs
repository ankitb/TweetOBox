using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOB.Entities;
using TOB.BLL;
using TweetSharp;
using System.Configuration;
using System.Threading;
using System.Windows;
using TOB.Utility;

namespace TOB.TweetSharpWrap
{
    public class TOBTwitterO : TOBBaseObject
    {
        private System.Timers.Timer _updateTimer = new System.Timers.Timer();
        private Random _randomGen = new Random();
        TwitterClientInfo _twitterClientInfo = new TwitterClientInfo();
        object _syncobject = new object();
        string  _consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        string _consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"]; 
        event System.Timers.ElapsedEventHandler  _timerEvent;
        private byte EventCounter = 0;
        private byte DMDownloadInterval = 5;
        private byte MiscDownloadInterval = 10;
        private StatusBO _statusBO = null;
        private DirectMessageBO _dmBO = null;
        private UserProfileBO _userProfileBO = null;

        private List<Status> _insertStatusList = new List<Status>();

        private UserProfileBO LocalUserProfileBO
        {
            get
            {
                if (_userProfileBO == null)
                {
                    _userProfileBO = new UserProfileBO();
                }
                return _userProfileBO;
            }
        }

        private StatusBO LocalStatusBO
        {
            get
            {
                if(_statusBO == null)
                {
                    _statusBO = new StatusBO();
                }
                
                return _statusBO;
            }

        }

        private DirectMessageBO LocalDirectMessageBO
        {
            get
            {
                if (_dmBO == null)
                {
                    _dmBO = new DirectMessageBO();
                }

                return _dmBO;
            }

        }

        public TOBTwitterO(Account acc)
            : base(acc)
        {           
            _twitterClientInfo.ClientName = "TweetOBox";
            _twitterClientInfo.ClientUrl = "http://www.tweetobox.com";
            _twitterClientInfo.ClientVersion = "1.0.0.0";
            _twitterClientInfo.ConsumerKey = _consumerKey;
            _twitterClientInfo.ConsumerSecret = _consumerSecret;
            
            //FluentTwitter.SetClientInfo(_twitterClientInfo);

            if (LocalUserProfileBO.Get(s => s.ScreenName == acc.Username) == null)
            {
                GetUserProfile(acc.Username);
            }

            _updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            _updateTimer.AutoReset = true;
        }

        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (sender == null)
                {
                    DownloadHomeTimeline(true);
                    DownloadReceivedDirectMessages();
                }
                else
                {
                    DownloadHomeTimeline(false);

                    EventCounter++;

                    if ((EventCounter % DMDownloadInterval) == 0)
                    {
                        DownloadReceivedDirectMessages();
                        //DownloadSentDirectMessages();
                    }

                    if ((EventCounter % MiscDownloadInterval) == 0)
                    {
                        DownloadTwitterLists();
                        EventCounter = 0;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.TOBLogger.WriteDebugInfo(ex.ToString());
            }
        }

        private TwitterService AuthenticateUser(Account account)
        {          
            TwitterService request = null;
            if (account.IsOAuth == true)
            {
                request = new TwitterService(_consumerKey, _consumerSecret, account.AccessToken, account.AccessTokenSecret);
            }
            else
            {
                request = new TwitterService(_twitterClientInfo);    
            }

            if (request == null)
            {
                Logger.TOBLogger.WriteDebugInfo("Request is null for the username = " + account.Username);              
            }

            return request;
        }

        private void DownloadReceivedDirectMessages()
        {
            List<TOBEntityBase> notifyStatusList = new List<TOBEntityBase>();
            //Retrive the last known max id of DirectMessage in the local DB.
            DirectMessageBO directMessageBo = LocalDirectMessageBO;

            DirectMessage maxObj = directMessageBo.GetListBySorting((t => t.AccountId == Acc.Id && t.Recieved == true), (t => t.TwitterId), System.Data.SqlClient.SortOrder.Descending).FirstOrDefault();

            //Get all the sent direct messages
            TwitterService request = AuthenticateUser(Acc);
            IEnumerable<TwitterDirectMessage> ftCol1 = null;

            if (maxObj != null && maxObj.Id != 0)
            {
                ftCol1 = request.ListDirectMessagesReceivedSince(maxObj.TwitterId.Value);
            }
            else
            {
                ftCol1 = request.ListDirectMessagesReceived();
            }

            if (ftCol1 == null)
            {
                Logger.TOBLogger.WriteDebugInfo(request.Response.RequestUri.ToString());
                return;
            }

            if (ftCol1.Count() == 0)
            {
                return;
            }

            foreach (TwitterDirectMessage dm in ftCol1)
            {
                //DirectMessage directMessage = new DirectMessageBO().GetAll().Where(d => d.TwitterId == dm.Id ).FirstOrDefault();
                //if (directMessage == null)
                //{
                    DirectMessage tobStatus = GetTOBDMFromTSDM(dm);
                    tobStatus.Recieved= true;
                    directMessageBo.Insert(tobStatus);
                    directMessageBo.SaveChanges();
                    //Add to notifyable list
                    notifyStatusList.Add(tobStatus);
                //}
            }

            //Throw Event notifying appropriate UI's new statuses are available
            if (notifyStatusList.Count > 0)
            {
                TOBEventArgs eventargs = new TOBEventArgs();
                eventargs.EntityList = notifyStatusList;
                eventargs.TweetEnum = TOBEntityEnum.DirectMessages;
                eventargs.TOBAccount = Acc;
                directMessageBo.SaveChanges();
                OnNewStatus(this, eventargs);
            }

            
        }

        private void DownloadSentDirectMessages()
        {            
            List<TOBEntityBase> notifyStatusList = new List<TOBEntityBase>();
            //Retrive the last known max id of DirectMessage in the local DB.
            DirectMessageBO directMessageBo = LocalDirectMessageBO;

            DirectMessage maxObj = directMessageBo.GetListBySorting((t => t.AccountId == Acc.Id && t.Recieved == false ), (t => t.TwitterId), System.Data.SqlClient.SortOrder.Descending).FirstOrDefault();

            //Get all the sent direct messages
            TwitterService request = AuthenticateUser(Acc);
            IEnumerable<TwitterDirectMessage> ftCol1 = null;

            if (maxObj != null && maxObj.Id != 0)
            {
                ftCol1 = request.ListDirectMessagesSentSince(maxObj.TwitterId.Value);
            }
            else
            {
                ftCol1 = request.ListDirectMessagesSent();
            }

            if (ftCol1 == null)
            {
                Logger.TOBLogger.WriteDebugInfo(request.Response.RequestUri.ToString());
                return;
            }

            if (ftCol1.Count() == 0)
            {
                return;
            }

            foreach (TwitterDirectMessage dm in ftCol1)
            {
                DirectMessage tobStatus = GetTOBDMFromTSDM(dm);
                tobStatus.Recieved = false;
                directMessageBo.Insert(tobStatus);
                //Add to notifyable list
                notifyStatusList.Add(tobStatus);
            }

            //Throw Event notifying appropriate UI's new statuses are available
            if (notifyStatusList.Count > 0)
            {
                TOBEventArgs eventargs = new TOBEventArgs();
                eventargs.EntityList = notifyStatusList;
                eventargs.TweetEnum = TOBEntityEnum.DirectMessages;
                eventargs.TOBAccount = Acc;
                OnNewStatus(this, eventargs);
            }

            directMessageBo.SaveChanges();
        }

        private void DownloadHomeTimeline(bool isFirstRun)
        {
            if (isFirstRun)
            {
                //Sleep while client loads
                Thread.Sleep(3000);
            }

            //Retrieve the last known max id in the local DB.                 
            Status maxObj = LocalStatusBO.GetListBySorting((t => t.AccountId == Acc.Id), (t => t.TwitterStatusId), System.Data.SqlClient.SortOrder.Descending).FirstOrDefault();
            
            TwitterService request = AuthenticateUser(Acc);

            IEnumerable<TwitterStatus> ftColl = null;

            if (maxObj != null)
            {
                ftColl = request.ListTweetsOnHomeTimelineSince((long)maxObj.TwitterStatusId, 200);
            }
            else
            {
                ftColl = request.ListTweetsOnHomeTimeline(150);
            }
            //Stress test
            //TwitterResult responce = new TwitterResult();
            //System.IO.TextReader tr = new System.IO.StreamReader("Twitter.txt");
            //responce.Response = tr.ReadToEnd();
            //var ftColl = responce.AsStatuses();

            if (ftColl == null)
            {
                Logger.TOBLogger.WriteDebugInfo(request.Response.RequestUri.ToString());
                
                MessageNotifier.Instance.NotifyMessage("Error communicating with Twitter Servers");
                return;
            }

            if (ftColl.Count() == 0)
            {
                return;
            }

            //TOB.Logger.TOBLogger.WriteInfo("FriendsTimeline objects recieved - " + ftColl.Count());

            List<TOBEntityBase> notifyStatusList = new List<TOBEntityBase>();

            //Keep default threshold = 5. Means dont accumulate thweets for more than 5 and notify UI for this.
            int THRESHOLD = 5;

            if (ftColl.Count() > THRESHOLD * THRESHOLD)
            {
                THRESHOLD = ftColl.Count() / THRESHOLD;
            }
            else
            {
                THRESHOLD = ftColl.Count() + 1;
            }

            foreach (TwitterStatus tsTS in ftColl)
            {
                Status tobStatus = GetTOBStatusFromTSStatus(tsTS);

                if (tobStatus == null)
                    continue;

                LocalStatusBO.Insert(tobStatus);
                
                //Add to notifyable list
                notifyStatusList.Add(tobStatus);

                //The HACK is that we dont wait for all the tweets to throw onto UI. We will make 
                //count tweets existence if equal to 5 or more than 5.
                if (notifyStatusList.Count >= THRESHOLD)
                {
                    //Throw Event notifying appropriate UI's new statuses are available
                    TOBEventArgs eventargs = new TOBEventArgs();
                    eventargs.EntityList = notifyStatusList.ToList();
                    eventargs.TOBAccount = Acc;
                    eventargs.TweetEnum = TOBEntityEnum.Status;

                    LocalStatusBO.SaveChanges();

                    if (!isFirstRun)
                    {
                        TOB.Logger.TOBLogger.WriteInfo("Sending NewStatus notficiation - " + eventargs.EntityList.Count);
                        OnNewStatus(this, eventargs);
                        notifyStatusList.Clear();

                        //if (isFirstRun)
                        //{
                        //    Thread.Sleep((THRESHOLD * 1000) / 3);
                        //}
                    }
                }
            }

            //Send remaining tweets for the count less than 5 as USUAL.
            //Throw Event notifying appropriate UI's new statuses are available for the remaing tweets for count less than 5
            if (notifyStatusList.Count > 0)
            {
                TOBEventArgs eventargs = new TOBEventArgs();
                eventargs.EntityList = notifyStatusList;
                eventargs.TOBAccount = Acc;
                eventargs.TweetEnum = TOBEntityEnum.Status;
                LocalStatusBO.SaveChanges();
                OnNewStatus(this, eventargs);
            }
        }

        private DirectMessage GetTOBDMFromTSDM(TwitterDirectMessage tdm)
        {
            DirectMessage dm = new DirectMessage();
            dm.AccountId = Acc.Id;
            dm.AccountsId = Acc.Id;
            //dm.Account = Acc;
            dm.CreationTime = DateTime.Now;         
            dm.SenderId = tdm.SenderId;
            dm.UserScreenName = tdm.SenderScreenName;
            dm.UserProfileId = tdm.Sender.Id;
            //dm.RecipientUserProfileId = tdm.Recipient.Id;
            dm.Text = tdm.Text;
            dm.TwitterId = tdm.Id;
            dm.TwitterCreatedDate = tdm.CreatedDate;
            dm.SortableColumn = tdm.CreatedDate;
           
            UserProfile tup = GetUserProfile(tdm.Sender);
            if (tup != null)
            {
                dm.UserProfileId = tup.Id;
            }
            tup = GetUserProfile(tdm.Recipient);
            if (tup != null)
            {
                //dm.RecipientUserProfileId = tup.Id;
                dm.SearchableString = (dm.Text + " " + tdm.Sender.ScreenName + " " + tdm.Sender.Name).ToLower();
            }
            if (MarkAsRead.Instance.IsMinimized == true || MarkAsRead.Instance.IsNotActive == true)
            {
                dm.IsMarkAsRead = false;
            }
            else
            {
                dm.IsMarkAsRead = true;
            }
            
            return dm;
        }
        
        private Status GetTOBStatusFromTSStatus(TwitterStatus ts)
        {
            //Return back null if Status is already part of the stream.
            //if (LocalStatusBO.Get(s => s.TwitterStatusId == ts.Id) != null)
            //    return null;

            Status retObj = new Status();

            retObj.AccountId = Acc.Id;
            retObj.AccountsId = Acc.Id;            
            // Why do we have creationdate? Is this the local time/date?
            retObj.CreationDate = DateTime.Now;
            retObj.InReplyToUserId = ts.InReplyToUserId;
            retObj.InReplyToStatusId = ts.InReplyToStatusId;
            retObj.IsFavorited = ts.IsFavorited;
            retObj.IsTruncated = ts.IsTruncated;
            retObj.Source = ts.Source;
            retObj.Text = ts.Text;
            retObj.TwitterCreatedDate = ts.CreatedDate;
            retObj.SortableColumn = ts.CreatedDate;
            retObj.TwitterStatusId = ts.Id;
            retObj.TwitterUserId = ts.User.Id;
            if (ts.Location != null && ts.Location.Coordinates != null)
            {
                retObj.Latitude = ts.Location.Coordinates.Latitude;
                retObj.Longitude = ts.Location.Coordinates.Longitude;
            }
            //else
            //{
            //    retObj.Latitude = 0;
            //    retObj.Longitude = 0;
            //}
            UserProfile tup = GetUserProfile(ts.User);
            
            if(tup != null)
            {
                retObj.UserProfileId = tup.Id;
                retObj.SearchableString = (retObj.Text + " " + ts.User.ScreenName + " " + ts.User.Name).ToLower();
            }
            
            if (MarkAsRead.Instance.IsMinimized == true || MarkAsRead.Instance.IsNotActive == true)
            {
                retObj.IsRead = false;
                retObj.IsMarkAsRead = false;
            }
            else
            {
                retObj.IsRead = true;
                retObj.IsMarkAsRead = true;
            }
            return retObj;
        }              

        public override void UploadNewStatus(string message)
        {
            TwitterService request = AuthenticateUser(Acc);        
            TwitterStatus status = request.SendTweet(message);            
            if (status == null)
            {
                Logger.TOBLogger.WriteDebugInfo(request.Response.RequestUri.ToString());
            }
        }           
   
        public override void UploadNewDM(string username, string message)
        {
            TwitterService request = AuthenticateUser(Acc);
            TwitterDirectMessage directMessage = request.SendDirectMessage(username, message);

            if (directMessage == null)
            {
                Logger.TOBLogger.WriteDebugInfo(request.Response.RequestUri.ToString());
            }
        }

        public override void ReplyTweet(string message)
        {
            TwitterService request = AuthenticateUser(Acc);
            TwitterStatus status = request.SendTweet(message);
            if (status == null)
            {
                Logger.TOBLogger.WriteDebugInfo(request.Response.RequestUri.ToString());
            }
        }
                
        public override void DeleteStatus(long messageId)
        {
            TwitterService request = AuthenticateUser(Acc);
            TwitterStatus status = request.DeleteTweet(messageId);
            
            if (status == null)
            {
                Logger.TOBLogger.WriteDebugInfo(request.Response.RequestUri.ToString());
            }

            //Remove it from the database
            Status tobStatus = LocalStatusBO.Get(s=>s.TwitterStatusId == messageId);
            LocalStatusBO.Delete(tobStatus);
            LocalStatusBO.SaveChanges();
        }

        public override void DeleteDM(long directMessageId)
        {
            TwitterService request = AuthenticateUser(Acc);
            TwitterDirectMessage directMessage = request.DeleteDirectMessage(directMessageId);
            
            //Remove it from the database
            DirectMessage dms = LocalDirectMessageBO.Get(d => d.TwitterId == directMessageId);
            LocalDirectMessageBO.Delete(dms);
            LocalDirectMessageBO.SaveChanges();            
        }

        public override List<TwitterUser> QueryUsers(string query)
        {
            TwitterService request = AuthenticateUser(Acc);

            var userColl = request.SearchForUser(query);

            if (userColl == null)
            {
                Logger.TOBLogger.WriteDebugInfo(request.Response.RequestUri.ToString());

                return null;
            }

            return userColl.ToList();
        }

        public override List<TwitterSearchStatus> QuerySearchResults(string query)
        {
            TwitterService request = AuthenticateUser(Acc);

            var results = request.Search(query, 100);

            if (results == null)
            {
                Logger.TOBLogger.WriteDebugInfo(request.Response.RequestUri.ToString());
                return null;
            }

            return results.Statuses.ToList();
        }

        public override TwitterList CreatePublicLists(string username, string listName)
        {
            TwitterService request = AuthenticateUser(Acc);
            var twitterList = request.CreateList(username, listName, "", "public");
            return twitterList;           
        }

        public override TwitterList CreatePrivateLists(string username, string listName)
        {
            TwitterService request = AuthenticateUser(Acc);
            var twitterList = request.CreateList(username, listName, "", "private");
            return twitterList;
        }

        public override void AddMemberToList(string username, long listId, long userId)
        {
            TwitterService request = AuthenticateUser(Acc);
            var member = request.AddListMember(username, listId.ToString(), userId);
        }

        public override void RemoveMemberFromList(string username, long listId, long userId)
        {
            TwitterService request = AuthenticateUser(Acc);
            var removeUser = request.RemoveListMember(username, listId.ToString(), userId);
        }

        public override bool IsMemberOfList(string username, long listId, long userId)
        {
            TwitterService request = AuthenticateUser(Acc);
            bool returnVal = true;
            var user = request.VerifyListMembership(username, listId.ToString(), userId);
            if (user != null)
            {
                returnVal = true;
            }
            else
            {
                returnVal = false;
            }
           return returnVal; 
        }

        public override List<UserProfile> GetMembersFromList(string username, long listId)
        {
            TwitterService request = AuthenticateUser(Acc);
            List<TwitterUser> usersList = request.ListListMembers(username, listId.ToString());
            List<UserProfile> UserProfileList = new List<UserProfile>();
            foreach (TwitterUser twitterUser in usersList)
            {
                UserProfile tup = GetTOBUserFromTSUser(twitterUser, null);
                UserProfileList.Add(tup);
            }
            return UserProfileList;            
        }

        public override List<TwitterStatus> GetStatusesFromList(string userName, long listId)
        {
            TwitterService request = AuthenticateUser(Acc);
            var lst = request.ListTweetsOnList(userName, listId.ToString(), 190);
            return lst == null? null: lst.ToList();
        }

        private void DownloadTwitterLists()
        {
            OnNewList(this, GetLists());
        }

        public override List<TwitterListExtended> GetLists()
        {
            TwitterService request = AuthenticateUser(Acc);
            List<TwitterListExtended> twitterLists = new List<TwitterListExtended>();
            
            var subsReq = request.ListListsFor(Acc.Username.ToLower());

            if (subsReq == null)
            {
                TOB.Logger.TOBLogger.WriteDebugInfo(request.Response.RequestUri.ToString());
            }
            else
            {
                foreach (TwitterList tl in subsReq)
                {
                    TwitterListExtended tle = new TwitterListExtended(tl);
                    tle.UserAccount = Acc;
                    twitterLists.Add(tle);
                }
            }

            //request = AuthenticateUser(Acc);

            var listsReq = request.ListListSubscriptionsFor(Acc.Username.ToLower());

            if (listsReq == null)
            {
                TOB.Logger.TOBLogger.WriteDebugInfo(request.Response.RequestUri.ToString());
            }
            else
            {
                foreach (TwitterList tl in listsReq)
                {
                    TwitterListExtended tle = new TwitterListExtended(tl);
                    tle.UserAccount = Acc;
                    twitterLists.Add(tle);
                }
            }

            return twitterLists;
        }

        public override void DeleteList(string userName, long listId)
        {
            TwitterService request = AuthenticateUser(Acc);
            var deleteList = request.DeleteList(userName, listId.ToString());            
        }

        //public override void GetMembersOfList(string userName, long listId)
        //{
        //    IFluentTwitter request = AuthenticateUser(Acc);
        //    var getMember = request.Lists().GetMembersOf(userName, listId).AsJson().Request().AsUsers();
        //}

        private UserProfile GetTOBUserFromTSUser(TwitterUser profile, UserProfile updateProfile)
        {
            UserProfile localProfile = null;

            if (profile != null)
            {
                if (updateProfile != null)
                {
                    localProfile = updateProfile;
                }
                else
                {
                    localProfile = new UserProfile();
                }
                
                localProfile.TwitterCreatedDate = profile.CreatedDate;
                localProfile.Description = profile.Description;
                localProfile.FavouritesCount = profile.FavouritesCount;
                localProfile.FollowersCount = profile.FollowersCount;
                localProfile.FriendsCount = profile.FriendsCount;
                localProfile.UserId = profile.Id;
                localProfile.IsProfileBackgroundTiled = profile.IsProfileBackgroundTiled;
                localProfile.IsProtected = profile.IsProtected;
                //localProfile.IsFollowing = profile;
                localProfile.Location = profile.Location;
                localProfile.Name = profile.Name;
                localProfile.ProfileBackgroundColor = profile.ProfileBackgroundColor;
                localProfile.ProfileBackgroundImageUrl = profile.ProfileBackgroundImageUrl;
                localProfile.ProfileImageUrl = profile.ProfileImageUrl;
                localProfile.ProfileLinkColor = profile.ProfileLinkColor;
                localProfile.ProfileSidebarBorderColor = profile.ProfileSidebarBorderColor;
                localProfile.ProfileSidebarFillColor = profile.ProfileSidebarFillColor;
                localProfile.ProfileTextColor = profile.ProfileTextColor;
                localProfile.ScreenName = profile.ScreenName;
                localProfile.StatusesCount = profile.StatusesCount;
                localProfile.TimeZone = profile.TimeZone;
                localProfile.UserUrl = profile.Url;
                localProfile.UtcOffset = profile.UtcOffset;
                //Associate current account id with either User's own profile or Friends User's profile
                localProfile.AccountId = Acc.Id;
                localProfile.LastUpdated = DateTime.Now;
                
            }
            return localProfile;

        }
        /// <summary>
        /// This function is only used to get the current user Profile related with the current account
        /// THIS CODE NEEDS TO BE CLEANED UP AND FIXED. 
        /// </summary>
        /// <returns></returns>
        public override UserProfile GetCurrentTOBUserProfile()
        {
            UserProfileBO twitterProfiles = LocalUserProfileBO;

            TwitterService request = AuthenticateUser(Acc);
            //Get current Account's user profile
            var currentUser = request.GetUserProfile();        
            //Check if the current user has been added to the UserProfile table.
            UserProfile localProfile = LocalUserProfileBO.Get(t => t.UserId == currentUser.Id && t.AccountId == Acc.Id);
            if (localProfile == null )
            {
                // Download profile from Twitter server and insert into local DB
                //Refresh current user with the latest information from the twitter
                localProfile = GetTOBUserFromTSUser(currentUser, null);
                //IMPORTANT!!
                twitterProfiles.Insert(localProfile);
                twitterProfiles.SaveChanges();
            }

            return localProfile;
        }

        public override TwitterUser SearchUser(string userName)
        {
            TwitterService request = AuthenticateUser(Acc);
            TwitterUser tup = request.GetUserProfileFor(userName);
            return tup.ScreenName == null?null:tup;
        }

        public override List<TwitterSearchStatus> GetStatuses(string userName)
        {           
            TwitterService request = AuthenticateUser(Acc);

            return request.Search(userName, 100).Statuses.ToList();                
        }

        /// <summary>
        /// This function is used to get the current user Profile related with the current account as
        /// well as related users profile ( Friends, Followers Followings etc);
        /// THIS CODE NEEDS TO BE CLEANED UP AND FIXED.
        /// </summary>
        /// <param name="downloadIfNeeded"></param>
        /// <returns></returns>
        public override UserProfile GetUserProfile(TwitterUser tsUser)
        {
            UserProfileBO tupBO = LocalUserProfileBO;

            UserProfile localProfile = tupBO.Get(t => t.UserId == tsUser.Id && t.AccountId == Acc.Id);
            if (localProfile == null)
            {
                localProfile = GetTOBUserFromTSUser(tsUser, null);
                tupBO.Insert(localProfile);
                tupBO.SaveChanges();
            }
            else
            {
                if (localProfile.LastUpdated.GetValueOrDefault().AddMinutes(5) < DateTime.Now)
                {
                    localProfile = GetTOBUserFromTSUser(tsUser, localProfile);
                    tupBO.SaveChanges();
                }
            }
            
            return localProfile;
        }

        public override UserProfile GetUserProfile(string screenName)
        {
            UserProfile localProfile = LocalUserProfileBO.Get(t => t.ScreenName == screenName && t.AccountId == Acc.Id);

            // Download profile from Twitter server and insert into local DB
            TwitterUser profile = null;
            
            if (localProfile == null)
            {
                TwitterService request = AuthenticateUser(Acc);
                profile = request.GetUserProfileFor(screenName);
            }
            
            if (profile == null)
            {
                Logger.TOBLogger.WriteDebugInfo("Cound not find Twitter user for screenName {0} ", screenName);
            }
            else
            {
                localProfile = GetTOBUserFromTSUser(profile, null);
                LocalUserProfileBO.Insert(localProfile);
                LocalUserProfileBO.SaveChanges();
            }

            return localProfile;
        }

        public override void BlockUser(object entitybase)
        {
            TwitterService request = AuthenticateUser(Acc);
            
            if (entitybase is Status)
            {
                Status status = entitybase as Status;
                request.BlockUser(status.TwitterUserId.Value);
            }
            else if (entitybase is DirectMessage)
            {
                DirectMessage directMessage = entitybase as DirectMessage;
                request.BlockUser(directMessage.UserProfile.UserId.Value);
            }
            else if (entitybase is TwitterStatus)
            {
                TwitterStatus tsStatus = entitybase as TwitterStatus;
                request.BlockUser(tsStatus.User.Id);
            }

            if (request.Response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Logger.TOBLogger.WriteDebugInfo("Error occured while Blocking user - " + request.Response.RequestUri.ToString());
                MessageNotifier.Instance.NotifyMessage("Error - BlockUser - " + request.Response.RequestUri.ToString());
            }
        }

        public override void UnFollowUser(object entitybase)
        {
            TwitterService request = AuthenticateUser(Acc);
            
            if (entitybase is Status)
            {
                Status status = entitybase as Status;
                request.UnfollowUser(status.TwitterUserId.Value);
            }
            else if (entitybase is DirectMessage)
            {
                DirectMessage directMessage = entitybase as DirectMessage;
                request.UnfollowUser(directMessage.UserProfile.UserId.Value);
            }
            else if (entitybase is TwitterStatus)
            {
                TwitterStatus tsStatus = entitybase as TwitterStatus;
                request.UnfollowUser(tsStatus.User.Id);
            }
            else if (entitybase is UserProfile)
            {
                UserProfile userProfile = entitybase as UserProfile;
                request.UnfollowUser(userProfile.UserId.Value);
            }

            if (request.Response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Logger.TOBLogger.WriteDebugInfo("Error occured while unfollowing user - "+request.Response.RequestUri.ToString());
                MessageNotifier.Instance.NotifyMessage("Error - UnfollowUser - " + request.Response.RequestUri.ToString());
            }
        }

        public override void ReportSpamUser(object entitybase)
        {
            TwitterService request = AuthenticateUser(Acc);
            
            //if (entitybase is Status)
            //{
            //    Status status = entitybase as Status;
            //    request.
            //}
            //else if (entitybase is DirectMessage)
            //{
            //    DirectMessage directMessage = entitybase as DirectMessage;
            //    response = request.ReportSpam().ReportSpammer((int)directMessage.UserProfile.UserId).AsJson().Request();
            //}
            //else if (entitybase is TwitterStatus)
            //{
            //    TwitterStatus tsStatus = entitybase as TwitterStatus;
            //    response = request.ReportSpam().ReportSpammer(tsStatus.User.Id).AsJson().Request();
            //}

            //if (response != null && response.IsTwitterError)
            //{
            //    Logger.TOBLogger.WriteDebugInfo("Error occured while ReportSpam - " + response.AsError().ErrorMessage);
            //    MessageNotifier.Instance.NotifyMessage("Error - ReportSpamUser - " + response.AsError().ErrorMessage);
            //}
        }

        public override void MarkAsFavorite(object entitybase)
        {
            TwitterService request = AuthenticateUser(Acc);
            
            if (entitybase is Status)
            {
                Status status = entitybase as Status;
                request.FavoriteTweet(status.TwitterStatusId.Value);
                
                StatusBO statusBO = LocalStatusBO;
                Status tweetStatus = statusBO.GetAll().Where(stat => stat.TwitterStatusId == status.TwitterStatusId).FirstOrDefault();
                tweetStatus.IsFavorited = true;
                statusBO.SaveChanges();
            }
            else if (entitybase is TwitterStatus)
            {
                TwitterStatus tsStatus = entitybase as TwitterStatus;
                request.FavoriteTweet(tsStatus.Id);

                StatusBO statusBO = LocalStatusBO;
                Status tweetStatus = statusBO.GetAll().Where(stat => stat.TwitterStatusId == tsStatus.Id).FirstOrDefault();
                tweetStatus.IsFavorited = true;
                statusBO.SaveChanges();
            }

            if (request.Response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Logger.TOBLogger.WriteDebugInfo("Error occured while setting Favorite - " + request.Response.RequestUri.ToString());
                MessageNotifier.Instance.NotifyMessage("Error - SetFavorite - " + request.Response.RequestUri.ToString());
            }
        }

        public override void FollowUser(string userName)
        {
            TwitterService request = AuthenticateUser(Acc);
            request.FollowUser(userName);
        }

        public override void StartUpdateDownloads()
        {
            StartUpdateDownloads(60000);
        }

        public override void StartUpdateDownloads(double updatePeriod)
        {
            if (!_updateTimer.Enabled)
            {
                // Add random factor to protect against DOS errors
                _updateTimer.Interval = updatePeriod + _randomGen.Next(-1000, 1000);
                _updateTimer.Start();
                _timerEvent += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
                _timerEvent.BeginInvoke(null, null, null, null);
            }
            else
            {
                //Already updating ... Ignore request.
            }
        }

        public override void StopUpdateDownloads()
        {
            if (_updateTimer.Enabled)
            {
                _updateTimer.Stop();
                _timerEvent -= new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    if (_updateTimer.Enabled)
                    {
                        _updateTimer.Stop();
                    }
                    _updateTimer.Elapsed -= new System.Timers.ElapsedEventHandler(Timer_Elapsed);
                    _timerEvent -= new System.Timers.ElapsedEventHandler(Timer_Elapsed);
                    _updateTimer.Dispose();
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            Disposed = true;
        }

        public override void Save()
        {
            LocalStatusBO.SaveChanges();
            LocalDirectMessageBO.SaveChanges();
        }
       
    }
}
