using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOB.Entities
{
    public class PeopleSearch
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _location;

        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _photoUrl;

        public string PhotoUrl
        {
            get { return _photoUrl; }
            set { _photoUrl = value; }
        }
    }
}
