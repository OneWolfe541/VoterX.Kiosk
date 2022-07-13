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
    public class VerifyDeletedVoterViewModel : VerifyVoterBaseViewModel
    {
        public VerifyDeletedVoterViewModel(NMVoter voter) : this(voter, null) { }
        public VerifyDeletedVoterViewModel(NMVoter voter, VoterSearchModel SearchItems)
        {
            VoterItem = voter;
            _searchItems = SearchItems;

            VoterItem.Data.IDRequired = false;

            SetDefaultQuestions();
            SetDefaultMessage();

            // Display Header
            StatusBar.PageHeader = "Voter Verification";
        }

        #region QuestionText
        // QUESTIONS TEXT
        public string NameQuestion { get; set; }
        public string AddressQuestion { get; set; }
        public string DateQuestion { get; set; }
        public string CheckVoterInfoMessage { get; set; }

        public string DeletedMessage { get; set; }

        private void SetDefaultQuestions()
        {
            CheckVoterInfoMessage = "PLEASE VERIFY THE VOTER'S INFORMATION BY CHECKING THE BOXES";
            NameQuestion = "CONFIRM THE VOTER'S FULL NAME";
            AddressQuestion = "CONFIRM THE VOTER'S ADDRESS";
            DateQuestion = "CONFIRM THE VOTER'S BIRTH YEAR";
        }

        private void SetDefaultMessage()
        {
            DeletedMessage = "THIS VOTER HAS BEEN DELETED OR REMOVED";
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
        #endregion
    }
}
