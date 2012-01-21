using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace TweetOBoxMain.Utility
{
   public class GlobalSearchProviderType
    {
        public GlobalSearchProviderType() { }
        public GlobalSearchProviderType(string ProviderName, string RelativeUrl, SearchTableEnum SearchType)
        {
            this.ProviderName = ProviderName;
            this.RelativeUrl = RelativeUrl;
            this.SearchType = SearchType;
        }
        public string ProviderName { get; set; }
        public string RelativeUrl { get; set; }
        public SearchTableEnum SearchType { get; set; }
        public string DisplayName
        {
            get
            {
                return "Search " + ProviderName;
            }
        }
        public string WatermarkText
        {
            get
            {
                return "Search" + " " + ProviderName + " in Twitter.com";
            }
        }
    }

   public class GlobalSearchProviderPresenter
    {
        public ObservableCollection<GlobalSearchProviderType> SearchProviders
        {
            get;
            set;
        }

        public GlobalSearchProviderPresenter()
        {
            SearchProviders = new ObservableCollection<GlobalSearchProviderType>();
            SearchProviders.Add(new GlobalSearchProviderType("Tweets", @"\Images\tweeter-search.png", SearchTableEnum.Tweets));
            SearchProviders.Add(new GlobalSearchProviderType("Peoples", @"\Images\people-search.png", SearchTableEnum.Peoples));
           // SearchProviders.Add(new GlobalSearchProviderType("Web", @"\Images\Ttwitter.png", SearchTableEnum.Web));
        }
    }
}
