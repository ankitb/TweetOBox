using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace TweetOBoxMain.Utility
{
    public class TwitterStatusUrl : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string returnValue = "";
            try
            {
                TOB.Entities.Status status = value as TOB.Entities.Status;
                if (status != null)
                {
                    returnValue = "http://twitter.com/" + status.UserProfile.ScreenName + "/statuses/" + status.TwitterStatusId;
                }
            }
            catch(Exception ex)
            {
                TOB.Logger.TOBLogger.WriteDebugInfo("VALUE-"+value+" "+ex.ToString());
            }
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TOB.Logger.TOBLogger.WriteDebugInfo("FUNCTION NOT IMPLEMENTED");
            
            return null;

        }

        #endregion
    }
        
}
