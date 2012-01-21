using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using TOB.Entities;
using TweetSharp;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace TweetOBoxMain.Utility
{
    public class TagsLinkConverter : IValueConverter
    {
        #region IValueConverter Members

        private static char[] _delimChars = ".,?!:()\"\'{}".ToCharArray();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return GetConvertedText(value);           
        }

        private InlineCollection GetConvertedText(object value)
        {
            if (value == null)
            {
                return null;
            }
            TextBlock tb = new TextBlock();
            TextBlock tbUserName = new TextBlock();
            string statusContent = string.Empty;
            Binding bind = new Binding();
            TweetsTemplate tweetTemplate = new TweetsTemplate();
            Hyperlink hyperLinkUser = new Hyperlink();
            Style usernameStyle = (Style)tweetTemplate.FindResource("UsernameLinkStyle");
            hyperLinkUser.Style = usernameStyle;
            hyperLinkUser.Command = TOBCommands.ShowUserProfile;
            switch (value.GetType().Name)
            {
                case "Status":
                    {
                        statusContent = StatusText(value, tb, tbUserName, statusContent, bind, hyperLinkUser);
                        break;
                    }
                case "TwitterSearchStatus":
                    {
                        statusContent = SearchText(value, tb, tbUserName, statusContent, bind, hyperLinkUser);
                        break;
                    }
                case "TwitterStatus":
                    {
                        statusContent = TwitterStatusText(value, tb, tbUserName, statusContent, bind, hyperLinkUser);
                        break;
                    }
            }
            return GetInlineCollections(tb, statusContent, bind, tweetTemplate, hyperLinkUser);
        }



        private InlineCollection GetInlineCollections(TextBlock tb, string statusContent, Binding bind, TweetsTemplate tweetTemplate, Hyperlink hyperLinkUser)
        {
            string[] statusParts = statusContent.Split(' ');
            Style linkStyle = (Style)tweetTemplate.FindResource("LinkStyle");
            string parsedStr = "";
            foreach (String part in statusParts)
            {
                parsedStr = part.TrimStart(_delimChars);

                //Check for links
                if (parsedStr.StartsWith("http") && parsedStr.Length > 7)
                {
                    Hyperlink link = new Hyperlink();
                    link.Style = linkStyle;                  
                    Run run = new Run(part + " ");
                    
                    try
                    {
                        link.NavigateUri = new Uri(parsedStr.TrimEnd(_delimChars));
                        link.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(link_RequestNavigate);
                    }
                    catch (Exception e)
                    {
                        TOB.Logger.TOBLogger.WriteDebugInfo(parsedStr + " failed - " + e.ToString());
                    }

                    link.Inlines.Add(run);
                    tb.Inlines.Add(link);
                }
                //Check for user tags
                else if (parsedStr.StartsWith("@"))
                {
                    Hyperlink hplink = new Hyperlink();
                    hplink.Style = linkStyle;
                    hplink.Command = TOBCommands.ShowUserProfileForTags;
                    hplink.CommandParameter = parsedStr.TrimEnd(_delimChars);
                    hyperLinkUser.SetBinding(Hyperlink.DataContextProperty, bind);
                    Run run = new Run(part + " ");
                    hplink.Inlines.Add(run);
                    tb.Inlines.Add(hplink);

                }
                //check for #tags.
                else if (parsedStr.StartsWith("#"))
                {
                    Hyperlink hplinks = new Hyperlink();
                    hplinks.Style = linkStyle;
                    hplinks.Command = TOBCommands.ShowFilterForTags;
                    hplinks.CommandParameter = parsedStr.TrimEnd(_delimChars);
                    Run run = new Run(part + " ");
                    hplinks.Inlines.Add(run);
                    tb.Inlines.Add(hplinks);
                }
                else
                {
                    Run magicRun = new Run();
                    magicRun.Text = part + " ";
                    tb.Inlines.Add(magicRun);
                }
                
            }
            return tb.Inlines;
        }

        static void link_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri.IsAbsoluteUri != false)
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
        }

        private string StatusText(object value, TextBlock tb, TextBlock tbUserName, string statusContent, Binding bind, Hyperlink hyperLinkUser)
        {
            Status status = value as Status;
            hyperLinkUser.CommandParameter = status;
            bind.Source = status;
            hyperLinkUser.SetBinding(Hyperlink.DataContextProperty, bind);
            tbUserName.Text = status.UserProfile.ScreenName;
            hyperLinkUser.Inlines.Add(tbUserName);
            tb.Inlines.Add(hyperLinkUser);
            tb.Inlines.Add(new Run(" "));
            statusContent = status.Text;
            return statusContent;
        }

        private string TwitterStatusText(object value, TextBlock tb, TextBlock tbUserName, string statusContent, Binding bind, Hyperlink hyperLinkUser)
        {

            TwitterStatus twitterStatus = value as TwitterStatus;
            hyperLinkUser.CommandParameter = twitterStatus;
            bind.Source = twitterStatus;
            hyperLinkUser.SetBinding(Hyperlink.DataContextProperty, bind);
            tbUserName.Text = twitterStatus.User.ScreenName;
            hyperLinkUser.Inlines.Add(tbUserName);
            tb.Inlines.Add(hyperLinkUser);
            tb.Inlines.Add(new Run(" "));
            statusContent = twitterStatus.Text;
            return statusContent;
        }

        private string SearchText(object value, TextBlock tb, TextBlock tbUserName, string statusContent, Binding bind, Hyperlink hyperLinkUser)
        {

            TwitterSearchStatus search = value as TwitterSearchStatus;
            hyperLinkUser.CommandParameter = search;
            bind.Source = search;
            hyperLinkUser.SetBinding(Hyperlink.DataContextProperty, bind);
            tbUserName.Text = search.FromUserScreenName;
            hyperLinkUser.Inlines.Add(tbUserName);
            tb.Inlines.Add(hyperLinkUser);
            tb.Inlines.Add(new Run(" "));
            statusContent = search.Text;
            return statusContent;
        }
                
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
