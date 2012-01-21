using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOB.Plugin;
using System.IO;
using System.Reflection;
using System.Deployment.Application;

namespace TweetOBoxMain
{
    /// <summary>
    /// This Class manages all plugins that are used by TOB Panels.
    /// </summary>
    class PluginManager
    {
        //private System.Collections.Specialized.HybridDictionary _pluginCollection;
        private List<KeyValuePair<TOBPluginInfo, Type>> _pluginCollection;
        private static PluginManager _instance = null;
        private FileSystemWatcher _fsw;
        private FileSystemWatcher _fswExt;
        private DirectoryInfo _pluginDir;
        private DirectoryInfo _pluginDirExt;

        public PluginManager()
        {
            try
            {
                _pluginDir = new DirectoryInfo(ApplicationDeployment.CurrentDeployment.DataDirectory + "\\Plugins");
            }
            catch (InvalidDeploymentException ide)
            {
                _pluginDir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Plugins");
            }
            finally
            {
                string extPluginFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TweetOBox\\Plugins";
                if (!Directory.Exists(extPluginFolder))
                {
                    Directory.CreateDirectory(extPluginFolder);
                }

                _pluginDirExt = new DirectoryInfo(extPluginFolder);
            }

            if (!_pluginDir.Exists)
            {
                TOB.Logger.TOBLogger.Assert(true, "Failed to instantiate PluginManager - Plugin Directory does not exist - " + _pluginDir.FullName);
                return;
            }

            ConstructPluginCollection();
            
            _fsw = new FileSystemWatcher(_pluginDir.FullName);
            _fsw.Changed += new FileSystemEventHandler(PluginDir_Changed);
            
            _fswExt = new FileSystemWatcher(_pluginDirExt.FullName);
            _fswExt.Changed+=new FileSystemEventHandler(PluginDir_Changed);
        }

        private void ConstructPluginCollection()
        {
            Assembly pluginAsm;
            TOBPluginInfo[] pluginInfos;
            List<FileInfo> fileInfos = _pluginDir.GetFiles("*.dll", SearchOption.AllDirectories).ToList();
            
            fileInfos.AddRange(_pluginDirExt.GetFiles("*.dll", SearchOption.AllDirectories));

            _pluginCollection = new List<KeyValuePair<TOBPluginInfo, Type>>();

            foreach (FileInfo plugin in fileInfos)
            {
                pluginAsm = Assembly.LoadFile(plugin.FullName);

                foreach (Type pluginType in pluginAsm.GetTypes())
                {
                    pluginInfos = pluginType.GetCustomAttributes(typeof(TOBPluginInfo), true) as TOBPluginInfo[];
                    if (pluginInfos == null || pluginInfos.Count() == 0)
                    {
                        //TOB.Logger.TOBLogger.LogError("No TOBPluginInfo Attribute found in ITOBPlugin in file " + pluginType.Module.FullyQualifiedName);
                        continue;
                    }
                    else
                    {
                        try
                        {
                            TOBPluginInfo pInfo = pluginInfos.FirstOrDefault();
                            pInfo.PluginVersion = pluginAsm.GetName().Version.ToString();
                            _pluginCollection.Add(new KeyValuePair<TOBPluginInfo, Type>(pluginInfos.FirstOrDefault(), pluginType));
                        }
                        //TOBPluginInfo can throw exceptions on GUID parse
                        catch (Exception e)
                        {
                            TOB.Logger.TOBLogger.WriteInfo("ERROR - Creation of TOBPluginInfo failed for "+pluginType.Name+" in "+pluginAsm.FullName);
                        }
                    }
                }
            }

            return;
        }

        public List<KeyValuePair<TOBPluginInfo, Type>> GetPluginInfos()
        {
            return _pluginCollection;
        }
            

        public ITOBPlugin GetPluginObject(Type pluginType)
        {
            return (ITOBPlugin) Activator.CreateInstance(pluginType);
        }

        void PluginDir_Changed(object sender, FileSystemEventArgs e)
        {
            // Make plugin construction smarter in the future by adding/removing new plugins instead of refreshing the whole list
            ConstructPluginCollection();
        }

        public static PluginManager Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PluginManager();
                }

                return _instance;
            }
        }
    }
}
