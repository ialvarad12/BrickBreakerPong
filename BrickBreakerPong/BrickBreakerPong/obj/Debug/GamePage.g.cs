﻿

#pragma checksum "C:\Users\A5mer_000\Documents\BrickBreakerPong\BrickBreakerPong\BrickBreakerPong\GamePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "CE60B09BC31E0D2A1EA985099F12CF05"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BrickBreakerPong
{
    partial class GamePage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 59 "..\..\GamePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.HomeButton_Clicked;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 60 "..\..\GamePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.HelpButton_Clicked;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 63 "..\..\GamePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.RestartGameButton_Clicked;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 64 "..\..\GamePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.NextLevelButton_Clicked;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


