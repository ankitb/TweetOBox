using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using TOBKPI.BLL;


namespace TOBKUIWCFService
{
    [MessageContract]
    public class TOBStartLoginRequestMessage
    {
        [MessageHeader]
        public string RegistrationId;
      
    }

    [MessageContract]
    public class ClientKUIRequest
    {
      [MessageBodyMember]
       public string RegId;
      [MessageBodyMember]
       public DateTime tobStartDt;
      [MessageBodyMember]
       public DateTime tobEndTime;
      
   }

    [MessageContract]
    public class ClientRegisterRequest
    {
        [MessageBodyMember]
        public long ticks;
    }
}
