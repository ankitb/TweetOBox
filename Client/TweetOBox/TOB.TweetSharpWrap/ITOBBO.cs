using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOB.Entities;
using TweetSharp;

namespace TOB.TweetSharpWrap
{
    /// <summary>
    /// This would be an interface defines a TOB BaseObject. All objects will inherit this interface. 
    /// This will have a requirement for definition of an Account object. 
    /// </summary>
    public abstract class TOBBaseObject : IDisposable
    {
        private bool _disposed;

        protected bool Disposed
        {
          get { return _disposed; }
          set { _disposed = value; }
        }
        private Account _acc;
        public Account Acc
        {
            get { return _acc; }
            set { _acc = value; }
        }

        public delegate void StatusHandler(TOBBaseObject from, TOBEventArgs args);

        public event StatusHandler NewStatus;

        public event ListHandler NewList;
        public delegate void ListHandler(TOBBaseObject from, List<TwitterListExtended> twitterList);

        public TOBBaseObject(Account acc)
        {
            CryptoHelper cryptoHelper = new CryptoHelper();            
            acc.DecryptedPassword = cryptoHelper.Decrypt(acc.Password);
            _acc = acc;
        }
        /// <summary>
        /// Adding message to account
        /// </summary>
        public abstract void UploadNewStatus(string message);
        public abstract void UploadNewDM(string username,string message);
        public abstract void DeleteStatus(long messageId);
        public abstract UserProfile GetCurrentTOBUserProfile();
        public abstract void StartUpdateDownloads();
        public abstract void StartUpdateDownloads(double updatePeriod);
        public abstract void StopUpdateDownloads();
        public abstract void ReplyTweet(string message);
        public abstract void UnFollowUser(object entitybase);
        public abstract void BlockUser(object entitybase);
        public abstract void ReportSpamUser(object entitybase);
        public abstract void MarkAsFavorite(object status);
        public abstract void DeleteDM(long directMessageId);
        public abstract TwitterList CreatePublicLists(string username, string listName);
        public abstract TwitterList CreatePrivateLists(string username, string listName);
        public abstract List<TwitterListExtended> GetLists();
        public abstract void DeleteList(string userName, long listId);
        //public abstract void GetMembersOfList(string userName, long listId);
        public abstract List<TwitterStatus> GetStatusesFromList(string userName, long listId);
        public abstract void AddMemberToList(string username, long listId, long userId);
        public abstract void RemoveMemberFromList(string username, long listId, long userId);
        public abstract List<UserProfile> GetMembersFromList(string username, long listId);
        public abstract bool IsMemberOfList(string username, long listId, long userId);
        //This function is used to get the Related user profile. 
        //i.e Current account(user)'s friends, follower's or followings's profile.
        public abstract UserProfile GetUserProfile(TwitterUser tsUser);
        public abstract UserProfile GetUserProfile(string userName);
        public abstract List<TwitterUser> QueryUsers(string query);
        public abstract List<TwitterSearchStatus> QuerySearchResults(string query);
        public abstract void FollowUser(string userName);
        public abstract TwitterUser SearchUser(string userName);
        public abstract List<TwitterSearchStatus> GetStatuses(string userName);
        public abstract void Save();

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
           
        }

        protected void OnNewStatus(TOBBaseObject current,TOBEventArgs e)
        {
            if (NewStatus != null)
            {
                NewStatus(current,e);
            }
        }

        protected void OnNewList(TOBBaseObject current, List<TwitterListExtended> twitterList)
        {
            if (NewList != null)
            {
                NewList(current, twitterList);
            }
        }
        #endregion
    }
}
