using VoterX.Kiosk.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VoterX.Kiosk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Maximize the window to fit with Task Bar
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

            MainWindowFrame.Navigate(new System.Uri("Views/MainVCCPage.xaml", UriKind.RelativeOrAbsolute));
            MainWindowFrame.Navigating += OnNavigating;

            // Maximize the application window if not in Debug Mode (AKA Running on my development computer)
            if (((App)Application.Current).debugMode == false)
            {
                Maximize();
            }

            //StartTimer();
        }

        // Disable the F5 (refresh) hot key
        private void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            ((Frame)sender).NavigationService.RemoveBackEntry();

            if (e.NavigationMode == NavigationMode.Refresh)
                e.Cancel = true;
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Remove previous pages from the main window frame's navigation history
            MainWindowFrame.NavigationService.RemoveBackEntry();
            GC.Collect();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Drag the main window when left mouse button is pressed in the title bar area
            if (e.ChangedButton == MouseButton.Left && e.GetPosition(this).Y < 75)
            {
                DragMove();
            }
        }

        private void MainWindowGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.MouseDown += delegate { DragMove(); };
        }

        // Maximize the window when mouse double clicks in the title bar area
        private void MaximizeMyWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && e.GetPosition(this).Y < 75)
            {
                WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            }
        }

        private void Maximize()
        {
            WindowState = WindowState.Maximized;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.F5)
            {
                e.Handled = true;
            }

            //StatusBar.StatusTextLeft = e.Key.ToString();
            //TimerMethods.RestartTimer();
            //StatusBar.StatusTextLeft = await TimerMethods.GetElapsedTime();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //var point = Mouse.GetPosition(this);
            //StatusBar.StatusTextLeft = "Mouse: X" + point.X.ToString() + " Y" + point.Y.ToString();
            //TimerMethods.RestartTimer();
            //StatusBar.StatusTextLeft = await TimerMethods.GetElapsedTime();

            // For Debugging the log out timer
            //DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            //dispatcherTimer.Tick += UpdateWatch;
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            //dispatcherTimer.Start();
        }

        //public async void UpdateWatch(object sender, EventArgs e)
        //{
        //    StatusBar.TextLeft = await TimerMethods.GetElapsedTime();
        //}

        //public void StartTimer()
        //{
        //    var timer = new DispatcherTimer
        //        (
        //        TimeSpan.FromSeconds(10),
        //        DispatcherPriority.ApplicationIdle,// Or DispatcherPriority.SystemIdle
        //        (s, e) => { this.Close(); }, // or something similar
        //        Application.Current.Dispatcher
        //        );
        //}
    }
}
