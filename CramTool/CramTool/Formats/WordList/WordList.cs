﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.17929.
// 
namespace CramTool.Formats.WordList {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://CramTool.WordList-1.0.0")]
    [System.Xml.Serialization.XmlRootAttribute("WordList", Namespace="http://CramTool.WordList-1.0.0", IsNullable=true)]
    public partial class WordListXml {
        
        private WordXml[] wordsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Word", IsNullable=false)]
        public WordXml[] Words {
            get {
                return this.wordsField;
            }
            set {
                this.wordsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://CramTool.WordList-1.0.0")]
    public partial class WordXml {
        
        private string nameField;
        
        private string descriptionField;
        
        private string tagsField;
        
        private WordEventXml[] eventsField;
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string Description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
        
        /// <remarks/>
        public string Tags {
            get {
                return this.tagsField;
            }
            set {
                this.tagsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("WordEvent", IsNullable=false)]
        public WordEventXml[] Events {
            get {
                return this.eventsField;
            }
            set {
                this.eventsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://CramTool.WordList-1.0.0")]
    public partial class WordEventXml {
        
        private System.DateTime eventDateField;
        
        private WordEventTypeXml eventTypeField;
        
        /// <remarks/>
        public System.DateTime EventDate {
            get {
                return this.eventDateField;
            }
            set {
                this.eventDateField = value;
            }
        }
        
        /// <remarks/>
        public WordEventTypeXml EventType {
            get {
                return this.eventTypeField;
            }
            set {
                this.eventTypeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://CramTool.WordList-1.0.0")]
    public enum WordEventTypeXml {
        
        /// <remarks/>
        Added,
        
        /// <remarks/>
        Remembered,
        
        /// <remarks/>
        Forgotten,
        
        /// <remarks/>
        Forgot,
    }
}
