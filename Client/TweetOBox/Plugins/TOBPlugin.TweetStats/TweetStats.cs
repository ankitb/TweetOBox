using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime;
using TOB.Plugin;
using TOB.Entities;
using System.Collections;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;


namespace TOBPlugin.TweetStats
{
    [TOBPluginInfo("TweetStats", "Instantly view metadata and statistics of all your tweets", "Beneflux Ventures LLC","{399A3529-D9D4-4042-8920-197A208F1E99}")]
    public class TweetStats:ITOBPlugin
    {
        TweetStatsData tsd;
        TweetStatsUC uc = null;
        public TweetStats()
        {
            tsd = new TweetStatsData();
            
        }//constructor ends


        #region ITOBPlugin Members

        public override void ProcessInStream(List<TOBEntityBase> inStreamList)
        {
            //This function is called whenever new feeds arrive
            foreach (TOBEntityBase tobEB in inStreamList)
            {
                if (tobEB.GetType() == typeof(Status))
                {
                    Status tempVar = (Status)tobEB;
                    tsd.AddFeedData(tempVar.UserProfile.ScreenName);
                }

                if (tobEB.GetType() == typeof(DirectMessage))
                {
                    DirectMessage tempDM = (DirectMessage)tobEB;
                    tsd.AddFeedData(tempDM.UserProfile.ScreenName);
                }
            }

            tsd.SortList();

            if (uc == null)
            {
                uc = CreateTweetStatsUC(tsd);
                AddControlToPanel(uc);
            }

            uc.SetItemSource(tsd.tweetStatsData);
            
            return;
        }

        //Create UC
        private TweetStatsUC CreateTweetStatsUC(TweetStatsData tsd)
        {
            TweetStatsUC newUC = null;

            //Needed to get into STA context.
            Action action = delegate()
            {
                newUC = new TweetStatsUC(tsd);
            };
            
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, action);

            return newUC;
        
        }

        public override void ProcessSearchStream(List<Search> searchStream)
        {

            foreach (Search search in searchStream)
            {
                //implement this
            }

            return;
         }

        #endregion

    }
    /**********************************/

    //Data Class
    public class TweetStatsData
    {
        public Dictionary<string, int> tweetStatsData;
        
        //constructor
        public TweetStatsData()
        {
            tweetStatsData = new Dictionary<string, int>();
        }

        
        public void AddFeedData(string name)
        {
            if (tweetStatsData.ContainsKey(name))
            {
                tweetStatsData[name]++;
            }
            else
            {
                tweetStatsData.Add(name, 1);
            }
        }

        public void SortList()
        {
            tweetStatsData = tweetStatsData.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

    }

   

  
}
