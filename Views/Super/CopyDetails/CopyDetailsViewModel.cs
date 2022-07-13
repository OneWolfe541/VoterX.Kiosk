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

namespace VoterX.Kiosk.Views.Super.CopyDetails
{
    public class CopyDetailsViewModel : NotifyPropertyChanged
    {
        public CopyDetailsViewModel()
        {

        }

        #region Lists
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
        #endregion

        #region FromVoterCommands
        public NMVoter CopyFromVoter { get; set; }

        private string _fromBarCode;
        public string FromBarCode
        {
            get { return _fromBarCode; }
            set
            {
                _fromBarCode = value;
                RaisePropertyChanged("FromBarCode");
            }
        }

        #region VoterHistoryList
        private List<VoterHistoryDisplayModel> _voterHistoryList;
        public List<VoterHistoryDisplayModel> VoterHistoryList
        {
            get
            {
                if (_voterHistoryList == null && CopyFromVoter != null)
                {
                    _voterHistoryList = CopyFromVoter.HistoryList()
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
                if (CopyFromVoter.Data.LogCode >= 9 && CopyFromVoter.Data.LogCode < 14)
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
                    _selectedVoterHistoryItem = VoterHistoryList.Where(h => h.LogCode == CopyFromVoter.Data.LogCode).OrderByDescending(h => h.HistoryDate).FirstOrDefault();
                }
                return _selectedVoterHistoryItem;
            }

            set
            {
                _selectedVoterHistoryItem = value;

                CopyFromVoter.RestoreHistory(_selectedVoterHistoryItem.HistoryId);

                //RaisePropertyChanged("Voter");
                //RaisePropertyChanged("SelectedPrecinctPartItem");
                //RaisePropertyChanged("SelectedBallotStyleItem");
                //RaisePropertyChanged("SelectedPartyItem");
                //RaisePropertyChanged("SelectedLogCodeItem");
                //RaisePropertyChanged("SelectedLocationItem");

                RaisePropertyChanged("CopyFromVoter");
                RaisePropertyChanged("SelectedFromLogCodeItem");
                RaisePropertyChanged("SelectedFromPrecinctPartItem");
                RaisePropertyChanged("SelectedFromBallotStyleItem");
                RaisePropertyChanged("SelectedFromLocationItem");
                RaisePropertyChanged("SelectedFromBallotRejectedReasonItem");
            }
        }
        #endregion

        #region FromPrecinctParts
        private PrecinctModel _selectedFromPrecinctPartItem;
        public PrecinctModel SelectedFromPrecinctPartItem
        {
            get
            {
                if (_selectedFromPrecinctPartItem == null && CopyFromVoter != null)
                {
                    _selectedFromPrecinctPartItem = PrecinctPartList
                    .Where(pp => pp.PrecinctPartID == CopyFromVoter.Data.PrecinctPartID).FirstOrDefault();
                }
                return _selectedFromPrecinctPartItem;
            }

            set
            {
                if (CopyFromVoter != null)
                {
                    _selectedFromPrecinctPartItem = value;

                    CopyFromVoter.Data.PrecinctPartID = _selectedFromPrecinctPartItem.PrecinctPartID;

                    RaisePropertyChanged("SelectedFromBallotStyleItem");

                    RaisePropertyChanged("CopyFromVoter");
                }
            }
        }
        #endregion

