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

namespace VoterX.Kiosk.Views.VoidedBallots
{
    public class VoidAbsenteeBallotViewModel : NotifyPropertyChanged
    {
        // Dont use Observable Collections for single voter display
        // Have to use INotifyPropertyChanged when not using an Observable Collection
        private NMVoter VoterItem { get; set; }

        public VoidAbsenteeBallotViewModel(NMVoter voter)
        {
            VoterItem = voter;

            SetDisplayMessages();

            // Initialize spoil ballot button
            CanSpoilBallot = true;
            CanPrintAffidavit = true;

            // Display Header
            StatusBar.PageHeader = "Void Absentee Ballot";
        }

        #region DisplayText
        public string SpoiledBallotMessage { get; set; }
        public string InnerSurrenderedQuestion { get; set; }

        private void SetDisplayMessages()
        {
            SpoiledBallotMessage = "VOID AN ABSENTEE BALLOT FOR CURRENT VOTER";
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
                List<int> onlyUse = new List<int> { 7 };

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
                if (ReasonsList != null)
                {
                    _selectedReasonItem = ReasonsList.Where(r => r.SpoiledReasonId == 7).FirstOrDefault();
                }
                return _selectedReasonItem;
            }
            set
            {
                _selectedReasonItem = value;                
            }
        }
        #endregion

        #region BallotSurrenderedQuestionControl
        private bool _spoilButtonVisible;
        public bool SpoilButtonVisible
        {
            get
            {
                _spoilButtonVisible = true;
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
                    _goBackCommand = new RelayCommand(param => this.ReturnToSearchClick());
                }
                return _goBackCommand;
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

            // Set local system values
            VoterItem.Localize(AppSettings.Global);

            {
                // Mark Spoiled Ballot
                VoterItem.SpoilBallot(SelectedReasonItem.SpoiledReasonId);

                // Update Ballot Number
                VoterItem.GetNextBallotNumber((int)AppSettings.System.SiteID);

                VoterItem.UpdateBallotNumber();

                // Change local voter's log code to match
                //VoterItem.Data.LogCode = 2; // I may want to retain the 6

                // Void the Absentee Ballot and mark Voted At Polls
                //VoterItem.VoidedAtPolls((int)AppSettings.System.VCCType);

                // Print Affidavit
                BallotPrinting.PrintAffidavit(VoterItem.Data, AppSettings.Global);

                // Go to signature capture screen
                NavigationMenuMethods.VoidSignatureCapturePage(VoterItem);

                //// Check if affidavit printed
                //YesNoDialog affidavitQuestion = new YesNoDialog("Affidavit Check", "DID THE AFFIDAVIT PRINT CORRECTLY?");
                //if (affidavitQuestion.ShowDialog() == true) // MAY NEED TO MAKE THIS INTO A LOOP
                //{
                //    // Navigate to Signature Capture screen
                //    NavigationMenuMethods.VoidSignatureCapturePage(VoterItem);
                //}
                //else
                //{
                //    YesNoDialog printQuestion = new YesNoDialog("Affidavit Check", "WOULD YOU LIKE TO PRINT ANOTHER AFFIDAVIT?");
                //    if(printQuestion.ShowDialog() == true)
                //    {
                //        // Print Affidavit
                //        BallotPrinting.PrintAffidavit(VoterItem.Data, AppSettings.Global);

                //        YesNoDialog affidavitQuestion2 = new YesNoDialog("Affidavit Check", "DID THE AFFIDAVIT PRINT CORRECTLY?");
                //        if (affidavitQuestion2.ShowDialog() == true) // MAY NEED TO MAKE THIS INTO A LOOP
                //        {
                //            // Navigate to Signature Capture screen
                //            NavigationMenuMethods.VoidSignatureCapturePage(VoterItem);
                //        }
                //    }
                //}

                // Print new Ballot
                //var errorMessage = await Task.Run(() => BallotPrinting.ReprintBallot(VoterItem.Data, AppSettings.Global));

                //// Permit on Election Day
                //if (AppSettings.System.VCCType == VotingCenterMode.ElectionDay)
                //{
                //    if (AppSettings.System.Permit == 1)
                //    {
                //        BallotPrinting.ReprintPermit(VoterItem.Data, AppSettings.Global);
                //    }
                //}
                //else if(AppSettings.System.VCCType == VotingCenterMode.EarlyVoting)
                //{
                //    BallotPrinting.ReprintApplication(VoterItem.Data, AppSettings.Global);
                //}

                //if (AppSettings.System.BallotStub == 1)
                //{
                //    BallotPrinting.ReprintStub(VoterItem.Data, AppSettings.Global);
                //}

                //if (errorMessage != null && errorMessage != "")
                //{
                //    // Display Error Message
                //    StatusBar.TextCenter = errorMessage;
                //}
                //else
                //{
                //    // Navigate to Spoiled Ballot Troubleshooting page
                //    NavigationMenuMethods.SpoiledPrintTroubleShootingPage(VoterItem);
                //}
                //NavigationMenuMethods.SpoiledPrintTroubleShootingPage(VoterItem);
            }
        }

        // Bound command for printing an official ballot
        public RelayCommand _printAffidavitCommand;
        public ICommand PrintAffidavitCommand
        {
            get
            {
                if (_printAffidavitCommand == null)
                {
                    _printAffidavitCommand = new RelayCommand(param => this.PrintAffidavitClick(), param => this.CanPrintAffidavit);
                }
                return _printAffidavitCommand;
            }
        }

        // Enable or Disable the Print Ballot Button
        private bool _canPrintAffidavit;
        public bool CanPrintAffidavit
        {
            get { return _canPrintAffidavit; }
            internal set
            {
                _canPrintAffidavit = value;
                RaisePropertyChanged("CanPrintAffidavit");
            }
        }

        private void PrintAffidavitClick()
        {
            // Disable spoil ballot button
            CanPrintAffidavit = false;

            // Print Affidavit
            BallotPrinting.PrintAffidavit(VoterItem.Data, AppSettings.Global);

            // Go to signature capture screen
            NavigationMenuMethods.VoidSignatureCapturePage(VoterItem);
        }
        #endregion
    }
}
