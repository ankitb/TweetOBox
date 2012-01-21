using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOB.Plugin
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple= false)]
    public class TOBPluginInfo : Attribute
    {
        private string _pluginName;
        private string _pluginDescription;
        private string _pluginVersion;
        private string _pluginCreator;
        private Guid _pluginGuid;

        public TOBPluginInfo(string pluginName, string pluginDescription, string pluginCreator, string pluginGuid)
        {
            _pluginName = pluginName;
            _pluginDescription = pluginDescription;
            _pluginCreator = pluginCreator;
            _pluginGuid = new Guid(pluginGuid);
        }

        public Guid PluginGUID
        {
            get { return _pluginGuid; }
        }

        public string PluginName
        {
            get { return _pluginName; }
        }

        public string PluginVersion
        {
            set { _pluginVersion = value; }
            get { return _pluginVersion; }
        }

        public string PluginCreator
        {
            get { return _pluginCreator; }
        }

        public string PluginDescription
        {
            get { return _pluginDescription; }
        }
    }
}
