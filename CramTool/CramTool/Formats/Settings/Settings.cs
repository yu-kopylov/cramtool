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
namespace CramTool.Formats.Settings {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://CramTool.Settings-1.0.0")]
    [System.Xml.Serialization.XmlRootAttribute("Settings", Namespace="http://CramTool.Settings-1.0.0", IsNullable=true)]
    public partial class SettingsXml {
        
        private string[] recentFilesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("RecentFile", IsNullable=false)]
        public string[] RecentFiles {
            get {
                return this.recentFilesField;
            }
            set {
                this.recentFilesField = value;
            }
        }
    }
}
