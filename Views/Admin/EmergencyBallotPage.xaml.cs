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
using VoterX.Core.Elections;
using VoterX.Utilities.Extensions;
using VoterX.Utilities.Methods;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Admin
{
    /// <summary>
    /// Interaction logic for AdministrationPage.xaml
    /// </summary>
    public partial class EmergencyBallotPage : Page
    {
        public EmergencyBallotPage()
        {
            InitializeComponent();

            StatusBar.PageHeader = "EMERGENCY BALLOTS";

            StatusBar.CheckPrinter(AppSettings.Printers);
            //StatusBar.CheckSignaturePad();

            StatusBar.Clear();

            CheckServer();

            LoadBallotStyles();
        }

        private async void CheckServer()
        {
            //if (await StatusBar.CheckServer(VoterMethods.Container) == true)
            //{
                
            //}
        }

        private async void LoadBallotStyles()
        {
            // Create animated loading list item
            var loadingItem = ComboBoxMethods.AddLoadingItem(BallotStyleList, TempLoadingSpinnerItem);

            if (await Task.Run(() => ElectionDataMethods.Exists) == true)
            {
                foreach (var ballotstyle in await Task.Run(() => ElectionDataMethods.BallotStyles.DistinctBallots()))
                {
                    ComboBoxMethods.AddComboItemToControl(
                        BallotStyleList,
                        ballotstyle.BallotStyleFileName,
                        ballotstyle.BallotStyleName,
                        ""
                        );
                }
            }
            else
            {
                StatusBar.TextCenter = "Database not found";
            }

            // Remove animated loading list item
            ComboBoxMethods.RemoveListItem(BallotStyleList, loadingItem);

            BallotStyleList.SelectedIndex = -1;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Open Menu 
            MainMenuMethods.OpenMenu();

            this.NavigateToPage(new AdministrationPage());
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (BallotStyleList.SelectedIndex > -1)
            {
                string ballotstylefile = ComboBoxMethods.GetSelectedItemData(BallotStyleList).ToString();
                StatusBar.TextCenter = BallotPrinting.PrintEmergencyBallot(AppSettings.Global, ballotstylefile);
            }
        }
    }
}
