using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace TweetOBoxMain.Utility
{
    public class FollowingConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility followingVisible = Visibility.Visible;
            try
            {                
                if (value.ToString() == "Collapsed")
                {
                    followingVisible = Visibility.Visible;
                }
                else
                {
                    followingVisible = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                TOB.Logger.TOBLogger.WriteDebugInfo(ex.Message);
            }
            return followingVisible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
