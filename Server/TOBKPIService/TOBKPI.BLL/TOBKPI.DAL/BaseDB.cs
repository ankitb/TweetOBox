using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOBKPI.BLL.TOBKPI.DAL
{
    /// <summary>
    /// Abstract class for DAO. The purpose of this class is to initialize connection string 
    /// based on the country id. This functionality is common to each n every DAO classes. 
    /// So this has been made abstract base class. All DAO Classes should inherit this. Provided 
    /// they all dealing with country specific entities.
    /// </summary>
    public abstract class BaseDB
    {
        /// <summary>
        /// 
        /// </summary>
        protected TOBServiceDB _tobServiceDB;
        /// <summary>
        /// 
        /// </summary>        
        /// <summary>
        /// Baseclass constructor takes country id as the argument.
        /// It takes connection strings from the Central database country master table.
        /// </summary>
        /// <param name="countryid"></param>
        protected BaseDB()
        {

            _tobServiceDB = ContextFactory.CreateTOBServiceDataContext();
        }


        #region IDisposable Members

        public virtual void Dispose()
        {
            _tobServiceDB.Dispose();
        }

        #endregion
    }
}
