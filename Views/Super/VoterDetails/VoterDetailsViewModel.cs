using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VoterX.Utilities.Commands;
using VoterX.Core.Voters;
using VoterX.Core.Elections;
using VoterX.SystemSettings.Models;
using VoterX.Kiosk.Methods;
using VoterX.Utilities.Dialogs;
using VoterX.Core.Extensions;
using VoterX.Utilities.Views;
using VoterX.SystemSettings.Enums;
using VoterX.Kiosk.Models;

namespace VoterX.Kiosk.Views.Super.VoterDetails
{
    public class VoterDetailsViewModel : NotifyPropertyChanged
    {
        public NMVoter Voter { get; set; }

        public VoterDetailsViewModel(NMVoter voter)
        {
            Voter = voter;
        }

        #region VoterHistoryList
        private List<VoterHistoryDisplayModel> _voterHistoryList;
        public List<VoterHistoryDisplayModel> VoterHistoryList
        {
            get
            {
                if (_voterHistoryList == null)
                {
                    _voterHistoryList = Voter.HistoryList()
                        .Select(h => new VoterHistoryDisplayModel()
                        {
                            HistoryTitle = h.LogCode.ToString() + " " + ParseHistoryAction(h.HistoryAction.ToUpper()) + " " + h.HistoryDate.ToString(),
                            LogCode = h.LogCode,
                            HistoryDate = h.HistoryDate,
                            HistoryId = h.VotedRecordHistoryId
                        })
                        .OrderByDescending(h => h.HistoryDate)
                        .ToList();
                }
                return _voterHistoryList;
            }
        }

        // Does not remove extra space at the end
        private string ParseHistoryAction(string action)
        {
            int index = action.IndexOf("(");
            if (index >= 0)
                return action.Substring(0, index);
            else
                return action;
        }

        public bool VoterHistoryIsEnabled
        {
            get
            {
                if (Voter.Data.LogCode >= 9 && Voter.Data.LogCode < 14)
                {
                    return false;
                }
                else
                    return true;
            }

            set
            {

            }
        }

        private VoterHistoryDisplayModel _selectedVoterHistoryItem;
        public VoterHistoryDisplayModel SelectedVoterHistoryItem
        {
            get
            {
                if(VoterHistoryList != null)
                {
                    _selectedVoterHistoryItem = VoterHistoryList.Where(h => h.LogCode == Voter.Data.LogCode).OrderByDescending(h => h.HistoryDate).FirstOrDefault();
                }
                return _selectedVoterHistoryItem;
            }

            set
            {
                _selectedVoterHistoryItem = value;

                Voter.RestoreHistory(_selectedVoterHistoryItem.HistoryId);

                RaisePropertyChanged("Voter");
                RaisePropertyChanged("SelectedPrecinctPartItem");
                RaisePropertyChanged("SelectedBallotStyleItem");
                RaisePropertyChanged("SelectedPartyItem");
                RaisePropertyChanged("SelectedLogCodeItem");
                RaisePropertyChanged("SelectedLocationItem");
            }
        }
        #endregion

        #region PrecinctParts
        private List<PrecinctModel> _precinctPartList;
        public List<PrecinctModel> PrecinctPartList
        {
            get
            {
                if (_precinctPartList == null)
                {
                    _precinctPartList = ((App)Application.Current).Election.Lists.Precincts
                        .OrderBy(p => p.PrecinctName)
                        .ToList();
                }
                return _precinctPartList;
            }
        }

        private PrecinctModel _selectedPrecinctPartItem;
        public PrecinctModel SelectedPrecinctPartItem
        {
            get
            {
                if (_selectedPrecinctPartItem == null)
                {
                    _selectedPrecinctPartItem = PrecinctPartList
                    .Where(pp => pp.PrecinctPartID == Voter.Data.PrecinctPartID).FirstOrDefault();
                }
                return _selectedPrecinctPartItem;
            }

            set
            {
                _selectedPrecinctPartItem = value;

                Voter.Data.PrecinctPartID = _selectedPrecinctPartItem.PrecinctPartID;

                RaisePropertyChanged("SelectedBallotStyleItem");

                RaisePropertyChanged("Voter");
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
                    _ballotStyleList = ((App)Application.Current).Election.Lists.BallotStyles
                        .ToList();
                }
                return _ballotStyleList;
            }
        }

