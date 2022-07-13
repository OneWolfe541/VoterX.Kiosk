using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoterX.Core.Voters;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Dialogs;
using VoterX.Utilities.Methods;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Validation
{
    public class VerifySampleVoterViewModel : VerifyVoterBaseViewModel
    {
        // Model constructors
        public VerifySampleVoterViewModel(NMVoter voter) : this(voter, null) { }
        public VerifySampleVoterViewModel(NMVoter voter, VoterSearchModel SearchItems)
        {
            VoterItem = voter;
            _searchItems = SearchItems;

            VoterItem.Data.IDRequired = false;

            SetDefaultQuestions();

            // Display Header
            StatusBar.PageHeader = "Voter Verification";
        }

        // Alternate constructor uses the Voter Navigation Model
        public VerifySampleVoterViewModel(VoterNavModel voterNav) : this(voterNav.Voter, voterNav.Search) { }

        #region QuestionText
        // QUESTIONS TEXT
        public string NameQuestion { get; set; }
        public string AddressQuestion { get; set; }
        public string DateQuestion { get; set; }
        public string CheckVoterInfoMessage { get; set; }

        private void SetDefaultQuestions()
        {
            CheckVoterInfoMessage = "PLEASE VERIFY THE VOTER'S INFORMATION BY CHECKING THE BOXES";
            NameQuestion = "CONFIRM THE VOTER'S FULL NAME";
            AddressQuestion = "CONFIRM THE VOTER'S ADDRESS";
            DateQuestion = "CONFIRM THE VOTER'S BIRTH YEAR";
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

        // Bound command for printing the sample ballot
        public RelayCommand _printBallotCommand;
        public ICommand PrintBallotCommand
        {
            get
            {
                if (_printBallotCommand == null)
                {
                    _printBallotCommand = new RelayCommand(param => this.PrintSampleBallotClick());
                }
                return _printBallotCommand;
            }
        }

        // Print the sample ballot
        public async void PrintSampleBallotClick()
        {
            // Check printer status
            if (await PrinterStatus.PrinterIsReadyAsync(AppSettings.Printers.SamplePrinter) == true)
            {
                // Print the ballot
                StatusBar.TextLeft = await Task.Run(() => BallotPrinting.PrintSampleBallot(AppSettings.Global, VoterItem.Data.BallotStyleFile));

                // Return to search
                NavigationMenuMethods.VoterSearchPage(_searchItems);
            }
            else
            {
                // Display printer message
                AlertDialog signatureDialog = new AlertDialog("THE PRINTER IS NOT READY");
                signatureDialog.ShowDialog();
            }
        }
        #endregion
    }
}
