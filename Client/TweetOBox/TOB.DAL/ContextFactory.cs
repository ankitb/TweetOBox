using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Deployment.Application;
using TOB.Logger;

namespace TOB.DAL
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
            internal const string TOBMappingKey = "TOBMappingKey";
            internal const string TOBEntitiesAssemblyKey = "TOBEntitiesAssemblyKey";
        }

        static readonly MappingSource _TOBMappingSource;
        static bool _logError = true;

        static ContextFactory()
        {
            _TOBMappingSource = CreateTOBMappingSource();
        }

        /// <summary>
        /// Create Mapping Source
        /// </summary>
        /// <returns></returns>

        private static MappingSource CreateTOBMappingSource()
        {
            string map = ConfigurationManager.AppSettings[Constants.TOBMappingKey];
            if (String.IsNullOrEmpty(map))
            {
                throw new ConfigurationErrorsException("No mapping xml file specified in configuration key " + Constants.TOBMappingKey);
            }
            string assemblyname = ConfigurationManager.AppSettings[Constants.TOBEntitiesAssemblyKey];
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

        public static TOB CreateTOBDataContext()
        {
            return new TOB(CreateConnectionString(), _TOBMappingSource);
        }

        /// <summary>
        /// Create a connection string
        /// </summary>
        /// <returns></returns>

        static string CreateConnectionString()
        {
            string dbFilePathlocal = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\DB\\TweetOBoxDB.sdf";
            string dbFilePath = null;
            bool isInClickOnce = false;

            try
            {
                dbFilePath = ApplicationDeployment.CurrentDeployment.DataDirectory + "\\DB\\TweetOBoxDB.sdf";
                isInClickOnce = true;
            }
            catch (InvalidDeploymentException ide)
            {
                if (_logError)
                {
                    TOBLogger.WriteDebugInfo("Not in ClickOnce deployed mode ... Using local DB file instead");
                }               
                dbFilePath = dbFilePathlocal;
            }
            
            if ( !File.Exists(dbFilePath))
            {
                TOBLogger.WriteDebugInfo("ERROR - DB file not found at " + dbFilePath);
                TOBLogger.WriteDebugInfo("Checking local path - "+dbFilePathlocal);

                if (File.Exists(dbFilePathlocal))
                {
                    if (isInClickOnce)
                    {
                        TOBLogger.WriteDebugInfo("Copying over local file to " + dbFilePath);
                        File.Copy(dbFilePathlocal, dbFilePath);
                    }
                    else
                    {
                        dbFilePath = dbFilePathlocal;
                    }
                }
                else
                {
                    throw new FileNotFoundException("Opps missing database.", dbFilePath);
                }
            }

            _logError = false;

            string ConnectionString = string.Format("Persist Security Info = False; Data Source={0}; Password={1};",dbFilePath,"12345");
            return ConnectionString;
        }
    }
}
