using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoterX.Core.Elections;
using VoterX.Core.Voters;
using VoterX.SystemSettings.Enums;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Controls;
using VoterX.Utilities.Dialogs;
using VoterX.Utilities.Extensions;
using VoterX.Utilities.Methods;
using VoterX.Utilities.Views.VoterDetails;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Voter.Ballots
{
    public class SpoilOfficialBallotViewModel : NotifyPropertyChanged
    {
        // Dont use Observable Collections for single voter display
        // Have to use INotifyPropertyChanged when not using an Observable Collection
        private NMVoter VoterItem { get; set; }

        public SpoilOfficialBallotViewModel(NMVoter voter)
        {
            VoterItem = voter;

            SetDisplayMessages();

            // Initialize spoil ballot button
            CanSpoilBallot = true;
            CanGoBackBallot = true;

            // Display Header
            StatusBar.PageHeader = "Spoil Ballot";
        }

        #region DisplayText
        public string SpoiledBallotMessage { get; set; }
        public string InnerSurrenderedQuestion { get; set; }

        private void SetDisplayMessages()
        {
            SpoiledBallotMessage = "SPOIL A BALLOT FOR CURRENT VOTER";
            InnerSurrenderedQuestion = "Has a physical ballot been surrendered?";
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

        #region SpoiledReasons
        private bool WrongVoter;
        private bool FledVoter;

        private bool _surrenderedVisible;
        public bool SurrenderedVisible
        {
            get
            {
                return _surrenderedVisible;
            }
            set
            {
                _surrenderedVisible = value;
            }
        }

        private List<SpoiledReasonModel> _reasonsList;
        public List<SpoiledReasonModel> ReasonsList
        {
            get
            {
                // Create list of invalid reasons
                List<int> doNotUse = new List<int> { 5, 9 };

                if (_reasonsList == null)
                {
                    _reasonsList = ElectionDataMethods.Election.Lists.SpoiledReasons
                        .Where(r => !doNotUse.Contains(r.SpoiledReasonId))
                        .ToList();
                }
                return _reasonsList;
            }
        }

        private SpoiledReasonModel _selectedReasonItem;
        public SpoiledReasonModel SelectedReasonItem
        {
            get
            {
                //if (_selectedReasonItem != null)
                //{
                //    GlobalReferences.StatusBar.TextLeft = _selectedReasonItem.ToString();
                //}
                return _selectedReasonItem;
            }
            set
            {
                _selectedReasonItem = value;
                if (_selectedReasonItem != null)
                {
                    SurrenderedVisible = true;
                    RaisePropertyChanged("SurrenderedVisible");

                    // Check if wrong or fled options were selected
                    switch (_selectedReasonItem.SpoiledReasonId)
                    {
                        case 3:
                            FledVoter = true;
                            WrongVoter = false;
                            break;
                        case 4:
                            FledVoter = false;
                            WrongVoter = true;
                            break;
                        default:
                            FledVoter = false;
                            WrongVoter = false;
                            break;
                    }
                }
                else
                {
                    SurrenderedVisible = false;
                    RaisePropertyChanged("SurrenderedVisible");
                }
            }
        }
        #endregion

        #region BallotSurrenderedQuestionControl
        private bool _spoilButtonVisible;
        public bool SpoilButtonVisible
        {
            get
            {
                return _spoilButtonVisible;
            }
            set
            {
                _spoilButtonVisible = value;
            }
        }

        public YesNoQuestionViewModel BallotSurrenderQuestion
        {
            get
            {
                YesNoQuestionViewModel BallotSurrenderQuestion = new YesNoQuestionViewModel(InnerSurrenderedQuestion);
                BallotSurrenderQuestion.PropertyChanged += BallotSurrenderPropertyChanged;
                return BallotSurrenderQuestion;
            }
        }

        private void BallotSurrenderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                // Show spoil ballot button
                SpoilButtonVisible = true;
                RaisePropertyChanged("SpoilButtonVisible");

                var question = sender as YesNoQuestionViewModel;

                // Update the voters ballot surrendered status
                VoterItem.Data.BallotSurrendered = question.IsChecked ?? false;
                RaisePropertyChanged("VoterItem");
            }
        }
        #endregion

        #region Commands
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
        }

        // Bound command for printing an official ballot
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
        private bool _canSpoilBallot;
        public bool CanSpoilBallot
        {
            get { return _canSpoilBallot; }
            internal set
            {
                _canSpoilBallot = value;
                RaisePropertyChanged("CanPrintBallot");
            }
        }

        private async void SpoilBallotClick()
        {
            // Disable spoil ballot button
            CanSpoilBallot = false;
            CanGoBackBallot = false;

            // Set local system values
            VoterItem.Localize(AppSettings.Global);

            if (FledVoter == true)
            {
                // Mark Fled Voter
                VoterItem.UpdateFledVoter();

                // Mark Spoiled Ballot
                VoterItem.SpoilBallot(SelectedReasonItem.SpoiledReasonId);

                // Display message
                AlertDialog fledDialog = new AlertDialog("THIS VOTER'S STATUS HAS BEEN CHANGED TO FLED VOTER");
                if (fledDialog.ShowDialog() == true)
                {
                    ReturnToSearchClick();
                }
            }
            else if (WrongVoter == true)
            {
                // Mark Wrong Voter
                VoterItem.UpdateWrongVoter();

                // Mark Spoiled Ballot
                VoterItem.SpoilBallot(SelectedReasonItem.SpoiledReasonId);

                // Display message
                AlertDialog wrongDialog = new AlertDialog("THIS VOTER'S STATUS HAS BEEN CHANGED TO WRONG VOTER");
                if (wrongDialog.ShowDialog() == true)
                {
                    ReturnToSearchClick();
                }
            }
            else
            {
                // Mark Spoiled Ballot
                VoterItem.SpoilBallot(SelectedReasonItem.SpoiledReasonId);

                // REMOVED BY JOHN 12/29/2020 "Spoiled Ballots should not increment the ballot numbers"
                // Update Ballot Number
                //VoterItem.GetNextBallotNumber((int)AppSettings.System.SiteID);
                //VoterItem.UpdateBallotNumber();

                // System Print Error should never increment the ballot number
                if (SelectedReasonItem.SpoiledReasonId == 2)
                {
                    VoterItem.GetNextBallotNumber((int)AppSettings.System.SiteID);
                    VoterItem.UpdateBallotNumber();
                }

                // Print new Ballot
                //var errorMessage = await BallotPrinting.PrintOfficialBallotBundleAsync(VoterItem, AppSettings.Global);
                var errorMessage = await Task.Run(() => BallotPrinting.ReprintBallot(VoterItem.Data, AppSettings.Global));

                // Reprint Permit on Election Day
                //if (AppSettings.System.VCCType == VotingCenterMode.ElectionDay)
                //{
                //    if (AppSettings.System.Permit == 1)
                //    {
                //        BallotPrinting.ReprintPermitSpoiled(VoterItem.Data, AppSettings.Global);
                //    }
                //}

                if (AppSettings.System.BallotStub == 1)
                {
                    BallotPrinting.ReprintStub(VoterItem.Data, AppSettings.Global);
                }

                if (errorMessage != null && errorMessage != "")
                {
                    // Display Error Message
                    StatusBar.TextCenter = errorMessage;
                }
                else
                {
                    // Navigate to Spoiled Ballot Troubleshooting page
                    NavigationMenuMethods.SpoiledPrintTroubleShootingPage(VoterItem);
                }
                NavigationMenuMethods.SpoiledPrintTroubleShootingPage(VoterItem);
            }
        }
        #endregion
    }
}
