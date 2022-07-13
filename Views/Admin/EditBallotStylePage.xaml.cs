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
using VoterX.Kiosk.Methods;
using VoterX.Utilities.Extensions;
using VoterX.Utilities.Methods;
//using VoterX.Core.Models.Database;
//using VoterX.Core.Models.ViewModels;
using VoterX.Core.Extensions;
using VoterX.Core.Voters;
using VoterX.SystemSettings.Enums;

namespace VoterX.Kiosk.Views.Admin
{
    /// <summary>
    /// Interaction logic for EditBallotStylePage.xaml
    /// </summary>
    public partial class EditBallotStylePage : Page
    {
        private VoterNavModel _voterNav = new VoterNavModel();

        private VoterLookupStatus _voter_status = VoterLookupStatus.None;

        private bool _officialBallot = false;

        bool _loaded = false;

        public EditBallotStylePage(VoterNavModel voterFromNav, bool official)
        {
            InitializeComponent();

            _voterNav = voterFromNav;

            _officialBallot = official;

            //_voter_status = VoterValidationMethods.CheckVoterStatus(_voterNav.Voter, (int)AppSettings.System.SiteID);

            LoadVoterFields();

            LoadBallotStyles();

            LoadPrecinctPartsStyles();

            LoadParties();

            GetElectionType();

            _loaded = true;

            StatusBar.PageHeader = "Edit Ballot Style";

            CheckServer();
        }

        private async void CheckServer()
        {
            BallotStyleFunctions.Visibility = Visibility.Collapsed;

            if (await StatusBar.CheckServer(ElectionDataMethods.Election) == true)
            {
                BallotStyleFunctions.Visibility = Visibility.Visible;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new EditBallotSearchPage(_voterNav.Search));
        }

        private void GetElectionType()
        {
            if (AppSettings.Election.ElectionType == ElectionType.General)
            {
                PartyGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadVoterFields()
        {
            // Default voter
            //_voter = await Task.Run(() => VoterMethods.Voter.Single("45769").FirstOrDefault() );

            //VoterID.Text = voter.VoterID;
            FullName.Text = _voterNav.Voter.Data.FullName;
            BirthYear.Text = _voterNav.Voter.Data.DOBYear;
            Address.Text = _voterNav.Voter.Data.Address1;
            CityStateAndZip.Text = _voterNav.Voter.Data.City + ", " + _voterNav.Voter.Data.State + " " + _voterNav.Voter.Data.Zip;
            BallotStyle.Text = _voterNav.Voter.Data.BallotStyle;
        }

        private async void LoadBallotStyles()
        {
            // Create animated loading list item
            var loadingItem = ComboBoxMethods.AddLoadingItem(BallotStylesList, TempLoadingSpinnerItem);

            //if (await Task.Run(() => VoterMethods.Exists) == true)
            //{
            //    var ballotstylelist = await Task.Run(() => VoterMethods.BallotStyleMaster.List());
            //    foreach (var ballotstyle in ballotstylelist)
            //    {
            //        ComboBoxMethods.AddComboItemToControl(
            //            BallotStylesList,
            //            ballotstyle.ballot_style_id,
            //            ballotstyle.ballot_style_name,
            //            -1
            //            );
            //    }
            //}
            //else
            //{
            //    StatusBar.StatusTextCenter = "Database not found";
            //}

            // Remove animated loading list item
            ComboBoxMethods.RemoveListItem(BallotStylesList, loadingItem);

            BallotStylesList.SelectedIndex = -1;
        }

        private async void LoadPrecinctPartsStyles()
        {
            // Create animated loading list item
            var loadingItem = ComboBoxMethods.AddLoadingItem(PrecinctPartsList, TempLoadingSpinnerItem);

            //if (await Task.Run(() => VoterMethods.Exists) == true)
            //{
            //    IEnumerable<tblPrecinctsMaster> PrecinctPartList = await PrecinctMethods.GetPrecinctsListAsync(VoterMethods.Container);
            //    // Sort list numericly instead of alphabeticly            
            //    foreach (var precinctPart in PrecinctPartList.OrderBy(x => Double.Parse(x.precinct_part)))
            //    {
            //        ComboBoxMethods.AddComboItemToControl(
            //            PrecinctPartsList,
            //            precinctPart.precinct_part_id,
            //            precinctPart.precinct_part.ToString(),
            //            "001.001"
            //            );
            //    }
            //}
            //else
            //{
            //    StatusBar.StatusTextCenter = "Database not found";
            //}

            // Remove animated loading list item
            ComboBoxMethods.RemoveListItem(PrecinctPartsList, loadingItem);

            PrecinctPartsList.SelectedIndex = -1;
        }

        private void LoadParties()
        {
            //foreach (var party in PartyMethods.GetPrimaryPartyList(VoterMethods.Container))
            //{
            //    ComboBoxMethods.AddComboItemToControl(
            //        PartyList,
            //        party,
            //        party,
            //        "DEM"
            //        );
            //}

            PartyList.SelectedIndex = -1;
        }

        private int GetVoterBallotId(VoterDataModel voter)
        {
            if (voter.BallotStyleID == null) return 0;
            else return (int)voter.BallotStyleID;
        }

        private void SaveBallotStyle_Click(object sender, RoutedEventArgs e)
        {
            // Dont know if we are going to save the ballot changes to the database or the local record
            // Or if the changes only go into the avVotedRecord table            

            //BallotStyle.Text = GetSelectedItemNumber(BallotStylesList).ToString();
            //BallotStyle.Text += ", " + GetSelectedItem(BallotStylesList);

            int ballotstyleid = GetSelectedItemNumber(BallotStylesList);

            //var newballotstyle = VoterMethods.BallotStyleMaster.GetByID(ballotstyleid);

            //_voterNav.Voter.BallotStyleID = newballotstyle.ballot_style_id;
            //_voterNav.Voter.BallotStyle = newballotstyle.ballot_style_name;
            //_voterNav.Voter.BallotStyleFile = newballotstyle.ballot_style_filename;

            //BallotStyle.Text = _voterNav.Voter.BallotStyleID.ToString() + ", " + _voterNav.Voter.BallotStyle + ", " + _voterNav.Voter.BallotStyleFile;
        }

        private int GetSelectedItemNumber(ComboBox sender)
        {
            if (sender.SelectedItem == null) return 0;
            else
                return (int)((ComboBoxItem)sender.SelectedItem).DataContext;
        }

        private string GetSelectedItem(ComboBox sender)
        {
            if (sender.SelectedItem == null) return "";
            else
                return ((ComboBoxItem)sender.SelectedItem).Content.ToString();
        }

        private void PrecinctPartsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PrecinctPartsList.IsLoaded)
            {
                GetNewBallotStyle(sender, e);
                TurnOnCorrectButton();
            }
        }

