using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class VerifyProvisionalVoterViewModel : VerifyVoterBaseViewModel
    {
        // Model constructors
        public VerifyProvisionalVoterViewModel(NMVoter voter) : this(voter, null) { }
        public VerifyProvisionalVoterViewModel(NMVoter voter, VoterSearchModel SearchItems)
        {
            VoterItem = voter;
            _searchItems = SearchItems;

            VoterItem.Data.IDRequired = false;

            SetDefaultQuestions();
            SetDefaultMessage();

            //GlobalReferences.StatusBar.TextLeft = VoterItem.Data.VoterID;

            // Display Header
            StatusBar.PageHeader = "Voter Verification";
        }

        // Alternate constructor uses the Voter Navigation Model
        public VerifyProvisionalVoterViewModel(VoterNavModel voterNav) : this(voterNav.Voter, voterNav.Search) { }

        #region QuestionText
        // QUESTIONS TEXT
        public string NameQuestion { get; set; }
        public string AddressQuestion { get; set; }
        public string DateQuestion { get; set; }
        public string CheckVoterInfoMessage { get; set; }

        public string ProvisionalMessage { get; set; }
        public string PreviousSite { get; set; }
        public string PreviousDateTime { get; set; }
        public string PreviousComputer { get; set; }

        private void SetDefaultQuestions()
        {
            CheckVoterInfoMessage = "PLEASE VERIFY THE VOTER'S INFORMATION BY CHECKING THE BOXES";
            NameQuestion = "CONFIRM THE VOTER'S FULL NAME";
            AddressQuestion = "CONFIRM THE VOTER'S ADDRESS";
            DateQuestion = "CONFIRM THE VOTER'S BIRTH YEAR";
        }

        private void SetDefaultMessage()
        {
            ProvisionalMessage = "THIS VOTER HAS ALREADY BEEN ISSUED A BALLOT IN THIS ELECTION";

            PreviousSite = VoterItem.Data.PollName;
            PreviousDateTime = VoterItem.Data.ActivityDate.ToString();
            PreviousComputer = VoterItem.Data.ComputerID.ToString();
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

        // Bound command for navigating to the void ballot screen
        private RelayCommand _voidBallotCommand;
        public ICommand VoidBallotCommand
        {
            get
            {
                if (_voidBallotCommand == null)
                {
                    _voidBallotCommand = new RelayCommand(param => this.VoidBallotClick());
                }
                return _voidBallotCommand;
            }
        }

        // Force parent frame to navigate to the void ballot page
        public void VoidBallotClick()
        {
            //_parent.Navigate(new VoterSearchPage(_parent, _searchItems));
            NavigationMenuMethods.VoidAbsenteeBallotPage(VoterItem);
        }
        #endregion

        #region ProvisionalLogic
        public bool VoidBallotVisibility
        {
            get
            {
                return VoterItem.Data.LogCode == 5 || VoterItem.Data.LogCode == 6 || VoterItem.Data.LogCode == 14 || VoterItem.Data.LogCode == 7 || VoterItem.Data.LogCode == 15;
            }
        }

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
