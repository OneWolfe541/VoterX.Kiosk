﻿using System;
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
//using VoterX.Core.Models.ViewModels;
using VoterX.Kiosk.Methods;
//using VoterX.Core.Models.Utilities;
using VoterX.Utilities.Methods;
using VoterX.Utilities.Extensions;
using VoterX.Core.Extensions;
using VoterX.Utilities.Dialogs;
using VoterX.Core.Voters;

namespace VoterX.Kiosk.Views.Admin
{
    /// <summary>
    /// Interaction logic for EditBallotSearchPage.xaml
    /// </summary>
    public partial class EditBallotSearchPage : Page
    {
        public EditBallotSearchPage()
        {
            InitializeComponent();

            StatusBar.PageHeader = "Edit Ballot Style Search";

            StatusBar.Clear();

            CheckServer();
        }

        public EditBallotSearchPage(VoterSearchModel searchItems)
        {
            InitializeComponent();

            // Reset search boxes to previous values
            if (searchItems != null)
            {
                SetSearchBoxes(searchItems);
            }
            else
            {
                ClearSearchBoxes();
            }

            StatusBar.PageHeader = "Edit Ballot Style Search";

            StatusBar.CheckPrinter(AppSettings.Printers);
            //StatusBar.CheckSignaturePad();

            StatusBar.Clear();

            CheckServer();
        }

        private async void CheckServer()
        {
            SearchGrid.Visibility = Visibility.Collapsed;
            VoterList.StartLoadAnimation();

            if (await StatusBar.CheckServer(ElectionDataMethods.Election) == true)
            {
                SearchGrid.Visibility = Visibility.Visible;
            }

            VoterList.EndLoadAnimation();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenuMethods.OpenMenu();
            this.NavigateToPage(new Admin.AdministrationPage());
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new Login.LoginPage());
        }

        // Scroll the list items
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Notes on scroll offsets
            // https://stackoverflow.com/questions/1033841/is-it-possible-to-implement-smooth-scroll-in-a-wpf-listview
            // https://social.msdn.microsoft.com/Forums/en-US/3594c80a-7ccf-4cfc-9cc0-9731fd080d72/in-what-unit-is-the-scrollviewerverticaloffset?forum=winappswithcsharp

