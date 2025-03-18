using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiveCharts;

namespace SmartFit
{
    public partial class MainWindow : Window
    {
        public class ChartViewModel
        {
            public ChartValues<double> ActivityValues { get; set; }
            public string[] Days { get; set; }
            public Func<double, string> YAxisLabelFormatter { get; set; }

            public ChartViewModel()
            {
                // Activity durations in minutes
                ActivityValues = new ChartValues<double> { 45, 75, 45, 105, 30, 100, 60 };
                Days = new string[] { "Sat", "Sun", "Mon", "Tue", "Wed", "Thu", "Fri" };

                // Custom formatter for y-axis labels
                YAxisLabelFormatter = value =>
                {
                    return value + "m";
                };
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ChartViewModel();
        }


        private bool isFullscreen = false;

        private void ToggleFullscreen()
        {
            if (isFullscreen)
            {
                // Exit Fullscreen
                this.WindowState = WindowState.Normal;
                this.ResizeMode = ResizeMode.CanResize;
            }


            isFullscreen = !isFullscreen;
        }


        // Enable window dragging by clicking on the title bar
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    ToggleFullscreen();
                }
            }
        }

        // Minimize window
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Maximize or Restore window
        // Store original size and position
        private double _originalWidth, _originalHeight, _originalLeft, _originalTop;

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "ابحث عن تمرينك")
            {
                SearchBox.IsReadOnly = false;
                SearchBox.Text = "";
                SearchBox.Foreground = Brushes.Black;
            }

        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "ابحث عن تمرينك";
                SearchBox.Foreground = Brushes.Gray;
                SearchBox.IsReadOnly = true;
            }
        }

        private void TBTypeFavoriteEXC_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TBTypeFavoriteEXC.Text == "اختياري")
            {
                TBTypeFavoriteEXC.IsReadOnly = false;
                TBTypeFavoriteEXC.Text = "";
                TBTypeFavoriteEXC.Foreground = Brushes.Black;
            }

        }

        private void TBTypeFavoriteEXC_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBTypeFavoriteEXC.Text))
            {
                TBTypeFavoriteEXC.Text = "اختياري";
                TBTypeFavoriteEXC.Foreground = Brushes.Gray;
                TBTypeFavoriteEXC.IsReadOnly = true;
            }
        }

        private void HomeBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                UserPopupScreen.IsOpen = false;
                AiPopupScreen.IsOpen = false;
                WindowsContainer.SelectedIndex = 0;

                HomeBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                HomeText.Foreground = Brushes.White;
                HomeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/vector.png"));

                ExcBorder.Background = Brushes.White;
                ExcText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                ExcIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Dumbbells2.png"));

                AiBorder.Background = Brushes.White;
                AiText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                AiIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/StarsMinimalistic2.png"));

                Userborder.Background = Brushes.White;
                UserText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                UserIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/User2.png"));

            }
        }

        private void ExcBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                UserPopupScreen.IsOpen = false;
                AiPopupScreen.IsOpen = false;
                WindowsContainer.SelectedIndex = 1;

                ExcBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                ExcText.Foreground = Brushes.White;
                ExcIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Dumbbells1.png"));

                HomeBorder.Background = Brushes.White;
                HomeText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                HomeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Home.png"));

                AiBorder.Background = Brushes.White;
                AiText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                AiIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/StarsMinimalistic2.png"));

                Userborder.Background = Brushes.White;
                UserText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                UserIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/User2.png"));

            }
        }

        private bool _IsUserEnterPlanInfos = false;

        private void AiBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                UserPopupScreen.IsOpen = false;

                WindowsContainer.SelectedIndex = 2;

                if (_IsUserEnterPlanInfos) AiPopupScreen.IsOpen = false;

                else AiPopupScreen.IsOpen = true;

                AiBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                AiText.Foreground = Brushes.White;
                AiIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/StarsMinimalistic1.png"));

                ExcBorder.Background = Brushes.White;
                ExcText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                ExcIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Dumbbells3.png"));

                HomeBorder.Background = Brushes.White;
                HomeText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                HomeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Home.png"));

                Userborder.Background = Brushes.White;
                UserText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                UserIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/User2.png"));

            }
        }

        private void Userborder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {

                UserPopupScreen.IsOpen = true;


                Userborder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                UserText.Foreground = Brushes.White;
                UserIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/User1.png"));

                ExcBorder.Background = Brushes.White;
                ExcText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                ExcIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Dumbbells2.png"));

                HomeBorder.Background = Brushes.White;
                HomeText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                HomeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Home.png"));

                AiBorder.Background = Brushes.White;
                AiText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
                AiIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/StarsMinimalistic2.png"));
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UserPopupScreen.IsOpen = false;
            WindowsContainer.SelectedIndex = 1;

            ExcBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
            ExcText.Foreground = Brushes.White;
            ExcIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Dumbbells1.png"));


            HomeBorder.Background = Brushes.White;
            HomeText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
            HomeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Home.png"));

            Userborder.Background = Brushes.White;
            UserText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
            UserIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/User2.png"));
        }

        private void btnMakeThePlanPopupWindown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((CBSelectAge.SelectedItem != null)
                && (CBSelectHeigt.SelectedItem != null)
                && (CBSelectWeight.SelectedItem != null)
                && (CBSelectSportingGoals.SelectedItem != null)
                && (CBSelectFitnessLevel.SelectedItem != null)
                && (CBSelectEXCTime.SelectedItem != null)
                && (CBSelectTrainingDaysOfWeek.SelectedItem != null)
                && (CBSelectEXCPlace.SelectedItem != null)
                && (CBSelectPlanType.SelectedItem != null)
                )
            {
                AiPopupScreen.IsOpen = false;
                _IsUserEnterPlanInfos = true;
            }
        }

        private void btnSaveTheNewUserInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UserPopupScreen.IsOpen = false;
        }

        private void UserPhoto_MouseDown(object sender, MouseButtonEventArgs e)
        {

            UserPopupScreen.IsOpen = true;


            Userborder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
            UserText.Foreground = Brushes.White;
            UserIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/User1.png"));

            ExcBorder.Background = Brushes.White;
            ExcText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
            ExcIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Dumbbells2.png"));

            HomeBorder.Background = Brushes.White;
            HomeText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
            HomeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Home.png"));

            AiBorder.Background = Brushes.White;
            AiText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7AB0"));
            AiIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/StarsMinimalistic2.png"));
        }


        private bool isDragging = false;
        private Point clickPosition;
        private void draggableBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            clickPosition = e.GetPosition(this); // Get initial mouse position relative to the window
            draggableBorder.CaptureMouse(); // Capture the mouse to track movement
        }

        private void draggableBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(this);

                double offsetX = currentPosition.X - clickPosition.X;
                double offsetY = currentPosition.Y - clickPosition.Y;

                // Move the Border
                draggableBorder.Margin = new Thickness(
                    draggableBorder.Margin.Left + offsetX,
                    draggableBorder.Margin.Top + offsetY,
                    0, 0);

                clickPosition = currentPosition; // Update position for smooth dragging
            }
        }

        private void draggableBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            draggableBorder.ReleaseMouseCapture(); // Release the mouse
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UserPopupScreen.IsOpen = false;
        }

        private void btnMakeYourPlanWithAi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowsContainer.SelectedIndex = 2;
            UserPopupScreen.IsOpen = false;


            if (_IsUserEnterPlanInfos) AiPopupScreen.IsOpen = false;
            else AiPopupScreen.IsOpen = true;
        }

        private void btnMakeNewPlan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_IsUserEnterPlanInfos)
            {
                WindowsContainer.SelectedIndex = 1;
            }

        }

        private void btnSuggestNewPlan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_IsUserEnterPlanInfos)
            {
                _IsUserEnterPlanInfos = false;
                AiPopupScreen.IsOpen = true;
                CBSelectAge.SelectedItem = null;
                CBSelectHeigt.SelectedItem = null;
                CBSelectWeight.SelectedItem = null;
                CBSelectSportingGoals.SelectedItem = null;
                CBSelectFitnessLevel.SelectedItem = null;
                CBSelectEXCTime.SelectedItem = null;
                CBSelectTrainingDaysOfWeek.SelectedItem = null;
                CBSelectEXCPlace.SelectedItem = null;
                CBSelectPlanType.SelectedItem = null;
                TBTypeFavoriteEXC.Text = "اختياري";
            }

        }

        private bool _isMaximized = false;

        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            if (_isMaximized) // Restore the window to its original size
            {
                Width = _originalWidth;
                Height = _originalHeight;
                Left = _originalLeft;
                Top = _originalTop;

                // Restore rounded corners
                MainBorder.CornerRadius = new CornerRadius(10);
                TitleBar.CornerRadius = new CornerRadius(10, 10, 0, 0);
                ToggleFullscreen();

                _isMaximized = false;
            }

            else // Maximize the window without making it fullscreen
            {
                // Save current size and position before maximizing
                _originalWidth = Width;
                _originalHeight = Height;
                _originalLeft = Left;
                _originalTop = Top;

                //Get the screen working area(excluding taskbar)
                var screen = SystemParameters.WorkArea;

                Left = screen.Left - 10;
                Top = screen.Top - 10;
                Width = screen.Width + 20;
                Height = screen.Height + 15;


                // Remove the border radius when maximized
                MainBorder.CornerRadius = new CornerRadius(0);
                TitleBar.CornerRadius = new CornerRadius(0);

                _isMaximized = true;
            }
        }

        // Close window
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
