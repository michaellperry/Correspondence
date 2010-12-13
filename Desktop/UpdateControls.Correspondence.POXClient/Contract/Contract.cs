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
// This source code was auto-generated by xsd, Version=4.0.30319.1.
// 
namespace UpdateControls.Correspondence.POXClient.Contract {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=true)]
    public partial class FactTree {
        
        private long databaseIdField;
        
        private Fact[] factsField;
        
        private FactRole[] rolesField;
        
        private FactType[] typesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public long DatabaseId {
            get {
                return this.databaseIdField;
            }
            set {
                this.databaseIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        [System.Xml.Serialization.XmlArrayItemAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Fact[] Facts {
            get {
                return this.factsField;
            }
            set {
                this.factsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        [System.Xml.Serialization.XmlArrayItemAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public FactRole[] Roles {
            get {
                return this.rolesField;
            }
            set {
                this.rolesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        [System.Xml.Serialization.XmlArrayItemAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public FactType[] Types {
            get {
                return this.typesField;
            }
            set {
                this.typesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=true)]
    public partial class Fact {
        
        private byte[] dataField;
        
        private long factIdField;
        
        private int factTypeIdField;
        
        private Predecessor[] predecessorsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary", IsNullable=true)]
        public byte[] Data {
            get {
                return this.dataField;
            }
            set {
                this.dataField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public long FactId {
            get {
                return this.factIdField;
            }
            set {
                this.factIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int FactTypeId {
            get {
                return this.factTypeIdField;
            }
            set {
                this.factTypeIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        [System.Xml.Serialization.XmlArrayItemAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Predecessor[] Predecessors {
            get {
                return this.predecessorsField;
            }
            set {
                this.predecessorsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=true)]
    public partial class Predecessor {
        
        private long predecessorIdField;
        
        private int roleIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public long PredecessorId {
            get {
                return this.predecessorIdField;
            }
            set {
                this.predecessorIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int RoleId {
            get {
                return this.roleIdField;
            }
            set {
                this.roleIdField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=true)]
    public partial class FactRole {
        
        private int declaringTypeIdField;
        
        private bool isPivotField;
        
        private int roleIdField;
        
        private string roleNameField;
        
        private int targetTypeIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int DeclaringTypeId {
            get {
                return this.declaringTypeIdField;
            }
            set {
                this.declaringTypeIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool IsPivot {
            get {
                return this.isPivotField;
            }
            set {
                this.isPivotField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int RoleId {
            get {
                return this.roleIdField;
            }
            set {
                this.roleIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string RoleName {
            get {
                return this.roleNameField;
            }
            set {
                this.roleNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int TargetTypeId {
            get {
                return this.targetTypeIdField;
            }
            set {
                this.targetTypeIdField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=true)]
    public partial class FactType {
        
        private int typeIdField;
        
        private string typeNameField;
        
        private int versionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int TypeId {
            get {
                return this.typeIdField;
            }
            set {
                this.typeIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string TypeName {
            get {
                return this.typeNameField;
            }
            set {
                this.typeNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int Version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=true)]
    public partial class ArrayOfFact {
        
        private Fact[] factField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Fact", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public Fact[] Fact {
            get {
                return this.factField;
            }
            set {
                this.factField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=true)]
    public partial class ArrayOfPredecessor {
        
        private Predecessor[] predecessorField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Predecessor", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public Predecessor[] Predecessor {
            get {
                return this.predecessorField;
            }
            set {
                this.predecessorField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=true)]
    public partial class ArrayOfFactRole {
        
        private FactRole[] factRoleField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FactRole", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public FactRole[] FactRole {
            get {
                return this.factRoleField;
            }
            set {
                this.factRoleField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=true)]
    public partial class ArrayOfFactType {
        
        private FactType[] factTypeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FactType", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public FactType[] FactType {
            get {
                return this.factTypeField;
            }
            set {
                this.factTypeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=false)]
    public partial class GetRequest {
        
        private FactTree pivotTreeField;
        
        private long pivotIdField;
        
        private long timestampField;
        
        private string clientGuidField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public FactTree PivotTree {
            get {
                return this.pivotTreeField;
            }
            set {
                this.pivotTreeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public long PivotId {
            get {
                return this.pivotIdField;
            }
            set {
                this.pivotIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public long Timestamp {
            get {
                return this.timestampField;
            }
            set {
                this.timestampField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ClientGuid {
            get {
                return this.clientGuidField;
            }
            set {
                this.clientGuidField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=false)]
    public partial class GetResponse {
        
        private FactTree factTreeField;
        
        private long timestampField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public FactTree FactTree {
            get {
                return this.factTreeField;
            }
            set {
                this.factTreeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public long Timestamp {
            get {
                return this.timestampField;
            }
            set {
                this.timestampField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=false)]
    public partial class PostRequest {
        
        private FactTree messageBodyField;
        
        private string clientGuidField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public FactTree MessageBody {
            get {
                return this.messageBodyField;
            }
            set {
                this.messageBodyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ClientGuid {
            get {
                return this.clientGuidField;
            }
            set {
                this.clientGuidField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=false)]
    public partial class PostResponse {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=false)]
    public partial class SubscribeRequest {
        
        private FactTree pivotTreeField;
        
        private long pivotIdField;
        
        private bool pivotIdFieldSpecified;
        
        private string deviceUriField;
        
        private string clientGuidField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public FactTree PivotTree {
            get {
                return this.pivotTreeField;
            }
            set {
                this.pivotTreeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public long PivotId {
            get {
                return this.pivotIdField;
            }
            set {
                this.pivotIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PivotIdSpecified {
            get {
                return this.pivotIdFieldSpecified;
            }
            set {
                this.pivotIdFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string DeviceUri {
            get {
                return this.deviceUriField;
            }
            set {
                this.deviceUriField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ClientGuid {
            get {
                return this.clientGuidField;
            }
            set {
                this.clientGuidField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=false)]
    public partial class UnsubscribeRequest {
        
        private FactTree pivotTreeField;
        
        private long pivotIdField;
        
        private bool pivotIdFieldSpecified;
        
        private string deviceUriField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public FactTree PivotTree {
            get {
                return this.pivotTreeField;
            }
            set {
                this.pivotTreeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public long PivotId {
            get {
                return this.pivotIdField;
            }
            set {
                this.pivotIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PivotIdSpecified {
            get {
                return this.pivotIdFieldSpecified;
            }
            set {
                this.pivotIdFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string DeviceUri {
            get {
                return this.deviceUriField;
            }
            set {
                this.deviceUriField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=false)]
    public partial class SubscribeResponse {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://correspondence.updatecontrols.com/pox/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://correspondence.updatecontrols.com/pox/1.0", IsNullable=false)]
    public partial class UnsubscribeResponse {
    }
}
