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
using VoterX.Core.Voters;
using VoterX.Utilities.Views;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Manage
{
    /// <summary>
    /// Interaction logic for EditBallotStylesPage.xaml
    /// </summary>
    public partial class EditBallotStylesPage : Page
    {
        public EditBallotStylesPage()
        {
            InitializeComponent();

            StatusBar.PageHeader = "Edit Ballot Style Search";

            StatusBar.Clear();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
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

                //NavigationMenuMethods.ProvisionalBallotPage(selectedVoter, true);
                NavigationMenuMethods.EditBallotPage(selectedVoter);
            }
        }

        // Search for a Voter with the given parameters
        private async void SearchForVoters(VoterSearchModel SearchItems)
        {
            VoterSearchViewModel voterViewModelObject = new VoterSearchViewModel(AppSettings.Global);

            // Wire up Voter Selection
            voterViewModelObject.PropertyChanged += OnVoterSelectedPropertyChanged;

            // Start Search Animation
            voterViewModelObject.SearchAnimation = true;
            VoterSearchView.DataContext = voterViewModelObject;

            // Load voters from database
            //var result = await voterViewModelObject.LoadVotersAsync(SearchItems, AppSettings.Election.ElectionType);
            var result = await voterViewModelObject.LoadVotersAsync(() => VoterMethods.Voters.LimitedList(SearchItems));

            // Stop Search Animation
            voterViewModelObject.SearchAnimation = false;
            VoterSearchView.DataContext = null;

            // Display List of Voters
            VoterSearchView.DataContext = voterViewModelObject;
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

            // Create Blank search panel
            voterSearchNameViewModel = new VoterSearchNameViewModel();

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
