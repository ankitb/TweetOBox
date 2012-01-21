using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOB.Plugin;
using TOB.Entities;
using System.Windows;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TOBPlugin.WebPreview
{
    [TOBPluginInfo("WebLinks Preview", "Instantly preview webpage snapshots in your stream", "Beneflux Ventures LLC", "{5109E24D-AC2E-4c76-B548-5610316DDE87}")]
    public class WebPreview : ITOBPlugin 
    {
        public WebPreview()
        {
            MessageBox.Show("Thanks for trying out the WebPreview Plugin. Since the plugin is in beta form, it is known to have performance issues. If you find this plugin to be useful and would like us to continue investing in it. Let us know at @beneflux");
            string cachePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\WebPreviewCache\\";

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
        }

        #region ITOBPlugin Members

        public override void ProcessInStream(List<TOBEntityBase> inStreamList)
        {
            Regex regx = new Regex("https?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);

            foreach (TOBEntityBase tobEB in inStreamList)
            {
                if (tobEB.GetType() == typeof(Status))
                {
                    Status tempVar = (Status)tobEB;

                    MatchCollection matches = regx.Matches(tempVar.Text);
                    
                    foreach (Match match in matches)
                    {
                        Uri webLink = new Uri(match.Value);

                        AddControlToPanel(CreateWebPreviewControl(tempVar.UserProfile.ScreenName, webLink));
                    }
                }

                if (tobEB.GetType() == typeof(DirectMessage))
                {
                    DirectMessage tempDM = (DirectMessage)tobEB;
                    MatchCollection matchesDM = regx.Matches(tempDM.Text);
                    foreach (Match match in matchesDM)
                    {
                        Uri webLink = new Uri(match.Value);
                        AddControlToPanel(CreateWebPreviewControl(tempDM.UserProfile.ScreenName,webLink));
                    }
                }
            }

            return;
        }

        public override void ProcessSearchStream(List<Search> searchStream)
        {
            Regex regx = new Regex("https?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
            foreach (Search search in searchStream)
            {
                MatchCollection matchesSearch = regx.Matches(search.SearchText);
                foreach (Match match in matchesSearch)
                {
                    Uri webLink = new Uri(match.Value);
                    AddControlToPanel(CreateWebPreviewControl(search.UserName, webLink));
                }
            }
        }

        #endregion

        private UserControl CreateWebPreviewControl(string user, Uri urlloc)
        {
            WebPreviewUC newUC = null;

            //Needed to get into STA context.
            Action action = delegate()
            {
                newUC = new WebPreviewUC(this, user, urlloc);
            };
            
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, action);

            return newUC;
        }
    }
}
