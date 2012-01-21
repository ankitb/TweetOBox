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
    public class NormalAuth
    {
        public Account Authenticate(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("userName");
            }

            TwitterService service = new TwitterService(ConfigurationManager.AppSettings["ConsumerKey"],
                ConfigurationManager.AppSettings["ConsumerSecret"]);

            OAuthAccessToken access = service.GetAccessTokenWithXAuth(userName, password);

            service.AuthenticateWith(access.Token, access.TokenSecret);

            var profile = service.GetUserProfile();

            Account account = AccountManager.Instance.GetCurrentAccounts().Where(acc => acc.Username == profile.ScreenName).FirstOrDefault();
            if (account != null)
            {
                throw new AuthFailureException("User " + account.Username + " already has an account with TweetOBox.");
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
                AccountManager.Instance.AddAccount(account, false);
            }
            else
            {
                throw new AuthFailureException(service.Response.StatusDescription);
            }

            return account;
        }

    }
}

