using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace TOB.Utility
{
    public class MarkAsRead
    {
        private MarkAsRead()
        { 
        }
        private DispatcherTimer _timer = null;
        private static MarkAsRead _instance;
        public delegate void MakeLogOff();
        static public event MakeLogOff MakeLogOffEvent;
        public static MarkAsRead Instance
        {           
            get
            {
                if (_instance == null)
                    _instance = new MarkAsRead();

                return _instance;
            }
        }

        private bool _isNotActive;

        public bool IsNotActive
        {
            get { return _isNotActive; }
            set { _isNotActive = value; }
        }

        public void StartActive()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(60);
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            IsNotActive = true;
            if (MakeLogOffEvent != null)
            {
                MakeLogOffEvent();
            }           
        }

        public void ResetTimer()
        {
            _timer.Stop();
            _timer.Start();
            IsNotActive = false;
        }

        private bool _isMinimized;

        public bool IsMinimized
        {
            get { return _isMinimized; }
            set { _isMinimized = value; }
        }

    }
}
