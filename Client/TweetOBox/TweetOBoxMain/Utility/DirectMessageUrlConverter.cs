using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using TOB.Entities;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using System.Diagnostics;

namespace TweetOBoxMain.Utility
{
    public class DirectMessageUrlConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DirectMessage directMessage = value as DirectMessage;
            if (directMessage != null)
            {
                TextBlock tb = new TextBlock();
                TweetsTemplate tweetTemplate = new TweetsTemplate();
                Hyperlink hyperLinkUser = new Hyperlink();
                Style usernameStyle = (Style)tweetTemplate.FindResource("UsernameLinkStyle");
                hyperLinkUser.Style = usernameStyle;
                hyperLinkUser.Command = TOBCommands.ShowUserProfile;
                hyperLinkUser.CommandParameter = directMessage;
                Binding bind = new Binding();
                bind.Source = directMessage;
                hyperLinkUser.SetBinding(Hyperlink.DataContextProperty, bind);
                TextBlock tbUserName = new TextBlock();
                tbUserName.Text = directMessage.UserProfile.ScreenName;
                hyperLinkUser.Inlines.Add(tbUserName);
                tb.Inlines.Add(hyperLinkUser);
                tb.Inlines.Add(new Run(" "));

                string dmContent = directMessage.Text;
                string[] statusParts = dmContent.Split(' ');
                Style linkStyle = (Style)tweetTemplate.FindResource("LinkStyle");
                foreach (String part in statusParts)
                {
                    Run magicRun = new Run();
                    if (part.StartsWith("http"))
                    {
                        Hyperlink link = new Hyperlink();
                        link.Style = linkStyle;
                        link.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(link_RequestNavigate);
                        TextBlock tblink = new TextBlock();
                        tblink.Text = part + " ";
                        link.NavigateUri = new Uri(part);
                        link.Inlines.Add(tblink);
                        tb.Inlines.Add(link);
                        magicRun = new Run();
                    }
                    else
                    {
                        magicRun.Text = part + " ";
                        tb.Inlines.Add(magicRun);
                    }
                }

                return tb.Inlines;
            }
            else
            {
                return null;
            }
        }

        void link_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri.IsAbsoluteUri != false)
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
