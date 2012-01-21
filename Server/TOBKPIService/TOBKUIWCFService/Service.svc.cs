using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using TOBKPI.BLL;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace TOBKUIWCFService
{
    // NOTE: If you change the class name "Service1" here, you must also update the reference to "Service1" in App.config.
    public class TOBKUIService : ITOBKUIService
    {

        #region ITOBKUIService Members

        public TOBRegistrationResponseMessage RegisterTOB(ClientRegisterRequest clientObj)
        {
            RegistrationManager registrationManager = new RegistrationManager();
            TOBRegistrationResponseMessage msg = new TOBRegistrationResponseMessage();
            string Regid = registrationManager.RegistrationTOBClient(clientObj.ticks);
            msg.RegistrationId = Regid;
            return msg;
        }

        public void TOBUpdateKUI(ClientKUIRequest clientObj)
        {
            RegistrationManager registrationManager = new RegistrationManager();
            registrationManager.TOBUpdate(clientObj.RegId, clientObj.tobStartDt, clientObj.tobEndTime);
        }

        #endregion
    }
}
