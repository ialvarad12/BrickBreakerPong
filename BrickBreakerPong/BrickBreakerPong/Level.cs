using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace BrickBreakerPong
{
    public class Level
    {
        public int[,] levelArray;

        public Level()
        {
            levelArray = new int[25, 27];
        }

        // Loads the file into a 2d array
        public void CreateLevel(string text, Game game, bool IsNewLevel)
        {
            if (IsNewLevel)
            {
                int row = 0, col = 0;
                foreach (var line in text)
                {

                    if (col == 27) // takes into account the '\n' in the text file
                    {
                        row++;
                        col = 0;
                    }

                    if (row < 25)
                    {
                        if (line == '1')
                        {
                            levelArray[row, col] = 1;
                        }
                        else
                        {
                            levelArray[row, col] = 0;
                        }
                    }
                    col++;
                }
            }
            else
            {
                for(int row = 0, count = 0; row < 25; row++)
                {
                    for(int col = 0; col < 25; col++, count++)
                    {
                        if (text[count] == 'T')
                            levelArray[row, col] = 1;
                        else
                            levelArray[row, col] = 0;

                    }
                }
            }

            double distanceBetweenPaddles = (uint)(game.rightPaddle.Position.X - (game.leftPaddle.Position.X + game.leftPaddle.Width));
            double distanceBetweenWalls = Game.boardHeight - game.topWall.Height - game.bottomWall.Height;

            CreateBricksForLevel(game, levelArray, distanceBetweenPaddles, distanceBetweenWalls);

            // WOOOOOOOOOOOOOWWWWWWWW
            int x = 9;
        }

        // uses the levelArray to create the gameboard
        private void CreateBricksForLevel(Game game, int[,] levelArray, double distanceBetweenPaddles, double distanceBetweenWalls)
        {
            Rectangle rect;
            Random rand = new Random();
            double ratio = 25.0;

            for (int row = 0; row < 25; row++)
            {
                for (int col = 0; col <= 27; col++)
                {
                    rect = new Rectangle();
                    SetBrickTextures(rect, rand);
                    rect.Width = distanceBetweenPaddles / ratio;
                    rect.Height = distanceBetweenWalls / ratio;
                    rect.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                    rect.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
                    rect.Margin = new Windows.UI.Xaml.Thickness(game.leftPaddle.Position.X + game.leftPaddle.Width + (col * distanceBetweenPaddles / ratio),
                                                                game.topWall.Height + (row * distanceBetweenWalls / ratio), 0, 0);
                    // Don't know exactly why it goes out of bounds, but this prevents it :)
                    if (game.topWall.Height + (col * distanceBetweenWalls / ratio) < distanceBetweenWalls &&
                        game.leftPaddle.Position.X + game.leftPaddle.Width + (row * distanceBetweenPaddles / ratio) < distanceBetweenPaddles)
                    {
                        if (col < 25)
                        {
                            if (levelArray[row, col] == 1)
                            {
                                // adds the brick to the page
                                GamePage.MainGrid.Children.Add(rect);
                                // adds to the bricks list
                                game.AddBrick(rect);
                            }
                        }
                    }
                }
            }
        }
        public void RemoveCellAtIndex(int index)
        {
            int count = 0;
            for (int row = 0; row < 25; row++)
            {
                for (int col = 0; col < 25; col++)
                {
                    if (count < index && this.levelArray[row, col] == 1)
                    {
                        count++;
                    }
                    else if (count == index && this.levelArray[row, col] == 1)
                    {
                        this.levelArray[row, col] = 0;
                        row = 25; col = 25; // end the loops
                    }
                }
            }
        }

        public void SetBrickTextures(Rectangle rect, Random rand)
        {
            ImageBrush brickTexture = new ImageBrush();
            int s = rand.Next(4);
            s += 1;

            brickTexture.ImageSource = new BitmapImage(
                    new Uri(Windows.ApplicationModel.Package.Current.InstalledLocation.Path +
                    @"\Assets\textures\brick_texture_" + s + ".png", UriKind.RelativeOrAbsolute));

            rect.Fill = brickTexture;
        }

        // Loads a Level from a text file
        public async Task<string> LoadFileAsync(string level_number)
        {
            Exception exception = null;

            string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            string path = root + @"\Levels";
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
            StorageFile storageFile = await folder.GetFileAsync(level_number);

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

            if (exception != null)
            {
                MessageDialog msg = new MessageDialog("Sorry, but the levels file wasn't found.", "File not found");
                await msg.ShowAsync();
            }

            return null;
        }
    }
}