        private BallotStyleModel _selectedBallotStyleItem;
        public BallotStyleModel SelectedBallotStyleItem
        {
            get
            {
                //if (_selectedBallotStyleItem == null)
                //{
                //    _selectedBallotStyleItem = BallotStyleList
                //    .Where(bs => bs.BallotStyleId == Voter.Data.BallotStyleID).FirstOrDefault();
                //}
                //return _selectedBallotStyleItem;

                if (AppSettings.Election.ElectionType == ElectionType.Primary) // Primary Election
                {
                    if (SelectedPrecinctPartItem != null && SelectedPartyItem != null)
                    {
                        _selectedBallotStyleItem = ElectionDataMethods.Election.Lists.BallotStyles
                            .Where(b => b.PrecinctPartID == SelectedPrecinctPartItem.PrecinctPartID &&
                            b.Party == SelectedPartyItem.PartyCode)
                            .FirstOrDefault();
                    }
                }
                else // General Election
                {
                    if (SelectedPrecinctPartItem != null)
                    {
                        _selectedBallotStyleItem = ElectionDataMethods.Election.Lists.BallotStyles
                            .Where(b => b.PrecinctPartID == SelectedPrecinctPartItem.PrecinctPartID)
                            .FirstOrDefault();
                    }
                }
                return _selectedBallotStyleItem;
            }

            set
            {
                _selectedBallotStyleItem = value;

                Voter.Data.BallotStyle = _selectedBallotStyleItem.BallotStyleName;
                Voter.Data.BallotStyleID = _selectedBallotStyleItem.BallotStyleId;
                Voter.Data.BallotStyleFile = _selectedBallotStyleItem.BallotStyleFileName;

                RaisePropertyChanged("Voter");
            }
        }
        #endregion

        #region Parties
        private List<PartyModel> _partyList;
        public List<PartyModel> PartyList
        {
            get
            {
                //if (_partyList == null && AppSettings.Election.ElectionType == ElectionType.Primary)
                //{
                _partyList = ElectionDataMethods.Election.Lists.Partys
                                                .OrderBy(p => p.PartyCode)
                                                .ToList();
                //}
                return _partyList;
            }
        }

        private PartyModel _selectedPartyItem;
        public PartyModel SelectedPartyItem
        {
            get
            {
                if (_selectedPartyItem == null && Voter.Data.Party != null && PartyList != null)
                {
                    _selectedPartyItem = PartyList
                        .Where(p => p.PartyCode == Voter.Data.Party)
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
                }
            }
        }
        #endregion

        #region VoterTypes
        private List<NameCharPairModel> _voterTylesList;
        public List<NameCharPairModel> VoterTypesList
        {
            get
            {
                if (_voterTylesList == null)
                {
                    _voterTylesList = new List<NameCharPairModel>();
                    _voterTylesList.Add(new NameCharPairModel { Char = "R", Description = "REGULAR" });
                    _voterTylesList.Add(new NameCharPairModel { Char = "U", Description = "UNIFORMED SERVICES" });
                    _voterTylesList.Add(new NameCharPairModel { Char = "O", Description = "OVERSEAS VOTER" });
                    _voterTylesList.Add(new NameCharPairModel { Char = "E", Description = "EMERGENCY RESPONDER" });
                }
                return _voterTylesList;
            }
        }

        private NameCharPairModel _selectedVoterTypesItem;
        public NameCharPairModel SelectedVoterTypesItem
        {
            get
            {
                // Always set voter type to voter's current value
                _selectedVoterTypesItem = VoterTypesList
                    .Where(t => t.Char == Voter.Data.AbsenteeType).FirstOrDefault();
                return _selectedVoterTypesItem;
            }

            set
            {
                _selectedVoterTypesItem = value;

                Voter.Data.AbsenteeType = _selectedVoterTypesItem.Char;

                RaisePropertyChanged("Voter");
            }
        }
        #endregion

