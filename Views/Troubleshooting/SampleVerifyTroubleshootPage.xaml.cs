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
using VoterX.SystemSettings.Extensions;
using VoterX.Utilities.Methods;
using VoterX.Core.Voters;

namespace VoterX.Kiosk.Views.Troubleshooting
{
    /// <summary>
    /// Interaction logic for PrintVerifyTroubleshootPage.xaml
    /// </summary>
    public partial class SampleVerifyTroubleshootPage : Page
    {
        private NMVoter _voter = new NMVoter();

        private int printingErrors = 0;

        private bool exitLogout = false;

        public SampleVerifyTroubleshootPage(NMVoter voter)
        {
            InitializeComponent();

            _voter = voter;

            DisplayVotersBallot();

            StatusBar.PageHeader = "Print Verification";

            StatusBar.Clear();

            CheckServer();
        }

        public SampleVerifyTroubleshootPage(NMVoter voter, bool returnStatus)
        {
            InitializeComponent();

            _voter = voter;

            exitLogout = returnStatus;
            if (returnStatus == true)
            {
                BackReturnLabel.Visibility = Visibility.Collapsed;
                BackLogoutLabel.Visibility = Visibility.Visible;
            }

            DisplayVotersBallot();

            StatusBar.PageHeader = "Print Verification";

            StatusBar.Clear();

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
                this.NavigateToPage(new Admin.AdministrationPage());
            }
        }

        private void DisplayVotersBallot()
        {
            BallotStyleName.Text = _voter.Data.BallotStyle;
            //BallotNumber.Text = _voter.BallotNumber.ToString(); 
        }

        private void ResetQuestionsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BallotPrintQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            // When Answer is True = Yes was clicked
            if (BallotPrintQuestion.GetAnswer() == true)
            {
                //if (AppSettings.Election.IsElectionDay())
                //{
                //    PermitPrintQuestion.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    //ApplicationVerificationPanel.Visibility = Visibility.Visible;
                //    ApplicationPrintQuestion.Visibility = Visibility.Visible;
                //}

                // Display Return to search button
                NextVerify.Visibility = Visibility.Visible;
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
                //if (AppSettings.Election.IsElectionDay())
                ////if(true)
                //{
                //    PermitPrintQuestion.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    ApplicationPrintQuestion.Visibility = Visibility.Visible;
                //}

                // Display Return to search button
                NextVerify.Visibility = Visibility.Visible;
            }
            else
            {
                // Display options panel
                BallotOptionsPanel.Visibility = Visibility.Visible;
            }
        }

        private async void ReprintBallotCheck_Click(object sender, RoutedEventArgs e)
        {
            if (await Task.Run(() => VoterMethods.Exists) == true)
            {
                // Uncheck the other box
                TransferVoterCheck.IsChecked = false;

                // Print a new ballot
                await Task.Run(() => BallotPrinting.PrintSampleBallot(AppSettings.Global, _voter.Data.BallotStyleFile));

                // DO NOT SPOIL SAMPLE BALLOTS
                // Spoil previous ballot with "print error" reason
                //VoterManagmentMethods.InsertSpoiledBallot(
                //_voter,
                //1,
                //AppSettings.Global);

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
                StatusBar.TextCenter = "Database not found";
            }
        }

        private void TransferVoterCheck_Click(object sender, RoutedEventArgs e)
        {
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
        
    }
}
