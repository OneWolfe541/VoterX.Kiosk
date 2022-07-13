using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoterX.Core.Voters;
using VoterX.SystemSettings.Enums;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Methods;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Validation
{
    public class VerifySpoiledVoterViewModel : VerifyVoterBaseViewModel
    {
        // Model constructors
        public VerifySpoiledVoterViewModel(NMVoter voter) : this(voter, null) { }
        public VerifySpoiledVoterViewModel(NMVoter voter, VoterSearchModel SearchItems)
        {
            VoterItem = voter;
            _searchItems = SearchItems;

            VoterItem.Data.IDRequired = false;

            SetDefaultQuestions();
            SetDefaultMessage();

            SetReprintVisibility(IsElectionDay);

            // Display Header
            StatusBar.PageHeader = "Voter Verification";
        }

        // Alternate constructor uses the Voter Navigation Model
        public VerifySpoiledVoterViewModel(VoterNavModel voterNav) : this(voterNav.Voter, voterNav.Search) { }

        #region QuestionText
        // QUESTIONS TEXT
        public string NameQuestion { get; set; }
        public string AddressQuestion { get; set; }
        public string DateQuestion { get; set; }
        public string CheckVoterInfoMessage { get; set; }

        // ID Question never used on spoiled ballots
        //public string IdQuestion { get; set; }
        //public string InnerIdQuestion { get; set; }

        public string SpoilMessage { get; set; }
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
            SpoilMessage = "THIS VOTER HAS ALREADY BEEN ISSUED A BALLOT AT THIS SITE";

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

        // Bound command for navigating to the Spoiled Ballot Screen
        private RelayCommand _spoilBallotCommand;
        public ICommand SpoilBallotCommand
        {
            get
            {
                if (_spoilBallotCommand == null)
                {
                    _spoilBallotCommand = new RelayCommand(param => this.SpoilBallotClick());
                }
                return _spoilBallotCommand;
            }
        }

        // Force parent frame to navigate to the spoil ballot page
        private void SpoilBallotClick()
        {
            //_parent.Navigate(new SpoilBallotPage(_parent));
            NavigationMenuMethods.SpoilOfficialBallotPage(VoterItem);
        }

        // Bound command for reprinting the voters application
        private RelayCommand _reprintApplicationCommand;
        public ICommand ReprintApplicationCommand
        {
            get
            {
                if (_reprintApplicationCommand == null)
                {
                    _reprintApplicationCommand = new RelayCommand(param => this.ReprintApplicationClick());
                }
                return _reprintApplicationCommand;
            }
        }

        // Reprint the voters Application
        private async void ReprintApplicationClick()
        {
            //_parent.Navigate(new SpoilBallotPage(_parent));
            var errorMessage = await Task.Run(() => BallotPrinting.ReprintApplication(VoterItem.Data, AppSettings.Global));
        }

        // Bound command for reprinting the voters permit
        private RelayCommand _reprintPermitCommand;
        public ICommand ReprintPermitCommand
        {
            get
            {
                if (_reprintPermitCommand == null)
                {
                    _reprintPermitCommand = new RelayCommand(param => this.ReprintPermitClick());
                }
                return _reprintPermitCommand;
            }
        }

        // Reprint the voters Permit
        private async void ReprintPermitClick()
        {
            //_parent.Navigate(new SpoilBallotPage(_parent));
            var errorMessage = await Task.Run(() => BallotPrinting.ReprintPermit(VoterItem.Data, AppSettings.Global));
        }

        // Bound command for reprinting the ballot stub
        private RelayCommand _reprintStubCommand;
        public ICommand ReprintStubCommand
        {
            get
            {
                if (_reprintStubCommand == null)
                {
                    _reprintStubCommand = new RelayCommand(param => this.ReprintStubClick());
                }
                return _reprintStubCommand;
            }
        }

        // Reprint the ballot stub
        private async void ReprintStubClick()
        {
            var errorMessage = await Task.Run(() => BallotPrinting.ReprintStub(VoterItem.Data, AppSettings.Global));
        }

        // Bound command for reprinting the absentee affidavit
        private RelayCommand _reprintAffidavitCommand;
        public ICommand ReprintAffidavitCommand
        {
            get
            {
                if (_reprintAffidavitCommand == null)
                {
                    _reprintAffidavitCommand = new RelayCommand(param => this.ReprintAffidavitClick());
                }
                return _reprintAffidavitCommand;
            }
        }

        // Reprint the absentee affidavit
        private async void ReprintAffidavitClick()
        {
            var errorMessage = await Task.Run(() => BallotPrinting.PrintAffidavit(VoterItem.Data, AppSettings.Global));
        }

        // Bound command for reprinting the absentee affidavit
        private RelayCommand _reprintSignatureCommand;
        public ICommand ReprintSignatureCommand
        {
            get
            {
                if (_reprintSignatureCommand == null)
                {
                    _reprintSignatureCommand = new RelayCommand(param => this.ReprintSignatureClick());
                }
                return _reprintSignatureCommand;
            }
        }

        // Reprint the absentee affidavit
        private async void ReprintSignatureClick()
        {
            var errorMessage = await Task.Run(() => BallotPrinting.PrintSignatureForm(VoterItem.Data, AppSettings.Global));
        }
        #endregion

        #region ReprintButtons
        private bool _applicationVisible;
        public bool ApplicationVisible
        {
            get
            {
                return _applicationVisible;
            }
            private set
            {
                _applicationVisible = value;
                RaisePropertyChanged("ApplicationVisible");
            }
        }

        private bool _permitVisible;
        public bool PermitVisible
        {
            get
            {
                return _permitVisible;
            }
            private set
            {
                _permitVisible = value;
                RaisePropertyChanged("PermitVisible");
            }
        }

        private bool _stubVisible;
        public bool StubVisible
        {
            get
            {
                return _stubVisible;
            }
            private set
            {
                _stubVisible = value;
                RaisePropertyChanged("PermitVisible");
            }
        }

        private bool _signatureVisible;
        public bool SignatureVisible
        {
            get
            {
                return _signatureVisible;
            }
            private set
            {
                _signatureVisible = value;
                RaisePropertyChanged("SignatureVisible");
            }
        }

        private bool IsElectionDay
        {
            get
            {
                if (AppSettings.System.VCCType == VotingCenterMode.ElectionDay) return true;
                else return false;
            }
        }

        private void SetReprintVisibility(bool IsElectionDay)
        {
            ApplicationVisible = !IsElectionDay;
            PermitVisible = IsElectionDay;

            if (IsElectionDay == true && AppSettings.System.BallotStub == 1)
            {
                StubVisible = true;
            }
            else
            {
                StubVisible = false;
            }

            if (IsElectionDay == true)
            {
                SignatureVisible = VoterItem.Data.SignRefused??false;
            }
            else
            {
                SignatureVisible = false;
            }
        }
        #endregion
    }
}
