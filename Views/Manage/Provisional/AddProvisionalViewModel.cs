using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VoterX.Core.Elections;
using VoterX.Core.Extensions;
using VoterX.Core.Voters;
using VoterX.SystemSettings.Enums;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Extensions;
using VoterX.Utilities.Methods;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Manage
{
    public class AddProvisionalViewModel : NotifyPropertyChanged
    {
        // Dont use Observable Collections for single voter display
        // Have to use INotifyPropertyChanged when not using an Observable Collection
        private NMVoter _voterItem;
        public NMVoter VoterItem
        {
            get
            {
                if(_voterItem == null)
                {
                    _voterItem = new NMVoter();
                    // Set connection string
                    _voterItem.ConnectionString = ((App)Application.Current).Connection;
                }
                return _voterItem;
            }
            set { _voterItem = value; }
        }

        public AddProvisionalViewModel()
        {
            SetDisplayMessages();

            // Initialize save button
            CanSaveChanges = true;
            CanGoBack = true;

            // Display page header
            StatusBar.PageHeader = "Add Provisional Voter";
        }

        #region DisplayText
        public string ProvisionalVoterMessage { get; set; }
        public string RequiredFieldsMessage { get; set; }
        public string SelectStyleMessage { get; set; }
        public string SelectReasonMessage { get; set; }

        private void SetDisplayMessages()
        {
            ProvisionalVoterMessage = "PLEASE ENTER THE VOTER'S INFORMATION IN THE BOXES BELOW";
            RequiredFieldsMessage = "* Indicates a required field";
            SelectStyleMessage = "SELECT A PRECINCT PART";
            if(AppSettings.Election.ElectionType == ElectionType.Primary)
            {
                SelectStyleMessage = "SELECT A PRECINCT PART AND PARTY";
            }
            SelectReasonMessage = "SELECT A REASON FOR PRINTING THIS PROVISIONAL BALLOT";
        }
        #endregion

        #region Commands
        public bool SaveButtonVisible
        {
            get
            {
                return (SelectedReasonItem != null && SelectedBallotStyleItem != null);
            }
        }

        // Bound command for returning to the search screen
        public RelayCommand _goBackCommand;
        public ICommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new RelayCommand(param => this.ReturnToSearchClick(), param => this.CanGoBack);
                }
                return _goBackCommand;
            }
        }

        private bool _canGoBack;
        public bool CanGoBack
        {
            get { return _canGoBack; }
            internal set
            {
                _canGoBack = value;
                RaisePropertyChanged("CanGoBack");
            }
        }

        // Force parent frame to navigate back to the search page
        public void ReturnToSearchClick()
        {
            MainMenuMethods.OpenMenu(); // THIS COMMAND SHOULD HAPPEN ELSEWHERE!

            //_parent.Navigate(new VoterSearchPage(_parent));
            //NavigationMenuMethods.VoterSearchPage();
            NavigationMenuMethods.ReturnToOrigin();
        }

        // Bound command for printing a provisional ballot
        public RelayCommand _printBallotCommand;
        public ICommand PrintBallotCommand
        {
            get
            {
                if (_printBallotCommand == null)
                {
                    _printBallotCommand = new RelayCommand(param => this.PrintBallotClick(), param => this.CanPrintBallot);
                }
                return _printBallotCommand;
            }
        }

        // Enable or Disable the Print Ballot Button
        private bool _canPrintBallot;
        public bool CanPrintBallot
        {
            get { return _canPrintBallot; }
            internal set
            {
                _canPrintBallot = value;
                RaisePropertyChanged("CanPrintBallot");
            }
        }

        private async void PrintBallotClick()
        {
            // Disable print button
            CanPrintBallot = false;
            CanGoBack = false;

            MainMenuMethods.HideMenu();
            MainMenuMethods.ClickEnabled = false;

            VoterItem.GetNextProvisionalId((int)AppSettings.System.SiteID);

            // Set local system values
            VoterItem.Localize(AppSettings.Global);

            // Set the selected ballot style
            VoterItem.SetBallotStyle(SelectedBallotStyleItem);

            // Mark the Provisional Ballot
            VoterItem.ProvisionalBallot(SelectedReasonItem.ProvisionalReasonId);

            // Print new Ballot
            //var errorMessage = await BallotPrinting.PrintProvisionalBallotBundleAsync(VoterItem, AppSettings.Global);
            var errorMessage = await Task.Run(() => BallotPrinting.PrintProvisionalBallot(VoterItem.Data, AppSettings.Global));

            if (errorMessage != null && errorMessage != "")
            {
                // Display Error Message
                StatusBar.TextCenter = errorMessage;
            }

            // Navigate to Provisional Ballot Troubleshooting page
            NavigationMenuMethods.ProvisionalPrintTroubleShootingPage(VoterItem);
        }

        // Bound command for saving the voter data
        public RelayCommand _saveChangesCommand;
        public ICommand SaveChangesCommand
        {
            get
            {
                if (_saveChangesCommand == null)
                {
                    _saveChangesCommand = new RelayCommand(param => this.SaveChangesClick(), param => this.CanSaveChanges);
                }
                return _saveChangesCommand;
            }
        }

        // Enable or Disable the Print Ballot Button
        private bool _canSaveChanges;
        public bool CanSaveChanges
        {
            get { return _canSaveChanges; }
            internal set
            {
                _canSaveChanges = value;
                RaisePropertyChanged("CanSaveChanges");
            }
        }

        private void SaveChangesClick()
        {
            CanSaveChanges = false;

            if(ValidateEntry() == true)
            {
                PrintBallotClick();
            }
            else
            {
                CanSaveChanges = true;
            }            
        }
        #endregion

        #region RequiredFields
        // Value are not being set in the GET method because initial values need to be NULL
        public bool? VoterIdRequired { get; set; }
        public bool? FirstNameRequired { get; set; }
        public bool? LastNameRequired { get; set; }
        public bool? AddressRequired { get; set; }
        public bool? CityRequired { get; set; }
        public bool? StateRequired { get; set; }
        public bool? ZipRequired { get; set; }
        public bool? BirthYearRequired { get; set; }

        // Set required field values based on voter data
        private bool ValidateEntry()
        {
            // Value to store final result
            // If any field is invalid then this value will be false
            bool result = true;

            // Display which field is missing
            string message = "";

            if (VoterItem != null)
            {
                //// Check Voter Id
                //VoterIdRequired = !VoterItem.Data.VoterID.IsNullOrEmpty();              // Check if field is empty
                //if (VoterIdRequired == false) message = "Invalid Id";                   // Create message
                //RaisePropertyChanged("VoterIdRequired");                                // Update proptery
                //result = result && (bool)VoterIdRequired;                               // Store result

                // Check First Name
                FirstNameRequired = !VoterItem.Data.FirstName.IsNullOrEmpty();          // Check if field is empty
                if (FirstNameRequired == false) message = "Invalid First Name";         // Create message
                RaisePropertyChanged("FirstNameRequired");                              // Update property
                result = result && (bool)FirstNameRequired;                             // Store result

                // Check Last Name
                LastNameRequired = !VoterItem.Data.LastName.IsNullOrEmpty();            // Check if field is empty
                if (LastNameRequired == false) message = "Invalid Last Name";           // Create message
                RaisePropertyChanged("LastNameRequired");                               // Update proptery
                result = result && (bool)LastNameRequired;                              // Store result

                // Check Address
                AddressRequired = !VoterItem.Data.Address1.IsNullOrEmpty();             // Check if field is empty
                if (AddressRequired == false) message = "Invalid Address";              // Create message
                RaisePropertyChanged("AddressRequired");                                // Update proptery
                result = result && (bool)AddressRequired;                               // Store result

                // Check City
                CityRequired = !VoterItem.Data.City.IsNullOrEmpty();                    // Check if field is empty
                if (CityRequired == false) message = "Invalid City";                    // Create message
                RaisePropertyChanged("CityRequired");                                   // Update proptery
                result = result && (bool)CityRequired;                                  // Store result

                // Check State
                StateRequired = !VoterItem.Data.State.IsNullOrEmpty();                  // Check if field is empty
                if (StateRequired == false) message = "Invalid State";                  // Create message
                RaisePropertyChanged("StateRequired");                                  // Update proptery
                result = result && (bool)StateRequired;                                 // Store result

                // Check Zip
                ZipRequired = !VoterItem.Data.Zip.IsNullOrEmpty();                      // Check if field is empty
                if (ZipRequired == false) message = "Invalid Zip";                      // Create message
                RaisePropertyChanged("ZipRequired");                                    // Update proptery
                result = result && (bool)ZipRequired;                                   // Store result

                // Check Birth Year
                BirthYearRequired = !VoterItem.Data.DOBYear.IsNullOrEmpty();            // Check if field is empty
                // ToInt() returns a 0 if the string is not an integer
                BirthYearRequired = VoterItem.Data.DOBYear.ToInt() > 1900;              // Check if date is in range
                if (BirthYearRequired == false) message = "Invalid Birth Year";         // Create message
                RaisePropertyChanged("BirthYearRequired");                              // Update proptery
                result = result && (bool)BirthYearRequired;                             // Store result
            }

            // Display message on status bar
            StatusBar.TextLeft = message;

            return result;
        }
        #endregion

        #region ProvisionalReasons
        private List<ProvisionalReasonModel> _reasonsList;
        public List<ProvisionalReasonModel> ReasonsList
        {
            get
            {
                // Create list of invalid reasons
                List<int> doNotUse = new List<int> { 7, 9 };

                // Get list of reasons from the proloaded election object
                if (_reasonsList == null)
                {
                    _reasonsList = ElectionDataMethods.Election.Lists.ProvisionalReasons
                        .Where(r => !doNotUse.Contains(r.ProvisionalReasonId))
                        .ToList();
                }
                return _reasonsList;
            }
        }

        private ProvisionalReasonModel _selectedReasonItem;
        public ProvisionalReasonModel SelectedReasonItem
        {
            get
            {
                if(_selectedReasonItem == null && ReasonsList != null)
                {
                    _selectedReasonItem = ReasonsList
                        .Where(r => r.ProvisionalReasonId == 4)
                        .FirstOrDefault();
                }
                return _selectedReasonItem;
            }
            set
            {
                // When a reason is selected display the print button
                _selectedReasonItem = value;
                if (_selectedReasonItem != null)
                {
                    RaisePropertyChanged("SaveButtonVisible");
                }
            }
        }
        #endregion

        #region Precincts
        private List<PrecinctModel> _precinctList;
        public List<PrecinctModel> PrecinctList
        {
            get
            {
                if (_precinctList == null)
                {
                    // Filter Precincts By Elligible Ballot Styles
                    var validPrecincts = ElectionDataMethods.BallotStyles.Select(b => b.PrecinctPartID).ToList();
                    _precinctList = ElectionDataMethods.Election.Lists.Precincts.Where(p => validPrecincts.Contains(p.PrecinctPartID)).OrderBy(p => p.PrecinctName).ToList();
                }
                return _precinctList;
            }
        }

        private PrecinctModel _selectedPrecinctItem;
        public PrecinctModel SelectedPrecinctItem
        {
            get
            {
                if (_selectedPrecinctItem == null && VoterItem.Data.PrecinctPartID != null)
                {
                    _selectedPrecinctItem = ElectionDataMethods.Election.Lists.Precincts
                        .Where(p => p.PrecinctPartID == VoterItem.Data.PrecinctPartID)
                        .FirstOrDefault();
                }
                return _selectedPrecinctItem;
            }
            set
            {
                _selectedPrecinctItem = value;
                if (_selectedPrecinctItem != null)
                {
                    RaisePropertyChanged("SelectedBallotStyleItem");

                    RaisePropertyChanged("SaveButtonVisible");
                }
            }
        }
        #endregion

        #region Parties
        private bool? _partyVisibility;
        public bool PartyVisibility
        {
            get
            {
                if (_partyVisibility == null)
                {
                    // Display party options for primary elections only
                    if (AppSettings.Election.ElectionType == ElectionType.Primary)
                    {
                        _partyVisibility = true;
                    }
                }
                return _partyVisibility ?? false;
            }

            set
            {
                _partyVisibility = value;
                RaisePropertyChanged("PartyVisibility");
            }
        }

        private List<PartyModel> _partyList;
        public List<PartyModel> PartyList
        {
            get
            {
                if (_partyList == null && AppSettings.Election.ElectionType == ElectionType.Primary)
                {
                    _partyList = ElectionDataMethods.Election.Lists.Partys
                                                    .Where(p => AppSettings.Election.EligibleParties.Contains(p.PartyCode))
                                                    .OrderBy(p => p.PartyCode)
                                                    .ToList();
                }
                return _partyList;
            }
        }

        private PartyModel _selectedPartyItem;
        public PartyModel SelectedPartyItem
        {
            get
            {
                if (_selectedPartyItem == null && VoterItem.Data.Party != null && PartyList != null)
                {
                    _selectedPartyItem = PartyList
                        .Where(p => p.PartyCode == VoterItem.Data.Party)
                        .FirstOrDefault();
                }
                return _selectedPartyItem;
            }
            set
            {
                _selectedPartyItem = value;
                if (_selectedPartyItem != null)
                {
                    RaisePropertyChanged("SelectedBallotStyleItem");

                    RaisePropertyChanged("SaveButtonVisible");
                }
            }
        }
        #endregion

        #region BallotStyles
        private List<BallotStyleModel> _ballotStyleList;
        public List<BallotStyleModel> BallotStyleList
        {
            get
            {
                if (_ballotStyleList == null)
                {
                    _ballotStyleList = ElectionDataMethods.Election.Lists.BallotStyles;
                }
                return _ballotStyleList;
            }
        }

        private BallotStyleModel _selectedBallotStyleItem;
        public BallotStyleModel SelectedBallotStyleItem
        {
            get
            {
                if (AppSettings.Election.ElectionType == ElectionType.Primary) // Primary Election
                {
                    if (SelectedPrecinctItem != null && SelectedPartyItem != null)
                    {
                        _selectedBallotStyleItem = ElectionDataMethods.Election.Lists.BallotStyles
                            .Where(b => b.PrecinctPartID == SelectedPrecinctItem.PrecinctPartID &&
                            b.Party == SelectedPartyItem.PartyCode)
                            .FirstOrDefault();
                    }
                }
                else // General Election
                {
                    if (SelectedPrecinctItem != null)
                    {
                        _selectedBallotStyleItem = ElectionDataMethods.Election.Lists.BallotStyles
                            .Where(b => b.PrecinctPartID == SelectedPrecinctItem.PrecinctPartID)
                            .FirstOrDefault();
                    }
                }
                return _selectedBallotStyleItem;
            }
            set
            {
                _selectedBallotStyleItem = value;
                if (_selectedBallotStyleItem != null)
                {

                }
            }
        }
        #endregion
    }
}
