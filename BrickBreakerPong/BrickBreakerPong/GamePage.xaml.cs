using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Runtime.InteropServices;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.Storage;
using System.Diagnostics;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using BrickBreakerPong.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BrickBreakerPong
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        // Took off static and passed to game class
        // static was causing audio issues when navigating through different pages
        public MediaElement sfx;

        Game game;
        Level level;
        private int levelNumber;
        private DispatcherTimer timer;

        int numOfPlayers;
        public GamePage()
        {
            //game = new Game(this, numOfPlayers);

            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            // Get the size of the screen
            mainGrid.Height = Window.Current.Bounds.Height;
            mainGrid.Width = Window.Current.Bounds.Width;

            // Draw the top and bottom wall
            topWall.Width = mainGrid.Width;
            bottomWall.Width = mainGrid.Width;

            // Move the bottom wall to the bottom of the screen
            bottomWall.Margin = new Thickness(0, mainGrid.Height - bottomWall.Height, 0, 0);

            //// Connect the view with the model
            ////game = new Game();
            //game.ball.Width = ball.Width;
            //game.ball.Height = ball.Height;
            //game.leftPaddle.Height = leftPaddle.Height;
            //game.leftPaddle.Width = leftPaddle.Width;
            //game.rightPaddle.Height = rightPaddle.Height;
            //game.rightPaddle.Width = rightPaddle.Width;
            //game.topWall.Height = topWall.Height;
            //game.bottomWall.Height = bottomWall.Height;

            //// Call game.Update any time you want the model to reflect the view
            //// (USED AFTER YOU MAKE CHANGES TO THE MODEL)
            //game.Update();

            //// Call UpdateGrid any time you want the view to reflect the model
            //UpdateGrid();

            //// Reads a file to create the bricks
            //level = new Level();
            //levelNumber = 1;
            //LoadLevel();


            //// Event for stopping and playing the game
            //CoreWindow.GetForCurrentThread().KeyDown += MainPage_KeyDown;

            //// Create a reference to the media element in the xaml
            //sfx = soundEffects;
            //soundEffects.DefaultPlaybackRate = 6.0; // Plays sound effects faster

            //timer = new DispatcherTimer();
            //timer.Start();
            //timer.Tick += timer_Tick;

            //// Game Over Labels
            //gameOverLabel.Visibility = Visibility.Collapsed;
            //winningPlayer.Visibility = Visibility.Collapsed;
            ////newGameLevel.Visibility = Visibility.Collapsed;
            ////replayLevel.Visibility = Visibility.Collapsed;

            // Play music!
            musicPlayer.Play();
        }

        private void CreateGame(Game game)
        {
            // Connect the view with the model
            //game = new Game();
            game.ball.Width = ball.Width;
            game.ball.Height = ball.Height;
            game.leftPaddle.Height = leftPaddle.Height;
            game.leftPaddle.Width = leftPaddle.Width;
            game.rightPaddle.Height = rightPaddle.Height;
            game.rightPaddle.Width = rightPaddle.Width;
            game.topWall.Height = topWall.Height;
            game.bottomWall.Height = bottomWall.Height;

            // Call game.Update any time you want the model to reflect the view
            // (USED AFTER YOU MAKE CHANGES TO THE MODEL)
            game.Update();

            // Call UpdateGrid any time you want the view to reflect the model
            UpdateGrid();

            //// Reads a file to create the bricks
            //if (level == null)
            //{
            //    level = new Level();
            //    levelNumber = 1;
            //    LoadLevel();
            //}

            // Event for stopping and playing the game
            CoreWindow.GetForCurrentThread().KeyDown += MainPage_KeyDown;

            // Create a reference to the media element in the xaml
            sfx = soundEffects;
            soundEffects.DefaultPlaybackRate = 6.0; // Plays sound effects faster

            timer = new DispatcherTimer();
            timer.Start();
            timer.Tick += timer_Tick;

            // Game Over Labels
            gameOverLabel.Visibility = Visibility.Collapsed;
            winningPlayer.Visibility = Visibility.Collapsed;
            //newGameLevel.Visibility = Visibility.Collapsed;
            //replayLevel.Visibility = Visibility.Collapsed;
        }

        public void RemoveCellAtIndex(int index)
        {
            int count = 0;
            for(int row = 0; row < 25; row++)
            {
                for(int col = 0; col < 25; col++)
                {
                    if(count < index && level.levelArray[row, col] == 1)
                    {
                        count++;
                    }
                    else if(count == index && level.levelArray[row, col] == 1)
                    {
                        level.levelArray[row, col] = 0;
                        row = 25; col = 25; // end the loops
                    }
                }
            }
        }

        void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // Save session data
            e.PageState["bricksArray"] = BricksToString();

            // Save app data
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["bricksArray"] = BricksToString();
        }

        void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            string numberOfPlayers = e.NavigationParameter as string;

            if(numberOfPlayers == "1")
            {
                numOfPlayers = 1;
            }
            else if(numberOfPlayers == "2")
            {
                numOfPlayers = 2;
            }
            
            // Restore session data
            if(e.PageState != null && e.PageState.ContainsKey("bricksArray"))
            {
                StringToBricks(e.PageState["bricksArray"].ToString());
            }

            // Restore app data
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey("brickArray"))
                StringToBricks(roamingSettings.Values["bricksArray"].ToString());

            game = new Game(this, numOfPlayers);
            CreateGame(game);

            bool result = false;
            AskUserToContinue(result, roamingSettings.Values["bricksArray"].ToString());
            

            //// Reads a file to create the bricks
            //if (level == null)
            //{
            //    level = new Level();
            //    levelNumber = 1;
            //    LoadLevel();
            //}
            //else
            //{
            //    level = new Level();
            //    level.CreateLevel(this, e.PageState["bricksArray"].ToString(), game, false);
            //}
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private string BricksToString()
        {
            string bricks = "";

            for (int row = 0; row < 25; row++)
            {
                for (int col = 0; col < 25; col++)
                {
                    if (level.levelArray[row, col] == 0)
                        bricks += "F";
                    else if (level.levelArray[row, col] == 1)
                        bricks += "T";
                }
            }

            return bricks;
        }
        private void StringToBricks(string Key)
        {
            for(int i = 0, j = 0; i < 25; i++, j++)
            {
                if(Key[j] == 'T')
                {
                    level.levelArray[i, j] = 1;
                }
                else if(Key[j] == 'F')
                {
                    level.levelArray[i, j] = 0;
                }
            }
        }

        private async void AskUserToContinue(bool result, string Key)
        {
            MessageDialog msg = new MessageDialog("Would you like to continue from your last saved game?", "Continue?");
            msg.Commands.Add(new UICommand("Yes", null, "YES"));
            msg.Commands.Add(new UICommand("No", null, "NO"));
            var op = await msg.ShowAsync();
            if ((string)op.Id == "YES")
                result = true;

            if (!result)
            {
                level = new Level();
                levelNumber = 1;
                LoadLevel();
            }
            else
            {
                level = new Level();
                level.CreateLevel(this, Key, game, false);
            }
        }

        private async void LoadLevel()
        {
            string text = await level.LoadFileAsync("lvl_" + levelNumber + ".txt");
            if (text != null)
            {
                level.CreateLevel(this, text, game, true);
            }
        }

        void timer_Tick(object sender, object e)
        {
            if (game.IsInPlay())
            {
                game.Run();
                UpdateGrid();
            }
        }

        void MainPage_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (!game.IsGameOver())
            {
                if (args.VirtualKey.ToString() == "Space")
                {
                    if (game.IsInPlay())
                    {
                        musicPlayer.Pause();
                        game.Pause();
                        this.BottomAppBar.IsOpen = true;
                        TurnOnInstructions();
                        timer.Stop();
                    }
                    else
                    {
                        musicPlayer.Play();
                        game.Continue();
                        this.BottomAppBar.IsOpen = false;
                        TurnOffInstructions();
                        timer.Start();
                    }

                }

                if (args.VirtualKey.ToString() == "F5")
                {
                    TurnOnInstructions();
                    game.Restart();
                    UpdateGrid();
                    timer.Stop();
                }
            }
            else
            {
                // game over screen 
                if (args.VirtualKey.ToString() == "F4")
                {
                    //newGameLevel.Visibility = Visibility.Collapsed;
                    //replayLevel.Visibility = Visibility.Collapsed;
                    gameOverLabel.Visibility = Visibility.Collapsed;
                    winningPlayer.Visibility = Visibility.Collapsed;

                    if (++levelNumber > 3)
                        levelNumber = 1;

                    LoadLevel();
                    game.NewGame();
                    UpdateGrid();
                    timer.Stop();
                }

                if(args.VirtualKey.ToString() == "F6")
                {
                    //newGameLevel.Visibility = Visibility.Collapsed;
                    //replayLevel.Visibility = Visibility.Collapsed;
                    gameOverLabel.Visibility = Visibility.Collapsed;
                    winningPlayer.Visibility = Visibility.Collapsed;

                    //LoadLevel();
                    game.Restart();
                    UpdateGrid();
                    timer.Stop();
                }
            }
        }
       
        private void TurnOffInstructions()
        {
            //startGameLabel.Visibility = Visibility.Collapsed;
            //restartLevelLabel.Visibility = Visibility.Collapsed;

        }
        
        private void TurnOnInstructions()
        {
            //startGameLabel.Visibility = Visibility.Visible;
            //restartLevelLabel.Visibility = Visibility.Visible;
        }

        // Have the view reflect the model
        void UpdateGrid()
        {
            rightPaddle.Margin = new Thickness(game.rightPaddle.Position.X, game.rightPaddle.Position.Y, 0, 0);
            leftPaddle.Margin  = new Thickness(game.leftPaddle.Position.X, game.leftPaddle.Position.Y, 0, 0);
            ball.Margin = new Thickness(game.ball.Position.X, game.ball.Position.Y, 0, 0);

            scoreLeft.Text = game.scoreLeft.ToString();
            scoreRight.Text = game.scoreRight.ToString();

            if(game.bricks.Count == 0)
            {
                game.ball.Speed += .01;
            }

            if(game.IsGameOver())
            {
                gameOverLabel.Visibility = Visibility.Visible;

                if (game.scoreLeft > game.scoreRight)
                    winningPlayer.Text = "Player 1 Wins!";
                else
                    winningPlayer.Text = "Player 2 Wins!";

                winningPlayer.Visibility = Visibility.Visible;

                this.BottomAppBar.IsOpen = true;
                //newGameLevel.Visibility = Visibility.Visible;
                //replayLevel.Visibility = Visibility.Visible;
            }
        }

        private void RestartGameButton_Clicked(object sender, RoutedEventArgs e)
        {
            game.Restart();
            UpdateGrid();
            timer.Stop();
        }

        private void NextLevelButton_Clicked(object sender, RoutedEventArgs e)
        {
            gameOverLabel.Visibility = Visibility.Collapsed;
            winningPlayer.Visibility = Visibility.Collapsed;

            if (++levelNumber > 3)
                levelNumber = 1;

            for (int i = 0; i < game.bricks.Count(); i++)
            {
                game.bricks[i].Visibility = Visibility.Collapsed;
            }

            LoadLevel();
            game.NewGame();
            UpdateGrid();
            timer.Stop();
        }

        private void HomeButton_Clicked(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            musicPlayer.Stop();
            this.Frame.Navigate(typeof(MenuPage));
        }

        private void HelpButton_Clicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
