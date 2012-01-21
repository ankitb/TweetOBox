using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOB.Utility
{
    public class CheckForDatabase
    {
        private CheckForDatabase()
        { }

        private static CheckForDatabase _instance;
        public static CheckForDatabase Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CheckForDatabase();

                return _instance;
            }
        }

        public void CheckDataContext()
        {
            if (TOB.DAL.ContextFactory.CreateTOBDataContext() == null)
            {
                throw new Exception();
            }
        }
    }

}
