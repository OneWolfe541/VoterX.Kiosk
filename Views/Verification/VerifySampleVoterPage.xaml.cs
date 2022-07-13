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
    public partial class VerifySampleVoterPage : Page
    {
        private VoterSearchModel search = new VoterSearchModel();
        private NMVoter _voter = new NMVoter();

        public VerifySampleVoterPage(VoterNavModel voterFromNav)
        {
            InitializeComponent();

            search = voterFromNav.Search;

            _voter = voterFromNav.Voter;

            LoadVoterFields(voterFromNav.Voter);

            StatusBar.PageHeader = "Voter Verification";

            StatusBar.Clear();

            StatusBar.TextLeft = "Voter ID: " + voterFromNav.Voter.Data.VoterID;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new SearchPage(search));
        }

        private void LoadVoterFields(NMVoter voter)
        {
            //VoterID.Text = voter.VoterID;
            FullName.Text = voter.Data.FullName;
            BirthYear.Text = voter.Data.DOBYear;
            Address.Text = voter.Data.Address1;
            CityStateAndZip.Text = voter.Data.City + ", " + voter.Data.State + " " + voter.Data.Zip;
            BallotStyle.Text = voter.Data.BallotStyle;

            // Set a flag when voter ID is required 
            // Then turn on the ID varification control group
            // And turn off the other groups
            IDVarification.DataContext = voter.Data.IDRequired;
            if (voter.Data.IDRequired == true && !voter.HasVoted())
            {
                IDVarification.Visibility = Visibility.Collapsed;
                CheckNameGrid.Visibility = Visibility.Visible;
                CheckDateGrid.Visibility = Visibility.Visible;
                CheckAddressGrid.Visibility = Visibility.Visible;
            }
            else
            {
                IDVarification.Visibility = Visibility.Collapsed;
            }
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

            if (AllValidationBoxesChecked()) BallotFunctions.Visibility = Visibility.Visible;
            else BallotFunctions.Visibility = Visibility.Collapsed;

            if ((bool)IDVarification.DataContext == true)
            {
                if (
                    //IDRequiredCheckQuestion.GetAnswer() == false
                    NameCorrect.IsChecked == true
                    && DateCorrect.IsChecked == true
                    && AddressCorrect.IsChecked == true
                    )
                {
                    BallotFunctions.Visibility = Visibility.Visible;
                    //Signature.Visibility = Visibility.Collapsed;
                    //ProvisionalBallot.Visibility = Visibility.Visible;
                    //SampleBallots.Visibility = Visibility.Visible;
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
            //if (!_voter.HasVoted())
            //{
            //    // Voter has not already voted

            //    // Check if voter ID is required                
            //    if ((bool)IDVarification.DataContext == true)
            //    {
            //        // When ID required also check name date and address
            //        //if (IDRequiredCheckQuestion.GetAnswer() == false) result = false;
            //        if (NameCorrect.IsChecked == false) result = false;
            //        if (DateCorrect.IsChecked == false) result = false;
            //        if (AddressCorrect.IsChecked == false) result = false;
            //    }
            //    else
            //    {
            //        // If ID is not required only check name date and address
            //        if (NameCorrect.IsChecked == false) result = false;
            //        if (DateCorrect.IsChecked == false) result = false;
            //        if (AddressCorrect.IsChecked == false) result = false;
            //    }
            //}
            //else
            //{
            //    // Voter has already voted

            //    // When they have already voted dont need to check their ID a second time

            //    if (NameCorrect.IsChecked == false) result = false;
            //    if (DateCorrect.IsChecked == false) result = false;
            //    if (AddressCorrect.IsChecked == false) result = false;
            //}

            // Return the final state
            return result;
        }

        private void Signature_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new SignatureCapturePage(_voter));
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

        }

        private void PrintSignatureForm_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IDRequiredCheckQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {

        }

        private async void SampleBallots_Click(object sender, RoutedEventArgs e)
        {
            if (await PrinterStatus.PrinterIsReadyAsync(AppSettings.Printers.SamplePrinter) == true)
            {
                StatusBar.TextLeft = await Task.Run(() => BallotPrinting.PrintSampleBallot(AppSettings.Global, _voter.Data.BallotStyleFile));

                this.NavigateToPage(new Troubleshooting.SampleVerifyTroubleshootPage(_voter));
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
