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
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Menu
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class ManagerMenu : MenuBasePage
    {
        public ManagerMenu()
        {
            InitializeComponent();            
        }

        public override string GetMenu()
        {
            return "Manager Menu";
        }

        //private void SystemSettings_Click(object sender, RoutedEventArgs e)
        //{
        //    MainMenuMethods.CloseMenu();
        //    ((App)Application.Current).mainpage.MainFrame.Navigate(new Settings.SettingsPage());
        //}

        private void ReportsMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenuMethods.CloseMenu();
            //((App)Application.Current).mainpage.MainFrame.Navigate(new Admin.ReportsMenuPage());
            NavigationMenuMethods.DailyReportsPage();
        }

        private void AddProvisional_Click(object sender, RoutedEventArgs e)
        {
            MainMenuMethods.CloseMenu();
            //((App)Application.Current).mainpage.MainFrame.Navigate(new Admin.AddProvisionalPage());
            NavigationMenuMethods.AddProvisionalPage();
        }

        private void ProvisionalSearch_Click(object sender, RoutedEventArgs e)
        {
            MainMenuMethods.CloseMenu();
            //((App)Application.Current).mainpage.MainFrame.Navigate(new Admin.ProvisionalSearchPage());
            NavigationMenuMethods.SearchProvisionalPage();
        }

        private void ChangeBallotStyle_Click(object sender, RoutedEventArgs e)
        {
            MainMenuMethods.CloseMenu();
            //((App)Application.Current).mainpage.MainFrame.Navigate(new Admin.EditBallotSearchPage());
            NavigationMenuMethods.EditBallotStylePage();
        }

        private void ClosePolls_Click(object sender, RoutedEventArgs e)
        {
            MainMenuMethods.CloseMenu();
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Admin.EndofDayPage());
        }

        private void MenuTest_Click(object sender, RoutedEventArgs e)
        {
            MainMenuMethods.CloseMenu();
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Reconcile.ReconcileStartPage());
        }

        private void MenuLogOut_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).mainpage.SaveLogout();

            //MainMenuMethods.SetMode(StateVoterX.Utilities.Models.MenuCollapseMode.None);
            MainMenuMethods.LoadMenu(null, StateVoterX.Utilities.Models.MenuCollapseMode.None);
            MainMenuMethods.CloseMenu();

            ((App)Application.Current).mainpage.MainFrame.Navigate(new Login.LoginPage());
        }

        private void EmergencyBallotMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenuMethods.CloseMenu();
            //((App)Application.Current).mainpage.MainFrame.Navigate(new Admin.EmergencyBallotPage());
            NavigationMenuMethods.EmergencyBallotsPage();
        }
    }
}
