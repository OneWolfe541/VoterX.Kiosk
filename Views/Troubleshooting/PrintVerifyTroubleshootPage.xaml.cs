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
using VoterX.Utilities.Extensions;
using VoterX.SystemSettings.Extensions;
using VoterX.Utilities.Methods;
using VoterX.Utilities.Dialogs;
using VoterX.SystemSettings.Enums;

namespace VoterX.Kiosk.Views.Troubleshooting
{
    /// <summary>
    /// Interaction logic for PrintVerifyTroubleshootPage.xaml
    /// </summary>
    public partial class PrintVerifyTroubleshootPage : Page
    {
        private NMVoter _voter = new NMVoter();

        private int printingErrors = 0;

        private bool exitLogout = false;

        public PrintVerifyTroubleshootPage(NMVoter voter)
        {
            InitializeComponent();

            _voter = voter;

            DisplayVotersBallot();

            StatusBar.PageHeader = "Print Verification";

            StatusBar.Clear();

            CheckServer();
        }

        public PrintVerifyTroubleshootPage(NMVoter voter, bool returnStatus)
        {
            InitializeComponent();

            _voter = voter;

            // Return Status is true when this page was called from a Management Screen
            exitLogout = returnStatus;
            if (returnStatus == true)
            {
                BackReturnLabel.Visibility = Visibility.Collapsed;
                BackLogoutLabel.Visibility = Visibility.Visible;
            }

            DisplayVotersBallot();

            StatusBar.PageHeader = "Print Verification";

            //StatusBar.Clear();

            CheckServer();
        }

