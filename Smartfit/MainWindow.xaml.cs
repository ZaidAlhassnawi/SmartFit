using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Business;
using LiveCharts;
using static Business.clsPlan;

namespace SmartFit
{
    public partial class MainWindow : Window
    {
        clsUser TestUser = LoginWindow.TestUser;

        private void InitializeUserInfoInApp(clsUser user)
        {
            SideOfThePhotoUserName.Content = TestUser.UserName;
            SideOfThePhotoUserAge.Content = TestUser.Age;
            TBUserName.Text = TestUser.UserName;
            TBUserAge.Text = TestUser.Age.ToString();
            TBUserEmail.Text = "Test@Email.com";
            if (TestUser.Gender == "Male") { RBmale.IsChecked = true; }
            else { RBFemale.IsEnabled = true; }

        }

        public MainWindow()
        {
            InitializeComponent();
            MakeYourAiPlanLabel();
            DataContext = new ChartViewModel();
            InitializeUserInfoInApp(TestUser);
        }

        // Maximize or Restore window
        // Store original size and position
        private double _originalWidth, _originalHeight, _originalLeft, _originalTop;
        private bool _isMaximized = false;
        private bool isFullscreen = false;
        private bool isDragging = false;
        private bool _IsUserEnterPlanInfos = false;
        private Point clickPosition;
        private byte ActiveWindow = 0;

        private void MakeYourAiPlanLabel()
        {
            Border MakeYouPlanWithAi = new Border
            {
                Background = Brushes.LightGray,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(15),
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Child = new TextBlock
                {
                    Text = "🚀 Make Your Plan With AI",
                    FontSize = 18,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center
                }
            };

            if (ExcPlanStackPanel.Children.Count == 0)
            {
                if (!ExcPlanStackPanel.Children.Contains(MakeYouPlanWithAi))
                    ExcPlanStackPanel.Children.Add(MakeYouPlanWithAi);
            }
            else
            {
                ExcPlanStackPanel.Children.Remove(MakeYouPlanWithAi);
            }
        }

        public class ChartViewModel
        {
            public ChartValues<double> ActivityValues { get; set; }
            public string[] Days { get; set; }
            public Func<double, string> YAxisLabelFormatter { get; set; }
            public ChartViewModel()
            {
                // Activity durations in minutes
                ActivityValues = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0 };
                Days = new string[] { "Sat", "Sun", "Mon", "Tue", "Wed", "Thu", "Fri" };

