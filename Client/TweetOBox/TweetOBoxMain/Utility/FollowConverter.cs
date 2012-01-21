using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using TOB.BLL;
using TweetSharp;
using TOB.Entities;
using System.Windows;
using System.ComponentModel;

namespace TweetOBoxMain.Utility
{
    public class FollowConverter : IValueConverter
    {
        #region IValueConverter Members
        private UserProfileBO _UserProfile = null;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility followingVisible = Visibility.Visible;
            try
            {
                TwitterUser twitterUser = value as TwitterUser;
                UserProfile userProfile = LocalUserProfileBO.Get(temp => temp.ScreenName == twitterUser.ScreenName);
                if (userProfile != null)
                {
                    if (userProfile.IsFollowing == true)
                    {
                        followingVisible = Visibility.Collapsed;
                    }
                }
                else
                {
                    followingVisible = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                TOB.Logger.TOBLogger.WriteDebugInfo(ex.Message);
            }

            return followingVisible;

        }

        private UserProfileBO LocalUserProfileBO
        {
            get
            {
                if (_UserProfile == null)
                {
                    _UserProfile = new UserProfileBO();
                }
                return _UserProfile;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
