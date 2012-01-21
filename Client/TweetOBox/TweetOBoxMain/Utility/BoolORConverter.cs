using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace TweetOBoxMain.Utility
{
    internal class BoolORConverter : IMultiValueConverter
    {

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null)
            {
                bool result = false;
                var query = from o in values
                            where o.GetType() != typeof(bool)
                            select o;
                if (query.Count() != 0)
                    return false;
                foreach (object item in values)
                {
                    if (item.GetType() == typeof(bool))
                        result = result || (bool)item;
                }
                return result;
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return new object[] { new object(), new object() };
        }

        #endregion
    }
}
