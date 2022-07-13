using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoterX.Core.Elections;
using VoterX.Core.Voters;
using VoterX.SystemSettings.Enums;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Extensions;
using VoterX.Utilities.Methods;
using VoterX.Utilities.Views.VoterDetails;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Manage
{
    public class EditBallotViewModel : NotifyPropertyChanged
    {
        // Dont use Observable Collections for single voter display
        // Have to use INotifyPropertyChanged when not using an Observable Collection
        private NMVoter VoterItem { get; set; }

        private bool _editStyle;

        public EditBallotViewModel(NMVoter voter) : this(voter, false) { }
        public EditBallotViewModel(NMVoter voter, bool editStyle)
        {
            VoterItem = voter;

            _editStyle = editStyle;

            SetDisplayMessages();

            // Initialize print button
            CanPrintBallot = true;
            CanGoBackBallot = true;

            // Display page header
            StatusBar.PageHeader = "Edit Ballot Style";
        }

        #region DisplayText
        public string ProvisionalBallotMessage { get; set; }
        public string SelectStyleMessage { get; set; }
        public string SelectReasonMessage { get; set; }

        private void SetDisplayMessages()
        {
            ProvisionalBallotMessage = "EDIT THE BALLOT STYLE FOR CURRENT VOTER";
            SelectStyleMessage = "SELECT A BALLOT STYLE";
            SelectReasonMessage = "SELECT A PROVISIONAL REASON";
        }
        #endregion

        #region VoterDetails
        private VoterDetailsViewModel _voterDetailsView;
        public VoterDetailsViewModel VoterDetailsView
        {
            get
            {
                if (_voterDetailsView == null)
                {
                    _voterDetailsView = new VoterDetailsViewModel(VoterItem);
                }
                return _voterDetailsView;
            }
            private set
            {
                _voterDetailsView = value;
            }
        }
        #endregion

        #region Commands
        public bool PrintButtonVisible
        {
            get
            {
                //return (SelectedReasonItem != null && SelectedBallotStyleItem != null);
                return true;
                //return VoterItem.CheckStatus(AppSettings.System.SiteID.Value) == VoterX.Core.Utilities.VoterLookupStatus.Spoilable;                
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
                    _goBackCommand = new RelayCommand(param => this.ReturnToSearchClick(), param => this.CanGoBackBallot);
                }
                return _goBackCommand;
            }
        }

        private bool _canGoBackBallot;
        public bool CanGoBackBallot
        {
            get { return _canGoBackBallot; }
            internal set
            {
                _canGoBackBallot = value;
                RaisePropertyChanged("CanGoBackBallot");
            }
        }

        // Force parent frame to navigate back to the search page
        public void ReturnToSearchClick()
        {
            //_parent.Navigate(new VoterSearchPage(_parent));
            //NavigationMenuMethods.VoterSearchPage();
            NavigationMenuMethods.ReturnToOrigin();

            //MainMenuMethods.OpenMenu();
        }

        // Bound command for printing an provisional ballot
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
            CanGoBackBallot = false;

            MainMenuMethods.HideMenu();
            MainMenuMethods.ClickEnabled = false;

            // Set local system values
            VoterItem.Localize(AppSettings.Global);

            // Set the selected ballot style
            VoterItem.SetBallotStyle(SelectedBallotStyleItem);

            // Mark the Provisional Ballot
            VoterItem.ProvisionalBallot(6);

            // Spoil Ballot if Voter Rejected
            //if (SelectedReasonItem.ProvisionalReasonId == 9)
            //{
            //    // Mark Ballot Surrendered
            //    VoterItem.Data.BallotSurrendered = true;

            //    // Spoil Ballot
            //    VoterItem.SpoilBallot(SelectedReasonItem.ProvisionalReasonId);

            //    // Mark Voter as Not Tabulated
            //    VoterManagmentMethods.MarkAsNotTabulated(VoterItem);
            //}

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

        // Bound command for printing an provisional ballot
        public RelayCommand _spoilBallotCommand;
        public ICommand SpoilBallotCommand
        {
            get
            {
                if (_spoilBallotCommand == null)
                {
                    _spoilBallotCommand = new RelayCommand(param => this.SpoilBallotClick(), param => this.CanSpoilBallot);
                }
                return _spoilBallotCommand;
            }
        }

        // Enable or Disable the Print Ballot Button
        private bool _canspoilBallot;
        public bool CanSpoilBallot
        {
            get
            {
                //return _canspoilBallot;
                return VoterItem.CheckStatus(AppSettings.System.SiteID.Value) == VoterX.Core.Utilities.VoterLookupStatus.Spoilable;
            }
            internal set
            {
                _canspoilBallot = value;
                RaisePropertyChanged("CanSpoilBallot");
            }
        }

        private async void SpoilBallotClick()
        {
            // Disable print button
            CanSpoilBallot = false;
            CanGoBackBallot = false;

            MainMenuMethods.HideMenu();
            MainMenuMethods.ClickEnabled = false;

            // Set local system values
            VoterItem.Localize(AppSettings.Global);

            // Set the selected ballot style
            VoterItem.SetBallotStyle(SelectedBallotStyleItem);

            // Mark Spoiled Ballot
            VoterItem.SpoilBallot(6, SelectedPrecinctItem.PrecinctPartID, SelectedBallotStyleItem.BallotStyleId);

            VoterItem.GetNextBallotNumber((int)AppSettings.System.SiteID);
            VoterItem.UpdateBallotNumber();

            // Print new Ballot
            //var errorMessage = await BallotPrinting.PrintProvisionalBallotBundleAsync(VoterItem, AppSettings.Global);
            var errorMessage = await Task.Run(() => BallotPrinting.ReprintBallot(VoterItem.Data, AppSettings.Global));

            if (AppSettings.System.BallotStub == 1)
            {
                BallotPrinting.ReprintStub(VoterItem.Data, AppSettings.Global);
            }

            if (errorMessage != null && errorMessage != "")
            {
                // Display Error Message
                StatusBar.TextCenter = errorMessage;
            }

            // Navigate to Provisional Ballot Troubleshooting page
            NavigationMenuMethods.SpoiledPrintTroubleShootingPage(VoterItem);
        }
        #endregion

        #region ProvisionalReasons
        private List<ProvisionalReasonModel> _reasonsList;
        public List<ProvisionalReasonModel> ReasonsList
        {
            get
            {
                // Create list of invalid reasons
                List<int> doNotUse = new List<int> { 7 };
                // Reason #9 "Voter Rejected Ballot" is only used from the edit ballot style process
                var spoilable = VoterItem.VotedHereToday(AppSettings.SiteId);
                if (_editStyle == false || spoilable == false) doNotUse.Add(9);
                else doNotUse.Add(5); // #5 and #9 are mutually exclusive              

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
                return _selectedReasonItem;
            }
            set
            {
                // When a reason is selected display the print button
                _selectedReasonItem = value;
                if (_selectedReasonItem != null)
                {
                    RaisePropertyChanged("PrintButtonVisible");
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
                // Get precinct from voter
                if (_selectedPrecinctItem == null && VoterItem.Data.PrecinctPartID != null)
                {
                    _selectedPrecinctItem = ElectionDataMethods.Election.Lists.Precincts
                        .Where(p => p.PrecinctPartID == VoterItem.Data.PrecinctPartID)
                        .FirstOrDefault();
                }
                // Get precinct from voter's ballot style
                else if (_selectedPrecinctItem == null && VoterItem.Data.BallotStyleID != null)
                {
                    var precinctPartId = ElectionDataMethods.Election.Lists.BallotStyles
                        .Where(bs => bs.BallotStyleId == VoterItem.Data.BallotStyleID)
                        .FirstOrDefault().PrecinctPartID;

                    _selectedPrecinctItem = ElectionDataMethods.Election.Lists.Precincts
                        .Where(p => p.PrecinctPartID == precinctPartId)
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

                    RaisePropertyChanged("PrintButtonVisible");
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
                if(_partyVisibility == null)
                {
                    // Display party options for primary elections only
                    if(AppSettings.Election.ElectionType == ElectionType.Primary)
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
                    // Display voters party from eligible list
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
                // Get ballot style from selected precinct
                if (SelectedPrecinctItem != null)
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
                }
                // Get ballot style from voter
                else if(VoterItem != null && VoterItem.Data.BallotStyleID != null)
                {
                    _selectedBallotStyleItem = ElectionDataMethods.Election.Lists.BallotStyles
                        .Where(b => b.BallotStyleId == VoterItem.Data.BallotStyleID)
                        .FirstOrDefault();
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
