﻿

#pragma checksum "C:\Users\aandrew1\Documents\BrickBreakerPong\BrickBreakerPong\BrickBreakerPong\GamePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "13BA93F29F8CC994E46900EF79555966"
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
    partial class GamePage : global::Windows.UI.Xaml.Controls.Page
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Page mainPage; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        public global::Windows.UI.Xaml.Controls.Grid mainGrid; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        public global::Windows.UI.Xaml.Shapes.Rectangle topWall; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        public global::Windows.UI.Xaml.Shapes.Rectangle bottomWall; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        public global::Windows.UI.Xaml.Shapes.Rectangle leftPaddle; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        public global::Windows.UI.Xaml.Shapes.Rectangle rightPaddle; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Shapes.Ellipse ball; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        public global::Windows.UI.Xaml.Controls.TextBox scoreLeft; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        public global::Windows.UI.Xaml.Controls.TextBox scoreRight; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        public global::Windows.UI.Xaml.Controls.TextBox gameOverLabel; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        public global::Windows.UI.Xaml.Controls.TextBox winningPlayer; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.MediaElement musicPlayer; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        public global::Windows.UI.Xaml.Controls.MediaElement soundEffects; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarButton NextLevelAppBar; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///GamePage.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            mainPage = (global::Windows.UI.Xaml.Controls.Page)this.FindName("mainPage");
            mainGrid = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("mainGrid");
            topWall = (global::Windows.UI.Xaml.Shapes.Rectangle)this.FindName("topWall");
            bottomWall = (global::Windows.UI.Xaml.Shapes.Rectangle)this.FindName("bottomWall");
            leftPaddle = (global::Windows.UI.Xaml.Shapes.Rectangle)this.FindName("leftPaddle");
            rightPaddle = (global::Windows.UI.Xaml.Shapes.Rectangle)this.FindName("rightPaddle");
            ball = (global::Windows.UI.Xaml.Shapes.Ellipse)this.FindName("ball");
            scoreLeft = (global::Windows.UI.Xaml.Controls.TextBox)this.FindName("scoreLeft");
            scoreRight = (global::Windows.UI.Xaml.Controls.TextBox)this.FindName("scoreRight");
            gameOverLabel = (global::Windows.UI.Xaml.Controls.TextBox)this.FindName("gameOverLabel");
            winningPlayer = (global::Windows.UI.Xaml.Controls.TextBox)this.FindName("winningPlayer");
            musicPlayer = (global::Windows.UI.Xaml.Controls.MediaElement)this.FindName("musicPlayer");
            soundEffects = (global::Windows.UI.Xaml.Controls.MediaElement)this.FindName("soundEffects");
            NextLevelAppBar = (global::Windows.UI.Xaml.Controls.AppBarButton)this.FindName("NextLevelAppBar");
        }
    }
}



