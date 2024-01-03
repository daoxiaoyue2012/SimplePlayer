using SimplePlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SimplePlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;

        public List<string> ImageList { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            List<string> specifiedOrder = new() { "Play", "Stop", "Rewind", "Forward" };
            List<string> fileList = new();

            ImageList = new List<string>();

            string[] files = Directory.GetFiles("C:\\Users\\java2\\Desktop\\icons");
            foreach (var file in files)
            {
                if (System.IO.Path.GetExtension(file).Equals(".png", StringComparison.OrdinalIgnoreCase))
                {
                    fileList.Add(file);
                }
            }
            ImageList = fileList.OrderBy(x => specifiedOrder.IndexOf(System.IO.Path.GetFileNameWithoutExtension(x))).ToList();
            DataContext = this;

            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            seekBar.Value = mediaElement.Position.TotalSeconds;
        }

        private void DockBar_DockBarButtonClick(object sender, DockBarButtonClickEventArgs e)
        {
            if (e.ButtonIndex == 0) //Play
            {
                mediaElement.Play();
            }
            else if (e.ButtonIndex == 1) //Stop
            {
                mediaElement.Stop();
            }
            else if (e.ButtonIndex == 2)    //Rewind
            {
                mediaElement.Position = TimeSpan.FromSeconds(mediaElement.Position.TotalSeconds - 15);
            }
            else if (e.ButtonIndex == 3) //Forward
            {
                mediaElement.Position = TimeSpan.FromSeconds(mediaElement.Position.TotalSeconds + 15);
            }
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (((DataObject)e.Data).GetFileDropList()[0] is string filename)
            {
                mediaElement.Source = new Uri(filename, UriKind.Relative);
                mediaElement.LoadedBehavior = MediaState.Manual;
                mediaElement.UnloadedBehavior = MediaState.Manual;

                timer.Start();
                mediaElement.Play();
            }
        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            TimeSpan timeSpan = mediaElement.NaturalDuration.TimeSpan;
            seekBar.Maximum = timeSpan.TotalSeconds;

            Width = Math.Max(Math.Min(mediaElement.NaturalVideoWidth, 1000), 500);
            Height = mediaElement.NaturalVideoHeight * Width / mediaElement.NaturalVideoWidth
                + container.RowDefinitions[0].ActualHeight + container.RowDefinitions[2].ActualHeight + seekBar.ActualHeight;
        }

        private void seekBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Position = TimeSpan.FromSeconds(seekBar.Value);
        }
    }
}