        #region FromBallotStyles
        private BallotStyleModel _selectedFromBallotStyleItem;
        public BallotStyleModel SelectedFromBallotStyleItem
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
                    if (SelectedFromPrecinctPartItem != null && SelectedFromPartyItem != null)
                    {
                        _selectedFromBallotStyleItem = ElectionDataMethods.Election.Lists.BallotStyles
                            .Where(b => b.PrecinctPartID == SelectedFromPrecinctPartItem.PrecinctPartID &&
                            b.Party == SelectedFromPartyItem.PartyCode)
                            .FirstOrDefault();
                    }
                }
                else // General Election
                {
                    if (SelectedFromPrecinctPartItem != null)
                    {
                        _selectedFromBallotStyleItem = ElectionDataMethods.Election.Lists.BallotStyles
                            .Where(b => b.PrecinctPartID == SelectedFromPrecinctPartItem.PrecinctPartID)
                            .FirstOrDefault();
                    }
                }
                return _selectedFromBallotStyleItem;
            }

            set
            {
                _selectedFromBallotStyleItem = value;

                CopyFromVoter.Data.BallotStyle = _selectedFromBallotStyleItem.BallotStyleName;
                CopyFromVoter.Data.BallotStyleID = _selectedFromBallotStyleItem.BallotStyleId;
                CopyFromVoter.Data.BallotStyleFile = _selectedFromBallotStyleItem.BallotStyleFileName;

                RaisePropertyChanged("CopyFromVoter");
            }
        }
        #endregion

        #region FromParties
        private PartyModel _selectedFromPartyItem;
        public PartyModel SelectedFromPartyItem
        {
            get
            {
                if (_selectedFromPartyItem == null && CopyFromVoter != null && CopyFromVoter.Data.Party != null && PartyList != null)
                {
                    _selectedFromPartyItem = PartyList
                        .Where(p => p.PartyCode == CopyFromVoter.Data.Party)
                        .FirstOrDefault();
                }
                return _selectedFromPartyItem;
            }
            set
            {
                _selectedFromPartyItem = value;
                if (_selectedFromPartyItem != null)
                {
                    RaisePropertyChanged("SelectedFromBallotStyleItem");
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
                    .Where(t => t.Char == CopyFromVoter.Data.AbsenteeType).FirstOrDefault();
                return _selectedVoterTypesItem;
            }

            set
            {
                _selectedVoterTypesItem = value;

                CopyFromVoter.Data.AbsenteeType = _selectedVoterTypesItem.Char;

                RaisePropertyChanged("Voter");
            }
        }
        #endregion

        #region FromLogCodes
        private LogCodeModel _selectedFromLogCodeItem;
        public LogCodeModel SelectedFromLogCodeItem
        {
            get
            {
                if (CopyFromVoter != null)
                {
                    // Always set log code to voter's current value
                    _selectedFromLogCodeItem = LogCodeList
                        .Where(lc => lc.LogCode == CopyFromVoter.Data.LogCode).FirstOrDefault();
                }
                return _selectedFromLogCodeItem;
            }

            set
            {
                _selectedFromLogCodeItem = value;

                CopyFromVoter.Data.LogCode = _selectedFromLogCodeItem.LogCode;

                RaisePropertyChanged("CopyFromVoter");
            }
        }
        #endregion

        #region FromLocations
        private LocationModel _selectedFromLocationItem;
        public LocationModel SelectedFromLocationItem
        {
            get
            {
                if (CopyFromVoter != null)
                {
                    // Always set location to voter's current value
                    _selectedFromLocationItem = LocationList
                        .Where(l => l.PollId == CopyFromVoter.Data.PollID).FirstOrDefault();
                }
                return _selectedFromLocationItem;
            }

            set
            {
                _selectedFromLocationItem = value;

                CopyFromVoter.Data.PollID = _selectedFromLocationItem.PollId;

                RaisePropertyChanged("CopyFromVoter");
            }
        }
        #endregion

        #region FromBallotRejectedReasons
        private BallotRejectedReasonModel _selectedFromBallotRejectedReasonItem;
        public BallotRejectedReasonModel SelectedFromBallotRejectedReasonItem
        {
            get
            {
                if (CopyFromVoter != null)
                {
                    // Always set location to voter's current value
                    _selectedFromBallotRejectedReasonItem = BallotRejectedReasonList
                        .Where(l => l.ServiseCode == CopyFromVoter.Data.BallotRejectedReason).FirstOrDefault();
                }
                return _selectedFromBallotRejectedReasonItem;
            }

            set
            {
                _selectedFromBallotRejectedReasonItem = value;

                CopyFromVoter.Data.BallotRejectedReason = _selectedFromBallotRejectedReasonItem.ServiseCode;

                RaisePropertyChanged("CopyFromVoter");
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

        #region SearchFromCommands
        public RelayCommand _searchFromVoterCommand;
        public ICommand SearchFromVoterCommand
        {
            get
            {
                if (_searchFromVoterCommand == null)
                {
                    _searchFromVoterCommand = new RelayCommand(param => this.SearchFromVoterClick());
                }
                return _searchFromVoterCommand;
            }
        }

        private void SearchFromVoterClick()
        {
            CopyFromVoter = VoterMethods.Voters.Single(FromBarCode).FirstOrDefault();
            RaisePropertyChanged("CopyFromVoter");
            RaisePropertyChanged("VoterHistoryList");
            RaisePropertyChanged("SelectedVoterHistoryItem");
            RaisePropertyChanged("SelectedFromLogCodeItem");
            RaisePropertyChanged("SelectedFromPrecinctPartItem");
            RaisePropertyChanged("SelectedFromBallotStyleItem");
            RaisePropertyChanged("SelectedFromLocationItem");
            RaisePropertyChanged("SelectedFromBallotRejectedReasonItem");

        }
        #endregion
        #endregion

        #region ToVoterCommands
        public NMVoter CopyToVoter { get; set; }

        private string _toBarCode;
        public string ToBarCode
        {
            get { return _toBarCode; }
            set
            {
                _toBarCode = value;
                RaisePropertyChanged("ToBarCode");
            }
        }

        #region ToLogCodes
        private LogCodeModel _selectedToLogCodeItem;
        public LogCodeModel SelectedToLogCodeItem
        {
            get
            {
                if (CopyToVoter != null)
                {
                    // Always set log code to voter's current value
                    _selectedToLogCodeItem = LogCodeList
                        .Where(lc => lc.LogCode == CopyToVoter.Data.LogCode).FirstOrDefault();
                }
                return _selectedToLogCodeItem;
            }

            set
            {
                _selectedToLogCodeItem = value;

                CopyToVoter.Data.LogCode = _selectedToLogCodeItem.LogCode;

                RaisePropertyChanged("CopyToVoter");
            }
        }
        #endregion

        #region ToPrecinctParts
        private PrecinctModel _selectedToPrecinctPartItem;
        public PrecinctModel SelectedToPrecinctPartItem
        {
            get
            {
                if (_selectedToPrecinctPartItem == null && CopyToVoter != null)
                {
                    _selectedToPrecinctPartItem = PrecinctPartList
                    .Where(pp => pp.PrecinctPartID == CopyToVoter.Data.PrecinctPartID).FirstOrDefault();
                }
                return _selectedToPrecinctPartItem;
            }

            set
            {
                _selectedToPrecinctPartItem = value;

                CopyToVoter.Data.PrecinctPartID = _selectedToPrecinctPartItem.PrecinctPartID;

                RaisePropertyChanged("SelectedToBallotStyleItem");

                RaisePropertyChanged("CopyToVoter");
            }
        }
        #endregion

        #region ToParties
        private PartyModel _selectedToPartyItem;
        public PartyModel SelectedToPartyItem
        {
            get
            {
                if (_selectedToPartyItem == null && CopyToVoter != null && CopyToVoter.Data.Party != null && PartyList != null)
                {
                    _selectedToPartyItem = PartyList
                        .Where(p => p.PartyCode == CopyToVoter.Data.Party)
                        .FirstOrDefault();
                }
                return _selectedToPartyItem;
            }
            set
            {
                _selectedToPartyItem = value;
                if (_selectedToPartyItem != null)
                {
                    RaisePropertyChanged("SelectedToBallotStyleItem");
                }
            }
        }
        #endregion

        #region ToBallotStyles
        private BallotStyleModel _selectedToBallotStyleItem;
        public BallotStyleModel SelectedToBallotStyleItem
        {
            get
            {
                if (AppSettings.Election.ElectionType == ElectionType.Primary) // Primary Election
                {
                    if (SelectedToPrecinctPartItem != null && SelectedToPartyItem != null)
                    {
                        _selectedToBallotStyleItem = ElectionDataMethods.Election.Lists.BallotStyles
                            .Where(b => b.PrecinctPartID == SelectedToPrecinctPartItem.PrecinctPartID &&
                            b.Party == SelectedToPartyItem.PartyCode)
                            .FirstOrDefault();
                    }
                }
                else // General Election
                {
                    if (SelectedToPrecinctPartItem != null)
                    {
                        _selectedToBallotStyleItem = ElectionDataMethods.Election.Lists.BallotStyles
                            .Where(b => b.PrecinctPartID == SelectedToPrecinctPartItem.PrecinctPartID)
                            .FirstOrDefault();
                    }
                }
                return _selectedToBallotStyleItem;
            }

            set
            {
                _selectedToBallotStyleItem = value;

                CopyToVoter.Data.BallotStyle = _selectedToBallotStyleItem.BallotStyleName;
                CopyToVoter.Data.BallotStyleID = _selectedToBallotStyleItem.BallotStyleId;
                CopyToVoter.Data.BallotStyleFile = _selectedToBallotStyleItem.BallotStyleFileName;

                RaisePropertyChanged("CopyToVoter");
            }
        }
        #endregion

        #region ToLocations
        private LocationModel _selectedToLocationItem;
        public LocationModel SelectedToLocationItem
        {
            get
            {
                if (CopyToVoter != null)
                {
                    // Always set location to voter's current value
                    _selectedToLocationItem = LocationList
                        .Where(l => l.PollId == CopyToVoter.Data.PollID).FirstOrDefault();
                }
                return _selectedToLocationItem;
            }

            set
            {
                _selectedToLocationItem = value;

                CopyToVoter.Data.PollID = _selectedToLocationItem.PollId;

                RaisePropertyChanged("CopyToVoter");
            }
        }
        #endregion

        #region ToBallotRejectedReasons
        private BallotRejectedReasonModel _selectedToBallotRejectedReasonItem;
        public BallotRejectedReasonModel SelectedToBallotRejectedReasonItem
        {
            get
            {
                if (CopyToVoter != null)
                {
                    // Always set location to voter's current value
                    _selectedToBallotRejectedReasonItem = BallotRejectedReasonList
                        .Where(l => l.ServiseCode == CopyToVoter.Data.BallotRejectedReason).FirstOrDefault();
                }
                return _selectedToBallotRejectedReasonItem;
            }

            set
            {
                _selectedToBallotRejectedReasonItem = value;

                CopyToVoter.Data.BallotRejectedReason = _selectedToBallotRejectedReasonItem.ServiseCode;

                RaisePropertyChanged("CopyToVoter");
            }
        }
        #endregion

        #region SearchToCommands
        public RelayCommand _searchToVoterCommand;
        public ICommand SearchToVoterCommand
        {
            get
            {
                if (_searchToVoterCommand == null)
                {
                    _searchToVoterCommand = new RelayCommand(param => this.SearchToVoterClick());
                }
                return _searchToVoterCommand;
            }
        }

        private void SearchToVoterClick()
        {
            CopyToVoter = VoterMethods.Voters.Single(ToBarCode).FirstOrDefault();
            RaisePropertyChanged("CopyToVoter");
            //RaisePropertyChanged("VoterHistoryList");
            RaisePropertyChanged("SelectedToLogCodeItem");
            RaisePropertyChanged("SelectedToPrecinctPartItem");
            RaisePropertyChanged("SelectedToPartyItem");
            RaisePropertyChanged("SelectedToBallotStyleItem");
            RaisePropertyChanged("SelectedToLocationItem");
            RaisePropertyChanged("SelectedToBallotRejectedReasonItem");
        }
        #endregion
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
            var test = CopyFromVoter;
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
                if (CopyFromVoter != null && CopyFromVoter.Data.LogCode != 2)
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
            YesNoDialog newMessage = new YesNoDialog("Are You Sure", "THIS WILL REVERT THE VOTER'S STATUS BACK TO " + SelectedFromLogCodeItem.LogDescription.ToUpper() +"\rAND CHANGE THEIR ACTIVITY DATES\r\n\r\nARE YOU SURE YOU WANT CONTINUE?");
            if (newMessage.ShowDialog() == true)
            {
                CopyFromVoter.Data.LocationID = AppSettings.System.SiteID;
                CopyFromVoter.SaveChanges("Manually Reverted To Previous State");
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
                if (CopyFromVoter != null && CopyFromVoter.Data.LogCode > 1)
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
                CopyFromVoter.Data.LogCode = 1;
                CopyFromVoter.Data.VotedDate = null;
                CopyFromVoter.Data.BallotIssued = null;
                CopyFromVoter.Data.BallotPrinted = null;
                CopyFromVoter.Data.ActivityCode = null;
                CopyFromVoter.Data.NotTabulated = false;
                CopyFromVoter.Data.CreatedOnDate = DateTime.MinValue;
                CopyFromVoter.Data.BallotStyleID = null;
                CopyFromVoter.Data.PrecinctPartID = null;
                CopyFromVoter.Data.PollID = null;

                CopyFromVoter.Data.ApplicationIssued = null;
                CopyFromVoter.Data.ApplicationAccepted = null;
                CopyFromVoter.Data.ApplicationRejected = null;
                CopyFromVoter.Data.ApplicationRejectedReason = null;

                CopyFromVoter.Data.AddressType = null;
                CopyFromVoter.Data.DeliveryAddress1 = null;
                CopyFromVoter.Data.DeliveryAddress2 = null;
                CopyFromVoter.Data.DeliveryCity = null;
                CopyFromVoter.Data.DeliveryState = null;
                CopyFromVoter.Data.DeliveryZip = null;
                CopyFromVoter.Data.DeliveryCountry = null;
                CopyFromVoter.Data.TempAddress = false;

                CopyFromVoter.Data.BallotRejectedDate = null;
                CopyFromVoter.Data.BallotRejectedReason = null;

                CopyFromVoter.Data.OutGoingIMB = null;
                CopyFromVoter.Data.InComingIMB = null;

                CopyFromVoter.Data.SpoiledReason = null;

                CopyFromVoter.Data.ActivityDate = DateTime.Now;

                RaisePropertyChanged("Voter");
                RaisePropertyChanged("SelectedPrecinctPartItem");
                RaisePropertyChanged("SelectedBallotStyleItem");
                RaisePropertyChanged("SelectedPartyItem");
                RaisePropertyChanged("SelectedLogCodeItem");
                RaisePropertyChanged("SelectedLocationItem");

                CopyFromVoter.Data.LocationID = AppSettings.System.SiteID;
                CopyFromVoter.SaveChanges("Manually Reverted To Registered To Vote");
            }
        }
        #endregion
    }
}
