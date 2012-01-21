using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Threading;
using TOB.Entities;
using TOB.BLL;
using System.Reflection;
using System.Windows.Shapes;

namespace TOB.Utility
{
    public class CleanUpObjectsInDB
    {
        private DispatcherTimer _timer = null;
        private StatusBO _statusBO = null;
        private DirectMessageBO _dmBO = null;
        private CleanUpObjectsInDB()
        {            
        }

        private static CleanUpObjectsInDB _instance;
        public static CleanUpObjectsInDB Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CleanUpObjectsInDB();

                return _instance;
            }
        }
        private StatusBO LocalStatusBO
        {
            get
            {
                if (_statusBO == null)
                {
                    _statusBO = new StatusBO();
                }

                return _statusBO;
            }

        }

        private DirectMessageBO LocalDirectMessageBO
        {
            get
            {
                if (_dmBO == null)
                {
                    _dmBO = new DirectMessageBO();
                }

                return _dmBO;
            }

        }
        //Update the time for 20 minutes after that 12 hours.
        public void CleanUpObjects()
        {

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(20);
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            try
            {
                ClearStatusObjectsDB();
                ClearDMObjectsInDB();
                _timer.Interval = TimeSpan.FromHours(12);
            }
            catch (Exception ex)
            {
                TOB.Logger.TOBLogger.WriteDebugInfo(ex.Message);
            }
        }

        //clean the Status objects
        void ClearStatusObjectsDB()
        {
            List<Status> statusList = LocalStatusBO.GetAll();
            if (statusList.Count > 5000)
            {
                List<Status> stList = LocalStatusBO.GetAll().OrderByDescending(s => s.TwitterCreatedDate).Skip(5000).ToList();
                LocalStatusBO.DeleteAll(stList);
                LocalStatusBO.SaveChanges();
            }
          
        }

        //clean the DirectMessages objects
        void ClearDMObjectsInDB()
        {
            List<DirectMessage> dmList = LocalDirectMessageBO.GetAll();
            if (dmList.Count > 500)
            {
                List<DirectMessage> directmsgList = LocalDirectMessageBO.GetAll().OrderByDescending(s => s.TwitterCreatedDate).Skip(500).ToList();
                LocalDirectMessageBO.DeleteAll(directmsgList);
                LocalDirectMessageBO.SaveChanges();
            }
        }
        
                
    }
}
