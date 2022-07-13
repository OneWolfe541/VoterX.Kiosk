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
using VoterX.Kiosk.Views.Settings;

namespace VoterX.Kiosk.Views.Menu
{
    /// <summary>
    /// Interaction logic for SystemSettingsMenu.xaml
    /// </summary>
    public partial class SystemSettingsMenu : Page
    {
        private SettingsPage _settingsPage;

        public SystemSettingsMenu(SettingsPage page)
        {
            InitializeComponent();

            _settingsPage = page;
        }

        private void SystemMenuButton_Click(object sender, RoutedEventArgs e)
        {
            _settingsPage.LoadSystemPage();
        }

        private void NetworkMenuButton_Click(object sender, RoutedEventArgs e)
        {
            _settingsPage.LoadNetworkPage();
        }

        private void PrintersMenuButton_Click(object sender, RoutedEventArgs e)
        {
            _settingsPage.LoadPrintersPage();
        }

        private void ElectionMenuButton_Click(object sender, RoutedEventArgs e)
        {
            _settingsPage.LoadElectionPage();
        }

        private void BallotsMenuButton_Click(object sender, RoutedEventArgs e)
        {
            _settingsPage.LoadBallotsPage();
        }

        private void SaveMenuButton_Click(object sender, RoutedEventArgs e)
        {
            _settingsPage.SaveSettings_Click(sender, e);
        }

        private void LogoutMenuButton_Click(object sender, RoutedEventArgs e)
        {
            _settingsPage.BackButton_Click(sender, e);
        }
    }
}
