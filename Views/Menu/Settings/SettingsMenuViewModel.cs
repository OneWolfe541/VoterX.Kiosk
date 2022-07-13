using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Controls;
using VoterX.Kiosk.Methods;
using VoterX.Kiosk.Views.Settings;

namespace VoterX.Kiosk.Views.Menu
{
    public class SettingsMenuViewModel : NotifyPropertyChanged
    {
        private SettingsPage _settingsPage;

        public SettingsMenuViewModel(SettingsPage page)
        {
            _settingsPage = page;
        }

        public bool HomeRegionVisibility { get; set; }

        private ObservableCollection<MenuButton> _homeCustomControls;
        public ObservableCollection<MenuButton> HomeCustomControls
        {
            get
            {
                //if (_homeCustomControls == null)
                //{
                //    // Create factory
                //    MenuButtonFactory buttonFactory = new MenuButtonFactory();

                //    _homeCustomControls = new ObservableCollection<MenuButton>();

                //    _homeCustomControls.Add(buttonFactory.CreateButton(
                //            FontAwesome.WPF.FontAwesomeIcon.Home,
                //            "MANAGE HOME",
                //            "MANAGE HOME",
                //            new Thickness(0, 0, 0, 0),
                //            param => NavigationMenuMethods.ManagePage()
                //        ));

                //}
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
                        "SYSTEM",
                        "SYSTEM",
                        new Thickness(0, 5, 0, 0),
                        param => _settingsPage.LoadSystemPage()
                    ));

                    _centerCustomControls.Add(buttonFactory.CreateButton(
                        FontAwesome.WPF.FontAwesomeIcon.Clipboard,
                        "NETWORK",
                        "NETWORK",
                        new Thickness(0, 25, 0, 0),
                        param => _settingsPage.LoadNetworkPage()
                    ));

                    _centerCustomControls.Add(buttonFactory.CreateButton(
                        FontAwesome.WPF.FontAwesomeIcon.Print,
                        "PRINTERS",
                        "PRINTERS",
                        new Thickness(0, 25, 0, 0),
                        param => _settingsPage.LoadPrintersPage()
                    ));

                    _centerCustomControls.Add(buttonFactory.CreateButton(
                        FontAwesome.WPF.FontAwesomeIcon.CheckSquareOutline,
                        "ELECTION",
                        "ELECTION",
                        new Thickness(0, 25, 0, 0),
                        param => _settingsPage.LoadElectionPage()
                    ));

                    _centerCustomControls.Add(buttonFactory.CreateButton(
                        FontAwesome.WPF.FontAwesomeIcon.FilePdfOutline,
                        "BALLOTS",
                        "BALLOTS",
                        new Thickness(0, 25, 0, 0),
                        param => _settingsPage.LoadBallotsPage()
                    ));

                    _centerCustomControls.Add(buttonFactory.CreateButton(
                        FontAwesome.WPF.FontAwesomeIcon.PencilSquareOutline,
                        "EDIT VOTER",
                        "EDIT VOTER",
                        new Thickness(0, 65, 0, 0),
                        param => NavigationMenuMethods.SuperSearchPage()
                    ));

                    //_centerCustomControls.Add(buttonFactory.CreateButton(
                    //    FontAwesome.WPF.FontAwesomeIcon.Copy,
                    //    "MOVE VOTER CREDIT",
                    //    "MOVE VOTER CREDIT",
                    //    new Thickness(0, 15, 0, 0),
                    //    param => NavigationMenuMethods.CopyVoterDetailsPage()
                    //));

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

                    _exitCustomControls.Add(buttonFactory.CreateButton(
                            FontAwesome.WPF.FontAwesomeIcon.Save,
                            "SAVE",
                            "SAVE",
                            new Thickness(0, 0, 0, 25),
                            param => _settingsPage.SaveSettings_Click(new object(), new RoutedEventArgs())
                        ));

                    _exitCustomControls.Add(buttonFactory.CreateButton(
                            FontAwesome.WPF.FontAwesomeIcon.SignOut,
                            "LOG OUT",
                            "LOG OUT",
                            new Thickness(0, 0, 0, 5),
                            param => _settingsPage.BackButton_Click(new object(), new RoutedEventArgs())
                        ));

                }
                ExitRegionVisibility = true;
                RaisePropertyChanged("ExitRegionVisibility");
                return _exitCustomControls;
            }
        }
    }
}
