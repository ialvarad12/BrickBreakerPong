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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BrickBreakerPong
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        // Made static in order for the Game class to have access
        // to the sound effects
        public static MediaElement sfx;

        Game game;
        Level level;
        private int levelNumber;
        private DispatcherTimer timer;
        public GamePage()
        {
            this.InitializeComponent();

            // Get the size of the screen
            mainGrid.Height = Window.Current.Bounds.Height;
            mainGrid.Width = Window.Current.Bounds.Width;

            // Draw the top and bottom wall
            topWall.Width = mainGrid.Width;
            bottomWall.Width = mainGrid.Width;

            // Move the bottom wall to the bottom of the screen
            bottomWall.Margin = new Thickness(0, mainGrid.Height - bottomWall.Height, 0, 0);

            // Connect the view with the model
            game = new Game();
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

            // Reads a file to create the bricks
            level = new Level();
            levelNumber = 3;
            LoadLevel();

            newGameLevel.Visibility = Visibility.Collapsed;
            replayLevel.Visibility = Visibility.Collapsed;

            // Event for stopping and playing the game
            CoreWindow.GetForCurrentThread().KeyDown += MainPage_KeyDown;

            // Create a reference to the media element in the xaml
            sfx = soundEffects;
            sfx.DefaultPlaybackRate = 6.0; // Plays sound effects faster

            timer = new DispatcherTimer();
            timer.Start();
            timer.Tick += timer_Tick;

            // Game Over Labels
            gameOverLabel.Visibility = Visibility.Collapsed;
            winningPlayer.Visibility = Visibility.Collapsed;

            // Play music!
            musicPlayer.Play();
        }

        private async void LoadLevel()
        {
            string text = await level.LoadFileAsync("lvl_" + levelNumber + ".txt");
            if (text != null)
            {
                level.CreateLevel(this, text, game);
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
                        TurnOnInstructions();
                        timer.Stop();
                    }
                    else
                    {
                        musicPlayer.Play();
                        game.Continue();
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
                    newGameLevel.Visibility = Visibility.Collapsed;
                    replayLevel.Visibility = Visibility.Collapsed;
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
                    newGameLevel.Visibility = Visibility.Collapsed;
                    replayLevel.Visibility = Visibility.Collapsed;
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
            startGameLabel.Visibility = Visibility.Collapsed;
            restartLevelLabel.Visibility = Visibility.Collapsed;

        }
        
        private void TurnOnInstructions()
        {
            startGameLabel.Visibility = Visibility.Visible;
            restartLevelLabel.Visibility = Visibility.Visible;
        }

        // Have the view reflect the model
        void UpdateGrid()
        {
            rightPaddle.Margin = new Thickness(game.rightPaddle.Position.X, game.rightPaddle.Position.Y, 0, 0);
            leftPaddle.Margin  = new Thickness(game.leftPaddle.Position.X, game.leftPaddle.Position.Y, 0, 0);
            ball.Margin = new Thickness(game.ball.Position.X, game.ball.Position.Y, 0, 0);

            scoreLeft.Text = game.scoreLeft.ToString();
            scoreRight.Text = game.scoreRight.ToString();

            if(game.IsGameOver())
            {
                gameOverLabel.Visibility = Visibility.Visible;

                if (game.scoreLeft > game.scoreRight)
                    winningPlayer.Text = "Player 1 Wins!";
                else
                    winningPlayer.Text = "Player 2 Wins!";

                winningPlayer.Visibility = Visibility.Visible;

                newGameLevel.Visibility = Visibility.Visible;
                replayLevel.Visibility = Visibility.Visible;
            }
        }
    }
}
