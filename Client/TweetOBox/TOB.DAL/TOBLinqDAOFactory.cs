using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOB.DataInterfaces;

namespace TOB.DAL
{
    public class TOBLinqTOBFactory : IDAOFactory
    {
        #region IDAOFactory Members

        public IBaseDAO CreateBaseDAO()
        {
            return new BaseDAO();
        }

        #endregion
    }
}