        #region LogCodes
        private List<LogCodeModel> _logCodeList;
        public List<LogCodeModel> LogCodeList
        {
            get
            {
                if (_logCodeList == null)
                {
                    _logCodeList = ((App)Application.Current).Election.Lists.LogCodes
                        //.Where(lc => lc.LogCode < 8)
                        .ToList();
                }
                return _logCodeList;
            }
        }

        private LogCodeModel _selectedLogCodeItem;
        public LogCodeModel SelectedLogCodeItem
        {
            get
            {

                // Always set log code to voter's current value
                _selectedLogCodeItem = LogCodeList
                    .Where(lc => lc.LogCode == Voter.Data.LogCode).FirstOrDefault();
                return _selectedLogCodeItem;
            }

            set
            {
                _selectedLogCodeItem = value;

                Voter.Data.LogCode = _selectedLogCodeItem.LogCode;

                RaisePropertyChanged("Voter");
            }
        }
        #endregion

        #region Locations
        private List<LocationModel> _locationList;
        public List<LocationModel> LocationList
        {
            get
            {
                if (_locationList == null)
                {
                    _locationList = ((App)Application.Current).Election.Lists.Locations
                        .ToList();
                }
                return _locationList;
            }
        }

        private LocationModel _selectedLocationItem;
        public LocationModel SelectedLocationItem
        {
            get
            {
                // Always set location to voter's current value
                _selectedLocationItem = LocationList
                    .Where(l => l.PollId == Voter.Data.PollID).FirstOrDefault();
                return _selectedLocationItem;
            }

            set
            {
                _selectedLocationItem = value;

                Voter.Data.PollID = _selectedLocationItem.PollId;

                RaisePropertyChanged("Voter");
            }
        }
        #endregion

        #region BallotRejectedReasons
        private List<BallotRejectedReasonModel> _ballotRejectedReasonList;
        public List<BallotRejectedReasonModel> BallotRejectedReasonList
        {
            get
            {
                if (_ballotRejectedReasonList == null)
                {
                    _ballotRejectedReasonList = ((App)Application.Current).Election.Lists.BallotRejectedReasons
                        .ToList();
                }
                return _ballotRejectedReasonList;
            }
        }

        private BallotRejectedReasonModel _selectedBallotRejectedReasonItem;
        public BallotRejectedReasonModel SelectedBallotRejectedReasonItem
        {
            get
            {
                // Always set location to voter's current value
                _selectedBallotRejectedReasonItem = BallotRejectedReasonList
                    .Where(l => l.ServiseCode == Voter.Data.BallotRejectedReason).FirstOrDefault();
                return _selectedBallotRejectedReasonItem;
            }

            set
            {
                _selectedBallotRejectedReasonItem = value;

                Voter.Data.BallotRejectedReason = _selectedBallotRejectedReasonItem.ServiseCode;

                RaisePropertyChanged("Voter");
            }
        }
        #endregion

