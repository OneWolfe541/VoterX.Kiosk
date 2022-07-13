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
using VoterX.Core.Voters;
using VoterX.Kiosk.Methods;
using System.Windows.Controls.Primitives;
using FontAwesome.WPF;
using VoterX.SystemSettings.Extensions;
using VoterX.Utilities.Methods;
using VoterX.Utilities.Extensions;
using VoterX.SystemSettings.Enums;

namespace VoterX.Kiosk.Views.Troubleshooting
{
    /// <summary>
    /// Interaction logic for SpoiledVerifyTroubleshootPage.xaml
    /// </summary>
    public partial class SpoiledVerifyTroubleshootPage : Page
    {
        private NMVoter _voter = new NMVoter();

        private int printingErrors = 0;

        private bool exitLogout = false;

        private int _spoiledReason;

        public SpoiledVerifyTroubleshootPage(NMVoter voter)
        {
            InitializeComponent();

            _voter = voter;

            DisplayVotersBallot();

            Methods.StatusBar.PageHeader = "Spoiled Print Verification";

            Methods.StatusBar.Clear();

            CheckServer();
        }

        public SpoiledVerifyTroubleshootPage(NMVoter voter, bool returnStatus)
        {
            InitializeComponent();

            _voter = voter;

            exitLogout = returnStatus;

            DisplayVotersBallot();

            Methods.StatusBar.PageHeader = "Spoiled Print Verification";

            Methods.StatusBar.Clear();

            CheckServer();
        }

        public SpoiledVerifyTroubleshootPage(NMVoter voter, int spoiledReason)
        {
            InitializeComponent();

            _voter = voter;

            _spoiledReason = spoiledReason;

            DisplayVotersBallot();

            Methods.StatusBar.PageHeader = "Spoiled Print Verification";

            Methods.StatusBar.Clear();

            CheckServer();
        }

        private async void CheckServer()
        {
            PrintVerificationGrid.Visibility = Visibility.Collapsed;

            if (await Methods.StatusBar.CheckServer(ElectionDataMethods.Election) == true)
            {
                PrintVerificationGrid.Visibility = Visibility.Visible;
            }
        }

        private void DisplayVotersBallot()
        {
            BallotStyleName.Text = _voter.Data.BallotStyle;
            BallotNumber.Text = _voter.Data.BallotNumber.ToString();
        }

        private void CheckYesNoPair(ToggleButton buttonToCheck, ToggleButton buttonToUncheck)
        {
            // Set checked button
            buttonToCheck.IsChecked = true;
            buttonToCheck.IsEnabled = false;

            var childListChecked = FindVisualChildren<ImageAwesome>(buttonToCheck);
            var iconChecked = childListChecked.FirstOrDefault();
            if (iconChecked != null)
            {
                iconChecked.Visibility = Visibility.Visible;
            }

            // Set unchecked button
            buttonToUncheck.IsChecked = false;
            buttonToUncheck.IsEnabled = false;

            var childListUnchecked = FindVisualChildren<ImageAwesome>(buttonToUncheck);
            var iconUnchecked = childListUnchecked.FirstOrDefault();
            if (iconUnchecked != null)
            {
                iconUnchecked.Visibility = Visibility.Hidden;
            }
        }

        private void ClearYesNoPair(ToggleButton buttonYes, ToggleButton buttonNo)
        {
            // Set checked button
            buttonYes.IsChecked = false;
            buttonYes.IsEnabled = true;

            var childListYes = FindVisualChildren<ImageAwesome>(buttonYes);
            var iconYes = childListYes.FirstOrDefault();
            if (iconYes != null)
            {
                iconYes.Visibility = Visibility.Hidden;
            }

            // Set unchecked button
            buttonNo.IsChecked = false;
            buttonNo.IsEnabled = true;

            var childListNo = FindVisualChildren<ImageAwesome>(buttonNo);
            var iconNo = childListNo.FirstOrDefault();
            if (iconNo != null)
            {
                iconNo.Visibility = Visibility.Hidden;
            }
        }

        // Find all children of tpye<t> sample
        // https://stackoverflow.com/questions/974598/find-all-controls-in-wpf-window-by-type
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void BallotValidationYes_Click(object sender, RoutedEventArgs e)
        {
            // Set Yes to checked and No to Disabled
            CheckYesNoPair(BallotValidationYes, BallotValidationNo);

            // Show the Next Question
            if (AppSettings.Election.IsElectionDay())
            //if (true)
            {
                PermitVerificationPanel.Visibility = Visibility.Visible;
            }
            else
            {
                NextVerify.Visibility = Visibility.Visible;
            }
        }

