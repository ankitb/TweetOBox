using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOB.Entities
{
    public partial class Search
    {
        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _SearchText;

        public string SearchText
        {
            get { return _SearchText; }
            set { _SearchText = value; }
        }

        private string _photoUrl;

        public string PhotoUrl
        {
            get { return _photoUrl; }
            set { _photoUrl = value; }
        }
               
    }
}
