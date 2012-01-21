using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOBKPI.BLL.TOBKPI.DAL;

namespace TOBKPI.BLL
{
    public class RegistrationManager
    {
       
        ClientRegistration _clientRegistration = new ClientRegistration();
        public string RegistrationTOBClient(long ticks)
        {           
            return _clientRegistration.RegisterTOBClient(ticks);
        }

        public void TOBUpdate(string regId, DateTime startTime, DateTime endTime)
        {
            _clientRegistration.TOBUpdate(regId, startTime,endTime);
        }
    }
}
