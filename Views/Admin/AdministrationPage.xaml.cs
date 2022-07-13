using VoterX.Utilities.Models;
using VoterX.Utilities.Views;
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

namespace VoterX.Kiosk.Views.Admin
{
    /// <summary>
    /// Interaction logic for AdministrationPage.xaml
    /// </summary>
    public partial class AdministrationPage : Page
    {
        public AdministrationPage()
        {
            InitializeComponent();

            StatusBar.PageHeader = "SYSTEM MANAGEMENT";

            //StatusBar.CheckPrinter(AppSettings.Printers);
            //StatusBar.CheckSignaturePad();

            StatusBar.Clear();

            LoadMenu();

            // Set Origin
            NavigationMenuMethods.SetOrigin(this);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Check for updates in system settings
            AppSettings.CheckUpdates(true);

            StatusBar.PageHeader = "SYSTEM MANAGEMENT";

            StatusBar.Clear();

            //LoadMenu();

            MainMenuMethods.LoadMenu(new DynamicMenuView(new Menu.ManageMenuViewModel()), StateVoterX.Utilities.Models.MenuCollapseMode.ShowIcons);

            MainMenuMethods.OpenMenu();

            MainMenuMethods.ClickEnabled = true;            
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadMenu()
        {
            if(MainMenuMethods.GetMode() == MenuCollapseMode.None)
            {
                MainMenuMethods.LoadMenu(new DynamicMenuView(new Menu.ManageMenuViewModel()), StateVoterX.Utilities.Models.MenuCollapseMode.ShowIcons);

                MainMenuMethods.OpenMenu();
            }
            MainMenuMethods.ClickEnabled = true;
        }
    }
}
