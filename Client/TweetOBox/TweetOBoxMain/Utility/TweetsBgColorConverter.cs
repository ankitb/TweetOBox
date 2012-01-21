using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Drawing;
using System.Windows.Media;

namespace TweetOBoxMain.Utility
{
    public class TweetsBgColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            bool bg = (bool)value;

            return bg ? new SolidColorBrush(Colors.White) : new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 239, 232));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
