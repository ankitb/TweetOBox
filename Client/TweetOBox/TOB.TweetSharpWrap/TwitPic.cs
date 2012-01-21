using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Linq;
using TweetSharp;
using System.Xml;

namespace TOB.TweetSharpWrap
{
    public class TwitPic
    {
        private const string TWITPIC_UPLAOD_API_URL = "http://api.twitpic.com/2/upload";
        private const string TWITPIC_UPLOAD_AND_POST_API_URL = "http://api.twitpic.com/1/uploadAndPost";
        
        /// 
        /// Uploads the photo and sends a new Tweet
        /// 
        /// <param name="binaryImageData">The binary image data.
        /// <param name="tweetMessage">The tweet message.
        /// <param name="filename">The filename.
        /// Return true, if the operation was succeded.
        public string UploadPhoto(string imageFile, string tpkey, string usrtoken, string usrsecret, string contoken, string consecret)
        {
            TwitterService service = new TwitterService(contoken, consecret);
            service.AuthenticateWith(usrtoken, usrsecret);

            Hammock.RestRequest request = service.PrepareEchoRequest();
            request.Path = "upload.xml";
            request.AddFile("media", "uploadfile", imageFile, "image/jpeg");
            request.AddField("key", tpkey);

            Hammock.RestClient client = new Hammock.RestClient() { Authority = "http://api.twitpic.com", VersionPath = "2" };
            Hammock.RestResponse response = client.Request(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                XDocument doc = XDocument.Parse(response.Content);

                XElement image = doc.Element("image");

                return image.Element("url").Value;
            }
            else
                throw new Exception("Error occured while uploading image to TwitPic servers. Please try again later");
            
            return "";
        }

        /// <summary>
        /// Converting image into byte
        /// </summary>
        /// <param name="p_postedImageFileName">Filename</param>
        /// <param name="p_fileType">file extensions</param>
        /// <returns></returns>
        public byte[] ReadImage(string p_postedImageFileName, string[] p_fileType)
        {
            bool isValidFileType = false;
            try
            {
                FileInfo file = new FileInfo(p_postedImageFileName);
                foreach (string strExtensionType in p_fileType)
                {
                    if (strExtensionType == file.Extension.ToLower())
                    {
                        isValidFileType = true;
                        break;
                    }
                }
                if (isValidFileType)
                {
                    FileStream fs = new FileStream(p_postedImageFileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    byte[] image = br.ReadBytes((int)fs.Length);
                    br.Close();
                    fs.Close();
                    return image;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       
    }
}
