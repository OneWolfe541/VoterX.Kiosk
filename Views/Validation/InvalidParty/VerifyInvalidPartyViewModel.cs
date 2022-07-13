using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoterX.Core.Voters;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Dialogs;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Validation
{
    public class VerifyInvalidPartyViewModel : VerifyVoterBaseViewModel
    {
        // Model constructors
        public VerifyInvalidPartyViewModel(NMVoter voter) : this(voter, null) { }
        public VerifyInvalidPartyViewModel(NMVoter voter, VoterSearchModel SearchItems)
        {
            VoterItem = voter;
            _searchItems = SearchItems;

            VoterItem.Data.IDRequired = false;

            SetDefaultQuestions();
            SetDefaultMessage();

            // Display Header
            StatusBar.PageHeader = "Voter Verification";
        }

        // Alternate constructor uses the Voter Navigation Model
        public VerifyInvalidPartyViewModel(VoterNavModel voterNav) : this(voterNav.Voter, voterNav.Search) { }

        #region QuestionText
        // QUESTIONS TEXT
        public string NameQuestion { get; set; }
        public string AddressQuestion { get; set; }
        public string DateQuestion { get; set; }
        public string CheckVoterInfoMessage { get; set; }

        public string InvalidMessage { get; set; }
        public string RegisteredPartyMessage { get; set; }

        private void SetDefaultQuestions()
        {
            CheckVoterInfoMessage = "PLEASE VERIFY THE VOTER'S INFORMATION BY CHECKING THE BOXES";
            NameQuestion = "CONFIRM THE VOTER'S FULL NAME";
            AddressQuestion = "CONFIRM THE VOTER'S ADDRESS";
            DateQuestion = "CONFIRM THE VOTER'S BIRTH YEAR";
        }

        private void SetDefaultMessage()
        {
            InvalidMessage = "THIS VOTER IS NOT ELIGIBLE TO VOTE IN THIS ELECTION";
            RegisteredPartyMessage = "ONLY VOTERS REGISTERED WITH A MAJOR PARTY ARE ELIGIBLE TO VOTE IN A NEW MEXICO PRIMARY ELECTION";
        }
        #endregion

        #region Commands
        // Bound command for returning to the search screen
        private RelayCommand _goBackCommand;
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
            //_parent.Navigate(new VoterSearchPage(_parent, _searchItems));
            NavigationMenuMethods.VoterSearchPage(_searchItems);
        }

        // Bound command for navigating to the provisional ballot screen
        private RelayCommand _provisionalBallotCommand;
        public ICommand ProvisionalBallotCommand
        {
            get
            {
                if (_provisionalBallotCommand == null)
                {
                    _provisionalBallotCommand = new RelayCommand(param => this.ProvisionalBallotClick());
                }
                return _provisionalBallotCommand;
            }
        }

        // Force parent frame to navigate to the provisional ballot page
        public void ProvisionalBallotClick()
        {
            //_parent.Navigate(new VoterSearchPage(_parent, _searchItems));
            NavigationMenuMethods.ProvisionalBallotPage(VoterItem);
        }
        #endregion

        #region ProvisionalLogic
        // Address is valid checkbox
        private bool _displayProvisionalVoter;
        public bool DisplayProvisionalVoter
        {
            get { return _displayProvisionalVoter; }
            set
            {
                if (_displayProvisionalVoter == value) return;
                else
                {
                    _displayProvisionalVoter = value;
                    RaisePropertyChanged("DisplayProvisionalVoter");
                }
            }
        }

        // Bound command to start the provisional process
        private RelayCommand _startProvisionalCommand;
        public ICommand StartProvisionalCommand
        {
            get
            {
                if (_startProvisionalCommand == null)
                {
                    _startProvisionalCommand = new RelayCommand(param => this.StartProvisionalClick());
                }
                return _startProvisionalCommand;
            }
        }

        // Prompt user for management password before displaying voter details
        public void StartProvisionalClick()
        {
            // Open Password Dialog
            ValidationDialog passwordDialog = new ValidationDialog(AppSettings.User, "Manager");
            if (passwordDialog.ShowDialog() == true)
            {
                DisplayProvisionalVoter = true;
            }
            else
            {
                // Display error message
                AlertDialog wrongPassword = new AlertDialog("THE WRONG PASSWORD WAS ENTERED");
                wrongPassword.ShowDialog();
            }
        }
        #endregion
    }
}
