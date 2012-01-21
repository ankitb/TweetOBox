using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TOBKPI.BLL.TOBKPI.DAL
{
    public class ClientRegistration 
    {
        static string connection = ConfigurationManager.ConnectionStrings["TOBServiceConnectionString"].ConnectionString;
        TOBServiceDB _tobServiceDB = new TOBServiceDB(connection);  
        public string RegisterTOBClient(long ticks)
        {
            DateTime clientDT = new DateTime(ticks);

            if (DateTime.UtcNow.Subtract(clientDT).TotalSeconds > 61)
            {
                return null;
            }

            TOBClientRegistration TOBClientReg = new TOBClientRegistration();            
            Guid RegId = Guid.NewGuid();
            TOBClientReg.RegistrationId = RegId;
            TOBClientReg.RegistrationDate = DateTime.UtcNow;
            _tobServiceDB.TOBClientRegistrations.InsertOnSubmit(TOBClientReg);
            _tobServiceDB.SubmitChanges();           
            return RegId.ToString();
        }
               
        public void TOBUpdate(string regId, DateTime startTime, DateTime endTime)
        {
            //Runtime corruption checks
            if (startTime > DateTime.UtcNow || endTime > DateTime.UtcNow)
                return;

            TOBClientRegistration existingClient = _tobServiceDB.TOBClientRegistrations.Where(s => s.RegistrationId == new Guid(regId)).FirstOrDefault();
            
            if(existingClient != null)
            {
                TOBClientTimeTracker tobClientTimeTracker = _tobServiceDB.TOBClientTimeTrackers.Where(s => s.RegisterationId == new Guid(regId) && s.TOBStartTime > startTime).FirstOrDefault();

                //Ignore all values that are before already present start time
                if (tobClientTimeTracker != null)
                {
                    //TimeSpan tsTime = endTime.Subtract(Convert.ToDateTime(tobClientTimeTracker.TOBStartTime));
                    
                    //CODE NEEDS FIXING
                    //if (tsTime.TotalHours > 36)
                    //{
                        return;
                    //}
                }

                UpdateTimeTracker(regId, ref startTime, ref endTime);
            }
          
        }

        private void UpdateTimeTracker(string regId, ref DateTime startTime, ref DateTime endTime)
        {
            TOBClientTimeTracker tob = new TOBClientTimeTracker();
            tob.RegisterationId = new Guid(regId);
            tob.TOBStartTime = startTime;
            tob.TOBEndTime = endTime;
            TimeSpan timeSpan = startTime - endTime;
            tob.Duration = (long?) timeSpan.TotalMinutes;
            _tobServiceDB.TOBClientTimeTrackers.InsertOnSubmit(tob);
            _tobServiceDB.SubmitChanges();
        }
    }
}
