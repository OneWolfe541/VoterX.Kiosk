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
using VoterX.Kiosk.Methods;
using VoterX.Utilities.Extensions;
using VoterX.Utilities.Dialogs;
using VoterX.Utilities.Views.Settings;
using VoterX.Utilities.Views;

namespace VoterX.Kiosk.Views.Settings
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();

            // Check for updates in system settings
            AppSettings.CheckUpdates(true);

            InitializeSettingsTabControl();            

            StatusBar.PageHeader = "System Settings";

            StatusBar.Clear();

            // MENU LOADING MOVED TO LOGIN SCREEN
            //MainMenuMethods.LoadMenu(new Menu.SystemSettingsMenu(this), StateVoterX.Utilities.Models.MenuCollapseMode.ShowIcons);
            //var menu = new DynamicMenuView(new Menu.SettingsMenuViewModel(this));
            //MainMenuMethods.LoadMenu(menu, StateVoterX.Utilities.Models.MenuCollapseMode.ShowIcons);
            //MainMenuMethods.SetMode(StateVoterX.Utilities.Models.MenuCollapseMode.ShowIcons);
            //MainMenuMethods.OpenMenu();

            SettingsTabControl.Status.SettingsChanged = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StatusBar.TextLeft = AppSettings.System.BODName + ":" + AppSettings.System.BODVersion;

            MainMenuMethods.LoadMenu(new DynamicMenuView(new Menu.SettingsMenuViewModel(this)), StateVoterX.Utilities.Models.MenuCollapseMode.ShowIcons);
        }

        private void InitializeSettingsTabControl()
        {
            // Pass references to the database and settings
            SettingsTabControl.Election = ElectionDataMethods.Election;
            SettingsTabControl.Settings = AppSettings.Global;
            // Enable Tabs to display
            SettingsTabControl.SystemSettingsTab = Visibility.Collapsed;
            SettingsTabControl.NetworkSettingsTab = Visibility.Collapsed;
            SettingsTabControl.PrinterSettingsTab = Visibility.Collapsed;
            SettingsTabControl.ElectionSettingsTab = Visibility.Collapsed;
            SettingsTabControl.BallotSettingsTab = Visibility.Collapsed;
            SettingsTabControl.TabulatorSettingsTab = Visibility.Collapsed;
            SettingsTabControl.UserSettingsTab = Visibility.Collapsed;
            SettingsTabControl.ServersSettingsTab = Visibility.Collapsed;
            // Display the left most tab
            SettingsTabControl.LoadFirstPage();
        }

        public void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if any changes were made
            if (SettingsTabControl.Status.SettingsChanged == true)
            {
                // Ask user to save their changes
                YesNoDialog sampleDialog = new YesNoDialog("Save Changes", "The system settings have been changed!\r\nDo you want to save your changes before exiting?");
                if (sampleDialog.ShowDialog() == true)
                    SaveSettings_Click(sender, e);

                // Reset Changed Status
                SettingsTabControl.Status.SettingsChanged = false;

                // Reset menu mode
                MainMenuMethods.LoadMenu(null, StateVoterX.Utilities.Models.MenuCollapseMode.None);
                MainMenuMethods.CloseMenu();

                // Return to login page
                this.NavigateToPage(new Login.LoginPage(true));
            }
            else
            {
                // Reset menu mode
                MainMenuMethods.LoadMenu(null, StateVoterX.Utilities.Models.MenuCollapseMode.None);
                MainMenuMethods.CloseMenu();

                // Return to log in page
                this.NavigateToPage(new Login.LoginPage());
            }                
        }

        public void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // Reset Changed Status
            SettingsTabControl.Status.SettingsChanged = false;

            SettingsTabControl.Savechanges();

            AlertDialog sampleDialog = new AlertDialog("Settings Saved!");
            if (sampleDialog.ShowDialog() == true)
                StatusBar.TextLeft = "Settings Saved";
        }

        public void LoadSystemPage()
        {
            SettingsHeader.Text = "SYSTEM";

            SettingsTabControl.NavigateToSystemTab();
        }

        public void LoadNetworkPage()
        {
            SettingsHeader.Text = "NETWORK";

            SettingsTabControl.NavigateToNetworkTab();
        }

        public void LoadPrintersPage()
        {
            SettingsHeader.Text = "PRINTERS";

            SettingsTabControl.NavigateToPrintersTab();
        }

        public void LoadElectionPage()
        {
            SettingsHeader.Text = "ELECTION";

            SettingsTabControl.NavigateToElectionTab();
        }

        public void LoadBallotsPage()
        {
            SettingsHeader.Text = "BALLOTS";

            SettingsTabControl.NavigateToBallotsTab();
        }

        public void LoadTabulatorsPage()
        {
            SettingsHeader.Text = "TABULATORS";

            SettingsTabControl.NavigateToTabulatorsTab();
        }
    }
}