        private void BallotValidationNo_Click(object sender, RoutedEventArgs e)
        {
            // Set No to checked and Yes to Disabled
            CheckYesNoPair(BallotValidationNo, BallotValidationYes);

            if (printingErrors == 0)
            {
                // Show the troubleshooting message
                BallotPrinterCheckPanel.Visibility = Visibility.Visible;
            }
            else
            {
                NextVerify.Visibility = Visibility.Visible;
            }
            printingErrors++;
        }

        private void BallotPrinterCheckYes_Click(object sender, RoutedEventArgs e)
        {
            // Set Yes to checked and No to Disabled
            CheckYesNoPair(BallotPrinterCheckYes, BallotPrinterCheckNo);

            // Reset Printer Check
            ClearYesNoPair(BallotValidationYes, BallotValidationNo);
            ClearYesNoPair(BallotPrinterCheckYes, BallotPrinterCheckNo);
            BallotPrinterCheckPanel.Visibility = Visibility.Collapsed;
            BallotOptionsPanel.Visibility = Visibility.Collapsed;
            ReprintBallotCheck.IsChecked = false;
            TransferVoterCheck.IsChecked = false;
            CheckYesNoPair(BallotValidationYes, BallotValidationNo);

            // Show the Next Question
            if (AppSettings.Election.IsElectionDay())
            //if(true)
            {
                PermitVerificationPanel.Visibility = Visibility.Visible;
            }
            else
            {
                NextVerify.Visibility = Visibility.Visible;
            }
        }

        private void BallotPrinterCheckNo_Click(object sender, RoutedEventArgs e)
        {
            // Set No to checked and Yes to Disabled
            CheckYesNoPair(BallotPrinterCheckNo, BallotPrinterCheckYes);
            // Display options panel
            BallotOptionsPanel.Visibility = Visibility.Visible;
        }

        private async void ReprintBallotCheck_Click(object sender, RoutedEventArgs e)
        {
            if (await Task.Run(() => VoterMethods.Exists) == true)
            {
                // Uncheck the other box
                TransferVoterCheck.IsChecked = false;

                // Print a new ballot
                await Task.Run(() => BallotPrinting.ReprintBallot(_voter.Data, AppSettings.Global));

                // Spoil previous ballot with "print error" reason
                VoterManagmentMethods.InsertSpoiledBallot(
                _voter,
                _spoiledReason,
                AppSettings.Global);

                // Get new ballot number
                VoterManagmentMethods.UpdateVoterBallotNumber(_voter, AppSettings.Global);

                // Store new ballot number in voted record
                VoterManagmentMethods.UpdateVoterBallotNumber(_voter, AppSettings.Global);

                // Update Ballot Number
                if (AppSettings.System.VCCType == VotingCenterMode.ElectionDay && AppSettings.System.Permit == 1) // If Election Day
                {
                    // Print a new permit
                    await Task.Run(() => BallotPrinting.ReprintPermitSpoiled(_voter.Data, AppSettings.Global));
                }

                // Display new printing message
                BallotPrintingStatus.Text = "A new Ballot was sent to the Printer";

                // Reset and hide questions
                ClearYesNoPair(BallotValidationYes, BallotValidationNo);
                ClearYesNoPair(BallotPrinterCheckYes, BallotPrinterCheckNo);
                BallotPrinterCheckPanel.Visibility = Visibility.Collapsed;
                BallotOptionsPanel.Visibility = Visibility.Collapsed;
                ReprintBallotCheck.IsChecked = false;
                TransferVoterCheck.IsChecked = false;
            }
            else
            {
                ReprintBallotCheck.IsChecked = false;
                Methods.StatusBar.TextCenter = "Database not found";
            }
        }

        private void TransferVoterCheck_Click(object sender, RoutedEventArgs e)
        {
            ReprintBallotCheck.IsChecked = false;

            // Return to search screen
            if (exitLogout == false)
            {
                this.NavigateToPage(new Voter.SearchPage(null));
            }
            else
            {
                MainMenuMethods.OpenMenu();
                this.NavigateToPage(new Admin.AdministrationPage());
            }

        }

