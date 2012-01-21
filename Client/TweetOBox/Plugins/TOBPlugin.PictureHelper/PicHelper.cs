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
using System.Windows.Media.Imaging;

namespace TOBPlugin.PictureHelper
{
    [TOBPluginInfo("Picture Helper", "Use this plugin to view pictures posted in your stream", "Beneflux Ventures LLC", "{B9BDFA9B-107C-46dd-B7C4-38C0A7043468}")]
    public class PicHelper : ITOBPlugin
    {
        PicHelperControl CurrentControl = null;
        PicHelperControl CurrentControlSearch = null;

        public PicHelper()
        {

        }

        public override void ProcessInStream(List<TOBEntityBase> inStreamList)
        {
            Regex regx = new Regex("https?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);

            foreach (TOBEntityBase tobEB in inStreamList)
            {
                try
                {
                    string msgText = "";
                    string screenName = "";
                    string profileImage = "";

                    if (tobEB.GetType() == typeof(Status))
                    {
                        Status tempVar = (Status)tobEB;
                        msgText = tempVar.Text;
                        screenName = tempVar.UserProfile.ScreenName;
                        profileImage = tempVar.UserProfile.ProfileImageUrl;
                    }
                    else if (tobEB is DirectMessage)
                    {
                        DirectMessage tempDM = (DirectMessage)tobEB;
                        msgText = tempDM.Text;
                        screenName = tempDM.UserProfile.ScreenName;
                        profileImage = tempDM.UserProfile.ProfileImageUrl;
                    }

                    MatchCollection matches = regx.Matches(msgText);

                    foreach (Match match in matches)
                    {
                        string imageURI = GetImageURI(match.Value);

                        if (imageURI != null)
                        {
                            PicHelperClass item = new PicHelperClass() { User = screenName, OrigUri=new Uri(match.Value), ProfileImage= new Uri(profileImage) , Image = new Uri(imageURI), Text = msgText };
                            Action action = delegate()
                            {
                                if (CurrentControl == null || !CurrentControl.AddItem(item))
                                {
                                    CurrentControl = new PicHelperControl();
                                    AddControlToPanel(CurrentControl);
                                    CurrentControl.AddItem(item);
                                }
                            };

                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, action);
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
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
                    string imageURI = GetImageURI(match.Value);

                    if (imageURI != null)
                    {
                        PicHelperClass item = new PicHelperClass() { User = search.UserName, Image = new Uri(imageURI), Text = search.SearchText };
                        Action action = delegate()
                        {
                            if (CurrentControl == null || !CurrentControlSearch.AddItem(item))
                            {
                                CurrentControlSearch = new PicHelperControl();
                                AddControlToPanel(CurrentControlSearch);
                                CurrentControlSearch.AddItem(item);
                            }
                        };

                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, action);
                    }
                }
            }
        }

        public string GetImageURI(string uri)
        {
            Uri hostURI = new Uri(uri);
            string origURL = uri;
            string returnImage = null;

            switch (hostURI.Host)
            {
                case "yfrog.com":
                    returnImage = origURL + ":iphone";
                    //si.SnapShot = origURL + ".th.jpg";
                    break;
                case "twitpic.com":
                    returnImage = "http://" + hostURI.Host + "/show/full" + hostURI.AbsolutePath + hostURI.Query;
                    //si.SnapShot = "http://" + hostURI.Host + "/show/thumb" + hostURI.AbsolutePath + hostURI.Query;
                    break;
                case "plixi.com":
                    returnImage = @"http://api.plixi.com/api/tpapi.svc/imagefromurl?size=big&url=" + origURL;
                    //si.SnapShot = @"http://api.plixi.com/api/tpapi.svc/json/imagefromurl?size=thumbnail&url=" + origURL;
                    break;
                case "tweetphoto.com":
                    returnImage = @"http://api.plixi.com/api/tpapi.svc/imagefromurl?size=big&url=" + origURL;
                    //si.SnapShot = @"http://api.plixi.com/api/tpapi.svc/json/imagefromurl?size=thumbnail&url=" + origURL;
                    break;
                case "twitgoo.com":
                    returnImage = origURL + "/img";
                    //si.SnapShot = origURL + "/mini";
                    break;
                case "flic.kr":
                    returnImage = @"http://flic.kr/p/img" + hostURI.AbsolutePath.Replace(@"/p", "") + "_m.jpg";
                    break;
            }

            return returnImage;
        }
        
    }
}