using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using TOBKPI.BLL;

namespace TOBKUIWCFService
{
    [MessageContract]
    public class TOBRegistrationResponseMessage
    {
        [MessageBodyMember]
        public string RegistrationId;
    }
}
