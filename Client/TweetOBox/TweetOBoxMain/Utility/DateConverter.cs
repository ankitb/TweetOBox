using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using TOB.Entities;

namespace TweetOBoxMain.Utility
{
    public class DateConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string returnVal = string.Empty;
            try
            {                
                if (value is Status)
                {
                    var status = value as Status;
                    returnVal = Show(System.Convert.ToDateTime(status.TwitterCreatedDate));
                }
                else if (value is DirectMessage)
                {
                    var directMessage = value as DirectMessage;
                    returnVal = Show(System.Convert.ToDateTime(directMessage.TwitterCreatedDate));
                }               
            }
            catch (Exception ex)
            {
                TOB.Logger.TOBLogger.WriteDebugInfo(ex.Message);
            }
            return returnVal;
        }
                
        public string Show(DateTime twitterCreatedDate)
        {
            string returnValue = "";
            int diff = System.Convert.ToInt32(DateTime.UtcNow.Subtract(System.Convert.ToDateTime(twitterCreatedDate)).TotalSeconds);
            string text = string.Empty;
            if (diff < 60)
                text = "less than a minute ago";
            else if (diff < 120)
                text = "about a minute ago";
            else if (diff < (45 * 60))
                text = (diff / 60).ToString() + " minutes ago";
            else if (diff < (90 * 60))
                text = "about an hour ago";
            else if (diff < (24 * 60 * 60))
                text = "about " + (diff / 3600).ToString() + " hours ago";
            else if (diff < (48 * 60 * 60)) text = "1 day ago";
            else
                text = (diff / 86400).ToString() + " days ago";

            returnValue = string.Format("{0}", text);

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
