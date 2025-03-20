﻿using System.Windows;
using System.Windows.Input;
using Business;


namespace SmartFit
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        public static clsUser TestUser = new clsUser();

        private clsUser CurrentUser;
        private double _originalWidth, _originalHeight, _originalLeft, _originalTop;
        private bool _isMaximized = false;
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

        private void LoginMaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            if (_isMaximized) // Restore the window to its original size
            {
                Width = _originalWidth;
                Height = _originalHeight;
                Left = _originalLeft;
                Top = _originalTop;

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

                _isMaximized = true;
            }
        }

        private void LoginMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void LoginClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoginTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
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

        private void Login_Click(object sender, RoutedEventArgs e)
        {

            string username = EmailTEXTBOX.Text;
            string password = PasswordTEXTBOX.Password;

            if (!String.IsNullOrWhiteSpace(EmailTEXTBOX.Text) && !String.IsNullOrWhiteSpace(PasswordTEXTBOX.Password))
            {
                // Check login credentials (for example, admin / 1234)
                if (username.Trim() == TestUser.UserName && password.Trim() == "1234")
                {
                    MainWindow mainWindow = new MainWindow(); // Create Main Window
                    mainWindow.Show();  // Show the main window
                    this.Close();       // Close the login window
                }
                else
                {
                    MessageBox.Show("Invalid Username or Password!", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            else
            {
                MessageBox.Show("Enter your Username and Password!", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Simulate button click when Enter is pressed
        private void LoginWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login_Click(sender, e);
            }
        }

    }

}
