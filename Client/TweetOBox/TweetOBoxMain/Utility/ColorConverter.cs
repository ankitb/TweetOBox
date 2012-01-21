using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace TweetOBoxMain.Utility
{
    public class ColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string foreColor = value.ToString();
            foreColor = "#FF" + foreColor;
            return foreColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TOB.Logger.TOBLogger.WriteDebugInfo("FUNCTION NOT IMPLEMENTED");

            return null;
        }

        #endregion
    }
}
