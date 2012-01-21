using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;

namespace TOBKPI.BLL.TOBKPI.DAL
{

    [System.Data.Linq.Mapping.DatabaseAttribute(Name = "TOBKPI")]
    public partial class TOBServiceDB : System.Data.Linq.DataContext
    {

        private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();

        #region Extensibility Method Definitions
        partial void OnCreated();
        partial void InsertTOBClientRegistration(TOBClientRegistration instance);
        partial void UpdateTOBClientRegistration(TOBClientRegistration instance);
        partial void DeleteTOBClientRegistration(TOBClientRegistration instance);
        partial void InsertTOBClientTimeTracker(TOBClientTimeTracker instance);
        partial void UpdateTOBClientTimeTracker(TOBClientTimeTracker instance);
        partial void DeleteTOBClientTimeTracker(TOBClientTimeTracker instance);
        #endregion

        public TOBServiceDB(string connection) :
            base(connection, mappingSource)
        {
            OnCreated();
        }

        public TOBServiceDB(System.Data.IDbConnection connection) :
            base(connection, mappingSource)
        {
            OnCreated();
        }

        public TOBServiceDB(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) :
            base(connection, mappingSource)
        {
            OnCreated();
        }

        public TOBServiceDB(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) :
            base(connection, mappingSource)
        {
            OnCreated();
        }

        public System.Data.Linq.Table<TOBClientRegistration> TOBClientRegistrations
        {
            get
            {
                return this.GetTable<TOBClientRegistration>();
            }
        }

        public System.Data.Linq.Table<TOBClientTimeTracker> TOBClientTimeTrackers
        {
            get
            {
                return this.GetTable<TOBClientTimeTracker>();
            }
        }
    }
}
