using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VoterX.Utilities.Controls;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Menu
{
    public class ManageMenuViewModel
    {
        public ManageMenuViewModel()
        {

        }

        private ObservableCollection<MenuButton> _homeCustomControls;
        public ObservableCollection<MenuButton> HomeCustomControls
        {
            get
            {
                if (_homeCustomControls == null)
                {
                    // Create factory
                    MenuButtonFactory buttonFactory = new MenuButtonFactory();

                    _homeCustomControls = new ObservableCollection<MenuButton>();

                    _homeCustomControls.Add(buttonFactory.CreateButton(
                            FontAwesome.WPF.FontAwesomeIcon.Home,
                            "MANAGE HOME",
                            "MANAGE HOME",
                            new Thickness(0, 5, 0, 0),
                            param => NavigationMenuMethods.ManagePage()
                        ));

                }
                return _homeCustomControls;
            }
        }

        private ObservableCollection<MenuButton> _centerCustomControls;
        public ObservableCollection<MenuButton> CenterCustomControls
        {
            get
            {
                if (_centerCustomControls == null)
                {
                    // Create factory
                    MenuButtonFactory buttonFactory = new MenuButtonFactory();

                    _centerCustomControls = new ObservableCollection<MenuButton>();

                    _centerCustomControls.Add(buttonFactory.CreateButton(
                        FontAwesome.WPF.FontAwesomeIcon.BarChart,
                        "DAILY REPORTS",
                        "DAILY REPORTS",
                        new Thickness(0, 25, 0, 0),
                        param => NavigationMenuMethods.DailyReportsPage()
                    ));

                    _centerCustomControls.Add(buttonFactory.CreateButton(
                        FontAwesome.WPF.FontAwesomeIcon.UserPlus,
                        "ADD PROVISIONAL VOTER",
                        "ADD PROVISIONAL VOTER",
                        new Thickness(0, 25, 0, 0),
                        param => NavigationMenuMethods.AddProvisionalPage()
                    ));

                    // "&#x1F50D;" // XAML CODE FOR MAGNIFYING GLASS
                    // http://www.fileformat.info/info/unicode/char/1f50d/index.htm
                    _centerCustomControls.Add(buttonFactory.CreateButton(
                        "\uD83D\uDD0D",
                        32,
                        32,
                        "PROVISIONAL SEARCH",
                        "PROVISIONAL SEARCH",
                        new Thickness(0, 25, 0, 0),
                        param => NavigationMenuMethods.SearchProvisionalPage()
                    ));

                    _centerCustomControls.Add(buttonFactory.CreateButton(
                        FontAwesome.WPF.FontAwesomeIcon.PencilSquareOutline,
                        "EDIT BALLOT STYLE",
                        "EDIT BALLOT STYLE",
                        new Thickness(0, 25, 0, 0),
                        param => NavigationMenuMethods.EditBallotStylePage()
                    ));

                    _centerCustomControls.Add(buttonFactory.CreateButton(
                        FontAwesome.WPF.FontAwesomeIcon.Clipboard,
                        "EMERGENCY BALLOTS",
                        "EMERGENCY BALLOTS",
                        new Thickness(0, 25, 0, 0),
                        param => NavigationMenuMethods.EmergencyBallotsPage()
                    ));

                    _centerCustomControls.Add(buttonFactory.CreateButton(
                        FontAwesome.WPF.FontAwesomeIcon.Download,
                        "END OF DAY",
                        "END OF DAY",
                        new Thickness(0, 25, 0, 0),
                        param => NavigationMenuMethods.EndOfDayPage(AppSettings.Election.ElectionType)
                    ));

                }
                return _centerCustomControls;
            }
        }

        private ObservableCollection<MenuButton> _exitCustomControls;
        public ObservableCollection<MenuButton> ExitCustomControls
        {
            get
            {
                if (_exitCustomControls == null)
                {
                    // Create factory
                    MenuButtonFactory buttonFactory = new MenuButtonFactory();

                    _exitCustomControls = new ObservableCollection<MenuButton>();

                    _exitCustomControls.Add(buttonFactory.CreateButton(
                            FontAwesome.WPF.FontAwesomeIcon.SignOut,
                            "LOG OUT",
                            "LOG OUT",
                            new Thickness(0, 0, 0, 5),
                            param => NavigationMenuMethods.LoginPage()
                        ));

                }
                return _exitCustomControls;
            }
        }
    }
}

