using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Xml.Linq;

namespace TweetOBoxMain.Utility
{
    public class TweetSourceConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string returnValue = "";
            string inputval=value as string;
            if (value.ToString() != "web")
            {
                try
                {
                    inputval = inputval.Replace("&", "&amp;");
                    XElement xl = XElement.Parse(inputval);
                    returnValue = xl.Value;
                }
                catch (System.Xml.XmlException xmlE)
                {
                    TOB.Logger.TOBLogger.WriteDebugInfo("Failed to parse XML in " + inputval + " --- " + xmlE.ToString());
                }
                
            }
            else
            {
                returnValue = "web";
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
