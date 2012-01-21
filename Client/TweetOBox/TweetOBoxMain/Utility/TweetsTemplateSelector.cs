using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using TOB.Entities;

namespace TweetOBoxMain.Utility
{
    public class TweetsTemplateSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            DataTemplate dataTemplate = null;
            try
            {
                TOBEntityBase tobEntityBase = item as TOBEntityBase;
                ContentPresenter pres = container as ContentPresenter;
                if (tobEntityBase is Status)
                {
                    dataTemplate = pres.FindResource("TweetsControlsDataTemplate") as DataTemplate;
                }
                else if (tobEntityBase is DirectMessage)
                {
                    dataTemplate = pres.FindResource("DirectMessageDataTemplate") as DataTemplate;
                }
                else if (item is TweetSharp.TwitterStatus)
                {
                    dataTemplate = pres.FindResource("GroupsStatusesDataTemplate") as DataTemplate;
                }
                else if (item is TweetSharp.TwitterSearchStatus)
                {
                    dataTemplate = pres.FindResource("SavedSearchDataTemplate") as DataTemplate;
                }
                else if (item is TweetSharp.TwitterUser)
                {
                    dataTemplate = pres.FindResource("QueryUserDataTemplate") as DataTemplate;
                }
                else
                {
                    TOB.Logger.TOBLogger.WriteDebugInfo("No Template found for " + item.GetType().ToString());
                }
            }
            catch (Exception ex)
            {
                TOB.Logger.TOBLogger.WriteDebugInfo(ex.Message);
            }
            return dataTemplate;
        }
    }
}
