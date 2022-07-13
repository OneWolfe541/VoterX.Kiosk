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
//using VoterX.Core.Models.Database;
using VoterX.Utilities.Methods;
using VoterX.Core.Voters;
using VoterX.Core.Elections;
using VoterX.SystemSettings.Enums;

namespace VoterX.Kiosk.Views.Ballots
{
    /// <summary>
    /// Interaction logic for SpoiledBallotPage.xaml
    /// </summary>
    public partial class SpoiledBallotPage : Page
    {
        NMVoter _Voter;

        private bool exitLogout = false;

        public SpoiledBallotPage(NMVoter voter)
        {
            InitializeComponent();

            _Voter = voter;

            LoadVoterFields(voter);

            LoadSpoiledReasons();

            StatusBar.PageHeader = "Spoil Ballot";

            StatusBar.Clear();
        }

        public SpoiledBallotPage(NMVoter voter, bool returnStatus)
        {
            InitializeComponent();

            _Voter = voter;

            exitLogout = returnStatus;

            LoadVoterFields(voter);

            LoadSpoiledReasons();

            StatusBar.PageHeader = "Spoil Ballot";

            StatusBar.Clear();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (exitLogout == true)
            {
                // Open Admin Menu
                MainMenuMethods.OpenMenu();

                this.NavigateToPage(new Admin.AdministrationPage());

                //this.NavigateToPage(new EditBallotSearchPage(null));
            }
            else
            {
                this.NavigateToPage(new Voter.SearchPage(null));
            }            
        }

        private void LoadVoterFields(NMVoter voter)
        {
            FullName.Text = voter.Data.FullName;
            BirthYear.Text = voter.Data.DOBYear;
            Address.Text = voter.Data.Address1;
            CityStateAndZip.Text = voter.Data.City + ", " + voter.Data.State + " " + voter.Data.Zip;
        }

        private async void LoadSpoiledReasons()
        {
            // Check DB connection
            if (await Task.Run(() => ElectionDataMethods.Exists) == true)
            {
                var spoiledList = ElectionDataMethods.SpoiledReasons
                    .Where(reason => reason.SpoiledReasonId != 9); // Do not display "Voter Rejected Ballot"

                SpoiledReason.ItemsSource = spoiledList;
            }
            else
            {
                StatusBar.TextCenter = "Database not found";
            }
        }

        private async void LoadFilteredSpoiledReasons()
        {
            if (await Task.Run(() => ElectionDataMethods.Exists) == true)
            {
                var spoiledList = ElectionDataMethods.SpoiledReasons
                    .Where(reason => reason.SpoiledReasonId == 3 || reason.SpoiledReasonId == 4)
                    .Where(reason => reason.SpoiledReasonId != 9); // Do not display "Voter Rejected Ballot"

                SpoiledReason.ItemsSource = spoiledList;
            }
            else
            {
                StatusBar.TextCenter = "Database not found";
            }
        }

        private SpoiledReasonModel GetSelectedItem(ComboBox sender)
        {
            return ((SpoiledReasonModel)sender.SelectedItem);
        }

        private void SpoilButton_Click(object sender, RoutedEventArgs e)
        {
            // Create Spoiled Record
            // Stores old ballot number in spoiled record
            VoterManagmentMethods.InsertSpoiledBallot(
                _Voter,
                GetSelectedItem(SpoiledReason).SpoiledReasonId,
                AppSettings.Global);

            // Update Ballot Number
            // Get new ballot number
            //_Voter.BallotNumber = VoterMethods.Voter.GetNextBallotNumber((int)AppSettings.System.SiteID);
            // Store new ballot number in voted record
            VoterManagmentMethods.UpdateVoterBallotNumber(_Voter, AppSettings.Global);

            // Reprint ballot and/or permit
            BallotPrinting.ReprintBallot(_Voter.Data, AppSettings.Global);

            // Reprint Permit on Election Day
            if (AppSettings.System.VCCType == VotingCenterMode.ElectionDay)
            {
                BallotPrinting.ReprintPermitSpoiled(_Voter.Data, AppSettings.Global);
            }

            // Goto the print varification page
            if (exitLogout == true)
            {
                this.NavigateToPage(new Views.Troubleshooting.SpoiledVerifyTroubleshootPage(_Voter, true));
            }
            else
            {
                this.NavigateToPage(new Views.Troubleshooting.SpoiledVerifyTroubleshootPage(_Voter, GetSelectedItem(SpoiledReason).SpoiledReasonId));
            }
        }

