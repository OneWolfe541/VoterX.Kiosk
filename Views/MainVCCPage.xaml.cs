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
using VoterX.Utilities.BasePageDefinitions;
using VoterX.Utilities.Extensions;
using VoterX.Kiosk.Methods;
using System.Collections;
using VoterX.Utilities.Methods;
using System.Diagnostics;
using VoterX.Utilities.Views;
using System.Configuration;
using VoterX.Utilities.Models;
using System.ComponentModel;
using VoterX.SystemSettings.Enums;

namespace VoterX.Kiosk.Views
{
    /// <summary>
    /// Interaction logic for MainVCCPage.xaml
    /// </summary>
    public partial class MainVCCPage : MasterBasePage
    {
        public override Frame MainFrame
        {
            get { return MainFrameControl; }
            set { MainFrameControl = value; }
        }

        public override string PageHeaderName
        {
            get
            {
                return ((App)Application.Current).MainHeader.PageHeader;
                //return PageHeaderText.Text;
            }
            set
            {
                ((App)Application.Current).MainHeader.PageHeader = value;
                //PageHeaderText.Text = value;
            }
        }

        public override System.Windows.Visibility CloseButtonVisibility
        {
            get
            {
                if (((App)Application.Current).MainHeader.CloseButtonVisibility == true)
                {
                    return System.Windows.Visibility.Visible;
                }
                else if (((App)Application.Current).MainHeader.CloseButtonVisibility == false)
                {
                    return System.Windows.Visibility.Collapsed;
                }
                else
                {
                    return System.Windows.Visibility.Collapsed;
                }
            }
            set
            {
                if(value == Visibility.Visible)
                {
                    ((App)Application.Current).MainHeader.CloseButtonVisibility = true;
                }
                else if(value == Visibility.Collapsed)
                {
                    ((App)Application.Current).MainHeader.CloseButtonVisibility = false;
                }
            }
        }

        public override System.Windows.Visibility MenuBarsVisibility
        {
            get;
            set;
        }

        public override System.Windows.Visibility HamburgerButtonVisibility
        {
            get
            {
                if (((App)Application.Current).MainHeader.HamburgerMenuVisibility == true)
                {
                    return System.Windows.Visibility.Visible;
                }
                else if (((App)Application.Current).MainHeader.HamburgerMenuVisibility == false)
                {
                    return System.Windows.Visibility.Collapsed;
                }
                else
                {
                    return System.Windows.Visibility.Collapsed;
                }
            }
            set
            {
                if (value == Visibility.Visible)
                {
                    ((App)Application.Current).MainHeader.HamburgerMenuVisibility = true;
                }
                else if (value == Visibility.Collapsed)
                {
                    ((App)Application.Current).MainHeader.HamburgerMenuVisibility = false;
                }
            }
        }

        public override string StatusRight
        {
            get { return ApplicationStatusRight.Text; }
            set { ApplicationStatusRight.Text = value; }
        }

        public MainVCCPage()
        {
            InitializeComponent();

            //StatusBarUserControl.StatusBarLeft = "VCC Type: " +
            //    ((App)Application.Current).GlobalSettings.System.VCCType.ToString() +
            //    " Version: " +
            //    ((App)Application.Current).GlobalSettings.System.BODVersion.ToString();

            //StatusBarUserControl.StatusBarCenter = ((App)Application.Current).GlobalSettings.Network.SQLServer.ToString();

            LoadHeader(Window.GetWindow(this));

            //fa_bars.Visibility = Visibility.Collapsed;
            CloseButtonVisibility = Visibility.Collapsed;
            HamburgerButtonVisibility = Visibility.Visible;

            //fa_bars.Visibility = Visibility.Collapsed;
            //HamburgerButton.Visibility = Visibility.Visible;
            //CloseButton.Visibility = Visibility.Collapsed;

            DynamicMenuSlider.Initialize(MenuCollapseMode.None);

            //DynamicMenuSlider.SetMenuSource(new System.Uri("AboutPage.xaml", UriKind.RelativeOrAbsolute));
            //DynamicMenuSlider.MenuFrame.Navigate(new Menu.AboutMenu());

            ((App)Application.Current).mainpage = this;
            //((App)Application.Current).mainstatusbar = StatusBarUserControl;
            ((App)Application.Current).mainslidermenu = DynamicMenuSlider;

            LoadStatusBar();

            //StatusBar.DisplayMode(AppSettings.System);

            //this.NavigateToPage(new Voter.SearchPage());
            //MainFrame.Navigate(new Voter.SearchPage());
            //MainFrame.Navigate(new Settings.SettingsPage());

            // LOAD LOGIN PAGE WITH FULL STARTUP TEST
            MainFrameControl.Navigate(new Login.LoginPage(true));
            MainFrameControl.Navigating += OnNavigating;


            //AdminMenuSlider.HideTopContent();

            // SAMPLE BALLOT MODE or DEVELOPMENT MODE
            //CheckMode(); // IS ALREADY BEING CALLED ON THE LOGIN PAGE
        }

