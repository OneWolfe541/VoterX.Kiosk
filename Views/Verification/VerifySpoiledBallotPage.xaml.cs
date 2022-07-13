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
using VoterX.Utilities.Methods;
using VoterX.Utilities.Dialogs;
using VoterX.Core.Voters;
using VoterX.SystemSettings.Enums;

namespace VoterX.Kiosk.Views.Verification
{
    /// <summary>
    /// Interaction logic for VerifySpoiledBallotPage.xaml
    /// </summary>
    public partial class VerifySpoiledBallotPage : Page
    {
        private VoterSearchModel search = new VoterSearchModel();
        private NMVoter _voter = new NMVoter();

        public VerifySpoiledBallotPage(VoterNavModel voterFromNav)
        {
            InitializeComponent();

            search = voterFromNav.Search;

            _voter = voterFromNav.Voter;

            LoadVoterFields(voterFromNav.Voter.Data);

            LoadPreviouslyVotedFields(voterFromNav.Voter.Data);

            StatusBar.PageHeader = "Voter Verification";

            StatusBar.Clear();

            StatusBar.TextLeft = "Voter ID: " + voterFromNav.Voter.Data.VoterID;

            // Show Reprint application button if Early Voting
            CheckApplicationMode();
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
            if (voter.IDRequired == true && !voter.HasVoted())
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

        private void LoadPreviouslyVotedFields(VoterDataModel voter)
        {
            PreviousSite.Text = voter.PollName;
            PreviousDate.Text = voter.ActivityDate.ToString();
            PreviousComputer.Text = voter.ComputerID.ToString();
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
                    if (IDCorrect.IsChecked == false) result = false;
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

        private void SpoilButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new Ballots.SpoiledBallotPage(_voter));
        }

        private async void ReprintApplication_Click(object sender, RoutedEventArgs e)
        {
            // Prevent Spam Clicking this button
            ReprintApplication.IsEnabled = false;

            // Print a new application
            await Task.Run(() => BallotPrinting.ReprintApplication(_voter.Data, AppSettings.Global));

            // Go to application print verification page
        }

        private void CheckApplicationMode()
        {
            if (AppSettings.System.VCCType == VotingCenterMode.EarlyVoting)
            {
                ReprintApplication.Visibility = Visibility.Visible;
            }
            else if(AppSettings.System.VCCType == VotingCenterMode.ElectionDay && AppSettings.System.Permit == 1)
            {
                PermitApplication.Visibility = Visibility.Visible;
            }
                
        }

        private async void PermitApplication_Click(object sender, RoutedEventArgs e)
        {
            // Prevent Spam Clicking this button
            PermitApplication.IsEnabled = false;

            // Print a new application
            await Task.Run(() => BallotPrinting.ReprintPermit(_voter.Data, AppSettings.Global));

            // Go to permit print verification page
        }

        
    }
}
