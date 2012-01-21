using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TweetSharp;
using System.Configuration;
using TOB.BLL;
using TOB.Entities;

namespace TOB.TweetSharpWrap
{
    /// <summary>
    /// OAuth is a singleton class.
    /// </summary>
    public class OAuth
    {
        private readonly OAuthRequestToken _requestToken;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly Uri _uri;

        public OAuth()
        {
            _consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            _consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
            TwitterService service = new TwitterService(_consumerKey, _consumerSecret);

            _requestToken = service.GetRequestToken();
            _uri = service.GetAuthorizationUri(_requestToken);
        }

        public void StartAuthorization()
        {
            System.Diagnostics.Process.Start(_uri.ToString());
        }

        public Account ProcessAuthentication(string pin)
        {
            TwitterService service = new TwitterService(_consumerKey, _consumerSecret);

            OAuthAccessToken access = service.GetAccessToken(_requestToken, pin);

            service.AuthenticateWith(access.Token, access.TokenSecret);

            var profile = service.GetUserProfile();

            Account account = AccountManager.Instance.GetCurrentAccounts().Where(acc => acc.Username == profile.ScreenName).FirstOrDefault();
            if (account != null)
            {
                throw new AuthFailureException("User " +account.Username +  " already has an account with TweetOBox.");
            }
            if (profile != null && account == null)
            {
                account = new Account();
                account.Username = profile.ScreenName;
               // account.Password = profile.p
                account.AccountType = (int)AccountTypeEnum.Twitter;
                account.AccessToken = access.Token;
                account.AccessTokenSecret = access.TokenSecret;
                account.IsOAuth = true;
                AccountManager.Instance.AddAccount(account,false);
            }
            else
            {
                throw new AuthFailureException(service.Response.StatusDescription);
            }

            return account;
        }
    }
}
