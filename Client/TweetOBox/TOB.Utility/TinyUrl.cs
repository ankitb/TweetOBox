using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace TOB.Utility
{
    public class TinyUrl
    {
        static Regex Regx = new Regex("https?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);

        public static string ToTinyURLS(string txt)
        {
            MatchCollection mactches = Regx.Matches(txt);

            foreach (Match match in mactches)
            {
                if (match.Value.StartsWith("http://tinyurl.com"))
                    continue;

                string tURL = MakeTinyUrl(match.Value);
                txt = txt.Replace(match.Value, tURL);
            }

            return txt;
        }

        public static string MakeTinyUrl(string Url)
        {
            try
            {
                if (Url.Length <= 12)
                {
                    return Url;
                }
                if (!Url.ToLower().StartsWith("http") && !Url.ToLower().StartsWith("ftp"))
                {
                    Url = "http://" + Url;
                }
                var request = WebRequest.Create("http://tinyurl.com/api-create.php?url=" + Url);
                var res = request.GetResponse();
                string text;
                using (var reader = new StreamReader(res.GetResponseStream()))
                {
                    text = reader.ReadToEnd();
                }
                return text;
            }
            catch (Exception)
            {
                return Url;
            }
        }
    }
}
