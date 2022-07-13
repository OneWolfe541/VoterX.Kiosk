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
using VoterX.Utilities.Extensions;
using VoterX.Kiosk.Views.Voter;
using VoterX.Kiosk.Methods;
using VoterX.Core.Extensions;
using VoterX.Utilities.Dialogs;
using VoterX.Core.Voters;

namespace VoterX.Kiosk.Views.Verification
{
    /// <summary>
    /// Interaction logic for VerifyProvisionalBallotPage.xaml
    /// </summary>
    public partial class VerifyDeletedVoterPage : Page
    {
        private VoterSearchModel search = new VoterSearchModel();
        private NMVoter _voter = new NMVoter();

        public VerifyDeletedVoterPage(VoterNavModel voterFromNav)
        {
            InitializeComponent();

            search = voterFromNav.Search;

            _voter = voterFromNav.Voter;

            LoadVoterFields(voterFromNav.Voter);

            LoadPreviouslyVotedFields(voterFromNav.Voter);

            HideValidationBox();

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

            // Set a flag when voter ID is required 
            // Then turn on the ID varification control group
            // And turn off the other groups
            IDVarification.DataContext = voter.Data.IDRequired;
            if (voter.Data.IDRequired == true && !voter.HasVoted())
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
        }

        private void LoadPreviouslyVotedFields(NMVoter voter)
        {
            PreviousSite.Text = voter.Data.PollName;
            PreviousDate.Text = voter.Data.ActivityDate.ToString();
            PreviousComputer.Text = voter.Data.ComputerID.ToString();
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
                " | ID: ", IDCorrect.IsChecked);
            //StatusBar.StatusTextLeft = StatusMessage;

            if (AllValidationBoxesChecked()) BallotFunctions.Visibility = Visibility.Visible;
            else BallotFunctions.Visibility = Visibility.Collapsed;
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

                // Check if voter ID is required                
                if ((bool)IDVarification.DataContext == true)
                {
                    // When ID required also check name date and address
                    //if (IDCorrect.IsChecked == false) result = false;
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

        private void HideValidationBox()
        {
            //VoterVerificationBorder.Visibility = Visibility.Collapsed;
            //VoterVerificationText.Visibility = Visibility.Collapsed;
            CheckVoterInfoMessage.Visibility = Visibility.Collapsed;
            IDVarification.Visibility = Visibility.Collapsed;
            CheckNameGrid.Visibility = Visibility.Collapsed;
            CheckDateGrid.Visibility = Visibility.Collapsed;
            CheckAddressGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowValidationBox()
        {
            //VoterVerificationBorder.Visibility = Visibility.Visible;
            //VoterVerificationText.Visibility = Visibility.Visible;
            CheckVoterInfoMessage.Visibility = Visibility.Visible;
            //IDVarification.Visibility = Visibility.Collapsed;
            CheckNameGrid.Visibility = Visibility.Visible;
            CheckDateGrid.Visibility = Visibility.Visible;
            CheckAddressGrid.Visibility = Visibility.Visible;
        }

        private void StartProvisionalButton_Click(object sender, RoutedEventArgs e)
        {
            ValidationDialog passwordDialog = new ValidationDialog(AppSettings.User, "Manager");
            if (passwordDialog.ShowDialog() == true)
            {
                //StatusBar.StatusTextLeft = "Password Correct";
                // show varification box
                ShowValidationBox();
                // hide button
                StartProvisionalButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Display error message
                AlertDialog wrongPassword = new AlertDialog("THE WRONG PASSWORD WAS ENTERED");
                wrongPassword.ShowDialog();
            }
        }

        private void ProvisionalButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new Ballots.ProvisionalBallotPage(_voter));
        }
    }
}
