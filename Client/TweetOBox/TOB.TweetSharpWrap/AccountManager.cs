using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOB.BLL;
using TOB.Entities;
using TOB.Logger;
using TweetSharp;

namespace TOB.TweetSharpWrap
{
    //Hiren: This will be called from the MainWindow finished the ui rendering. 
    //Start method is required to call
    public class AccountManager : IDisposable
    {
        private List<TOBBaseObject> _tobObjects;
        private static AccountManager __this;
        private static object _syncObject = new object();
        private AccountBO _accountBO;
        private List<Account> _cachedAccountList = null;

        private AccountManager()
        {
            _tobObjects = new List<TOBBaseObject>();
            _accountBO = new AccountBO();

            _cachedAccountList = _accountBO.GetAll();

            //Populate and create TOBBaseObject list from account object
            PopulateTOBBaseObjectList();
        }

        private void PopulateTOBBaseObjectList()
        {
            foreach (Account acc in _cachedAccountList)
            {
                //Check to see if the tobbase exists for the corresponding account. If not create it.
                TOBBaseObject tobBase = _tobObjects.Where(t => t.Acc.Id == acc.Id).FirstOrDefault();
                if (tobBase == null)
                {
                    TOBBaseObject tob = CreateTOBObject(acc);
                    _tobObjects.Add(tob);
                }
            }
        }
        /// <summary>
        /// Readonly getter for the getting the existing TOBObjects
        /// </summary>

        public List<TOBBaseObject> TobObjects
        {
            get { return _tobObjects; }
        }

        public List<Account> GetCurrentAccounts()
        {
            return _cachedAccountList;
            //return _accountBO.GetAll();
        }

        /// <summary>
        /// Add an account
        /// </summary>
        /// <param name="acc">Add account to be added</param>
        /// <param name="downloadIfNeeded">After adding an account if tweets to be downloaded or not</param>
        public void AddAccount(Account acc, bool downloadIfNeeded)
        {
            Account tempAcc = _accountBO.Get(ac => ac.Username == acc.Username);
            if (tempAcc == null)
            {
                _accountBO.Insert(acc);
                _accountBO.SaveChanges();

                _cachedAccountList = _accountBO.GetAll();

                //Call this function to create a TOBBaseObject corresponding to newly added account object
                PopulateTOBBaseObjectList();
                //Call Start to download all the tweets from the newly added account.
                if (downloadIfNeeded)
                {
                    Start();
                }
            }
            else
            {
                throw new AuthFailureException("User " + acc.Username + " already exists!");
            }
        }

        public void DeleteAccount(Account acc)
        {
            //Temp fix : Need to improve.            
            UserProfileBO userProfileBO = new UserProfileBO();
            List<UserProfile> userProfileList = userProfileBO.GetAll().Where(user => user.AccountId == acc.Id).ToList();
            if (userProfileList.Count > 1)
            {
                userProfileBO.DeleteAll(userProfileList);
                userProfileBO.SaveChanges();
            }
            SavedFilterBO savedFilterBO = new SavedFilterBO();
            List<SavedFilter> savedFilterList = savedFilterBO.GetAll();
            foreach (SavedFilter sf in savedFilterList)
            {
                AccountFilterMappingBO accFilterBO = new AccountFilterMappingBO();
                AccountFilterMapping accFilter = accFilterBO.Get(af => af.FilterId == sf.Id);
                accFilterBO.Delete(accFilter);
                accFilterBO.SaveChanges();
            }
            foreach (SavedFilter sf in savedFilterList)
            {
                savedFilterBO.Delete(sf);
                savedFilterBO.SaveChanges();
            }
            SavedSearchBO savedSearchBO = new SavedSearchBO();
            List<SavedSearch> savedSearchList = savedSearchBO.GetAll();
            if (savedSearchList.Count > 1)
            {
                savedSearchBO.DeleteAll(savedSearchList);
                savedSearchBO.SaveChanges();
            }

            _accountBO.Delete(acc);
            _accountBO.SaveChanges();

            _cachedAccountList = _accountBO.GetAll();

            //Stop Temporary
            Stop();

            //Get TOBBaseOBject
            TOBBaseObject baseObj = GetTOBObject(acc);
            if (baseObj != null)
            {
                _tobObjects.Remove(baseObj);
                baseObj = null;
            }
            //Again Start the update operation
            Start();
        }

        public TOBBaseObject GetTOBObject(Account acc)
        {
            if (acc == null)
            {
                throw new ArgumentNullException("Account","Pease specify valid Account object");
            }
            if ( acc.Id == 0)
            {
                throw new ArgumentException("Opps. Account does not have valid accountid");
            }
            if (_tobObjects == null || _tobObjects.Count == 0)
            {
                throw new NullReferenceException("AccountManger is not initialized property. Could not find any account to Manage.");
            }
            
            TOBBaseObject tobBase = _tobObjects.Where(t => t.Acc.Id == acc.Id).FirstOrDefault();
            if (tobBase == null)
            {
                throw new Exception("Oops!!. Could find account with accountId = " + acc.Id);
            }
            return tobBase;
        }

        public TOBBaseObject GetTOBObjectUser(object entitybase)
        {
            if (_tobObjects.Count == 1)
            {
                return _tobObjects[0];
            }

            TOBBaseObject tobBase = null;
            if (entitybase is Status)
            {
                Status status = entitybase as Status;
                tobBase = _tobObjects.Where(t => t.Acc.Id == status.AccountId).FirstOrDefault();
                if (tobBase == null)
                {
                    throw new Exception("Oops!!. Could find account with accountId = " + status.AccountId);
                }
            }
            else if(entitybase is DirectMessage)
            {
                DirectMessage directMessage = entitybase as DirectMessage;
                tobBase = _tobObjects.Where(t => t.Acc.Id == directMessage.AccountId).FirstOrDefault();
            }
            //else if (entitybase is TwitterStatus)
            //{
            //    TwitterStatus ts = entitybase as TwitterStatus;
            //    tobBase = _tobObjects.Where(t => t.Acc.Id == ts.).FirstOrDefault();
            //}

            return tobBase;
        }


        private TOBBaseObject CreateTOBObject(Account acc)
        {
            TOBBaseObject tobBaseObject = null;
            switch (acc.AccountType)
            {
                case 1:
                    tobBaseObject = new TOBTwitterO(acc);
                    break;
                case 2:
                    tobBaseObject = new TOBTwitterO(acc);
                    break;
            }
            return tobBaseObject;
        }

        public void Start()
        {
            foreach (TOBBaseObject tobBase in _tobObjects)
            {
                tobBase.StartUpdateDownloads();
            }
            TOBLogger.WriteInfo("TOB Account manager has started successfully.");
        }

        public void Stop()
        {
            foreach (TOBBaseObject tobBase in _tobObjects)
            {
                tobBase.StopUpdateDownloads();
            }
        }
        
        public static AccountManager Instance
        {
            get
            {
                //locking to avoid any race condition
                lock (_syncObject)
                {
                    if (__this == null)
                    {
                        __this = new AccountManager();
                    }
                }
                
                return __this;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (TOBBaseObject tobBase in _tobObjects)
            {
                tobBase.Dispose();
            }
        }

        #endregion
    }
}
