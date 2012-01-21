using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows.Forms;
using TOB.BLL;
using TOB.Entities;
using System.ServiceModel.Description;
using TweetOBoxMain.TOBKUIService;

namespace TweetOBoxMain
{
    internal class ClientKUI : IDisposable
    {
        private DispatcherTimer _timer = null;
        private DateTime _startTime;
        private string _registerId;
        private TOBKUIBO _tobKUIBO = null;
        
        private ClientKUI()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromHours(6);
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();

            _startTime = DateTime.UtcNow;
        }

        private TOBKUIBO LocalTOBKUIBO
        {
            get
            {
                if (_tobKUIBO == null)
                {
                    _tobKUIBO = new TOBKUIBO();
                }
                return _tobKUIBO;
            }
        }

        //Reset the start time.
        void _timer_Tick(object sender, EventArgs e)
        {
            TOBStop();
            _startTime = DateTime.UtcNow;
        }

        private static ClientKUI _instance;
        
        public static ClientKUI Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ClientKUI();

                return _instance;
            }
        }
                      
        public void RegisterTOB()
        {
            try
            {
                if (LocalTOBKUIBO.GetAll().Count < 1)
                {
                    //Get registration Id from service and insert into localDB.
                    _registerId = GetRegisterId();
                    TOBRegister tobRegister = new TOBRegister();
                    tobRegister.RegisterId = new Guid(_registerId);
                    LocalTOBKUIBO.Insert(tobRegister);
                    LocalTOBKUIBO.SaveChanges();
                }
                else
                {
                    _registerId = GetRegisterIDFromLocal();
                }
            }
            catch (Exception ex)
            {
                TOB.Logger.TOBLogger.WriteDebugInfo(ex.Message);
            }
        }

        //Get registration Id from localDB.
        private string GetRegisterIDFromLocal()
        {
            TOBRegister tobReg = (LocalTOBKUIBO.GetAll()).FirstOrDefault();
            return tobReg.RegisterId.ToString() ;
        }

        //Get registration Id from WCF Service.
        private string GetRegisterId()
        {
            TOBKUIServiceClient tobKUIService = new TOBKUIServiceClient();
            string regId = tobKUIService.RegisterTOB(_startTime.Ticks);
            tobKUIService.Close();
            return regId;
        }

        //Set the starting time of TOB.
        public void TOBStart()
        {
            _startTime = DateTime.UtcNow;
        }

        /// <summary>
        /// TweetOBox stop 
        /// </summary>
        public void TOBStop()
        {
            try
            {
                TOBKUIServiceClient tobKUIService = new TOBKUIServiceClient();
                tobKUIService.TOBUpdateKUI(_registerId, _startTime, DateTime.UtcNow);
                tobKUIService.Close();
            }
            catch (Exception ex)
            {
                TOB.Logger.TOBLogger.WriteDebugInfo(ex.Message);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            TOBStop();
        }

        #endregion
    }
}
