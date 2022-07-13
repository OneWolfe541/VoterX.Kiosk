using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using VoterX.Logging;
using VoterX.Core.Utilities;
using VoterX.Core.Voters;
using VoterX.SystemSettings.Enums;
using VoterX.Utilities.Views;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Super.Search
{
    /// <summary>
    /// Interaction logic for VoterSearchPage.xaml
    /// </summary>
    public partial class SuperSearchPage : Page
    {
        VoterSearchModel _searchItems;

        public SuperSearchPage() : this(null) { }
        public SuperSearchPage(VoterSearchModel SearchItems)
        {
            InitializeComponent();

            _searchItems = SearchItems;

            // Clear Status Bar
            //GlobalReferences.StatusBar.TextClear();

            // Set page header
            //GlobalReferences.Header.PageHeader = "Voter Lookup";

            StatusBar.PageHeader = "Edit Voter Lookup";

            StatusBar.Clear();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Check for updates in system settings
            AppSettings.CheckUpdates(true);

            StatusBar.PageHeader = "Edit Voter Lookup";

            VoterSearchNameView.Visibility = Visibility.Collapsed;
            VoterSearchScanView.Visibility = Visibility.Visible;

            MainMenuMethods.LoadMenu(new DynamicMenuView(new Menu.SuperSearchMenuViewModel(this)), StateVoterX.Utilities.Models.MenuCollapseMode.ShowIcons);
        }

        public void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            SaveLogout();
            NavigationMenuMethods.NavigateToPage(new Login.LoginPage());
        }

        private async void SaveLogout()
        {
            if (await Task.Run(() => ElectionDataMethods.Exists) == true)
            {
                StatusBar.TextLeft = "Logging Out";
                StatusBar.ShowLeftSpinner();
                if (await Task.Run(() => PollSummaryMethods.SaveLogout()) == false) StatusBar.TextCenter = "Logout not saved";
                StatusBar.TextLeft = "";
                StatusBar.HideLeftSpinner();
            }
        }

        #region VoterListView
        // Load Default Search View
        private void VoterSearchView_Loaded(object sender, RoutedEventArgs e)
        {
            VoterSearchViewModel voterViewModelObject = new VoterSearchViewModel(AppSettings.Global);

            voterViewModelObject.PropertyChanged += OnVoterSelectedPropertyChanged;

            voterViewModelObject.SearchAnimation = false;

            VoterSearchView.DataContext = voterViewModelObject;
        }

        // Get the Selected Voter
        private void OnVoterSelectedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedVoter") 
            {
                //Code to respond to a change in the ViewModel
                Console.WriteLine(
                    "Selected Voter: " +
                    ((VoterSearchViewModel)sender).SelectedVoter.Data.VoterID.ToString() + " | STATUS: " +
                    ((VoterSearchViewModel)sender).SelectedVoter.CheckStatus((int)AppSettings.System.SiteID));

                // Get selected voter
                var selectedVoter = ((VoterSearchViewModel)sender).SelectedVoter;

                // THIS SHOULD ONLY BE SET WHEN A VOTER IS MARKED
                //// Set local system values to selected voter
                //selectedVoter.Data.ElectionID = AppSettings.Election.ElectionID;
                //selectedVoter.Data.UserId = AppSettings.User.UserID;
                //selectedVoter.Data.UserName = AppSettings.User.UserName;
                //if (selectedVoter.Data.PollID == null)
                //{
                //    selectedVoter.Data.PollID = AppSettings.System.SiteID;
                //    selectedVoter.Data.PollName = AppSettings.System.SiteName;
                //}

                // Create Blank search panels
                //LoadVoterSearchName();
                //LoadVoterSearchScan();

                // Set Origin
                //NavigationMenuMethods.SetOrigin(this);
                NavigationMenuMethods.SetOrigin(new SuperSearchPage());

                // Navigate to next page based on the selected voters status
                //NavigationMenuMethods.NavigateToPage(ChooseVoterDestination(selectedVoter, (int)AppSettings.System.SiteID));

                NavigationMenuMethods.NavigateToPage(new Views.Super.VoterDetails.VoterDetailsPage(selectedVoter));
            }
        }

        private Page ChooseVoterDestination(NMVoter Voter, int SiteId)
        {
            VoterNavModel voterNav = new VoterNavModel
            {
                Voter = Voter,
                Search = _searchItems
            };

            // Get the voters current status
            VoterLookupStatus status;
            if (AppSettings.Global.Site.HybridLocation == true)
            {
                status = Voter.CheckStatusHybrid(SiteId);
            }
            else
            {
                status = Voter.CheckStatus(SiteId);
            }

            if (AppSettings.System.VCCType == VotingCenterMode.SampleBallots)
            {
                // Check if voter is eligible at the current site
                if (status == VoterLookupStatus.Ineligible)
                {
                    return new Validation.VerifyInvalidVoterPage(voterNav);
                    //return null;
                }
                else
                {
                    // Sample Ballots Mode
                    return new Validation.VerifySampleVoterPage(voterNav);
                }
            }
            else
            {
                switch (status)
                {
                    case VoterLookupStatus.Eligible:
                        //return new Verification.VerifyValidVoterPage(voterNav);
                        return new Validation.VerifyValidVoterPage(voterNav);
                    //break;
                    case VoterLookupStatus.Ineligible:
                        if (AppSettings.Election.ElectionType == ElectionType.Primary)
                        {
                            return new Validation.VerifyInvalidPartyPage(voterNav);
                        }
                        else
                        {
                            //return new Verification.VerifyInvalidVoterPage(voterNav);
                            return new Validation.VerifyInvalidVoterPage(voterNav);
                        }                        
                    //break;
                    case VoterLookupStatus.Spoilable:
                        //return new Verification.VerifySpoiledBallotPage(voterNav);
                        return new Validation.VerifySpoiledVoterPage(voterNav);
                    //break;
                    case VoterLookupStatus.Provisional:
                        //return new Verification.VerifyProvisionalBallotPage(voterNav);
                        return new Validation.VerifyProvisionalVoterPage(voterNav);
                    //break;
                    case VoterLookupStatus.Deleted:
                        //return new Verification.VerifyDeletedVoterPage(voterNav);
                        return new Validation.VerifyDeletedVoterPage(voterNav);
                    //break;
                    case VoterLookupStatus.Hybrid:
                        //return new Validation.VerifyHybridBallotPage(voterNav);
                        return new Validation.VerifyHybridVoterPage(voterNav);
                    //break;
                    default:
                        return null;
                        //break;
                }
            }

        }

        // Search for a Voter with the given parameters
        private async void SearchForVoters(VoterSearchModel SearchItems)
        {
            // Clear any error messages from the status bar
            StatusBar.Clear();

            _searchItems = SearchItems;

            if (!_searchItems.IsNullOrEmpty())
            {
                VoterSearchViewModel voterViewModelObject = new VoterSearchViewModel(AppSettings.Global);

                // Wire up Voter Selection
                voterViewModelObject.PropertyChanged += OnVoterSelectedPropertyChanged;

                // Start Search Animation
                voterViewModelObject.SearchAnimation = true;
                VoterSearchView.DataContext = voterViewModelObject;

                //var test = VoterMethods.Voters.List();

                // Load voters from database
                try
                {
                    //var result = await voterViewModelObject.LoadVotersAsync(SearchItems, AppSettings.Election.ElectionType);
                    //var result = await voterViewModelObject.LoadVotersAsync(VoterMethods.LimitedList(SearchItems));

                    

                    var result = await voterViewModelObject.LoadVotersAsync(() => VoterMethods.Voters.LimitedList(SearchItems));

                    //voterViewModelObject.LoadVoters(SearchItems, (int)AppSettings.Election.ElectionType);
                }
                catch (Exception e)
                {
                    StatusBar.TextCenter = e.Message;

                    var searchlog = new VoterXLogger("VCClogs", true);
                    searchlog.WriteLog("Load Voter List Failed: " + e.Message);
                    if (e.InnerException != null)
                    {
                        searchlog.WriteLog(e.InnerException.Message);
                    }
                }

                // Stop Search Animation
                voterViewModelObject.SearchAnimation = false;
                VoterSearchView.DataContext = null;

                // Display List of Voters
                VoterSearchView.DataContext = voterViewModelObject;
            }
            else
            {
                VoterSearchViewModel voterViewModelObject = new VoterSearchViewModel(AppSettings.Global);

                // Display no criteria message
                StatusBar.TextLeft = "No Search Parameters Entered";
                voterViewModelObject.DisplayResults("Please enter valid search criteria.");

                VoterSearchView.DataContext = voterViewModelObject;
            }
        }

        // Clear the Search View
        private void ClearVotersList()
        {
            VoterSearchViewModel voterViewModelObject = new VoterSearchViewModel(AppSettings.Global)
            {
                SearchAnimation = false
            };

            VoterSearchView.DataContext = voterViewModelObject;
        }
        #endregion VoterListView


        #region SearchByName
        // Load initial Search By Name View
        private void VoterSearchName_Loaded(object sender, RoutedEventArgs e)
        {
            LoadVoterSearchName();
        }

        private void LoadVoterSearchName()
        {
            // Initialize the Name Search panel
            VoterSearchNameViewModel voterSearchNameViewModel;
            if (_searchItems == null)
            {
                // Create Blank search panel
                voterSearchNameViewModel = new VoterSearchNameViewModel();
            }
            else
            {
                // Create a panel with values from the previous search
                voterSearchNameViewModel = new VoterSearchNameViewModel(_searchItems);
            }

            // Wire up property changed methods
            voterSearchNameViewModel.PropertyChanged += OnSearchNamePropertyChanged;
            voterSearchNameViewModel.PropertyChanged += OnSearchNameClearPropertyChanged;
            voterSearchNameViewModel.PropertyChanged += OnSearchNameScanPropertyChanged;

            VoterSearchNameView.DataContext = voterSearchNameViewModel;
        }

        // Get Search Parameters from View
        private void OnSearchNamePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VoterSearch")
            {
                // Check for updates in system settings
                AppSettings.CheckUpdates(true);

                // Search for voters
                Console.WriteLine("Search Options: " + ((VoterSearchNameViewModel)sender).VoterSearch.LastName);
                SearchForVoters(((VoterSearchNameViewModel)sender).VoterSearch);
            }
        }

        // Get the Clear command from View
        private void OnSearchNameClearPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VoterClear")
            {
                // Clear the list
                Console.WriteLine("Search Cleared: ");
                ClearVotersList();
            }
        }

        // Get the Switch command from View
        private void OnSearchNameScanPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VoterScan")
            {
                // Switch to Scan View
                Console.WriteLine("Search Set To Scan: ");
                VoterSearchNameView.Visibility = Visibility.Collapsed;
                VoterSearchScanView.Visibility = Visibility.Visible;
            }
        }
        #endregion SearchByName


        #region SearchByNumber
        // Load initial Search By Number View
        private void VoterSearchScan_Loaded(object sender, RoutedEventArgs e)
        {
            LoadVoterSearchScan();
        }

        private void LoadVoterSearchScan()
        {
            VoterSearchScanViewModel voterSearchScanViewModel = new VoterSearchScanViewModel();

            // Wire up property changed methods
            voterSearchScanViewModel.PropertyChanged += OnSearchScanPropertyChanged;
            voterSearchScanViewModel.PropertyChanged += OnSearchScanClearPropertyChanged;
            voterSearchScanViewModel.PropertyChanged += OnSearchScanSwtichPropertyChanged;

            VoterSearchScanView.DataContext = voterSearchScanViewModel;
        }

        // Get Search Parameters from View
        private void OnSearchScanPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VoterSearch")
            {
                // Check for updates in system settings
                AppSettings.CheckUpdates(true);

                // Search for a voter
                Console.WriteLine("Search Options: " + ((VoterSearchScanViewModel)sender).VoterSearch.VoterID);
                SearchForVoters(((VoterSearchScanViewModel)sender).VoterSearch);
            }
        }

        // Get the Clear command from View
        private void OnSearchScanClearPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VoterClear")
            {
                // Clear the list
                Console.WriteLine("Search Cleared: ");
                ClearVotersList();
            }
        }

        // Get the Switch command from View
        private void OnSearchScanSwtichPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VoterSwitch")
            {
                // Switch to Name View
                Console.WriteLine("Search Set To Name: ");
                VoterSearchNameView.Visibility = Visibility.Visible;
                VoterSearchScanView.Visibility = Visibility.Collapsed;
            }
        }
        #endregion SearchByNumber        
    }
}
