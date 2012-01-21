using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Data;

namespace TOBKPI.BLL.TOBKPI.DAL
{
    /// <summary>
    /// Initializes the data context.
    /// The context factory optimizes DataContext creation by doing a one time
    /// load of the mapping file using a Singleton.
    /// </summary>
    /// <remarks>
    /// In enterprise applications that may have multiple deployment environments
    /// such as UAT, QA, Production the mapping assembly name and connection string
    /// can be stored in an application configuration file. The ContextFactory can 
    /// load off the config file.
    /// </remarks>
    public static class ContextFactory
    {
        static class Constants
        {
            //Proxio Central Connection Key
            internal const string TOBServiceDataConnectionString = "TOBServiceConnectionString";
            internal const string TOBServiceMappingSourceKey = "TOBMappingSource";
            internal const string MappingAssemblyKey = "MappingAssemblyKey";
        }
        static readonly MappingSource _tobserviceMappingSrc;

        static ContextFactory()
        {
            _tobserviceMappingSrc = CreateTOBMappingSource();
        }


        private static MappingSource CreateTOBMappingSource()
        {
            string map = ConfigurationManager.AppSettings[Constants.TOBServiceMappingSourceKey];
            if (String.IsNullOrEmpty(map))
            {
                throw new ConfigurationErrorsException("No mapping xml file specified in configuration key " + Constants.TOBServiceMappingSourceKey);
            }
            string assemblyname = ConfigurationManager.AppSettings[Constants.MappingAssemblyKey];
            Assembly a = Assembly.Load(assemblyname);
            Stream s = null;
            if (a != null)
            {
                s = a.GetManifestResourceStream(map);
            }
            if (s == null)
            {
                throw new InvalidOperationException(String.Format(@"The XML mapping file [{0}] 
                            must be an embedded resource in the TransferObjects project.", map));
            }
            return XmlMappingSource.FromStream(s);
        }

        public static TOBServiceDB CreateTOBServiceDataContext()
        {
            return new TOBServiceDB(CreateConnection(), _tobserviceMappingSrc);
        }

        static IDbConnection CreateConnection()
        {
            ConnectionStringSettings c = ConfigurationManager.ConnectionStrings[Constants.TOBServiceDataConnectionString];
            if (String.IsNullOrEmpty(c.ConnectionString))
            {
                throw new ConfigurationErrorsException("No connection string specified in configuration key " + Constants.TOBServiceMappingSourceKey);
            }
            return new SqlConnection(c.ConnectionString);
        }


    }
}
