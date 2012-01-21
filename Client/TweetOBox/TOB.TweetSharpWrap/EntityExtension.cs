using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TweetSharp;
using TOB.Entities;

namespace TOB.TweetSharpWrap
{
    public class TwitterListExtended : TwitterList
    {
        private RoutedUICommand _commandGroups;
        public RoutedUICommand CommandGroups
        {
            get { return _commandGroups; }
            set { _commandGroups = value; }
        }

        private RoutedUICommand _commandDeleteGroups;
        public RoutedUICommand CommandDeleteGroups
        {
            get { return _commandDeleteGroups; }
            set { _commandDeleteGroups = value; }
        }

        private RoutedUICommand _commandAddUserToList;

        public RoutedUICommand CommandAddUserToList
        {
            get { return _commandAddUserToList; }
            set { _commandAddUserToList = value; }
        }

        private Account _account = null;

        public Account UserAccount
        {
            get { return _account; }
            set { _account = value; }
        }

        public TwitterListExtended(TwitterList lst)
        {
            Type theType = typeof(TwitterList);
            System.Reflection.PropertyInfo[] properties = theType.GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                System.Reflection.MethodInfo getMethod =
                    property.GetGetMethod();
                System.Reflection.MethodInfo setMethod =
                    property.GetSetMethod();
                object val = getMethod.Invoke(lst, null);
                object[] param = new object[] { val };
                setMethod.Invoke(this, param);
            }
        }
    }
}
