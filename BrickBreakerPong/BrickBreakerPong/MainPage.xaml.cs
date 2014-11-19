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
    public sealed partial class MainPage : Page
    {
        // Made static in order for the Game class to have access
        // to the sound effects
        public static MediaElement sfx;

        Game game;
        private DispatcherTimer timer;
        public MainPage()
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
            //CreateBricks();
            GetLevelString();

            // Event for stopping and playing the game
            CoreWindow.GetForCurrentThread().KeyDown += MainPage_KeyDown;

            // Create a reference to the media element in the xaml
            sfx = soundEffects;
            sfx.DefaultPlaybackRate = 6.0; // Plays sound effects faster


            timer = new DispatcherTimer();
            timer.Start();
            timer.Tick += timer_Tick;

            // Play music!
            musicPlayer.Play();
        }

        private async void GetLevelString()
        {
            string text = await LoadFileAsync();
            if (text != null)
            {
                Rectangle rect;

                double distanceBetweenPaddles = (uint)(rightPaddle.Margin.Left - (leftPaddle.Margin.Left + leftPaddle.Width));
                double distanceBetweenWalls = game.boardHeight - topWall.Height - bottomWall.Height;
                double ratio = 25.0;

                int row = 0, col = 0;
                foreach (var line in text)
                {
                    if (col == 27) // takes into account the '\n' in the text file
                    {
                        row++;
                        col = 0;
                    }

                    rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Colors.Gray);
                    rect.Stroke = new SolidColorBrush(Colors.White);
                    rect.Width = distanceBetweenPaddles / ratio;
                    rect.Height = distanceBetweenWalls / ratio;
                    rect.HorizontalAlignment = HorizontalAlignment.Left;
                    rect.VerticalAlignment = VerticalAlignment.Top;
                    rect.Margin = new Thickness(leftPaddle.Margin.Left + leftPaddle.Width + (col * distanceBetweenPaddles / ratio),
                                                topWall.Height + (row * distanceBetweenWalls / ratio), 0, 0);

                    // Don't know exactly why it goes out of bounds, but this prevents it :)
                    if (topWall.Height + (col * distanceBetweenWalls / ratio) < distanceBetweenWalls &&
                        leftPaddle.Margin.Left + leftPaddle.Width + (row * distanceBetweenPaddles / ratio) < distanceBetweenPaddles)
                    {
                        if(line == '1')
                        {
                            mainGrid.Children.Add(rect);

                            game.AddBrick(rect);
                        }
                    }

                    col++;
                }
            }
        }

        private async Task<string> LoadFileAsync()
        {
            Exception exception = null;

            string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            string path = root + @"\Levels";
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
            StorageFile storageFile = await folder.GetFileAsync("lvl_2.txt");

            if (storageFile == null)
                return null;

            try
            {
                using (IRandomAccessStream stream = await storageFile.OpenReadAsync())
                {
                    using (DataReader dataReader = new DataReader(stream))
                    {
                        uint length = (uint)stream.Size;
                        await dataReader.LoadAsync(length);
                        return dataReader.ReadString(length);
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                exception = ex;
            }

            if(exception != null)
            {
                MessageDialog msg = new MessageDialog("Sorry, but the levels file wasn't found.", "File not found");
                await msg.ShowAsync();
            }

            return null;
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

            if(args.VirtualKey.ToString() == "F5")
            {
                TurnOnInstructions();
                game.Restart();
                UpdateGrid();
                timer.Stop();
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
            rightPaddle.Margin = new Thickness(game.rightPaddle.Position.X,
                                               game.rightPaddle.Position.Y, 0, 0);
            leftPaddle.Margin  = new Thickness(game.leftPaddle.Position.X,
                                               game.leftPaddle.Position.Y, 0, 0);
            ball.Margin = new Thickness(game.ball.Position.X,
                                        game.ball.Position.Y, 0, 0);
        }
    }
}
