using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime;
using TOB.Plugin;
using TOB.Entities;
using System.Collections;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;


namespace PluginSkeleton
{
    [TOBPluginInfo("PluginSkeleton", "Plugin Skeleton Sample", "Your Company Name",/* Unique GUID*/"{399A3529-D9D4-4042-8920-197A208F1E99}")]
    public class PluginSkeleton:ITOBPlugin
    {
        PluginData tsd;
        UserControl uc = null;
        public PluginSkeleton()
        {
            //Initialize Elements
            tsd = new PluginData();
            
        }//constructor ends


        #region ITOBPlugin Members

        public override void ProcessInStream(List<TOBEntityBase> inStreamList)
        {
            //This function is called whenever new feeds arrive
            foreach (TOBEntityBase tobEB in inStreamList)
            {
                if (tobEB.GetType() == typeof(Status))
                {
                   //Process Status
                }

                if (tobEB.GetType() == typeof(DirectMessage))
                {
                    //Process DirectMessage
                }
            }

            if (uc == null)
            {
                uc = CreatePluginSkeletonUC(tsd);
                AddControlToPanel(uc);
            }
            
            
            return;
        }


        public override void ProcessSearchStream(List<Search> inStreamList)
        {
            // obsolete
        }

        //Creates UserControl asynchronously
        private UserControl CreatePluginSkeletonUC(PluginData tsd)
        {
            PluginSkeletonUC newUC = null;

            //Needed to get into STA context.
            Action action = delegate()
            {
                newUC = new PluginSkeletonUC(tsd);
            };
            
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, action);

            return newUC;
        
        }

     
        #endregion

    }
    /**********************************/

    //This class is used to Store Data in a custom format as per the need of the plugin 
    public class PluginData
    {
        public Dictionary<string, int> tweetsData; //Custom DataStructure
        
        //constructor
        public PluginData()
        {
            tweetsData = new Dictionary<string, int>(); //
        }
        
    }

   

  
}
