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
using VoterX.Utilities.Controls;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Menu
{
    /// <summary>
    /// Interaction logic for ManageMenuView.xaml
    /// </summary>
    public partial class ManageMenuView_old : Page
    {
        public ManageMenuView_old()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new ManageMenuViewModel();

            //// Create factory
            //MenuButtonFactory buttonFactory = new MenuButtonFactory();

            //// Add home button to page
            //HomeRegion.Children.Add(buttonFactory.CreateButton(
            //    FontAwesome.WPF.FontAwesomeIcon.Home,
            //    "MANAGE HOME",
            //    "MANAGE HOME",
            //    new Thickness(0, 0, 0, 0),
            //    param => NavigationMenuMethods.ManagePage()
            //));

            //// Add Log Out Button
            //ExitRegion.Children.Add(buttonFactory.CreateButton(
            //    FontAwesome.WPF.FontAwesomeIcon.SignOut,
            //    "LOG OUT",
            //    "LOG OUT",
            //    new Thickness(0, 0, 0, 5),
            //    param => NavigationMenuMethods.LoginPage()
            //));

            //// Add dynamic button list
            //MenuButtonList buttonList = new MenuButtonList();
            //buttonList.Add(new MenuButtonViewModel(
            //    FontAwesome.WPF.FontAwesomeIcon.UserPlus,
            //    "ADD PROVISIONAL VOTER",
            //    "ADD PROVISIONAL VOTER",
            //    new Thickness(0, 0, 0, 5),
            //    param => NavigationMenuMethods.AddProvisionalPage()
            //));
            //buttonList.Add(new MenuButtonViewModel(
            //    FontAwesome.WPF.FontAwesomeIcon.PencilSquareOutline,
            //    "EDIT BALLOT STYLE",
            //    "EDIT BALLOT STYLE",
            //    new Thickness(0, 0, 0, 5),
            //    param => NavigationMenuMethods.EditBallotStylePage()
            //));
            //buttonList.Add(new MenuButtonViewModel(
            //    FontAwesome.WPF.FontAwesomeIcon.BarChart,
            //    "EARLY VOTING REPORTS",
            //    "EARLY VOTING REPORTS",
            //    new Thickness(0, 0, 0, 5),
            //    param => NavigationMenuMethods.DailyReportsPage()
            //));
            //buttonList.Add(new MenuButtonViewModel(
            //    FontAwesome.WPF.FontAwesomeIcon.Clipboard,
            //    "EMERGENCY BALLOTS",
            //    "EMERGENCY BALLOTS",
            //    new Thickness(0, 0, 0, 5),
            //    param => NavigationMenuMethods.EmergencyBallotsPage()
            //));
            //if (buttonList.Count() != null)
            //{
            //    foreach (var button in buttonList.GetButtons())
            //    {
            //        CenterRegion.Children.Add(button);
            //    }
            //}
        }
    }
}
