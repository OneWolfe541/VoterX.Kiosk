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
//using VoterX.Core.Models.Utilities;
using VoterX.Core.Extensions;
using VoterX.Kiosk.Methods;
using VoterX.Utilities.Extensions;
using VoterX.Kiosk.Views.Voter;
using VoterX.Kiosk.Views.Signature;
using VoterX.Utilities.Methods;
using VoterX.Utilities.Dialogs;
using VoterX.Core.Voters;

namespace VoterX.Kiosk.Views.Verification
{
    /// <summary>
    /// Interaction logic for VerifyValidVoterPage.xaml
    /// </summary>
    public partial class VerifyValidVoterPage : Page
    {
        private VoterSearchModel search = new VoterSearchModel();
        private NMVoter _voter = new NMVoter();

        public VerifyValidVoterPage(VoterNavModel voterFromNav)
        {
            InitializeComponent();

            search = voterFromNav.Search;

            _voter = voterFromNav.Voter;

            LoadVoterFields(voterFromNav.Voter.Data);

            StatusBar.PageHeader = "Voter Verification";

            StatusBar.Clear();

            //StatusBar.StatusTextLeft = "Voter ID: " + voterFromNav.Voter.VoterID;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new SearchPage(search));
        }

        private void LoadVoterFields(VoterDataModel voter)
        {
            //VoterID.Text = voter.VoterID;
            FullName.Text = voter.FullName;
            BirthYear.Text = voter.DOBYear;
            Address.Text = voter.Address1;
            CityStateAndZip.Text = voter.City + ", " + voter.State + " " + voter.Zip;
            BallotStyle.Text = voter.BallotStyle;

            // Set a flag when voter ID is required 
            // Then turn on the ID varification control group
            // And turn off the other groups
            IDVarification.DataContext = voter.IDRequired;
            if ((AppSettings.System.IdRequired == true || voter.IDRequired == true) && !voter.HasVoted())
            {
                IDVarification.Visibility = Visibility.Visible;
                CheckNameGrid.Visibility = Visibility.Visible;
                CheckDateGrid.Visibility = Visibility.Visible;
                CheckAddressGrid.Visibility = Visibility.Visible;
            }
            else
            {
                IDVarification.Visibility = Visibility.Collapsed;
            }

            //StatusBar.StatusTextLeft = "Voter ID: " + voter.VoterID;
            //BallotStyle.Text = voter.VoterID;
        }

        // When any check box is clicked check of all boxes are checked
        // If all the boxes are checked turn on the Print action button group
        // Which button in the group gets displayed is determined in CheckVoterStatus()
        private void Validation_Click(object sender, RoutedEventArgs e)
        {
            string StatusMessage;
            StatusMessage = string.Concat(
                "Name: ", NameCorrect.IsChecked,
                " | Date: ", DateCorrect.IsChecked,
                " | Address: ", AddressCorrect.IsChecked,
                " | ID: ", IDRequiredCheckQuestion.GetAnswer());
            //StatusBar.ApplicationStatus(StatusMessage);

            //var trial = AllValidationBoxesChecked();
            if (AllValidationBoxesChecked()) BallotFunctions.Visibility = Visibility.Visible;
            else BallotFunctions.Visibility = Visibility.Collapsed;

            if ((AppSettings.System.IdRequired == true || (bool)IDVarification.DataContext == true) && IDRequiredCheckQuestion.GetAnswer() != null)
            {
                //var test = IDRequiredCheckQuestion.GetAnswer();
                if (
                    IDRequiredCheckQuestion.GetAnswer() == false
                    && NameCorrect.IsChecked == true
                    && DateCorrect.IsChecked == true
                    && AddressCorrect.IsChecked == true
                    )
                {
                    BallotFunctions.Visibility = Visibility.Visible;
                    Signature.Visibility = Visibility.Collapsed;
                    ProvisionalBallot.Visibility = Visibility.Visible;
                }
            }
        }

