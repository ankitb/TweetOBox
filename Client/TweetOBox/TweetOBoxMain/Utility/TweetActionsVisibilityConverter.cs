using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using TOB.TweetSharpWrap;
using TOB.Entities;
using System.ComponentModel;

namespace TweetOBoxMain.Utility
{
    public class TweetActionsVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {           
                bool returnVal = true;
                TOB.Entities.Status status = value as TOB.Entities.Status;
                foreach (Account acc in AccountManager.Instance.GetCurrentAccounts())
                {
                    if (status.UserProfile.ScreenName == acc.Username)
                    {
                        returnVal = false;
                    }
                }
                return returnVal;          
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TOB.Logger.TOBLogger.WriteDebugInfo("FUNCTION NOT IMPLEMENTED");

            return null;
        }

        #endregion
    }
}
