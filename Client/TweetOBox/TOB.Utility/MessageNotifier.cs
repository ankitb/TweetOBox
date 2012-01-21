using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOB.Utility
{
    public delegate void NotifyMessageDelegate(string Message);

    public class MessageNotifier
    {
        //private DispatcherTimer _timer = null;
        private MessageNotifier()
        {
        }

        private static MessageNotifier _instance;
        public static MessageNotifier Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MessageNotifier();

                return _instance;
            }
        }

        public NotifyMessageDelegate NotifyMessage;

        //public void NotifyMessage(string Message)
        //{
        //    if (((Application.Current as App).MainWindow as TOBMain) != null)
        //    {
        //        ((Application.Current as App).MainWindow as TOBMain).tbStatus.Text = Message;
        //    }
        //}
    }
}