        private void PermitValidationYes_Click(object sender, RoutedEventArgs e)
        {
            // Set Yes to checked and No to Disabled
            CheckYesNoPair(PermitValidationYes, PermitValidationNo);

            // Check for Stub
            if (AppSettings.System.BallotStub == 1)
            {
                // Display Stub Question
                StubVerificationPanel.Visibility = Visibility.Visible;
            }
            else
            {
                // Display Return to search button
                NextVerify.Visibility = Visibility.Visible;
            }
        }

        private void PermitValidationNo_Click(object sender, RoutedEventArgs e)
        {
            // Set No to checked and Yes to Disabled
            CheckYesNoPair(PermitValidationNo, PermitValidationYes);

            // Display Troubleshooting message & question
            PermitOptionsPanel.Visibility = Visibility.Visible;
        }

        private async void PermitReprintCheck_Click(object sender, RoutedEventArgs e)
        {
            // Uncheck the other box
            PermitTransferVoterCheck.IsChecked = false;

            // Print a new application
            await Task.Run(() => BallotPrinting.ReprintPermit(_voter.Data, AppSettings.Global));

            // Display new printing message
            //BallotPrintingStatus.Text = "A new Ballot was sent to the Printer";

            // Reset and hide questions
            ClearYesNoPair(PermitValidationYes, PermitValidationNo);
            //ClearYesNoPair(PermitPrinterCheckYes, PermitPrinterCheckNo);
            //PermitPrinterCheckPanel.Visibility = Visibility.Collapsed;
            PermitOptionsPanel.Visibility = Visibility.Collapsed;
            PermitReprintCheck.IsChecked = false;
            PermitTransferVoterCheck.IsChecked = false;
        }

        private void PermitTransferVoterCheck_Click(object sender, RoutedEventArgs e)
        {
            PermitReprintCheck.IsChecked = false;
            // Return to search screen
            if (exitLogout == false)
            {
                this.NavigateToPage(new Voter.SearchPage(null));
            }
            else
            {
                MainMenuMethods.OpenMenu();
                this.NavigateToPage(new Admin.AdministrationPage());
            }
        }

        private void StubValidationYes_Click(object sender, RoutedEventArgs e)
        {
            // Set Yes to checked and No to Disabled
            CheckYesNoPair(StubValidationYes, StubValidationNo);

            // Display Return to search button
            NextVerify.Visibility = Visibility.Visible;
        }

        private void StubValidationNo_Click(object sender, RoutedEventArgs e)
        {
            // Set No to checked and Yes to Disabled
            CheckYesNoPair(StubValidationNo, StubValidationYes);

            // Display Troubleshooting message & question
            StubOptionsPanel.Visibility = Visibility.Visible;
        }

        private async void StubReprintCheck_Click(object sender, RoutedEventArgs e)
        {
            // Uncheck the other box
            StubTransferVoterCheck.IsChecked = false;

            // Print a new application
            await Task.Run(() => BallotPrinting.ReprintStub(_voter.Data, AppSettings.Global));

            // Display new printing message
            //BallotPrintingStatus.Text = "A new Ballot was sent to the Printer";

            // Reset and hide questions
            ClearYesNoPair(StubValidationYes, StubValidationNo);
            //ClearYesNoPair(StubPrinterCheckYes, StubPrinterCheckNo);
            //StubPrinterCheckPanel.Visibility = Visibility.Collapsed;
            StubOptionsPanel.Visibility = Visibility.Collapsed;
            StubReprintCheck.IsChecked = false;
            StubTransferVoterCheck.IsChecked = false;
        }

        private void StubTransferVoterCheck_Click(object sender, RoutedEventArgs e)
        {
            StubReprintCheck.IsChecked = false;
            // Return to search screen
            if (exitLogout == false)
            {
                this.NavigateToPage(new Voter.SearchPage(null));
            }
            else
            {
                MainMenuMethods.OpenMenu();
                this.NavigateToPage(new Admin.AdministrationPage());
            }
        }

        private void NextVerifyButton_Click(object sender, RoutedEventArgs e)
        {
            // Return to search screen
            if (exitLogout == false)
            {
                this.NavigateToPage(new Voter.SearchPage(null));
            }
            else
            {
                MainMenuMethods.OpenMenu();
                this.NavigateToPage(new Admin.AdministrationPage());
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