                // Custom formatter for y-axis labels
                YAxisLabelFormatter = value =>
                {
                    return value + "m";
                };
            }
        }

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

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void draggableBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == draggableBorder)
            {
                UserPopupScreen.Placement = PlacementMode.Absolute;
                isDragging = true;
                clickPosition = e.GetPosition(this); // Get initial mouse position relative to the window
                draggableBorder.CaptureMouse(); // Capture the mouse to track movement
            }

            else if (sender == BorderMeasuringCaloriesInFood)
            {
                PopUpMeasuringCaloriesInFood.Placement = PlacementMode.Absolute;
                isDragging = true;
                clickPosition = e.GetPosition(this); // Get initial mouse position relative to the window
                BorderMeasuringCaloriesInFood.CaptureMouse(); // Capture the mouse to track movement
            }

            else if (sender == BorderMeasuringCaloriesInPlan)
            {
                PopUpMeasuringCaloriesInPlan.Placement = PlacementMode.Absolute;
                isDragging = true;
                clickPosition = e.GetPosition(this); // Get initial mouse position relative to the window
                BorderMeasuringCaloriesInPlan.CaptureMouse(); // Capture the mouse to track movement
            }
        }

        private void draggableBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender == draggableBorder)
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

                else if (sender == BorderMeasuringCaloriesInFood)
                {
                    Point currentPosition = e.GetPosition(this);

                    double offsetX = currentPosition.X - clickPosition.X;
                    double offsetY = currentPosition.Y - clickPosition.Y;

                    // Move the Border
                    BorderMeasuringCaloriesInFood.Margin = new Thickness(
                        BorderMeasuringCaloriesInFood.Margin.Left + offsetX,
                        BorderMeasuringCaloriesInFood.Margin.Top + offsetY,
                        0, 0);

                    clickPosition = currentPosition; // Update position for smooth dragging
                }

                else if (sender == BorderMeasuringCaloriesInPlan)
                {
                    Point currentPosition = e.GetPosition(this);

                    double offsetX = currentPosition.X - clickPosition.X;
                    double offsetY = currentPosition.Y - clickPosition.Y;

                    // Move the Border
                    BorderMeasuringCaloriesInPlan.Margin = new Thickness(
                        BorderMeasuringCaloriesInPlan.Margin.Left + offsetX,
                        BorderMeasuringCaloriesInPlan.Margin.Top + offsetY,
                        0, 0);

                    clickPosition = currentPosition; // Update position for smooth dragging
                }
            }
        }

        private void draggableBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender == draggableBorder)
            {
                isDragging = false;
                draggableBorder.ReleaseMouseCapture(); // Release the mouse
            }

            else if (sender == BorderMeasuringCaloriesInFood)
            {
                isDragging = false;
                BorderMeasuringCaloriesInFood.ReleaseMouseCapture(); // Release the mouse
            }

            else if (sender == BorderMeasuringCaloriesInPlan)
            {
                isDragging = false;
                BorderMeasuringCaloriesInPlan.ReleaseMouseCapture(); // Release the mouse
            }

        }

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

        private void TextBoxAnimation(object sender, bool GotFocus)
        {
            if (sender == SearchBox)
            {
                if (GotFocus)
                {
                    if (SearchBox.Text == "ابحث عن تمرينك")
                    {
                        SearchBox.IsReadOnly = false;
                        SearchBox.Text = "";
                        SearchBox.Foreground = Brushes.Black;
                    }
                }

                else
                {
                    if (string.IsNullOrWhiteSpace(SearchBox.Text))
                    {
                        SearchBox.Text = "ابحث عن تمرينك";
                        SearchBox.Foreground = Brushes.Gray;
                        SearchBox.IsReadOnly = true;
                    }
                }
            }

            else if (sender == TBTypeFavoriteEXC)
            {
                if (GotFocus)
                {

                    if (TBTypeFavoriteEXC.Text == "اختياري")
                    {
                        TBTypeFavoriteEXC.IsReadOnly = false;
                        TBTypeFavoriteEXC.Text = "";
                        TBTypeFavoriteEXC.Foreground = Brushes.Black;
                    }
                }

                else
                {

                    if (string.IsNullOrWhiteSpace(TBTypeFavoriteEXC.Text))
                    {
                        TBTypeFavoriteEXC.Text = "اختياري";
                        TBTypeFavoriteEXC.Foreground = Brushes.Gray;
                        TBTypeFavoriteEXC.IsReadOnly = true;
                    }
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxAnimation(sender, true);

        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBoxAnimation(sender, false);
        }

        private void NavigationButtonsactions(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (sender == HomeBorder)
                {
                    ActiveWindow = 0;
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

                else if ((sender == ExcBorder) || (sender == btnContinuYourEXC))
                {
                    ActiveWindow = 1;

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

                else if ((sender == AiBorder) || (sender == btnMakeYourPlanWithAi))
                {
                    ActiveWindow = 2;

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

                else if ((sender == Userborder) || (sender == btnUserPhoto))
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
        }

        private void NavigationButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NavigationButtonsactions(sender, e);
        }

        private void AddExerciseToStackPanel(StackPanel targetStackPanel, string exerciseName, string reps, string imagePath = "")
        {
            // Create Border
            Border exerciseBorder = new Border
            {
                CornerRadius = new CornerRadius(32),
                Margin = new Thickness(16, 12, 16, 12),
                Width = double.NaN, // Auto width
                Height = 147,
                Background = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Cursor = Cursors.Hand
            };

            // Create Grid
            Grid exerciseGrid = new Grid
            {
                Margin = new Thickness(10),
                Width = double.NaN, // Auto width
                Height = 127,
                VerticalAlignment = VerticalAlignment.Top
            };

            // Column Definitions
            exerciseGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Background Grid (Left)
            Grid leftBackgroundGrid = new Grid
            {
                Background = Brushes.White,
                Margin = new Thickness(0, 0, 861, 0)
            };

            // Exercise Image
            Image exerciseImage = new Image
            {
                Source = new BitmapImage(new Uri(imagePath, UriKind.Relative)), // Local path
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 172
            };

            // Background Grid (Right)
            Grid rightBackgroundGrid = new Grid
            {
                Background = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 861,
                Margin = new Thickness(176, 0, 0, 0)
            };

            // Exercise Name TextBlock
            TextBlock exerciseTextBlock = new TextBlock
            {
                FontSize = 24,
                Text = exerciseName,
                Margin = new Thickness(181, 10, 0, 72),
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 236
            };

            // Reps Label
            Label repsLabel = new Label
            {
                FontSize = 24,
                Content = $"×{reps}",
                Margin = new Thickness(181, 90, 0, 0),
                IsEnabled = false,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 236
            };

            // Add elements to Grid
            exerciseGrid.Children.Add(leftBackgroundGrid);
            exerciseGrid.Children.Add(exerciseImage);
            exerciseGrid.Children.Add(rightBackgroundGrid);
            exerciseGrid.Children.Add(exerciseTextBlock);
            exerciseGrid.Children.Add(repsLabel);

            // Set Grid inside Border
            exerciseBorder.Child = exerciseGrid;

            // Add Border to the specified StackPanel
            targetStackPanel.Children.Add(exerciseBorder);
        }

        private void IntializePrompt(Prompt MyPrompt)
        {
            MyPrompt.Age = TestUser.Age.ToString();
            MyPrompt.Height = CBSelectHeight.Text;
            MyPrompt.Weight = CBSelectWeight.Text;
            MyPrompt.ExerciseTime = CBSelectEXCTime.Text;
            MyPrompt.ExerciseDayes = CBSelectTrainingDaysOfWeek.Text;
            MyPrompt.Favoriteexercises = TBTypeFavoriteEXC.Text;
            MyPrompt.ExercisesGoal = CBSelectExercisesGoals.Text;

            BuildPrompt(MyPrompt);
        }
        //Also There is a Problem Here With the Ai Api !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        private void btnMakeThePlanPopupWindown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((CBSelectHeight.SelectedItem != null)
                && (CBSelectWeight.SelectedItem != null)
                && (CBSelectExercisesGoals.SelectedItem != null)
                && (CBSelectFitnessLevel.SelectedItem != null)
                && (CBSelectEXCTime.SelectedItem != null)
                && (CBSelectTrainingDaysOfWeek.SelectedItem != null)
                && (CBSelectEXCPlace.SelectedItem != null)
                && (CBSelectPlanType.SelectedItem != null)
                )
            {
                AiPopupScreen.IsOpen = false;
                _IsUserEnterPlanInfos = true;

                //
                AiExcPlanStackPanel.Children.Clear();
                //AddExerciseToStackPanel(AiExcPlanStackPanel, "Mountain Climbers", "12");
                //AddExerciseToStackPanel(AiExcPlanStackPanel, "Push-ups", "15");
                //AddExerciseToStackPanel(AiExcPlanStackPanel, "Squats", "20");
                Prompt MyPrompt = new Prompt();

                IntializePrompt(MyPrompt);
                PromptResult MyPromptResult = clsPlan.CreatePlan(MyPrompt);


                Dictionary<string, (int reps, int Time)>? AiExercises = MyPromptResult.Exercises;
                lblAmountOfWater.Content = MyPromptResult.AmountOfWater;
                lblCalorieIntake.Content = MyPromptResult.CalorieIntake;
                lblExerciseTime.Content = MyPromptResult.ExerciseTime;

                if (AiExercises != null) // Ensure it's not null
                {
                    foreach (var exercise in AiExercises)
                    {
                        string exerciseName = exercise.Key;
                        int reps = exercise.Value.reps;
                        int time = exercise.Value.Time;
                        // Add the TextBlock to the StackPanel
                    }
                    AddExerciseToStackPanel(AiExcPlanStackPanel, "Mountain Climbers", "12");
                }
                else
                {
                    MessageBox.Show("The Ai Failed");
                }

            }
        }

        private void btnCloseUserPopUpScreen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UserPopupScreen.IsOpen = false;
            switch (ActiveWindow)
            {
                case 0:
                    NavigationButtonsactions(HomeBorder, e);
                    break;

                case 1:
                    NavigationButtonsactions(ExcBorder, e);
                    break;

                case 2:
                    NavigationButtonsactions(AiBorder, e);
                    break;

                default:
                    break;
            }
        }

        //There is Problem Here With the back end !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //Saves the New User Info to the data base
        private void btnSaveTheNewUserInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(TBUserName.Text)
                && !string.IsNullOrEmpty(TBUserAge.Text)
                && !string.IsNullOrEmpty(TBUserEmail.Text)
                )
            {

                UserPopupScreen.IsOpen = false;
                switch (ActiveWindow)
                {
                    case 0:
                        NavigationButtonsactions(HomeBorder, e);
                        break;

                    case 1:
                        NavigationButtonsactions(ExcBorder, e);
                        break;

                    case 2:
                        NavigationButtonsactions(AiBorder, e);
                        break;

                    default:
                        break;
                }

                TestUser.UserName = TBUserName.Text;
                TestUser.Age = Convert.ToInt32(TBUserAge.Text);
                if (RBmale.IsChecked == true)
                { TestUser.Gender = "Male"; }
                else { TestUser.Gender = "Female"; }

                TestUser.Mode = clsUser.enMode.Update;
                TestUser.Save();
            }
            else
            {
                MessageBox.Show("Enter all info. to save !", "Save Failed", MessageBoxButton.OK, MessageBoxImage.Error);

            }


        }

        private void btnCloseMeasuringCaloriesInFood_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PopUpMeasuringCaloriesInFood.IsOpen = false;
        }

        private void btnCalcCaloriesInFood_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PopUpMeasuringCaloriesInFood.IsOpen)
                PopUpMeasuringCaloriesInFood.IsOpen = false;

            else
            {
                PopUpMeasuringCaloriesInFood.IsOpen = true;
                PopUpMeasuringCaloriesInPlan.IsOpen = false;
            }
        }

        private void btnCloseMeasuringCaloriesInPlan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PopUpMeasuringCaloriesInPlan.IsOpen = false;

        }

        private void btnMeasuringCaloriesInPlan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PopUpMeasuringCaloriesInPlan.IsOpen)
                PopUpMeasuringCaloriesInPlan.IsOpen = false;
            else
            {
                PopUpMeasuringCaloriesInFood.IsOpen = false;
                PopUpMeasuringCaloriesInPlan.IsOpen = true;
            }
        }

        //AiChat button ( in side of searchbar ) and Notification
        private void btnSoon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PopUpMeasuringCaloriesInPlan.IsOpen)
                PopUpMeasuringCaloriesInPlan.IsOpen = false;
            else
            {
                PopUpMeasuringCaloriesInFood.IsOpen = false;
                PopUpMeasuringCaloriesInPlan.IsOpen = true;
            }
        }

        private void mainwindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (mainwindow.WindowState == WindowState.Normal && e.Key == Key.F11)
            {
                mainwindow.WindowState = WindowState.Maximized;
            }
            else if (mainwindow.WindowState == WindowState.Maximized && e.Key == Key.F11)
            {
                mainwindow.WindowState = WindowState.Normal;
            }
        }

        private void CopyStackPanelToAnother(StackPanel AiExcPlanStackPanel, StackPanel ExcPlanStackPanel)
        {
            // Clear the destination StackPanel first
            ExcPlanStackPanel.Children.Clear();

            // Loop through all elements inside AiExcPlanStackPanel
            foreach (UIElement element in AiExcPlanStackPanel.Children)
            {
                if (element is Border originalBorder)
                {
                    // Create a new Border
                    Border newBorder = new Border
                    {
                        CornerRadius = originalBorder.CornerRadius,
                        Margin = originalBorder.Margin,
                        Width = originalBorder.Width,
                        Height = originalBorder.Height,
                        Background = originalBorder.Background,
                        HorizontalAlignment = originalBorder.HorizontalAlignment,
                        VerticalAlignment = originalBorder.VerticalAlignment,
                        Cursor = originalBorder.Cursor
                    };

                    // Check if the Border contains a Grid
                    if (originalBorder.Child is Grid originalGrid)
                    {
                        Grid newGrid = new Grid
                        {
                            Margin = originalGrid.Margin,
                            Width = originalGrid.Width,
                            Height = originalGrid.Height,
                            VerticalAlignment = originalGrid.VerticalAlignment
                        };

                        // Copy ColumnDefinitions
                        foreach (var colDef in originalGrid.ColumnDefinitions)
                        {
                            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        }

                        // Copy Child Elements
                        foreach (UIElement child in originalGrid.Children)
                        {
                            if (child is Image originalImage)
                            {
                                Image newImage = new Image
                                {
                                    Source = originalImage.Source,
                                    HorizontalAlignment = originalImage.HorizontalAlignment,
                                    Width = originalImage.Width
                                };
                                newGrid.Children.Add(newImage);
                            }
                            else if (child is TextBlock originalTextBlock)
                            {
                                TextBlock newTextBlock = new TextBlock
                                {
                                    FontSize = originalTextBlock.FontSize,
                                    Text = originalTextBlock.Text,
                                    Margin = originalTextBlock.Margin,
                                    HorizontalAlignment = originalTextBlock.HorizontalAlignment,
                                    Width = originalTextBlock.Width
                                };
                                newGrid.Children.Add(newTextBlock);
                            }
                            else if (child is Label originalLabel)
                            {
                                Label newLabel = new Label
                                {
                                    FontSize = originalLabel.FontSize,
                                    Content = originalLabel.Content,
                                    Margin = originalLabel.Margin,
                                    IsEnabled = originalLabel.IsEnabled,
                                    HorizontalAlignment = originalLabel.HorizontalAlignment,
                                    Width = originalLabel.Width
                                };
                                newGrid.Children.Add(newLabel);
                            }
                        }

                        newBorder.Child = newGrid;
                    }

                    // Add the cloned Border to the target StackPanel
                    ExcPlanStackPanel.Children.Add(newBorder);
                }
            }

        }

        private void btnAddNewPlan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_IsUserEnterPlanInfos)
            {
                WindowsContainer.SelectedIndex = 1;

                CopyStackPanelToAnother(AiExcPlanStackPanel, ExcPlanStackPanel);

            }

        }

        private void btnSuggestNewPlan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_IsUserEnterPlanInfos)
            {
                _IsUserEnterPlanInfos = false;
                AiPopupScreen.IsOpen = true;
                //CBSelectAge.SelectedItem = null;
                //CBSelectHeigt.SelectedItem = null;
                //CBSelectWeight.SelectedItem = null;
                CBSelectExercisesGoals.SelectedItem = null;
                CBSelectFitnessLevel.SelectedItem = null;
                CBSelectEXCTime.SelectedItem = null;
                CBSelectTrainingDaysOfWeek.SelectedItem = null;
                CBSelectEXCPlace.SelectedItem = null;
                CBSelectPlanType.SelectedItem = null;
                TBTypeFavoriteEXC.Text = "اختياري";
            }

        }

    }


}
