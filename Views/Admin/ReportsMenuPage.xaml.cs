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
using VoterX.Utilities.Methods;
using VoterX.Kiosk.Factories;
using VoterX.Utilities.Interfaces;

namespace VoterX.Kiosk.Views.Admin
{
    /// <summary>
    /// Interaction logic for ReportsMenuPage.xaml
    /// </summary>
    public partial class ReportsMenuPage : Page
    {
        private string _DateOptions = "TODAY";

        public ReportsMenuPage()
        {
            InitializeComponent();

            StatusBar.PageHeader = "Reports";

            LoadActiveDates();

            LoadReports();
        }

        private void LoadReports()
        {
            var reportList = VCCReportingFactory.GetVCCReports();

            foreach (var report in reportList)
            {
                ComboBoxMethods.AddComboItemToControl(
                    ReportList,
                    report,
                    report.ReportName,
                    false
                    );
            }

            ReportList.SelectedIndex = 0;
        }

        private async void LoadActiveDates()
        {
            // Create animated loading list item
            var loadingItem = ComboBoxMethods.AddLoadingItem(ActiveDateList, TempLoadingSpinnerItem);

            // Check if the server is alive
            if (await Task.Run(() => VoterMethods.Exists) == true)
            {
                // Get list of active dates
                foreach (var dateList in await Task.Run(() => VoterMethods.Voters.ActivityDates((int)AppSettings.System.SiteID)))
                {
                    // Add date to combo box
                    // and select todays todat
                    ComboBoxMethods.AddComboItemToControl(
                        ActiveDateList,
                        dateList,
                        dateList,
                        DateTime.Now.ToShortDateString()
                        );
                }
            }
            else
            {
                StatusBar.TextCenter = "Database not found";
            }

            // Remove animated loading list item
            ComboBoxMethods.RemoveListItem(ActiveDateList, loadingItem);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Open Admin Menu
            MainMenuMethods.OpenMenu();

            this.NavigateToPage(new Admin.AdministrationPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new Login.LoginPage());
        }

        private void ReportList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TodaysActivityRadio_Click(object sender, RoutedEventArgs e)
        {
            // Set todays option
            _DateOptions = "TODAY";

            SpecificDatePanel.Visibility = Visibility.Collapsed;
        }

        private void SpecificDateRadio_Click(object sender, RoutedEventArgs e)
        {
            // Set specific option
            _DateOptions = "SPECIFIC";

            SpecificDatePanel.Visibility = Visibility.Visible;
        }

        private void ActiveDateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //StatusBar.StatusTextLeft = ActiveDateList.SelectedValue.ToString();
            //StatusBar.StatusTextLeft = ComboBoxMethods.GetSelectedItem(ActiveDateList);
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            // Get selected item
            var item = (System.Windows.Controls.ComboBoxItem)ReportList.SelectedItem;

            // Convert selected item into Reporting Object
            var report = (IReports)item.DataContext;

            string message = "";

            // Check which date option is selected
            switch (_DateOptions)
            {
                // Todays activity only
                case "TODAY":
                    message = report.PrintReport(
                        AppSettings.Election.ElectionID,
                        (int)AppSettings.System.SiteID,
                        AppSettings.Global);
                    break;

                // Selected a specific date
                case "SPECIFIC":
                    // Initialize the date
                    DateTime theDate = DateTime.Now;

                    // Vaidate the given date
                    if (DateTime.TryParse(ComboBoxMethods.GetSelectedItem(ActiveDateList), out theDate) == true)
                    {
                        message = report.PrintReport(
                            AppSettings.Election.ElectionID,
                            (int)AppSettings.System.SiteID,
                            theDate,
                            AppSettings.Global);
                    }
                    else
                    {
                        message = "Invalid Date";
                    }
                    break;
            }

            StatusBar.TextCenter = message;
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            // Get selected item
            var item = (System.Windows.Controls.ComboBoxItem)ReportList.SelectedItem;

            // Convert selected item into Reporting Object
            var report = (IReports)item.DataContext;

            string message = "";

            // Check which date option is selected
            switch (_DateOptions)
            {
                // Todays activity only
                case "TODAY":
                    message = report.PreviewReport(
                        AppSettings.Election.ElectionID,
                        (int)AppSettings.System.SiteID,
                        AppSettings.Global);
                    break;

                // Selected a specific date
                case "SPECIFIC":
                    // Initialize the date
                    DateTime theDate = DateTime.Now;

                    // Vaidate the given date
                    if (DateTime.TryParse(ComboBoxMethods.GetSelectedItem(ActiveDateList), out theDate) == true)
                    {
                        message = report.PreviewReport(
                            AppSettings.Election.ElectionID,
                            (int)AppSettings.System.SiteID,
                            theDate,
                            AppSettings.Global);
                    }
                    else
                    {
                        message = "Invalid Date";
                    }
                    break;
            }

            StatusBar.TextCenter = message;
        }
    }
}