        #region Commands
        public RelayCommand _goBackCommand;
        public ICommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new RelayCommand(param => this.GoBackClick());
                }
                return _goBackCommand;
            }
        }

        private void GoBackClick()
        {
            //NavigationMenuMethods.NavigateToPage(new Login.LoginPage());

            var settingsPage = new Views.Settings.SettingsPage();

            MainMenuMethods.LoadMenu(new DynamicMenuView(new Menu.SettingsMenuViewModel(settingsPage)), StateVoterX.Utilities.Models.MenuCollapseMode.ShowIcons);
            
            NavigationMenuMethods.NavigateToPage(new Super.Search.SuperSearchPage(null));
        }
        #endregion

        #region AddressCommands
        private string _addressUpdatedMessage;
        public string AddressUpdatedMessage
        {
            get
            {
                return _addressUpdatedMessage;
            }
            set
            {
                _addressUpdatedMessage = value;
                RaisePropertyChanged("AddressUpdatedMessage");

                Voter.Data.AddressChanged = true;
            }
        }

        private bool _isMailingAddressSelected;
        public bool IsMailingAddressSelected
        {
            get
            {
                return _isMailingAddressSelected;
            }
            set
            {
                _isMailingAddressSelected = value;
                RaisePropertyChanged("IsMailingAddressSelected");
            }
        }

        private bool _isRegisteredAddressSelected;
        public bool IsRegisteredAddressSelected
        {
            get
            {
                return _isRegisteredAddressSelected;
            }
            set
            {
                _isRegisteredAddressSelected = value;
                RaisePropertyChanged("IsRegisteredAddressSelected");
            }
        }

        private bool _isTempAddressSelected;
        public bool IsTempAddressSelected
        {
            get
            {
                return _isTempAddressSelected;
            }
            set
            {
                _isTempAddressSelected = value;
                RaisePropertyChanged("IsTempAddressSelected");
            }
        }

        private RelayCommand _tempAddressChanged;
        public ICommand TempAddressChanged
        {
            get
            {
                if (_tempAddressChanged == null)
                {
                    _tempAddressChanged = new RelayCommand(param => this.TempAddressClick());
                }
                return _tempAddressChanged;
            }
        }

        private void TempAddressClick()
        {
            IsMailingAddressSelected = false;
            IsRegisteredAddressSelected = false;
            IsTempAddressSelected = true;
        }

        public RelayCommand _saveAddressCommand;
        public ICommand SaveAddressCommand
        {
            get
            {
                if (_saveAddressCommand == null)
                {
                    _saveAddressCommand = new RelayCommand(param => this.SaveAddressClick());
                }
                return _saveAddressCommand;
            }
        }

        private void SaveAddressClick()
        {
            // Check for valid temp address
            if (Voter.Data.TempAddress1 != "" && Voter.Data.TempAddress1 != null
                &&
                Voter.Data.TempCity != "" && Voter.Data.TempCity != null
                &&
                Voter.Data.TempState != "" && Voter.Data.TempState != null
                &&
                Voter.Data.TempZip != "" & Voter.Data.TempZip != null)
            {
                // Check for issued ballots
                if (Voter.Data.LogCode > 1 && (Voter.Data.LogCode <= 5 || Voter.Data.LogCode == 14))
                {
                    // Set delivered address with the new temp address
                    Voter.Data.DeliveryAddress1 = Voter.Data.TempAddress1;
                    Voter.Data.DeliveryAddress2 = Voter.Data.TempAddress2;
                    Voter.Data.DeliveryCity = Voter.Data.TempCity;
                    Voter.Data.DeliveryState = Voter.Data.TempState;
                    Voter.Data.DeliveryZip = Voter.Data.TempZip;
                    Voter.Data.DeliveryCountry = Voter.Data.TempCountry;

                    // Update display
                    RaisePropertyChanged("Voter");

                    // Save address to database
                    Voter.UpdateAddress();
                }
                else if (Voter.Data.LogCode == null || Voter.Data.LogCode <= 1)
                {
                    AlertDialog newMessage = new AlertDialog("NO APPLICATION HAS BEEN ISSUED TO THIS VOTER\r\nFROM THIS SYSTEM");
                    newMessage.ShowDialog();
                }
                else
                {
                    AlertDialog newMessage = new AlertDialog("THIS VOTER HAS ALREADY BEEN ISSUED A BALLOT");
                    newMessage.ShowDialog();
                }
            }
            else
            {
                AlertDialog newMessage = new AlertDialog("INVALID TEMP ADDRESS");
                newMessage.ShowDialog();
            }
        }

        private bool GetSelectionOrDeliveryAddress(int type)
        {
            if (Voter.HasDeliveryAddress())
            {
                return true;
            }
            else
            {
                return GetAddressFromSelection(type);
            }
        }

        private bool GetAddressFromSelection(int type)
        {
            bool result = false;

            if (IsMailingAddressSelected)
            {
                CopyAddressSelection(1);

                result = true;
            }
            else if (IsRegisteredAddressSelected)
            {
                CopyAddressSelection(2);

                result = true;
            }
            else if (IsTempAddressSelected)
            {
                // Check temp address fields
                if (
                // Address 1 not blank
                Voter.Data.TempAddress1.IsNullOrSpace()
                &&
                // Address 2 not blank
                Voter.Data.TempAddress2.IsNullOrSpace()
                &&
                // City not blank
                Voter.Data.TempCity.IsNullOrSpace()
                &&
                // State not blank
                Voter.Data.TempState.IsNullOrSpace()
                &&
                // Zip not blank
                Voter.Data.TempZip.IsNullOrSpace()
                )
                {
                    AlertDialog newMessage = new AlertDialog("INVALID TEMP ADDRESS");
                    newMessage.ShowDialog();

                    result = false;
                }
                else
                {
                    CopyAddressSelection(3);

                    result = true;
                }
            }

            return result;
        }

        private bool GetAddressFromDialog(int type)
        {
            bool result = false;

            // Copy Temp fields to voter.tempaddress
            //_voter.Data.TempAddress1 = TempAddress1.Text;
            //_voter.Data.TempAddress2 = TempAddress2.Text;
            //_voter.Data.TempCity = TempCity.Text;
            //_voter.Data.TempState = TempState.Text;
            //_voter.Data.TempZip = TempZip.Text;
            //_voter.Data.TempCountry = TempCountry.Text;

            // Check if addresses match
            if (
                // Temp address is not blank 
                !Voter.Data.TempAddress1.IsNullOrSpace()
                ||
                // Address 1 does not match
                Voter.Data.MailingAddress1 != Voter.Data.Address1
                ||
                // Address 2 does not match
                Voter.Data.MailingAddress2 != Voter.Data.Address2
                ||
                // City does not match
                Voter.Data.MailingCity != Voter.Data.City
                ||
                // State does not match
                Voter.Data.MailingState != Voter.Data.State
                ||
                // Zip does not match
                Voter.Data.MailingZip != Voter.Data.Zip
                )
            {
                // ADDRESSES DONT MATCH 
                // (SOME VOTERS DO NOT HAVE A MAILING ADDRESS)
                // If no mailing address is found and no temp address supplied then use the registered address (8/30/2019)

                // Check for blanks
                if (!Voter.Data.TempAddress1.IsNullOrSpace())
                {
                    result = PromptUserToSelectAddress(type);
                }
                else if (Voter.Data.MailingAddress1.IsNullOrSpace() && !Voter.Data.Address1.IsNullOrSpace())
                {
                    // Use Physical
                    CopyAddressSelection(2);
                    result = true;
                }
                else if (!Voter.Data.MailingAddress1.IsNullOrSpace() && Voter.Data.Address1.IsNullOrSpace())
                {
                    // Use Mailing
                    CopyAddressSelection(1);
                    result = true;
                }
                else if (!Voter.Data.Address1.IsNullOrSpace() && !Voter.Data.MailingAddress1.IsNullOrSpace())
                {
                    result = PromptUserToSelectAddress(type);
                }
                else if (Voter.Data.TempAddress1.IsNullOrSpace() && Voter.Data.Address1.IsNullOrSpace() && Voter.Data.MailingAddress1.IsNullOrSpace())
                {
                    AlertDialog newMessage = new AlertDialog("NO ADDRESS COULD BE FOUND FOR THIS VOTER");
                    newMessage.ShowDialog();
                }
                else
                {
                    AlertDialog newMessage = new AlertDialog("NO ADDRESS COULD BE FOUND FOR THIS VOTER");
                    newMessage.ShowDialog();
                }
            }
            else
            {
                Voter.Data.DeliveryAddress1 = Voter.Data.Address1;
                Voter.Data.DeliveryAddress2 = Voter.Data.Address2;
                Voter.Data.DeliveryCity = Voter.Data.City;
                Voter.Data.DeliveryState = Voter.Data.State;
                Voter.Data.DeliveryZip = Voter.Data.Zip;
                Voter.Data.DeliveryCountry = "";

                RaisePropertyChanged("Voter");

                result = true;
            }

            return result;
        }

        private bool PromptUserToSelectAddress(int type)
        {
            //// Prompt user to select a mailing address
            //AddressSelectionDialog addresSelection = new AddressSelectionDialog(Voter.Data, type);
            //if (addresSelection.ShowDialog() == true)
            //{
            //    //StatusBar.ApplicationStatus(addresSelection.Selection.ToString());

            //    CopyAddressSelection(addresSelection.Selection);

            //    return true;
            //}
            //else
            //{
            return false;
            //}
        }

        private void CopyAddressSelection(int selection)
        {
            // Copy address from user selection
            switch (selection)
            {
                case 1:
                    //StatusBar.ApplicationStatus("Ballot Sent To: " + _voter.MailingAddress1 + " " + _voter.MailingAddress2 + " " + _voter.MailingCity + " " + _voter.MailingState + " " + _voter.MailingZip);
                    // Copy Mailing Address to Delivery Address
                    Voter.Data.DeliveryAddress1 = Voter.Data.MailingAddress1;
                    Voter.Data.DeliveryAddress2 = Voter.Data.MailingAddress2;
                    Voter.Data.DeliveryCity = Voter.Data.MailingCity;
                    Voter.Data.DeliveryState = Voter.Data.MailingState;
                    Voter.Data.DeliveryZip = Voter.Data.MailingZip;
                    Voter.Data.DeliveryCountry = Voter.Data.MailingCountry;
                    Voter.Data.TempAddress = false;
                    break;
                case 2:
                    //StatusBar.ApplicationStatus("Ballot Sent To: " + _voter.Address1 + " " + _voter.Address2 + " " + _voter.City + " " + _voter.State + " " + _voter.Zip);
                    // Copy Registered Address to Delivery Address
                    Voter.Data.DeliveryAddress1 = Voter.Data.Address1;
                    Voter.Data.DeliveryAddress2 = Voter.Data.Address2;
                    Voter.Data.DeliveryCity = Voter.Data.City;
                    Voter.Data.DeliveryState = Voter.Data.State;
                    Voter.Data.DeliveryZip = Voter.Data.Zip;
                    Voter.Data.DeliveryCountry = null;
                    Voter.Data.TempAddress = false;
                    break;
                case 3:
                    //StatusBar.ApplicationStatus("Ballot Sent To: " + _voter.TempAddress1 + " " + _voter.TempAddress2 + " " + _voter.TempCity + " " + _voter.TempState + " " + _voter.TempZip);
                    // Copy Temp Address to Delivery Address
                    Voter.Data.DeliveryAddress1 = Voter.Data.TempAddress1;
                    Voter.Data.DeliveryAddress2 = Voter.Data.TempAddress2;
                    Voter.Data.DeliveryCity = Voter.Data.TempCity;
                    Voter.Data.DeliveryState = Voter.Data.TempState;
                    Voter.Data.DeliveryZip = Voter.Data.TempZip;
                    Voter.Data.DeliveryCountry = Voter.Data.TempCountry;
                    Voter.Data.TempAddress = true;
                    break;

                case 4:
                    //StatusBar.ApplicationStatus("Ballot Sent To: " + _voter.MailingAddress1 + " " + _voter.MailingAddress2 + " " + _voter.MailingCity + " " + _voter.MailingState + " " + _voter.MailingZip);
                    // Copy Mailing Address to Delivery Address
                    Voter.Data.DeliveryAddress1 = Voter.Data.MailingAddress1;
                    Voter.Data.DeliveryAddress2 = Voter.Data.MailingAddress2;
                    Voter.Data.DeliveryCity = Voter.Data.MailingCity;
                    Voter.Data.DeliveryState = Voter.Data.MailingState;
                    Voter.Data.DeliveryZip = Voter.Data.MailingZip;
                    Voter.Data.DeliveryCountry = Voter.Data.MailingCountry;
                    Voter.Data.OutofCountry = true;
                    break;
                case 5:
                    //StatusBar.ApplicationStatus("Ballot Sent To: " + _voter.Address1 + " " + _voter.Address2 + " " + _voter.City + " " + _voter.State + " " + _voter.Zip);
                    // Copy Registered Address to Delivery Address
                    Voter.Data.DeliveryAddress1 = Voter.Data.Address1;
                    Voter.Data.DeliveryAddress2 = Voter.Data.Address2;
                    Voter.Data.DeliveryCity = Voter.Data.City;
                    Voter.Data.DeliveryState = Voter.Data.State;
                    Voter.Data.DeliveryZip = Voter.Data.Zip;
                    Voter.Data.DeliveryCountry = null;
                    Voter.Data.OutofCountry = true;
                    break;
                case 6:
                    //StatusBar.ApplicationStatus("Ballot Sent To: " + _voter.TempAddress1 + " " + _voter.TempAddress2 + " " + _voter.TempCity + " " + _voter.TempState + " " + _voter.TempZip);
                    // Copy Temp Address to Delivery Address
                    Voter.Data.DeliveryAddress1 = Voter.Data.TempAddress1;
                    Voter.Data.DeliveryAddress2 = Voter.Data.TempAddress2;
                    Voter.Data.DeliveryCity = Voter.Data.TempCity;
                    Voter.Data.DeliveryState = Voter.Data.TempState;
                    Voter.Data.DeliveryZip = Voter.Data.TempZip;
                    Voter.Data.DeliveryCountry = Voter.Data.TempCountry;
                    Voter.Data.OutofCountry = true;
                    break;
            }

            RaisePropertyChanged("Voter");
        }

        private void InitializeAddressSelection()
        {
            //Voter.Data.AddressChanged = false;

            // Check Temp Address Fields
            if (
            // Address 1 not blank
            !Voter.Data.TempAddress1.IsNullOrSpace()
            ||
            // Address 2 not blank
            !Voter.Data.TempAddress2.IsNullOrSpace()
            ||
            // City not blank
            !Voter.Data.TempCity.IsNullOrSpace()
            ||
            // State not blank
            !Voter.Data.TempState.IsNullOrSpace()
            ||
            // Zip not blank
            !Voter.Data.TempZip.IsNullOrSpace()
            ||
            // Country not blank
            !Voter.Data.TempCountry.IsNullOrSpace()
            )
            {
                // Set temp address selection
                IsTempAddressSelected = true;
            }

            // Check Mailing Address Fields
            else if (
            // Address 1 not blank
            !Voter.Data.MailingAddress1.IsNullOrSpace()
            ||
            // Address 2 not blank
            !Voter.Data.MailingAddress2.IsNullOrSpace()
            ||
            // City not blank
            !Voter.Data.MailingCity.IsNullOrSpace()
            ||
            // State not blank
            !Voter.Data.MailingState.IsNullOrSpace()
            ||
            // Zip not blank
            !Voter.Data.MailingZip.IsNullOrSpace()
            )
            {
                // Set mailing address selection
                IsMailingAddressSelected = true;
            }

            // Check Registered Address fields
            else if (
            // Address 1 not blank
            !Voter.Data.Address1.IsNullOrSpace()
            ||
            // Address 2 not blank
            !Voter.Data.Address2.IsNullOrSpace()
            ||
            // City not blank
            !Voter.Data.City.IsNullOrSpace()
            ||
            // State not blank
            !Voter.Data.State.IsNullOrSpace()
            ||
            // Zip not blank
            !Voter.Data.Zip.IsNullOrSpace()
            )
            {
                // Set registered address selection
                IsRegisteredAddressSelected = true;
            }
            else
            {
                IsTempAddressSelected = true;
            }
        }

        private RelayCommand _editAddressCommand;
        public ICommand EditAddressCommand
        {
            get
            {
                if (_editAddressCommand == null)
                {
                    _editAddressCommand = new RelayCommand(param => this.EditAddressClick(), param => this.CanEditAddress);
                }
                return _editAddressCommand;
            }
        }

        // Enable or Disable the Edit Address Button
        public bool CanEditAddress
        {
            get
            {
                return Voter.Data.LogCode > 1;
            }
        }

        private void EditAddressClick()
        {
            //GetAddressFromDialog(1);
            GetAddressFromSelection(1);

            AddressUpdatedMessage = "ADDRESS UPDATED";
        }
        #endregion

        #region EditVoterCommands
        private RelayCommand _saveChangesCommand;
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

        public bool CanSaveChanges
        {
            get
            {
                return true;
            }
        }

        private void SaveChangesClick()
        {
            var test = Voter;
        }

        private RelayCommand _revertHistoryCommand;
        public ICommand RevertHistoryCommand
        {
            get
            {
                if (_revertHistoryCommand == null)
                {
                    _revertHistoryCommand = new RelayCommand(param => this.RevertHistoryClick(), param => this.CanRevertHistory);
                }
                return _revertHistoryCommand;
            }
        }

        public bool CanRevertHistory
        {
            get
            {
                if (Voter.Data.LogCode != 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void RevertHistoryClick()
        {
            YesNoDialog newMessage = new YesNoDialog("Are You Sure", "THIS WILL REVERT THE VOTER'S STATUS BACK TO " + SelectedLogCodeItem.LogDescription.ToUpper() +"\rAND CHANGE THEIR ACTIVITY DATES\r\n\r\nARE YOU SURE YOU WANT CONTINUE?");
            if (newMessage.ShowDialog() == true)
            {
                Voter.Data.LocationID = AppSettings.System.SiteID;
                Voter.SaveChanges("Manually Reverted To Previous State");
            }
        }

        private RelayCommand _markBackCommand;
        public ICommand MarkBackCommand
        {
            get
            {
                if (_markBackCommand == null)
                {
                    _markBackCommand = new RelayCommand(param => this.MarkBackClick(), param => this.CanMarkBack);
                }
                return _markBackCommand;
            }
        }

        public bool CanMarkBack
        {
            get
            {
                if (Voter.Data.LogCode > 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                //return true;
            }
        }

        private void MarkBackClick()
        {
            YesNoDialog newMessage = new YesNoDialog("Are You Sure", "THIS WILL REVERT THE VOTER'S STATUS BACK TO REGISTERED TO VOTE\rAND ERASE THEIR ACTIVITY DATES\r\n\r\nARE YOU SURE YOU WANT CONTINUE?");
            if (newMessage.ShowDialog() == true)
            {
                Voter.Data.LogCode = 1;
                Voter.Data.VotedDate = null;
                Voter.Data.BallotIssued = null;
                Voter.Data.BallotPrinted = null;
                Voter.Data.ActivityCode = null;
                Voter.Data.NotTabulated = false;
                Voter.Data.CreatedOnDate = DateTime.MinValue;
                Voter.Data.BallotStyleID = null;
                Voter.Data.PrecinctPartID = null;
                Voter.Data.PollID = null;

                Voter.Data.ApplicationIssued = null;
                Voter.Data.ApplicationAccepted = null;
                Voter.Data.ApplicationRejected = null;
                Voter.Data.ApplicationRejectedReason = null;

                Voter.Data.AddressType = null;
                Voter.Data.DeliveryAddress1 = null;
                Voter.Data.DeliveryAddress2 = null;
                Voter.Data.DeliveryCity = null;
                Voter.Data.DeliveryState = null;
                Voter.Data.DeliveryZip = null;
                Voter.Data.DeliveryCountry = null;
                Voter.Data.TempAddress = false;

                Voter.Data.BallotRejectedDate = null;
                Voter.Data.BallotRejectedReason = null;

                Voter.Data.OutGoingIMB = null;
                Voter.Data.InComingIMB = null;

                Voter.Data.SpoiledReason = null;

                Voter.Data.ActivityDate = DateTime.Now;

                RaisePropertyChanged("Voter");
                RaisePropertyChanged("SelectedPrecinctPartItem");
                RaisePropertyChanged("SelectedBallotStyleItem");
                RaisePropertyChanged("SelectedPartyItem");
                RaisePropertyChanged("SelectedLogCodeItem");
                RaisePropertyChanged("SelectedLocationItem");

                Voter.Data.LocationID = AppSettings.System.SiteID;
                Voter.SaveChanges("Manually Reverted To Registered To Vote");
            }
        }
        #endregion
    }
}
