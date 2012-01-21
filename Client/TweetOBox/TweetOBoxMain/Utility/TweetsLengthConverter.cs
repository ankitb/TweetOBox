using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace TweetOBoxMain.Utility
{
    public class TweetsLengthConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }

            string inputText = value as string;
            int txtLength = 140 - inputText.Length;

            if (inputText.StartsWith("d") && inputText.Length > 2)
            {
                txtLength += inputText.IndexOf(' ', 0, 3);
            }
            return txtLength;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
