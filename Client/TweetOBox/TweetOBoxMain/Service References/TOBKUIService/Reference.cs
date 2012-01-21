﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TweetOBoxMain.TOBKUIService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="TOBKUIService.ITOBKUIService")]
    public interface ITOBKUIService {
        
        // CODEGEN: Generating message contract since the wrapper name (ClientRegisterRequest) of message ClientRegisterRequest does not match the default value (RegisterTOB)
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITOBKUIService/RegisterTOB", ReplyAction="http://tempuri.org/ITOBKUIService/RegisterTOBResponse")]
        TweetOBoxMain.TOBKUIService.TOBRegistrationResponseMessage RegisterTOB(TweetOBoxMain.TOBKUIService.ClientRegisterRequest request);
        
        // CODEGEN: Generating message contract since the operation TOBUpdateKUI is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITOBKUIService/TOBUpdateKUI", ReplyAction="http://tempuri.org/ITOBKUIService/TOBUpdateKUIResponse")]
        TweetOBoxMain.TOBKUIService.TOBUpdateKUIResponse TOBUpdateKUI(TweetOBoxMain.TOBKUIService.ClientKUIRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ClientRegisterRequest", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class ClientRegisterRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public long ticks;
        
        public ClientRegisterRequest() {
        }
        
        public ClientRegisterRequest(long ticks) {
            this.ticks = ticks;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="TOBRegistrationResponseMessage", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class TOBRegistrationResponseMessage {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string RegistrationId;
        
        public TOBRegistrationResponseMessage() {
        }
        
        public TOBRegistrationResponseMessage(string RegistrationId) {
            this.RegistrationId = RegistrationId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ClientKUIRequest", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class ClientKUIRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string RegId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public System.DateTime tobEndTime;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public System.DateTime tobStartDt;
        
        public ClientKUIRequest() {
        }
        
        public ClientKUIRequest(string RegId, System.DateTime tobEndTime, System.DateTime tobStartDt) {
            this.RegId = RegId;
            this.tobEndTime = tobEndTime;
            this.tobStartDt = tobStartDt;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class TOBUpdateKUIResponse {
        
        public TOBUpdateKUIResponse() {
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ITOBKUIServiceChannel : TweetOBoxMain.TOBKUIService.ITOBKUIService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TOBKUIServiceClient : System.ServiceModel.ClientBase<TweetOBoxMain.TOBKUIService.ITOBKUIService>, TweetOBoxMain.TOBKUIService.ITOBKUIService {
        
        public TOBKUIServiceClient() {
        }
        
        public TOBKUIServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TOBKUIServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TOBKUIServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TOBKUIServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TweetOBoxMain.TOBKUIService.TOBRegistrationResponseMessage TweetOBoxMain.TOBKUIService.ITOBKUIService.RegisterTOB(TweetOBoxMain.TOBKUIService.ClientRegisterRequest request) {
            return base.Channel.RegisterTOB(request);
        }
        
        public string RegisterTOB(long ticks) {
            TweetOBoxMain.TOBKUIService.ClientRegisterRequest inValue = new TweetOBoxMain.TOBKUIService.ClientRegisterRequest();
            inValue.ticks = ticks;
            TweetOBoxMain.TOBKUIService.TOBRegistrationResponseMessage retVal = ((TweetOBoxMain.TOBKUIService.ITOBKUIService)(this)).RegisterTOB(inValue);
            return retVal.RegistrationId;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TweetOBoxMain.TOBKUIService.TOBUpdateKUIResponse TweetOBoxMain.TOBKUIService.ITOBKUIService.TOBUpdateKUI(TweetOBoxMain.TOBKUIService.ClientKUIRequest request) {
            return base.Channel.TOBUpdateKUI(request);
        }
        
        public void TOBUpdateKUI(string RegId, System.DateTime tobEndTime, System.DateTime tobStartDt) {
            TweetOBoxMain.TOBKUIService.ClientKUIRequest inValue = new TweetOBoxMain.TOBKUIService.ClientKUIRequest();
            inValue.RegId = RegId;
            inValue.tobEndTime = tobEndTime;
            inValue.tobStartDt = tobStartDt;
            TweetOBoxMain.TOBKUIService.TOBUpdateKUIResponse retVal = ((TweetOBoxMain.TOBKUIService.ITOBKUIService)(this)).TOBUpdateKUI(inValue);
        }
    }
}
