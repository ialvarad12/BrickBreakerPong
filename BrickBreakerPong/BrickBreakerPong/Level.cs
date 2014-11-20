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
using Windows.UI.Xaml.Shapes;

namespace BrickBreakerPong
{
    class Level
    {

        public Level()
        {
        }

        public void CreateLevel(MainPage mainPage, string text, Game game)
        {
            Rectangle rect;

            double distanceBetweenPaddles = (uint)(mainPage.rightPaddle.Margin.Left - (mainPage.leftPaddle.Margin.Left + mainPage.leftPaddle.Width));
            double distanceBetweenWalls = game.boardHeight - mainPage.topWall.Height - mainPage.bottomWall.Height;
            double ratio = 25.0;

            int row = 0, col = 0;
            foreach (var line in text)
            {
                if(col == 27) // takes into account the '\n' in the text file
                {
                    row++;
                    col = 0;
                }

                rect = new Rectangle();
                rect.Fill = new SolidColorBrush(Colors.Gray);
                rect.Stroke = new SolidColorBrush(Colors.White);
                rect.Width = distanceBetweenPaddles / ratio;
                rect.Height = distanceBetweenWalls / ratio;
                rect.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                rect.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
                rect.Margin = new Windows.UI.Xaml.Thickness(mainPage.leftPaddle.Margin.Left + mainPage.leftPaddle.Width + (col * distanceBetweenPaddles / ratio),
                                                            mainPage.topWall.Height + (row * distanceBetweenWalls / ratio), 0, 0);

                // Don't know exactly why it goes out of bounds, but this prevents it :)
                if (mainPage.topWall.Height + (col * distanceBetweenWalls / ratio) < distanceBetweenWalls &&
                    mainPage.leftPaddle.Margin.Left + mainPage.leftPaddle.Width + (row * distanceBetweenPaddles / ratio) < distanceBetweenPaddles)
                {
                    if (line == '1')
                    {
                        // adds the brick to the page
                        mainPage.mainGrid.Children.Add(rect);
                        // adds to the bricks list
                        game.AddBrick(rect);
                    }
                }

                col++;
            }
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
