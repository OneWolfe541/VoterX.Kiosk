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
using VoterX.Utilities.Methods;
using VoterX.Kiosk.Methods;
using VoterX.Utilities.Extensions;
using VoterX.Utilities.Dialogs;
//using VoterX.Core.Models.Database;
using VoterX.SystemSettings.Extensions;
//using VoterX.Core.Repositories;
using VoterX.Logging;
using VoterX.Core.Elections;
using System.Windows.Threading;
using VoterX.Kiosk.Views.Settings;
using VoterX.Kiosk.Views.Menu;
using VoterX.Utilities.Views;
using VoterX.SystemSettings.Enums;

namespace VoterX.Kiosk.Views.Login
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private bool pwCorrect = false;

        VoterXLogger settingsLog;

        public LoginPage()
        {
            InitializeComponent();

            settingsLog = new VoterXLogger("VCClogs", true);

            ShowExitButton();

            StatusBar.PageHeader = "Log on";

            StatusBar.Clear();

            // Hide Menu
            MainMenuMethods.LoadMenu(null, StateVoterX.Utilities.Models.MenuCollapseMode.None);
            MainMenuMethods.CloseMenu();

            //StatusBar.StatusTextRight = "Copyright © AUTOMATED ELECTION SERVICES 2018";

            //AppSettings.SetElectionMode();

            ((App)Application.Current).mainpage.SetElectionTitle();

            // CHECK FOR SAMPLE MODE WHEN EVER THE USER RETURNS TO THE LOGIN PAGE
            // AND CHECK FOR ELECTION DAY TURN OVER -- ADDED (8/15/2018)
            ((App)Application.Current).mainpage.CheckMode();

            StatusBar.DisplayMode(AppSettings.System);

            CheckServerLight();

            //LoadElectionDetails();

            //LoadPollSiteDetails();

            Keyboard.Focus(Password);

            // Stop timer on log out
            TimerMethods.KillTimer();
        }

        public LoginPage(bool fullCheck)
        {
            InitializeComponent();

            settingsLog = new VoterXLogger("VCClogs", true);

            ShowExitButton();

            StatusBar.PageHeader = "Log on";

            StatusBar.Clear();

            //StatusBar.StatusTextRight = "Copyright © AUTOMATED ELECTION SERVICES 2018";

            //AppSettings.SetElectionMode();

            ((App)Application.Current).mainpage.SetElectionTitle();

            // CHECK FOR SAMPLE MODE
            ((App)Application.Current).mainpage.CheckMode();

            StatusBar.DisplayMode(AppSettings.System);

            if (fullCheck == true)
            {
                CheckServer();
            }
            else
            {
                CheckServerLight();
            }

            //LoadElectionDetails();

            //LoadPollSiteDetails();

            Keyboard.Focus(Password);

            // Stop timer on log out
            TimerMethods.KillTimer();
        }

        private async void CheckServer()
        {
            //bool done = false;

            StatusBar.PageHeader = "Loading";

            LoginFields.Visibility = Visibility.Collapsed;
            LoadingMessage.Visibility = Visibility.Visible;

            await PutTaskDelay(400);

            StatusBar.TextLeft = "Loading System Settings";

            await PutTaskDelay(400);

            StatusBar.TextLeft = "Initializing Peripherals";

            await PutTaskDelay(800);

            //done = await StatusBar.CheckPrinter(AppSettings.Printers);
            //done = await Task.Run(() => StatusBar.CheckSignaturePad());

            PDFToolsMethods.SetLicenseKey(AppSettings.System.PDFTools);

            StatusBar.TextLeft = "Scanning Printer Drivers";

            LoadPrinterDrivers();

            //AppSettings.Global.Printers;

            await PutTaskDelay(1000);

            StatusBar.TextLeft = "Validating Server Connection";

            await PutTaskDelay(1500);

            try
            {
                if (await StatusBar.CheckServer(new ElectionFactory()) == false)
                {
                    Console.WriteLine("Login Error Alert Called");
                    AlertDialog messageBox = new AlertDialog("A DATABASE CONNECTION ERROR WAS ENCOUNTERED\r\nPLEASE CONTACT TECHNICAL SUPPORT FOR ASSISTANCE");
                    messageBox.ShowDialog();
                }
                else
                {
                    StatusBar.TextLeft = "Loading Election";

                    // Load Site and Election details
                    await LoadElectionDetails();

                    LoadPollSiteDetails();

                    // Save Changes to file
                    //((App)Application.Current).GlobalSettings.SaveSettings();
                    //AppSettings.SaveChanges();
                }
            }
            catch (Exception error)
            {
                settingsLog.WriteLog("Database Check Failed: " + error.Message);
                if (error.InnerException != null)
                {
                    settingsLog.WriteLog(error.InnerException.Message);
                }
            }

            StatusBar.TextLeft = "";
            LoginFields.Visibility = Visibility.Visible;
            LoadingMessage.Visibility = Visibility.Collapsed;

            StatusBar.PageHeader = "Log on";

            Keyboard.Focus(Password);
        }

        private async void CheckServerLight()
        {
            //bool done = false;

            StatusBar.PageHeader = "Loading";

            LoginFields.Visibility = Visibility.Collapsed;
            LoadingMessage.Visibility = Visibility.Visible;

            await PutTaskDelay(400);

            //done = await StatusBar.CheckPrinter(AppSettings.Printers);
            //done = await StatusBar.CheckSignaturePad();

            PDFToolsMethods.SetLicenseKey(AppSettings.System.PDFTools);

            LoadPrinterDrivers();

            StatusBar.TextLeft = "Validating Server Connection";

            await PutTaskDelay(500);

            if (await StatusBar.CheckServer(new ElectionFactory()) == false)
            {
                Console.WriteLine("Login Error Alert Called");
                AlertDialog messageBox = new AlertDialog("A DATABASE CONNECTION ERROR WAS ENCOUNTERED\r\nPLEASE CONTACT TECHNICAL SUPPORT FOR ASSISTANCE");
                messageBox.ShowDialog();
            }
            else
            {
                StatusBar.TextLeft = "Loading Election";

                // Load Site and Election details
                //await LoadElectionDetails();

                //LoadPollSiteDetails();

                // Save Changes to file
                //((App)Application.Current).GlobalSettings.SaveSettings();
                //AppSettings.SaveChanges();
            }

            StatusBar.TextLeft = "";
            LoginFields.Visibility = Visibility.Visible;
            LoadingMessage.Visibility = Visibility.Collapsed;

            StatusBar.PageHeader = "Log on";

            Keyboard.Focus(Password);
        }

        private async Task<bool> LoadElectionDetails()
        {
            return await Task.Run(() =>
            {
                //((App)Application.Current).Election.Load(AppSettings.Election.ElectionID);
                using (ElectionFactory factory = new ElectionFactory())
                {
                    try
                    {
                        if (((App)Application.Current).Connection != null)
                        {
                            ((App)Application.Current).Election = factory.Create(AppSettings.Election.ElectionID, ((App)Application.Current).Connection);
                        }
                        else
                        {
                            ((App)Application.Current).Election = factory.Create(AppSettings.Election.ElectionID);
                        }
                        
                        if (((App)Application.Current).Election != null && ((App)Application.Current).Election.Error == null)
                        {
                            LogElectionData();

                            // Election object created with no errors
                            return true;
                        }
                        else
                        {
                            if (((App)Application.Current).Election.Error != null)
                            {
                                settingsLog.WriteLog("Load Election Failed: " + ((App)Application.Current).Election.Error.Message);
                                if(((App)Application.Current).Election.Error.InnerException != null)
                                {
                                    settingsLog.WriteLog(((App)Application.Current).Election.Error);
                                }
                            }
                            return false;
                        }
                    }
                    catch (Exception e)
                    {
                        settingsLog.WriteLog("Load Election Failed: " + e.Message);
                        if (e.InnerException != null)
                        {
                            settingsLog.WriteLog(e);
                        }
                        return false;
                    }
                }
            });
            //var electionDetails = await Task.Run(() =>
            //{
            //    try
            //    {
            //        return VoterMethods.Container.Resolve<ElectionRepo>().Query(0).Where(elec => elec.election_id == AppSettings.Election.ElectionID).FirstOrDefault();
            //    }
            //    catch (Exception e)
            //    {
            //        settingsLog.WriteLog("Loading Election Details: " + e.Message);
            //        return null;
            //    }
            //}
            //);
            //if (electionDetails == null)
            //{
            //    settingsLog.WriteLog("Election Not Found: " + AppSettings.Election.ElectionID.ToString() + " " + AppSettings.Election.ElectionTitle);
            //    StatusBar.StatusTextCenter = "An error was encountered in tblElection";
            //    AlertDialog messageBox = new AlertDialog("A DATABASE CONNECTION ERROR WAS ENCOUNTERED\r\nPLEASE CONTACT TECHNICAL SUPPORT FOR ASSISTANCE");
            //    messageBox.ShowDialog();                
            //}
            //else
            //{
            //    AppSettings.Election.CountyCode = electionDetails.county_code;
            //    AppSettings.Election.ElectionEntity = electionDetails.county_name;
            //}
        }

        private void LogElectionData()
        {
            settingsLog.WriteLog("LOADING ELECTION DATA");

            try
            {
                settingsLog.WriteLog("ApplicationRejectedReasons:" + ((App)Application.Current).Election.Lists.ApplicationRejectedReasons.Count().ToString());
            }
            catch
            {
                settingsLog.WriteLog("ApplicationRejectedReasons:null");
            }

            try
            { 
            settingsLog.WriteLog("BallotRejectedReasons:" + ((App)Application.Current).Election.Lists.BallotRejectedReasons.Count().ToString());
            }
            catch
            {
                settingsLog.WriteLog("BallotRejectedReasons:null");
            }

            try
            { 
            settingsLog.WriteLog("BallotStyles:" + ((App)Application.Current).Election.Lists.BallotStyles.Count().ToString());
            }
            catch
            {
                settingsLog.WriteLog("BallotStyles:null");
            }

            try
            { 
            settingsLog.WriteLog("Election:" + ((App)Application.Current).Election.Lists.Election.ToString());
            }
            catch
            {
                settingsLog.WriteLog("Election:null");
            }

            try
            { 
            settingsLog.WriteLog("Jurisdictions:" + ((App)Application.Current).Election.Lists.Jurisdictions.Count().ToString());
            }
            catch
            {
                settingsLog.WriteLog("Jurisdictions:null");
            }

            try
            { 
            settingsLog.WriteLog("JurisdictionTypes:" + ((App)Application.Current).Election.Lists.JurisdictionTypes.Count().ToString());
            }
            catch
            {
                settingsLog.WriteLog("JurisdictionTypes:null");
            }

            try
            { 
            settingsLog.WriteLog("Locations:" + ((App)Application.Current).Election.Lists.Locations.Count().ToString());
            }
            catch
            {
                settingsLog.WriteLog("Locations:null");
            }

            try
            { 
            settingsLog.WriteLog("LogCodes:" + ((App)Application.Current).Election.Lists.LogCodes.Count().ToString());
            }
            catch
            {
                settingsLog.WriteLog("LogCodes:null");
            }

            try
            { 
            settingsLog.WriteLog("Partys:" + ((App)Application.Current).Election.Lists.Partys.Count().ToString());
            }
            catch
            {
                settingsLog.WriteLog("Partys:null");
            }

            try
            { 
            settingsLog.WriteLog("PollWorkers:" + ((App)Application.Current).Election.Lists.PollWorkers.Count().ToString());
            }
            catch
            {
                settingsLog.WriteLog("PollWorkers:null");
            }

            try
            { 
            settingsLog.WriteLog("Precincts:" + ((App)Application.Current).Election.Lists.Precincts.Count().ToString());
            }
            catch
            {
                settingsLog.WriteLog("Precincts:null");
            }

            try
            { 
            settingsLog.WriteLog("ProvisionalReasons:" + ((App)Application.Current).Election.Lists.ProvisionalReasons.Count().ToString());
            }
            catch
            {
                settingsLog.WriteLog("ProvisionalReasons:null");
            }

            try
            { 
            settingsLog.WriteLog("SpoiledReasons:" + ((App)Application.Current).Election.Lists.SpoiledReasons.Count().ToString());
            }
            catch
            {
                settingsLog.WriteLog("SpoiledReasons:null");
            }

            try
            { 
            settingsLog.WriteLog("Tabulators:" + ((App)Application.Current).Election.Lists.Tabulators.Count().ToString());
            }
            catch
            {
                settingsLog.WriteLog("Tabulators:null");
            }

        }

        private async void LoadPollSiteDetails()
        {
            //var siteName = await Task.Run(()=>
            //{
            //    try
            //    {
            //        return VoterMethods.Container.Resolve<LocationRepo>().Query(0).Where(loc => loc.poll_id == AppSettings.System.SiteID).FirstOrDefault().place_name;
            //    }
            //    catch (Exception e)
            //    {
            //        settingsLog.WriteLog("Loading Location Details: " + e.Message);
            //        return null;
            //    }
            //});
            //if (siteName == null)
            //{
            //    settingsLog.WriteLog("Poll Site Not Found: " + AppSettings.System.SiteID.ToString() + " " + AppSettings.System.SiteName);
            //    //StatusBar.StatusTextCenter = "An error was encountered in tblLocations";
            //    //AlertDialog messageBox = new AlertDialog("A DATABASE CONNECTION ERROR WAS ENCOUNTERED\r\nPLEASE CONTACT TECHNICAL SUPPORT FOR ASSISTANCE");
            //    //messageBox.ShowDialog();
            //}
            //else
            //{
            //    AppSettings.System.SiteName = siteName;
            //}
        }

        // https://stackoverflow.com/questions/22158278/wait-some-seconds-without-blocking-ui-execution
        private async Task PutTaskDelay(int delay)
        {
            await Task.Delay(delay);
        }

        private async void LoadPrinterDrivers()
        {
            var done = await AppSettings.Global.LoadPrinterLists();
            if(done == true)
            {
                //StatusBar.StatusTextLeft = "Printers Loaded";
            }
        }

        //private async void btnTaskDelay_Click(object sender, EventArgs e)
        //{
        //    await PutTaskDelay();
        //    MessageBox.Show("I am back");
        //}

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Prevent spam clicking the login button
            if (pwCorrect == false)
            {
                Login();
            }
        }

        private void Login()
        {
            StatusBar.Clear();
            string userName = UserList.GetSelectedItem();

            // Check for updates in system settings
            //StatusBar.TextCenter = AppSettings.CheckCurrentUpdates;

            // Login Admin
            if (userName == "Administrator")
            {
                if (Password.Password.ToUpper() == AppSettings.User.AdminPassword.ToUpper())
                {
                    pwCorrect = true;

                    HideExitButton();
                    ShowHamburger();                    

                    // Save Login
                    SaveLogin(userName);

                    var settingsPage = new Views.Settings.SettingsPage();

                    ShowSettingsMenu(settingsPage);

                    // Before navigating make sure to load site & user settings
                    this.NavigateToPage(settingsPage);
                }
                else
                {
                    AlertDialog message = new AlertDialog("Incorrect Username or Password");
                    if (message.ShowDialog() == true)
                    {
                        Password.Password = "";
                        Keyboard.Focus(Password);
                    }
                }
            }

            // Login Manager
            else if (userName == "Presiding Judge")
            {
                if (Password.Password.ToUpper() == AppSettings.User.ManagePassword.ToUpper())
                {
                    pwCorrect = true;

                    HideExitButton();
                    ShowHamburger();
                    HideSettingsMenu();
                    ShowManagerMenu();

                    // Save Login
                    SaveLogin(userName);

                    //// Create Idle Timer
                    //TimerMethods.StartLogTimer(
                    //    AppSettings.System.TimeOut,
                    //    NavigationMethods.FindParent<Frame>(this));
                    
                    OpenAdminMenu();
                    this.NavigateToPage(new Views.Admin.AdministrationPage());
                }
                else
                {
                    AlertDialog message = new AlertDialog("Incorrect Username or Password");
                    if (message.ShowDialog() == true)
                    {
                        Password.Password = "";
                        Keyboard.Focus(Password);
                    }
                }
            }

            // Login basic user
            else if (userName == "VoterX User")
            {
                if (Password.Password.ToUpper() == AppSettings.User.GlobalPassword.ToUpper())
                {
                    Password.IsEnabled = false;
                    pwCorrect = true;
                    //HideExitButton();
                    //ShowHamburger();
                    //ShowAboutMenu();
                    //HideHamburger();
                    RemoveExitButton();
                    RemoveHamburger();
                    LoadGlobalUserData();

                    // Save Login
                    SaveLogin(userName);

                    //// Create Idle Timer 
                    //TimerMethods.StartLogTimer(
                    //    AppSettings.System.TimeOut,
                    //    NavigationMethods.FindParent<Frame>(this));

                    if (AppSettings.System.SiteVerified == true)
                    {
                        this.NavigateToPage(new Voter.Search.VoterSearchPage(null));
                    }
                    else
                    {
                        this.NavigateToPage(new Views.Admin.SiteVerificationPage());
                    }
                }
                else
                {
                    Password.IsEnabled = false;
                    AlertDialog message = new AlertDialog("Incorrect Username or Password");
                    if (message.ShowDialog() == true)
                    {
                        Password.IsEnabled = true;
                        Password.Password = "";
                        Keyboard.Focus(Password);
                    }
                }
            }

            // Login registered user from database
            else if (AppSettings.System.LoginType != 1)
            {
                //if (await Task.Run(() => VoterMethods.Exists) == true)
                //{
                //    var user = PollWorkerMethods.UserLogin(VoterMethods.Container, userName, Password.Password);
                //    if (user != null)
                //    {
                //        pwCorrect = true;

                //        //StatusBar.ApplicationStatus("User Logged In As: " + user.name_first + " " + user.name_last);
                //        HideExitButton();
                //        ShowHamburger();
                //        ShowAboutMenu();
                //        // Before navigating make sure to load site & user settings
                //        LoadUserData(user);

                //        // Save Login
                //        SaveLogin();

                //        if (AppSettings.System.SiteVerified == true)
                //        {
                //            this.NavigateToPage(new Views.Voter.SearchPage(null));
                //        }
                //        else
                //        {
                //            this.NavigateToPage(new Views.Admin.SiteVerificationPage());
                //        }
                //    }
                //    else
                //    {
                //        AlertDialog message = new AlertDialog("Incorrect Username or Password");
                //        if (message.ShowDialog() == true)
                //        {
                //            Password.Password = "";
                //            Keyboard.Focus(Password);
                //        }
                //    }
                //}
                //else
                //{
                //    StatusBar.StatusTextCenter = "Database not found";
                //}
            }
        }

        private async void SaveLogin(string user)
        {
            if (await Task.Run(() => ElectionDataMethods.Exists) == true)
            {
                StatusBar.TextLeft = "Logging In";
                StatusBar.ShowLeftSpinner();
                //if (PollSummaryMethods.SaveLoginSync(user) == false) StatusBar.TextCenter = "Login not saved";
                StatusBar.TextLeft = "";
                StatusBar.HideLeftSpinner();
            }
        }

        private void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = ComboBoxMethods.GetSelectedItem(sender);

            //Password.Focus = true;
            Keyboard.Focus(Password);
        }

        private void UserList_Loaded(object sender, RoutedEventArgs e)
        {
            // Create animated loading list item
            var loadingItem = ComboBoxMethods.AddLoadingItem(UserList, TempLoadingSpinnerItem);
            
            if (AppSettings.System.LoginType == 1) // Single User
            {
                AdminUser.Visibility = Visibility.Visible;
                //AdminUser.IsSelected = true;
                SetManagerLoginVisibility();
                GlobalUser.Visibility = Visibility.Visible;
                GlobalUser.IsSelected = true;
                //EndOfDay.Visibility = Visibility.Visible;
                //HelpDesk.Visibility = Visibility.Visible;
            }
            else if (AppSettings.System.LoginType == 2)// Multi User
            {
                //if (await Task.Run(() => VoterMethods.Exists) == true)
                //{
                //    var userList = await Task.Run(() => PollWorkerMethods.UserList(VoterMethods.Container, (int)AppSettings.System.SiteID));

                //    int count = 1;
                //    ComboBoxItem selectedItem = new ComboBoxItem();
                //    foreach (var user in userList)
                //    {
                //        ComboBoxItem item = new ComboBoxItem();
                //        item.Content = user;
                //        ((ComboBox)sender).Items.Add(item);
                //        if (count == 1) selectedItem = item;
                //        count++;
                //    }

                //    selectedItem.IsSelected = true;
                //}
                //else
                //{
                //    GlobalUser.Visibility = Visibility.Visible;
                //    GlobalUser.IsSelected = true;
                //    StatusBar.StatusTextCenter = "Database not found";
                //}

                    
                AdminUser.Visibility = Visibility.Visible;
                //AdminUser.IsSelected = true;
                SetManagerLoginVisibility();
                //EndOfDay.Visibility = Visibility.Visible;
                //HelpDesk.Visibility = Visibility.Visible;
            }
            

            // Remove animated loading list item
            ComboBoxMethods.RemoveListItem(UserList, loadingItem);
        }

        //// Load the current user into the global settings
        //private void LoadUserData(tblPollWorker user)
        //{
        //    AppSettings.User.UserID = user.voter_id;
        //    AppSettings.User.UserName = user.username;
        //    AppSettings.User.FullName = user.name_first + " " + user.name_last;
        //    AppSettings.User.Party = user.party;
        //    AppSettings.User.PositionName = user.postion_name;
        //    AppSettings.Network.LastLogin = DateTime.Now;
        //}

        // Load the global user into settings
        private void LoadGlobalUserData()
        {
            AppSettings.User.UserID = 0;
            AppSettings.User.UserName = "Global";
            AppSettings.User.FullName = "Global User";
            AppSettings.User.Party = "NON";
            AppSettings.User.PositionName = "General";
            //AppSettings.Network.LastLogin = DateTime.Now;
        }

        //private void SaveLogin()
        //{
        //    // Get Date
        //    avPollSummary pollsite = new avPollSummary
        //    {
        //        poll_id = AppSettings.System.SiteID,
        //        poll_type_id = AppSettings.System.InvertVCCType(),
        //        computer = AppSettings.System.MachineID,
        //        logged_In = DateTime.Now,
        //        logged_out = null
        //    };

        //    // Save date to database
        //    VoterMethods.Container.PollSummary().UpdateLoggedIn(pollsite);
                
        //}

        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (pwCorrect == false)
                {
                    Login();
                }
            }
        }

        // Swap exit button and hamburger bars
        private void ShowExitButton()
        {
            MainMenuMethods.CloseMenu();
            MainMenuMethods.ShowExitButton();
        }

        private void RemoveExitButton()
        {
            MainMenuMethods.RemoveExitButton();
        }

        private void HideExitButton()
        {
            MainMenuMethods.HideExitButton();
        }

        private void ShowHamburger()
        {
            MainMenuMethods.ShowHamburger();
        }

        private void HideHamburger()
        {
            MainMenuMethods.HideHamburger();
        }

        private void RemoveHamburger()
        {
            MainMenuMethods.RemoveHamburger();
        }

        private void ShowAboutMenu()
        {
            MainMenuMethods.LoadMenu(new Menu.AboutMenu(), StateVoterX.Utilities.Models.MenuCollapseMode.Full);
        }

        private void ShowManagerMenu()
        {
            MainMenuMethods.LoadMenu(new DynamicMenuView(new Menu.ManageMenuViewModel()), StateVoterX.Utilities.Models.MenuCollapseMode.ShowIcons);

            MainMenuMethods.OpenMenu();
        }

        private void ShowSettingsMenu(SettingsPage page)
        {
            MainMenuMethods.LoadMenu(new DynamicMenuView(new Menu.SettingsMenuViewModel(page)), StateVoterX.Utilities.Models.MenuCollapseMode.ShowIcons);
        }

        private void HideSettingsMenu()
        {
            MainMenuMethods.HideTopContent();
        }

        private void OpenAdminMenu()
        {
            MainMenuMethods.OpenMenu();
        }

        private void SetManagerLoginVisibility()
        {
            if (AppSettings.System.VCCType == VotingCenterMode.SampleBallots) 
            {
                ManageUser.Visibility = Visibility.Collapsed;
            }
            else
            {
                ManageUser.Visibility = Visibility.Visible;
            }
        }

        // For tracking the log out timer
        //private void TestTimer()
        //{
        //    ((App)Application.Current).TestTimer = new DispatcherTimer
        //        (
        //        TimeSpan.FromSeconds(1),
        //        DispatcherPriority.Background,
        //        (s, e) =>
        //        {
        //            DisplayLogOutTime();
        //        },
        //        Application.Current.Dispatcher
        //        );

        //}

        private void DisplayLogOutTime()
        {
            StatusBar.TextLeft = ((App)Application.Current).IdleTimer.GetElapsedTime();
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.F11)
            //{
            //    ValidationDialog passwordDialog = new ValidationDialog(AppSettings.User, "SuperUser");
            //    if (passwordDialog.ShowDialog() == true)
            //    {
            //        this.NavigateToPage(new Super.Search.SuperSearchPage(null));
            //    }
            //    else
            //    {
            //        // Display error message
            //        AlertDialog wrongPassword = new AlertDialog("THE WRONG PASSWORD WAS ENTERED");
            //        wrongPassword.ShowDialog();
            //    }

            //    e.Handled = true;
            //}
        }
    }
}
