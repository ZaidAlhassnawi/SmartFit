using System.Windows;
using System.Windows.Input;

namespace SmartFit
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Enable window dragging by clicking on the title bar
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
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
                MainBorder.CornerRadius = new CornerRadius(50);
                TitleBar.CornerRadius = new CornerRadius(50, 50, 0, 0);

                _isMaximized = false;
            }

            else // Maximize the window without making it fullscreen
            {
                // Save current size and position before maximizing
                _originalWidth = Width;
                _originalHeight = Height;
                _originalLeft = Left;
                _originalTop = Top;

                // Get the screen working area (excluding taskbar)
                var screen = SystemParameters.WorkArea;

                Left = screen.Left;
                Top = screen.Top;
                Width = screen.Width;
                Height = screen.Height;

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
