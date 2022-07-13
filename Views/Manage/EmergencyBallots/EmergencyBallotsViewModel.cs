using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoterX.Core.Elections;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Methods;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Manage
{
    public class EmergencyBallotsViewModel : NotifyPropertyChanged
    {
        public EmergencyBallotsViewModel()
        {
            SetDisplayMessages();

            // Initialize print button
            CanPrintBallot = true;

            // Display page header
            StatusBar.PageHeader = ("Emergency Ballots");
        }

        #region DisplayText
        public string ProvisionalBallotMessage { get; set; }
        public string SelectStyleMessage { get; set; }
        public string SelectReasonMessage { get; set; }

        private void SetDisplayMessages()
        {
            ProvisionalBallotMessage = "PRINT PROVISIONAL BALLOT FOR CURRENT VOTER";
            SelectStyleMessage = "SELECT A BALLOT STYLE";
            SelectReasonMessage = "SELECT A PROVISIONAL REASON";
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
            MainMenuMethods.OpenMenu(); // THIS COMMAND SHOULD HAPPEN ELSEWHERE!

            //_parent.Navigate(new VoterSearchPage(_parent));
            NavigationMenuMethods.ReturnToOrigin();
        }
        #endregion

        #region PrintBallot
        // Bound command for printing an official ballot
        public RelayCommand _printBallotCommand;
        public ICommand PrintBallotCommand
        {
            get
            {
                if (_printBallotCommand == null)
                {
                    _printBallotCommand = new RelayCommand(param => this.PrintBallotClick(), param => this.CanPrintBallot);
                }
                return _printBallotCommand;
            }
        }

        // Enable or Disable the Print Ballot Button
        private bool _canPrintBallot;
        public bool CanPrintBallot
        {
            get { return _canPrintBallot; }
            internal set
            {
                _canPrintBallot = value;
                RaisePropertyChanged("CanPrintBallot");
            }
        }

        private async void PrintBallotClick()
        {
            // Disable print button
            CanPrintBallot = false;

            // Print new Ballot
            var errorMessage = await Task.Run(() => BallotPrinting.PrintEmergencyBallot(AppSettings.Global, SelectedBallotStyleItem.BallotStyleFileName));

            if (errorMessage != null && errorMessage != "")
            {
                // Display Error Message
                StatusBar.TextCenter = errorMessage;
            }

            CanPrintBallot = true;
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
                    _ballotStyleList = ElectionDataMethods.DistinctBallots.OrderBy(o => o.BallotStyleName).ToList();
                }
                return _ballotStyleList;
            }
        }

        private BallotStyleModel _selectedBallotStyleItem;
        public BallotStyleModel SelectedBallotStyleItem
        {
            get
            {
                if (BallotStyleList != null && _selectedBallotStyleItem == null)
                {
                    _selectedBallotStyleItem = BallotStyleList
                        .FirstOrDefault();
                }
                if (_selectedBallotStyleItem == null)
                {
                    _selectedBallotStyleItem = new BallotStyleModel();
                }
                return _selectedBallotStyleItem;
            }
            set
            {
                _selectedBallotStyleItem = value;
            }
        }
        #endregion
    }
}