        private async void CheckServer()
        {
            PrintVerificationGrid.Visibility = Visibility.Collapsed;

            if (await StatusBar.CheckServer(ElectionDataMethods.Election) == true)
            {
                PrintVerificationGrid.Visibility = Visibility.Visible;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (exitLogout == false)
            {
                this.NavigateToPage(new Voter.SearchPage(null));
            }
            else
            {
                // Open Admin Menu
                MainMenuMethods.OpenMenu();

                this.NavigateToPage(new Admin.AdministrationPage());
            }
        }

        private void NextVerifyButton_Click(object sender, RoutedEventArgs e)
        {
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

        private void DisplayVotersBallot()
        {
            BallotStyleName.Text = _voter.Data.BallotStyle;
            BallotNumber.Text = _voter.Data.BallotNumber.ToString();
        }

        private void ResetQuestionsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BallotPrintQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            // When Answer is True = Yes was clicked
            if (BallotPrintQuestion.GetAnswer() == true)
            {
                if (AppSettings.System.VCCType == VotingCenterMode.ElectionDay) // If election day
                {
                    PermitPrintQuestion.Visibility = Visibility.Visible;
                    PermitPrintBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    //ApplicationVerificationPanel.Visibility = Visibility.Visible;
                    ApplicationPrintQuestion.Visibility = Visibility.Visible;
                    ApplicationPrintBorder.Visibility = Visibility.Visible;
                }
            }
            // Answer False = No was clicked
            else
            {
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

        }

        private void BallotPrinterCheckQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (BallotPrinterCheckQuestion.GetAnswer() == true)
            {
                // reset printer check
                BallotPrintQuestion.ChangeAnswer(true);

                // hide self
                BallotPrinterCheckPanel.Visibility = Visibility.Collapsed;
                BallotPrinterCheckQuestion.Visibility = Visibility.Collapsed;
                BallotPrinterCheckQuestion.Reset();

                // show the next question
                if (AppSettings.System.VCCType == VotingCenterMode.ElectionDay)
                //if(true)
                {
                    PermitPrintQuestion.Visibility = Visibility.Visible;
                    PermitPrintBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    ApplicationPrintQuestion.Visibility = Visibility.Visible;
                    ApplicationPrintBorder.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // Display options panel
                BallotOptionsPanel.Visibility = Visibility.Visible;
            }
        }

        private async void ReprintBallotCheck_Click(object sender, RoutedEventArgs e)
        {
            // Uncheck the other box
            TransferVoterCheck.IsChecked = false;

            OkCancelDialog spoilDialog = new OkCancelDialog("Spoil The Ballot", "A NEW BALLOT WILL BE SENT TO THE PRINTER\r\nAND THE PREVIOUS BALLOT WILL BE SPOILED.\r\n\r\nMAKE SURE TO FOLLOW THE PROPER PROCEDURES\r\nTO SPOIL THE OLD BALLOT.");
            if (spoilDialog.ShowDialog() == true)
            {
                // Print a new ballot
                await Task.Run(() => BallotPrinting.ReprintBallot(_voter.Data, AppSettings.Global));

                // Spoil previous ballot with "print error" reason
                VoterManagmentMethods.InsertSpoiledBallot(
                _voter,
                1,
                AppSettings.Global);

                //// Get new ballot number
                //_voter.BallotNumber = VoterMethods.Voter.GetNextBallotNumber((int)AppSettings.System.SiteID);

                // Store new ballot number in voted record
                VoterManagmentMethods.UpdateVoterBallotNumber(_voter, AppSettings.Global);

                BallotNumber.Text = _voter.Data.BallotNumber.ToString();

                // Update Ballot Number
                if (AppSettings.System.VCCType == VotingCenterMode.ElectionDay && AppSettings.System.Permit == 1) // If Election Day
                {
                    // Print a new permit
                    await Task.Run(() => BallotPrinting.ReprintPermitSpoiled(_voter.Data, AppSettings.Global));
                }

                // Display new printing message
                BallotPrintingStatus.Text = "A NEW BALLOT WAS SENT TO THE PRINTER";

                // Reset and hide questions
                BallotPrintQuestion.Reset();
                BallotPrinterCheckQuestion.Reset();
                BallotPrinterCheckPanel.Visibility = Visibility.Collapsed;
                BallotOptionsPanel.Visibility = Visibility.Collapsed;
                ReprintBallotCheck.IsChecked = false;
                TransferVoterCheck.IsChecked = false;
            }
            else
            {
                ReprintBallotCheck.IsChecked = false;
            }
        }

        private void TransferVoterCheck_Click(object sender, RoutedEventArgs e)
        {
            // Uncheck the other box
            ReprintBallotCheck.IsChecked = false;

            // Return to search screen or logout
            if (exitLogout == false)
            {
                this.NavigateToPage(new Voter.SearchPage(null));
            }
            else
            {
                this.NavigateToPage(new Admin.AdministrationPage());
            }
        }

        private void ApplicationPrintQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (ApplicationPrintQuestion.GetAnswer() == true)
            {
                // Check for Stub
                if (AppSettings.System.BallotStub == 1)
                {
                    // Display Stub Question
                    StubPrintQuestion.Visibility = Visibility.Visible;
                    StubPrintBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    // Display Return to search button
                    NextVerify.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // Show the troubleshooting message
                ApplicationOptionsPanel.Visibility = Visibility.Visible;
            }
        }

        private async void ReprintApplicationCheck_Click(object sender, RoutedEventArgs e)
        {
            // Uncheck the other box
            TransferVoterCheck.IsChecked = false;

            // Print a new application
            await Task.Run(() => BallotPrinting.ReprintApplication(_voter.Data, AppSettings.Global));

            // Display new printing message
            //BallotPrintingStatus.Text = "A new Ballot was sent to the Printer";

            // Reset and hide questions
            ApplicationPrintQuestion.Reset();
            ApplicationOptionsPanel.Visibility = Visibility.Collapsed;
            ReprintApplicationCheck.IsChecked = false;
            ApplicationTransferVoterCheck.IsChecked = false;
        }

        private void ApplicationTransferVoterCheck_Click(object sender, RoutedEventArgs e)
        {
            ReprintApplicationCheck.IsChecked = false;
            // Return to search screen
            if (exitLogout == false)
            {
                this.NavigateToPage(new Voter.SearchPage(null));
            }
            else
            {
                this.NavigateToPage(new Admin.AdministrationPage());
            }
        }

        private void PermitPrintQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (PermitPrintQuestion.GetAnswer() == true)
            {
                // Check for Stub
                if (AppSettings.System.BallotStub == 1)
                {
                    // Display Stub Question
                    StubPrintQuestion.Visibility = Visibility.Visible;
                    StubPrintBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    // Display Return to search button
                    NextVerify.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // Display Troubleshooting message & question
                PermitOptionsPanel.Visibility = Visibility.Visible;
            }
        }

        private async void PermitReprintCheck_Click(object sender, RoutedEventArgs e)
        {
            // Uncheck the other box
            PermitTransferVoterCheck.IsChecked = false;

            // Print a new permit
            await Task.Run(() => BallotPrinting.ReprintPermit(_voter.Data, AppSettings.Global));

            // Display new printing message
            //BallotPrintingStatus.Text = "A new Ballot was sent to the Printer";

            // Reset and hide questions
            PermitPrintQuestion.Reset();
            PermitOptionsPanel.Visibility = Visibility.Collapsed;
            PermitReprintCheck.IsChecked = false;
            PermitTransferVoterCheck.IsChecked = false;
        }

        private void PermitTransferVoterCheck_Click(object sender, RoutedEventArgs e)
        {
            ReprintApplicationCheck.IsChecked = false;
            // Return to search screen
            if (exitLogout == false)
            {
                this.NavigateToPage(new Voter.SearchPage(null));
            }
            else
            {
                this.NavigateToPage(new Admin.AdministrationPage());
            }
        }

        private void StubPrintQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (StubPrintQuestion.GetAnswer() == true)
            {
                // Display Return to search button
                NextVerify.Visibility = Visibility.Visible;
            }
            else
            {
                // Display Troubleshooting message & question
                StubOptionsPanel.Visibility = Visibility.Visible;
            }
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
            StubPrintQuestion.Reset();
            StubOptionsPanel.Visibility = Visibility.Collapsed;
            StubReprintCheck.IsChecked = false;
            StubTransferVoterCheck.IsChecked = false;
        }

        private void StubTransferVoterCheck_Click(object sender, RoutedEventArgs e)
        {
            ReprintApplicationCheck.IsChecked = false;
            // Return to search screen
            if (exitLogout == false)
            {
                this.NavigateToPage(new Voter.SearchPage(null));
            }
            else
            {
                this.NavigateToPage(new Admin.AdministrationPage());
            }
        }

        
    }
}
