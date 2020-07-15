using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPMahjong
{
    public sealed partial class MainPage : Page
    {
        Random rand = new Random();
        int tappedTag = -1, tappedCol = -1, tappedRow = -1;

        public MainPage()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            SetupGrid();
        }

        private void SetupGrid()
        {
            tappedTag = -1;
            tappedCol = -1;
            tappedRow = -1;
            MainGrid.RowDefinitions.Clear();
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.Children.Clear();

            for (int i = 0; i < 6; i++)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition());
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 1; i < 19; i++)
            {
                int counter = 0;
                while (counter < 2)
                {
                    int row = rand.Next(0, 6);
                    int col = rand.Next(0, 6);
                    if (MainGrid.Children.Cast<Image>().Any(img => Grid.GetColumn(img) == col && Grid.GetRow(img) == row)) continue;

                    Image image = new Image
                    {
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Stretch = Windows.UI.Xaml.Media.Stretch.Fill,
                        Source = GetImage(i),
                        Margin = new Thickness(1, 1, 1, 1),
                        Tag = i
                    };
                    image.Tapped += Image_Tapped;
                    MainGrid.Children.Add(image);
                    Grid.SetColumn(image, col);
                    Grid.SetRow(image, row);
                    counter++;
                }
            }
            foreach (var img in MainGrid.Children.Cast<Image>())
            {
                img.Source = img.Source;
            }
            HideImages();
        }

        private BitmapImage GetImage(int tag)
        {
            return new BitmapImage(new Uri($"ms-appx:///Assets/Images/{tag}.jpg"));
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var image = sender as Image;
            DisplayImage(image);
            int tag = Convert.ToInt32(image.Tag);
            int row = Grid.GetRow(image);
            int col = Grid.GetColumn(image);
            if (tappedTag != -1)
            {
                if ((Grid.GetColumn(image) != tappedCol || Grid.GetRow(image) != tappedRow) && tappedTag == tag)
                {
                    foreach(var img in MainGrid.Children.Cast<Image>().Where(i => Convert.ToInt32(i.Tag) == tag))
                    {
                        img.IsTapEnabled = false;
                        img.Visibility = Visibility.Collapsed;
                    }
                }
            }
            tappedTag = tag;
            tappedRow = row;
            tappedCol = col;
            if (IsGameOver())
                SetupGrid();
        }

        private bool IsGameOver()
        {
            return !MainGrid.Children.Cast<Image>().Any(i => i.Visibility != Visibility.Collapsed);
        }

        private void DisplayImage(Image image)
        {
            new Task(async () =>
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    image.Source = GetImage(Convert.ToInt32(image.Tag));
                });
                Task.Delay(2000).Wait();
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    image.Source = GetImage(0);
                });
            }).Start();
        }

        private void HideImages()
        {
            new Task(async () =>
            {
                for (int i = 5; i > 0; i--)
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Timer.Text = i.ToString();
                        if (i == 1) Timer.Text = "";
                    });
                    Task.Delay(1000).Wait();
                }
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (var image in MainGrid.Children.Cast<Image>())
                    {
                        image.Source = GetImage(0);
                    }
                });
            }).Start();
        }
    }
}