            //double delta = (e.Delta * .26978); // Roughly half of 1 list item
            double delta = (e.Delta * .36);
            //double delta = (e.Delta / 120)*32; // Reduce to +1 or -1 then multiply to get exact units
            //StatusBar.ApplicationStatusCenter("Scrolling:" + (delta).ToString());

            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - (delta));
            e.Handled = true;
        }

        // Scroll the list items from anywhere on the Page
        private void Page_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Mouse Wheel Status
            //StatusBar.ApplicationStatusCenter("Mouse Wheel Move: " + e.Delta);

            //ScrollViewer_PreviewMouseWheel(SearchScrollViewer, e);

            VoterList.ScrollList(sender, e);
        }

        // Run the voter search for the search box values
        // Running the search asynchronously does two things
        //// First it allows the user to perform other tasks while the search is running
        //// Second it allows the Status bar message to display that the application is currently working 
        //// ~ without having to wait for the search to finish
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            //// Check if there is anything to search
            if (SearchIsNotNullOrEmpty())
            {
                // Create the Voter Search Object (Search Parameters)
                VoterSearchModel searchItems = new VoterSearchModel
                {
                    FirstName = FirstNameSearch.Text,
                    LastName = LastNameSearch.Text,
                    BirthYear = BirthYearSearch.Text
                };

                SearchVoterList(searchItems);
            }
            else
            {
                StatusBar.TextLeft = "No Search Parameters Entered";
                VoterList.DisplayResults("Please enter valid search criteria.");
            }
        }

        // Returns true if any of the search boxes have a value
        private bool SearchIsNotNullOrEmpty()
        {
            return
                !FirstNameSearch.Text.IsNullOrEmpty() ||
                !LastNameSearch.Text.IsNullOrEmpty() ||
                !BirthYearSearch.Text.IsNullOrEmpty();
        }

        // Clear both the search boxes and the list view
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear Search Boxes
            //FirstNameSearch.Text = null;
            //LastNameSearch.Text = null;
            //BirthYearSearch.Text = null;
            ClearSearchBoxes();
            // Clear ListView
            //VoterList.ItemsSource = null;
            ClearListView();

            // Clear results message
            //SearchResultsText.Text = "";
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSearchBoxes();
            ClearListView();

            SearchGrid.Visibility = Visibility.Collapsed;

            ScanGrid.Visibility = Visibility.Visible;

            Keyboard.Focus(BarCodeSearch);
        }

        private void SearchAltButton_Click(object sender, RoutedEventArgs e)
        {
            SearchGrid.Visibility = Visibility.Visible;

            ScanGrid.Visibility = Visibility.Collapsed;

            Keyboard.Focus(LastNameSearch);
        }

        // Load the search boxes from the search items model
        private void SetSearchBoxes(VoterSearchModel searchItems)
        {
            FirstNameSearch.Text = searchItems.FirstName;
            LastNameSearch.Text = searchItems.LastName;
            BirthYearSearch.Text = searchItems.BirthYear;
        }

        // Erase all values from search boxes
        // This only needs to be done when I have default values set
        private void ClearSearchBoxes()
        {
            FirstNameSearch.Text = null;
            LastNameSearch.Text = null;
            BirthYearSearch.Text = null;

            BarCodeSearch.Text = null;
            LastBarCode.Text = null;
        }

        // Clear the list view object
        private void ClearListView()
        {
            VoterList.ClearList();
        }

        private void BarCodeSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                // Create the Voter Search Object (Search Parameters)
                VoterSearchModel searchItems = new VoterSearchModel
                {
                    VoterID = BarCodeSearch.Text
                };
                //StatusBar.ApplicationStatusCenter(BarCodeSearch.Text);
                //SearchSingleVoter(BarCodeSearch.Text);
                SearchVoterList(searchItems);

                Keyboard.Focus(BarCodeSearch);

                LastBarCode.Text = BarCodeSearch.Text;

                BarCodeSearch.Text = "";
            }
        }

        private async void SearchVoterList(VoterSearchModel searchItems)
        {
            if (searchItems != null)
            {
                // Reset search result messages and
                // Clear the list box
                VoterList.ClearList();

                // Turn on the spining progress wheel
                VoterList.StartSearchAnimation();
                StatusBar.TextLeft = "Searching ";
                StatusBar.ShowLeftSpinner();

                if (await Task.Run(() => VoterMethods.Exists) == true)
                {

                    int count = 0;
                    // Get list of voters from Search Parameters
                    // Call voter lookup asynchronously so user can still perform tasks while the search is running
                    // #### Might want to move the Async to the static method definition ####
                    var list = await VoterMethods.Voters.PagedListAsync(searchItems, 1, 50);
                    string errorMessage = list.CheckForErrors();
                    if (errorMessage != null)
                    {
                        StatusBar.TextCenter = "SQL ERROR: " + errorMessage;
                    }
                    else
                    {
                        count = list.Count();
                        VoterList.SetVoterList(list.OrderByFullName());
                    }

                    // Display result count in the status bar
                    // #### We want to get the record count before reducing the return list ####
                    //StatusBar.ApplicationStatus(string.Concat("Results: ", count.ToString(), " voters found."));                
                    if (count < 1)
                    {
                        //SearchResults.Visibility = Visibility.Visible;
                        //SearchResults.Text = string.Concat("Results: ", count.ToString(), " voters found.");
                        string resultmessage = "Results: " + count.ToString() + " voters found.";
                        VoterList.DisplayResults(resultmessage);
                        StatusBar.TextLeft = resultmessage;
                    }
                    else if (count < 50)
                    {
                        //StatusBar.ApplicationStatus(string.Concat("Results: ", count.ToString(), " voters found."));
                        //SearchResultsText.Text = "Search Results:\r\n" + count.ToString() + " voters found.";
                        //VoterList.DisplayResults("Search Results:\r\n" + count.ToString() + " voters found.");
                    }
                    else
                    {
                        //StatusBar.ApplicationStatus(string.Concat("Results too large!"));
                        //SearchResultsText.Text = "Search Results:\r\nFirst 50 of " + count.ToString() + " voters shown.\r\n\r\n";
                        //SearchResultsText.Text += "Try refining the search by\r\n";
                        //SearchResultsText.Text += SuggestRefinements(searchItems);
                    }
                }
                else
                {
                    StatusBar.TextCenter = "Database not found";
                }

                // Turn off the progress spinner
                VoterList.EndSearchAnimation();
                StatusBar.TextLeft = "";
                StatusBar.HideLeftSpinner();
            }
        }

        //// Send selected voter and the search parameters to the next page
        //private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    var item = sender as ListViewItem;
        //    if (item != null)
        //    {
        //        string voterID = ((VoterDataModel)item.DataContext).VoterID;
        //        StatusBar.StatusTextLeft = "Voter ID: " + voterID;

        //        // Get the search parameters
        //        VoterSearchModel searchItems = new VoterSearchModel
        //        {
        //            FirstName = FirstNameSearch.Text,
        //            LastName = LastNameSearch.Text,
        //            BirthYear = BirthYearSearch.Text
        //        };

        //        // Merge the voter and the search parameters into a single object
        //        VoterNavModel voterNav = new VoterNavModel
        //        {
        //            // Get full voter details
        //            //Voter = await Task.Run(() => VoterMethods.Voter.Single(voterID, AppSettings.Election.ElectionType).FirstOrDefault()),
        //            Voter = VoterMethods.Voter.Single(voterID, AppSettings.Election.ElectionType).FirstOrDefault(),
        //            Search = searchItems
        //        };

        //        // Check where to send the voter next
        //        //RedirectVoter(voterNav);

        //        var _voter_status = VoterValidationMethods.CheckVoterStatus(voterNav.Voter, AppSettings.System.SiteID);

        //        if (
        //        _voter_status == VoterLookupStatus.Provisional ||
        //        _voter_status == VoterLookupStatus.Spoilable
        //        )
        //        {
        //            // Goto edit page
        //            //this.NavigateToPage(new Admin.EditBallotStylePage(voterNav, false));
        //        }
        //        else
        //        {
        //            // Goto official selection page
        //            //this.NavigateToPage(new Admin.EditBallotOptionPage(voterNav));
        //        }
        //    }
        //}

        //private void RedirectVoter(VoterNavModel voterNav)
        //{
        //    this.NavigateToPage(ChooseVoterDestination(voterNav));
        //}

        private async void VoterListControl_VoterClick(object sender, RoutedEventArgs e)
        {
            var selectedVoter = VoterList.SelectedVoter;
            StatusBar.TextLeft = "Voter Clicked: " + selectedVoter.Data.VoterID;

            this.Cursor = Cursors.Wait;
            StatusBar.TextLeft = "Loading ";
            StatusBar.ShowLeftSpinner();

            if (await Task.Run(() => VoterMethods.Exists) == true)
            {

                // Get the search parameters
                VoterSearchModel searchItems = new VoterSearchModel
                {
                    FirstName = FirstNameSearch.Text,
                    LastName = LastNameSearch.Text,
                    BirthYear = BirthYearSearch.Text
                };

                // Merge the voter and the search parameters into a single object
                VoterNavModel voterNav = new VoterNavModel
                {
                    // Get full voter details
                    Voter = selectedVoter,
                    //Voter = await Task.Run(() => VoterMethods.Voter.Single(selectedVoter.VoterID, AppSettings.Election.ElectionType).FirstOrDefault()),
                    //Voter = VoterMethods.Voter.Single(selectedVoter.VoterID, AppSettings.Election.ElectionType).FirstOrDefault(),
                    Search = searchItems
                };

                this.Cursor = Cursors.Arrow;
                StatusBar.TextLeft = "";
                StatusBar.HideLeftSpinner();

                // Check where to send the voter next
                //RedirectVoter(voterNav);

                //var _voter_status = VoterValidationMethods.CheckVoterStatus(voterNav.Voter, AppSettings.System.SiteID);

                //if (
                ////_voter_status == VoterLookupStatus.Ineligible || 
                //_voter_status == VoterLookupStatus.Provisional ||
                //_voter_status == VoterLookupStatus.Spoilable
                //)
                //{
                //    // Goto edit page
                //    this.NavigateToPage(new Admin.EditBallotStylePage(voterNav, false));
                //}
                //else if(_voter_status == VoterLookupStatus.Eligible)
                //{
                //    // Goto official selection page
                //    this.NavigateToPage(new Admin.EditBallotOptionPage(voterNav));
                //}
                //else
                //{
                //    AlertDialog settingsFailed = new AlertDialog("UNABLE TO EDIT THE BALLOT STYLE AT THIS TIME");
                //    settingsFailed.ShowDialog();
                //}

                // Always goto provisional page
                this.NavigateToPage(new Ballots.ProvisionalBallotPage(voterNav.Voter, true, true));
            }
            else
            {
                StatusBar.TextCenter = "Database not found";

                this.Cursor = Cursors.Arrow;
                StatusBar.TextLeft = "";
                StatusBar.HideLeftSpinner();
            }
        }

        //public async Task<VoterDataModel> GetSingleVoterData(string voterID)
        //{
        //    var voter = await Task.Run(() => VoterMethods.Voter.Single(voterID, AppSettings.Election.ElectionType).FirstOrDefault());
        //    string errorMessage = voter.CheckForError();
        //    if (errorMessage != null)
        //    {
        //        StatusBar.StatusTextCenter = "SQL ERROR: " + errorMessage;
        //    }
        //    return voter;
        //}
    }
}
