using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimplePlayer
{
    /// <summary>
    /// Interaction logic for DockBar.xaml
    /// </summary>
    public partial class DockBar : UserControl
    {
        public DockBar()
        {
            InitializeComponent();
        }

        public List<string> ImageList
        {
            get { return (List<string>)GetValue(ImageListProperty); }
            set { SetValue(ImageListProperty, value); }
        }

        public static readonly DependencyProperty ImageListProperty =
            DependencyProperty.Register("ImageList", typeof(List<string>), typeof(DockBar), new PropertyMetadata(new List<string>(), new PropertyChangedCallback(OnImageListChanged)));

        public int ImageDimension
        {
            get { return (int)GetValue(ImageDimensionProperty); }
            set { SetValue(ImageDimensionProperty, value); }
        }

        public static readonly DependencyProperty ImageDimensionProperty =
            DependencyProperty.Register("ImageDimension", typeof(int), typeof(DockBar), new PropertyMetadata(0));


        private static void OnImageListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DockBar dockBar)
            {
                RowDefinition row1 = new()
                {
                    Height = new GridLength(1, GridUnitType.Star)
                };
                RowDefinition row2 = new()
                {
                    Height = new GridLength(dockBar.ImageDimension + 10)
                };
                dockBar.root.RowDefinitions.Add(row1);
                dockBar.root.RowDefinitions.Add(row2);

                ColumnDefinition col1 = new()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                ColumnDefinition col2 = new()
                {
                    Width = new GridLength((dockBar.ImageDimension + 10) * dockBar.ImageList.Count)
                };
                ColumnDefinition col3 = new()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                dockBar.root.ColumnDefinitions.Add(col1);
                dockBar.root.ColumnDefinitions.Add(col2);
                dockBar.root.ColumnDefinitions.Add(col3);

                Border borderOfBar = new()
                {
                    CornerRadius = new CornerRadius((dockBar.ImageDimension + 10) / 2),
                    Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x32, 0x33, 0x45))
                };

                Grid dockBarContainer = new();
                borderOfBar.Child = dockBarContainer;

                Grid.SetRow(borderOfBar, 1);
                Grid.SetColumn(borderOfBar, 1);

                dockBar.root.Children.Add(borderOfBar);
                for (int i=0; i<dockBar.ImageList.Count; i++)
                {
                    ColumnDefinition columnDefinition = new()
                    {
                        Width = new GridLength(60)
                    };
                    dockBarContainer.ColumnDefinitions.Add(columnDefinition);
                }

                for (var i=0; i<dockBar.ImageList.Count; i++)
                {
                    Button button = new()
                    {
                        Style = (Style)dockBar.FindResource("DockBar.ButtonStyle"),
                        Background = new ImageBrush(new BitmapImage(new Uri(dockBar.ImageList[i], UriKind.Relative)))
                        {
                            Stretch = Stretch.Uniform
                        }
                    };
                    button.Click += (sender, args) =>
                    {
                        dockBar.OnDockBarButtonClick((Button)sender, Grid.GetColumn((Button)sender));
                    };

                    Popup popup = new()
                    {
                        Style = (Style)dockBar.FindResource("DockBar.Button.PopupStyle")
                    };

                    Border border = new()
                    {
                        Style = (Style)(dockBar.FindResource("DockBar.Button.Popup.BorderStyle"))
                    };

                    TextBlock textBlock = new()
                    {
                        Text = System.IO.Path.GetFileNameWithoutExtension(dockBar.ImageList[i]),
                        Style = (Style)(dockBar.FindResource("DockBar.Button.Popup.TextBlockStyle"))
                    };
                    border.Child = textBlock;

                    Path path = new()
                    {
                        Style = (Style)((dockBar.FindResource("DockBar.Button.Popup.PathStyle")))
                    };

                    Grid grid = new();
                    grid.Children.Add(border); 
                    grid.Children.Add(path);

                    popup.Child = grid;
                    popup.PlacementTarget = button;
                    popup.Opened += Popup_Opened;

                    BindingOperations.SetBinding(popup, Popup.IsOpenProperty, new Binding
                    {
                        Path = new PropertyPath("IsMouseOver"),
                        Source = button,
                        Mode = BindingMode.OneWay
                    });

                    button.Content = popup;
                    Grid.SetColumn(button, i);
                    dockBarContainer.Children.Add(button);

                    AttachMouseEnterAnimation(button, dockBar.ImageDimension + 10, dockBar.ImageDimension + 10);
                    AttachMouseLeaveAnimation(button, dockBar.ImageDimension, dockBar.ImageDimension);
                }
            }
        }

        private static void Popup_Opened(object? sender, EventArgs e)
        {
            if (sender is Popup popup)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    double offset = (((Button)popup.Parent).ActualWidth - popup.Child.RenderSize.Width) / 2;
                    popup.HorizontalOffset = offset;
                }), System.Windows.Threading.DispatcherPriority.Render);
            }
        }

        private static void AttachMouseEnterAnimation(Button button, int width, int height)
        {
            Storyboard storyboard = new();
            DoubleAnimation widthAnimation = new()
            {
                To = width,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(widthAnimation, button);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(Button.WidthProperty));
            storyboard.Children.Add(widthAnimation);

            DoubleAnimation heightAnimation = new()
            {
                To = height,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(heightAnimation, button);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(Button.HeightProperty));
            storyboard.Children.Add(heightAnimation);

            button.MouseEnter += (sender, e) => storyboard.Begin(button);
        }

        private static void AttachMouseLeaveAnimation(Button button, int width, int height)
        {
            Storyboard storyboard = new();
            DoubleAnimation widthAnimation = new()
            {
                To = width,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(widthAnimation, button);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(Button.WidthProperty));
            storyboard.Children.Add(widthAnimation);

            DoubleAnimation heightAnimation = new()
            {
                To = height,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(heightAnimation, button);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(Button.HeightProperty));
            storyboard.Children.Add(heightAnimation);

            button.MouseLeave += (sender, e) => storyboard.Begin(button);
        }

        public event EventHandler<DockBarButtonClickEventArgs>? DockBarButtonClick;
        protected virtual void OnDockBarButtonClick(Button clickedButton, int buttonIndex)
        {
            DockBarButtonClick?.Invoke(this, new DockBarButtonClickEventArgs(clickedButton, buttonIndex));
        }
    }

    public class DockBarButtonClickEventArgs : EventArgs
    {
        public Button ClickedButton { get; }
        public int ButtonIndex { get; }

        public DockBarButtonClickEventArgs(Button clickedButton, int buttonIndex)
        {
            ClickedButton = clickedButton;
            ButtonIndex = buttonIndex;
        }
    }
}
