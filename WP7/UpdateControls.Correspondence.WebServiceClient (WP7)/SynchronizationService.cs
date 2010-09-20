﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 4.0.50826.0
// 
namespace UpdateControls.Correspondence.WebServiceClient
{
    using System.Runtime.Serialization;


    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "FactTree", Namespace = "http://correspondence.updatecontrols.com")]
    public partial class FactTree : object, System.ComponentModel.INotifyPropertyChanged
    {

        private long DatabaseIdField;

        private System.Collections.ObjectModel.ObservableCollection<Fact> FactsField;

        private System.Collections.ObjectModel.ObservableCollection<FactRole> RolesField;

        private long TimestampField;

        private System.Collections.ObjectModel.ObservableCollection<FactType> TypesField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public long DatabaseId
        {
            get
            {
                return this.DatabaseIdField;
            }
            set
            {
                if ((this.DatabaseIdField.Equals(value) != true))
                {
                    this.DatabaseIdField = value;
                    this.RaisePropertyChanged("DatabaseId");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<Fact> Facts
        {
            get
            {
                return this.FactsField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FactsField, value) != true))
                {
                    this.FactsField = value;
                    this.RaisePropertyChanged("Facts");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<FactRole> Roles
        {
            get
            {
                return this.RolesField;
            }
            set
            {
                if ((object.ReferenceEquals(this.RolesField, value) != true))
                {
                    this.RolesField = value;
                    this.RaisePropertyChanged("Roles");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Timestamp
        {
            get
            {
                return this.TimestampField;
            }
            set
            {
                if ((this.TimestampField.Equals(value) != true))
                {
                    this.TimestampField = value;
                    this.RaisePropertyChanged("Timestamp");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<FactType> Types
        {
            get
            {
                return this.TypesField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TypesField, value) != true))
                {
                    this.TypesField = value;
                    this.RaisePropertyChanged("Types");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Fact", Namespace = "http://correspondence.updatecontrols.com")]
    public partial class Fact : object, System.ComponentModel.INotifyPropertyChanged
    {

        private byte[] DataField;

        private long FactIdField;

        private int FactTypeIdField;

        private System.Collections.ObjectModel.ObservableCollection<Predecessor> PredecessorsField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] Data
        {
            get
            {
                return this.DataField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DataField, value) != true))
                {
                    this.DataField = value;
                    this.RaisePropertyChanged("Data");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public long FactId
        {
            get
            {
                return this.FactIdField;
            }
            set
            {
                if ((this.FactIdField.Equals(value) != true))
                {
                    this.FactIdField = value;
                    this.RaisePropertyChanged("FactId");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int FactTypeId
        {
            get
            {
                return this.FactTypeIdField;
            }
            set
            {
                if ((this.FactTypeIdField.Equals(value) != true))
                {
                    this.FactTypeIdField = value;
                    this.RaisePropertyChanged("FactTypeId");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<Predecessor> Predecessors
        {
            get
            {
                return this.PredecessorsField;
            }
            set
            {
                if ((object.ReferenceEquals(this.PredecessorsField, value) != true))
                {
                    this.PredecessorsField = value;
                    this.RaisePropertyChanged("Predecessors");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "FactRole", Namespace = "http://correspondence.updatecontrols.com")]
    public partial class FactRole : object, System.ComponentModel.INotifyPropertyChanged
    {

        private int DeclaringTypeIdField;

        private bool IsPivotField;

        private int RoleIdField;

        private string RoleNameField;

        private int TargetTypeIdField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DeclaringTypeId
        {
            get
            {
                return this.DeclaringTypeIdField;
            }
            set
            {
                if ((this.DeclaringTypeIdField.Equals(value) != true))
                {
                    this.DeclaringTypeIdField = value;
                    this.RaisePropertyChanged("DeclaringTypeId");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsPivot
        {
            get
            {
                return this.IsPivotField;
            }
            set
            {
                if ((this.IsPivotField.Equals(value) != true))
                {
                    this.IsPivotField = value;
                    this.RaisePropertyChanged("IsPivot");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int RoleId
        {
            get
            {
                return this.RoleIdField;
            }
            set
            {
                if ((this.RoleIdField.Equals(value) != true))
                {
                    this.RoleIdField = value;
                    this.RaisePropertyChanged("RoleId");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RoleName
        {
            get
            {
                return this.RoleNameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.RoleNameField, value) != true))
                {
                    this.RoleNameField = value;
                    this.RaisePropertyChanged("RoleName");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TargetTypeId
        {
            get
            {
                return this.TargetTypeIdField;
            }
            set
            {
                if ((this.TargetTypeIdField.Equals(value) != true))
                {
                    this.TargetTypeIdField = value;
                    this.RaisePropertyChanged("TargetTypeId");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "FactType", Namespace = "http://correspondence.updatecontrols.com")]
    public partial class FactType : object, System.ComponentModel.INotifyPropertyChanged
    {

        private int TypeIdField;

        private string TypeNameField;

        private int VersionField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TypeId
        {
            get
            {
                return this.TypeIdField;
            }
            set
            {
                if ((this.TypeIdField.Equals(value) != true))
                {
                    this.TypeIdField = value;
                    this.RaisePropertyChanged("TypeId");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TypeName
        {
            get
            {
                return this.TypeNameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TypeNameField, value) != true))
                {
                    this.TypeNameField = value;
                    this.RaisePropertyChanged("TypeName");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Version
        {
            get
            {
                return this.VersionField;
            }
            set
            {
                if ((this.VersionField.Equals(value) != true))
                {
                    this.VersionField = value;
                    this.RaisePropertyChanged("Version");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Predecessor", Namespace = "http://correspondence.updatecontrols.com")]
    public partial class Predecessor : object, System.ComponentModel.INotifyPropertyChanged
    {

        private long PredecessorIdField;

        private int RoleIdField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public long PredecessorId
        {
            get
            {
                return this.PredecessorIdField;
            }
            set
            {
                if ((this.PredecessorIdField.Equals(value) != true))
                {
                    this.PredecessorIdField = value;
                    this.RaisePropertyChanged("PredecessorId");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int RoleId
        {
            get
            {
                return this.RoleIdField;
            }
            set
            {
                if ((this.RoleIdField.Equals(value) != true))
                {
                    this.RoleIdField = value;
                    this.RaisePropertyChanged("RoleId");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://correspondence.updatecontrols.com", ConfigurationName = "ServiceReference1.ISynchronizationService")]
    public interface ISynchronizationService
    {

        [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://correspondence.updatecontrols.com/ISynchronizationService/Get", ReplyAction = "http://correspondence.updatecontrols.com/ISynchronizationService/GetResponse")]
        System.IAsyncResult BeginGet(FactTree pivotTree, long pivotId, long timestamp, System.AsyncCallback callback, object asyncState);

        FactTree EndGet(System.IAsyncResult result);

        [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://correspondence.updatecontrols.com/ISynchronizationService/Post", ReplyAction = "http://correspondence.updatecontrols.com/ISynchronizationService/PostResponse")]
        System.IAsyncResult BeginPost(FactTree messageBody, System.AsyncCallback callback, object asyncState);

        void EndPost(System.IAsyncResult result);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ISynchronizationServiceChannel : ISynchronizationService, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        public GetCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        public FactTree Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return ((FactTree)(this.results[0]));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SynchronizationServiceClient : System.ServiceModel.ClientBase<ISynchronizationService>, ISynchronizationService
    {

        private BeginOperationDelegate onBeginGetDelegate;

        private EndOperationDelegate onEndGetDelegate;

        private System.Threading.SendOrPostCallback onGetCompletedDelegate;

        private BeginOperationDelegate onBeginPostDelegate;

        private EndOperationDelegate onEndPostDelegate;

        private System.Threading.SendOrPostCallback onPostCompletedDelegate;

        private BeginOperationDelegate onBeginOpenDelegate;

        private EndOperationDelegate onEndOpenDelegate;

        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;

        private BeginOperationDelegate onBeginCloseDelegate;

        private EndOperationDelegate onEndCloseDelegate;

        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;

        public SynchronizationServiceClient()
        {
        }

        public SynchronizationServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public SynchronizationServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public SynchronizationServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public SynchronizationServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public System.Net.CookieContainer CookieContainer
        {
            get
            {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null))
                {
                    return httpCookieContainerManager.CookieContainer;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null))
                {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else
                {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }

        public event System.EventHandler<GetCompletedEventArgs> GetCompleted;

        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> PostCompleted;

        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;

        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult ISynchronizationService.BeginGet(FactTree pivotTree, long pivotId, long timestamp, System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginGet(pivotTree, pivotId, timestamp, callback, asyncState);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        FactTree ISynchronizationService.EndGet(System.IAsyncResult result)
        {
            return base.Channel.EndGet(result);
        }

        private System.IAsyncResult OnBeginGet(object[] inValues, System.AsyncCallback callback, object asyncState)
        {
            FactTree pivotTree = ((FactTree)(inValues[0]));
            long pivotId = ((long)(inValues[1]));
            long timestamp = ((long)(inValues[2]));
            return ((ISynchronizationService)(this)).BeginGet(pivotTree, pivotId, timestamp, callback, asyncState);
        }

        private object[] OnEndGet(System.IAsyncResult result)
        {
            FactTree retVal = ((ISynchronizationService)(this)).EndGet(result);
            return new object[] {
                    retVal};
        }

        private void OnGetCompleted(object state)
        {
            if ((this.GetCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetCompleted(this, new GetCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }

        public void GetAsync(FactTree pivotTree, long pivotId, long timestamp)
        {
            this.GetAsync(pivotTree, pivotId, timestamp, null);
        }

        public void GetAsync(FactTree pivotTree, long pivotId, long timestamp, object userState)
        {
            if ((this.onBeginGetDelegate == null))
            {
                this.onBeginGetDelegate = new BeginOperationDelegate(this.OnBeginGet);
            }
            if ((this.onEndGetDelegate == null))
            {
                this.onEndGetDelegate = new EndOperationDelegate(this.OnEndGet);
            }
            if ((this.onGetCompletedDelegate == null))
            {
                this.onGetCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetCompleted);
            }
            base.InvokeAsync(this.onBeginGetDelegate, new object[] {
                        pivotTree,
                        pivotId,
                        timestamp}, this.onEndGetDelegate, this.onGetCompletedDelegate, userState);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult ISynchronizationService.BeginPost(FactTree messageBody, System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginPost(messageBody, callback, asyncState);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void ISynchronizationService.EndPost(System.IAsyncResult result)
        {
            base.Channel.EndPost(result);
        }

        private System.IAsyncResult OnBeginPost(object[] inValues, System.AsyncCallback callback, object asyncState)
        {
            FactTree messageBody = ((FactTree)(inValues[0]));
            return ((ISynchronizationService)(this)).BeginPost(messageBody, callback, asyncState);
        }

        private object[] OnEndPost(System.IAsyncResult result)
        {
            ((ISynchronizationService)(this)).EndPost(result);
            return null;
        }

        private void OnPostCompleted(object state)
        {
            if ((this.PostCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.PostCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }

        public void PostAsync(FactTree messageBody)
        {
            this.PostAsync(messageBody, null);
        }

        public void PostAsync(FactTree messageBody, object userState)
        {
            if ((this.onBeginPostDelegate == null))
            {
                this.onBeginPostDelegate = new BeginOperationDelegate(this.OnBeginPost);
            }
            if ((this.onEndPostDelegate == null))
            {
                this.onEndPostDelegate = new EndOperationDelegate(this.OnEndPost);
            }
            if ((this.onPostCompletedDelegate == null))
            {
                this.onPostCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnPostCompleted);
            }
            base.InvokeAsync(this.onBeginPostDelegate, new object[] {
                        messageBody}, this.onEndPostDelegate, this.onPostCompletedDelegate, userState);
        }

        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState)
        {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }

        private object[] OnEndOpen(System.IAsyncResult result)
        {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }

        private void OnOpenCompleted(object state)
        {
            if ((this.OpenCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }

        public void OpenAsync()
        {
            this.OpenAsync(null);
        }

        public void OpenAsync(object userState)
        {
            if ((this.onBeginOpenDelegate == null))
            {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null))
            {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null))
            {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }

        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState)
        {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }

        private object[] OnEndClose(System.IAsyncResult result)
        {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }

        private void OnCloseCompleted(object state)
        {
            if ((this.CloseCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }

        public void CloseAsync()
        {
            this.CloseAsync(null);
        }

        public void CloseAsync(object userState)
        {
            if ((this.onBeginCloseDelegate == null))
            {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null))
            {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null))
            {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }

        protected override ISynchronizationService CreateChannel()
        {
            return new SynchronizationServiceClientChannel(this);
        }

        private class SynchronizationServiceClientChannel : ChannelBase<ISynchronizationService>, ISynchronizationService
        {

            public SynchronizationServiceClientChannel(System.ServiceModel.ClientBase<ISynchronizationService> client) :
                base(client)
            {
            }

            public System.IAsyncResult BeginGet(FactTree pivotTree, long pivotId, long timestamp, System.AsyncCallback callback, object asyncState)
            {
                object[] _args = new object[3];
                _args[0] = pivotTree;
                _args[1] = pivotId;
                _args[2] = timestamp;
                System.IAsyncResult _result = base.BeginInvoke("Get", _args, callback, asyncState);
                return _result;
            }

            public FactTree EndGet(System.IAsyncResult result)
            {
                object[] _args = new object[0];
                FactTree _result = ((FactTree)(base.EndInvoke("Get", _args, result)));
                return _result;
            }

            public System.IAsyncResult BeginPost(FactTree messageBody, System.AsyncCallback callback, object asyncState)
            {
                object[] _args = new object[1];
                _args[0] = messageBody;
                System.IAsyncResult _result = base.BeginInvoke("Post", _args, callback, asyncState);
                return _result;
            }

            public void EndPost(System.IAsyncResult result)
            {
                object[] _args = new object[0];
                base.EndInvoke("Post", _args, result);
            }
        }
    }
}