        // Disable the F5 (refresh) hot key
        private void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            ((Frame)sender).NavigationService.RemoveBackEntry();

            if (e.NavigationMode == NavigationMode.Refresh)
                e.Cancel = true;
        }

        private void MasterBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.Global.Dispose();

            GC.Collect();

            var myWindow = Window.GetWindow(this);
            myWindow.Close();
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Clear previous status bar messages
            //StatusBarUserControl.Clear();

            GC.Collect();
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            DynamicMenuSlider.Toggle();
        }

        public override void RemoveHamburger()
        {
            ((App)Application.Current).MainHeader.RemoveHamburgerMenu();
        }

        private void SystemSettings_Click(object sender, RoutedEventArgs e)
        {
            MainMenuMethods.CloseMenu();
            MainFrame.Navigate(new Settings.SettingsPage());
        }

        private void ReportsMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenuMethods.CloseMenu();
            MainFrame.Navigate(new Admin.ReportsMenuPage());
        }

        private void AddProvisional_Click(object sender, RoutedEventArgs e)
        {
            MainMenuMethods.CloseMenu();
            MainFrame.Navigate(new Admin.AddProvisionalPage());
        }

        private void ProvisionalSearch_Click(object sender, RoutedEventArgs e)
        {
            MainMenuMethods.CloseMenu();
            MainFrame.Navigate(new Admin.ProvisionalSearchPage());
        }

        private void ChangeBallotStyle_Click(object sender, RoutedEventArgs e)
        {
            MainMenuMethods.CloseMenu();
            MainFrame.Navigate(new Admin.EditBallotSearchPage());
        }

        private void MenuLogOut_Click(object sender, RoutedEventArgs e)
        {
            //PollSummaryMethods.SaveLogout();
            SaveLogout();
            MainMenuMethods.CloseMenu();
            MainFrame.Navigate(new Login.LoginPage());
        }

        public override async void SaveLogout()
        {
            if (await Task.Run(() => ElectionDataMethods.Exists) == true)
            {
                StatusBar.TextLeft = "Logging Out";
                StatusBar.ShowLeftSpinner();
                if (await Task.Run(() => PollSummaryMethods.SaveLogout()) == false) StatusBar.TextCenter = "Logout not saved";
                StatusBar.TextLeft = "";
                //StatusBar.HideLeftSpinner();
            }
        }

        private void MenuTest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClosePolls_Click(object sender, RoutedEventArgs e)
        {
            // OPEN END OF DAY PAGE
            MainMenuMethods.CloseMenu();
            MainFrame.Navigate(new Admin.EndofDayPage());

            //// Print todays ballots
            //StatusBar.StatusTextCenter = ReportPrintingMethods.PrintDailyDetailBSReport(AppSettings.System.SiteID, DateTime.Now, AppSettings.Global);
            //// Print todays spoiled
            //StatusBar.StatusTextCenter = ReportPrintingMethods.PrintDailySpoiledPrimaryReport(AppSettings.Election.ElectionID, AppSettings.System.SiteID, DateTime.Now, AppSettings.Global);
            //// Print todays provisional
            //StatusBar.StatusTextCenter = ReportPrintingMethods.PrintDailyProvisionalReport(AppSettings.Election.ElectionID, AppSettings.System.SiteID, DateTime.Now, AppSettings.Global);

            //// Activate Reconsile Application
            //// https://stackoverflow.com/questions/1585354/get-return-value-from-process

            //Process P = Process.Start("C:\\VoterX\\VoterXReconcile.exe");
            //P.WaitForExit();
            //int result = P.ExitCode;

            //StatusBar.StatusTextLeft = result.ToString();

            //// Print End of Day reports
            //StatusBar.StatusTextCenter = ReportPrintingMethods.PrintEndOfDayReport(DateTime.Now, AppSettings.Global);
        }

        public override void SetElectionTitle()
        {
            //ElectionEntity.Text = ((App)Application.Current).GlobalSettings.Election.ElectionEntity;
            //ElectionName.Text = ((App)Application.Current).GlobalSettings.Election.ElectionTitle;
            //PollLocationName.Text = ((App)Application.Current).GlobalSettings.System.SiteName;
        }

        public override void CheckMode()
        {
            //StatusBarUserControl.StatusBarLeft = "VCC Type: " + 
            //    ((App)Application.Current).GlobalSettings.System.VCCType.ToString() + 
            //    " Version: " +
            //    ((App)Application.Current).GlobalSettings.System.BODVersion.ToString();

            //StatusBarUserControl.StatusBarCenter = ((App)Application.Current).GlobalSettings.Network.SQLServer.ToString();

            // Sample Ballot Mode
            if (((App)Application.Current).GlobalSettings.System.VCCType == VotingCenterMode.SampleBallots)
            {
                //SamplePageHeaderText.Visibility = Visibility.Visible;
                //SamplePageHeaderText.Text = "SAMPLE BALLOT MODE";
                //PageHeaderText.Foreground = new SolidColorBrush(Colors.Yellow);
                ((App)Application.Current).MainHeader.SampleMode = true;
            }
            else
            {
                //SamplePageHeaderText.Text = " ";
                //SamplePageHeaderText.Visibility = Visibility.Hidden;
                //PageHeaderText.Foreground = new SolidColorBrush(Colors.White);
                ((App)Application.Current).MainHeader.SampleMode = false;
            }

            // Devlopment Mode
            if(((App)Application.Current).debugMode == true)
            {
                //DevelopmentPageHeaderText.Visibility = Visibility.Visible;
            }

            // AUTO SWAP FROM EARLY VOTING TO ELECTION DAY MODE
            // Check Election Date - Added 08/15/2018
            if (((App)Application.Current).GlobalSettings.System.VCCType == VotingCenterMode.EarlyVoting)
            {
                if (((App)Application.Current).GlobalSettings.Election.ElectionDate.AddDays(-1) < DateTime.Now)
                {
                    // Set VCCType to Election Day (2)
                    ((App)Application.Current).GlobalSettings.System.VCCType = VotingCenterMode.ElectionDay;                    

                    // Check if election day location (conversion ID) is set
                    if (((App)Application.Current).GlobalSettings.Site.ElectionDaySiteID != null)
                    {
                        // Put new location into main location field
                        ((App)Application.Current).GlobalSettings.System.SiteID = (int)((App)Application.Current).GlobalSettings.Site.ElectionDaySiteID;
                        ((App)Application.Current).GlobalSettings.System.SiteName = ((App)Application.Current).GlobalSettings.Site.ElectionDaySiteName;
                    }

                    // Change Site Verification to false
                    ((App)Application.Current).GlobalSettings.System.SiteVerified = false;

                    // Save Changes to file or database
                    ((App)Application.Current).GlobalSettings.SaveSettings();
                }
            }

            // Set Site ID
            //if (((App)Application.Current).GlobalSettings.System.VCCType == 1)
            //{
            //    // Set Side ID to Early Voting location
            //    if (((App)Application.Current).GlobalSettings.Site.EarlyVotingSiteID != null)
            //    {
            //        ((App)Application.Current).GlobalSettings.System.SiteID = (int)((App)Application.Current).GlobalSettings.Site.EarlyVotingSiteID;
            //        ((App)Application.Current).GlobalSettings.System.SiteName = ((App)Application.Current).GlobalSettings.Site.EarlVotingSiteName;
            //    }
            //}
            //else
            //{
            //    // Check if election day location (conversion ID) is set
            //    if (((App)Application.Current).GlobalSettings.Site.ElectionDaySiteID != null)
            //    {
            //        ((App)Application.Current).GlobalSettings.System.SiteID = (int)((App)Application.Current).GlobalSettings.Site.ElectionDaySiteID;
            //        ((App)Application.Current).GlobalSettings.System.SiteName = ((App)Application.Current).GlobalSettings.Site.EarlVotingSiteName;
            //    }
            //}
        }

        private void MasterBasePage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
               e.Handled = true;
            }
        }

        private void StatusBarUserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void LoadStatusBar() 
        {
            // Get connection string
            //string connection = "Server=AESSQL2;Database=StateDBRevision;Trusted_Connection=True;";
            string connection = ConfigurationManager.ConnectionStrings["VoterDatabase"].ToString();

            // Initialize Status Bar View Model
            ((App)Application.Current).StatusBar = new StatusBarViewModel(connection);

            // Bind View Model to Control
            StatusBarUserControl.DataContext = ((App)Application.Current).StatusBar;

            //((App)Application.Current).StatusBar.TextLeft = "This is the status bar.";            
            ((App)Application.Current).StatusBar.DisplaySystemMode(AppSettings.System.VCCType);
        }

        private void LoadHeader(Window parent)
        {
            //var parent = Window.GetWindow(this);
            ((App)Application.Current).MainHeader = new MainHeaderViewModel(parent);
            ((App)Application.Current).MainHeader.PropertyChanged += OnHeaderPropertyChanged;
            MainHeaderControl.DataContext = ((App)Application.Current).MainHeader;
        }

        private void OnHeaderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MenuClicked")
            {
                DynamicMenuSlider.Toggle();
            }
        }

        private void MainHeaderControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
