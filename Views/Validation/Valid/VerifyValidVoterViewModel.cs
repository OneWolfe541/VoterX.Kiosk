using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoterX.Core.Voters;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Controls;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Validation
{
    public class VerifyValidVoterViewModel : VerifyVoterBaseViewModel
    {
        // Model constructors
        public VerifyValidVoterViewModel(NMVoter voter) : this(voter, null) { }
        public VerifyValidVoterViewModel(NMVoter voter, VoterSearchModel SearchItems)
        {
            VoterItem = voter;
            _searchItems = SearchItems;

            SetDefaultQuestions();

            // Display Header
            StatusBar.PageHeader = "Voter Verification";
        }

        // Alternate constructor uses the Voter Navigation Model
        public VerifyValidVoterViewModel(VoterNavModel voterNav) : this(voterNav.Voter, voterNav.Search) { }

        #region QuestionText
        // QUESTIONS TEXT
        public string NameQuestion { get; set; }
        public string AddressQuestion { get; set; }
        public string DateQuestion { get; set; }
        public string CheckVoterInfoMessage { get; set; }

        public string IdQuestion { get; set; }
        public string InnerIdQuestion { get; set; }

        private void SetDefaultQuestions()
        {
            CheckVoterInfoMessage = "PLEASE VERIFY THE VOTER'S INFORMATION BY CHECKING THE BOXES";
            NameQuestion = "CONFIRM THE VOTER'S FULL NAME";
            AddressQuestion = "CONFIRM THE VOTER'S ADDRESS";
            DateQuestion = "CONFIRM THE VOTER'S BIRTH YEAR";
            IdQuestion = "VOTER MUST PRESENT A VALID ID";
            InnerIdQuestion = "Did the voter present valid identification?";
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
            //_parent.Navigate(new VoterSearchPage(_parent, _searchItems));
            NavigationMenuMethods.VoterSearchPage(_searchItems);
        }

        // Bound command for returning to the search screen
        public RelayCommand _goSignatureCommand;
        public ICommand GoSignatureCommand
        {
            get
            {
                if (_goSignatureCommand == null)
                {
                    _goSignatureCommand = new RelayCommand(param => this.CaptureSignatureClick());
                }
                return _goSignatureCommand;
            }
        }

        // Force parent frame to navigate back to the search page
        public void CaptureSignatureClick()
        {
            //_parent.Navigate(new SignatureCapturePage(_parent, (NMVoter)VoterItem));
            NavigationMenuMethods.SignatureCapturePage(VoterItem);
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

        #region IdRequireQuestionControl
        public YesNoQuestionViewModel IdRequiredQuestion
        {
            get
            {
                YesNoQuestionViewModel IdRequireQuestion = new YesNoQuestionViewModel(InnerIdQuestion);
                IdRequireQuestion.PropertyChanged += OnIdRequiredPropertyChanged;
                return IdRequireQuestion;
            }
        }

        private void OnIdRequiredPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                //Code to respond to a change in the ViewModel
                Console.WriteLine(
                    "Id Given: " +
                    ((YesNoQuestionViewModel)sender).IsChecked.ToString());
                IDIsSelected = (bool)((YesNoQuestionViewModel)sender).IsChecked;
                ValidateVoter();
            }
        }
        #endregion
    }
}
