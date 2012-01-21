using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOB.Entities;
using System.Windows;
using System.Windows.Controls;

namespace TOB.Plugin
{
    /// <summary>
    /// Interface to the TweetOBox Plugin. All TOB Plugins need to inherit this interface.
    /// </summary>
    //public interface ITOBPlugin
    public abstract class ITOBPlugin
    {
        /// <summary>
        /// Function to process all incoming TOBEntityBase objects. During initialization all existing objects are passed to this function. 
        /// Sequentially, as new updates arrive, function is called with the new objects.
        /// </summary>
        /// <param name="inStreamList">List of objects passed down from TOB. In the case of Twitter, these would could be Status or DirectMessage </param>
        /// <returns>List of objects to be rendered in TOB Panel</returns>
        public abstract void ProcessInStream(List<TOBEntityBase> inStreamList);
        public abstract void ProcessSearchStream(List<Search> searchStream);
        //// <summary>
        //// Function for getting the DataTemplate that defines the rendering schema for all objects returned from this plugin.  
        //// </summary>
        //// <returns></returns>
        //DataTemplate GetItemListDataTemplate();

        public PanelInteractionDelegate RemoveControlFromPanel;
        public PanelInteractionDelegate AddControlToPanel;
    }

    public delegate void PanelInteractionDelegate(UserControl obj);
}