        private void SurrenderYes_Click(object sender, RoutedEventArgs e)
        {
            var item = GetSelectedItem(SpoiledReason);

            // Set Surrendered Ballot Status
            _Voter.Data.BallotSurrendered = true;

            // Cannot uncheck this button
            SurrenderYes.IsChecked = true;
            SurrenderYes.IsEnabled = false;

            // Turn on the Check Icon
            ballot_fa_check_yes.Visibility = Visibility.Visible;

            SurrenderNo.IsEnabled = true;

            // Uncheck the other button
            SurrenderNo.IsChecked = false;
            // Turn of Check Icon on the other button
            ballot_fa_check_no.Visibility = Visibility.Hidden;

            if (item.SpoiledReasonId == 3 || item.SpoiledReasonId == 4)
            {
                SpoilWrongOrFledBallot.Visibility = Visibility.Visible;
                SpoilBallot.Visibility = Visibility.Collapsed;
            }
            else
            {
                SpoilWrongOrFledBallot.Visibility = Visibility.Collapsed;
                SpoilBallot.Visibility = Visibility.Visible;
            }
        }

        private void SurrenderNo_Click(object sender, RoutedEventArgs e)
        {
            var item = GetSelectedItem(SpoiledReason);

            // Set Surrendered Ballot Status
            _Voter.Data.BallotSurrendered = false;

            // Cannot uncheck this button
            SurrenderNo.IsChecked = true;
            SurrenderNo.IsEnabled = false;

            // Turn on the Check Icon
            ballot_fa_check_no.Visibility = Visibility.Visible;

            SurrenderYes.IsEnabled = true;

            // Uncheck the other button
            SurrenderYes.IsChecked = false;
            // Turn of Check Icon on the other button
            ballot_fa_check_yes.Visibility = Visibility.Hidden;

            if (item.SpoiledReasonId == 3 || item.SpoiledReasonId == 4)
            {
                // Show Wrong or Fled Button
                SpoilWrongOrFledBallot.Visibility = Visibility.Visible;
                SpoilBallot.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Hide Wrong or Fled button
                SpoilWrongOrFledBallot.Visibility = Visibility.Collapsed;
                SpoilBallot.Visibility = Visibility.Visible;
            }
        }

        private void SpoiledReason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = GetSelectedItem((ComboBox)sender);

            ResetSpoiledQuestion();

            if (item != null)
            {
                // Check if surrendered ballot boxes are checked
                //if (item.spoiled_reason_id == 1)
                //{
                //    // Show Spoiled Button
                //    SpoilBallot.Visibility = Visibility.Visible;
                //    // Hide fled wrong button
                //    SpoilWrongOrFledBallot.Visibility = Visibility.Collapsed;
                //    // Hide surrender question
                //    SurrenderBallotVerificationPanel.Visibility = Visibility.Visible;
                //}
                if (item.SpoiledReasonId >= 1)
                {
                    // Show surrender question
                    SurrenderBallotVerificationPanel.Visibility = Visibility.Visible;

                    // When Wrong or Fled options are checked
                    if (item.SpoiledReasonId == 3 || item.SpoiledReasonId == 4)
                    {
                        // Display the Wrong or Fled button if a surrender option has already been clicked
                        if (SurrenderYes.IsChecked == true || SurrenderNo.IsChecked == true)
                        {
                            SpoilWrongOrFledBallot.Visibility = Visibility.Visible;
                            SpoilBallot.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            SpoilWrongOrFledBallot.Visibility = Visibility.Collapsed;
                            SpoilBallot.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        // Display the normal Spoiled button if a surrender option has already been clicked
                        if (SurrenderYes.IsChecked == true || SurrenderNo.IsChecked == true)
                        {
                            SpoilWrongOrFledBallot.Visibility = Visibility.Collapsed;
                            SpoilBallot.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            SpoilWrongOrFledBallot.Visibility = Visibility.Collapsed;
                            SpoilBallot.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        private void ResetSpoiledQuestion()
        {
            _Voter.Data.BallotSurrendered = false;

            SurrenderYes.IsChecked = false;
            SurrenderYes.IsEnabled = true;
            ballot_fa_check_yes.Visibility = Visibility.Collapsed;
            SurrenderNo.IsChecked = false;
            SurrenderNo.IsEnabled = true;
            ballot_fa_check_no.Visibility = Visibility.Collapsed;
        }

        private void SpoilWrongOrFledBallot_Click(object sender, RoutedEventArgs e)
        {
            int reasonID = GetSelectedItem(SpoiledReason).SpoiledReasonId;

            // Create Spoiled Record
            VoterManagmentMethods.InsertSpoiledBallot(
                _Voter,
                GetSelectedItem(SpoiledReason).SpoiledReasonId,
                AppSettings.Global);

            if (reasonID == 3)
            {
                VoterManagmentMethods.MarkAsFledVoter(_Voter);
            }
            if (reasonID == 4)
            {
                VoterManagmentMethods.MarkAsWrongVoter(_Voter);
            }

            this.NavigateToPage(new Voter.SearchPage(null));
        }
    }
}
