using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TOB.Entities;
using TOB.TweetSharpWrap;
using System.ComponentModel;
using TweetSharp;

namespace TweetOBoxMain.UserControls
{
    /// <summary>
    /// Interaction logic for UserProfileDetails.xaml
    /// </summary>
    public partial class UserProfileDetails : UserControl
    {
        public UserProfileDetails()
        {
            InitializeComponent();
        }
        
        public void SetUserDetails(UserProfile tup)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                BitmapImage imgSource = new BitmapImage(new Uri(tup.ProfileImageUrl));
                imgUserPhoto.Source = imgSource;
                tbUserName.Text = tup.ScreenName;
                tbUserDesc.Text = tup.Description;
                tbLocation.Text = tup.Location;
                tbFollowers.Text = Convert.ToString((int)tup.FollowersCount);
                tbFollowing.Text = Convert.ToString((int)tup.FriendsCount);
                dmuserProfile.CommandParameter = tup.ScreenName;
                btnUnFollow.Visibility = Visibility.Visible;
                btnFollow.Visibility = Visibility.Collapsed;               
                btnUnFollow.DataContext = tup;
            }
        }

        public void SetUserDetails(TwitterUser tup)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                BitmapImage imgSource = new BitmapImage(new Uri(tup.ProfileImageUrl));
                imgUserPhoto.Source = imgSource;
                tbUserName.Text = tup.ScreenName;
                tbUserDesc.Text = tup.Description;
                tbLocation.Text = tup.Location;
                tbFollowers.Text = Convert.ToString((int)tup.FollowersCount);
                tbFollowing.Text = Convert.ToString((int)tup.FriendsCount);
                dmuserProfile.CommandParameter = tup.ScreenName;
                btnUnFollow.Visibility = Visibility.Visible;
                btnFollow.Visibility = Visibility.Collapsed;             
                btnUnFollow.DataContext = tup;
                btnFollow.DataContext = tup;
            }
        }
    }
}
