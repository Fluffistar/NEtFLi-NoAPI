﻿#pragma checksum "D:\Projekte\NOAPI\NEtFLi\NEtFLi\MainPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "C091F01AC6E913E39634C014A965AD885984CC6A0853DA8C987C60795F60767C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NEtFLi
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.19041.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1: // MainPage.xaml line 1
                {
                    global::Windows.UI.Xaml.Controls.Page element1 = (global::Windows.UI.Xaml.Controls.Page)(target);
                    ((global::Windows.UI.Xaml.Controls.Page)element1).Loaded += this.Page_Loaded;
                }
                break;
            case 2: // MainPage.xaml line 35
                {
                    this.navbar = (global::Windows.UI.Xaml.Controls.NavigationView)(target);
                }
                break;
            case 3: // MainPage.xaml line 45
                {
                    global::Windows.UI.Xaml.Controls.NavigationViewItem element3 = (global::Windows.UI.Xaml.Controls.NavigationViewItem)(target);
                    ((global::Windows.UI.Xaml.Controls.NavigationViewItem)element3).Tapped += this.NavigationViewItem_Tapped;
                }
                break;
            case 4: // MainPage.xaml line 53
                {
                    this.autobox = (global::Windows.UI.Xaml.Controls.AutoSuggestBox)(target);
                    ((global::Windows.UI.Xaml.Controls.AutoSuggestBox)this.autobox).QuerySubmitted += this.autobox_QuerySubmitted;
                    ((global::Windows.UI.Xaml.Controls.AutoSuggestBox)this.autobox).TextChanged += this.autobox_TextChanged;
                }
                break;
            case 5: // MainPage.xaml line 54
                {
                    this.info = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 6: // MainPage.xaml line 65
                {
                    this.viewer = (global::Windows.UI.Xaml.Controls.ScrollViewer)(target);
                }
                break;
            case 7: // MainPage.xaml line 72
                {
                    this.content = (global::Windows.UI.Xaml.Controls.Frame)(target);
                }
                break;
            case 8: // MainPage.xaml line 67
                {
                    this.SerienList = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                }
                break;
            case 9: // MainPage.xaml line 55
                {
                    this.imginfo = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 10: // MainPage.xaml line 56
                {
                    this.Fsk = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 11: // MainPage.xaml line 57
                {
                    this.b1 = (global::Windows.UI.Xaml.Controls.Border)(target);
                }
                break;
            case 12: // MainPage.xaml line 60
                {
                    this.genres = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                }
                break;
            case 13: // MainPage.xaml line 62
                {
                    this.Description = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 14: // MainPage.xaml line 58
                {
                    this.Title = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.19041.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