        // Returns true if all of the vadilation boxes are checked
        private bool AllValidationBoxesChecked()
        {
            // Set state to true
            bool result = true;

            // If any of the following conditions are met state will be set to false

            // Check if the voter has already voted
            if (!_voter.HasVoted())
            {
                // Voter has not already voted

                //StatusBar.StatusTextLeft = IDVarification.DataContext.ToString();
                // Check if voter ID is required     
                //if (IDRequiredCheckQuestion.GetAnswer() != null)
                //{

                //}
                //else
                //{
                //    result = false;
                //}

                if (AppSettings.System.IdRequired == true || (bool)IDVarification.DataContext == true)
                {
                    // When ID required also check name date and address
                    //var test = IDRequiredCheckQuestion.GetAnswer(); 
                    if (IDRequiredCheckQuestion.GetAnswer() == null) result = false;
                    if (IDRequiredCheckQuestion.GetAnswer() == false || IDRequiredCheckQuestion.GetAnswer() == null) result = false;
                    if (NameCorrect.IsChecked == false) result = false;
                    if (DateCorrect.IsChecked == false) result = false;
                    if (AddressCorrect.IsChecked == false) result = false;
                }
                else
                {
                    // If ID is not required only check name date and address
                    if (NameCorrect.IsChecked == false) result = false;
                    if (DateCorrect.IsChecked == false) result = false;
                    if (AddressCorrect.IsChecked == false) result = false;
                }
            }
            else
            {
                // Voter has already voted

                // When they have already voted dont need to check their ID a second time

                if (NameCorrect.IsChecked == false) result = false;
                if (DateCorrect.IsChecked == false) result = false;
                if (AddressCorrect.IsChecked == false) result = false;
            }

            // Return the final state
            return result;
        }

        private void Signature_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new SignatureCapturePage(_voter));
            //this.NavigateToPage(new SignatureCaptureUWPPage(_voter));
        }

        private void PrintApplication_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PrintPermit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PrintStub_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProvisionalButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new Ballots.ProvisionalBallotPage(_voter));
        }

        private void PrintSignatureForm_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PrintBallot_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new Ballots.PrintBundlePage(_voter));
        }

        private void IDRequiredCheckQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (AllValidationBoxesChecked()) BallotFunctions.Visibility = Visibility.Visible;
            else BallotFunctions.Visibility = Visibility.Collapsed;

            if ((AppSettings.System.IdRequired == true || (bool)IDVarification.DataContext == true) && IDRequiredCheckQuestion.GetAnswer() != null)
            {
                //var test = IDRequiredCheckQuestion.GetAnswer();
                if (
                    IDRequiredCheckQuestion.GetAnswer() == false
                    && NameCorrect.IsChecked == true
                    && DateCorrect.IsChecked == true
                    && AddressCorrect.IsChecked == true
                    )
                {
                    BallotFunctions.Visibility = Visibility.Visible;
                    Signature.Visibility = Visibility.Collapsed;
                    ProvisionalBallot.Visibility = Visibility.Visible;
                }
                else
                {
                    //BallotFunctions.Visibility = Visibility.Visible;
                    Signature.Visibility = Visibility.Visible;
                    ProvisionalBallot.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async void SampleBallots_Click(object sender, RoutedEventArgs e)
        {
            if (await PrinterStatus.PrinterIsReadyAsync(AppSettings.Printers.SamplePrinter) == true)
            {
                // Display message
                YesNoDialog signatureDialog = new YesNoDialog("SAMPLE BALLOT","ARE YOU SURE YOU WANT TO PRINT A SAMPLE BALLOT?");
                if (signatureDialog.ShowDialog() == true)
                {
                    StatusBar.TextLeft = await Task.Run(() => BallotPrinting.PrintSampleBallot(AppSettings.Global, _voter.Data.BallotStyleFile));

                    this.NavigateToPage(new Troubleshooting.SampleVerifyTroubleshootPage(_voter));
                }
            }
            else
            {
                // Display message
                AlertDialog signatureDialog = new AlertDialog("THE PRINTER IS NOT READY");
                signatureDialog.ShowDialog();
            }
        }
    }
}
