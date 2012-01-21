using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOB.TweetSharpWrap
{
    //Hiren :This used when OAuthentication is failed
    public class AuthFailureException : Exception
    {
        public AuthFailureException(string message)
            : base(message)
        {
        }
    }
}
