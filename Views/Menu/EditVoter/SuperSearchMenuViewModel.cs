using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Controls;
using VoterX.Kiosk.Views.Super.VoterDetails;
using VoterX.Kiosk.Methods;
using VoterX.Kiosk.Views.Settings;
using VoterX.Kiosk.Views.Super.Search;

namespace VoterX.Kiosk.Views.Menu
{
    public class SuperSearchMenuViewModel : NotifyPropertyChanged
    {
        private SuperSearchPage _superSearchPage;

        public SuperSearchMenuViewModel(SuperSearchPage page)
        {
            _superSearchPage = page;
        }

        public bool HomeRegionVisibility { get; set; }

        private ObservableCollection<MenuButton> _homeCustomControls;
        public ObservableCollection<MenuButton> HomeCustomControls
        {
            get
            {
                if (_homeCustomControls == null)
                {
                    // Create factory
                    //MenuButtonFactory buttonFactory = new MenuButtonFactory();

                    //_homeCustomControls = new ObservableCollection<MenuButton>();

                    //_homeCustomControls.Add(buttonFactory.CreateButton(
                    //        FontAwesome.WPF.FontAwesomeIcon.Home,
                    //        "BACK TO SEARCH",
                    //        "BACK TO SEARCH",
                    //        new Thickness(0, 5, 0, 0),
                    //        param => _voterDetailsPage.GoBackCommand.Execute(null)
                    //    ));

                }
                _homeCustomControls = null;
                HomeRegionVisibility = false;
                RaisePropertyChanged("HomeRegionVisibility");
                return _homeCustomControls;
            }
        }

        public bool CenterRegionVisibility { get; set; }

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
                        FontAwesome.WPF.FontAwesomeIcon.Gear,
                        "SETTINGS HOME",
                        "SETTINGS HOME",
                        new Thickness(0, 5, 0, 0),
                        param => NavigationMenuMethods.SettingsPage()
                    ));

                }
                CenterRegionVisibility = true;
                RaisePropertyChanged("CenterRegionVisibility");
                return _centerCustomControls;
            }
        }

        public bool ExitRegionVisibility { get; set; }

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

                    //_exitCustomControls.Add(buttonFactory.CreateButton(
                    //        FontAwesome.WPF.FontAwesomeIcon.SignOut,
                    //        "LOG OUT",
                    //        "LOG OUT",
                    //        new Thickness(0, 0, 0, 5),
                    //        param => _settingsPage.BackButton_Click(new object(), new RoutedEventArgs())
                    //    ));

                    _exitCustomControls.Add(buttonFactory.CreateButton(
                                FontAwesome.WPF.FontAwesomeIcon.SignOut,
                                "LOG OUT",
                                "LOG OUT",
                                new Thickness(0, 0, 0, 5),
                                param => _superSearchPage.LogOutButton_Click(new object(), new RoutedEventArgs())
                            ));

                }
                ExitRegionVisibility = true;
                RaisePropertyChanged("ExitRegionVisibility");
                return _exitCustomControls;
            }
        }

        private void SettingsPage()
        {

        }
    }
}
