﻿#pragma checksum "..\..\..\..\Group Windows\EditGroup.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "444681B1F14AFAB32C993F63160946CDBD382AE1"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Client.Group_Windows;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Client.Group_Windows {
    
    
    /// <summary>
    /// EditGroup
    /// </summary>
    public partial class EditGroup : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\..\..\Group Windows\EditGroup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox Groups_ListBox;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\Group Windows\EditGroup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox newName_TextBox;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\Group Windows\EditGroup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ChangeName_Button;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\..\Group Windows\EditGroup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox Users_ListBox;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\..\Group Windows\EditGroup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddButton;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\..\Group Windows\EditGroup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox UsersInGroup_ListBox;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\..\Group Windows\EditGroup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RemoveButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.6.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Client;component/group%20windows/editgroup.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Group Windows\EditGroup.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.6.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Groups_ListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 2:
            this.newName_TextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.ChangeName_Button = ((System.Windows.Controls.Button)(target));
            
            #line 38 "..\..\..\..\Group Windows\EditGroup.xaml"
            this.ChangeName_Button.Click += new System.Windows.RoutedEventHandler(this.ChangeName_Button_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Users_ListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 5:
            this.AddButton = ((System.Windows.Controls.Button)(target));
            
            #line 55 "..\..\..\..\Group Windows\EditGroup.xaml"
            this.AddButton.Click += new System.Windows.RoutedEventHandler(this.AddButton_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.UsersInGroup_ListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 7:
            this.RemoveButton = ((System.Windows.Controls.Button)(target));
            
            #line 66 "..\..\..\..\Group Windows\EditGroup.xaml"
            this.RemoveButton.Click += new System.Windows.RoutedEventHandler(this.RemoveButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