        private void PartyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PartyList.IsLoaded)
            {
                GetNewBallotStyle(sender, e);
                TurnOnCorrectButton();
            }
        }

        private async void GetNewBallotStyle(object sender, SelectionChangedEventArgs e)
        {
            // Convert sender to a ComboBox
            var comboBox = (ComboBox)sender;
            // Check if the combobox is ready
            if (comboBox.IsLoaded)
            {
                //if (await Task.Run(() => VoterMethods.Exists) == true)
                //{
                //    // Get the precint part Id
                //    string precinctPart = ComboBoxMethods.GetSelectedItemData(PrecinctPartsList).ToString();
                //    // Get the party name
                //    string party = ComboBoxMethods.GetSelectedItem(PartyList);
                //    // Check if either precinct part id or party name are blank
                //    if (!precinctPart.IsNullOrEmpty() && (!party.IsNullOrEmpty() || AppSettings.Election.ElectionType == 2))
                //    {
                //        // Get the Ballot Style Lookup query
                //        var ballotStyleQuery = AppSettings.Election.ElectionType == 2 ?
                //            // General Election Type is not Party dependant
                //            BallotStyleMethods.GetBallotStyleLookup(precinctPart, VoterMethods.Container)
                //            :
                //            // Primary Election Type is party dependant
                //            BallotStyleMethods.GetBallotStyleLookup(precinctPart, party, VoterMethods.Container);
                //        // Check of query returns 0 records
                //        if (ballotStyleQuery.Count() != 0)
                //        {
                //            // Collect all ballot styles asynchronously
                //            string ballotStyle = await Task.Run(() => ballotStyleQuery.FirstOrDefault().BallotStyleName);
                //            // Set Ballot Style list and display results
                //            string message = SetBallotStyleSelectedItem(BallotStylesList, ballotStyle);
                //            StatusBar.StatusTextLeft = message;
                //            //ComboBoxMethods.SetSelectedItem(BallotStylesList, ballotStyle);
                //        }
                //        else
                //        {
                //            // Set Ballot Style list to blank
                //            BallotStylesList.SelectedIndex = -1;
                //            // Display failure message
                //            StatusBar.StatusTextLeft = "No Ballot Style Found";
                //        }
                //    }
                //} 
                //else
                //{
                //    StatusBar.StatusTextCenter = "Database not found";
                //}
            }
        }

        private string SetBallotStyleSelectedItem(ComboBox sender, string ballotStyle)
        {
            // Create blank message
            string message = "";

            // Check if ballot style is empty
            if (!ballotStyle.IsNullOrEmpty())
            {
                // Check if the destination list is empty
                if (sender.Items.Count > 0)
                {
                    // Loop through list items
                    foreach (ComboBoxItem item in sender.Items)
                    {
                        // Compare list item to the string provided
                        if (item.Content.ToString() == ballotStyle)
                        {
                            // Set the destination selected item
                            sender.SelectedIndex = sender.Items.IndexOf(item);
                            // Set message to location of item found
                            message = (item.Content.ToString() + " Found At: " + sender.Items.IndexOf(item).ToString());
                            // Break out of the loop
                            return message;
                        }
                    }

                    // If the item was not found in the list message remains in default state
                    if (message == "")
                    {
                        // Set failure message
                        message = ("No Ballot Style Found");
                    }
                }
                else
                {
                    // Set failure message
                    message = ("No Items In List");
                }
            }
            else
            {
                // Set failure message
                message = ("No Ballot Style Found");
            }

            return message;
        }

        private void TurnOnCorrectButton()
        {
            // Hide the buttons
            HideAllButtons();

            StatusBar.TextLeft = " [" + BallotStylesList.SelectedIndex.ToString() + "] ";

            var test = BallotStylesList.SelectedItem;
            var spin = TempLoadingSpinnerItem;

            // Only show buttons if a ballot style is selected
            if (BallotStylesList.SelectedIndex >= 0 && _loaded == true)
            {
                // Choose correct button to display
                switch (_voter_status)
                {
                    case VoterLookupStatus.Eligible:
                    case VoterLookupStatus.Ineligible:
                        if (_officialBallot == true)
                        {
                            // Turn on offical button
                            OfficialBallotButton.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            // Turn on provisional button
                            ProvisionalBallotButton.Visibility = Visibility.Visible;
                        }
                        break;
                    case VoterLookupStatus.Spoilable:
                        // Turn on spoil button
                        SpoilBallotButton.Visibility = Visibility.Visible;
                        break;
                    case VoterLookupStatus.Provisional:
                        // Turn on provisional button
                        ProvisionalBallotButton.Visibility = Visibility.Visible;
                        break;
                    default:

                        break;
                }
            }
        }

        private void HideAllButtons()
        {
            //SaveBallotStyle.Visibility = Visibility.Collapsed;
            ProvisionalBallotButton.Visibility = Visibility.Collapsed;
            OfficialBallotButton.Visibility = Visibility.Collapsed;
            SpoilBallotButton.Visibility = Visibility.Collapsed;
        }

        private void OfficialBallotCheck_Click(object sender, RoutedEventArgs e)
        {
            TurnOnCorrectButton();
        }

        private void BallotStylesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TurnOnCorrectButton();
        }

        private async void ProvisionalBallotButton_Click(object sender, RoutedEventArgs e)
        {
            //if (await Task.Run(() => VoterMethods.Exists) == true)
            //{
            //    // Save to quarantine 
            //    VoterManagmentMethods.InsertQuarantine(
            //    _voterNav.Voter,
            //    "Ballot Style Changed as Provisional",
            //    VoterMethods.Container,
            //    AppSettings.Global);

            //    // Goto provisional page
            //    this.NavigateToPage(new Ballots.ProvisionalBallotPage(_voterNav.Voter, true));
            //}
            //else
            //{
            //    StatusBar.StatusTextCenter = "Database not found";
            //}
        }

        private async void OfficialBallotButton_Click(object sender, RoutedEventArgs e)
        {
            //if (await Task.Run(() => VoterMethods.Exists) == true)
            //{
            //    // Save to quarantine
            //    VoterManagmentMethods.InsertQuarantine(
            //    _voterNav.Voter
            //    , "Ballot Style Changed as Official",
            //    VoterMethods.Container,
            //    AppSettings.Global);

            //    // Goto official page
            //    this.NavigateToPage(new Signature.SignatureCapturePage(_voterNav.Voter, true));
            //}
            //else
            //{
            //    StatusBar.StatusTextCenter = "Database not found";
            //}
        }

        private async void SpoilBallotButton_Click(object sender, RoutedEventArgs e)
        {
            //if (await Task.Run(() => VoterMethods.Exists) == true)
            //{
            //    // Save to quarantine
            //    VoterManagmentMethods.InsertQuarantine(
            //    _voterNav.Voter,
            //    "Ballot Style Changed as Spoiled",
            //    VoterMethods.Container,
            //    AppSettings.Global);

            //    // Goto spoiled page
            //    this.NavigateToPage(new Ballots.SpoiledBallotPage(_voterNav.Voter, true));
            //}
            //else
            //{
            //    StatusBar.StatusTextCenter = "Database not found";
            //}
        }
    }
}
