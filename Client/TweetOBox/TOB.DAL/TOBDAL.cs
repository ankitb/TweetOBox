using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOB.Entities;
using System.Data.Linq.Mapping;

namespace TOB.DAL
{

    public partial class TOB : System.Data.Linq.DataContext
    {

        #region Extensibility Method Definitions
        partial void OnCreated();
        partial void InsertAccount(Account instance);
        partial void UpdateAccount(Account instance);
        partial void DeleteAccount(Account instance);
        partial void InsertAccountFilterMapping(AccountFilterMapping instance);
        partial void UpdateAccountFilterMapping(AccountFilterMapping instance);
        partial void DeleteAccountFilterMapping(AccountFilterMapping instance);
        partial void InsertAccountType(AccountType instance);
        partial void UpdateAccountType(AccountType instance);
        partial void DeleteAccountType(AccountType instance);
        partial void InsertDirectMessage(DirectMessage instance);
        partial void UpdateDirectMessage(DirectMessage instance);
        partial void DeleteDirectMessage(DirectMessage instance);
        partial void InsertPanelType(PanelType instance);
        partial void UpdatePanelType(PanelType instance);
        partial void DeletePanelType(PanelType instance);
        partial void InsertSavedFilter(SavedFilter instance);
        partial void UpdateSavedFilter(SavedFilter instance);
        partial void DeleteSavedFilter(SavedFilter instance);
        partial void InsertSavedPluginView(SavedPluginView instance);
        partial void UpdateSavedPluginView(SavedPluginView instance);
        partial void DeleteSavedPluginView(SavedPluginView instance);
        partial void InsertSavedSearch(SavedSearch instance);
        partial void UpdateSavedSearch(SavedSearch instance);
        partial void DeleteSavedSearch(SavedSearch instance);
        partial void InsertStatus(Status instance);
        partial void UpdateStatus(Status instance);
        partial void DeleteStatus(Status instance);
        partial void InsertTOBRegister(TOBRegister instance);
        partial void UpdateTOBRegister(TOBRegister instance);
        partial void DeleteTOBRegister(TOBRegister instance);
        partial void InsertUserProfile(UserProfile instance);
        partial void UpdateUserProfile(UserProfile instance);
        partial void DeleteUserProfile(UserProfile instance);
        #endregion

        public TOB(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) :
            base(connection, mappingSource)
        {
            OnCreated();
        }

        public TOB(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) :
            base(connection, mappingSource)
        {
            OnCreated();
        }

        public System.Data.Linq.Table<Account> Accounts
        {
            get
            {
                return this.GetTable<Account>();
            }
        }

        public System.Data.Linq.Table<AccountFilterMapping> AccountFilterMappings
        {
            get
            {
                return this.GetTable<AccountFilterMapping>();
            }
        }

        public System.Data.Linq.Table<AccountType> AccountTypes
        {
            get
            {
                return this.GetTable<AccountType>();
            }
        }

        public System.Data.Linq.Table<DirectMessage> DirectMessages
        {
            get
            {
                return this.GetTable<DirectMessage>();
            }
        }

        public System.Data.Linq.Table<PanelType> PanelTypes
        {
            get
            {
                return this.GetTable<PanelType>();
            }
        }

        public System.Data.Linq.Table<SavedFilter> SavedFilters
        {
            get
            {
                return this.GetTable<SavedFilter>();
            }
        }

        public System.Data.Linq.Table<SavedPluginView> SavedPluginViews
        {
            get
            {
                return this.GetTable<SavedPluginView>();
            }
        }

        public System.Data.Linq.Table<SavedSearch> SavedSearches
        {
            get
            {
                return this.GetTable<SavedSearch>();
            }
        }

        public System.Data.Linq.Table<Status> Status
        {
            get
            {
                return this.GetTable<Status>();
            }
        }

        public System.Data.Linq.Table<TOBRegister> TOBRegisters
        {
            get
            {
                return this.GetTable<TOBRegister>();
            }
        }

        public System.Data.Linq.Table<UserProfile> UserProfiles
        {
            get
            {
                return this.GetTable<UserProfile>();
            }
        }
    }
}
